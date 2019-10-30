using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuReturn : MonoBehaviour
{
	public enum MenuReturnType { DEFAULT, NONE, NO_RETURN, KEEP_LAST_OPEN }

	public MenuReturnType returnType = MenuReturnType.DEFAULT;

	// The menu to return to
	public Menu targetMenu;

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

			default:break;
		}

	}
}
