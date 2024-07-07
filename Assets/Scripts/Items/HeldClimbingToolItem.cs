using UnityEngine;

public class HeldClimbingToolItem : HeldItem
{
    ClimbingToolInfo climbingTool;
    [SerializeField] string digSfx;
    [SerializeField] Rigidbody playerRb;
    [SerializeField] PlayerStamina playerStamina;
    [SerializeField] PlayerMovement playerMovement;

    [SerializeField] GameObject leftHook;
    [SerializeField] Transform leftHookRayPoint;
    [SerializeField] Transform leftHookRestPoint;
    Transform leftHookFollowPoint;
    bool leftHooked;
    float lastLeftHook;

    [SerializeField] GameObject rightHook;
    [SerializeField] Transform rightHookRayPoint;
    [SerializeField] Transform rightHookRestPoint;
    Transform rightHookFollowPoint;
    bool rightHooked;
    float lastRightHook;

    bool hookFlag;

    // Start is called before the first frame update
    void Start ()
    {
        climbingTool = item as ClimbingToolInfo;
    }

    public override void Use ()
    {
        HookLeft();
    }

    public override void StopUse ()
    {
        UnhookLeft();
    }

    public override void AltUse ()
    {
        HookRight();
    }

    public override void StopAltUse ()
    {
        UnhookRight();
    }

    public override void ItemUpdate ()
    {
        if (leftHooked)
        {
            leftHook.transform.position = leftHookFollowPoint.transform.position;
        }

        if (rightHooked)
        {
            rightHook.transform.position = rightHookFollowPoint.transform.position;
        }
    }

    public override void ItemFixedUpdate ()
    {
        if (leftHooked)
        {
            HookRangeFunctions(leftHook.transform.position);
        }

        if (rightHooked)
        {
            HookRangeFunctions(rightHook.transform.position);
        }

        StaminaChecks();
    }

    internal void HookLeft ()
    {
        if (!HookRaycast(leftHookRayPoint, out RaycastHit hit))
            return;
        if (playerStamina.Value < 1)
            return;
        if (Time.time < lastLeftHook + 0.7f)
            return;

        if (leftHooked)
            return;

        leftHooked = true;
        lastLeftHook = Time.time;

        HookFunctions();

        GlobalAudioPlayer.Instance.PlayClip(digSfx, leftHook.transform);

        if (leftHookFollowPoint)
        {
            Destroy(leftHookFollowPoint.gameObject);
        }

        leftHookFollowPoint = new GameObject().transform;

        if (hit.transform)
        {
            leftHookFollowPoint.parent = hit.transform;
        }

        leftHookFollowPoint.position = hit.point + (leftHook.transform.position - leftHookRayPoint.position);
    }

    internal void HookRight ()
    {
        if (!HookRaycast(rightHookRayPoint, out RaycastHit hit))
            return;
        if (playerStamina.Value < 1)
            return;
        if (Time.time < lastRightHook + 0.7f)
            return;

        if (rightHooked)
            return;

        rightHooked = true;
        lastRightHook = Time.time;

        HookFunctions();

        GlobalAudioPlayer.Instance.PlayClip(digSfx, rightHook.transform);

        if (rightHookFollowPoint)
        {
            Destroy(rightHookFollowPoint.gameObject);
        }

        rightHookFollowPoint = new GameObject().transform;

        if (hit.transform)
        {
            rightHookFollowPoint.parent = hit.transform;
        }

        rightHookFollowPoint.position = hit.point + (rightHook.transform.position - rightHookRayPoint.position);
    }

    void HookFunctions ()
    {
        UpdateHotTexts();

        if (hookFlag)
            return;

        hookFlag = true;

        playerMovement.SetAbseilingState(true);
        playerMovement.ApplySpeedMult(climbingTool.speedMult);
    }

    internal void UnhookLeft ()
    {
        if (!leftHooked)
            return;

        leftHooked = false;

        UnhookFunctions();

        leftHook.transform.position = leftHookRestPoint.position;
    }

    internal void UnhookRight ()
    {
        if (!rightHooked)
            return;

        rightHooked = false;

        UnhookFunctions();

        rightHook.transform.position = rightHookRestPoint.position;
    }

    void UnhookFunctions ()
    {
        UpdateHotTexts();

        if (!hookFlag || leftHooked || rightHooked)
            return;

        hookFlag = false;

        playerMovement.RemoveSpeedMult(climbingTool.speedMult);
        playerMovement.SetAbseilingState(false);
    }

    void UpdateHotTexts ()
    {
        HotTextManager.Instance.UpdateHotText(new HotTextInfo(leftHooked ? "Unhook (L)" : "Hook (L)", KeyCode.Mouse0, HotTextInfo.Priority.UseItem, "climbingToolL"));
        HotTextManager.Instance.UpdateHotText(new HotTextInfo(rightHooked ? "Unhook (R)" : "Hook (R)", KeyCode.Mouse1, HotTextInfo.Priority.AltUseItem, "climbingToolR"));
    }

    public override void SetChildStateFunctions (bool state)
    {
        UnhookLeft();
        UnhookRight();

        if (state)
        {
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Hook (L)", KeyCode.Mouse0, HotTextInfo.Priority.UseItem, "climbingToolL"));
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Hook (R)", KeyCode.Mouse1, HotTextInfo.Priority.AltUseItem, "climbingToolR"));
        }
        else
        {
            HotTextManager.Instance.RemoveHotText("climbingToolL");
            HotTextManager.Instance.RemoveHotText("climbingToolR");
        }
    }

    void StaminaChecks ()
    {
        if (!leftHooked && !rightHooked)
            return;

        if (!playerStamina.ChangeValue(-climbingTool.staminaUse * Time.fixedDeltaTime))
        {
            UnhookLeft();
            UnhookRight();
        }
    }

    bool HookRaycast (Transform rayPoint, out RaycastHit hit, bool offsetCast = true)
    {
        Vector3 position = rayPoint.position - (offsetCast ? rayPoint.forward : Vector3.zero);
        bool raycastSuccess = Physics.Raycast(position, rayPoint.forward, out hit, climbingTool.range, ~LayerMask.GetMask("Player", "Ignore Raycast", "Item"), QueryTriggerInteraction.Ignore);

        float surfaceDot = Vector3.Dot(Vector3.up, hit.normal);
        bool verticalCheck = surfaceDot > -0.9f && surfaceDot < 0.6f;

        return raycastSuccess && verticalCheck;
    }

    void HookRangeFunctions (Vector3 hookPosition)
    {
        Vector3 diff = hookPosition - playerRb.position;

        if (diff.sqrMagnitude < climbingTool.range * climbingTool.range)
        {
            playerRb.velocity = Vector3.zero;
            return;
        }

        playerRb.AddForce(diff * 200f);
    }
}
