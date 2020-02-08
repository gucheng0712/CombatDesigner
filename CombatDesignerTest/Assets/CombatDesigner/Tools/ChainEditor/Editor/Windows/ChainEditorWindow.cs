
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace CombatDesigner.EditorTool
{
    public class ChainEditorWindow : EditorWindow
    {
        /// <summary>
        /// The graph of the chain editor
        /// </summary>
        public ChainGraph graph;

        /// <summary>
        /// Property Space
        /// </summary>
        public View_Property _propertyView;
        /// <summary>
        /// Nodes Space
        /// </summary>
        public View_NodeWorkspace _workView;

        /// <summary>
        /// Chain Editor Window
        /// </summary>
        static ChainEditorWindow _win;

        /// <summary>
        ///  the percentage of the node workspace size
        /// </summary>
        float _viewPercentage = 0.75f;

        /// <summary>
        /// Open the ChainEditor window
        /// </summary>
        public static void Open()
        {
            ChainEditorWindow winTemp = GetWindow<ChainEditorWindow>("Behavior-Tree Editor");
            _win = winTemp;
            CreateViews();
        }

        /// <summary>
        /// Draw the views
        /// </summary>
        void OnGUI()
        {
            if (_propertyView == null)
            {
                CreateViews();
                return;
            }

            // update events
            Event e = Event.current;

           

            // update views
            _workView.UpdateView(position, new Rect(0, 0, _viewPercentage, 1f), e, graph);
            _propertyView.UpdateView(new Rect(position.width, position.y, position.width, position.height),
                                    new Rect(_viewPercentage, 0, 1 - _viewPercentage, 1f), e, graph);

            ProecessEvents(e);
            Repaint();// make it to update constantly
        }

        /// <summary>
        /// A static method to create the eidtor window's view spaces.
        /// </summary>
        static void CreateViews()
        {
            if (_win != null)
            {
                _win._workView = new View_NodeWorkspace();
                _win._propertyView = new View_Property();
            }
            else
            {
                _win = GetWindow<ChainEditorWindow>(true, "Behavior-Tree Editor");
            }
        }

        /// <summary>
        /// Update the events
        /// </summary>
        /// <param name="e"></param>
        void ProecessEvents(Event e)
        {
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftArrow)
            {
                _viewPercentage -= 0.01f;
            }
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.RightArrow)
            {
                _viewPercentage += 0.01f;
            }
        }
    }
}
#endif