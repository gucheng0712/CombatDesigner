using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace CombatDesigner.EditorTool
{
    public class EditModelPopUp : EditorWindow
    {
        static EditModelPopUp popup;
        /// <summary>
        /// model of the chain editor
        /// </summary>
        public static ActorModel model;

        /// <summary>
        /// Open a popup window
        /// </summary>
        public static void Open()
        {
            popup = GetWindow<EditModelPopUp>(true, "Edit Model Popup");
            popup.maxSize = popup.minSize = new Vector2(300, 120);
            model = ChainEditorWindow._win.graph.model;
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
            EditorGUILayout.LabelField("Change Graph Model:", EditorStyles.boldLabel);
            //name = EditorGUILayout.TextField("Enter Name: ", name);

            model = (ActorModel)EditorGUILayout.ObjectField("Model", model, typeof(ActorModel), true);

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply Model", GUILayout.Height(40)))
            {
                if (model != null)
                {
                    ChainEditorWindow._win.graph.model = model;
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











