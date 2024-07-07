using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldGrappleItem : HeldItem
{
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] PlayerStamina playerStamina;
    GrappleInfo grappleInfo;
    [SerializeField] AudioSource grapplePullSFX;
    [SerializeField] string grappleLaunchSFXName;
    [SerializeField] string grappleStopSFXName;
    [SerializeField] Transform grappleRestPoint;
    [SerializeField] GameObject grappleHeadVisual;
    [SerializeField] LineRenderer rope;
    //Vector3 grapplePoint;
    Transform currentGrapplePoint;
    Quaternion initialHeadRotation;

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

        StaminaChecks();
    }

    public override void ItemUpdate ()
    {
        base.ItemUpdate();

        float cooldownTime = Mathf.Ceil(CooldownTimeLeft() * 10f) / 10f;
        bool grappleCooldown = cooldownTime > 0 && !grappling;

        HotTextManager.Instance.UpdateHotText(new HotTextInfo("Grapple" + (grappleCooldown ? $" ({cooldownTime})" : ""), KeyCode.Mouse0, HotTextInfo.Priority.UseItem, "grappleTool", !grappling && !GrappleChecks()));

        if (grappling)
        {
            grappleHeadVisual.transform.position = Vector3.Lerp(grappleRestPoint.position, currentGrapplePoint.position, (Time.time - lastUse) / grappleTime);
            grappleHeadVisual.transform.rotation = initialHeadRotation;
        }

        rope.SetPosition(0, Vector3.zero);
        rope.SetPosition(1, grappleHeadVisual.transform.localPosition - rope.transform.localPosition);
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

        GlobalAudioPlayer.Instance.PlayClip(grappleLaunchSFXName, transform);

        grapplePullSFX.Play();

        lastUse = Time.time;
        grappling = true;

        RaycastHit grappleHit = GrappleRaycast();
        SetGrapplePos(grappleHit.point, grappleHit.transform);
        initialHeadRotation = grappleHeadVisual.transform.rotation;
    }

    void ProgressGrapple ()
    {
        if (!grappling)
            return;

        if (grappled)
        {
            playerRigidbody.AddForce((currentGrapplePoint.position - grappleRestPoint.position) * grappleInfo.pullForce, ForceMode.Acceleration);
            return;
        }

        if (Time.time > lastUse + grappleTime)
        {
            grappled = true;

            grapplePullSFX.Stop();

            GlobalAudioPlayer.Instance.PlayClip(grappleStopSFXName, transform);
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
        grappleHeadVisual.transform.up = -itemGameObject.transform.forward; // Dirty line. Ew ew ew. Not really much i can do while using .blend files, though

        grapplePullSFX.Stop();
    }

    void SetGrapplePos (Vector3 grapplePos, Transform grappleParent = null)
    {
        if (currentGrapplePoint)
        {
            Destroy(currentGrapplePoint.gameObject);
        }

        currentGrapplePoint = new GameObject().transform;

        if (grappleParent)
        {
            currentGrapplePoint.parent = grappleParent;
        }

        currentGrapplePoint.position = grapplePos;

        grappleTime = Vector3.Distance(grappleRestPoint.position, grapplePos) / grappleInfo.travelSpeed;
    }

    bool GrappleChecks ()
    {
        if (Time.time < lastUse + grappleInfo.cooldown)
            return false;

        RaycastHit hit = GrappleRaycast();
        if (hit.transform == null)
            return false;

        return true;
    }

    void StaminaChecks ()
    {
        if (!grappled)
            return;

        if (!playerStamina.ChangeValue(-grappleInfo.staminaUse * Time.fixedDeltaTime))
        {
            StopGrapple();
        }
    }

    float CooldownTimeLeft ()
    {
        return Mathf.Max(0, grappleInfo.cooldown - (Time.time - lastUse)); // clamps value to be no less than 0.
    }

    RaycastHit GrappleRaycast ()
    {
        Physics.Raycast(grappleRestPoint.position, grappleRestPoint.forward, out RaycastHit hit, grappleInfo.range, ~LayerMask.GetMask("Ignore Raycast", "Item"), QueryTriggerInteraction.Ignore);
        return hit;

    }
}
