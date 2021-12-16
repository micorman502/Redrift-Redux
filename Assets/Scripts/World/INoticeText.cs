using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INoticeText
{
    string GetNoticeText();
    void SetNoticeText(string noticeText);

    /*
    [SerializeField] string tooltip;
    public string GetTooltip () {
        return tooltip;
    }

    public void SetTooltip (string _tooltip) {
        tooltip = _tooltip;
    } */
}
