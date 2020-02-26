using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

public class MenuManager : MonoBehaviour
{
	public static MenuManager Instance;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(Instance.gameObject);
		}

		Instance = this;

		pauseMenu.SetBool("Open", false);
	}

	[SerializeField] private Animator pauseMenu;

	public OpenMenuType openMenu = OpenMenuType.MENU;

	private bool _gamePaused;

	public void NavigateNodeMap(Vector2 direction)
	{
		if (direction.x >= 0.05f)
		{
			NodeManager.Instance.SelectNodeChoice(1);
		}
		else if (direction.x <= -0.05f)
		{
			NodeManager.Instance.SelectNodeChoice(-1);
		}
	}

	public void NavigateMenu(Vector2 direction)
	{

	}

	public void Select()
	{
		//CinematicController.Instance.Select();
	}

	public void Return()
	{
		//CinematicController.Instance.Return();
	}

	public void PauseGame(bool state)
	{
		pauseMenu.SetBool("Open", state);

		foreach (Player player in GameObject.FindObjectsOfType<Player>())
		{
			if (state)
			{
				player.SetPlayerActionMap("MenuNavigation");
			}
			else
			{
				player.SetPlayerActionMap("Ship");
			}
		}

		_gamePaused = state;
	}
}
