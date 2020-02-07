
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CombatDesigner;
using System;

namespace CombatDesigner.EditorTool
{
    [System.Serializable]
    public class ChainGraph : ScriptableObject
    {
        public ActorModel model;
        public string graphName = "New Graph";
        public ChainBehaviorNode selectedNode;
        public ChainBehaviorNode defaultNode;

        [HideInInspector] public List<ChainBehaviorNode> nodes;

        [HideInInspector] public bool connectionRequest;
        [HideInInspector] public ChainBehaviorNode connectedNode;


        void OnEnable()
        {
            if (nodes == null)
            {
                nodes = new List<ChainBehaviorNode>();
            }
        }

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

        // runtime version
        public void UpdateGraph()
        {
            if (nodes.Count > 0)
            {

            }
        }

        // editor version
#if UNITY_EDITOR
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

        public void DeselectAllNodes()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].isSelected = false;
            }
        }

        void DrawConnectionToMouse(Vector2 mousePos)
        {
            Handles.BeginGUI();
            Handles.color = Color.cyan;
            Handles.DrawAAPolyLine(3, new Vector3(connectedNode.nodeRect.x + connectedNode.nodeRect.width / 2, connectedNode.nodeRect.y + connectedNode.nodeRect.height + 5, 0f),
                new Vector3(mousePos.x, mousePos.y, 0));
        }
    }
}
#endif