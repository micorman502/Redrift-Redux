using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotTextManager : MonoBehaviour
{
    public static HotTextManager Instance;
    Dictionary<string, HotTextInfo> hotText = new Dictionary<string, HotTextInfo>();
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

    public void AddHotText (HotTextInfo info)
    {
        if (!hotText.ContainsKey(info.id))
        {
            hotText.Add(info.id, info);
            ReloadUI();
        } else
        {
            Debug.LogWarning("Cannot AddHotText, as there is already a Hot Text with ID: " + info.id + ". Should ReplaceHotText be used instead?");
        }
    }

    public void RemoveHotText (HotTextInfo info)
    {
        RemoveHotText(info.id);
    }

    public void RemoveHotText (string id)
    {
        if (hotText.ContainsKey(id))
        {
            hotText.Remove(id);
            ReloadUI();
        }
        else
        {
            Debug.LogWarning("Cannot RemoveHotText, as there is no Hot Text with ID: " + id);
        }
    }

    public void ReplaceHotText (HotTextInfo info)
    {
        if (hotText.ContainsKey(info.id))
        {
            hotText.Remove(info.id);
        }
        hotText.Add(info.id, info);
        ReloadUI();
    }

    void ReloadUI ()
    {
        List<HotTextInfo> hottexts = new List<HotTextInfo>();

        foreach (string id in hotText.Keys)
        {
            int priority = hotText[id].priority;

            if (hottexts.Count > 0) {
                /*for (int i = 0; i < hottexts.Count; i++)
                {
                    if (hottexts[i].priority >= priority)
                    {
                        hottexts.Insert(i, hotText[id]);
                        break;
                    } else
                    {
                        hottexts.Insert(i+1, hotText[id]);
                        break;
                    }
                }*/ //Maybe i shouldn't comment it out, but wtf am i supposed to do?
            } else
            {
                hottexts.Add(hotText[id]);
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
