
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace CombatDesigner.EditorTool
{
    public enum DrawLineStyle
    {
        Line, Curve, AngleLine
    }

    public class View_Property : ViewBase
    {
        public DrawLineStyle lineStyle;

        bool _pressed;
        GUIStyle _redStyle;
        GUIStyle _buttonOffsetStyle;

        /// <summary>
        /// Constructor
        /// </summary>
        public View_Property() : base("Property View")
        {
            _redStyle = new GUIStyle("Button");
            _redStyle.onNormal.background = ChainEditorUtilities.MakeTex(2, 2, new Color(0f, 1f, 0f, 0.5f));
            _buttonOffsetStyle = new GUIStyle("Button");
            _buttonOffsetStyle.margin = new RectOffset(0, 0, 0, -1);
        }

        /// <summary>
        /// Update Property GUI
        /// </summary>
        /// <param name="editorRect"> the entire editor window rect </param>
        /// <param name="percentageRect"> the percentage of this view occupied </param>
        /// <param name="e"> mouse key event </param>
        /// <param name="graph"> the graph reference </param>
        public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, ChainGraph graph)
        {
            base.UpdateView(editorRect, percentageRect, e, graph);

            GUI.Box(viewRect, "", "Box");
            if (graph != null)
            {
                GUILayout.BeginArea(viewRect);
                DrawHeader(viewTitle + " Property", 1);
                GUILayout.Space(16);

                DrawActorModelGUI(graph);

                GUILayout.Space(10);

                if (graph.model != null)
                {

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginVertical();

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Add Node", GUILayout.Width(160), GUILayout.Height(30)))
                    {
                        //todo create new scriptable object
                    }
                    if (GUILayout.Button("Delete Node", GUILayout.Width(160), GUILayout.Height(30)))
                    {
                        //todo create new scriptable object
                    }
                    if (GUILayout.Button("Clear Nodes", GUILayout.Width(160), GUILayout.Height(30)))
                    {
                        //todo create new scriptable object
                    }
                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();



                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginVertical();

                    if (GUILayout.Button("Load Graph", GUILayout.Height(50)))
                    {
                        // todo load graph from model data
                    }

                    if (GUILayout.Button("Update Graph", GUILayout.Height(50)))
                    {
                        if (EditorUtility.DisplayDialog("Confirm Dialogue", "Do you really want to update this graph data to the " + graph.model.name + " ActorModel data?", "Yes", "No"))
                        {
                            graph.model.chainBehaviors.Clear();
                            for (int i = 0; i < graph.nodes.Count; i++)
                            {
                                graph.model.chainBehaviors.Add(new ChainBehavior(graph.nodes[i].id, graph.nodes[i].behavior, graph.nodes[i].followUps));
                            }

                            EditorUtility.SetDirty(graph.model);
                        }
                    }
                    GUILayout.EndVertical();


                    GUILayout.BeginHorizontal();

                    _redStyle.fontStyle = FontStyle.Bold;
                    _redStyle.alignment = TextAnchor.MiddleCenter;
                    _pressed = GUILayout.Toggle(_pressed, "Runtime\n\n Update", _redStyle, GUILayout.Width(100), GUILayout.Height(100));
                    // todo runtime editing

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.EndHorizontal();

                    GUILayout.Space(100);
                }
                GUILayout.EndArea();
            }
            ProcessEvent(e);
        }

        /// <summary>
        /// Draw ActorModel GUI
        /// </summary>
        /// <param name="graph"></param>
        private void DrawActorModelGUI(ChainGraph graph)
        {
            // Draw Model Object Field
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUIUtility.labelWidth = viewRect.width;
            GUILayout.Label("Model Reference", GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();


            EditorGUIUtility.labelWidth = viewRect.width / 5;
            EditorGUIUtility.fieldWidth = viewRect.width / 3;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();


            graph.model = (ActorModel)EditorGUILayout.ObjectField("Model", graph.model, typeof(ActorModel), false);

            if (GUILayout.Button("New", _buttonOffsetStyle, GUILayout.Width(46)))
            {
                //todo create new scriptable object

            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///  Process Key Mouse Event
        /// </summary>
        /// <param name="e"> Event </param>
        protected override void ProcessEvent(Event e)
        {
            base.ProcessEvent(e);
        }
    }
}
#endif