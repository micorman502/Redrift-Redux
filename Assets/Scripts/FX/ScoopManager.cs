using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoopManager : MonoBehaviour
{
    [SerializeField] ConveyorBelt targetConveyor;
    [SerializeField] GameObject scoopPrefab;
    [SerializeField] Transform scoopHolder;
    [SerializeField] Animator[] animators;
    [SerializeField] float loopLength = 3.2f;
    [SerializeField] int scoopCount;

    void OnEnable ()
    {
        targetConveyor.OnSpeedSet += OnSpeedSet;
    }

    void OnDisable ()
    {
        targetConveyor.OnSpeedSet -= OnSpeedSet;
    }

    void Start ()
    {
        InitialiseScoops();
        OnSpeedSet(targetConveyor.Speed);
    }

    void InitialiseScoops ()
    {
        animators = new Animator[scoopCount];
        for (int i = 0; i < scoopCount; i++)
        {
            animators[i] = Instantiate(scoopPrefab, scoopHolder).GetComponent<Animator>();

            float offsetTime = ((i + 1) / (float)scoopCount) * loopLength;
            animators[i].Update(offsetTime);
        }
    }

    void OnSpeedSet (float speed)
    {
        if (animators.Length == 0)
            return;

        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].speed = speed;
        }
    }
}
