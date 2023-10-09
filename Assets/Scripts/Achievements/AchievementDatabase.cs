using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementDatabase : MonoBehaviour
{
    public static AchievementDatabase Instance { get; private set; }
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

        IDAchievements();
    }

    void IDAchievements ()
    {
        for (int i = 0; i < register.achievements.Length; i++)
        {
            register.achievements[i].id = i;
        }
    }

    public Achievement[] GetAllAchievements ()
    {
        return register.achievements;
    }

    public Achievement GetAchievement (int id)
    {
        return register.achievements[id];
    }

    public Achievement GetAchievementByInternalName (string objectName)
    {
        for (int i = 0; i < register.achievements.Length; i++)
        {
            if (register.achievements[i].name == objectName)
            {
                return register.achievements[i];
            }
        }

        Debug.Log("No achievement with the object name '" + objectName + "' could be found. Returning first achievement.");
        return register.achievements[0];
    }

    public Achievement GetAchievementByExternalName (string achievementName)
    {
        for (int i = 0; i < register.achievements.Length; i++)
        {
            if (register.achievements[i].achievementName == achievementName)
            {
                return register.achievements[i];
            }
        }

        Debug.Log("No achievement with the name '" + achievementName + "' could be found. Returning first achievement.");
        return register.achievements[0];
    }
}
