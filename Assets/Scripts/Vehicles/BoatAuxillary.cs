using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoatAuxillary : MonoBehaviour
{
    [SerializeField] BoatControls controls;
    [SerializeField] TMP_Text gearText;
    [SerializeField] Transform steeringWheel;
    [SerializeField] Transform waterWheel;
    [SerializeField] Transform rudder;
    [SerializeField] AudioSource engineIdleAudio;
    [SerializeField] AudioSource engineActiveAudio;

    float angle;
    int gear;

    void OnEnable ()
    {
        controls.OnAngleChange += OnAngleChange;
        controls.OnGearSwitch += OnGearSwitch;
        controls.OnToggleActivation += OnToggleActivation;

        OnGearSwitch(0);
        OnToggleActivation(false);

        engineIdleAudio.enabled = true;
        engineActiveAudio.enabled = true;
    }

    void OnDisable ()
    {
        controls.OnAngleChange -= OnAngleChange;
        controls.OnGearSwitch -= OnGearSwitch;
        controls.OnToggleActivation -= OnToggleActivation;

        engineIdleAudio.enabled = false;
        engineActiveAudio.enabled = false;
    }

    void OnToggleActivation (bool active)
    {
        if (!active)
        {
            return;
        }

        engineIdleAudio.Play();
        engineActiveAudio.Play();
    }

    void OnAngleChange (float _angle)
    {
        angle = _angle;

        steeringWheel.localEulerAngles = new Vector3(0, 0, -angle * 4f);
        rudder.localEulerAngles = new Vector3(0, -angle, 0);
    }

    void OnGearSwitch (int _gear)
    {
        gear = _gear;
        gearText.text = gear.ToString();
    }

    private void FixedUpdate ()
    {
        engineActiveAudio.volume = Mathf.Lerp(engineActiveAudio.volume, controls.EngineEffort(), Time.fixedDeltaTime);
        engineIdleAudio.volume = Mathf.Lerp(engineIdleAudio.volume, controls.Active() ? 0.5f : 0f, Time.fixedDeltaTime);

        if (!controls.Active())
            return;

        waterWheel.Rotate(new Vector3(gear * Time.fixedDeltaTime * 25f, 0, 0), Space.Self);
    }
}
