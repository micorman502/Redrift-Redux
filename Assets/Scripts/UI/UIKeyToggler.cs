using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIKeyToggler : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] KeyCode toggleKey;
    [SerializeField] bool lockLook;
    public UnityEvent<bool> calledFunctions;
    bool state;
    // Start is called before the first frame update
    void Start()
    {
        
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
        target.SetActive(state);
        if (lockLook)
        {
            PlayerEvents.OnLockStateSet(!state);
        }
        calledFunctions.Invoke(state);
    }
}
