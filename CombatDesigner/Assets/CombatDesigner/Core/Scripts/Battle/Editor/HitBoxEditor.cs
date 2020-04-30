
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace CombatDesigner.EditorTool
{
    /// <summary>
    /// A custom editor for HitBox class
    /// </summary>
    [CustomEditor(typeof(HitBox))]
    public class HitBoxEditor : Editor
    {

        HitBox hitBox;

        List<ActorBehavior> behaviorList = new List<ActorBehavior>(); // a list of behaviors 
        int behaviorIndex; // the current index

        List<string> behaviorStrs = new List<string>(); // a list of behavior names
        List<string> attackInfoStrs = new List<string>(); //  a list of BehaviorAttack index in the current behavior
        ActorBehavior behavior; // current behavior
        int attackInfoIndex; //current BehaviorAttack Index

        private void OnEnable()
        {
            hitBox = target as HitBox;
            ActorController controller = hitBox.GetComponentInParent<ActorController>();

            behaviorList = controller?.model?.behaviors;
        }

        /// Draw Inspector GUI
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (behaviorList.IsNullOrEmpty())
                return;

            BuildBehavior();
            BuildBehaviorAttackInfo();
            PreviewHitBox();
            SetupHitBox();
        }

        /// <summary>
        /// Build the current behavior
        /// </summary>
        void BuildBehavior()
        {
            if (behaviorStrs.Count != behaviorList.Count)
            {
                behaviorStrs.Clear();
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

        /// <summary>
        /// Build the current behavior's BehaviorAttack
        /// </summary>
        void BuildBehaviorAttackInfo()
        {
            // match the index of the attackinfo list
            if (attackInfoStrs.Count != behavior.attackInfos.Count)
            {
                for (int i = 0; i < behavior.attackInfos.Count; i++)
                {
                    attackInfoStrs.Add((i).ToString());
                }
            }
            // a enum popup of the behaviors's BehaviorAttack index
            attackInfoIndex = EditorGUILayout.Popup("AttackInfo Index: ", attackInfoIndex, attackInfoStrs.ToArray());
        }

        void PreviewHitBox()
        {
            if (GUILayout.Button("Preview Hitbox"))
            {
                if (behavior == null && behavior.attackInfos != null)
                {
                    Debug.LogError("Behavior is Null");
                    return;
                }

                BehaviorAttack atk = behavior.attackInfos[attackInfoIndex];
                hitBox.transform.localPosition = atk.hitboxInfo.hitBoxPos;
                hitBox.transform.localScale = atk.hitboxInfo.hitBoxScale;
                hitBox.transform.localRotation = atk.hitboxInfo.hitBoxRot;
            }
        }
        /// <summary>
        /// A method to apply the transformation of the hitbox
        /// </summary>
        /// <param name="h"></param>
        void SetupHitBox()
        {
            if (GUILayout.Button("Apply Hitbox"))
            {
                if (behavior == null)
                {
                    Debug.LogError("Behavior is Null");
                    return;
                }

                BehaviorAttack atk = behavior.attackInfos[attackInfoIndex];
                atk.hitboxInfo.hitBoxPos = hitBox.transform.localPosition;
                atk.hitboxInfo.hitBoxScale = hitBox.transform.localScale;
                atk.hitboxInfo.hitBoxRot = Quaternion.Euler(hitBox.transform.localEulerAngles);

                Debug.Log("Successfully set Hitbox for " + behavior.name + "'s AttackInfo Index " + attackInfoIndex);
                EditorUtility.SetDirty(behavior);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
#endif