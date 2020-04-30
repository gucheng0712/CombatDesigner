
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace CombatDesigner.EditorTool
{
    /// <summary>
    /// A custom editor for ColliderVisualizer class
    /// </summary>
    [CustomEditor(typeof(ColliderVisualizer))]
    public class ColliderVisualizerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI(); // show the original GUI content

            // Assign the target object
            ColliderVisualizer colliderVisualizer = target as ColliderVisualizer;

            if (colliderVisualizer == null)
                return;

            Collider col = colliderVisualizer.GetComponent<Collider>();

            GUILayout.Label("Visualizer Type:" + col);

            // A Btn for Updating the Collider Visualizer
            if (GUILayout.Button("UpdateVisualizer"))
            {
                LineRenderer lr = colliderVisualizer.GetComponent<LineRenderer>();
                MeshRenderer mr = colliderVisualizer.GetComponent<MeshRenderer>();
                if (lr != null)
                {
                    lr.enabled = true;
                }
                if (mr != null)
                {
                    mr.enabled = true;
                }
                colliderVisualizer.Init();
            }

            // A Btn for Closing the Collider Visualizer
            if (GUILayout.Button("CloseVisualizer"))
            {
                LineRenderer lr = colliderVisualizer.GetComponent<LineRenderer>();
                MeshRenderer mr = colliderVisualizer.GetComponent<MeshRenderer>();
                if (lr != null)
                {
                    lr.enabled = false;
                }
                if (mr != null)
                {
                    mr.enabled = false;
                }
            }
        }
    }
}
#endif