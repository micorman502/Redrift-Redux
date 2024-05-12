using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldBuildingItem : HeldItem
{
    public override void SetChildStateFunctions (bool state)
    {
        if (state)
        {
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Use Construction Hammer", KeyCode.Mouse0, 0, "buildingBuild", true));
        }
        else
        {
            HotTextManager.Instance.RemoveHotText("buildingBuild");
        }
    }
}
