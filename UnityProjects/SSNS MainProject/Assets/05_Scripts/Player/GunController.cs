using SNSSTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GunController : MonoBehaviour
{
	[SerializeField] private Animator gunAnimator;
	[SerializeField] private Animator swapAnimator;

	public GameObject standardShot;
	public GameObject energyShot;
	public GameObject missileShot;
	public GameObject chargedShot;

	public LaserBehaviour laserShot;

	[SerializeField] private Transform barrelL;
	[SerializeField] private Transform barrelR;

	[SerializeField] private ShieldProjector shieldProjector;

	public AmmoCounter ammoCount;

	private Transform _chargeShotTransform;

	[SerializeField] private ShipController _shipController;

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

			foreach (RaycastHit hit in hits) {
				if (hit.collider.TryGetComponent(out HealthAndShields hp))
				{
					hp.TakeDamage(laserShot.Damage / 3, laserShot.Damage);
				}
			}

			ammoCount.Take1Ammo(currentWeapon);
			CheckAmmo();
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
		InitShot(Instantiate(standardShot, _shipController.ShipModel.position + _shipController.ShipModel.forward * 10, _shipController.ShipModel.rotation));
	}

	public void ResetSpeed()
	{
		gunAnimator.speed = 1;
	}

	public void CheckAmmo()
	{
		if (ammoCount.HasAmmo(currentWeapon) == false) {
			_shipController.SwapWeapon(new Vector2(0, -1));
			UpdateAttacking(false);
		}
	}

	// Standard Shot
	public float FireRateStandard = 5f;
	public float FireRateEnergy = 5f;
	public void UpdateStandardShot()
	{
		gunAnimator.speed = currentWeapon == WeaponType.Regular ? FireRateStandard : FireRateEnergy;
	}

	// Missile Shot
	public float FireRateMissile = 1f;
	public void UpdateMissileShot()
	{
		gunAnimator.speed = FireRateMissile;
	}

	// Charge Shot
	public float FireRateCharge = 2f;
	public void UpdateChargeShot()
	{
		gunAnimator.speed = FireRateCharge;
	}

	// Charge Shot
	public float FireRateLaser = 0.2f;
	public void UpdateLaserShot()
	{
		gunAnimator.speed = FireRateLaser;
	}

	public void ShootStandard()
	{
		// TODO -- Play Sound
		// ammoCount.Take1Ammo(currentWeapon);
		GameObject shotPrefab = currentWeapon == WeaponType.Regular ? standardShot : energyShot;
		InitShot(Instantiate(shotPrefab, barrelL.position, barrelL.rotation));
	}

	public void ShootMissileL()
	{
		// TODO -- Play Sound
		ammoCount.Take1Ammo(currentWeapon);
		InitShot(Instantiate(missileShot, barrelL.position, barrelL.rotation));
		CheckAmmo();
	}

	public void ShootMissileR()
	{
		// TODO -- Play Sound
		ammoCount.Take1Ammo(currentWeapon);
		InitShot(Instantiate(missileShot, barrelR.position, barrelR.rotation));
		CheckAmmo();
	}

	public void StartCharge()
	{
		// TODO -- Play Sound
		GameObject newChargeShot = Instantiate(chargedShot, barrelL.position, barrelL.rotation, barrelL.transform);
		InitShot(newChargeShot);

		_chargeShotTransform = newChargeShot.transform;
	}

	public void EndCharge()
	{
		// TODO -- Stop Sound
		if (_chargeShotTransform != null)
		{
			ChargedShotBehaviour behaviour = _chargeShotTransform.GetComponent<ChargedShotBehaviour>();
			behaviour.HasShot = true;

			_chargeShotTransform = null;

			ammoCount.Take1Ammo(currentWeapon);
			CheckAmmo();
		}
	}

	public void StartLaser()
	{
		// TODO -- Play Sound
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
		// TODO -- Stop Sound
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
		else
		{
			if (currentWeapon == WeaponType.Energy) {
				gunAnimator.SetBool("Energy", true);
			}

			if (currentWeapon == WeaponType.Regular) {
				gunAnimator.SetBool("Energy", true);
			}
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
