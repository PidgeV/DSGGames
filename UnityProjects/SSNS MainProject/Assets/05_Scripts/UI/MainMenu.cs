using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
	public override void InitializeMenu() { }
	public override void PlayTransition() { }
	public override void UpdateMenu() { }

	/// <summary>
	///  When the Quit Button is pressed
	/// </summary>
	public void QuitGame()
	{
		Application.Quit();
	}
}
