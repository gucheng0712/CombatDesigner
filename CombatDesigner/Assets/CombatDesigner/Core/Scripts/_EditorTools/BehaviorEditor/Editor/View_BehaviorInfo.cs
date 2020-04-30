
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using CombatDesigner;
using UnityEditor;
using UnityEngine;
namespace CombatDesigner.EditorTool
{
    public class View_BehaviorInfo
    {
        int timeLineTab;
        Vector2 actionScrollPos = new Vector2();
        private int maxLength = 100;
        float attackEndFrame;
        public void Init()
        {
            if (BehaviorEditorWindow.win == null) return;
        }
        public void DrawBehaviorInfoToolBar()
        {
            // tool bar for the behavior action and attackinfos
            timeLineTab = GUILayout.Toolbar(timeLineTab, new string[] { "BehaviorAction", "AttackInfo" });

            switch (timeLineTab)
            {
                case 0:
                    BehaviorActionFrameDetail();
                    break;
                case 1:
                    AttackInfoFrameDetail();
                    break;
            }
        }

        /// <summary>
        /// The BehaviorAction Frame Range List which will be shown under the timeline
        /// </summary>
        void BehaviorActionFrameDetail()
        {
            if (BehaviorEditorWindow.win.selectedBehavior == null || BehaviorEditorWindow.win.selectedBehavior.behaviorActions == null)
            {
                return;
            }
            actionScrollPos = GUILayout.BeginScrollView(actionScrollPos);

            if (BehaviorEditorWindow.win.selectedBehavior.behaviorActions.Count > 0)
            {
                foreach (IBehaviorAction action in BehaviorEditorWindow.win.selectedBehavior.behaviorActions)
                {
                    GUILayout.BeginVertical(GUI.skin.box);
                    GUILayout.BeginHorizontal();

                    GUILayout.Label(action.ToString().Replace("CombatDesigner.", ""));
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("FrameRange");
                    action.startFrame = EditorGUILayout.IntField((int)action.startFrame, GUILayout.Width(30));
                    action.startFrame = Mathf.Max(action.startFrame, 0);
                    GUILayout.Label(" ~ ");
                    action.endFrame = EditorGUILayout.IntField((int)action.endFrame, GUILayout.Width(30));
                    action.endFrame = Mathf.Clamp(action.endFrame, 0, BehaviorEditorWindow.win.timeline.totalStateFrames);


                    GUILayout.EndHorizontal();
                    EditorGUILayout.MinMaxSlider(ref action.startFrame, ref action.endFrame, 0, BehaviorEditorWindow.win.timeline.totalStateFrames);
                    action.startFrame = Mathf.Floor(action.startFrame);
                    action.endFrame = Mathf.Floor(action.endFrame);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
            }

            GUILayout.EndScrollView();
        }

        void AttackInfoFrameDetail()
        {
            if (BehaviorEditorWindow.win.selectedBehavior == null || BehaviorEditorWindow.win.selectedBehavior.attackInfos == null)
            {
                return;
            }
            actionScrollPos = GUILayout.BeginScrollView(actionScrollPos);

            int index = 0;
            foreach (BehaviorAttack atkInfo in BehaviorEditorWindow.win.selectedBehavior.attackInfos)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Atk Index " + (index++).ToString(), "Box");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                maxLength = (int)Mathf.Max(atkInfo.frameInfo.startFrame, maxLength);
                attackEndFrame = atkInfo.frameInfo.startFrame + atkInfo.frameInfo.length;
                EditorGUILayout.MinMaxSlider(ref atkInfo.frameInfo.startFrame, ref attackEndFrame, 0, maxLength);
                atkInfo.frameInfo.startFrame = Mathf.Floor(atkInfo.frameInfo.startFrame);
                atkInfo.frameInfo.length = Mathf.Floor(attackEndFrame) - atkInfo.frameInfo.startFrame;
                GUILayout.Label("Atk Start Frame: " + atkInfo.frameInfo.startFrame.ToString());
                GUILayout.BeginHorizontal();
                GUILayout.Label("Current Atk Length: " + atkInfo.frameInfo.length.ToString());
                GUILayout.FlexibleSpace();
                GUILayout.Label("Atk Max Length: ");
                maxLength = EditorGUILayout.IntField(maxLength, GUILayout.Width(30));
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            GUILayout.EndScrollView();
        }

    }
}
#endif