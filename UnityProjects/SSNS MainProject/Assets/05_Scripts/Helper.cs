using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
	public static void PrintTime(string s)
	{
		Debug.Log("[ " + (Time.realtimeSinceStartup).ToString("#####0.00") + "s ] " + s);
	}
}
