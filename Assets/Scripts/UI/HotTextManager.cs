using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotTextManager : MonoBehaviour
{
    public static HotTextManager Instance;
    Dictionary<string, HotTextInfo> hotTexts = new Dictionary<string, HotTextInfo>();
    Dictionary<string, HotTextListItem> hotTextListItems = new Dictionary<string, HotTextListItem>();
    [SerializeField] Transform hotTextHolder;
    [SerializeField] GameObject hotTextObject;

    bool queueUIReload;

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

    void Update ()
    {
        if (queueUIReload)
        {
            queueUIReload = false;

            ReloadUI();
        }
    }

    public void UpdateHotText (HotTextInfo info)
    {
        if (hotTextListItems.ContainsKey(info.id))
        {
            hotTextListItems[info.id].Setup(info);
        } else
        {
            AddHotText(info);
        }
    }

    public void AddHotText (HotTextInfo info)
    {
        if (!hotTexts.ContainsKey(info.id))
        {
            hotTexts.Add(info.id, info);
            QueueUIReload();
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
            QueueUIReload();
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
        QueueUIReload();
    }

    void QueueUIReload ()
    {
        queueUIReload = true;
    }

    void ReloadUI ()
    {
        List<HotTextInfo> hottexts = new List<HotTextInfo>();

        foreach (string id in hotTexts.Keys)
        {
            HotTextInfo.Priority priority = hotTexts[id].priority;

            if (hottexts.Count > 0) {
                for (int i = 0; i < hottexts.Count; i++)
                {
                    if (hottexts[i].priority == priority)
                    {
                        hottexts.Insert(i, hotTexts[id]);
                        break;
                    } else if (hottexts[i].priority > priority)
                    {
                        hottexts.Insert(i, hotTexts[id]);
                        break;
                    } else if (i == hottexts.Count - 1)
                    {
                        hottexts.Add(hotTexts[id]);
                        break;
                    }
                }
            } else
            {
                hottexts.Add(hotTexts[id]);
            }
        }

        Debug.Log(hottexts.Count);

        for (int i = 0; i < hotTextHolder.childCount; i++)
        {
            GameObject child =  hotTextHolder.GetChild(i).gameObject;
            Destroy(child);
            hotTextListItems.Remove(child.GetComponent<HotTextListItem>().hotText.id);
        }

        for (int i = 0; i < hottexts.Count; i++)
        {
            GameObject hotTextListItem = Instantiate(hotTextObject, hotTextHolder);
            if (hottexts[i] == null)
            {
                Debug.Log("hot text is null");
            };
            HotTextListItem listItem =  hotTextListItem.GetComponent<HotTextListItem>();
            listItem.Setup(hottexts[i]);
            listItem.PlayAnimation();
            hotTextListItems.Add(hottexts[i].id, listItem);
        }
    }
}

[System.Serializable]
public class HotTextInfo
{
    public string text;
    public KeyCode key = KeyCode.None;
    public enum Priority { UseItem, AltUseItem, Build, Rotate, Pickup, Interact, Open };
    public Priority priority;
    public string id;
    public bool blocked; //If the usual input for this hotkey is "blocked" and will not work (for example, while trying to deploy an autominer mid-air)

    public HotTextInfo (HotTextInfo newInfo)
    {
        this.text = newInfo.text;
        this.key = newInfo.key;
        this.priority = newInfo.priority;
        this.id = newInfo.id;
        this.blocked = newInfo.blocked;
    }

    public HotTextInfo (string text, KeyCode key, Priority priority, string id, bool blocked)
    {
        this.text = text;
        this.key = key;
        this.priority = priority;
        this.id = id;
        this.blocked = blocked;
    }

    public HotTextInfo (string text, KeyCode key, Priority priority, string id)
    {
        this.text = text;
        this.key = key;
        this.priority = priority;
        this.id = id;
        this.blocked = false;
    }
    public HotTextInfo (string text, Priority priority, string id)
    {
        this.text = text;
        this.priority = priority;
        this.id = id;
        this.blocked = false;
    }
}
