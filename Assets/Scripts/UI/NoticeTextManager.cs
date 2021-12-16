using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeTextManager : MonoBehaviour
{
    [SerializeField] Text noticeText;
    List<NoticeTextPriority> textPriority = new List<NoticeTextPriority>();
    public static NoticeTextManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void AddNoticeText (string text, int priority)
    {
        textPriority.Add(new NoticeTextPriority(text, priority));
    }

    public void AddNoticeText(string text, int priority, float duration)
    {
        textPriority.Add(new NoticeTextPriority(text, priority, duration));
    }

    void LateUpdate()
    {
        string text = "";
        int currentPriority = -1;
        List<NoticeTextPriority> tempTextPriority = new List<NoticeTextPriority>();

        for (int i = 0; i < textPriority.Count; i++)
        {
            if (textPriority[i].priority > currentPriority)
            {
                currentPriority = textPriority[i].priority;
                text = textPriority[i].text;

                if (Time.time <= textPriority[i].timeAdded + textPriority[i].duration)
                {
                    tempTextPriority.Add(textPriority[i]);
                }
            }
        }

        noticeText.text = text;
        textPriority = tempTextPriority;
    }
}

[System.Serializable]
public class NoticeTextPriority
{
    public NoticeTextPriority (string _text, int _priority)
    {
        text = _text;
        priority = _priority;
        duration = 0;
        timeAdded = Time.time;
    }

    public NoticeTextPriority(string _text, int _priority, float _duration)
    {
        text = _text;
        priority = _priority;
        duration = _duration;
        timeAdded = Time.time;
    }

    public string text;
    public int priority;
    public float duration;
    public float timeAdded;
}
