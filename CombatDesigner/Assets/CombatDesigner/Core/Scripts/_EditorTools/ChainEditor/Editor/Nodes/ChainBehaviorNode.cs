
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace CombatDesigner.EditorTool
{
    /// <summary>
    /// A struct of input node, single input
    /// </summary>
    [System.Serializable]
    public class NodeInput
    {
        public bool isOccupied = false;
        public ChainBehaviorNode node;
    }

    /// <summary>
    /// A struct of output nodes, multiple nodes
    /// </summary>
    [System.Serializable]
    public class NodeOutput
    {
        public bool isOccupied = false;
        public List<ChainBehaviorNode> nodes = new List<ChainBehaviorNode>();
    }

    /// <summary>
    /// ChainBehaviorNode is a class that inherits from ScriptableObject to represent a node of the behavior
    /// </summary>
    [System.Serializable]
    public class ChainBehaviorNode : ScriptableObject
    {
        /// <summary>
        /// Each node has a unique id
        /// </summary>
        [Tooltip("Each node has a unique id")]
        public int id;
        /// <summary>
        /// Node has priority
        /// </summary>
        [Tooltip("Node has priority")]
        public int priority;

        /// <summary>
        ///  The behavior object of the node
        /// </summary>
        [Tooltip("The behavior object of the node")]
        public ActorBehavior behavior;

        /// <summary>
        /// output struct
        /// </summary>
        [HideInInspector] public NodeOutput output;
        /// <summary>
        /// input srtruct
        /// </summary>
        [HideInInspector] public NodeInput input;
        /// <summary>
        /// The graph object that the node is in
        /// </summary>
        [HideInInspector] public ChainGraph parentGraph;
        /// <summary>
        /// if the node is currently selected
        /// </summary>
        [HideInInspector] public bool isSelected;
        /// <summary>
        ///  The name of the node
        /// </summary>
        [HideInInspector] public string nodeName;
        /// <summary>
        /// The size of the node
        /// </summary>
        [HideInInspector] public Rect nodeRect;
        /// <summary>
        /// the leaf nodes of current node
        /// </summary>
        [HideInInspector] public List<int> followUps = new List<int>();

        /// <summary>
        /// The type of the node
        /// </summary>
        protected NodeType nodeType;

        /// <summary>
        /// GUI Skin of input port
        /// </summary>
        protected GUIStyle inPortSkin;
        /// <summary>
        /// GUI Skin of output port
        /// </summary>
        protected GUIStyle outPortSkin;

        protected string skinName;

        List<string> behaviorStrs = new List<string>(); // a list of behavior names
        public int behaviorIndex; // the current index

        /// <summary>
        /// Init the Node
        /// </summary>
        /// <param name="parentGraph"></param>
        public virtual void InitNode(ChainGraph parentGraph)
        {
            output = new NodeOutput();
            input = new NodeInput();

            inPortSkin = new GUIStyle("Button");
            outPortSkin = new GUIStyle("Button");
            inPortSkin.overflow.bottom = 10;
            outPortSkin.overflow.top = 10;

            nodeType = NodeType.ChainBehaviorNode;
            nodeRect = new Rect(10, 10, 100, 100);

        }


#if UNITY_EDITOR

        /// <summary>
        ///  Draw Line GUI (Before Node GUI)
        /// </summary>
        public virtual void UpdateLineGUI()
        {
            DrawInputLines();
        }

        /// <summary>
        ///  Draw Node GUI
        /// </summary>
        /// <param name="e"></param>
        /// <param name="viewRect"></param>
        /// <param name="skin"></param>
        public virtual void UpdateNodeGUI(Event e, Rect viewRect, GUISkin skin)
        {

            if (parentGraph != null)
            {
                // Change the style of in/out ports btn style
                inPortSkin = new GUIStyle("Button");
                inPortSkin.overflow.bottom = 10;
                outPortSkin = new GUIStyle("Button");
                outPortSkin.overflow.top = 10;

                // output
                if (GUI.Button(new Rect(nodeRect.x + nodeRect.width / 4, nodeRect.y + nodeRect.height - 2, nodeRect.width / 2, 16), "", outPortSkin))
                {
                    parentGraph.connectionRequest = true;
                    parentGraph.connectedNode = this;
                }

                // Input
                if (GUI.Button(new Rect(nodeRect.x + nodeRect.width / 2 - 20, nodeRect.y - 14, 40, 16), "", inPortSkin))
                {
                    // If there is no connect input, then add the connection, else remove the connection
                    if (parentGraph.connectedNode != null)
                    {
                        AddConnection();
                    }
                    else
                    {
                        RemoveConnection();
                    }
                    // Update the connection status every time press the button
                    ResetConnectionStatus();
                }

                // Update the GUI style of selected and non-selected node
                if (!isSelected)
                {
                    skinName = IsMatchingRuntimeBehavior() ? "Node_Executing" : "NodeDefault";
                    GUI.Box(nodeRect, "", skin.GetStyle(skinName));
                }
                else
                {
                    skinName = IsMatchingRuntimeBehavior() ? "NodeSelected_Executing" : "NodeSelected";
                    GUI.Box(nodeRect, "", skin.GetStyle(skinName));
                }
            }

            // Process the Keyboard and mouse events
            ProcessEvents(e, viewRect);

            // Draw Node GUI
            NodeBodyGUI();

            // Mark Dirty to save the data in editor mode
            EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// A method to draw chain behavior node body gui
        /// </summary>
        void NodeBodyGUI()
        {
            GUILayout.BeginArea(nodeRect);

            GUI.Box(new Rect(0, 4, nodeRect.width, 24), behavior == null ? "Null" : behavior.name);

            EditorGUI.LabelField(new Rect(nodeRect.width / 2 - 35, nodeRect.height / 10 + 20, 20, 16), "ID");
            EditorGUI.IntField(new Rect(nodeRect.width / 2 + 15, nodeRect.height / 10 + 20, 20, 16), id);
            EditorGUI.LabelField(new Rect(nodeRect.width / 2 - 35, nodeRect.height / 10 + 40, 40, 16), "Priority");
            priority = EditorGUI.IntField(new Rect(nodeRect.width / 2 + 15, nodeRect.height / 10 + 40, 20, 16), priority);


            if (behaviorStrs.Count != parentGraph.model.behaviors.Count)
            {
                behaviorStrs.Clear();
                // match the name of the behavior list
                foreach (var b in parentGraph.model.behaviors)
                {
                    if (b != null)
                    {
                        if (string.IsNullOrEmpty(b.name))
                        {
                            behaviorStrs.Add("No Name Behavior");
                        }
                        else
                        {
                            behaviorStrs.Add(b.name);
                        }
                    }
                }
            }
            EditorGUI.BeginChangeCheck();
            behaviorIndex = EditorGUI.Popup(new Rect(nodeRect.width / 2 - 35, nodeRect.height / 10 + 60, 70, 16), behaviorIndex, behaviorStrs.ToArray(), EditorStyles.popup);
            behavior = parentGraph.model.behaviors[behaviorIndex];
            GUILayout.EndArea();
        }
#endif

        protected bool IsMatchingRuntimeBehavior()
        {
            return (parentGraph.model.currentBehavior == behavior && Application.isPlaying);
        }

        /// <summary>
        /// Process Input Events
        /// </summary>
        /// <param name="e"></param>
        /// <param name="viewRect"></param>
        public void ProcessEvents(Event e, Rect viewRect)
        {
            DragNodeEvent(e, viewRect);
        }

        void UpdatePosition(NodeOutput output, Event e)
        {
            if (output == null || this is RootBehaviorNode)
                return;
            foreach (var n in output.nodes)
            {
                UpdatePosition(n.output, e);
                n.nodeRect.position += e.delta;
            }
        }

        int counter;
        // Method to drag the node in graph workspace
        void DragNodeEvent(Event e, Rect viewRect)
        {
            if (EditorWindow.focusedWindow == ChainEditorWindow._win)
            {
                if (isSelected)
                {
                    if (e.button == 0)
                    {
                        if (viewRect.Contains(e.mousePosition))
                        {
                            if (e.type == EventType.MouseDrag)
                            {
                                counter++;
                                if (counter > 1)
                                {
                                    parentGraph.selectedNode.nodeRect.position += e.delta;
                                    UpdatePosition(output, e);
                                }
                            }

                        }
                    }
                }
                if (e.type == EventType.MouseUp)
                {
                    counter = 0;
                }
            }


            ////Box Select
            //if (e.type == EventType.MouseDrag)
            //{
            //    if (!dragBox)
            //    {
            //        dragBox = true;
            //        startPos = e.mousePosition;
            //    }

            //}
            //if (e.type == EventType.MouseUp)
            //{
            //    dragBox = false;
            //}

            //if (dragBox)
            //{
            //    Rect boxRect = new Rect(startPos.x, startPos.y, e.mousePosition.x - startPos.x, e.mousePosition.y - startPos.y);
            //    GUI.Box(boxRect, "");
            //}
        }

        /// <summary>
        ///  A method to draw line connection between two nodes
        /// </summary>
        void DrawInputLines()
        {
            if (input.isOccupied && input.node != null)
            {
                Vector3 nodePosA = new Vector3(input.node.nodeRect.x + input.node.nodeRect.width / 2, input.node.nodeRect.y + input.node.nodeRect.height + 10, 0);
                Vector3 nodePosB = new Vector3(nodeRect.x + nodeRect.width / 2, nodeRect.y - 6, 0);
                DrawLine(nodePosA, nodePosB);
            }
            else
            {
                input.isOccupied = false;
            }
        }

        /// <summary>
        /// A method to draw line based on two Vector
        /// </summary>
        void DrawLine(Vector3 a, Vector3 b)
        {
            Handles.BeginGUI();
            Handles.color = new Color(0.12f, 0.8f, 1f);
            Handles.DrawAAPolyLine(3, a, b);
            Handles.EndGUI();
        }

        /// <summary>
        /// A method to reset the node connection status
        /// </summary>
        void ResetConnectionStatus()
        {
            input.isOccupied = (input.node != null);
            parentGraph.connectionRequest = false;
            parentGraph.connectedNode = null;
        }

        /// <summary>
        /// Add Connection between two nodes
        /// </summary>
        void AddConnection()
        {
            input.node = parentGraph.connectedNode;
            if (input.node != null)
            {
                if (input.node.output.nodes != null)
                {
                    if (!input.node.output.nodes.Contains(this))
                    {
                        input.node.output.nodes.Add(this);
                    }

                }
            }
            ChainEditorUtilities.UpdateFollowUps(parentGraph);
        }

        /// <summary>
        /// Remove the connection between two nodes
        /// </summary>
        void RemoveConnection()
        {
            if (input.node != null)
            {
                if (input.node.output.nodes.Contains(this))
                {
                    input.node.output.nodes.Remove(this);
                }
            }
            input.node = parentGraph.connectedNode;
            ChainEditorUtilities.UpdateFollowUps(parentGraph);
        }
    }
}