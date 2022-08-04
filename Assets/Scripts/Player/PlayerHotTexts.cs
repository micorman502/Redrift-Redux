using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHotTexts : MonoBehaviour
{
    PlayerController controller;

    List<IHotText> currentHotTexts = new List<IHotText>();

    void Start ()
    {
        controller = gameObject.GetComponent<PlayerController>();
    }

    void Update ()
    {
        IHotText[] newHotTexts = GetHotTexts(controller.GetTarget());

        TryAddHotTexts(newHotTexts);
        TryRemoveHotTexts(newHotTexts);

        UpdateHotTexts();
    }

    void TryRemoveHotTexts (IHotText[] newHotTexts)
    {
        for (int i = currentHotTexts.Count - 1; i >= 0; i--)
        {
            bool currentHotTextFound = false;
            for (int j = 0; j < newHotTexts.Length; j++)
            {
                if (newHotTexts[j] == currentHotTexts[i])
                {
                    currentHotTextFound = true;
                    break;
                }
            }
            if (!currentHotTextFound)
            {
                currentHotTexts[i].HideHotText();
                currentHotTexts.RemoveAt(i);
            }
        }
    }

    void TryAddHotTexts (IHotText[] newHotTexts)
    {
        for (int i = 0; i < newHotTexts.Length; i++)
        {
            int newHotTextIndex = i;
            for (int j = 0; j < currentHotTexts.Count; j++)
            {
                if (newHotTexts[i] == currentHotTexts[j])
                {
                    newHotTextIndex = -1;
                    break;
                }
            }

            if (newHotTextIndex != -1)
            {
                currentHotTexts.Add(newHotTexts[newHotTextIndex]);
            }
        }
    }

    void UpdateHotTexts ()
    {
        for (int i = 0; i < currentHotTexts.Count; i++)
        {
            currentHotTexts[i].ShowHotText();
        }
    }

    IHotText[] GetHotTexts (GameObject target)
    {
        if (target == null)
            return new IHotText[0];
        IHotText[] hotTexts = target.GetComponents<IHotText>();
        if (hotTexts.Length == 0)
        {
            hotTexts = target.GetComponentsInParent<IHotText>();
        }
        return hotTexts;
    }
}
