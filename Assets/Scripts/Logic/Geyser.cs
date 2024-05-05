using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Geyser : MonoBehaviour
{
    [SerializeField] GameObject visualFX;
    [SerializeField] GameObject effectArea;
    [SerializeField] Vector3 effectForce;


    [SerializeField] int minFrequency;
    [SerializeField] int maxFrequency;
    float frequency;

    int lastLoopCount;

    void Start ()
    {
        frequency = new Random(SaveManager.Instance.GetSaveSeed()).Next(minFrequency, maxFrequency);

        int loopCount = Mathf.FloorToInt(SaveManager.Instance.GetSaveAge() / frequency);
        lastLoopCount = loopCount;
    }

    void FixedUpdate ()
    {
        int loopCount = Mathf.FloorToInt(SaveManager.Instance.GetSaveAge() / frequency);

        if (loopCount > lastLoopCount)
        {
            Trigger();
        }

        lastLoopCount = loopCount;
    }

    void Trigger ()
    {
        GameObject newFX = Instantiate(visualFX, effectArea.transform);

        Destroy(newFX, 5f);

        Collider[] colliders = Physics.OverlapBox(effectArea.transform.position, effectArea.transform.localScale / 2f);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].attachedRigidbody)
                continue;

            colliders[i].attachedRigidbody.AddForce(effectForce, ForceMode.VelocityChange);
        }
    }
}
