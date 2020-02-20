using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float timeToRespawn;

    private ShipController player;

    public void Respawn()
    {
        player.transform.position = AreaManager.Instance.PlayerDestination;
        player.transform.rotation = Quaternion.identity;

        if (player.TryGetComponent(out HealthAndShields health))
        {
            health.ResetValues(true);
        }

        player.gameObject.SetActive(true);
    }

    private void Start()
    {
        TryGetComponent(out player);
    }
}
