using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// RingControl.cs
// Uses the ring in the center to determine how the ship rotates
[RequireComponent(typeof(RectTransform))]
public class RingControl : MonoBehaviour
{
    Image target; // The dot in the ring

    Vector2 dir; // The direction from the center to the target

    Vector2 center; // The center

    // Radius for the inner and outer rings
    float ringRadius;
    float innerRingRadius;

    private void Awake()
    {
        target = transform.Find("Target").GetComponent<Image>();
        center = transform.Find("Redicle").GetComponent<RectTransform>().transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Gets the size of the image to determine the radius for the outer ring
        RectTransform rectTransform = GetComponent<RectTransform>();

        if (rectTransform.sizeDelta.x > rectTransform.sizeDelta.y)
        {
            ringRadius = rectTransform.sizeDelta.x / 2.0f;
        }
        else
        {
            ringRadius = rectTransform.sizeDelta.y / 2.0f;
        }

        // Gets the size of the image to determine the radius for the inner ring
        RectTransform innerRectTransform = transform.Find("Inner Ring").GetComponent<RectTransform>();

        if (innerRectTransform.sizeDelta.x > innerRectTransform.sizeDelta.y)
        {
            innerRingRadius = innerRectTransform.sizeDelta.x / 2.0f;
        }
        else
        {
            innerRingRadius = innerRectTransform.sizeDelta.y / 2.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Doesn't update when there's no input or the game isn't focused
        if (mouseX == 0 && mouseY == 0 || !Application.isFocused) return;

        // Finds the new position for the target
        Vector2 newPosition = target.transform.position + new Vector3(mouseX, mouseY) * Time.deltaTime * 100;

        dir = newPosition - center;

        // Determines if the target is in the inner ring or is outside of the outer ring
        Vector3 diff = newPosition - center;
        float distance = diff.sqrMagnitude;

        if (distance <= innerRingRadius * innerRingRadius)
        {
            dir = new Vector2();
        }
        else if (distance > ringRadius * ringRadius)
        {
            newPosition = center + dir.normalized * ringRadius;
        }

        // Moves the target
        target.transform.position = newPosition;
    }

    // Converts the direction for using with the ship's rotation
    public Vector2 GetShipRotation { get { return new Vector2(-dir.y, dir.x); } }
}
