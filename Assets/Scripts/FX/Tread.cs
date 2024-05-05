using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tread : MonoBehaviour
{
    [SerializeField] MeshRenderer rend;
    [SerializeField] int materialIndex;
    [SerializeField] float offset;
    [SerializeField] Vector2 baseOffset;
    [SerializeField] float offsetIncrement;

    // Update is called once per frame
    void Update ()
    {
        rend.materials[materialIndex].mainTextureOffset = baseOffset * offset;

        offset += offsetIncrement * Time.deltaTime;
    }
}
