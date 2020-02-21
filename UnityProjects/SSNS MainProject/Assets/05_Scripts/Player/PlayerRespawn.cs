using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float timeToRespawn;

    private ShipController player;

    private Camera camera;

    public void Respawn()
    {
        player.transform.position = AreaManager.Instance.PlayerDestination;
        player.transform.rotation = Quaternion.identity;

        if (player.TryGetComponent(out HealthAndShields health))
        {
            health.ResetValues(true);
        }

        player.gameObject.SetActive(true);

        Destroy(camera.gameObject);
    }

    private void OnDeath()
    {
        camera = new Camera();
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
