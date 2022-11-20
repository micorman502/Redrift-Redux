using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoader : MonoBehaviour
{
    const string menuSceneName = "Menu";
    const string logoSceneName = "Logo";

    void Start ()
    {
        if (Application.isEditor)
        {
            SceneManager.LoadScene("Menu");
        } else
        {
            SceneManager.LoadScene("Logo");
        }
    }
}
