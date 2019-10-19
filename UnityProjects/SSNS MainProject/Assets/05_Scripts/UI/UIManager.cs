using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE -- Use the Tag "Menu" on all the parent items [Canvass] per menu

// The UI Manager controlles the game menus
// This is mainly controlled through the public ChangeMenu() function allowing you to transition menus
public class UIManager : MonoBehaviour
{
	// The current and last menu
	Menu lastMenu;
	Menu currentMenu;

	public Menu startingMenu;

	// Start is called before the first frame update
	void Start()
	{
		// Disable each menu with the tag "Menu"
		foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Menu"))
		{
			gameObject.SetActive(false);
		}

		// Change  to the starting menu
		ChangeMenu(startingMenu);
	}

	// Change the menu and keep the last menu
	public void ChangeMenu(Menu newMenu)
	{
		if (currentMenu)
		{
			lastMenu = currentMenu;
			currentMenu.gameObject.SetActive(false);
		}

		currentMenu = newMenu;
		currentMenu.gameObject.SetActive(true);
	}

	// Quit the game
	public void Quit()
	{
		// Application.Quit();
		Helper.PrintTime("You are quitting!");
	}

	// Start the game 
	public void StartGame()
	{
		Helper.PrintTime("You are going into game!");
	}
}
