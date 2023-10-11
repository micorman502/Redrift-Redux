using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverableUI : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string hoverSfxName;
    [SerializeField] string hoverBoolName;
    
    public void SetHoverState (bool state)
    {
        animator.SetBool(hoverBoolName, state);

        if (state)
        {
            AudioManager.Instance.Play(hoverSfxName);
        }
    }
}
