using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementHandler : MonoBehaviour {

	public Achievement achievement;

	public Image backgroundImage;
	public TMP_Text achievementNameText;
	public TMP_Text achievementDescText;
	public Image achievementIconImage;
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
			backgroundImage.color = Color.white;
			achievementDescText.text = "? ? ?";
			achievementIconImage.color = Color.black;

			return;
		}

		achievementDescText.text = achievement.achievementDesc;
		achievementIconImage.color = Color.white;
		backgroundImage.color = Color.white;
	}
}
