using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResource
{
    WorldItem[] HandGather ();
    WorldItem[] ToolGather (ToolInfo tool);
    Resource GetResource ();
}
