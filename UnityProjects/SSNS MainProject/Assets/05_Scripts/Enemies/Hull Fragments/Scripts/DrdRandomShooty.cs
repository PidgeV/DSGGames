using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrdRandomShooty : MonoBehaviour
{
    public static DrdRandomShooty manager;
    [SerializeField] GameObject[] AllShooty;
    [SerializeField] Text totalText;
    int shot = -1;  //Must be -1 so addshoot triggers first pass through
    public float curLife;
    public float CurLife { get { return curLife; } }
    // Start is called before the first frame update
    void Start()
    {
        if (manager == null)
        {
            manager = this;

            for (int i = 0; i < AllShooty.Length; i++)
            {
                AllShooty[i].SetActive(false);
            }

            AddShoot();
        }

        StartCoroutine(Count());
    }

    public void AddShoot()
    {
        shot++;
        

        if (shot % 2 == 0 && shot < AllShooty.Length)
        {
            bool hasDecided = false;
            int counter = 0;
            while (!hasDecided )//&& counter <= 100)
            {
                int rand = Random.Range(0, AllShooty.Length);
                //if(AllShooty[rand].GetComponent<HealthAndShields>().currentLife != 0)
                if(!AllShooty[rand].activeInHierarchy)
                {
                    AllShooty[rand].SetActive(true);
                    AllShooty[rand].GetComponent<DrdPieceRepulsion>().fireToOff.SetActive(true);
                    hasDecided = true;
                }
                counter++;
            }
            hasDecided = false;
            counter = 0;
            while (!hasDecided )//&& counter <= 100)
            {
                int rand = Random.Range(0, AllShooty.Length);
                //if(AllShooty[rand].GetComponent<HealthAndShields>().currentLife != 0)
                if (!AllShooty[rand].activeInHierarchy)
                {
                    AllShooty[rand].SetActive(true);
                    AllShooty[rand].GetComponent<DrdPieceRepulsion>().fireToOff.SetActive(true);
                    hasDecided = true;
                }
                counter++;
            }
        }
    }

    IEnumerator Count()
    {
        while (true)
        {
            curLife = 0;
            for (int i = 0; i < AllShooty.Length; i++)
            {
                curLife += AllShooty[i].GetComponent<HealthAndShields>().currentLife;
            }

            if(totalText != null)
                totalText.text = curLife.ToString();
            yield return new WaitForSeconds(1);
        }
    }
}
