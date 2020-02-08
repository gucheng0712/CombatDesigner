
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;


namespace CombatDesigner.EditorTool
{
    public class View_NodeWorkspace : ViewBase
    {
        /// <summary>        /// current zoom value        /// </summary>
        public static float zoom = 1.0f;
        public static Vector2 ZoomPivot;

        /// <summary>        ///  Min and Max Zoom value        /// </summary>
        const float ZOOM_MIN = 0.5f;
        const float ZOOM_MAX = 5.0f;

        /// <summary>        /// The mouse position        /// </summary>
        Vector2 _mousePos;

        /// <summary>        /// The node index to delete        /// </summary>
        int _deleteNodeIndex = 0;

        public View_NodeWorkspace() : base("Work View")
        {
        }

        /// <summary>        /// Update the the view aspect of the         /// </summary>        /// <param name="editorRect"></param>        /// <param name="percentageRect"></param>        /// <param name="e"></param>        /// <param name="graph"></param>
        public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, ChainGraph graph)
        {
            base.UpdateView(editorRect, percentageRect, e, graph);
            GUI.Box(viewRect, "", skin.GetStyle("ViewBG"));
            ChainEditorUtilities.DrawGrid(viewRect, 50f, 0.25f, Color.white);

            ChainEditorUtilities.DrawGrid(viewRect, 10f, 0.15f, Color.white);

            if (graph != null)
            {
                DrawZoomableGraph(e, graph);

                GUILayout.BeginArea(new Rect(viewRect.x, viewRect.y, viewRect.width * zoom, viewRect.height));
                DrawStateToolBar(graph);
                GUILayout.EndArea();
            }

            ProcessEvent(e);
        }

        void DrawZoomableGraph(Event e, ChainGraph graph)
        {
            ChainEditorUtilities.BeginZoomArea(zoom, viewRect);
            viewRect.size /= zoom;
            graph.UpdateGraphGUI(e, viewRect, skin);
            ChainEditorUtilities.EndZoomArea();
        }

        void DrawStateToolBar(ChainGraph graph)
        {
            if (graph.model != null)
            {
                DrawHeader(viewTitle + " Graph - " + graph.model.name, zoom);
            }
            else
            {
                DrawHeader(viewTitle + " Graph", zoom);
            }
        }

        protected override void ProcessEvent(Event e)
        {
            base.ProcessEvent(e);
            ContextMenuEvent(e);
            PanEvent(e);
            ZoomEvent();
        }

        void ContextMenuEvent(Event e)
        {
            //right mouse
            if (e.button == 1)
            {
                if (e.type == EventType.MouseDown)
                {
                    if (viewRect.Contains(e.mousePosition))
                    {
                        //Debug.Log("Right Down in: " + viewTitle);
                        _mousePos = e.mousePosition;

                        bool overNode = false;
                        _deleteNodeIndex = 0;
                        if (graph != null)
                        {
                            if (graph.nodes.Count > 0)
                            {
                                for (int i = 0; i < graph.nodes.Count; i++)
                                {
                                    if (graph.nodes[i].nodeRect.Contains(_mousePos))
                                    {
                                        _deleteNodeIndex = i;
                                        overNode = true;
                                    }
                                }
                            }
                        }

                        if (!overNode)
                        {
                            ProcessContextMenu(e, 0);
                        }
                        else
                        {
                            ProcessContextMenu(e, 1);
                        }
                    }
                }
            }
        }

        void ProcessContextMenu(Event e, int contextID)
        {
            if (graph != null)
            {
                graph.DeselectAllNodes();
            }
            GenericMenu menu = new GenericMenu();

            if (contextID == 0)
            {
                menu.AddItem(new GUIContent("Create Graph"), false, ContextCallback, "0");
                menu.AddItem(new GUIContent("Load Graph"), false, ContextCallback, "1");

                if (graph != null)
                {
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Unload Graph"), false, ContextCallback, "2");
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("ChainBehavior Node"), false, ContextCallback, "3");
                    menu.AddItem(new GUIContent("RootBehavior Node"), false, ContextCallback, "4");
                }
            }
            if (contextID == 1)
            {
                if (graph != null)
                {
                    menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "5");
                }
            }
            menu.ShowAsContext();
            e.Use();
        }

        void ContextCallback(object obj)
        {
            switch (obj.ToString())
            {
                case "0":
                    //Debug.Log("Created a new graph");
                    //ChainEditorUtilities.CreateNewGraph();
                    ChainEditorPopupWindow.Open();
                    break;
                case "1":
                    //Debug.Log("Loaded a graph");
                    ChainEditorUtilities.LoadGraph();
                    break;
                case "2":
                    //Debug.Log("Unloaded a graph");
                    ChainEditorUtilities.UnloadGraph();
                    break;
                case "3":
                    ChainEditorUtilities.CreateNode(graph, NodeType.ChainBehaviorNode, _mousePos);
                    break;
                case "4":
                    ChainEditorUtilities.CreateNode(graph, NodeType.RootBehaviorNode, _mousePos);
                    break;
                case "5":
                    ChainEditorUtilities.DeleteNode(_deleteNodeIndex, graph);
                    break;
            }
        }

        void PanEvent(Event e)
        {
            if (e.button == 2)
            {
                if (e.type == EventType.MouseDrag)
                {
                    foreach (ChainBehaviorNode n in graph.nodes)
                    {
                        n.nodeRect.x += e.delta.x;
                        n.nodeRect.y += e.delta.y;
                    }
                }
            }
        }

        void ZoomEvent()
        {
            // Allow adjusting the zoom with the mouse wheel as well. In this case, use the mouse coordinates
            // as the zoom center instead of the top left corner of the zoom area. This is achieved by
            // maintaining an origin that is used as offset when drawing any GUI elements in the zoom area.
            if (Event.current.type == EventType.ScrollWheel)
            {
                Vector2 screenCoordsMousePos = Event.current.mousePosition;
                Vector2 delta = Event.current.delta;
                Vector2 zoomCoordsMousePos = ChainEditorUtilities.ConvertScreenCoordsToZoomCoords(screenCoordsMousePos, viewRect, zoom, ZoomPivot);
                float zoomDelta = -delta.y / 150.0f;
                float oldZoom = zoom;
                zoom += zoomDelta;
                zoom = Mathf.Clamp(zoom, ZOOM_MIN, ZOOM_MAX);
                ZoomPivot += (zoomCoordsMousePos - ZoomPivot) - (oldZoom / zoom) * (zoomCoordsMousePos - ZoomPivot);
            }
        }
    }
}#endif