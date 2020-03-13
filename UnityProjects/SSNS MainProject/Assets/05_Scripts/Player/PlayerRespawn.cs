using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float timeToRespawn;
    [SerializeField] private GameObject cameraPrefab;
    [SerializeField] private RectTransform hud;

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

        hud.gameObject.SetActive(true);

        GameManager.Instance.SwitchState(SNSSTypes.GameState.BATTLE);
    }

    private void OnDeath()
    {
        camera = Instantiate(cameraPrefab);
        camera.transform.position = transform.position + Quaternion.LookRotation(transform.forward) * player.Behaviour.deathPos;
        camera.transform.LookAt(transform.position);

        camera.transform.rotation = Quaternion.Euler(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, transform.eulerAngles.z);

        hud.gameObject.SetActive(false);
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
