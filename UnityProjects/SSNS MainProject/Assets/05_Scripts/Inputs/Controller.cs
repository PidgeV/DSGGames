using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
	// When a controller joins the game 
	// It adds itself to the player manager
	public void JoinGame()
	{
		PlayerManager.Instance.Join(this);

		// Initialize the controllers data
	}
}
