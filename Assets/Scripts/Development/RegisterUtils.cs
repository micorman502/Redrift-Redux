using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace RollingTundra.DataUtils
{
    public class RegisterUtils
    {
#if UNITY_EDITOR
        // Thanks to user "spiney199" from https://forum.unity.com/threads/populating-an-array-with-scriptable-objects-directly-through-script.1147094/
        public static List<T> FindAllScriptableObjectsOfType<T> (string folder = "Assets") where T : ScriptableObject
        {
            return AssetDatabase.FindAssets($"t:{typeof(T).FullName}", new[] { folder })
                .Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid)))
                .Cast<T>().ToList();
        }

        public static List<T> AppendScriptableObjectsToList<T> (IList<T> baseList) where T : ScriptableObject
        {
            IList<T> allSOs = FindAllScriptableObjectsOfType<T>();
            List<T> newList = new List<T>();

            // Transfer over original ScriptableObjects.
            newList.AddRange(baseList);

            // Add in new ScriptableObjects, ensuring that there are no duplicates.
            for (int i = 0; i < allSOs.Count; i++)
            {
                if (baseList.Contains(allSOs[i]))
                    continue;

                newList.Add(allSOs[i]);
            }

            return newList;
        }
#endif

        public static void PopulateRegister<T> (ScriptableObject register, ref T[] registerArray) where T : ScriptableObject
        {
#if UNITY_EDITOR
            registerArray = AppendScriptableObjectsToList(registerArray).ToArray();
            EditorUtility.SetDirty(register);
#endif
        }
    }
}
