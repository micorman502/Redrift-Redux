using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldGrappleItem : HeldItem
{
    [SerializeField] Rigidbody playerRigidbody;
    GrappleInfo grappleInfo;
    [SerializeField] AudioSource grappleSFX;
    [SerializeField] Transform grappleRestPoint;
    [SerializeField] GameObject grappleHeadVisual;
    [SerializeField] LineRenderer rope;
    Vector3 grapplePoint;

    bool grappled; // true if grapple has reached grapplePoint
    bool grappling; // true if grapple is "travelling" to grapplePoint or is at grapplePoint
    float grappleTime; // how long it will take for the grapple to start applying force.
    float lastUse;

    void Awake ()
    {
        grappleInfo = item as GrappleInfo;

        rope.positionCount = 2;
    }

    public override void Use ()
    {
        base.Use();

        StartGrapple();
    }

    public override void StopUse ()
    {
        base.StopUse();

        StopGrapple();
    }

    public override void ItemFixedUpdate ()
    {
        base.ItemFixedUpdate();

        ProgressGrapple();
    }

    public override void ItemUpdate ()
    {
        base.ItemUpdate();

        float cooldownTime = Mathf.Ceil(CooldownTimeLeft() * 10f) / 10f;
        bool grappleCooldown = cooldownTime > 0 && !grappling;

        HotTextManager.Instance.UpdateHotText(new HotTextInfo("Grapple" + (grappleCooldown ? $" ({cooldownTime})" : ""), KeyCode.Mouse0, HotTextInfo.Priority.UseItem, "grappleTool", !grappling && !GrappleChecks()));

        if (grappling)
        {
            grappleHeadVisual.transform.position = Vector3.Lerp(grappleRestPoint.position, grapplePoint, (Time.time - lastUse) / grappleTime);
        }

        rope.SetPosition(0, Vector3.zero);
        rope.SetPosition(1, grappleHeadVisual.transform.localPosition);
    }

    public override void SetChildStateFunctions (bool state)
    {
        base.SetChildStateFunctions(state);

        if (!state)
        {
            StopGrapple();
            HotTextManager.Instance.RemoveHotText("grappleTool");
        }
        else
        {
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Grapple", KeyCode.Mouse0, HotTextInfo.Priority.UseItem, "grappleTool", !grappling && !GrappleChecks()));
        }
    }

    void StartGrapple ()
    {
        if (grappling)
            return;
        if (!GrappleChecks())
            return;

        grappleSFX?.Play();

        lastUse = Time.time;

        grappling = true;
        SetGrapplePoint(GrappleRaycast().point);
    }

    void ProgressGrapple ()
    {
        if (!grappling)
            return;

        if (grappled)
        {
            playerRigidbody.AddForce((grapplePoint - grappleRestPoint.position) * grappleInfo.pullForce, ForceMode.Acceleration);
            return;
        }

        if (Time.time > lastUse + grappleTime)
        {
            grappled = true;
        }
    }

    void StopGrapple ()
    {
        if (!grappling)
            return;

        lastUse = Time.time;

        grappling = false;
        grappled = false;

        grappleHeadVisual.transform.position = grappleRestPoint.position;
    }

    void SetGrapplePoint (Vector3 _grapplePoint)
    {
        grapplePoint = _grapplePoint;
        grappleTime = Vector3.Distance(grappleRestPoint.position, grapplePoint) / grappleInfo.travelSpeed;
    }

    bool GrappleChecks ()
    {
        if (Time.time < lastUse + grappleInfo.cooldown)
            return false;

        RaycastHit hit = GrappleRaycast();
        if (hit.transform == null)
            return false;

        grapplePoint = hit.point;

        return true;

    }

    float CooldownTimeLeft ()
    {
        return Mathf.Max(0, grappleInfo.cooldown - (Time.time - lastUse)); // clamps value to be no less than 0.
    }

    RaycastHit GrappleRaycast ()
    {
        Physics.Raycast(grappleRestPoint.position, grappleRestPoint.forward, out RaycastHit hit, grappleInfo.range);
        return hit;

    }
}
