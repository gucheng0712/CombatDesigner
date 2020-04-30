using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace CombatDesigner
{
    [System.Serializable]
    public struct HurtboxTransform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    /// <summary>
    /// ActorBehavior is a class that inherits from ScriptableObject.
    /// Represents the behavior data.
    /// </summary>
    [CreateAssetMenu(fileName = "NewActorBehavior", menuName = "CombatDesigner/ActorBehavior")]
    public class ActorBehavior : SerializedScriptableObject
    {
        #region Public Field      

        /// <summary>
        ///  The Input keycode of the behavior
        /// </summary>
        [Tooltip("The Input keycode of the behavior")]
        [LabelWidth(120)]
        public KeyCode inputKey; // todo switch to key string for joystick support

        /// <summary>
        ///  The Alternative Input keycode of the behavior
        /// </summary>
        [Tooltip("The Alternative Input keycode of the behavior")]
        [LabelWidth(120)]
        public KeyCode altInputKey;


        /// <summary>
        ///  The animation clip of the behavior
        /// </summary>
        [Tooltip("The animation clip of the behavior")]
        [LabelWidth(120)]
        public AnimationClip animClip;

        /// <summary>
        ///  Animation BlendRate
        /// </summary>
        [Tooltip("Animation BlendRate")]
        [LabelWidth(120)]
        public float blendRate = 0.2f;

        /// <summary>
        /// The total frame length of the behavior
        /// </summary>
        [Tooltip("The total frame length of the behavior")]
        [LabelWidth(120)]
        public double frameLength = 60;

        /// <summary>
        ///  The hurtbox transformation of the behavior
        /// </summary>
        [Tooltip("The hurtbox transformation of the behavior")]
        [HideInInspector] public HurtboxTransform hurtBoxInfo = new HurtboxTransform() { position = Vector3.zero, rotation = Quaternion.identity, scale = Vector3.one };

        /// <summary>
        /// The cooldown of the behavior. Zero means no cooldown
        /// </summary>
        [Tooltip("The cooldown of the behavior. Zero means no cooldown")]
        [LabelWidth(120)]
        public float cooldown;

        /// <summary>
        /// The energy point cost
        /// </summary>
        [Tooltip("The energy point cost")]
        [LabelWidth(120)]
        public int energyPointCost;

        /// <summary>
        /// How many air jump points cost
        /// </summary>
        [Tooltip("How many air jump points cost")]
        [LabelWidth(120)]
        public int airJumpPointCost;


        /// <summary>
        /// The current behavior's friction
        /// </summary>
        [Tooltip("The current behavior's friction")]
        [LabelWidth(120)]
        public Vector3 behaviorFriction = new Vector3(0.5f, 1f, 0.5f);

        /// <summary>
        /// The angle of current behavior that can block attack
        /// </summary>
        [Tooltip("The angle of current behavior that can block attack")]
        [LabelWidth(120)]
        public float atkBlockingAngle;

        /// <summary>
        /// A flag to determine the Behavior is active or not.
        /// </summary>
        [Tooltip("A flag to determine the Behavior is active or not.")]
        [FoldoutGroup("Flags")]
        public bool isActivated = true;

        /// <summary>
        /// A flag to determine if loop the behavior
        /// </summary>
        [Tooltip("A flag to determine if loop the behavior")]
        [FoldoutGroup("Flags")]
        public bool loopBehavior;

        /// <summary>
        /// A flag to determine if the behavior is a hurt/damaged behavior such as get hit behavior
        /// </summary>
        [Tooltip("A flag to determine if the behavior is a hurt/damaged behavior such as get hit behavior")]
        [FoldoutGroup("Flags")]
        public bool isHurtBehavior;


        /// <summary>
        /// Does this behavior require a target?
        /// </summary>
        [Tooltip("Does this behavior require a target?")]
        [FoldoutGroup("Flags")]
        public bool requireTarget;

        /// <summary>
        /// Does this behavior require to under attack?
        /// </summary>
        [Tooltip("Does this behavior require to under attack?")]
        [FoldoutGroup("Flags")]
        public bool requireUnderAttack;

        /// <summary>
        /// Does this behavior require character to be grounded?
        /// </summary>
        [Tooltip("Does this behavior require character to be grounded?")]
        [FoldoutGroup("Flags")]
        public bool requireGrounded;

        /// <summary>
        /// Does this behavior require character to be in the air?
        /// </summary>
        [Tooltip("Does this behavior require character to be in the air?")]

        [FoldoutGroup("Flags")]
        public bool requireAerial;

        /// <summary>
        /// Does this behavior require character to be running?
        /// </summary>
        [Tooltip("Does this behavior require character to be running?")]
        [FoldoutGroup("Flags")]
        public bool requireRunning;

        /// <summary>
        /// Does this behavior require character to be Attacking?
        /// </summary>
        [Tooltip("Does this behavior require character to be Attacking?")]
        [FoldoutGroup("Flags")]
        public bool requireAttacking;

        /// <summary>
        /// Can this behavior break current behavior?
        /// </summary>
        [Tooltip("Can this behavior break current behavior?")]
        [FoldoutGroup("Flags")]
        public bool canForceExecute;

        /// <summary>
        /// Events that will be executed while under attack
        /// </summary>
        [Tooltip("Get Hurt Events that will be executed while undering attack")]
        [TabGroup("UnderAttackEventTab", "Hurt")]
        public List<IUnderAttack> getHurtEvents = new List<IUnderAttack>();

        /// <summary>
        /// Events that will be executed while under attack
        /// </summary>
        [Tooltip("Block Hit Events that will be executed while undering attack")]
        [TabGroup("UnderAttackEventTab", "Block")]
        public List<IUnderAttack> blockHitEvents = new List<IUnderAttack>();

        /// <summary>
        /// The Behavior Actions this behavior has
        /// </summary>
        [Tooltip("The Behavior Actions this behavior has")]
        [TabGroup("ActorBehaviorTab", "Action")]
        public List<IBehaviorAction> behaviorActions = new List<IBehaviorAction>();
        /// <summary>
        /// The Attack infomation this behavior has
        /// </summary>
        [Tooltip("The Attack infomation this behavior has")]
        [ListDrawerSettings(ShowIndexLabels = true), TabGroup("ActorBehaviorTab", "Attack")]
        public List<BehaviorAttack> attackInfos = new List<BehaviorAttack>();

        /// <summary>
        /// Timer Object
        /// </summary>
        CountDownTimer cdTimer;

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

            if (!isActivated)
            {
                return false;
            }
            // if character has recovered from an attack?
            //if (model.HitRecoverFrames > 0)
            //{
            //    return false;
            //}

            // if this behavior require character to be grounded?
            if (requireGrounded)
            {
                if (!model.cc.isGrounded)
                {
                    CombatDebugger.Log("Grounded requirement failed", LogDomain.BehaviorRequirement);
                    return false;
                }
            }

            if (requireRunning)
            {
                if (model.moveInputDir.magnitude <= 0.75f)
                {
                    CombatDebugger.Log("Running requirement failed", LogDomain.BehaviorRequirement);
                    return false;
                }
            }

            // if this behavior require character to be in the air?
            if (requireAerial)
            {
                if (!model.isAerial)
                {
                    CombatDebugger.Log("In Air requirement failed", LogDomain.BehaviorRequirement);
                    return false;
                }
            }

            // if this behavior require to have a target?
            if (requireTarget)
            {
                if (model.target == null)
                {
                    return false;
                }
            }

            // if this behavior can force cancel other behaviors?
            if (canForceExecute && !model.currentBehavior.canForceExecute && !model.currentBehavior.isHurtBehavior)
            {
                // Disable Hitbox when force execute Behavior                
                model.CanCancel = true;
                if (model.hitBox != null)
                {
                    model.hitBox.SetActive(false);
                    model.hitBox.SetLocalScale(Vector3.zero);
                }
            }
            if (requireUnderAttack)
            {

                if (model.currentBehavior.isHurtBehavior)
                    model.CanCancel = true;
                else
                    return false;
            }


            // if this behavior can be canncel?
            if (model.CanCancel == false)
            {
                CombatDebugger.Log("CanCancel requirement failed", LogDomain.BehaviorRequirement);
                return false;
            }
            else
            {
                model.CanCancel = false;
            }

            // if the behavior have CoolDown
            if (cooldown > 0)
            {
                if (!cdTimer.IsTimeUp)
                {
                    CombatDebugger.Log("Cooldown requirement failed", LogDomain.BehaviorRequirement);
                    return false; // if count down haven't finish, return
                }
                ResetTimer(cooldown);
                cdTimer.Start(); // Start count down again
            }

            // if the character have enough energy to execute this behavior
            if (model.actorStats.currentEnergy < energyPointCost)
            {
                CombatDebugger.Log("PowerMeter requirement failed", LogDomain.BehaviorRequirement);
                return false;
            }

            // if the character have enough air jump points to execute this behavior
            if (model.actorStats.currentAirJumpPoint < airJumpPointCost)
            {
                CombatDebugger.Log("AirJumpPoints requirement failed", LogDomain.BehaviorRequirement);
                return false;
            }

            // if passing all the conditions, then return true
            return true;
        }
    }
}