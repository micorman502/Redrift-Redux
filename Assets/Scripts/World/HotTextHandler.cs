using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotTextHandler : MonoBehaviour, IHotText
{
    List<HotTextInfo> hotTexts = new List<HotTextInfo>();
    bool shown;
    
    public void AddHotText (HotTextInfo hotText)
    {
        if (shown)
        {
            HotTextManager.Instance.AddHotText(hotText);
        }
        hotTexts.Add(hotText);
    }

    public void RemoveHotText (HotTextInfo hotText)
    {
        if (shown)
        {
            HotTextManager.Instance.RemoveHotText(hotText);
        }
        hotTexts.Remove(hotText);
    }

    public void ShowHotText ()
    {
        shown = true;
        for (int i = 0; i < hotTexts.Count; i++)
        {
            HotTextManager.Instance.ReplaceHotText(hotTexts[i]);
        }
    }

    public void HideHotText ()
    {
        shown = false;
        for (int i = 0; i < hotTexts.Count; i++)
        {
            HotTextManager.Instance.RemoveHotText(hotTexts[i]);
        }
    }

    public void UpdateHotText ()
    {
        shown = true;

        for (int i = 0; i < hotTexts.Count; i++)
        {
            HotTextManager.Instance.UpdateHotText(hotTexts[i]);
        }
    }

    void OnDestroy ()
    {
        HideHotText();
    }
}
