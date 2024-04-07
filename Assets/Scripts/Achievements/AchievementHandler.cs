using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementHandler : MonoBehaviour {

	Achievement achievement;

	[SerializeField] Image backgroundImage;
	[SerializeField] TMP_Text achievementNameText;
	[SerializeField] TMP_Text achievementDescText;
	[SerializeField] Image achievementIconImage;
	[SerializeField] Color baseColour;
	[SerializeField] Color unlockedColour;
	bool unlocked;

	public void Setup (Achievement _achievement)
    {
		achievement = _achievement;

		UpdateUI();
    }

	public void SetAchievementState (bool _unlocked)
    {
		unlocked = _unlocked;

		UpdateUI();
    }

	void UpdateUI ()
	{
		achievementNameText.text = achievement.achievementName;
		achievementIconImage.sprite = achievement.achievementIcon;

		if (!unlocked)
		{
			backgroundImage.color = baseColour;
			achievementDescText.text = "? ? ?";
			achievementIconImage.color = Color.black;

			return;
		}

		achievementDescText.text = achievement.achievementDesc;
		achievementIconImage.color = Color.white;
		backgroundImage.color = unlockedColour;
	}

	public Achievement GetAchievement ()
    {
		return achievement;
    }
}
