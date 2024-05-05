using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubResourceHandler : MonoBehaviour, IResource //this is to be used on colliders of resources
{
    [SerializeField] ResourceHandler handler;

    public Resource GetResource ()
    {
        return handler.GetResource();
    }

    public WorldItem[] HandGather ()
    {
        return handler.HandGather();
    }

    public WorldItem[] ToolGather (ToolInfo tool)
    {
        return handler.ToolGather(tool);
    }
}
