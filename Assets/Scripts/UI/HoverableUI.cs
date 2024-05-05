using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverableUI : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string hoverSfxName;
    [SerializeField] string clickSfxName;
    [SerializeField] string hoverBoolName;

    public void SetHoverState (bool state)
    {
        if (animator)
        {
            animator.SetBool(hoverBoolName, state);
        }

        if (state)
        {
            AudioManager.Instance.Play(hoverSfxName);
        }
    }

    public void OnClick ()
    {
        AudioManager.Instance.Play(clickSfxName);
    }
}
