using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static Helper;

// NOTE -- Use the Tag "Menu" on all the parent items [Canvass] per menu

// The UI Manager controlles the games menus
// This is mainly controlled through the public ChangeMenu() function, allowing you to transition menus

/// <summary>
/// The Singleton to the UIManager Script
/// </summary>
public class UIManager : MonoBehaviour
{
	// The Singleton reference
	public  static UIManager Instance { get { return instance; } }
	private static UIManager instance;

	/// <summary> The first Menu to load on start </summary>
	public  Menu StartingMenu;
	private Menu currentMenu;

	private Menu lastMenu;

	/// <summary> The current Menu that is selected </summary>
	private SelectableUI currentlySelected;

	/// <summary> A Reference to the selector gameobject </summary>
	public Image Selector;

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
		ChangeMenu(StartingMenu);
	}

	/// <summary>
	///  When the Quit Button is pressed
	/// </summary>
	public void Quit()
	{
		// Application.Quit();
		Helper.PrintTime("You are quitting!");
	}
	
	/// <summary>
	///  When the Start Game Button is pressed
	/// </summary>
	public void StartGame()
	{
		Helper.PrintTime("You are going into game!");
	}

	/// <summary>
	/// Try to use what is selected
	/// </summary>
	public void Enter()
	{
		currentlySelected.Press();
	}

	/// <summary>
	/// Change the currently selected Menu
	/// </summary>
	/// <param name="newMenu">The new Menu to display</param>
	/// <param name="allowReturn">If the player can return to the last menu</param>
	public void ChangeMenu(Menu newMenu)
	{ 
		// Turn off the old menu
		if (currentMenu)
		{
			currentMenu.gameObject.SetActive(false);
		}

		// Set our last Menu 
		lastMenu = currentMenu;

		// Update the current menu to the new menu
		currentMenu = newMenu;

		// Turn on the new menu
		currentMenu.gameObject.SetActive(true);
		
		// Target the initial target when loading a new menu
		UpdateElement(currentMenu.StartingSelection);
	}
	
	/// <summary>
	/// Returns to the last menu
	/// </summary>
	public void ReturnToLastMenu()
	{
		if (lastMenu)
		{
			ChangeMenu(lastMenu);
			lastMenu = null;
		}
	}

	/// <summary>
	/// Handles a Menu change based on a given direction
	/// </summary>
	/// <param name="direction">The direction that was pressed</param>
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

	/// <summary>
	/// Re-Target a new Menu element
	/// </summary>
	/// <param name="newTarget">The direction to handle</param>
	public void UpdateElement(SelectableUI newTarget)
	{
		// This Method takes in a direction and tries to move the Selector to any
		// selectable menus  in that direction
		
		// First we check if the menu that we currently are at is null
		if (newTarget == null)
		{
			// If it is null, that means the direction we inputted leads to nothing

			// If what we are currently at is not active, that means we have just switched menus
			if (!currentlySelected.gameObject.activeSelf)
			{
				// So We Want to disable ourselves and do nothing
				Selector.gameObject.SetActive(false);
			}

			// If we are active and the input leads to nothing, that means we can't go there. So do nothing
			return;
		}

		// If the new target is NOT null

		// We want to make sure our Selector is active
		Selector.gameObject.SetActive(true);
		
		// We then set our current parent to the active Menu / Canvas
		Selector.transform.parent = newTarget.GetComponentInParent<Canvas>().gameObject.transform;

		// We want to move ourselves to the top of the siblings 
		Selector.transform.SetSiblingIndex(0);

		// Set ourselves to the new target
		currentlySelected = newTarget;

		// Finally we have to update our selector
		// We need to reposition it and resize it depending on what's selected

		// POSITION
		Selector.transform.position = newTarget.transform.position;

		// SIZE
		Vector2 newSize = newTarget.GetComponent<RectTransform>().sizeDelta;
		Vector2 boarder = Vector2.one * 15.0f;

		Selector.rectTransform.sizeDelta = newSize + boarder;
	}
}
