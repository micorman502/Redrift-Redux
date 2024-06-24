using System.Collections;
using UnityEngine;

public class BoatBuoyancy : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float buoyancy;
    [SerializeField] float buoyancyRampDistance;
    [SerializeField] Transform[] buoyancyPoints;

    void FixedUpdate ()
    {
        for (int i = 0; i < buoyancyPoints.Length; i++)
        {
            ProcessBuoyancy(buoyancyPoints[i]);
        }
    }

    void ProcessBuoyancy (Transform buoyancyPoint)
    {
        if (buoyancyPoint.transform.position.y > VoidOcean.startThreshold)
            return;

        float distance = VoidOcean.startThreshold - buoyancyPoint.transform.position.y;

        float mod = Mathf.Lerp(0, 1, distance / buoyancyRampDistance);

        rb.AddForceAtPosition(Vector3.up * buoyancy * mod, buoyancyPoint.position, ForceMode.Force);
    }
}