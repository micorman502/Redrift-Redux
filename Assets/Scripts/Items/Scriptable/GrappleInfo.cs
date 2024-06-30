using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemGrapple", menuName = "Items/Grapple")]
public class GrappleInfo : ItemInfo
{
    public float range;
    public float travelSpeed;
    public float pullForce;
    public float staminaUse;
    public float cooldown;

    public override void CompileDescription ()
    {
        base.CompileDescription();

        AssignDescriptionStat(range, "range");
        AssignDescriptionStat(travelSpeed, "travelSpeed");
        AssignDescriptionStat(pullForce, "pullForce");
        AssignDescriptionStat(staminaUse, "staminaUse");
        AssignDescriptionStat(cooldown, "cooldown");
    }
}
