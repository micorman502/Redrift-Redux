using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{

    public static PersistentData Instance;
    public bool loadingFromSave;
    public int saveToLoad;
    public int difficulty;
    public int mode;
    public string newSaveName;

    void Awake ()
    {
        if (Instance)
        {
            //Debug.Log("There is already a PersistentData in existence. Destroying this PersistentData.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
