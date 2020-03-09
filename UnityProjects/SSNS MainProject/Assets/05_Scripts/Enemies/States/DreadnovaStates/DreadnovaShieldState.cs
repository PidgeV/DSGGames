using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnovaShieldState : FSMState
{
    private DreadnovaController controller;

    private HealthAndShields[] generators;
    private int shieldAliveCount;

    private bool shieldGone;

    private float shieldDissolve;

    public DreadnovaShieldState(DreadnovaController enemyController, HealthAndShields[] shieldGenerators)
    {
        controller = enemyController;
        stateID = FSMStateID.Defend;
        generators = shieldGenerators;

        for (int i = 0; i < generators.Length; i++)
        {
            generators[i].onDeath += OnGeneratorDeath;
        }

        EnterStateInit();
    }

    public override void EnterStateInit()
    {
        shieldGone = false;
        shieldDissolve = 1;
        shieldAliveCount = generators.Length;
        controller.dreadnovaShield.SetActive(true);

        foreach (HealthAndShields generator in generators)
        {
            generator.ResetValues();
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
        if (shieldAliveCount <= 0)
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

    public void OnGeneratorDeath()
    {
        shieldAliveCount--;
    }

    ~DreadnovaShieldState()
    {
        shieldAliveCount = 0;

        for (int i = 0; i < generators.Length; i++)
        {
            generators[i].onDeath -= OnGeneratorDeath;
        }

        generators = null;
    }
}
