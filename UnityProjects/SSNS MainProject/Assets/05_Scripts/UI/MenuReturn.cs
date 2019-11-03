using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Uses to determine the menu that should be returned to and how to behave
/// </summary>
public class MenuReturn : MonoBehaviour
{
	// Menu Return Type Enumeration
	public enum MenuReturnType
	{
		// Return to [target] menu
		DEFAULT,

		// Do NOT return to a menu
		NONE,

		// Do not let the player return to the last menu [ One way ]
		NO_RETURN,

		// Keep the last menu Open
		KEEP_LAST_OPEN
	}

	/// <summary> The Menu Return Type </summary>
	public MenuReturnType returnType = MenuReturnType.DEFAULT;

	/// <summary> The Menu  to change to </summary>
	public Menu targetMenu;

	/// <summary>
	///  Return to the last Menu
	/// </summary>
	/// <param name="manager">The Menu to change to</param>
	public void ReturnToLastMenu(UIManager manager)
	{
		switch (returnType)
		{
			// Default Change Menu
			case MenuReturnType.DEFAULT:
				manager.ChangeMenu(targetMenu);
				break;

			case MenuReturnType.NONE:
				break;

			case MenuReturnType.NO_RETURN:
				break;

			case MenuReturnType.KEEP_LAST_OPEN:
				break;

			default: break;
		}
	}
}
