using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
	/// <summary>
	///  When the Return button is pressed
	/// </summary>
	public void ReturnToMainMenu(GameObject mainMenu)
	{
		UIManager.Instance.ChangeMenu(mainMenu, gameObject);
	}
}
