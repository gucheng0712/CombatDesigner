
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using CombatDesigner;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class View_BehaviorSelector
{
    ActorController actorController;

    int selectedIndex = -1;
    int prevSelectedIndex = -2;

    Vector2 behaviorScrollPos = new Vector2();

    public void Init()
    {
        if (BehaviorEditorWindow.win == null) return;
        if (BehaviorEditorWindow.win.actorController != null)
            actorController = BehaviorEditorWindow.win.actorController;
    }

    public void DrawBehaviorList()
    {
        if (BehaviorEditorWindow.selectedBehavior == null)
        {
            selectedIndex = -1;
        }

        behaviorScrollPos = EditorGUILayout.BeginScrollView(behaviorScrollPos);

        if (actorController != null)
        {
            List<ActorBehavior> behaviors = actorController.model.behaviors;
            if (behaviors != null)
            {
                for (int i = 0; i < behaviors.Count; i++)
                {
                    string behaviorName = behaviors[i].name;
                    if (string.IsNullOrEmpty(behaviorName))
                    {
                        behaviorName = "Need to Assign a Nname";
                    }
                    bool change = GUILayout.Toggle(selectedIndex == i, behaviorName, "Button", GUILayout.Height(30));
                    if (change == true)
                    {
                        selectedIndex = i;
                        if (selectedIndex != prevSelectedIndex)
                        {
                            // Selection.activeObject = actorController.model.behaviors[i];
                            prevSelectedIndex = selectedIndex;
                            BehaviorEditorWindow.selectedBehavior = behaviors[i];
                        }
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }
}
#endif