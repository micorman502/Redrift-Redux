using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    void Awake ()
    {
        if (Instance)
        {
            Debug.Log($"A static instance of {this.name} exists. Destroying this instance...");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(this);
    }

}
