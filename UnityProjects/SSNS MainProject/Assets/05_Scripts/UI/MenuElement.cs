using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The MenuElements Class represents elements in a menu the player can select

public class MenuElement : MonoBehaviour
{
	// The menu element that the player retargets on input
	// LEFT
	public MenuElement TransitionLeft;

	// RIGHT
	public MenuElement TransitionRight;

	// UP
	public MenuElement TransitionUp;

	// DOWN
	public MenuElement TransitionDown;

	public GameObject highlightBox;

	// Change the selected menu element
	public void TransitionElement(NoJobInputs controller, Helper.eMenuDirection direction)
	{
		switch (direction)
		{
			// LEFT
			case Helper.eMenuDirection.LEFT:
				UpdateElement(controller, TransitionLeft);
				break;

			// RIGHT
			case Helper.eMenuDirection.RIGHT:
				UpdateElement(controller, TransitionRight);
				break;

			// UP
			case Helper.eMenuDirection.UP:
				UpdateElement(controller, TransitionUp);
				break;

			// DOWN
			case Helper.eMenuDirection.DOWN:
				UpdateElement(controller, TransitionDown);
				break;

			default: break;
		}
	}

	public void UpdateElement(NoJobInputs controller, MenuElement newTarget)
	{
		// If the new target is null we don't do anything
		if (newTarget == null)
		{
			return;
		}

		// SET the new menu for the player
		controller.selectedMenu = newTarget;

		// You now have to highlight the menu somehow

		highlightBox.SetActive(false);
		newTarget.highlightBox.gameObject.SetActive(true);
	}
}
