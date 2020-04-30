
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using CombatDesigner;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace CombatDesigner.EditorTool
{

    public class View_Timeline
    {
        ActorController actorController;
        ActorModel model;
        public int totalStateFrames;
        int currentStateFrame;

        float currentAnimDuration;
        float normalizedTime;

        //bool preview = false;
        bool play = false;

        public void Init()
        {
            if (BehaviorEditorWindow.win.actorController != null)
            {
                actorController = BehaviorEditorWindow.win.actorController;
                model = actorController.model;
            }
        }

        public void DrawTimeLine()
        {
            // the framelength of current selected behavior
            totalStateFrames = (int)BehaviorEditorWindow.win.selectedBehavior.frameLength;

            // null reference check
            if (model == null)
            {
                return;
            }
            if (model.anim == null)
            {
                Debug.LogError(" No Animator Detected. In order to preview the behavior,  Animator is required");
                return;
            }

            // Draw the Menu bar above the timeline
            DrawTimeLineMenuBar();

            Rect playTrackRect = DrawTimeLinePlayTrack();

            // Draw KeyFrames of the time line
            DrawKeyFrames(playTrackRect);

            // Draw PlayHead of the timeline
            DrawPlayHead(playTrackRect);

            // add a little bit space at the end
            GUILayout.Space(30);
        }

        public void UpdateFrameInfo()
        {

        }

        private void DrawPlayHead(Rect playTrackRect)
        {
            float currrentFrameX = playTrackRect.x + currentStateFrame * playTrackRect.width / totalStateFrames;
            Handles.color = Color.red;
            Handles.DrawAAPolyLine(3, new Vector3(currrentFrameX, playTrackRect.yMin), new Vector3(currrentFrameX, playTrackRect.yMax));
        }

        private Rect DrawTimeLinePlayTrack()
        {
            // outter Rect of the Timeline
            var borderRect = EditorGUILayout.GetControlRect(false, 30);
            EditorGUI.DrawRect(borderRect, Color.black);
            //inner Rect of the Timeline
            var innerRect = new Rect(borderRect.x + 2, borderRect.y + 2, borderRect.width - 4, borderRect.height - 4);
            EditorGUI.DrawRect(innerRect, Color.grey);
            return innerRect;
        }

        void DrawKeyFrames(Rect innerRect)
        {
            // the space between each keyframe  
            float singleSpace = innerRect.width / totalStateFrames;
            // a threshold to hide the number label
            int threshold = (int)(totalStateFrames / singleSpace);
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < totalStateFrames; i++)
            {
                EditorGUI.DrawRect(new Rect(innerRect.x + i * singleSpace, innerRect.y + 15, 1, innerRect.height - 18), Color.black);

                if (threshold > 0)
                {
                    if (i % threshold == 0)
                    {
                        EditorGUI.LabelField(new Rect(innerRect.x + i * singleSpace, innerRect.y + 30, 20, innerRect.height - 13), i.ToString());
                    }
                }
                else if (threshold == 0)
                {
                    EditorGUI.LabelField(new Rect(innerRect.x + i * singleSpace, innerRect.y + 30, 20, innerRect.height - 13), i.ToString());
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Drawing the Buttons for The timeline
        /// </summary>
        void DrawTimeLineMenuBar()
        {
            EditorGUILayout.BeginHorizontal();

            GUI.contentColor = Color.black;
            // Previous Frame Btn
            if (SirenixEditorGUI.IconButton(EditorIcons.Previous, 20, 20))
            {
                if (currentStateFrame > 0)
                {
                    currentStateFrame--;
                    model.e_currentStateTime -= BehaviorEditorWindow.deltaTime;
                    UpdateStepBehavior();
                }
            }
            GUI.contentColor = Color.white;

            // Play or Pause Btn Frame Btn
            if (play == false)
            {
                if (SirenixEditorGUI.IconButton(EditorIcons.Play, 20, 20))
                {
                    PlayAnimationPreview();
                }
            }
            else
            {
                if (SirenixEditorGUI.IconButton(EditorIcons.Pause, 20, 20))
                {
                    PauseAnimatorPreview();
                }
            }

            // Next Frame Btn
            if (SirenixEditorGUI.IconButton(EditorIcons.Next, 20, 20))
            {
                if (currentStateFrame < totalStateFrames)
                {
                    currentStateFrame++;
                    model.e_currentStateTime += BehaviorEditorWindow.deltaTime;
                    UpdateStepBehavior();
                }
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField("CurrentFrame", GUILayout.Width(90));
            EditorGUI.BeginChangeCheck();
            currentStateFrame = EditorGUILayout.IntField(currentStateFrame, GUILayout.Width(30), GUILayout.Height(16));
            currentStateFrame = Mathf.Clamp(currentStateFrame, 0, totalStateFrames);
            model.e_currentStateTime = currentStateFrame * BehaviorEditorWindow.deltaTime;
            if (EditorGUI.EndChangeCheck())
            {
                UpdateStepBehavior();
            }

            GUILayout.Space(5);
            EditorGUILayout.LabelField("|", GUILayout.Width(5));
            GUILayout.Space(5);

            EditorGUILayout.LabelField("PlaySpeed", GUILayout.Width(70));
            EditorGUI.BeginChangeCheck();
            BehaviorEditorWindow.playbackSpeed = EditorGUILayout.Slider(BehaviorEditorWindow.playbackSpeed, 0, 1, GUILayout.Width(120), GUILayout.Height(16));
            if (EditorGUI.EndChangeCheck())
            {
                Time.timeScale = BehaviorEditorWindow.playbackSpeed;
            }
            EditorGUILayout.EndHorizontal();
        }

        void UpdateStepBehavior()
        {
            if (play == true)
                play = false;
            normalizedTime = model.e_currentStateTime / currentAnimDuration;
            PlayAnimation();
            UpdateBehaviorActions();
            model.anim.Update(BehaviorEditorWindow.deltaTime);
        }


        void PauseAnimatorPreview()
        {
            play = false;
        }
        void PlayAnimationPreview()
        {
            play = true;
        }

        void ResetAnimatorPreview()
        {
            GameObject.DestroyImmediate(model.vfxGO);
            model.e_currentStateTime = 0;
            currentStateFrame = 0;
            normalizedTime = 0;
            var cc = BehaviorEditorWindow.win.character.GetComponent<CharacterController>();
            cc.SimpleMove(Vector3.zero);
            model.velocity = Vector3.zero;
            BehaviorEditorWindow.win.character.transform.position = Vector3.zero;
            model.anim.Play("Default");
            model.anim.Update(0);
            play = false;
        }

        public void UpdateAnimationPreviewInEditor()
        {
            if (EditorApplication.isPlaying)
            {
                if(play == true)
                {
                    model.StartBehavior(BehaviorEditorWindow.win.selectedBehavior);
                    play =false;
                }
                return;
            }

            if (play == false)
            {
                return;
            }
            PlayAnimation();

            model.animMoveSpeed = 1;
            // Update Behavior Actions
            UpdateBehaviorActions();
            // UpdateVFX();

            if (skipCount < 1)
            {
                skipCount += BehaviorEditorWindow.playbackSpeed;
                if (skipCount >= 1)
                {
                    skipCount = 0;
                    currentStateFrame++;
                }
            }

            model.e_currentStateTime += BehaviorEditorWindow.deltaTime * BehaviorEditorWindow.playbackSpeed;
            normalizedTime = model.e_currentStateTime / currentAnimDuration;
            // Update Character's Physics
            actorController.UpdatePhysics();

            model.anim.Update(BehaviorEditorWindow.deltaTime);

            if (currentStateFrame >= BehaviorEditorWindow.win.selectedBehavior.frameLength)
            {
                Debug.Log("Reset");
                ResetAnimatorPreview();
            }
        }
        float skipCount = 0;
        void PlayAnimation()
        {
            AnimatorController controller = actorController.gameObject.GetComponentInChildren<Animator>().runtimeAnimatorController as AnimatorController;
            AnimatorStateMachine asm = controller.layers[0].stateMachine;
            foreach (ChildAnimatorState cas in asm.states)
            {
                if (cas.state.name == BehaviorEditorWindow.win.selectedBehavior.name)
                {
                    if (cas.state.motion != null)
                    {
                        currentAnimDuration = cas.state.motion.averageDuration;
                        // Debug.Log(currentAnimDuration);
                        model.anim.Play(BehaviorEditorWindow.win.selectedBehavior.name, 0, normalizedTime);
                        break;
                    }

                }
            }
        }
        private void UpdateBehaviorActions()
        {
            foreach (IBehaviorAction action in BehaviorEditorWindow.win.selectedBehavior.behaviorActions)
            {
                if (currentStateFrame >= action.startFrame && currentStateFrame <= action.endFrame)
                {
                    action.ExecuteInEditor(model, normalizedTime);
                }
            }
        }

 

        public void DrawHitbox()
        {
            if (BehaviorEditorWindow.win.selectedBehavior == null || currentStateFrame == 0)
                return;

            foreach (BehaviorAttack atkInfo in BehaviorEditorWindow.win.selectedBehavior.attackInfos)
            {
                if (currentStateFrame >= atkInfo.frameInfo.startFrame && currentStateFrame <= atkInfo.frameInfo.startFrame + atkInfo.frameInfo.length)
                {
                    Handles.DrawWireCube(atkInfo.hitboxInfo.hitBoxPos + BehaviorEditorWindow.win.character.transform.position, atkInfo.hitboxInfo.hitBoxScale);
                }
            }
        }
    }
}
#endif