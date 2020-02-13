﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicController : MonoBehaviour
{	
	[SerializeField] private GameObject shipModel;
	[SerializeField] private GameObject firstMenu;

	bool canSkip = true;

	// Start is called before the first frame update
	void Start()
    {
	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetMouseButtonDown(0)) {
			EndIntroCinematic();
		}
    }
	
	public void EndIntroCinematic()
	{
		Animator animator = GetComponent<Animator>();

		if (animator && canSkip) {
			animator.Play("Intro_Animation", 0, 0.95f);
			animator.SetTrigger("TransitionToMain");
		}

		canSkip = false;
	}

	public void InitializeFirstMenus()
	{
		firstMenu.SetActive(true);
	}
}
