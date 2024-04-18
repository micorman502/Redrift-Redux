using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {

	public static PauseManager Instance { get; private set; }
	PlayerController player;

	float originalTimeScale;

	[HideInInspector] public bool paused = false;

	[SerializeField] Animator canvasAnim;

    void Awake ()
    {
		if (Instance)
		{
			Destroy(this);
			return;
		}

		Instance = this;
	}

    void Start() {
		Time.timeScale = 1;
		originalTimeScale = Time.timeScale;
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}

	public void SetPauseState (bool isPaused)
    {
		if (isPaused)
        {
			Pause();
        } else
        {
			Resume();
        }
    }

	public void Pause() {
		if (paused)
			return;

		paused = true;

		canvasAnim.SetTrigger("PauseMenuEnter");

		Time.timeScale = 0f;
		LookLocker.AddUnlockingObject(this);
	}

	public void Resume() {
		if (!paused)
			return;

		paused = false;

		if (canvasAnim.GetCurrentAnimatorStateInfo(0).IsName("PauseMenuEnter")) {
			canvasAnim.SetTrigger("PauseMenuExit");
		}
		if(canvasAnim.GetCurrentAnimatorStateInfo(1).IsName("SettingsMenuEnter")) {
			canvasAnim.SetTrigger("SettingsMenuExit");
		}
		if(canvasAnim.GetCurrentAnimatorStateInfo(2).IsName("AchievementMenuEnter")) {
			canvasAnim.SetTrigger("AchievementMenuExit");
		}
		if(canvasAnim.GetCurrentAnimatorStateInfo(3).IsName("HelpMenuEnter")) {
			canvasAnim.SetTrigger("HelpMenuExit");
        }

        Time.timeScale = originalTimeScale;
		LookLocker.RemoveUnlockingObject(this);
	}

    public void Menu() {
		if (!paused)
			return;

		paused = false;

		Time.timeScale = originalTimeScale;
		// Stop this from unlocking the mouse, but also make sure the default state of MouseLocked is false.
		LookLocker.RemoveUnlockingObject(this);
		LookLocker.MouseLocked = false;

		Destroy(PersistentData.Instance.gameObject); // Destroy the persistent data object before we return to the menu, otherwise the game will save with a blank save name.
		SceneManager.LoadScene("Menu");
	}
}
