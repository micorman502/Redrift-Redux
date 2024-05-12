using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIKeyToggler : MonoBehaviour
{
    public bool State { get { return state; } set { SetState(value); } }
    [SerializeField] bool state = true;
    [SerializeField] GameObject target;
    [SerializeField] Canvas canvasTarget;
    [SerializeField] KeyCode toggleKey;
    [SerializeField] bool unlockLook;
    public UnityEvent<bool> calledFunctions;
    // Start is called before the first frame update
    void Start ()
    {
        SetState(state);
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            SetState(!state);
        }
    }

    public void SetState (bool activeState)
    {
        state = activeState;

        if (target)
        {
            target.SetActive(state);
        }
        if (canvasTarget)
        {
            canvasTarget.enabled = activeState;
        }

        ManageLookUnlocking();

        if (unlockLook)
        {
            LookLocker.MouseLocked = !state;
        }
        calledFunctions.Invoke(state);
    }

    void ManageLookUnlocking ()
    {
        if (!unlockLook)
            return;

        if (state)
        {
            LookLocker.AddUnlockingObject(this);
        }
        else
        {
            LookLocker.RemoveUnlockingObject(this);
        }
    }
}
