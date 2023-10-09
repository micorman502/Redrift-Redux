using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementDatabase : MonoBehaviour
{
    public static AchievementDatabase Instance { get; private set; }
    static AchievementRegister Register;
    [SerializeField] AchievementRegister register;

    private void Awake ()
    {
        if (Instance)
        {
            Debug.Log("There is already an AchievementDatabase in existence. Deleting this AchievementDatabase.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Register = register;

        IDAchievements();
    }

    static void IDAchievements ()
    {
        for (int i = 0; i < Register.achievements.Length; i++)
        {
            Register.achievements[i].id = i;
        }
    }

    public static Achievement[] GetAllAchievements ()
    {
        return Register.achievements;
    }

    public static Achievement GetAchievement (int id)
    {
        return Register.achievements[id];
    }

    public static Achievement GetAchievementByInternalName (string objectName)
    {
        for (int i = 0; i < Register.achievements.Length; i++)
        {
            if (Register.achievements[i].name == objectName)
            {
                return Register.achievements[i];
            }
        }

        Debug.Log("No achievement with the object name '" + objectName + "' could be found. Returning first achievement.");
        return Register.achievements[0];
    }

    public static Achievement GetAchievementByExternalName (string achievementName)
    {
        for (int i = 0; i < Register.achievements.Length; i++)
        {
            if (Register.achievements[i].achievementName == achievementName)
            {
                return Register.achievements[i];
            }
        }

        Debug.Log("No achievement with the name '" + achievementName + "' could be found. Returning first achievement.");
        return Register.achievements[0];
    }
}
