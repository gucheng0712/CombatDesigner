using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class SettingUtils
{

#if UNITY_EDITOR
    public static void AddLayer(string layerName)
    {
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

        if ((asset != null) && (asset.Length > 0))
        {
            SerializedObject serializedObject = new SerializedObject(asset[0]);
            SerializedProperty layers = serializedObject.FindProperty("layers");

            for (int i = 0; i < layers.arraySize; ++i)
            {
                if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
                {
                    return;     // Layer already present, nothing to do.
                }
            }

            //  layers.InsertArrayElementAtIndex(0);
            //  layers.GetArrayElementAtIndex(0).stringValue = layerName;

            for (int i = 0; i < layers.arraySize; i++)
            {
                if (layers.GetArrayElementAtIndex(i).stringValue == "")
                {
                    // layers.InsertArrayElementAtIndex(i);
                    layers.GetArrayElementAtIndex(i).stringValue = layerName;
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                    if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
                    {
                        return;     // to avoid unity locked layer
                    }
                }
            }
        }
    }

    public static void AddTag(string tagName)
    {
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if ((asset != null) && (asset.Length > 0))
        {
            SerializedObject serializedObject = new SerializedObject(asset[0]);
            SerializedProperty tags = serializedObject.FindProperty("tags");

            for (int i = 0; i < tags.arraySize; ++i)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue == tagName)
                {
                    return;     // Tag already present, nothing to do.
                }
            }

            tags.InsertArrayElementAtIndex(tags.arraySize);
            tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tagName;
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
#endif
}














