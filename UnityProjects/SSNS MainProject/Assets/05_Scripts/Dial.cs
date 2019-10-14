using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dial : MonoBehaviour
{
	// The number of points
	public int size = 8;

	// The size of a point
	public float pointSize = 0.1f;
	
	// The amount of time between changes when holding Q or E
	public float changeIntervulle = 0.2f;

	// A counter for the time between change intervulles
	private float counter = 0.0f;

	// The index the player has selected
	private int currentIndex = 0;

	// The items in the dial
	private List<GameObject> points = new List<GameObject>();

	// Start is called before the first frame update
	void Start()
	{
		// The amount to rotate per point
		float percent = 360f / size;

		// Initialize the points
		for (int i = 0; i < size; i++)
		{
			GameObject point = GameObject.CreatePrimitive(PrimitiveType.Quad);

			point.transform.parent = transform;
			point.transform.localScale = Vector3.one * pointSize;
			point.transform.localPosition = Vector3.zero;
			point.transform.Rotate(90.0f, -180.0f, percent * i);
			point.transform.Translate(point.transform.forward);
			point.name = "Dial " + (percent * i).ToString("000.0") + " [" + i + "]";
			point.gameObject.GetComponent<Renderer>().material.color = Color.cyan;

			points.Add(point);
		}

		// Set the selected point
		points[currentIndex].gameObject.GetComponent<Renderer>().material.color = Color.blue;

		// Fix the rotation for the Dial Manager
		transform.Rotate(-90f, 0f, 0f);
	}

	private void Update()
	{
		counter += Time.deltaTime;

		// E Click
		if (Input.GetAxis("LBumper") < 0)
		{
			if (counter > changeIntervulle)
			{
				counter = 0.0f;
				SetPoint(currentIndex + 1);
			}
		}

		// Q Click
		if (Input.GetAxis("RBumper") > 0)
		{
			if (counter > changeIntervulle)
			{
				counter = 0.0f;
				SetPoint(currentIndex - 1);
			}
		}
	}

	public void SetPoint(int index)
	{
		// Turn old selection the non selected color
		points[currentIndex].gameObject.GetComponent<Renderer>().material.color = Color.cyan;

		// Set the new index
		currentIndex = index < 0 ? points.Count - 1 : index % points.Count;

		// Highlight the new selection
		points[currentIndex].gameObject.GetComponent<Renderer>().material.color = Color.blue;
	}
}
