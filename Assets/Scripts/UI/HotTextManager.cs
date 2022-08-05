using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotTextManager : MonoBehaviour
{
    public static HotTextManager Instance;
    Dictionary<string, HotTextInfo> hotTexts = new Dictionary<string, HotTextInfo>();
    [SerializeField] Transform hotTextHolder;
    [SerializeField] GameObject hotTextObject;

    private void Awake ()
    {
        if (Instance)
        {
            Debug.Log("There is already a HotTextManager in existence. Destroying this HotTextManager.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdateHotText (HotTextInfo info)
    {
        if (hotTexts.ContainsKey(info.id))
        {
            if (hotTexts[info.id].text != info.text)
            {
                hotTexts[info.id].text = info.text;
                hotTexts[info.id].priority = info.priority;

                ReloadUI();
            }
        } else
        {
            Debug.LogWarning("Cannot UpdateHotText, as there is no Hot Text with Id: " + info.id);
        }
    }

    public void AddHotText (HotTextInfo info)
    {
        if (!hotTexts.ContainsKey(info.id))
        {
            hotTexts.Add(info.id, info);
            ReloadUI();
        } else
        {
            Debug.LogWarning("Cannot AddHotText, as there is already a Hot Text with Id: " + info.id + ". Should ReplaceHotText be used instead?");
        }
    }

    public void RemoveHotText (HotTextInfo info)
    {
        RemoveHotText(info.id);
    }

    public void RemoveHotText (string id)
    {
        if (hotTexts.ContainsKey(id))
        {
            hotTexts.Remove(id);
            ReloadUI();
        }
        else
        {
            Debug.LogWarning("Cannot RemoveHotText, as there is no Hot Text with Id: " + id);
        }
    }

    public void ReplaceHotText (HotTextInfo info)
    {
        if (hotTexts.ContainsKey(info.id))
        {
            hotTexts.Remove(info.id);
        }
        hotTexts.Add(info.id, info);
        ReloadUI();
    }

    void ReloadUI ()
    {
        List<HotTextInfo> hottexts = new List<HotTextInfo>();

        foreach (string id in hotTexts.Keys)
        {
            int priority = hotTexts[id].priority;

            if (hottexts.Count > 0) {
                for (int i = 0; i < hottexts.Count; i++)
                {
                    if (hottexts[i].priority >= priority)
                    {
                        hottexts.Insert(i, hotTexts[id]);
                        break;
                    } else
                    {
                        hottexts.Insert(i+1, hotTexts[id]);
                        break;
                    }
                }
            } else
            {
                hottexts.Add(hotTexts[id]);
            }
        }

        for (int i = 0; i < hotTextHolder.childCount; i++)
        {
            Destroy(hotTextHolder.GetChild(i).gameObject);
        }

        for (int i = 0; i < hottexts.Count; i++)
        {
            GameObject hotTextListItem = Instantiate(hotTextObject, hotTextHolder);
            if (hottexts[i] == null)
            {
                Debug.Log("hot text is null");
            }
            hotTextListItem.GetComponent<HotTextListItem>().Setup(hottexts[i]);
        }
    }
}

[System.Serializable]
public class HotTextInfo
{
    public string text;
    public KeyCode key = KeyCode.None;
    public int priority;
    public string id;

    public HotTextInfo (string text, KeyCode key, int priority, string id)
    {
        this.text = text;
        this.key = key;
        this.priority = priority;
        this.id = id;
    }
    public HotTextInfo (string text, int priority, string id)
    {
        this.text = text;
        this.priority = priority;
        this.id = id;
    }
}
