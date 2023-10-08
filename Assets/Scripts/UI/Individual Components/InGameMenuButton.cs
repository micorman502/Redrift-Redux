using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuButton : MonoBehaviour
{
    public void LoadToMenu ()
    {
        SaveManager.Instance.SaveGame();
        PauseManager.Instance.Menu();
    }
}
