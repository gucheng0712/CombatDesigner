#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CombatDesigner.EditorTool
{
    [System.Serializable]
    public class NodeInput
    {
        public bool isOccupied = false;
        public ChainBehaviorNode node;
    }

    [System.Serializable]
    public class NodeOutput
    {
        public bool isOccupied = false;
        public List<ChainBehaviorNode> nodes = new List<ChainBehaviorNode>();
    }

    [System.Serializable]
    public class ChainBehaviorNode : ScriptableObject
    {
        public int id;
        public int priority;

        public KeyCode keyCode;

        public ActorBehavior behavior;

        [HideInInspector] public NodeOutput output;
        [HideInInspector] public NodeInput input;
        [HideInInspector] public ChainGraph parentGraph;
        [HideInInspector] public bool isSelected;
        [HideInInspector] public string nodeName;
        [HideInInspector] public Rect nodeRect;
        [HideInInspector] public List<int> followUps = new List<int>();

        protected NodeType nodeType;
        protected GUIStyle inPortSkin;
        protected GUIStyle outPortSkin;


        SerializedObject _serializedObject;
        SerializedProperty _serializedBehavior;


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

            _serializedObject = new SerializedObject(this);

            _serializedBehavior = _serializedObject.FindProperty("behavior");
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
            if (_serializedObject != null)
            {
                _serializedObject.Update();
            }
            if (_serializedObject == null)
            {
                _serializedObject = new SerializedObject(this);
                _serializedBehavior = _serializedObject.FindProperty("behavior");
            }

            ProcessEvents(e, viewRect);

            if (parentGraph != null)
            {

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
                    // input
                    if (parentGraph.connectedNode != null)
                    {
                        AddConnection();
                    }
                    else
                    {
                        RemoveConnection();
                    }
                    ResetConnectionStatus();
                }

                if (!isSelected)
                {
                    GUI.Box(nodeRect, "", skin.GetStyle("NodeDefault"));
                }
                else
                {
                    GUI.Box(nodeRect, "", skin.GetStyle("NodeSelected"));
                }
            }

            NodeBodyGUI();

            EditorUtility.SetDirty(this);
        }

        void NodeBodyGUI()
        {
            GUILayout.BeginArea(nodeRect);

            GUI.Box(new Rect(0, 4, nodeRect.width, 24), behavior == null ? "Null" : behavior.name);


            EditorGUI.LabelField(new Rect(nodeRect.width / 2 - 35, nodeRect.height / 10 + 20, 20, 16), "ID");
            id = EditorGUI.IntField(new Rect(nodeRect.width / 2 + 15, nodeRect.height / 10 + 20, 20, 16), id);
            EditorGUI.LabelField(new Rect(nodeRect.width / 2 - 35, nodeRect.height / 10 + 40, 40, 16), "Priority");
            priority = EditorGUI.IntField(new Rect(nodeRect.width / 2 + 15, nodeRect.height / 10 + 40, 20, 16), priority);



            if (_serializedObject != null)
            {
                EditorGUI.PropertyField(new Rect(nodeRect.width / 2 - 35, nodeRect.height / 10 + 60, 70, 16), _serializedBehavior, GUIContent.none);
                _serializedObject.ApplyModifiedProperties();
            }

            GUILayout.EndArea();

        }
#endif

        /// <summary>
        /// Process Input Events
        /// </summary>
        /// <param name="e"></param>
        /// <param name="viewRect"></param>
        public void ProcessEvents(Event e, Rect viewRect)
        {
            DragNodeEvent(e, viewRect);
        }

        // Method to drag the node in graph workspace
        void DragNodeEvent(Event e, Rect viewRect)
        {
            if (isSelected)
            {
                if (e.button == 0)
                {
                    if (e.type == EventType.MouseDrag)
                    {
                        if (viewRect.Contains(e.mousePosition))
                        {
                            nodeRect.x += e.delta.x;
                            nodeRect.y += e.delta.y;
                        }
                    }
                }
            }
        }

        void DrawInputLines()
        {
            if (input.isOccupied && input.node != null)
            {
                DrawLine();
            }
            else
            {
                input.isOccupied = false;
            }
        }

        void DrawLine()
        {
            Handles.BeginGUI();
            Handles.color = new Color(0.12f, 0.8f, 1f);
            Handles.DrawAAPolyLine(3, new Vector3(input.node.nodeRect.x + input.node.nodeRect.width / 2, input.node.nodeRect.y + input.node.nodeRect.height + 10, 0),
                new Vector3(nodeRect.x + nodeRect.width / 2, nodeRect.y - 6));
            //Handles.DrawLine(new Vector3(input.node.nodeRect.x + input.node.nodeRect.width / 2, input.node.nodeRect.y + input.node.nodeRect.height + 10, 0), new Vector3(nodeRect.x + nodeRect.width / 2, nodeRect.y - 6));
            Handles.EndGUI();
        }

        void ResetConnectionStatus()
        {
            input.isOccupied = (input.node != null);
            parentGraph.connectionRequest = false;
            parentGraph.connectedNode = null;
        }

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
#endif