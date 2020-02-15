using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
	[SerializeField] private Text text;

	float time;

	// Update is called once per frame
	void Update()
	{
		time += Time.deltaTime / 100;
		float newTile = time;
		string s = newTile.ToString("###00.00");
		text.text = s.Replace('.', ':');
	}
}
