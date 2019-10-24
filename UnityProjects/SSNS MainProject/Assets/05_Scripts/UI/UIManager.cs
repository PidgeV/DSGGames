using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static Helper;

// NOTE -- Use the Tag "Menu" on all the parent items [Canvass] per menu

// The UI Manager controlles the game menus
// This is mainly controlled through the public ChangeMenu() function allowing you to transition menus
public class UIManager : MonoBehaviour
{
	// Singleton
	public  static UIManager Instance { get { return instance; } }
	private static UIManager instance;

	// The current menu
	private Menu currentMenu;
	public  Menu startingMenu;

	// current seelection
	private SelectableUI currentlySelected;

	// This should be a pnl
	public Image selector;

	// Setting up the Singleton
	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			instance = this;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		// Disable each menu with the tag "Menu"
		foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Menu"))
		{
			gameObject.SetActive(false);
		}

		// Change to the starting menu
		ChangeMenu(startingMenu);
	}

	// Change the menu and keep the last menu
	public void ChangeMenu(Menu newMenu)
	{
		if (currentMenu)
		{
			currentMenu.gameObject.SetActive(false);
		}

		currentMenu = newMenu;
		currentMenu.gameObject.SetActive(true);

		// Change to the starting selected element
		UpdateElement(currentMenu.startingSelection);
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

	// Move the selected Menu
	public void TransitionElement(eMenuDirection direction)
	{
		switch (direction)
		{
			// LEFT
			case eMenuDirection.LEFT:
				UpdateElement(currentlySelected.TransitionLeft);
				break;

			// RIGHT
			case eMenuDirection.RIGHT:
				UpdateElement(currentlySelected.TransitionRight);
				break;

			// UP
			case eMenuDirection.UP:
				UpdateElement(currentlySelected.TransitionUp);
				break;

			// DOWN
			case eMenuDirection.DOWN:
				UpdateElement(currentlySelected.TransitionDown);
				break;

			default: break;
		}
	}

	//
	public void UpdateElement(SelectableUI newTarget)
	{
		// If the new target is null we don't do anything
		if (newTarget == null)
		{
			if (!currentlySelected.gameObject.activeSelf)
			{
				selector.gameObject.SetActive(false);
			}

			return;
		}

		selector.gameObject.SetActive(true);

		// Set the selector  to a child of the canvas in a menu
		selector.transform.parent = newTarget.GetComponentInParent<Canvas>().gameObject.transform;

		// Set the new target	
		currentlySelected = newTarget;

		// SET new POSITION
		selector.transform.position = newTarget.transform.position;

		// SET new SIZE
		Vector2 newSize = newTarget.GetComponent<RectTransform>().sizeDelta;
		Vector2 boarder = Vector2.one * 15.0f;

		selector.rectTransform.sizeDelta = newSize + boarder;
	}
}
