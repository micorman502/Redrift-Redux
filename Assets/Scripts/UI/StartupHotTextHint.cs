using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupHotTextHint : MonoBehaviour
{
    [SerializeField] string hintText;
    [SerializeField] KeyCode hintKey;
    float spawnTime;

    void Start ()
    {
        HotTextManager.Instance.AddHotText(new HotTextInfo(hintText, hintKey, HotTextInfo.Priority.UseItem, "startupHint"));

        spawnTime = Time.time;
    }

    void Update ()
    {
        if (Time.time > spawnTime + 3f)
        {
            HotTextManager.Instance.RemoveHotText(new HotTextInfo("", hintKey, HotTextInfo.Priority.UseItem, "startupHint"));
            Destroy(gameObject);
        }
    }
}
