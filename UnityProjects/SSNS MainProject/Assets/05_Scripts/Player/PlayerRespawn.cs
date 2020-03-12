using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float timeToRespawn;
    [SerializeField] private GameObject cameraPrefab;

    private ShipController player;

    private GameObject camera;

    public void Respawn()
    {
        Transform playerSpawn = AreaManager.Instance.CurrentArea.FindSafeSpawn();

        player.transform.position = playerSpawn.position;
        player.transform.rotation = playerSpawn.rotation;

        if (player.TryGetComponent(out HealthAndShields health))
        {
            health.ResetValues(true);
        }

        AreaManager.Instance.CurrentArea.RespawnArea();

        player.gameObject.SetActive(true);

        Destroy(camera);

        GameManager.Instance.SwitchState(SNSSTypes.GameState.BATTLE);
    }

    private void OnDeath()
    {
        camera = Instantiate(cameraPrefab);
        camera.transform.position = transform.position;
        camera.transform.LookAt(transform.position);
    }

    private void Start()
    {
        TryGetComponent(out player);

        if (TryGetComponent(out HealthAndShields health))
        {
            health.onDeath += OnDeath;
        }
    }
}
