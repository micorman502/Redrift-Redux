using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Other/Credits Data")]
public class CreditsData : ScriptableObject
{
    public Credit[] credits;
    public string joiner;

    public string GetCredits ()
    {
        string creditString = "";
        for (int i = 0; i < credits.Length; i++)
        {
            creditString += credits[i].creditName + joiner + credits[i].creditLink + "\n";
        }

        return creditString;
    }
}

[System.Serializable]
public class Credit
{
    public string creditName;
    public string creditLink;
}
