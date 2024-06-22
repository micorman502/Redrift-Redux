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
            GlobalAudioPlayer.Instance.PlayClip(hoverSfxName);
        }
    }

    public void OnClick ()
    {
        GlobalAudioPlayer.Instance.PlayClip(clickSfxName);
    }
}
