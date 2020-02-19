using SNSSTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
	[SerializeField] private Animator animator;
	[SerializeField] private Animator swapAnimator;

	private WeaponType currentWeapon;

	public bool Attacking = false;

	public bool CanAttack = true;
	public bool CanSwap = true;

	private bool inReturnAnimation = false;
	private bool inSwapAnimation = false;

	public void EndReturnAnimation() { inReturnAnimation = false; }
	public void EndSwapAnimation() { inSwapAnimation = false; }

	void Start()
	{
		animator = animator ? animator : GetComponent<Animator>();
		currentWeapon = WeaponType.Regular;
	}

	public void UpdateAttacking(bool state)
	{
		Attacking = state;
		animator.SetBool("Attacking", Attacking);
	}

	public void Shoot()
	{

	}

	public void SwapWeapon( WeaponType newWeapon)
	{
		// Do Nothing
		if (currentWeapon == newWeapon) return;

		if (CanSwap == true)
		{
			CanSwap = false;
			StartCoroutine(coSwapWeapon(newWeapon));
		}
	}

	private IEnumerator coSwapWeapon(WeaponType newWeapon)
	{        
		// Wait to stop attacking
		while (Attacking == true) { yield return null; }

		CanAttack = false;

		// Show the Swap UI
		swapAnimator.SetBool("ShowUI", true);

		// If we're NOT in the default state..
		// We need to change back to the default state first
		if (currentWeapon != WeaponType.Regular && 
			currentWeapon != WeaponType.Energy)
		{
			// Start the return animation
			inReturnAnimation = true;
			animator.SetTrigger("Reset");
			swapAnimator.SetTrigger("Return");

			// Wait for the return animation to end
			while (inReturnAnimation) { yield return null; }
		}

		// Now we can start the swap animation
		inSwapAnimation = true;

		// Call the appropriate  animation trigger
		switch (newWeapon)
		{
			case WeaponType.Laser:
				animator.SetTrigger("Change_Laser");
				swapAnimator.SetTrigger("Idle_Laser");
				break;

			case WeaponType.Charged:
				animator.SetTrigger("Change_Charged");
				swapAnimator.SetTrigger("Idle_Charge");
				break;

			case WeaponType.Missiles:
				animator.SetTrigger("Change_Rockets");
				swapAnimator.SetTrigger("Idle_Missile");
				break;

			default: inSwapAnimation = false; break;
		}

		// Wait for the swap animation to end
		while (inSwapAnimation) { yield return null; }

		// Hide the Swap UI
		swapAnimator.SetBool("ShowUI", false);

		CanAttack = true;
		CanSwap = true;

		currentWeapon = newWeapon;
	}
}