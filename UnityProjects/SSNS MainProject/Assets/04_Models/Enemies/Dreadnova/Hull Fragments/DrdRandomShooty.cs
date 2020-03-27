using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrdRandomShooty : MonoBehaviour
{
    public static DrdRandomShooty manager;
    GameObject[] AllShooty;
    int shot = 0;
   
    public void AddShoot()
    {
        shot++;
        

        if (shot % 2 == 0 && shot<AllShooty.Length)
        {
            bool hasDecided = false;
            int counter = 0;
            while (!hasDecided || counter <= 100)
            {
                int rand = Random.Range(0, AllShooty.Length);
                //if(AllShooty[rand].GetComponent<HealthAndShields>().currentLife != 0)
                if(!AllShooty[rand].activeInHierarchy)
                {
                    AllShooty[rand].SetActive(true);
                    AllShooty[rand].GetComponent<DrdFireArea>().SetGlowy();
                    hasDecided = true;
                }
                counter++;
            }
            hasDecided = false;
            counter = 0;
            while (!hasDecided || counter <= 100)
            {
                int rand = Random.Range(0, AllShooty.Length);
                //if(AllShooty[rand].GetComponent<HealthAndShields>().currentLife != 0)
                if (!AllShooty[rand].activeInHierarchy)
                {
                    AllShooty[rand].SetActive(true);
                    AllShooty[rand].GetComponent<DrdFireArea>().SetGlowy();
                    hasDecided = true;
                }
                counter++;
            }
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (manager ==null)
        {
            manager = this;
        }
    }
}
