using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGLBodgeFix : MonoBehaviour
{
    string audioClip = "UIClick";
    int counter = 4;

    void FixedUpdate ()
    {
        if (counter <= 0)
            return;

        GlobalAudioPlayer.Instance.PlayClip(audioClip);
        counter--;
    }
}
