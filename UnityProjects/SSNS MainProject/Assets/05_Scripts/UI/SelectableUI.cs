using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// The Selectable UI script holds what should be switched to, On input

/// <summary>
/// A selectable element in a Menu
/// </summary>
public class SelectableUI : MonoBehaviour
{
	private Button button;

	// Start is called before the first frame update
	private void Awake()
	{
		button = GetComponent<Button>();
	}

	/// <summary>
	/// Use the Button
	/// </summary>
	public void Press()
	{
		if (button)
		{
			button.onClick.Invoke();
		}
	}

	#region Transition

	/// <summary> What to change to when hitting LEFT </summary>
	public SelectableUI TransitionLeft;

	/// <summary> What to change to when hitting RIGHT </summary>
	public SelectableUI TransitionRight;

	/// <summary> What to change to when hitting UP </summary>
	public SelectableUI TransitionUp;

	/// <summary> What to change to when hitting DOWN </summary>
	public SelectableUI TransitionDown;

	#endregion
}
