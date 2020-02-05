using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO..
// - Trigger shield break
// - Change Shape when while is not up
// 

public enum FadeType
{
	NO_FADE,
	HALF_FADE,
	FULL_FADE
}

public class ShieldProjector : MonoBehaviour
{
	// This is a delegate that is called when the shield is hit
	public delegate void OnShieldHit(GameObject attacker);
	public OnShieldHit onShieldHit;

	// Keeps material references from impact objects that need to be faded out
	private List<Material> impactObjects = new List<Material>();

	[Space(5)]
	[SerializeField] MeshRenderer MeshRenderer;
	[SerializeField] GameObject HitVFX;

	[Space(5)]
	[ColorUsage(true, true)]
	public Color Color;

	[Space(5)]
	public FadeType fadeType = FadeType.NO_FADE;

	// Start is called before the first frame update
	void Start()
	{
		MeshRenderer.material.SetColor("_BaseColor", Color);
		MeshRenderer.material.SetColor("_FresnelColor", Color);
	}

	// Update is called once per frame
	void Update()
	{
		// The shader for the impact objects used object space, I believe that is why the object using the shader (the shield)
		// needs to not be rotated. This like makes sure the impact shader works
		transform.rotation = Quaternion.identity;

		// Loop through each ImpactObjects
		foreach (Material material in impactObjects)
		{
			// If that material is destroyed we remove it from the list
			if (material == null)
			{
				impactObjects.Remove(material);
			}
			else
			{
				// Fade the Impact objects 
				Color currentColor = material.GetColor("_ImpactColor");
				Color newColor = currentColor + new Color(0, 0, 0, -(2f * Time.deltaTime));

				if (newColor.a < 0) {
					newColor.a = 0;
				}

				material.SetColor("_ImpactColor", newColor);
			}
		}

		if (fadeType == FadeType.FULL_FADE)
		{
			// FULL_FADE means we fade the shield to nothing
			MeshRenderer.material.SetColor("_BaseColor", Color.Lerp(MeshRenderer.material.GetColor("_BaseColor"), Color.clear, 0.05f));
			MeshRenderer.material.SetColor("_FresnelColor", Color.Lerp(MeshRenderer.material.GetColor("_FresnelColor"), Color.clear, 0.05f));
		}
		else if (fadeType == FadeType.HALF_FADE)
		{
			// HALF_FADE means we fade the shield out so that only a small ring of color can be seen
			MeshRenderer.material.SetFloat("_FresnelSize", Mathf.Lerp(MeshRenderer.material.GetFloat("_FresnelSize"), 23, 0.1f));
		}
	}

	/// <summary>
	/// When the shield is hit
	/// </summary>
	private void OnCollisionEnter(Collision collision)
	{
		if (fadeType == FadeType.FULL_FADE)
		{
			// When the shield is hit. We set the shield to full visibility
			MeshRenderer.material.SetColor("_BaseColor", Color);
			MeshRenderer.material.SetColor("_FresnelColor", Color);
		}
		else if (fadeType == FadeType.HALF_FADE)
		{
			// HALF_FADE means we fade the shield out so that only a small ring of color can be seen
			MeshRenderer.material.SetFloat("_FresnelSize", 3);
		}

		// Spawn in a Impact effect object
		GameObject impactObject = Instantiate(HitVFX, transform) as GameObject;

		// Get that objects material
		Material material = impactObject.GetComponent<Renderer>().material;

		// Add the impactObject to a list of materials
		// This is used to fade out hits to the shield
		impactObjects.Add(material);

		// Impact Position
		Vector3 position = -(transform.position - collision.contacts[0].point).normalized / 2f;
		material.SetVector("_ImpactPosition", new Vector4(position.x, position.y, position.z, 0));

		// Destroy the new impact object after 2 seconds
		Destroy(impactObject, 2);

		// Invoke the onShieldHit delegate
		onShieldHit.Invoke(collision.gameObject);
	}
}
