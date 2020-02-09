using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace CombatDesigner
{

    /// <summary>
    /// A base class of behavior action 
    /// </summary>
    [System.Serializable]
    public abstract class IBehaviorAction
    {
        /// <summary>
        /// Starting frame of this behavior action 
        /// </summary>
        public float startFrame;

        /// <summary>
        /// Ending frame of the behavior action 
        /// </summary>
        public float endFrame;

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
