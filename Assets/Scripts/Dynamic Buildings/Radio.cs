using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour, IItemSaveable, IInteractable, IHotText
{

    [SerializeField] ItemHandler handler;
    [SerializeField] string saveID;
    [SerializeField] GameObject slider;
    [SerializeField] Vector2 sliderMinMax;
    [SerializeField] AudioSource songAudio;
    [SerializeField] AudioSource noiseAudio;
    [SerializeField] AudioClip casetteSound;
    [SerializeField] Song[] availiableSongs;
    Animator anim;
    int currentSong = -1;

    SettingsManager settingsManager;

    void Awake ()
    { // The radio ignores pitch
        settingsManager = FindObjectOfType<SettingsManager>();
        anim = GetComponent<Animator>();

        SetSong(-1);
    }

    public void Interact ()
    {
        ChangeSong();
        AchievementManager.Instance.GetAchievement(11);
    }

    public void ChangeSong ()
    {
        SetSong(currentSong + 1);
    }

    public void SetSong (int song)
    {
        currentSong = song;

        if (currentSong >= availiableSongs.Length)
        {
            currentSong = -1;
        }

        if (currentSong >= 0)
        {
            SongSwitched();
        }
        else
        {
            StopSong();
        }

        UpdateGraphics();
    }

    void SongSwitched ()
    {
        songAudio.clip = casetteSound;
        songAudio.Play();
        Invoke("PlaySong", casetteSound.length);
    }

    void PlaySong ()
    {
        noiseAudio.Play();
        songAudio.clip = CurrentSongClip();
        songAudio.Play();
    }

    void StopSong ()
    {
        songAudio.Stop();
        noiseAudio.Stop();
    }

    void UpdateGraphics ()
    {
        Vector3 newPos = slider.transform.localPosition;
        newPos.y = Mathf.Lerp(sliderMinMax.x, sliderMinMax.y, (float)(currentSong + 1) / (float)availiableSongs.Length);
        slider.transform.localPosition = newPos;
        if (currentSong == -1)
        {
            anim.SetBool("playing", false);
        }
        else
        {
            anim.SetBool("playing", true);
            //anim.playbackTime = 0;
            anim.SetFloat("bpm", availiableSongs[currentSong].bpm);
        }
    }

    AudioClip CurrentSongClip ()
    {
        if (currentSong == -1)
        {
            return null;
        }
        else
        {
            return availiableSongs[currentSong].clip;
        }
    }

    public void GetData (out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
    {
        ItemSaveData newData = new ItemSaveData();
        ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.GetIntegerID(saveID));

        newData.num = currentSong;

        data = newData;
        objData = newObjData;
        dontSave = false;
    }

    public void SetData (ItemSaveData data, ObjectSaveData objData)
    {
        SetSong(data.num);
    }

    void IHotText.HideHotText ()
    {
        HotTextManager.Instance.RemoveHotText(new HotTextInfo("", KeyCode.F, HotTextInfo.Priority.Open, "radioSwitch"));
    }

    void IHotText.ShowHotText ()
    {
        string infoText = ((currentSong + 1) >= availiableSongs.Length) ? "Turn Off" : (currentSong == -1) ? "Turn On" : "Next Song";
        HotTextManager.Instance.ReplaceHotText(new HotTextInfo(infoText, KeyCode.F, HotTextInfo.Priority.Open, "radioSwitch"));
    }

    void IHotText.UpdateHotText ()
    {
        string infoText = ((currentSong + 1) >= availiableSongs.Length) ? "Turn Off" : (currentSong == -1) ? "Turn On" : "Next Song";
        HotTextManager.Instance.UpdateHotText(new HotTextInfo(infoText, KeyCode.F, HotTextInfo.Priority.Open, "radioSwitch"));
    }
}

[System.Serializable]
public class Song
{
    public string name;
    public AudioClip clip;
    public float bpm;
}