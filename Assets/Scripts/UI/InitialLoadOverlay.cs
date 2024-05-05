using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialLoadOverlay : MonoBehaviour
{
    [SerializeField] Image overlayImage;
    [SerializeField] float overlayDuration;
    Color baseColour;
    float loadTime;
    bool doEffect;
    // Start is called before the first frame update
    void Start ()
    {
        if (!PersistentData.Instance.loadingFromSave)
        {
            doEffect = true;
            baseColour = overlayImage.color;
            loadTime = Time.time;
        }
        else
        {
            overlayImage.gameObject.SetActive(false);
        }
    }

    private void Update ()
    {
        if (doEffect)
        {
            if (Time.time < loadTime + overlayDuration)
            {
                overlayImage.color = Color.Lerp(baseColour, Color.clear, (Time.unscaledTime - loadTime) / overlayDuration);
            }
            else
            {
                doEffect = false;
                enabled = false;
                overlayImage.gameObject.SetActive(false);
            }
        }
    }
}
