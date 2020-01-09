using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The PlayerConnection Script represents the UI for a player connection in the connection manager menu
/// </summary>
public class PlayerConnection : MonoBehaviour
{
	// A Reference to a text component used to display this players name
	public Text txtName;

	// A Reference to a Image component used to display this players name
	public Image imgColor;

	/// <summary>
	/// Initialize the PlayerConnection UI
	/// </summary>
	/// <param name="playerData">The Player Data for this players UI</param>
	//public void Initialize(PlayerData playerData)
	//{
	//	if (playerData != null)
	//	{
	//		txtName.text = playerData.GetName;
	//		imgColor.color = playerData.playerColor;
	//	}
	//}
}
