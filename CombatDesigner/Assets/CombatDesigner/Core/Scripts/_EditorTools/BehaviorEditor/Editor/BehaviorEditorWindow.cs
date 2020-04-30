
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CombatDesigner;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace CombatDesigner.EditorTool
{

    public class BehaviorEditorWindow : OdinEditorWindow
    {
        public static BehaviorEditorWindow win;
        public static float deltaTime = 1 / 60f;
        public static float playbackSpeed = 1f;

        [HideInInspector] public ActorBehavior selectedBehavior;
        ActorBehavior previousBehavior;
        [InfoBox("Please Select a Character Prefab to Preview", "IsCharacterNull")]
        [BoxGroup("BehaviorEditor/Property")]
        [LabelWidth(100)]
        [OnValueChanged("GetComponentsCallback")]
        public GameObject character;

        HitBox hitbox;
        HurtBox hurtbox;
        EditorWindow hitboxInspector;
        EditorWindow hurtboxInspector;
        EditorWindow modelInspector;
        EditorWindow behaviorInspector;

        bool showHitBoxInspector;
        bool showHurtBoxInspector;
        bool showModelInspector;
        bool showBehaviorInspector;
        [HideInInspector] public ActorController actorController;
        [HideInInspector] public CharacterController cc;
        [HideInInspector] public AnimatorController controller;

        [HideInInspector] public View_Timeline timeline = new View_Timeline();
        View_BehaviorInfo behaviorInfo = new View_BehaviorInfo();
        View_BehaviorSelector behaviorSelector = new View_BehaviorSelector();
        private Vector2 behaviorScrollPos;

        bool IsCharacterNull()
        {
            return character == null;
        }

        public void GetComponentsCallback()
        {
            Debug.Log("Get Components of cc and model");
            if (character != null)
            {
                actorController = character.GetComponent<ActorController>();
                actorController.Init();
                if (!actorController.model.character)
                    actorController.model.character = character.transform.GetChild(0).gameObject;
                cc = character.GetComponent<CharacterController>();
                hitbox = actorController.model.hitBox;
                hurtbox = actorController.model.hurtBox;

                timeline.Init();
                behaviorInfo.Init();
                behaviorSelector.Init();
            }
        }

        [MenuItem("CombatDesigner/BehaviorEditor")]
        public static void Open()
        {
            win = GetWindow<BehaviorEditorWindow>();
        }

        protected override void OnEnable()
        {
            if (win != null)
            {
                win.Close();
            }
            EditorApplication.playModeStateChanged += StateChange;
            base.OnEnable();

            Init();

            EditorApplication.update += timeline.UpdateAnimationPreviewInEditor;
            SceneView.onSceneGUIDelegate += OnSceneViewGUI;
        }

        private void Init()
        {
            if (win == null)
            {
                win = GetWindow<BehaviorEditorWindow>();
            }
            timeline = new View_Timeline();
            behaviorInfo = new View_BehaviorInfo();
            behaviorSelector = new View_BehaviorSelector();
            timeline.Init();
            behaviorInfo.Init();
            behaviorSelector.Init();

        }

        void Update()
        {
        }

        private void StateChange(PlayModeStateChange state)
        {
            GetComponentsCallback();
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= StateChange;
            EditorApplication.update -= timeline.UpdateAnimationPreviewInEditor;
            SceneView.onSceneGUIDelegate -= OnSceneViewGUI;
        }

        public void LoadState(string path, DataFormat dataFormat, List<UnityEngine.Object> unityObjectReferences)
        {
            var bytes = File.ReadAllBytes(path);

            // If you have a string to deserialize, get the bytes using UTF8 encoding
            // var bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);

            var data = SerializationUtility.DeserializeValue<ActorBehavior>(bytes, dataFormat, unityObjectReferences);
        }

        [OnInspectorGUI]
        [HorizontalGroup("BehaviorEditor", 0.7f)]
        [BoxGroup("BehaviorEditor/Timeline")]
        [PropertyOrder(-1)]
        void DrawTimeLine()
        {
            if (win == null) return;
            if (selectedBehavior == null) return;
            if (character == null) return;

            timeline.DrawTimeLine();
        }


        [OnInspectorGUI]
        [VerticalGroup("BehaviorEditor/Timeline/Inspectors")]
        void DrawBehaviorInfoToolBar()
        {

            if (win == null || actorController == null)
                return;

            behaviorInfo.DrawBehaviorInfoToolBar();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawInspectorToggleBtn("Hitbox Inspector", Color.cyan, hitbox, ref hitboxInspector, ref showHitBoxInspector);
            DrawInspectorToggleBtn("Hurtbox Inspector", Color.red, hurtbox, ref hurtboxInspector, ref showHurtBoxInspector);
            DrawInspectorToggleBtn("Model Inspector", Color.yellow, actorController.model, ref modelInspector, ref showModelInspector);


            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

        }


        void DrawInspectorToggleBtn(string label, Color btnColor, UnityEngine.Object objToInspect, ref EditorWindow inspectorWindow, ref bool showInspectorFlag)
        {
            if (objToInspect != null)
            {
                var style = new GUIStyle(GUI.skin.button);
                GUI.backgroundColor = btnColor;
                showInspectorFlag = GUILayout.Toggle(showInspectorFlag, label, style, GUILayout.Width(120), GUILayout.Height(40));

                if (showInspectorFlag)
                {
                    if (inspectorWindow == null)
                    {
                        inspectorWindow = GetInspectTarget(objToInspect);
                        inspectorWindow.Show();
                        DockUtilities.DockWindow(win, inspectorWindow, DockUtilities.DockPosition.Right);
                    }
                }
                else
                {
                    if (inspectorWindow != null)
                    {
                        inspectorWindow.Close();
                    }
                }



                GUI.backgroundColor = GUI.color;
            }
        }

        public EditorWindow GetInspectTarget(UnityEngine.Object targetGO)
        {
            // Get Unity Internal Objects
            Type inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            // Create an inspector window Instance
            EditorWindow inspectorInstance = ScriptableObject.CreateInstance(inspectorType) as EditorWindow;
            // We display it - currently, it will inspect whatever gameObject is currently selected
            // So we need to find a way to let it inspect/aim at our target GO that we passed

            // 1. Cache the current selected gameObject
            UnityEngine.Object prevSelection = Selection.activeObject;
            // 2. Set the current selection to our target GO
            Selection.activeObject = targetGO;
            // 3. Get a ref to the "locked" property, which will lock the state of the inspector to the current inspected target
            var isLocked = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public);
            // 4. Invoke 'isLocked' setter method passing "true" to lock the inspector
            isLocked.GetSetMethod().Invoke(inspectorInstance, new object[] { true });
            // 5. Finally revert back to the previous selection so that other inspector will continue to inspector whatever they were inspecting
            Selection.activeObject = prevSelection;

            return inspectorInstance;
        }



        [OnInspectorGUI]
        [BoxGroup("BehaviorEditor/Property/Behaviors")]
        void DrawBehaviorList()
        {
            if (win == null) return;
            behaviorSelector.DrawBehaviorList();
        }

        void OnSceneViewGUI(SceneView sceneView)
        {
            if (win == null) return;
            timeline.DrawHitbox();

            if (showHitBoxInspector)
            {
                Handles.DrawWireCube(hitbox.transform.position, hitbox.transform.localScale);
            }
        }
    }
}
#endif