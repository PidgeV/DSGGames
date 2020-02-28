using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHUDHandler : MonoBehaviour
{
	private ShipController _shipController;
	private HealthAndShields _shipHealth;

	[HideInInspector] public Slider _slider_Health;
	[HideInInspector] public Slider _slider_Shield;
	[HideInInspector] public Slider _slider_Boost;

	[HideInInspector] public Image _healthImage;
	[HideInInspector] public Image _shieldImage;
	[HideInInspector] public Image _boostImage;

	private void Awake()
	{
		_shipController = GetComponent<ShipController>();

		_slider_Health = GameObject.Find("[Slider] Health").GetComponent<Slider>();
		_slider_Shield = GameObject.Find("[Slider] Shield").GetComponent<Slider>();
		_slider_Boost  = GameObject.Find("[Slider] Boost").GetComponent<Slider>();

		_healthImage = _slider_Health.gameObject.GetComponentInChildren<Image>();
		_shieldImage = _slider_Shield.gameObject.GetComponentInChildren<Image>();
		_boostImage  = _slider_Boost.gameObject.GetComponentInChildren<Image>();

		_shipHealth = GetComponent<HealthAndShields>();
	}

	private void Update()
	{
		UpdateBoostGauge();

		UpdateHealthAndShields();
	}

	public void UpdateBoostGauge()
	{
		// Boost Gauge
		if (_shipController.BoostGauge < _shipController.myStats.maxBoostGauge && _shipController.Boosting == false)
		{
			float rechargeRate = 1.5f;
			
			_shipController.BoostGauge += rechargeRate * Time.deltaTime;
			
			if (_shipController.BoostGauge > _shipController.myStats.maxBoostGauge) {
				_shipController.BoostGauge = _shipController.myStats.maxBoostGauge;
			}

			// Boost Slider
			_slider_Boost.value = (1 / _shipController.myStats.maxBoostGauge) * _shipController.BoostGauge;

			// Set the color of the boost slider
			_boostImage.color = Color.Lerp(Color.red, Color.yellow, (1 / _shipController.myStats.maxBoostGauge) * _shipController.BoostGauge);
		}
	}

	public void UpdateHealthAndShields()
	{
		_slider_Health.value = (1 / _shipHealth.MaxLife) * _shipHealth.currentLife;
		_slider_Shield.value = (1 / _shipHealth.MaxShield) * _shipHealth.currentShield;
	}

}
