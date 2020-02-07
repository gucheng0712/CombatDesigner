
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CombatDesigner.EditorTool
{

    public class ACT_MasterEditorWindow : OdinMenuEditorWindow
    {
        public static ACT_MasterEditorWindow win;

        public ActorModel model;

        public List<ActorBehavior> behaviors;

        [HorizontalGroup("Settings")]
        [FolderPath(RequireExistingPath = true), BoxGroup("Settings/Behavior OutputPath", CenterLabel = true, ShowLabel = true)]
        public string OutputPath;

        [HorizontalGroup("Settings/Btn", Width = 200f, MarginRight = -50, PaddingRight = 0), Button(ButtonHeight = 38), GUIColor(0, 1, 1)]
        public void CreateNewBehavior()
        {

        }

        [LabelWidth(150)]
        [HorizontalGroup("Group One", 200)]
        public List<IBehaviorAction> actions;

        [LabelWidth(120)]
        [HorizontalGroup("Group Two", 200)]
        [ListDrawerSettings(ShowIndexLabels = true)]
        public List<BehaviorAttack> attackInfos;


        [MenuItem("ACT-System/Master Editor")]
        static void Open()
        {
            win = GetWindow<ACT_MasterEditorWindow>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        void OnDisable()
        {


        }

        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create New Behavior")))
                {
                    //ScriptableObjectCreator.ShowDialog<Character>("Assets/Plugins/Sirenix/Demos/Sample - RPG Editor/Character", obj =>
                    //{
                    //    obj.Name = obj.name;
                    //    base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                    //});
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create New State")))
                {
                    //ScriptableObjectCreator.ShowDialog<Item>("Assets/Plugins/Sirenix/Demos/Sample - RPG Editor/Items", obj =>
                    //{
                    //    obj.Name = obj.name;
                    //    base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                    //});
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create New Model")))
                {
                    //ScriptableObjectCreator.ShowDialog<Character>("Assets/Plugins/Sirenix/Demos/Sample - RPG Editor/Character", obj =>
                    //{
                    //    obj.Name = obj.name;
                    //    base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                    //});
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        protected override void OnGUI()
        {
            if (model != null)
            {
                if (behaviors == null || model.behaviors.Count != behaviors.Count)
                {

                    behaviors = model.behaviors;
                }
            }

            base.OnGUI();

        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.Selection.SupportsMultiSelect = false;

            //tree.Add("Settings", GeneralDrawerConfig.Instance);
            //tree.Add("Utilities", new TextureUtilityEditor());
            //tree.AddAllAssetsAtPath("Odin Settings", "Assets/Plugins/Sirenix", typeof(ScriptableObject), true, true);
            tree.AddAllAssetsAtPath("Actor Models", "Assets/ACT-System/Data", typeof(ActorModel), true, true);
            tree.AddAllAssetsAtPath("Actor Behaviors", "Assets/ACT-System/Data", typeof(ActorBehavior), true, true).ForEach(this.AddDragHandles);

            return tree;
        }

        private void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
        }

    }

}
#endif