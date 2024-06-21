using UnityEngine;

public class HeldClimbingToolItem : HeldItem
{
    ClimbingToolInfo climbingTool;
    [SerializeField] Rigidbody playerRb;

    [SerializeField] GameObject leftHook;
    [SerializeField] Transform leftHookRayPoint;
    [SerializeField] Transform leftHookRestPoint;
    bool leftHooked;

    [SerializeField] GameObject rightHook;
    [SerializeField] Transform rightHookRayPoint;
    [SerializeField] Transform rightHookRestPoint;
    bool rightHooked;

    // Start is called before the first frame update
    void Start ()
    {
        climbingTool = item as ClimbingToolInfo;
    }

    public override void ItemUpdate ()
    {

    }

    public override void ItemFixedUpdate ()
    {

    }

    internal void UnhookLeft ()
    {
        if (!leftHooked)
            return;

        leftHooked = false;

        UpdateHotTexts();
    }

    internal void UnhookRight ()
    {
        if (!rightHooked)
            return;

        rightHooked = false;

        UpdateHotTexts();
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
            HotTextManager.Instance.RemoveHotText("climbingTool");
            HotTextManager.Instance.RemoveHotText("climbingTool");
        }
    }
}
