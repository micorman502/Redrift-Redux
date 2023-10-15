using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurgatoryTimer : MonoBehaviour
{
    [SerializeField] TextMesh text;
    float timeSet;
    float duration;
    public void SetTimer (float time)
    {
        timeSet = Time.time;
        duration = time;
    }

    void Update()
    {
        text.text = (Mathf.Round((timeSet + duration - Time.time) * 10) / 10).ToString();

        if (Time.time > timeSet + duration)
            RespawnPlayer();
    }

    void RespawnPlayer ()
    {
        if (!PlayerController.currentPlayer.dead)
            return;

        PlayerController.currentPlayer.Respawn();
    }
}
