using SNSSTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GunController : MonoBehaviour
{
	[SerializeField] private Animator gunAnimator;
	[SerializeField] private Animator swapAnimator;

	[SerializeField] private GameObject standardShot;
	[SerializeField] private GameObject energyShot;
	[SerializeField] private GameObject missileShot;
	[SerializeField] private GameObject chargedShot;

	[SerializeField] private LaserBehaviour laserShot;

	[SerializeField] private Transform barrelL;
	[SerializeField] private Transform barrelR;

	[SerializeField] private ShieldProjector shieldProjector;

	public AmmoCounter ammoCount;

	private Transform _chargeShotTransform;

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
		gunAnimator = gunAnimator ? gunAnimator : GetComponent<Animator>();

		currentWeapon = WeaponType.Regular;
	}

	private void Update()
	{
		if (laserShot.fadeIn)
		{
			Ray ray = new Ray(barrelL.transform.position, barrelL.transform.forward);

			LayerMask enemyLayer = new LayerMask();
			enemyLayer += LayerMask.GetMask("Enemies");
			enemyLayer += LayerMask.GetMask("Swarm");

			RaycastHit[] hits = Physics.RaycastAll(ray, laserShot.Length, enemyLayer);

			foreach (RaycastHit hit in hits)
			{
				if (hit.collider.TryGetComponent(out HealthAndShields hp))
				{
					hp.TakeDamage(laserShot.Damage / 3, laserShot.Damage);
				}
			}
			ammoCount.Take1Ammo(currentWeapon);
		}	
	}

	public void UpdateAttacking(bool state)
	{
		Attacking = state;
		gunAnimator.SetBool("Attacking", Attacking);
	}

	// Shoot from Ship
	public void ShootFromShip()
	{
	}

	public void ResetSpeed()
	{
		gunAnimator.speed = 1;
	}

	// Standard Shot
	private float StandardShotFireRate = 5f;
	public void UpdateStandardShot()
	{
		gunAnimator.speed = StandardShotFireRate;
	}

	// Missile Shot
	private float MissileShotFireRate = 1f;
	public void UpdateMissileShot()
	{
		gunAnimator.speed = MissileShotFireRate;
	}

	// Charge Shot
	private float ChargeShotFireRate = 2f;
	public void UpdateChargeShot()
	{
		gunAnimator.speed = ChargeShotFireRate;
	}

	// Charge Shot
	private float LaserShotFireRate = 0.2f;
	public void UpdateLaserShot()
	{
		gunAnimator.speed = LaserShotFireRate;
	}

	public void ShootStandard() {
		ammoCount.Take1Ammo(currentWeapon);
		InitShot(Instantiate(standardShot, barrelL.position, barrelL.rotation));
	}

	public void ShootEnergy() {
		ammoCount.Take1Ammo(currentWeapon);
		InitShot(Instantiate(standardShot, barrelL.position, barrelL.rotation));
	}

	public void ShootMissileL() {
		ammoCount.Take1Ammo(currentWeapon);
		InitShot(Instantiate(missileShot, barrelL.position, barrelL.rotation));
	}

	public void ShootMissileR() {
		ammoCount.Take1Ammo(currentWeapon);
		InitShot(Instantiate(missileShot, barrelR.position, barrelR.rotation));
	}

	public void StartCharge()
	{
		GameObject newChargeShot = Instantiate(chargedShot, barrelL.position, barrelL.rotation, barrelL.transform);
		InitShot(newChargeShot);

		_chargeShotTransform = newChargeShot.transform;
	}

	public void EndCharge()
	{
		if (_chargeShotTransform != null)
		{
			ChargedShotBehaviour behaviour = _chargeShotTransform.GetComponent<ChargedShotBehaviour>();
			behaviour.HasShot = true;

			_chargeShotTransform = null;
		}
	}

	public void StartLaser()
	{
		LayerMask enemyLayer = new LayerMask();
		enemyLayer += LayerMask.GetMask("Enemies");
		enemyLayer += LayerMask.GetMask("Swarm");

		laserShot.fadeIn = true;

		RaycastHit[] hits = Physics.SphereCastAll(barrelL.transform.position, laserShot.radius, barrelL.transform.forward.normalized, laserShot.Length, enemyLayer);

		// The Initial cone in front of the laser
		foreach (RaycastHit hit in hits) {
			if (hit.collider.TryGetComponent(out HealthAndShields hp))
			{
				hp.TakeDamage(laserShot.Damage / 3, laserShot.Damage);
			}
		}
	}

	public void EndLaser()
	{
		laserShot.fadeIn = false;
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
			gunAnimator.SetTrigger("Reset");
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
				gunAnimator.SetTrigger("Change_Laser");
				swapAnimator.SetTrigger("Idle_Laser");
				break;

			case WeaponType.Charged:
				gunAnimator.SetTrigger("Change_Charged");
				swapAnimator.SetTrigger("Idle_Charge");
				break;

			case WeaponType.Missiles:
				gunAnimator.SetTrigger("Change_Rockets");
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

	public void InitShot(GameObject newShot)
	{
		if (newShot.TryGetComponent(out ShotThing shotThing)) {
			shotThing.whoSent = ShotThing.shotFrom.Player;
		}

		shieldProjector.IgnoreCollider(newShot.GetComponent<Collider>());
	}
}
