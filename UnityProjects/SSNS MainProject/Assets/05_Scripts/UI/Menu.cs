﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
	/// <summary> The Animation that will play when transitioning to this Menu </summary>
	[SerializeField] protected Animator transition;
	
	/// <summary> The Menu that directed us to this Menu </summary>
	Menu parentMenu;

	public abstract void InitializeMenu();
	public abstract void PlayTransition();
	public abstract void UpdateMenu();

	// Start is called before the first frame update
	void Start()
	{
		InitializeMenu();
	}
	// Update is called once per frame
	void Update()
	{
		UpdateMenu();
	}

	/// <summary> Transition to a different menu from this menu </summary>
	public virtual void OpenMenu(Menu newMenu)
	{
		newMenu.gameObject.SetActive(true);
		newMenu.parentMenu = this;
		newMenu.PlayTransition();

		CloseMenu();
	}
	public virtual void OpenMenu(Menu newMenu, bool updateParent = true)
	{
		newMenu.gameObject.SetActive(true);
		newMenu.PlayTransition();

		if (updateParent == true) {
			newMenu.parentMenu = this;
		}

		CloseMenu();
	}

	/// <summary> Handle closing the current Menu </summary> 
	public virtual void CloseMenu()
	{
		gameObject.SetActive(false);
	}

	/// <summary> Return to the parent Menu </summary>
	public virtual void ReturnToLastMenu()
	{
		OpenMenu(parentMenu, false);
	}
}
