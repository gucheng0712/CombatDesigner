
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CombatDesigner.EditorTool
{
    /// <summary>    /// A class to create a Chain Editor menu item in menu bar    /// </summary>
    public static class ChainEditorMenus
    {
        [MenuItem("CombatDesigner/Chain Editor")]
        public static void InitNodeEditor()
        {
            ChainEditorWindow.Open();
        }
    }
}#endif