using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverableAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string hoverBoolName;
    
    public void SetHoverState (bool state)
    {
        animator.SetBool(hoverBoolName, state);
    }
}
