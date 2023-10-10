using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TMPDropdownExtensions : MonoBehaviour
{
    [SerializeField] TMP_Dropdown target;
    [SerializeField] Dropdown.DropdownEvent extraEvent;

    public void NextOption ()
    {
        if (target.value + 1 >= target.options.Count)
        {
            target.value = 0;
        }
        else
        {
            target.value++;
        }
    }

    public void PreviousOption ()
    {
        if (target.value - 1 < 0)
        {
            target.value = target.options.Count - 1;
        }
        else
        {
            target.value--;
        }
    }

    public void FakeUpdateValue ()
    {
        target.value = target.value;
    }

    public void CallExtraEvent ()
    {
        extraEvent.Invoke(target.value);
    }
}
