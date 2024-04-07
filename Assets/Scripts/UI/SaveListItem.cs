using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveListItem : MonoBehaviour
{
    const float deleteConfirmTime = 0.5f;
    int saveNum;
    [SerializeField] TMP_Text saveText;
    [SerializeField] Button loadSaveButton;
    [SerializeField] Button confirmDeleteButton;
    [SerializeField] Image confirmDeleteUnderlay;
    bool confirmingDelete;
    float confirmDeleteTimer;
    
    public void Setup (string saveName, int saveNumber)
    {
        saveText.text = saveName;
        saveNum = saveNumber;
    }

    public Button GetLoadSaveButton ()
    {
        return loadSaveButton;
    }

    public Button GetConfirmDeleteButton ()
    {
        return confirmDeleteButton;
    }

    public void OnConfirmDeletePressed ()
    {
        confirmingDelete = true;
    }

    void Update ()
    {
        if (!confirmingDelete)
            return;

        if (Input.GetMouseButton(0))
        {
            confirmingDelete = true;
            confirmDeleteTimer += Time.deltaTime;

            confirmDeleteUnderlay.fillAmount = 0.15f + (confirmDeleteTimer / deleteConfirmTime) * 0.85f;

            if (confirmDeleteTimer > deleteConfirmTime)
            {
                MenuSaveManager.Instance.DeleteSave(saveNum);
            }
        }
        else
        {
            confirmingDelete = false;
            confirmDeleteTimer = 0f;
            confirmDeleteUnderlay.fillAmount = 0;
        }
    }

    public int GetSaveNumber ()
    {
        return saveNum;
    }
}
