
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using CombatDesigner;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class BehaviorEditorWindow : OdinEditorWindow
{
    public static BehaviorEditorWindow win;
    public static float deltaTime = 1 / 60f;
    public static float playbackSpeed = 1f;
    public static ActorBehavior selectedBehavior;

    [InfoBox("Please Select a Character Prefab to Preview", "IsCharacterNull")]
    [BoxGroup("BehaviorEditor/Property")]
    [LabelWidth(100)]
    [OnValueChanged("GetComponentsCallback")]
    public GameObject character;

    HitBox hitbox;
    EditorWindow hitboxInspactor;
    EditorWindow modelInspector;
    EditorWindow behaviorInspector;

    bool showHitBoxInspector;
    bool showModelInspector;
    bool showBehaviorInspector;
    [HideInInspector] public ActorController actorController;

    bool IsCharacterNull()
    {
        return character == null;
    }

    void GetComponentsCallback()
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

            timeline.Init();
            behaviorInfo.Init();
            behaviorSelector.Init();
        }
    }



    [HideInInspector] public CharacterController cc;
    [HideInInspector] public AnimatorController controller;

    [HideInInspector] public View_Timeline timeline = new View_Timeline();
    View_BehaviorInfo behaviorInfo = new View_BehaviorInfo();
    View_BehaviorSelector behaviorSelector = new View_BehaviorSelector();

    [MenuItem("CombatDesigner/BehaviorEditor")]
    static void Open()
    {
        win = GetWindow<BehaviorEditorWindow>();
    }

    protected override void OnEnable()
    {
        Time.timeScale = 1;
        base.OnEnable();
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

        EditorApplication.update += timeline.UpdateAnimationPreviewInEditor;
        SceneView.onSceneGUIDelegate += OnSceneViewGUI;
        
    }

    private void OnDisable()
    {
        EditorApplication.update -= timeline.UpdateAnimationPreviewInEditor;
        SceneView.onSceneGUIDelegate -= OnSceneViewGUI;
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
    [BoxGroup("BehaviorEditor/Timeline/InspectorToggleGroup")]
    void DrawBehaviorInfoToolBar()
    {
        if (win == null || actorController == null) return;

        behaviorInfo.DrawBehaviorInfoToolBar();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        DrawInspectorToggleBtn("Hitbox Inspector", Color.green, hitbox, ref hitboxInspactor, ref showHitBoxInspector);

        DrawInspectorToggleBtn("Model Inpector", Color.red, actorController.model, ref modelInspector, ref showModelInspector);

        DrawInspectorToggleBtn("Behavior Inspector", Color.yellow, selectedBehavior, ref behaviorInspector, ref showBehaviorInspector);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

    }

    void DrawInspectorToggleBtn(string label, Color btnColor, UnityEngine.Object objToInspect, ref EditorWindow inspectorWindow, ref bool showInspectorFlag)
    {
        if (objToInspect != null)
        {
            var style = new GUIStyle(GUI.skin.button);
            GUI.backgroundColor = btnColor;
            showInspectorFlag = GUILayout.Toggle(showInspectorFlag, label, style, GUILayout.Width(150), GUILayout.Height(40));
            if (showInspectorFlag)
            {
                if (inspectorWindow == null)
                {
                    inspectorWindow = GetInspectTarget(objToInspect);
                    inspectorWindow.Show();
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
#endif