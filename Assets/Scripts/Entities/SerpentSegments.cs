using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerpentSegments : MonoBehaviour
{
    [SerializeField] GameObject[] segments;
    Vector3[] positions;
    [SerializeField] float segmentRadius;
    [SerializeField] float correctionRate;

    private void Start ()
    {
        positions = new Vector3[segments.Length];

        for (int i = 0; i < segments.Length; i++)
        {
            positions[i] = segments[i].transform.position;
        }
    }

    private void FixedUpdate ()
    {
        positions[0] = segments[0].transform.position;

        for (int i = 1; i < positions.Length; i++)
        {
            ProcessSegmentPosition(i);
        }
    }

    void ProcessSegmentPosition (int index)
    {
        Vector3 currentSegmentPos = positions[index];
        Vector3 previousSegmentPos = positions[index - 1];

        if (Vector3.Distance(currentSegmentPos, previousSegmentPos) > segmentRadius * 2f)
        {
            positions[index] = Vector3.Lerp(currentSegmentPos, previousSegmentPos, correctionRate * Time.fixedDeltaTime);
        }
        if (currentSegmentPos.y > VoidOcean.startThreshold)
        {
            positions[index] += Physics.gravity * Time.fixedDeltaTime;
        }

        segments[index].transform.position = positions[index];
        segments[index].transform.forward = previousSegmentPos - currentSegmentPos;
    }
}
