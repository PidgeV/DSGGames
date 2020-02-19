using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FocusButton : MonoBehaviour
{
	// Start is called before the first frame update
	void OnEnable()
	{
		GetComponent<Button>().Select();
        Debug.Log("Called");
	}
}
