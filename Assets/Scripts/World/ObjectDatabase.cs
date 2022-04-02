using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDatabase : MonoBehaviour
{
    public static ObjectDatabase Instance;
    [SerializeField] ObjectRegister register;
    Dictionary<string, ObjectRegisterObject> idAcessibleObjects = new Dictionary<string, ObjectRegisterObject>();

    private void Awake ()
    {
        if (Instance)
        {
            Debug.Log("There is already an ObjectDatabase in existence. Deleting this ObjectDatabase.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Initialise();
    }

    void Initialise ()
    {
        for (int i = 0; i < register.objects.Length; i++)
        {
            register.objects[i].id = i;
            idAcessibleObjects.Add(register.objects[i].nameIdentifier, register.objects[i]);
        }
    }

    public GameObject[] GetAllObjects ()
    {
        GameObject[] objects = new GameObject[register.objects.Length];
        for (int i = 0; i < register.objects.Length; i++)
        {
            objects[i] = register.objects[i].prefab;
        }

        return objects;
    }

    public GameObject GetObject (int id)
    {
        return register.objects[id].prefab;
    }

    public GameObject GetObject (string id)
    {
        return idAcessibleObjects[id].prefab;
    }

    public int GetIntID (string id)
    {
        if (!idAcessibleObjects.ContainsKey(id))
        {
            Debug.Log("Nope on id: " + id);
        }
        return idAcessibleObjects[id].id;
    }
}
