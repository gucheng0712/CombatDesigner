
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CombatDesigner.EditorTool
{
    public class ViewBase
    {
        public Rect viewRect;

        protected ChainGraph graph;
        protected GUISkin skin;
        protected string viewTitle;

        public ViewBase(string title)
        {
            viewTitle = title;
            GetEditorSkin();
        }

        public virtual void UpdateView(Rect editorRect, Rect percentageRect, Event e, ChainGraph graph)
        {
            // set the current graph
            this.graph = graph;

            if (skin == null)
            {
                GetEditorSkin();
                return;
            }

            // update viewTitle
            viewTitle = (graph != null) ? graph.name : "No Graph";

            // Update ViewRect
            viewRect = new Rect(editorRect.x * percentageRect.x, editorRect.y * percentageRect.y, editorRect.width * percentageRect.width, editorRect.height * percentageRect.height);
        }

        protected virtual void ProcessEvent(Event e)
        {
        }

        void GetEditorSkin()
        {
            skin = (GUISkin)Resources.Load("GUISkins/EditorSkins/NodeEditorGUISkin");
        }

        protected void DrawHeader(string title, float zoom)
        {
            GUIStyle headerStyle = new GUIStyle("textarea");

            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.font = new GUIStyle(EditorStyles.boldLabel).font;

            headerStyle.fixedHeight = 30;
            GUI.Label(new Rect(viewRect.x, viewRect.y, viewRect.width * zoom, 30), title, headerStyle);
        }
    }
}
#endif