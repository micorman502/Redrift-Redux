using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditsText : MonoBehaviour
{
    [SerializeField] CreditsData data;
    [SerializeField] TMP_Text text;

    void Start ()
    {
        text.text = data.GetCredits();
    }
}
