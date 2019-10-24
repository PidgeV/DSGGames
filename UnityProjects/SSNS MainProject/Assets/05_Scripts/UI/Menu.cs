using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE -- This class is mainly used for clarity.

// The Menu class represents large Menus, as opposed to, The MenuElements class 
// witch represents a single element in a menu that the player can select

/// <summary>
/// A Game Menu
/// </summary>
public class Menu : MonoBehaviour
{
	/// <summary> The first element that should be selected on entering a new menu </summary>
	public SelectableUI StartingSelection;
}
