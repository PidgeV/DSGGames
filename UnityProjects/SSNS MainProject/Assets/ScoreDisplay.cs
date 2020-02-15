using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
	[SerializeField] private Animator animator;
	[SerializeField] private Text text;

	private void Start()
	{
		StartCoroutine(AddToScore());
	}

	private void Update()
	{

	}

	public void AddScore(int score)
	{
		int currentScore = int.Parse(text.text);
		int newScore = currentScore + score;

		text.text = newScore.ToString();

		animator.Play("AddScore");
	}

	IEnumerator AddToScore()
	{
		while (true)
		{
			Debug.Log("dfs");
			AddScore(Random.Range(0, 5));
			yield return new WaitForSeconds(0.1f);
		}
	}
}
