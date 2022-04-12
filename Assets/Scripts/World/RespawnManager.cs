using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] float respawnTime;
    [SerializeField] PurgatoryTimer timer;
    private void OnEnable()
    {
        PlayerController.OnDeath += ReceiveDeath;
    }

    private void OnDisable()
    {
        PlayerController.OnDeath -= ReceiveDeath;
    }

    void ReceiveDeath ()
    {
        timer.SetTimer(respawnTime);
        Invoke("RespawnPlayer", respawnTime);
    }

    void RespawnPlayer ()
    {
        PlayerController.currentPlayer.Respawn();
    }
}
