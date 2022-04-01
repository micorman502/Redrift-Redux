using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HotTextListItem : MonoBehaviour
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] GameObject keybindObject;
    [SerializeField] TMP_Text keybindText;
    HotTextInfo hotText;
    
    public void Setup (HotTextInfo hotText)
    {
        this.hotText = hotText;

        infoText.text = hotText.text;

        if (hotText.key != KeyCode.None)
        {
            keybindText.text = hotText.key.ToString();
        } else
        {
            keybindObject.SetActive(false);
        }
    }
}
