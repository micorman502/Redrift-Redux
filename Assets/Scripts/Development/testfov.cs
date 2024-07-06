using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testfov : MonoBehaviour
{
    private void Awake ()
    {
        Debug.Log(Camera.HorizontalToVerticalFieldOfView(103f, 1920f / 1080f));
    }
}
