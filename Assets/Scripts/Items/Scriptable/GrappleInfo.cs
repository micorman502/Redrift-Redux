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

    public override string GetDescription ()
    {
        return base.GetDescription() + range + "m Range, " + cooldown + "s Cooldown";
    }
}
