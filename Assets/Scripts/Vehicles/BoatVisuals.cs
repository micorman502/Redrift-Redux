using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoatVisuals : MonoBehaviour
{
    [SerializeField] BoatControls controls;
    [SerializeField] TMP_Text gearText;
    [SerializeField] Transform steeringWheel;

    void OnEnable ()
    {
        controls.OnIgnition += OnIgnition;
        controls.OnAngleChange += OnAngleChange;
        controls.OnGearSwitch += OnGearSwitch;

        OnIgnition(false);
    }

    void OnDisable ()
    {
        controls.OnIgnition -= OnIgnition;
        controls.OnAngleChange -= OnAngleChange;
        controls.OnGearSwitch -= OnGearSwitch;
    }

    void OnIgnition (bool ignition)
    {
        gearText.enabled = ignition;
    }

    void OnAngleChange (float angle)
    {
        steeringWheel.localEulerAngles = new Vector3(0, 0, -angle * 4f);
    }

    void OnGearSwitch (int gear)
    {
        gearText.text = gear.ToString();
    }
}
