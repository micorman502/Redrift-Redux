using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoatVisuals : MonoBehaviour
{
    [SerializeField] BoatControls controls;
    [SerializeField] TMP_Text gearText;
    [SerializeField] Transform steeringWheel;
    [SerializeField] Transform waterWheel;

    float waterWheelRotation;
    float angle;
    int gear;

    void OnEnable ()
    {
        controls.OnAngleChange += OnAngleChange;
        controls.OnGearSwitch += OnGearSwitch;

        OnGearSwitch(0);
    }

    void OnDisable ()
    {
        controls.OnAngleChange -= OnAngleChange;
        controls.OnGearSwitch -= OnGearSwitch;
    }

    void OnAngleChange (float _angle)
    {
        angle = _angle;

        steeringWheel.localEulerAngles = new Vector3(0, 0, -angle * 4f);
        waterWheel.localEulerAngles = new Vector3(waterWheelRotation, -angle, 0);
    }

    void OnGearSwitch (int _gear)
    {
        gear = _gear;
        gearText.text = gear.ToString();
    }

    private void FixedUpdate ()
    {
        waterWheelRotation += gear * Time.fixedDeltaTime * 25f;
        Mathf.Repeat(waterWheelRotation, 360f);

        waterWheel.localEulerAngles = new Vector3(waterWheelRotation, -angle, 0);
    }
}
