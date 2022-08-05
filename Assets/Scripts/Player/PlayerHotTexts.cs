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

        TryRemoveHotTexts(newHotTexts);
        TryAddHotTexts(newHotTexts);

        UpdateHotTexts();
    }

    void TryRemoveHotTexts (IHotText[] newHotTexts)
    {
        for (int i = currentHotTexts.Count - 1; i >= 0; i--)
        {
            bool removingHotTextFound = false; //if the hot text we are trying to remove has been found in newHotTexts
            for (int j = 0; j < newHotTexts.Length; j++)
            {
                if (newHotTexts[j] == currentHotTexts[i])
                {
                    removingHotTextFound = true;
                    break;
                }
            }
            if (!removingHotTextFound) //don't remove the hottext if it exists in currentHotTexts and newHotTexts.
            {
                RemoveHotText(i);
            }
        }
    }

    void TryAddHotTexts (IHotText[] newHotTexts)
    {
        for (int i = 0; i < newHotTexts.Length; i++)
        {
            bool addingHotTextExists = false; //if the hot text we are trying to add exists in currentHotTexts
            for (int j = 0; j < currentHotTexts.Count; j++)
            {
                if (newHotTexts[i] == currentHotTexts[j])
                {
                    addingHotTextExists = true;
                    break;
                }
            }

            if (!addingHotTextExists) //don't add the hottext if it already exists in currentHotTexts and newHotTexts.
            {
                AddHotText(newHotTexts[i]);
            }
        }
    }

    void RemoveHotText (int index)
    {
        currentHotTexts[index].HideHotText();
        currentHotTexts.RemoveAt(index);
    }

    void AddHotText (IHotText hotText)
    {
        currentHotTexts.Add(hotText);
        hotText.ShowHotText();
    }

    void UpdateHotTexts ()
    {
        for (int i = 0; i < currentHotTexts.Count; i++)
        {
            currentHotTexts[i].UpdateHotText();
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
