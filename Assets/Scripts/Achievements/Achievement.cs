using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Other/Achievement")]
public class Achievement : ScriptableObject
{

    public int id;
    public int achievementID;

    public string achievementName;
    public string achievementDesc;
    public Sprite achievementIcon;
}
