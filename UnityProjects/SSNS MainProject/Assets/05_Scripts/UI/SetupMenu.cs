using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupMenu : Menu
{
	public override void InitializeMenu() { }
	public override void UpdateMenu() { }

	public override void PlayTransition()
	{
		if (transition == null)
		{
			// Do nothing
			return;
		}
		else
		{
			transition.SetTrigger("TransitionToSetup");
		}
	}

	/// <summary>
	/// Start or load a game
	/// </summary>
	public void StartGame()
	{
		transition.SetTrigger("PlayGame");
	}
}
