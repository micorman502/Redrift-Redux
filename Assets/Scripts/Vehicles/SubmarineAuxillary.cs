using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineAuxillary : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform outerRing;

    private void Update ()
    {
        //outerRing.transform.forward = Vector3.Lerp(outerRing.transform.forward, rb.velocity.normalized, 2 * Time.deltaTime);
    }
}
