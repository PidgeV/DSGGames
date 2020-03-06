using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    private readonly int maxAttack = 10;
    private int curAttack = 0;

    public static AIManager aiManager;
    public enum AITypes { Swarmer, Charger, Fighter };
    readonly int[] countsPer1Point = { 6, 1, 2 };
    int[] countsOfAI = { 0, 0, 0 };

    int MaxAttack { get { return maxAttack; } } // set { maxAttack = value; } }
    int CurAttack { get { return curAttack; } set { curAttack = value; } }

    private int TestTotal(AITypes whichAI, int numOfAI)
    {
        int[] counts = countsOfAI;
        counts[(int)whichAI] += numOfAI;
        int total = 0;
        for (int i = 0; i < counts.Length; i++)
        {
            total += counts[i] / countsPer1Point[i];
        }

        return total;
    }

    private void SetTotal(AITypes whichAI, int numOfAI)
    {
        countsOfAI[(int)whichAI] += numOfAI;
        int total = 0;
        for (int i = 0; i < countsOfAI.Length; i++)
        {
            total += countsOfAI[i] / countsPer1Point[i];
        }

        aiManager.CurAttack = total;
    }


    /// <summary>
    /// For if n enemies are attacking
    /// </summary>
    public bool CanAttack(AITypes whichAI, int numOfAI=1)//(int numAttacking = 1)
    {
        int points = TestTotal(whichAI,numOfAI);
        bool willAttack = points <= aiManager.MaxAttack;
        if (willAttack) SetTotal(whichAI, numOfAI);
        return willAttack;
    }

    /// <summary>
    /// For if n enemies are attacking
    /// </summary>
    public void StopAttack(AITypes whichAI, int numOfAI=1)//(int numStopAttacking = 1)
    {

        SetTotal(whichAI, -numOfAI);
        //int points = numOfAI / countsPer1Point[(int)whichAI];
        //aiManager.CurAttack -= points;
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
