using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevLoader : MonoBehaviour
{
#if UNITY_EDITOR
    static bool loadedInitial;

    void Awake ()
    {
        if (loadedInitial)
            return;

        if (SceneManager.GetActiveScene().name == "Initial")
        {
            loadedInitial = true;
            return;
        }

        SceneManager.LoadScene("Initial");
        loadedInitial = true;
    }
#endif
}