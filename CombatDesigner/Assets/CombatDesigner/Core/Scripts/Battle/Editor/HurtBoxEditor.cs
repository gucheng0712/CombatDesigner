#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace CombatDesigner.EditorTool
{
    [CustomEditor(typeof(HurtBox))]
    public class HurtBoxEditor : Editor
    {
        HurtBox hurtBox;
        List<ActorBehavior> behaviorList = new List<ActorBehavior>();
        int behaviorIndex;

        List<string> behaviorStrs = new List<string>(); // a list of behavior names
        ActorBehavior behavior; // current behavior

        void OnEnable()
        {
            hurtBox = target as HurtBox;

            behaviorList = hurtBox.GetComponentInParent<ActorController>()?.model?.behaviors;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (behaviorList.IsNullOrEmpty()) 
                return;

            BuildBehavior();
            PreviewBehaviorHurtBox(behavior);
            SetupHurtBoxToBehavior(behavior);
        }

        /// <summary>
        /// Build the current behavior
        /// </summary>
        void BuildBehavior()
        {
            if (behaviorStrs.Count != behaviorList.Count)
            {
                // match the name of the behavior list
                foreach (var b in behaviorList)
                {
                    if (string.IsNullOrEmpty(b.name))
                    {
                        behaviorStrs.Add("No Name Behavior");
                    }
                    else
                    {
                        behaviorStrs.Add(b.name);
                    }
                }
            }

            // a enum popup of the behaviors's name
            behaviorIndex = EditorGUILayout.Popup("ActorBehavior: ", behaviorIndex, behaviorStrs.ToArray());
            // get the current behavior by index
            behavior = behaviorList[behaviorIndex];
        }

        private void SetupHurtBoxToBehavior(ActorBehavior behavior)
        {
            if (GUILayout.Button("Setup Hurtbox"))
            {
                if (behavior == null)
                {
                    Debug.LogError("Behavior is Null");
                    return;
                }

                behavior.hurtBoxInfo.position = hurtBox.transform.localPosition;
                behavior.hurtBoxInfo.rotation = Quaternion.Euler(hurtBox.transform.localEulerAngles);
                behavior.hurtBoxInfo.scale = hurtBox.transform.localScale;

                Debug.Log("Successfully Update" + behavior.name + "'s Hurbox Transformation ");
                EditorUtility.SetDirty(behavior);
                AssetDatabase.SaveAssets();
            }
        }

        void PreviewBehaviorHurtBox(ActorBehavior behavior)
        {
            if (GUILayout.Button("Preview Hurtbox"))
            {
                if (behavior == null)
                {
                    Debug.LogError("Behavior is Null");
                    return;
                }

                hurtBox.transform.localPosition = behavior.hurtBoxInfo.position;
                hurtBox.transform.localRotation = behavior.hurtBoxInfo.rotation;
                hurtBox.transform.localScale = behavior.hurtBoxInfo.scale;
            }
        }

    }
}
#endif