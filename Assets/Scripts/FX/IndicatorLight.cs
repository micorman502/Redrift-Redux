using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorLight : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material activeMaterial;
    [SerializeField] Material inactiveMaterial;
    bool active;

    public void Toggle ()
    {
        SetState(!active);
    }

    public void SetState (bool state)
    {
        active = state;

        meshRenderer.material = active ? activeMaterial : inactiveMaterial;
    }
}
