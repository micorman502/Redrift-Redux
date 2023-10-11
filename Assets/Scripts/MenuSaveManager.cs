using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MenuSaveManager : MonoBehaviour {

	public static MenuSaveManager Instance;
	[SerializeField] GameObject saveListItem;
	[SerializeField] GameObject saveList;

	[SerializeField] Animator canvasAnim;

	[SerializeField] TMP_InputField saveNameInputField;
	[SerializeField] TMP_Dropdown saveDifficultyDropdown;
	[SerializeField] TMP_Dropdown saveModeDropdown;
	[SerializeField] TMP_Text difficultyBlurb;
	[SerializeField] TMP_Text modeBlurb;
	[SerializeField] TMP_Text saveErrorText;

	FileInfo[] info;

	MenuManager menuManager;

	List<GameObject> saveListItems = new List<GameObject>();

	string[] difficultyBlurbs = {"Keep your inventory items when you die and start with 2 crates.",
		"Keep your inventory items when you die.",
		"Lose your inventory items when you die."};
	string[] difficultyNames = {"Easy",
		"Normal",
		"Hard"};


	string[] modeBlurbs = {"Survive on the island.",
		"Health and hunger disabled, with an infinite supply of all items and the ability to fly."};
	string[] modeNames = {"Survival",
		"Creative"};

	void Awake() {
		if (Instance)
		{
			Debug.Log($"Duplicate Instance of '{this.GetType().Name}' exists on object '{gameObject.name}'. Destroying '{this.GetType().Name}'");
			Destroy(this);
			return;
		}
		Instance = this;

		menuManager = FindObjectOfType<MenuManager>();
	}

	void Start() {
		if(!Directory.Exists(Application.persistentDataPath + "/saves")) {
			Directory.CreateDirectory(Application.persistentDataPath + "/saves");
		}
		DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/saves");
		info = dir.GetFiles("*.*");

		RenderList();
		InitialiseDropdowns();

		OnChangeDifficulty();
		OnChangeMode();
	}

	void InitialiseDropdowns ()
    {
		List<TMP_Dropdown.OptionData> difficultyOptions = new List<TMP_Dropdown.OptionData>();
		saveDifficultyDropdown.ClearOptions();
		for (int i = 0; i < difficultyNames.Length; i++)
        {
			difficultyOptions.Add(new TMP_Dropdown.OptionData(difficultyNames[i]));
        }
		saveDifficultyDropdown.AddOptions(difficultyOptions);

		List<TMP_Dropdown.OptionData> modeOptions = new List<TMP_Dropdown.OptionData>();
		saveModeDropdown.ClearOptions();
		for (int i = 0; i < modeNames.Length; i++)
		{
			modeOptions.Add(new TMP_Dropdown.OptionData(modeNames[i]));
		}
		saveModeDropdown.AddOptions(modeOptions);
	}

	void RenderList() {
		saveNameInputField.text = "World " + (info.Length + 1);
		for(int i = 0; i < info.Length; i++) {
			int saveNum = i;
			GameObject go = Instantiate(saveListItem, saveList.transform);
			saveListItems.Add(go);
			SaveListItem item = go.GetComponent<SaveListItem>();
			string[] infoSplit = info[saveNum].Name.Split('.');
			item.Setup(infoSplit[infoSplit.Length - 2], saveNum);
			item.GetLoadSaveButton().onClick.AddListener(delegate { LoadSave(saveNum); });
			//item.GetConfirmDeleteButton().onClick.AddListener(delegate { DeleteSave(saveNum); });
		}
	}

	void ClearList() {
		foreach(GameObject go in saveListItems) {
			Destroy(go);
		}
		DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/saves");
		info = dir.GetFiles("*.*");
		saveListItems.Clear();
	}

	public void LoadSave(int saveNum) {
		PersistentData.Instance.loadingFromSave = true;
		PersistentData.Instance.saveToLoad = saveNum;
		menuManager.LoadScene("World");
	}

	public void DeleteSave(int saveNum) {
		foreach(GameObject go in saveListItems) {
			if(go.GetComponentInChildren<Text>().text == info[saveNum].Name) {
				Destroy(go);
				break;
			}
		}
		File.Delete(Application.persistentDataPath + "/saves/" + info[saveNum].Name);
		ClearList();
		RenderList();
	}

	public void OpenNewSaveMenu() {
		canvasAnim.SetTrigger("NewSaveMenuEnter");
		canvasAnim.SetTrigger("PlayMenuExit");
		Invoke("SelectSaveNameInputField", 0.1f); // Kinda hacky, but gets the job done :)
	}

	public void SelectSaveNameInputField() {
		saveNameInputField.Select();
		saveNameInputField.ActivateInputField();
	}

	public void OnChangeDifficulty() {
		difficultyBlurb.text = difficultyBlurbs[saveDifficultyDropdown.value];
	}

	public void OnChangeMode() {
		modeBlurb.text = modeBlurbs[saveModeDropdown.value];
	}

	public void CreateNewSave() {
		string saveName = saveNameInputField.text;

		if(string.IsNullOrEmpty(saveName)) {
			saveErrorText.text = "Please enter a valid name.";
			saveErrorText.gameObject.SetActive(true);
			return;
		} else {
			foreach(FileInfo f in info) {
				string[] infoSplit = f.Name.Split('.');
				if(infoSplit[infoSplit.Length - 2] == saveName) {
					saveErrorText.text = "Please enter a name that isn't taken.";
					saveErrorText.gameObject.SetActive(true);
					return;
				}
			}
		}
		try
		{
			FileStream newFile = File.Create(SaveManager.GetSavePath(saveName, false));
			newFile.Close();
		} catch (Exception e)
        {
			Debug.LogWarning("Could not create save named '" + saveName + "', due to exception '" + e.Message + "'.");
			if (saveErrorText)
			{
				saveErrorText.text = "Please enter a valid name.";
				saveErrorText.gameObject.SetActive(true);
			}
			return; 
		}

		PersistentData.Instance.newSaveName = saveNameInputField.text;
		PersistentData.Instance.difficulty = saveDifficultyDropdown.value;
		PersistentData.Instance.mode = saveModeDropdown.value;
		PersistentData.Instance.loadingFromSave = false;
		PersistentData.Instance.saveToLoad = info.Length;
		menuManager.LoadScene("World");
	}
}
