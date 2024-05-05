using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialInertia : MonoBehaviour
{

    Transform root; // Optional to set custom root in inspector

    void Awake ()
    {
        if (!root)
        {
            root = transform.root;
        }
    }

    void OnCollisionEnter (Collision col)
    {
        CheckRoot();

        if (col.gameObject.layer == LayerMask.NameToLayer("Small Island"))
        {
            root.SetParent(col.transform);
        }
    }

    void OnCollisionStay (Collision col)
    {
        CheckRoot();

        if (col.gameObject.layer == LayerMask.NameToLayer("Small Island"))
        {
            root.SetParent(col.transform);
        }
    }

    void OnCollisionExit (Collision col)
    {
        CheckRoot();

        root.SetParent(null);
    }

    void CheckRoot ()
    {
        if (!root)
        {
            root = transform.root;
        }
    }

    public void SetRoot (Transform newRoot)
    {
        root = newRoot;
    }

    public void SetRootParent (Transform newRootParent)
    {
        root.SetParent(newRootParent);
    }
}