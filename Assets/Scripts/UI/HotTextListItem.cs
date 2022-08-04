using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotTextListItem : MonoBehaviour
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] GameObject keybindObject;
    [SerializeField] Image keybindImage;
    [SerializeField] TMP_Text keybindText;
    [SerializeField] Sprite mouseButtonLeftImage;
    [SerializeField] Sprite mouseButtonMiddleImage;
    [SerializeField] Sprite mouseButtonRightImage;
    HotTextInfo hotText;
    
    public void Setup (HotTextInfo hotText)
    {
        this.hotText = hotText;

        infoText.text = hotText.text;

        if (hotText.key != KeyCode.None)
        {
            SetKeybind(hotText.key);
        } else
        {
            keybindObject.SetActive(false);
        }
    }

    void SetKeybind (KeyCode key)
    {
        keybindText.text = "";

        if (key == KeyCode.Mouse0)
        {
            keybindImage.sprite = mouseButtonLeftImage;
            keybindImage.type = Image.Type.Simple;
            return;
        }
        if (key == KeyCode.Mouse1)
        {
            keybindImage.sprite = mouseButtonRightImage;
            keybindImage.type = Image.Type.Simple;
            return;
        }
        if (key == KeyCode.Mouse2)
        {
            keybindImage.sprite = mouseButtonMiddleImage;
            keybindImage.type = Image.Type.Simple;
            return;
        }

        keybindText.text = hotText.key.ToString();
    }
}
