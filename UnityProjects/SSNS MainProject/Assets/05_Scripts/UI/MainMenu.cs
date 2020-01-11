using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	/// <summary>
	///  When the Start Game button is pressed
	/// </summary>
	public void OpenSetupGame(GameObject setupMenu)
	{
		UIManager.Instance.ChangeMenu(setupMenu, gameObject);
	}

	/// <summary>
	///  When the Options button is pressed
	/// </summary>
	public void OpenOptions(GameObject optionsMenu)
	{
		UIManager.Instance.ChangeMenu(optionsMenu, gameObject);
	}

	/// <summary>
	///  When the Quit Button is pressed
	/// </summary>
	public void QuitGame()
	{
		Application.Quit();
	}
}
