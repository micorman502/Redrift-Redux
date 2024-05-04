using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotTextListItem : MonoBehaviour
{
    [SerializeField] Animation initialAnimation;
    [SerializeField] TMP_Text infoText;
    [SerializeField] GameObject keybindObject;
    [SerializeField] Image keybindImage;
    [SerializeField] TMP_Text keybindText;
    [SerializeField] Sprite mouseButtonLeftImage;
    [SerializeField] Sprite mouseButtonMiddleImage;
    [SerializeField] Sprite mouseButtonRightImage;
    [SerializeField] Sprite defaultKeySprite;
    public HotTextInfo hotText { get; private set; }

    public void PlayAnimation () //the HotTextManager uses this; when a new HotTextListItem appears, you want to play the animation. However, if it is just replacing a previous one, don't play the animation.
    {
        initialAnimation.Play();
    }

    public void Setup (HotTextInfo hotText)
    {
        this.hotText = hotText;

        infoText.text = hotText.text;

        SetKeybind(hotText.key);
    }

    void SetKeybind (KeyCode key)
    {
        keybindText.text = "";
        string keyBindString = hotText.key.ToString();

        if (hotText.blocked)
        {
            keybindImage.color = Color.grey;
            keyBindString = "";
        }
        else
        {
            keybindImage.color = Color.white;
        }

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

        keybindImage.sprite = defaultKeySprite;
        //keybindImage.type = Image.Type.Sliced;

        keybindText.text = keyBindString;
    }
}
