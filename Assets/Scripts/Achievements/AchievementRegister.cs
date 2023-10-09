using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Registers/Achievement Register")]
public class AchievementRegister : ScriptableObject
{
    public Achievement[] achievements;
}
