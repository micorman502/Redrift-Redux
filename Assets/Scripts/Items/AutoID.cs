using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoID : MonoBehaviour
{
    [SerializeField] ItemOrder order;

    void Awake ()
    {
        for (int i = 0; i < order.items.Length; i++)
        {
            order.items[i].id = i;
        }
    }
}
