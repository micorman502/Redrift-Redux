using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIKeyToggler : MonoBehaviour
{
    [SerializeField] bool state = true;
    [SerializeField] GameObject target;
    [SerializeField] Canvas canvasTarget;
    [SerializeField] KeyCode toggleKey;
    [SerializeField] bool lockLook;
    public UnityEvent<bool> calledFunctions;
    // Start is called before the first frame update
    void Start()
    {
        SetState(state);
    }

    // Update is called once per frame
    void Update()
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

        if (lockLook)
        {
            LookLocker.MouseLocked = !state;
        }
        calledFunctions.Invoke(state);
    }
}
