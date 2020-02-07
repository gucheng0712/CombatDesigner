
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CombatDesigner.EditorTool
{
    public static class ChainEditorMenus
    {
        [MenuItem("CombatDesigner/ChainEditor")]
        public static void InitNodeEditor()
        {
            ChainEditorWindow.Open();
        }
    }
}
#endif