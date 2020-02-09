
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CombatDesigner;
using System;

namespace CombatDesigner.EditorTool
{
    /// <summary>    /// The Graph Class    /// </summary>
    [System.Serializable]
    public class ChainGraph : ScriptableObject
    {
        /// <summary>        /// The model reference of current graph        /// </summary>
        public ActorModel model;

        /// <summary>        /// The name of the graph        /// </summary>
        public string graphName = "New Graph";

        /// <summary>        /// Selected node        /// </summary>
        public ChainBehaviorNode selectedNode;

        /// <summary>        /// nodes that the graph contains        /// </summary>
        [HideInInspector] public List<ChainBehaviorNode> nodes;

        /// <summary>        /// A flag the determine if the node is sending connection request to the graph        /// </summary>
        [HideInInspector] public bool connectionRequest;

        /// <summary>        /// The latest connected node        /// </summary>
        [HideInInspector] public ChainBehaviorNode connectedNode;


        void OnEnable()
        {
            if (nodes == null)
            {
                nodes = new List<ChainBehaviorNode>();
            }
        }

        /// <summary>        /// Initialization        /// </summary>        /// <param name="model"></param>
        public void InitGraph(ActorModel model)
        {
            this.model = model;
            model.fsm = new ActorFSM(this.model);
            if (nodes.Count > 0)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].InitNode(this);
                }
            }
        }

        /// <summary>        /// Runtime version of Graph Update        /// </summary>
        public void UpdateGraph()
        {
            if (nodes.Count > 0)
            {

            }
        }

#if UNITY_EDITOR
        // Editor version of Graph Update
        public void UpdateGraphGUI(Event e, Rect viewRect, GUISkin skin)
        {
            // lock for connection mode
            if (connectionRequest)
            {
                DrawConnectionToMouse(e.mousePosition);
            }

            if (nodes.Count > 0)
            {
                ProcessEvents(e, viewRect);
                // Update all lines
                foreach (var node in nodes)
                {
                    node.UpdateLineGUI();
                }

                // update all nodes
                foreach (var node in nodes)
                {
                    node.UpdateNodeGUI(e, viewRect, skin);
                }
            }
            EditorUtility.SetDirty(this); // Save all changes we updated for the scriptable object in editor window
        }

#endif

        /// <summary>        /// Update key and mouse events        /// </summary>        /// <param name="e"></param>        /// <param name="viewRect"></param>
        void ProcessEvents(Event e, Rect viewRect)
        {
            if (viewRect.Contains(e.mousePosition))
            {
                if (e.button == 0)
                {
                    if (e.type == EventType.MouseDown)
                    {
                        DeselectAllNodes();
                        bool setNode = false;
                        selectedNode = null;

                        // Select node
                        for (int i = 0; i < nodes.Count; i++)
                        {
                            if (nodes[i].nodeRect.Contains(e.mousePosition))
                            {
                                nodes[i].isSelected = true;
                                selectedNode = nodes[i];
                                Selection.activeObject = selectedNode;
                                setNode = true;
                            }
                        }

                        if (!setNode)
                        {
                            DeselectAllNodes();
                        }

                        if (connectionRequest)
                        {
                            connectionRequest = false;
                        }
                    }
                }
            }
        }

        /// <summary>        /// A method to deselect all nodes in graph        /// </summary>
        public void DeselectAllNodes()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].isSelected = false;
            }
        }

        /// <summary>        /// Draw lines between output and mouse position        /// </summary>        /// <param name="mousePos"></param>
        void DrawConnectionToMouse(Vector2 mousePos)
        {
            Handles.BeginGUI();
            Handles.color = Color.cyan;
            Handles.DrawAAPolyLine(3, new Vector3(connectedNode.nodeRect.x + connectedNode.nodeRect.width / 2, connectedNode.nodeRect.y + connectedNode.nodeRect.height + 5, 0f),
                new Vector3(mousePos.x, mousePos.y, 0));
        }
    }
}#endif