using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace CombatDesigner.EditorTool
{
    public class View_MenuBar : ViewBase
    {

        public View_MenuBar() : base("MenuBar")
        {

        }
        public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, ChainGraph graph)
        {
            base.UpdateView(editorRect, percentageRect, e, graph);


            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            DrawToolStrip();
            GUILayout.EndHorizontal();

        }

        protected override void ProcessEvent(Event e)
        {
            base.ProcessEvent(e);
        }

        void DrawToolStrip()
        {
            if (GUILayout.Button(" File ", EditorStyles.toolbarDropDown))
            {
                GenericMenu toolsMenu = new GenericMenu();
                toolsMenu.AddItem(new GUIContent("Create Graph"), false, OnFile_CreateGrapph);
                toolsMenu.AddItem(new GUIContent("Load Graph"), false, OnFile_LoadGraph);
                toolsMenu.AddItem(new GUIContent("Save Graph"),false,OnFile_SaveGraph);
                toolsMenu.AddSeparator("");
                
                if (graph != null)
                {
                    toolsMenu.AddItem(new GUIContent("Unload Graph"), false, OnFile_UnloadGraph);
                }
                else
                {
                    toolsMenu.AddDisabledItem(new GUIContent("Unload Graph"));
                }
                toolsMenu.DropDown(new Rect(0, 20, 0, 0));
                EditorGUIUtility.ExitGUI();
            }
            if (GUILayout.Button(" Edit ", EditorStyles.toolbarDropDown))
            {
                GenericMenu toolsMenu = new GenericMenu();
                toolsMenu.AddItem(new GUIContent("Edit Graph Model"), false, OnEdit_EditGraphModel);

                toolsMenu.AddSeparator("");

                toolsMenu.AddItem(new GUIContent("Create Node/Chain Node"), false, OnCreate_ChainNode);
                if (graph.nodes.OfType<RootBehaviorNode>().Any())
                {
                    toolsMenu.AddDisabledItem(new GUIContent("Create Node/Root Chain Node"));
                }
                else
                {
                    toolsMenu.AddItem(new GUIContent("Create Node/Root Chain Node"), false, OnCreate_RootChainNode);
                }

                toolsMenu.DropDown(new Rect(45, 20, 0, 0));
                EditorGUIUtility.ExitGUI();
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Help", EditorStyles.toolbarDropDown))
            {
                GenericMenu toolsMenu = new GenericMenu();

                toolsMenu.AddItem(new GUIContent("Documentation"), false, OnDocumentationBtnClick);
                // Offset menu from right of editor window
                toolsMenu.DropDown(new Rect(Screen.width - 42, 20, 0, 0));
                EditorGUIUtility.ExitGUI();
            }
        }

        private void OnCreate_RootChainNode()
        {
            ChainEditorUtilities.CreateNode(graph, NodeType.RootBehaviorNode, new Vector2(Screen.width / 3, Screen.height / 3));
        }

        private void OnCreate_ChainNode()
        {
            ChainEditorUtilities.CreateNode(graph, NodeType.ChainBehaviorNode, new Vector2(Screen.width/3,Screen.height/3));
        }

        private void OnFile_SaveGraph()
        {
            ChainEditorUtilities.UpdateGraphToModel(graph);
        }

        private void OnEdit_EditGraphModel()
        {
            EditModelPopUp.Open();
        }

        private void OnFile_UnloadGraph()
        {
            ChainEditorUtilities.UnloadGraph();
        }

        private void OnFile_LoadGraph()
        {
            ChainEditorUtilities.LoadGraph();
        }

        private void OnFile_CreateGrapph()
        {
            GraphCreationPopUp.Open();
        }

        void OnDocumentationBtnClick()
        {
            Help.BrowseURL("https://github.com/gucheng0712/CombatDesigner/wiki");
        }
    }

}














