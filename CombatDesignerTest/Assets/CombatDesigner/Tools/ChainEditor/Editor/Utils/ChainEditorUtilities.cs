
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace CombatDesigner.EditorTool
{
    /// <summary>    /// A utility class of Chain Editor    /// </summary>
    public static class ChainEditorUtilities
    {
        /// <summary>        /// A static method to create a new graph into Chain Editor based on a ActorModel         /// </summary>        /// <param name="model"></param>
        public static void CreateNewGraph(ActorModel model)
        {
            string graphPath = EditorUtility.SaveFilePanelInProject("Create Graph", "NewGraph", "", "Please enter a file name to save");
            if (string.IsNullOrWhiteSpace(graphPath))
                return;

            ChainGraph graph = ScriptableObject.CreateInstance<ChainGraph>();    // create a scriptableObject
            if (graph == null)
            {
                EditorUtility.DisplayDialog("Node Message", "Unable to create a new graph", "Ok");
            }

            if (!graphPath.Contains(".asset"))
            {
                graphPath += ".asset";
            }

            graph.InitGraph(model);

            AssetDatabase.CreateAsset(graph, graphPath); // create the asset
            AssetDatabase.SaveAssets();// save the asset
            AssetDatabase.Refresh();// after saving refresh to update

            ChainEditorWindow win = EditorWindow.GetWindow<ChainEditorWindow>();
            if (win != null)
            {
                win.graph = graph;
            }

            Vector2 rootNodePos = new Vector2(win._workView.viewRect.width / 2 - 50, win._workView.viewRect.height / 2 + 50);

            CreateNode(graph, NodeType.RootBehaviorNode, rootNodePos);
        }

        /// <summary>        /// A static method to load existing graph into Chain Editor        /// </summary>
        public static void LoadGraph()
        {
            string graphPath = EditorUtility.OpenFilePanel("Load Graph", Application.dataPath, "asset");
            if (string.IsNullOrWhiteSpace(graphPath))
                return;

            int appPathLen = Application.dataPath.Length;
            string finalPath = graphPath.Substring(appPathLen - 6);

            ChainGraph graph = AssetDatabase.LoadAssetAtPath(finalPath, typeof(ChainGraph)) as ChainGraph;
            if (graph != null)
            {
                ChainEditorWindow win = EditorWindow.GetWindow<ChainEditorWindow>();
                if (win != null)
                {
                    win.graph = graph;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Node Message", "Unable to load selected graph!", "OK");
            }
        }

        /// <summary>        ///  A static method to remove the graph in the Chain Editor        /// </summary>
        public static void UnloadGraph()
        {
            ChainEditorWindow win = EditorWindow.GetWindow<ChainEditorWindow>();
            if (win != null)
            {
                win.graph = null;
            }
        }

        /// <summary>        ///  A static method to create a node in graph        /// </summary>        /// <param name="graph"></param>        /// <param name="nodeType"></param>        /// <param name="pos"></param>
        public static void CreateNode(ChainGraph graph, NodeType nodeType, Vector2 pos)
        {
            if (graph != null)
            {
                ChainBehaviorNode node = null;
                switch (nodeType)
                {
                    case NodeType.ChainBehaviorNode:
                        node = ScriptableObject.CreateInstance<ChainBehaviorNode>();
                        node.nodeName = "Chain Behavior";
                        break;
                    case NodeType.RootBehaviorNode:
                        node = ScriptableObject.CreateInstance<RootBehaviorNode>();
                        node.nodeName = "Root Behavior";
                        break;
                    default:
                        break;
                }

                if (node != null)
                {
                    node.InitNode(graph);
                    node.nodeRect.x = pos.x;
                    node.nodeRect.y = pos.y;
                    node.parentGraph = graph;
                    graph.nodes.Add(node);
                    UpdateChainBehaviorNodeID(graph);

                    node.name = node.nodeName;
                    AssetDatabase.AddObjectToAsset(node, graph);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        /// <summary>        /// A static method to delete the node in the graph        /// </summary>        /// <param name="nodeIndex"></param>        /// <param name="graph"></param>
        public static void DeleteNode(int nodeIndex, ChainGraph graph)
        {
            if (graph != null)
            {
                if (graph.nodes.Count >= nodeIndex)
                {
                    ChainBehaviorNode deleteNode = (ChainBehaviorNode)graph.nodes[nodeIndex];
                    if (deleteNode != null)
                    {
                        RemoveNodeAndCleanUp(graph, deleteNode);
                        UpdateChainBehaviorNodeID(graph);
                        UpdateFollowUps(graph);
                        Object.DestroyImmediate(deleteNode, true);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
        }        /// <summary>        /// A static method to remove the node from graph's nodes list and clean up        /// </summary>        /// <param name="graph"></param>        /// <param name="deleteNode"></param>        static void RemoveNodeAndCleanUp(ChainGraph graph, ChainBehaviorNode deleteNode)
        {
            foreach (ChainBehaviorNode n in graph.nodes)
            {
                if (n.output.nodes != null)
                {
                    if (n.output.nodes.Contains(deleteNode))
                    {
                        n.output.nodes.Remove(deleteNode);
                    }
                }
            }
            graph.nodes.Remove(deleteNode);

        }

        /// <summary>        /// Update the ID of the node        /// </summary>        /// <param name="graph"></param>
        static void UpdateChainBehaviorNodeID(ChainGraph graph)
        {
            for (int i = 0; i < graph.nodes.Count; i++)
            {
                graph.nodes[i].id = i;
            }
        }

        /// <summary>        ///  A static method to update the followups of the nodes        /// </summary>        /// <param name="graph"></param>
        public static void UpdateFollowUps(ChainGraph graph)
        {
            foreach (ChainBehaviorNode n in graph.nodes)
            {
                if (n.output.nodes != null)
                {
                    ReorderByPriority(n);
                    n.followUps.Clear();
                    for (int i = 0; i < n.output.nodes.Count; i++)
                    {
                        n.followUps.Add(n.output.nodes[i].id);
                    }
                }
            }
        }

        /// <summary>        /// A static method to reorder the list by its priority        /// </summary>        /// <param name="n"></param>
        public static void ReorderByPriority(ChainBehaviorNode n)
        {
            n.output.nodes = n.output.nodes.OrderBy(o => o.priority).ToList();
        }

        /// <summary>        /// A static method to draw grid        /// </summary>        /// <param name="viewRect"></param>        /// <param name="gridSpacing"></param>        /// <param name="gridOpacity"></param>        /// <param name="gridColor"></param>
        public static void DrawGrid(Rect viewRect, float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(viewRect.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(viewRect.height / gridSpacing);
            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            for (int x = 0; x < widthDivs; x++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * x, 0, 0), new Vector3(gridSpacing * x, viewRect.height, 0));
            }
            for (int y = 0; y < heightDivs; y++)
            {
                Handles.DrawLine(new Vector3(0, gridSpacing * y, 0), new Vector3(viewRect.width, gridSpacing * y, 0));
            }
            Handles.color = Color.white;
            Handles.EndGUI();
        }

        /// <summary>        /// A static method to create a texture        /// </summary>        /// <param name="width"></param>        /// <param name="height"></param>        /// <param name="col"></param>        /// <returns></returns>
        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }


        const float k_EditorWindowTabHeight = 20.0f;
        static Matrix4x4 s_prevGuiMatrix;

        /// <summary>        /// A static method to start a Zoom Area        /// </summary>        /// <param name="zoomScale"></param>        /// <param name="screenCoordsArea"></param>        /// <returns></returns>
        public static Rect BeginZoomArea(float zoomScale, Rect screenCoordsArea)
        {
            GUI.EndGroup();        // End the group Unity begins automatically for an EditorWindow to clip out the window tab. This allows us to draw outside of the size of the EditorWindow.

            Rect clippedArea = screenCoordsArea.ScaleSizeBy(1.0f / zoomScale, screenCoordsArea.TopLeft());
            clippedArea.y += k_EditorWindowTabHeight;
            GUI.BeginGroup(clippedArea);

            s_prevGuiMatrix = GUI.matrix;
            //translates the clip area's top-left corner to the origin
            Matrix4x4 translation = Matrix4x4.TRS(clippedArea.TopLeft(), Quaternion.identity, Vector3.one);
            //scaling around the origin
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1.0f));
            //apply and translates the zoomed result back to where the clip area is supposed to be
            GUI.matrix = translation * scale * translation.inverse * GUI.matrix;

            return clippedArea;
        }

        /// <summary>        /// A static method to ends a Zoom Area        /// </summary>
        public static void EndZoomArea()
        {
            GUI.matrix = s_prevGuiMatrix;
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(0.0f, k_EditorWindowTabHeight, Screen.width, Screen.height));
        }

        /// <summary>        /// Convert Screen Coordinates to the zoom coordinatess         /// </summary>        /// <param name="screenCoords"></param>        /// <param name="zoomRect"></param>        /// <param name="zoomScale"></param>        /// <param name="zoomPivot"></param>        /// <returns></returns>
        public static Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords, Rect zoomRect, float zoomScale, Vector2 zoomPivot)
        {
            return (screenCoords - zoomRect.TopLeft()) / zoomScale + zoomPivot;
        }



        #region Debug
        public static void RepaintInspector(System.Type t)
        {
            Editor[] ed = (Editor[])Resources.FindObjectsOfTypeAll<Editor>();
            for (int i = 0; i < ed.Length; i++)
            {
                if (ed[i].GetType() == t)
                {
                    ed[i].Repaint();
                    return;
                }
            }
        }
        #endregion
    }
}#endif