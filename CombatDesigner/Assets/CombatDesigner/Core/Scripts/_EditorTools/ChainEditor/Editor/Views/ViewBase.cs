
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CombatDesigner.EditorTool
{
    /// <summary>
    /// A base class of view in ChainEditor
    /// </summary>
    public class ViewBase
    {
        /// <summary>
        /// The rect of the window
        /// </summary>
        public Rect viewRect;

        /// <summary>
        /// The graph of current window
        /// </summary>
        protected ChainGraph graph;

        /// <summary>
        /// GUI skin
        /// </summary>
        protected GUISkin skin;

        /// <summary>
        /// The title name of the view window
        /// </summary>
        protected string viewTitle;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title"></param>
        public ViewBase(string title)
        {
            viewTitle = title;
            GetEditorSkin();
        }

        /// <summary>
        /// A virtual method to update the visualization
        /// </summary>
        /// <param name="editorRect"></param>
        /// <param name="percentageRect"></param>
        /// <param name="e"></param>
        /// <param name="graph"></param>
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

        /// <summary>
        /// A virtual mehtod to update event 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void ProcessEvent(Event e)
        {
        }

        /// <summary>
        /// Get the skin of the chain editor 
        /// </summary>
        void GetEditorSkin()
        {
            skin = (GUISkin)Resources.Load("GUISkins/EditorSkins/NodeEditorGUISkin");
        }

        /// <summary>
        /// Draw the header of the view space
        /// </summary>
        /// <param name="title"></param>
        /// <param name="zoom"></param>
        protected void DrawHeader(string title, float zoom)
        {
            GUIStyle headerStyle = new GUIStyle();

            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.font = new GUIStyle(EditorStyles.boldLabel).font;
            headerStyle.normal.textColor = Color.white;
            headerStyle.fontSize = 20;

            headerStyle.fixedHeight = 40;
            GUI.Label(new Rect(viewRect.x, viewRect.y+30, viewRect.width * zoom, 40), title, headerStyle);
        }
    }
}
#endif