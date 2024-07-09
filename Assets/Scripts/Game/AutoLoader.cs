using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoader : MonoBehaviour
{
    const string menuSceneName = "Menu";
    const string logoSceneName = "Logo";

    [SerializeField] bool executeInFirstUpdate = true;

    void Start ()
    {
        if (executeInFirstUpdate)
            return;

        AutoLoad();
    }

    private void Update ()
    {
        if (!executeInFirstUpdate)
            return;

        AutoLoad();

        enabled = false;
    }

    void AutoLoad ()
    {
        if (Application.isEditor)
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            SceneManager.LoadScene("Logo");
        }
    }
}
