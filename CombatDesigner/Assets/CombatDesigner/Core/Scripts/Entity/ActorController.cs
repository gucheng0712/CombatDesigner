using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CombatDesigner
{
    /// <summary>
    /// A base class of the character's control logic
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(ActorModel))]
    public abstract class ActorController : MonoBehaviour
    {
        #region Fields
        /// <summary>
        ///  Core data of each character 
        /// </summary>
        public ActorModel model;



        /// <summary>
        /// A Delay to Aerial Condition
        /// </summary>
        protected float aerialThreahold = 0.15f;

        ActorBehavior nextBehavior;
        #endregion

        /// <summary>
        ///  Initialization in the Awake Function
        /// </summary>
        public virtual void Awake()
        {
            Init();
        }

        /// <summary>
        ///  Initialization for ActorController
        /// </summary>
        public virtual void Init()
        {
            if (model == null)
            {
                Debug.LogError("No ActorModel Detected");
                return;
            }
            model.Init(gameObject);
        }

        /// <summary>
        /// An virtual method of MonoBehavior's <b>Update()</b>.
        /// </summary>
        public virtual void Update()
        {
            // Check for Hit Pause 
            if (model.IsInHitPauseFrames())
                return;

            if (!model.isDead)
            {
                UpdateBuff();

                switch (model.actorType)
                {
                    case ActorType.AI:
                        UpdateAILogic();
                        break;
                    case ActorType.PLAYER:
                        UpdatePlayerLogic();
                        break;
                }
            }

            model.fsm.UpdateBehavior();
            UpdatePhysics();
            UpdateAnimation();
        }

        /// <summary>
        /// A method to update the buff system
        /// </summary>
        void UpdateBuff()
        {
            model.buffSystem.UpdateBuff();
        }

        /// <summary>
        /// A method to update character Animator. 
        /// Called in <b>Update()</b>
        /// </summary>
        protected virtual void UpdateAnimation()
        {
            if (model.anim == null) 
                return;
            model.animSpeed = 1; // Set overall animation speed to 1
            model.anim.speed = model.objectTimeScale;

            // Check if is in attack Pausing Frame
            if (model.CurrentHitPauseFrames > 0)
            {
                model.animSpeed = 0;
            }
        }

        /// <summary>
        /// A virtual method to update the Player control logics.
        /// Called in <b>Update()</b>
        /// </summary>
        protected virtual void UpdatePlayerLogic()
        {
            UpdatePlayerBehaviorCtrl();
        }

        /// <summary>
        ///  Update the Control such as input, condition logics to determine the Next Behavior
        /// </summary>
        void UpdatePlayerBehaviorCtrl()
        {
            // if meet all the requirements then start the next behavior
            if (CanStartNextBehavior())
            {
                model.StartBehavior(nextBehavior);
            }
        }

        /// <summary>
        ///  Return a bool to determine if meet the next behavior's condition
        /// </summary>
        /// <returns></returns>
        bool CanStartNextBehavior()
        {
            if (model.chainBehaviors.Count == 0 || model.chainBehaviors == null)
            {
                Debug.LogError("There is no chain connection in your behaivor, please create chain connection by using ChainEditor");
                return false;
            }

            // by default set to Default
            if (model.currentChainIndex >= model.chainBehaviors.Count)
            {
                model.currentChainIndex = 0;
            }

            int currentPriority = -1;
            for (int f = 0; f < model.chainBehaviors[model.currentChainIndex].followUps.Count; f++)
            {
                ChainBehavior nextChainBehavior = GetChainBehavior(f);
                ActorBehavior exceptedBehavior = nextChainBehavior.behavior;
                // Check Input buffer
                if (GameManager_Input.Instance.bufferKeys.Any(p => p == exceptedBehavior.inputKey || p == exceptedBehavior.altInputKey)) // todo maybe add key combo and motion input buffer 
                {
                    // the behaviors will have certain requirements
                    if (exceptedBehavior.MetRequirements(model))
                    {
                        // Update the current priority level
                        if (UpdatePriorityLevel(nextChainBehavior.priority, ref currentPriority))
                        {
                            // if there is no followups, then go to the index 1 which is Default behavior
                            model.currentChainIndex = (nextChainBehavior.followUps.Count > 0) ? nextChainBehavior.idIndex : 0;

                            //nextBehavior = model.GetBehavior(nextInputInfo.behaviorIndex);
                            nextBehavior = model.GetBehavior(exceptedBehavior.name);


                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///  Get the ChainBehavior by Index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        ChainBehavior GetChainBehavior(int index)
        {
            int nextChainBehaviorIndex = model.chainBehaviors[model.currentChainIndex].followUps[index];
            ChainBehavior nextChainBehavior = model.chainBehaviors[nextChainBehaviorIndex];
            return nextChainBehavior;
        }

        /// <summary>
        ///  Check the priority of the Behavior
        /// </summary>
        /// <param name="nextPriority"></param>
        /// <param name="currentPriority"></param>
        /// <returns></returns>
        bool UpdatePriorityLevel(int nextPriority, ref int currentPriority)
        {
            bool result = nextPriority > currentPriority;
            if (result == true)
                currentPriority = nextPriority; // Update the priority
            return result;
        }

        /// <summary>
        /// A virtual method to update the AI Control. 
        /// Called in <b>Update()</b>
        /// </summary>
        protected virtual void UpdateAILogic()
        {
            // todo add enemy behavior tree
        }

        /// <summary>
        /// A virtual method to update the Physics.
        /// Called in <b>Update()</b>
        /// </summary>
        public virtual void UpdatePhysics()
        {
            UpdateVelocity();

            UpdateGrounndCheckInfo();

            UpdateFriction();
        }

        /// <summary>
        ///A virtual method to update the overall velocity. 
        /// Called in <b>UpdatePhysics()</b>
        /// </summary>
        void UpdateVelocity()
        {
            model.cc?.Move(model.velocity * model.moveSpeedModifier * model.objectTimeScale * Time.deltaTime); // Move the actor based on the velocity
            //model.rb.MovePosition(model.velocity * model.objectTimeScale * Time.deltaTime);
            model.velocity.y += model.gravity * model.objectTimeScale * Time.deltaTime; // gravity
            model.velocity.y = Mathf.Max(model.velocity.y, -20f); // the max value of gravity velocity will be -100;
        }

        /// <summary>
        /// A virtual method to update the Infomation based on grounded or not. 
        /// Called in <b>UpdatePhysics()</b>
        /// </summary>
        void UpdateGrounndCheckInfo()
        {
            //Grounded Check 
            if (model.cc.isGrounded)
            {
                UpdateGrounededInfo();
            }
            else
            {
                UpdateAerialInfo();
            }
        }

        /// <summary>
        /// A virtual method to update the <b><i>Ungroundeded</i></b> Infomation. 
        /// Called in <b>UpdateGroundCheckInfo()</b>
        /// </summary>
        protected virtual void UpdateAerialInfo()
        {
            // if is in falling threhold
            if (!model.isAerial)
            {
                model.aerialTimer += Time.deltaTime;// start counting time to be in Aerial state
            }

            if (model.aerialTimer >= aerialThreahold)
            {
                model.isAerial = true;
            }
        }

        /// <summary>
        /// A virtual method to update the <b><i>Groundeded</i></b> Infomation. 
        /// Called in <b>UpdateGroundCheckInfo()</b>
        /// </summary>
        protected virtual void UpdateGrounededInfo()
        {
            model.velocity.y = 0;

            model.isAerial = false;
            model.aerialTimer = 0;
        }

        /// <summary>
        /// A virtual method to update the Friction. 
        /// Called in <b>UpdatePhysics()</b>
        /// </summary>
        protected virtual void UpdateFriction()
        {
            // Update the friction based on model's friction 
            Vector3 friction = new Vector3(model.friction.x * model.objectTimeScale, model.friction.y, model.friction.z * model.objectTimeScale);

            // Scale the velocity based on the fraction
            model.velocity.Scale(friction); // add friction to player make it slow down
        }

    }
}
