using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    private readonly int maxAttack = 10;
    private int curAttack = 0;

    public static AIManager aiManager;

    int MaxAttack { get { return maxAttack; } }// set { maxAttack = value; } }
    int CurAttack { get { return curAttack; } set { curAttack = value; } }

    /// <summary>
    /// For if only 1 is attacking
    /// </summary>
    //bool CanAttack
    //{
    //    get
    //    {
    //        bool willAttack = aiManager.CurAttack < aiManager.MaxAttack;
    //        if (willAttack) aiManager.CurAttack++;
    //        return willAttack;
    //    }
    //}

    /// <summary>
    /// For if n enemies are attacking
    /// </summary>
    public bool CanAttack(int numAttacking = 1)
    {
        bool willAttack = aiManager.CurAttack + numAttacking <= aiManager.MaxAttack;
        if (willAttack) aiManager.CurAttack += numAttacking;
        return willAttack;
    }

    /// <summary>
    /// For if n enemies are attacking
    /// </summary>
    public void StopAttack(int numStopAttacking = 1)
    {
        aiManager.CurAttack -= numStopAttacking;
        if(aiManager.CurAttack < 0)
        {
            Debug.LogError("Y'all fudged up.  AICount < 0");
            aiManager.CurAttack = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
