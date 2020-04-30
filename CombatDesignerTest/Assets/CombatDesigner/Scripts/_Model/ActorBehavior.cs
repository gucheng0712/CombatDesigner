using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace CombatDesigner
{
    /// <summary>
    /// ActorBehavior is a class that inherits from ScriptableObject.
    /// Represents the behavior data.
    /// </summary>
    [CreateAssetMenu(fileName = "NewActorBehavior", menuName = "ACT-System/ActorBehavior")]
    public class ActorBehavior : SerializedScriptableObject
    {
        #region Public Field      
        /// <summary>
        /// The Name of the behavior, corresponding to the animation state name
        /// </summary>
        [Tooltip("The Name of the behavior, corresponding to the Animation State name")]
        [FormerlySerializedAs("name")]
        public new string behaviorName;
        /// <summary>
        ///  The Input keycode of the behavior
        /// </summary>
        [Tooltip("The Input keycode of the behavior")]
        public KeyCode inputKey; // todo switch to key string for joystick support
        /// <summary>
        ///  Animation BlendRate
        /// </summary>
        [Tooltip("Animation BlendRate")]
        public float animBlend = 0.2f;
        /// <summary>
        /// The total frame length of the behavior
        /// </summary>
        [Tooltip("The total frame length of the behavior")]
        public double frameLength = 60;
        /// <summary>
        /// A flag to determine if loop the behavior
        /// </summary>
        [Tooltip("A flag to determine if loop the behavior")]
        public bool loopBehavior;
        /// <summary>
        /// The cooldown of the behavior. Zero means no cooldown
        /// </summary>
        [Tooltip("The cooldown of the behavior. Zero means no cooldown")]
        public float cooldown;
        /// <summary>
        /// The energy point cost
        /// </summary>
        [Tooltip("The energy point cost")]
        public int energyPointCost;
        /// <summary>
        /// How many air jump points cost
        /// </summary>
        [Tooltip("How many air jump points cost")]
        public int airJumpPointCost;
        /// <summary>
        /// Does this behavior require a target?
        /// </summary>
        [Tooltip("Does this behavior require a target?")]
        public bool requireTarget;
        /// <summary>
        /// Does this behavior require character to be grounded?
        /// </summary>
        [Tooltip("Does this behavior require character to be grounded?")]
        public bool requireGrounded;
        /// <summary>
        /// Does this behavior require character to be in the air?
        /// </summary>
        [Tooltip("Does this behavior require character to be in the air?")]
        public bool requireAerial;
        /// <summary>
        /// Does this behavior require character to be running?
        /// </summary>
        [Tooltip("Does this behavior require character to be running?")]
        public bool requireRunning;
        /// <summary>
        /// Does this behavior require character to be Attacking?
        /// </summary>
        [Tooltip("Does this behavior require character to be Attacking?")]
        public bool requireAttacking;
        /// <summary>
        /// Does this behavior require a character to be under attacked?
        /// </summary>
        [Tooltip("Does this behavior require a character to be under attacked?")]
        public bool requireBeingAttacked;
        /// <summary>
        /// Can this behavior break current behavior?
        /// </summary>
        [Tooltip("Can this behavior break current behavior?")]
        public bool canForceExecute;
        /// <summary>
        /// The Behavior Actions this behavior has
        /// </summary>
        [Tooltip("The Behavior Actions this behavior has")]
        [TabGroup("ActorBehaviorTab", "Behavior Action")]
        public List<IBehaviorAction> behaviorActions = new List<IBehaviorAction>();
        /// <summary>
        /// The Attack infomation this behavior has
        /// </summary>
        [Tooltip("The Attack infomation this behavior has")]
        [ListDrawerSettings(ShowIndexLabels = true), TabGroup("ActorBehaviorTab", "Behavior Attack")]
        public List<BehaviorAttack> attackInfos = new List<BehaviorAttack>();
        /// <summary>
        /// Timer Object
        /// </summary>
        CountDownTimer cdTimer;
        public List<ActorBehavior> followUps;
        public int priority;

        #endregion

        /// <summary>
        /// Initialization
        /// </summary>
        void OnEnable()
        {
            ResetTimer(cooldown);
        }

        /// <summary>
        /// A Method to Reset the Timer based on a cooldown value
        /// </summary>
        /// <param name="cd"> cooldown</param>
        private void ResetTimer(float cd)
        {
            cdTimer = new CountDownTimer(cd); // Create a timer for 5 seconds
            cdTimer.End();
        }

        /// <summary>
        /// The Condition Requirement to execute this behavior
        /// </summary>
        /// <param name="model"> ActorMode object </param>
        /// <returns></returns>
        public bool MetRequirements(ActorModel model)
        {
            // if character has recovered from an attack?
            // if (model.HitRecoverFrames > 0)
            // {
            //     return false;
            // }

            // if this behavior require character to be grounded?
            if (requireGrounded)
            {
                if (!model.cc.isGrounded)
                    return false;
            }

            // if this behavior require character to be in the air?
            if (requireAerial)
            {
                if (!model.isAerial)
                    return false;
            }

            // if this behavior require to have a target?
            if (requireTarget)
            {

            }

            // if this behavior can force cancel other behaviors?
            if (canForceExecute)
            {
                // Disable Hitbox when force execute Behavior                
                model.CanCancel = true;
                if (model.hitBox != null)
                {
                    model.hitBox.SetActive(false);
                    model.hitBox.SetLocalScale(Vector3.zero);
                }
            }

            // if this behavior can be canncel?
            if (model.CanCancel == false)
            {
                return false;
            }


            // if passing all the conditions, then return true
            return true;
        }
    }
}
