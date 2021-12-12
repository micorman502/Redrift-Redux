using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] float respawnTime;
    [SerializeField] PurgatoryTimer timer;
    private void OnEnable()
    {
        PlayerEvents.OnPlayerDeath += ReceiveDeath;
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerDeath -= ReceiveDeath;
    }

    void ReceiveDeath ()
    {
        timer.SetTimer(respawnTime);
        Invoke("RespawnPlayer", respawnTime);
    }

    void RespawnPlayer ()
    {
        PlayerEvents.RespawnPlayer();
    }
}
