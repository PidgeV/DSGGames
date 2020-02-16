using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

public class ShieldProjector : MonoBehaviour
{
	// When the shield is hit
	public delegate void OnShieldHit(GameObject attacker);
	public OnShieldHit onShieldHit;

	// When the shield goes from any value to 0
	public delegate void OnShieldBreak();
	public OnShieldBreak onShieldBreak;

	// When the shield goes from 0 to any value
	public delegate void OnShieldRegen();
	public OnShieldRegen onShieldRegen;


	// Keeps material references from impact objects that need to be faded out
	private List<Material> objectsToFade = new List<Material>();


	[Header("Shield Renderer")]
	[SerializeField] MeshRenderer ShieldMeshRenderer;


	// The initial and final color of a shield
	[Header("Colors")]
	[ColorUsage(true, true)] public Color BaseColor;
	[ColorUsage(true, true)] public Color BrokenColor;

	[Header("Effects")]
	[SerializeField] GameObject DissolveVFX;
	[SerializeField] GameObject ImpactVFX;
	[SerializeField] GameObject HitVFX;


	// The current collider that is on this ship
	Collider shipCollider;
	SphereCollider shieldCollider;


	// The damage percent is the smount of shield remaining. 0 - 1
	 float DamagePercent = 1;

	[Header("Shield Properties")]
	public float ShieldSize = 1.0f;
	public bool ResizeShield = false;


	// Hows does the shield fade out
	[Header("Shiled Behaviour")]
	[SerializeField] FadeType fadeType = FadeType.NO_FADE;


	// Get the current color of the shield between the base color and the broken color
	public Color GetColor { get { return Color.Lerp(BrokenColor, BaseColor, DamagePercent); } }

	// Start is called before the first frame update
	void Start()
	{
		ShieldMeshRenderer.material.SetColor("_BaseColor", BaseColor);
		ShieldMeshRenderer.material.SetColor("_FresnelColor", BaseColor);

		//if (gameObject.TryGetComponent<Collider>(out shipCollider)) {
		//	shipCollider.enabled = false;
		//}

		shieldCollider = gameObject.AddComponent<SphereCollider>();
		shieldCollider.radius = ShieldSize;

		ShieldMeshRenderer.gameObject.transform.localScale *= ShieldSize;

        //Physics.IgnoreCollision(shipCollider, shieldCollider);
	}

	// Update is called once per frame
	void Update()
	{
		// The shader for the impact objects used object space, I believe that is why the object using the shader (the shield)
		// needs to not be rotated. This like makes sure the impact shader works
		ShieldMeshRenderer.gameObject.transform.rotation = Quaternion.identity;

		FadeShieldImpacts();
		FadeShieldShader();
	}

	// When the shield is hit
	private void OnCollisionEnter(Collision collision)
	{
		SpawnShieldImpact(collision);
		ResetShieldShader();

		if (onShieldHit != null)
		{
			onShieldHit.Invoke(collision.gameObject);
		}
	}

	/// <summary>
	/// Update the shield fill percent 
	/// </summary>
	/// <param name="current">The current shield value</param>
	/// <param name="max">The max shield value</param>
	public void UpdateShieldPercent(float current, float max)
	{
		float newPercent = 1 / max * current;

		// If our shield is at 0%
		if (newPercent <= 0 && DamagePercent > 0)
		{
			// Invoke the On Shield Break Event
			if (onShieldBreak != null)
			{
				onShieldBreak.Invoke();
			}

			if (fadeType == FadeType.HALF_FADE)
			{
				// HALF_FADE means we fade the shield out so that only a small ring of color can be seen
				ShieldMeshRenderer.material.SetColor("_BaseColor", Color.clear);
			}

			SpawnShieldDissolve();

			UpdateShieldCollider(false);
		}

		// If we currently have a 0% shield and our new percent is anything above 0%
		if (DamagePercent <= 0 && newPercent > DamagePercent)
		{
			// Invoke the On Shield Regen Event
			if (onShieldRegen != null)
			{
				onShieldRegen.Invoke();
			}

			if (fadeType == FadeType.HALF_FADE)
			{
				// HALF_FADE means we fade the shield out so that only a small ring of color can be seen
				ShieldMeshRenderer.material.SetColor("_BaseColor", GetColor);
			}

			UpdateShieldCollider(true);
		}

		// Update the damage percent
		DamagePercent = newPercent;
	}

	// Resize this shields hitbox depending on the shield damage percent
	void UpdateShieldCollider(bool shieldEnabled)
	{
		if (ResizeShield && shipCollider != null)
		{
			//shipCollider.enabled = !shieldEnabled;
			shieldCollider.enabled = shieldEnabled;
		}
	}

	// Handle the spawning and despawning of of a impact effect
	void SpawnShieldImpact(Collision collision)
	{
		if (DamagePercent <= 0)
		{
			// Do nothing
			return;
		}

		// Spawn in a Impact effect object
		GameObject impact = Instantiate(ImpactVFX, ShieldMeshRenderer.transform.parent) as GameObject;
		impact.transform.localScale = Vector3.one * ShieldSize * 2;

		//GameObject hit = Instantiate(HitVFX, collision.contacts[0].point, Quaternion.identity) as GameObject;

		// Get that objects material
		Material material = impact.GetComponent<Renderer>().material;

		// Add the impactObject to a list of materials
		// This is used to fade out hits to the shield
		objectsToFade.Add(material);

		// Set the position of the sphere mask in the shield shader to display the new hit
		material.SetVector("_ImpactPosition", (transform.position - collision.contacts[0].point).normalized / -2f);

		// Destroy the new impact object after 2 seconds
		Destroy(impact, 2);
	}

	// Spawn a shield dissolve object
	void SpawnShieldDissolve()
	{
		if (DissolveVFX == null)
		{
			// Do nothing
			return;
		}

		GameObject dissolve = Instantiate(DissolveVFX, ShieldMeshRenderer.transform.parent) as GameObject;
		
		dissolve.transform.localScale = Vector3.one * ShieldSize * 2;
		dissolve.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", GetColor);

		Destroy(dissolve, 2);
	}

	// Fade the shield impact objects
	void FadeShieldImpacts()
	{
		// Loop through each ImpactObjects
		foreach (Material material in objectsToFade)
		{
			// If that material is destroyed we remove it from the list
			if (material == null)
			{
				objectsToFade.Remove(material);
			}
			else
			{
				// Fade the Impact objects 
				Color newColor = material.GetColor("_ImpactColor") + new Color(0, 0, 0, -2f) * Time.deltaTime;

				if (newColor.a < 0) {
					newColor.a = 0;
				}

				material.SetColor("_ImpactColor", newColor);
			}
		}
	}

	// Fade the values of the shield depending on the fade type
	void FadeShieldShader()
	{
		if (fadeType == FadeType.FULL_FADE)
		{
			// FULL_FADE means we fade the shield to nothing
			ShieldMeshRenderer.material.SetColor("_BaseColor", Color.Lerp(ShieldMeshRenderer.material.GetColor("_BaseColor"), Color.clear, 0.05f));
			ShieldMeshRenderer.material.SetColor("_FresnelColor", Color.Lerp(ShieldMeshRenderer.material.GetColor("_FresnelColor"), Color.clear, 0.05f));
		}
		else if (fadeType == FadeType.HALF_FADE)
		{
			// HALF_FADE means we fade the shield out so that only a small ring of color can be seen
			ShieldMeshRenderer.material.SetFloat("_FresnelSize", Mathf.Lerp(ShieldMeshRenderer.material.GetFloat("_FresnelSize"), 23, 0.1f));
		}
	}

	// Reset the values of the shield depending on the fade type
	void ResetShieldShader()
	{
		if (ResizeShield && DamagePercent == 0)
		{
			// Do nothying
			return;
		}

		// When the shield is hit. We set the shield to full visibility
		ShieldMeshRenderer.material.SetColor("_BaseColor", GetColor);
		ShieldMeshRenderer.material.SetColor("_FresnelColor", GetColor);

		if (fadeType == FadeType.HALF_FADE)
		{
			// HALF_FADE means we fade the shield out so that only a small ring of color can be seen
			ShieldMeshRenderer.material.SetFloat("_FresnelSize", 3);
		}
	}

    public void IgnoreCollider(Collider collider)
    {
        Physics.IgnoreCollision(shieldCollider, collider);
    }
}
