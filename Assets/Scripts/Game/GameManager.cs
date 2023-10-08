using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool gameInitialised { get; private set; }
    void Awake ()
    {
        if (Instance)
        {
            Debug.Log($"A static instance of {this.name} exists. Destroying this instance...");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gameInitialised = true;

        DontDestroyOnLoad(this);
    }

}
