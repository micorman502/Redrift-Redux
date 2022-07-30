using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialTutorialUIElement : MonoBehaviour
{
    [SerializeField] KeyCode key;
    // Start is called before the first frame update
    void Start()
    {
        if (PersistentData.Instance.loadingFromSave)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            Destroy(gameObject);
        }
    }
}
