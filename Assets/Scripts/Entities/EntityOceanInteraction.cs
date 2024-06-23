using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityOceanInteraction : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    private void FixedUpdate ()
    {
        if (transform.position.y < VoidOcean.startThreshold)
        {
            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;
        }
    }
}
