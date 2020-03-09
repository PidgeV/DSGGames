using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnovaShieldState : FSMState
{
    private DreadnovaController controller;

    private bool shieldGone;

    private float shieldDissolve;

    public DreadnovaShieldState(DreadnovaController enemyController)
    {
        controller = enemyController;
        stateID = FSMStateID.Defend;

        EnterStateInit();
    }

    public override void EnterStateInit()
    {
        shieldGone = false;
        shieldDissolve = 1;
        controller.dreadnovaShield.SetActive(true);

        foreach (ShieldGenerator generator in controller.Generators)
        {
            generator.InitializeGenerator();
        }

        foreach (Transform child in controller.dreadnovaModel.transform)
        {
            if (child.TryGetComponent(out Collider collider))
            {
                collider.enabled = false;
            }
        }
    }

    public override void Reason()
    {
        if (IsGeneratorsDead())
        {
            if (shieldGone)
            {
                controller.dreadnovaShield.SetActive(false);

                foreach (Transform child in controller.dreadnovaModel.transform)
                {
                    if (child.TryGetComponent(out Collider collider))
                    {
                        collider.enabled = true;
                    }
                }

                if (controller.State == SNSSTypes.DreadnovaState.SHIELD_STAGE)
                    controller.PerformTransition(Transition.NoShield);
                else
                    controller.PerformTransition(Transition.Attack);
            }
            else
            {
                shieldDissolve = Mathf.Max(shieldDissolve - 0.2f * Time.deltaTime, 0);

                if (shieldDissolve == 0)
                    shieldGone = true;
            }
        }
    }

    public override void Act()
    {
        foreach (Transform child in controller.dreadnovaShield.transform)
        {
            if (child.TryGetComponent(out MeshRenderer renderer))
            {
                renderer.material.SetFloat("_Dissolve", shieldDissolve);
            }
        }
    }

    private bool IsGeneratorsDead()
    {
        bool dead = true;
        foreach (ShieldGenerator generator in controller.Generators)
        {
            if (generator.IsAlive)
                dead = false;
        }

        return dead;
    }
}
