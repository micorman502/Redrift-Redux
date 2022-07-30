using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldClimbingToolItem : HeldItem
{
    ClimbingToolInfo climbingTool;
    [SerializeField] Transform rayPos;
    [SerializeField] Rigidbody rb;
    Vector3 originalEuler;
    bool hooked;

    // Start is called before the first frame update
    void Start ()
    {
        climbingTool = item as ClimbingToolInfo;
        originalEuler = itemGameObject.transform.localEulerAngles;
    }

    public override void ItemUpdate ()
    {
        bool facingGround = Physics.SphereCast(rayPos.position, climbingTool.radius, rayPos.transform.forward, out RaycastHit dummy, climbingTool.range, -1, QueryTriggerInteraction.Ignore);

        if (Input.GetMouseButton(1) && facingGround)
        {
            hooked = true;
        }
        else
        {
            hooked = false;
        }
    }

    public override void ItemFixedUpdate ()
    {
        if (hooked)
        {
            rb.velocity = rb.velocity / (1 + climbingTool.drag * Time.fixedDeltaTime);
            itemGameObject.transform.localEulerAngles = originalEuler + new Vector3(10, 0, 0); //TEMP
        } else
        {
            itemGameObject.transform.localEulerAngles = originalEuler;
        }
    }

    public override void SetChildState (bool _state)
    {
        base.SetChildState(_state);
        hooked = false;
    }
}
