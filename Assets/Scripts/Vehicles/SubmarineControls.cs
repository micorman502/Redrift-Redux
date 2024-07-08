using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineControls : MonoBehaviour, IInteractable, IGameplayInputHandler, IHotText
{
    bool controlsActive;

    public void Interact ()
    {
        if (controlsActive)
        {
            Deactivate();
        }
        else
        {
            Activate();
        }
    }

    public void TakeMovementInput (Vector3 moveAxes, bool accel, bool jump)
    {

    }

    void Activate ()
    {
        if (controlsActive)
            return;

        Debug.Log("Activate");

        controlsActive = true;

        GameplayInputProvider.Instance.AddOverrideHandler(this);

        HotTextManager.Instance.AddHotText(new HotTextInfo("Emergency Dismount", KeyCode.Space, HotTextInfo.Priority.Jump, "boatDismount"));
    }

    void Deactivate ()
    {
        if (!controlsActive)
            return;

        controlsActive = false;

        GameplayInputProvider.Instance.RemoveOverrideHandler(this);

        HotTextManager.Instance.RemoveHotText("boatDismount");
    }

    public void ShowHotText ()
    {
        HotTextManager.Instance.AddHotText(new HotTextInfo(controlsActive ? "Dismount" : "Pilot", KeyCode.F, HotTextInfo.Priority.Interact, "boatControls"));
    }

    public void UpdateHotText ()
    {
        HotTextManager.Instance.UpdateHotText(new HotTextInfo(controlsActive ? "Dismount" : "Pilot", KeyCode.F, HotTextInfo.Priority.Interact, "boatControls"));
    }

    public void HideHotText ()
    {
        HotTextManager.Instance.RemoveHotText("boatControls");
    }
}
