using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace CombatDesigner
{

    /// <summary>
    /// A base class of behavior action 
    /// </summary>
    [System.Serializable]
    public abstract class IBehaviorAction
    {
        [MinMaxSlider("FrameRange", true)]
        public Vector2 MinMaxSlider = new Vector2(0, 60);

        /// <summary>
        /// Starting frame of this behavior action 
        /// </summary>
        [FormerlySerializedAs("startFrame")]
        public float startFrame;

        /// <summary>
        /// Ending frame of the behavior action 
        /// </summary>
        [FormerlySerializedAs("endFrame")]
        public float endFrame;

        [OnInspectorGUI]
        public void DrawProperty()
        {
            EditorGUILayout.MinMaxSlider(ref startFrame, ref endFrame, 0, 100);
            startFrame = Mathf.Floor(startFrame);
           endFrame = Mathf.Floor(endFrame);
        }

        /// <summary>
        /// A virtual method to Initialize the behavior action .
        /// </summary>
        /// <param name="model"></param>
        public virtual void Init(ActorModel model)
        {

        }

        /// <summary>
        /// A virtual mehtod of entering the behavior action  event
        /// </summary>
        /// <param name="model"></param>
        public virtual void Enter(ActorModel model)
        {

        }

        /// <summary>
        /// A virtual mehtod of updating the behavior action  event
        /// </summary>
        /// <param name="model"></param>
        public virtual void Execute(ActorModel model)
        {

        }

        /// <summary>
        /// A virtual method of updating the behavior action  event in Editor Mode
        /// </summary>
        /// <param name="model"></param>
        public virtual void ExecuteInEditor(ActorModel model)
        {

        }

        /// <summary>
        /// A virtual mehtod of exiting the behavior action  event
        /// </summary>
        /// <param name="model"></param>
        public virtual void Exit(ActorModel model)
        {

        }
    }
}
