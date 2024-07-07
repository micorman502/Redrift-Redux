using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffectScale : MonoBehaviour
{
    [SerializeField] Vector3 constantScaleChange;

    private void Update ()
    {
        transform.localScale += constantScaleChange * Time.deltaTime;
    }
}
