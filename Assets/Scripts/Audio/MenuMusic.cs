using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    [SerializeField] AudioSource music;

    void Start ()
    {
        music.Play();
    }

    void OnDestroy ()
    {
        music.Stop();
    }
}
