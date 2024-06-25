using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatControls : MonoBehaviour, IInteractable, IHotText, IGameplayInputHandler
{
    bool controlsActive;
    public void Interact ()
    {
        Debug.Log("Interact");
        if (controlsActive)
        {
            Deactivate();
        }
        else
        {
            Activate();
        }
    }

    void Activate ()
    {
        if (controlsActive)
            return;

        Debug.Log("Activate");

        controlsActive = true;

        GameplayInputProvider.Instance.AddOverrideHandler(this);
    }

    void Deactivate ()
    {
        if (!controlsActive)
            return;

        controlsActive = false;

        GameplayInputProvider.Instance.RemoveOverrideHandler(this);
    }

    public void TakeMovementInput (Vector3 movementAxes, bool accelerate, bool jump)
    {

    }

    public void ShowHotText ()
    {
        HotTextManager.Instance.AddHotText(new HotTextInfo(controlsActive ? "Deactivate" : "Activate", KeyCode.F, HotTextInfo.Priority.Interact, "boatControls"));
    }

    public void UpdateHotText ()
    {
        HotTextManager.Instance.UpdateHotText(new HotTextInfo(controlsActive ? "Deactivate" : "Activate", KeyCode.F, HotTextInfo.Priority.Interact, "boatControls"));
    }

    public void HideHotText ()
    {
        HotTextManager.Instance.RemoveHotText("boatControls");
    }
}
