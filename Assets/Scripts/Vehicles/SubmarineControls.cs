using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineControls : MonoBehaviour, IInteractable, IGameplayInputHandler, IHotText
{
    bool controlsActive;
    [SerializeField] Transform exitPoint;
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float verticalAngleLimit = 85f;

    Vector3 rawEulers;
    Vector3 lerpEulers;

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
        if (accel)
        {
            Deactivate();
        }

        moveAxes.Normalize();

        rb.AddForce(transform.forward * moveAxes.z * speed, ForceMode.Acceleration);

        rawEulers.y += moveAxes.x * rotationSpeed * Time.fixedDeltaTime;
        rawEulers.x -= moveAxes.y * rotationSpeed * Time.fixedDeltaTime;
        rawEulers.x = Mathf.Clamp(rawEulers.x, -verticalAngleLimit, verticalAngleLimit);

        lerpEulers = Vector3.Lerp(lerpEulers, rawEulers, 3f * Time.fixedDeltaTime);
        transform.eulerAngles = lerpEulers;
    }

    public void FixedUpdate ()
    {
        if (!controlsActive)
            return;

        Player.GetPlayerObject().transform.position = transform.position;
    }

    void Activate ()
    {
        if (controlsActive)
            return;

        Debug.Log("Activate");

        controlsActive = true;

        Player.GetPlayerObject().GetComponent<Rigidbody>().detectCollisions = false;

        GameplayInputProvider.Instance.AddOverrideHandler(this);

        HotTextManager.Instance.AddHotText(new HotTextInfo("Dismount", KeyCode.LeftShift, HotTextInfo.Priority.Jump, "boatDismount"));
    }

    void Deactivate ()
    {
        if (!controlsActive)
            return;

        controlsActive = false;

        Player.GetPlayerObject().GetComponent<Rigidbody>().detectCollisions = true;
        Player.GetPlayerObject().transform.position = exitPoint.position;

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
