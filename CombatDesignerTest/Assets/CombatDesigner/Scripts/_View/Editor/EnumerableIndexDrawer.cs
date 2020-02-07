
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CombatDesigner;
using System.IO;

namespace CombatDesigner.EditorTool
{
    /// <summary>
    /// A drawer for EnumerableIndexAttribute 
    /// </summary>
    [CustomPropertyDrawer(typeof(EnumerableIndexAttribute))]
    public class EnumerableIndexDrawer : PropertyDrawer
    {
        public ActorModel model;
        public static string modelPath;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent labels)
        {
            EnumerableIndexAttribute enumIndex = attribute as EnumerableIndexAttribute;
            if (model == null)
            {
                foreach (var guid in AssetDatabase.FindAssets("SuzukaModel"))
                {
                    model = AssetDatabase.LoadAssetAtPath<ActorModel>(AssetDatabase.GUIDToAssetPath(guid));
                }
            }

            switch (enumIndex.itemType)
            {
                case EnumerableIndexAttribute.ItemType.Behavior:
                    property.intValue = EditorGUI.IntPopup(position, "Behavior ", property.intValue, model.GetBehaviorNames(), null);
                    break;
                case EnumerableIndexAttribute.ItemType.ChainBehavior:
                    property.intValue = EditorGUI.IntPopup(position, "ChainBehavior ", property.intValue, model.GetChainBehaviorNames(), null);
                    break;
                case EnumerableIndexAttribute.ItemType.Action:
                    property.intValue = EditorGUI.IntPopup(position, "Action ", property.intValue, model.GetBehaviorActionNames(), null);
                    break;
            }
        }
    }
}
#endif