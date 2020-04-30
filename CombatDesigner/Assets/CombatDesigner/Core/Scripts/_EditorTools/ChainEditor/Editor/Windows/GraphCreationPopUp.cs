
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// deprecated
/// </summary>

namespace CombatDesigner.EditorTool
{
    public class GraphCreationPopUp : EditorWindow
    {
        static GraphCreationPopUp popup;
        /// <summary>
        /// model of the chain editor
        /// </summary>
        public ActorModel model;

        /// <summary>
        /// Open a popup window
        /// </summary>
        public static void Open()
        {
            popup = GetWindow<GraphCreationPopUp>(true, "Graph Creation Popup");
            popup.maxSize = popup.minSize = new Vector2(300, 130);
        }

        /// <summary>
        /// Draw popup window
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("Create New Graph:", EditorStyles.boldLabel);
            //name = EditorGUILayout.TextField("Enter Name: ", name);
            model = (ActorModel)EditorGUILayout.ObjectField("Model", model, typeof(ActorModel), true);

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Graph", GUILayout.Height(40)))
            {
                if (model != null)
                {
                    ChainEditorUtilities.CreateNewGraph(model);
                    popup.Close();
                }
                else
                {
                    EditorUtility.DisplayDialog("Node Message:", "Please select a valid ActorModel!", "OK");
                }
            }
            if (GUILayout.Button("Cancel", GUILayout.Height(40)))
            {
                popup.Close();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
        }

    }
}
#endif