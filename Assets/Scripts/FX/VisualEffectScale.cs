using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffectScale : MonoBehaviour
{
    [SerializeField] Vector3 constantScaleChange;
    [SerializeField] Vector3 maxScaleThreshold;

    private void Update ()
    {
        transform.localScale += constantScaleChange * Time.deltaTime;

        if (maxScaleThreshold != Vector3.zero && transform.localScale.sqrMagnitude > maxScaleThreshold.sqrMagnitude)
        {
            transform.localScale = Vector3.zero;
            enabled = false;
        }
    }
}
