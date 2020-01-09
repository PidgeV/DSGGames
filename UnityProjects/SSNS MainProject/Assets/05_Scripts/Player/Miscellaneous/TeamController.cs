using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour
{
	public PilotController pilotController;
	public TurretController turretController;

	public void AssignController(PilotController controller)
	{
		controller = pilotController;
	}

	public void AssignController(TurretController controller)
	{
		controller = turretController;
	}

	public Camera pilotCamera;
	public Camera turretCamera;

	public void SetCameras(int ID)
	{
		pilotCamera.targetDisplay = ID;
		turretCamera.targetDisplay = ID;
	}
}
