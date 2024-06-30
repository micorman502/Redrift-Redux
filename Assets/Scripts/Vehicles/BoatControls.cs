using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatControls : MonoBehaviour, IInteractable, IHotText, IGameplayInputHandler
{
    public event Action<bool> OnToggleActivation;
    public event Action<int> OnGearSwitch;
    public event Action<float> OnAngleChange;

    bool controlsActive;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform forceApplicationPoint;
    [SerializeField] float baseForce = 100f;
    [SerializeField] float baseReverseMult = 0.6f;
    [SerializeField] float baseTurnSpeed = 22.5f;
    [SerializeField] int maxForwardGear = 2;
    [SerializeField] int maxReverseGear = 1;
    [SerializeField] float angleLimit = 45f;
    int currentGear;
    float currentAngle;

    bool gearInputLastFrame;

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

        OnToggleActivation?.Invoke(true);

        GameplayInputProvider.Instance.AddOverrideHandler(this);

        HotTextManager.Instance.AddHotText(new HotTextInfo("Emergency Dismount", KeyCode.Space, HotTextInfo.Priority.Jump, "boatDismount"));
    }

    void Deactivate ()
    {
        if (!controlsActive)
            return;

        controlsActive = false;

        OnToggleActivation?.Invoke(false);

        GameplayInputProvider.Instance.RemoveOverrideHandler(this);

        HotTextManager.Instance.RemoveHotText("boatDismount");
    }

    void FixedUpdate ()
    {
        if (!controlsActive)
            return;

        ManageMovement();
    }

    void ManageMovement ()
    {
        rb.AddForceAtPosition(Vector3.SlerpUnclamped(forceApplicationPoint.forward, forceApplicationPoint.right, -currentAngle / 180f) * GetForce(currentGear), forceApplicationPoint.position);
    }

    public void TakeMovementInput (Vector3 movementAxes, bool accelerate, bool jump)
    {
        float angleInput = Mathf.Clamp(movementAxes.x, -1, 1) * baseTurnSpeed * Time.fixedDeltaTime;
        int gearInput = Mathf.RoundToInt(movementAxes.z);

        if (gearInputLastFrame && gearInput != 0)
        {
            gearInput = 0;
        }
        else
        {
            gearInputLastFrame = false;
        }

        if (jump)
        {
            EmergencyDismount();
        }

        SwitchGear(gearInput);
        ChangeAngle(angleInput);

        if (gearInput != 0)
        {
            gearInputLastFrame = true;
        }

        if (angleInput == 0)
        {
            ManageSteeringReturn();
        }
    }

    void ManageSteeringReturn ()
    {
        if (Mathf.Abs(currentAngle) < 3f)
        {
            SetAngle(Mathf.Lerp(currentAngle, 0, Time.fixedDeltaTime * 2f));
        }
    }

    void SwitchGear (int gearChange)
    {
        SetGear(currentGear + gearChange);
    }

    void SetGear (int gear)
    {
        if (gear == currentGear)
            return;

        currentGear = Mathf.Clamp(gear, -maxReverseGear, maxForwardGear);

        OnGearSwitch?.Invoke(currentGear);
    }

    void ChangeAngle (float angleChange)
    {
        SetAngle(currentAngle + angleChange);
    }

    void SetAngle (float angle)
    {
        if (angle == currentAngle)
            return;

        currentAngle = Mathf.Clamp(angle, -angleLimit, angleLimit);

        OnAngleChange?.Invoke(currentAngle);
    }

    void EmergencyDismount ()
    {
        Deactivate();
    }

    float GetForce (int gear)
    {
        if (gear > 0)
        {
            return baseForce * gear;
        }

        if (gear < 0)
        {
            return baseForce * gear * baseReverseMult;
        }

        return 0f;
    }

    public bool Active ()
    {
        return controlsActive;
    }

    public float EngineEffort ()
    {
        if (!controlsActive)
            return 0f;

        if (currentGear < 0)
        {
            return (-currentGear / (float)maxReverseGear) * baseReverseMult;
        }
        if (currentGear > 0)
        {
            return currentGear / (float)maxForwardGear;
        }

        return 0;
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
