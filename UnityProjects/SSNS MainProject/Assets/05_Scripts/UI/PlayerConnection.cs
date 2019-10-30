using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerConnection : MonoBehaviour
{
	public Text txtName;
	public Image imgColor;

	private int ID;

	public void Initialize(PlayerData playerData)
	{
		if (playerData != null)
		{
			txtName.text = playerData.GetName;
			imgColor.color = playerData.playerColor;
		}
	}
}
