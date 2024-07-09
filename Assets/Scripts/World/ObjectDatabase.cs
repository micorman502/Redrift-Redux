using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObjectDatabase : MonoBehaviour
{
    public static ObjectDatabase Instance { get; private set; }
    static ObjectRegister Register;
    [SerializeField] ObjectRegister register;
    static Dictionary<string, ObjectRegisterObject> idAcessibleObjects = new Dictionary<string, ObjectRegisterObject>();

    private void Awake ()
    {
        if (Instance)
        {
            Debug.Log("There is already an ObjectDatabase in existence. Deleting this ObjectDatabase.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Register = register;

        Initialise();
    }

    static void Initialise ()
    {
        for (int i = 0; i < Register.objects.Length; i++)
        {
            Register.objects[i].id = i;
            idAcessibleObjects.Add(Register.objects[i].nameIdentifier, Register.objects[i]);

            if (!Register.objects[i].prefab)
                continue;

            GameObject tempObject = Instantiate(Register.objects[i].prefab);
            tempObject.SetActive(false);
            IItemSaveable[] saveables = tempObject.GetComponents<IItemSaveable>();

            Destroy(tempObject);

            for (int s = 0; s < saveables.Length; s++)
            {
                string saveableName = saveables[s].GetSaveID(out bool dontSave);

                if (dontSave)
                    continue;

                Register.objects[i].nameIdentifier = saveableName;
            }
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(Register);
#endif
    }

    public static GameObject[] GetAllObjects ()
    {
        GameObject[] objects = new GameObject[Register.objects.Length];
        for (int i = 0; i < Register.objects.Length; i++)
        {
            objects[i] = Register.objects[i].prefab;
        }

        return objects;
    }

    public static GameObject GetObject (int id)
    {
        return Register.objects[id].prefab;
    }

    public static GameObject GetObject (string id)
    {
        return idAcessibleObjects[id].prefab;
    }

    public static int GetIntegerID (string stringId)
    {
        if (!idAcessibleObjects.ContainsKey(stringId))
        {
            Debug.Log("No object found for stringId '" + stringId + "'");
        }
        return idAcessibleObjects[stringId].id;
    }
}
