using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	public void Resume()
	{
		GameManager.Instance.PauseGame();
	}

	public void Options()
	{

	}

	public void Quit()
	{
		SceneManager.LoadScene(0);
	}
}
