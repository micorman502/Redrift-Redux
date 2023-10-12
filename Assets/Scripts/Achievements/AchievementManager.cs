using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour {

	public static AchievementManager Instance;
	public Transform achievementContainer;
	public Transform acievementList;
	public GameObject achievementPrefab;

	bool[] hasAchievements;

	AchievementHandler[] achievementHandlers;

	void Awake() {
		if (Instance)
        {
			Destroy(this);
			return;
        }
		Instance = this;

		int achievementCount = AchievementDatabase.GetAllAchievements().Length;
		hasAchievements = new bool[achievementCount];
		achievementHandlers = new AchievementHandler[achievementCount];

		int i = 0;
		foreach (Achievement achievement in AchievementDatabase.GetAllAchievements())
        {
			GameObject achievementObj = Instantiate(achievementPrefab, acievementList);
			AchievementHandler handler = achievementObj.GetComponent<AchievementHandler>();
			handler.achievementNameText.text = achievement.achievementName;
			handler.achievementDescText.text = achievement.achievementDesc;
			handler.achievementIconImage.sprite = achievement.achievementIcon;
			handler.achievement = achievement;
			achievementHandlers[i] = handler;

			i++;
		}
	}

    public void ShowAchievement(int achievementID) {
		GameObject achievementObj = Instantiate(achievementPrefab, achievementContainer);
		AchievementHandler handler = achievementObj.GetComponent<AchievementHandler>();

		foreach(Achievement achievement in AchievementDatabase.GetAllAchievements()) {
			if(achievement.achievementID == achievementID) {
				handler.achievementNameText.text = achievement.achievementName;
				handler.achievementDescText.text = achievement.achievementDesc;
				handler.achievementIconImage.sprite = achievement.achievementIcon;
				break;
			}
		}

		Destroy(achievementObj, 7f);
	}

	public void SetAchievements(List<int> achievementIDs) {
		for(int i = 0; i < achievementIDs.Count; i++) {
			foreach(Achievement achievement in AchievementDatabase.GetAllAchievements()) {
				if(achievement.achievementID == achievementIDs[i]) {
					hasAchievements[achievement.achievementID] = true;
					break;
				}
			}

			foreach(AchievementHandler handler in achievementHandlers) {
				if(handler.achievement.achievementID == achievementIDs[i]) {
					handler.backgroundImage.color = Color.green;
				}
			}
		}
	}

	public void GetAchievement(int _achievementID) {
		if (_achievementID <= -1)
			return;
		if(!hasAchievements[_achievementID]) {
			ShowAchievement(_achievementID);

			foreach(Achievement achievement in AchievementDatabase.GetAllAchievements()) {
				if(achievement.achievementID == _achievementID) {
					hasAchievements[_achievementID] = true;
					break;
				}
			}

			foreach(AchievementHandler handler in achievementHandlers) { // Set the achievement background to green in the achievement UI
				if(handler.achievement.achievementID == _achievementID) {
					handler.backgroundImage.color = Color.green;
				}
			}
		}
	}

	public List<int> ObtainedAchievements() {
		List<int> a = new List<int>();
		for(int i = 0; i < hasAchievements.Length; i++) {
			if(hasAchievements[i]) {
				a.Add(i);
			}
		}
		return a;
	}
}