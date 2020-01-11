using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupMenu : MonoBehaviour
{
	/// <summary>
	/// Start or load a game
	/// </summary>
	public void StartGame()
	{
	}

	/// <summary>
	///  When the Return button is pressed
	/// </summary>
	public void ReturnToMainMenu(GameObject mainMenu)
	{
		UIManager.Instance.ChangeMenu(mainMenu, gameObject);
	}
}
