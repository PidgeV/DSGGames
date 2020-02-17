using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicController : MonoBehaviour
{
	private Animator animator;
	private GameObject ship;

	// Start is called before the first frame update
	void Start()
	{
		ship = GameObject.FindGameObjectWithTag("Ship");
		animator = GetComponent<Animator>();
	}

	public void TransitionMainTo_Options()
	{
		animator.SetTrigger("TransitionToOptions");
	}

	public void TransitionMainTo_Setup()
	{
		animator.SetTrigger("TransitionToSetup");
	}

	public void TransitionOptionsTo_Main()
	{
		animator.SetTrigger("Exit");
	}

	public void TransitionSetupTo_Main()
	{
		animator.SetTrigger("Exit");
	}

	public void PlayGame()
	{
		animator.SetTrigger("PlayGame");
	}

	public void QuitGame()
	{

	}



	bool canSkip = true;
	public bool inTransition = false;


	public void StartTransition()
	{
		inTransition = true;
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
			TurnOffTrails();
			animator.Play("Intro_Animation", 0, 0.95f);
			animator.SetTrigger("TransitionToMain");
		}

		canSkip = false;
	}

	public void TurnOnTrails()
	{
		TrailRenderer[] trails = GameObject.FindObjectsOfType<TrailRenderer>();
		foreach (TrailRenderer trail in trails) { trail.emitting = true; }
	}
	public void TurnOffTrails()
	{
		TrailRenderer[] trails = GameObject.FindObjectsOfType<TrailRenderer>();
		foreach (TrailRenderer trail in trails) { trail.emitting = false; }
	}

	[SerializeField] private GameObject mainMenu;
	public void ShowMainMenu()
	{
		mainMenu.SetActive(true);
	}
	public void HideMainMenu()
	{
		mainMenu.SetActive(false);
	}
}
