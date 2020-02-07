using System;
using UnityEngine;

namespace CombatDesigner
{
    /// <summary>
    /// A base class of the character's control logic
    /// </summary>
    public abstract class ActorController : MonoBehaviour
    {
        /// <summary>
        ///  Core data of each character 
        /// </summary>
        public ActorModel model;

        /// <summary>
        /// A Delay to Aerial Condition
        /// </summary>
        protected float aerialThreahold = 15f;

        ActorBehavior nextBehavior;

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
            if (!model) Debug.LogError("No ActorModel Detected");
            model.Init(gameObject);
        }

        /// <summary>
        /// An virtual method of MonoBehavior's <b>Update()</b>.
        /// </summary>
        public virtual void Update()
        {
            switch (model.actorType)
            {
                case ActorType.AI:
                    UpdateAILogic();
                    break;
                case ActorType.PLAYER:
                    UpdatePlayerLogic();
                    break;
            }

            model.fsm.UpdateBehavior();

            UpdatePhysics();
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

            // by default set to Neutral
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
                if (GameManager_Input.Instance.bufferKeys.Contains(exceptedBehavior.inputKey)) // todo maybe add key combo and motion input buffer 
                {
                    // the behaviors will have certain requirements
                    if (exceptedBehavior.MetRequirements(model))
                    {
                        // Update the current priority level
                        if (UpdatePriorityLevel(nextChainBehavior.priority, ref currentPriority))
                        {
                            // if there is no followups, then go to the index 1 which is neutral behavior
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
        protected virtual void UpdateVelocity()
        {
            // Update the velocity based on model's velocity and model's local time scale
            Vector3 velocity = new Vector3(model.velocity.x * model.objectTimeScale,
                                            model.velocity.y * model.objectTimeScale * model.objectTimeScale,
                                            model.velocity.z * model.objectTimeScale);

            model.cc.Move(velocity * Time.deltaTime); // Move the actor based on the velocity

            model.velocity.y += model.gravity * Time.fixedDeltaTime; // gravity
            model.velocity.y = Mathf.Max(model.velocity.y, -25f); // the max value of gravity velocity will be -100;
        }

        /// <summary>
        /// A virtual method to update the Infomation based on grounded or not. 
        /// Called in <b>UpdatePhysics()</b>
        /// </summary>
        protected virtual void UpdateGrounndCheckInfo()
        {
            //Grounded Check 
            if (model.cc.isGrounded)
            {
                UpdateGrounededInfo();
            }
            else
            {
                UpdateUngrounededInfo();
            }
        }

        /// <summary>
        /// A virtual method to update the <b><i>Ungroundeded</i></b> Infomation. 
        /// Called in <b>UpdateGroundCheckInfo()</b>
        /// </summary>
        protected virtual void UpdateUngrounededInfo()
        {
            // if is in falling threhold
            if (!model.isAerial)
            {
                model.aerialTimer++;// start counting time to be in Aerial state
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
            model.velocity.Scale(model.friction); // add friction to player make it slow down
        }

        /// <summary>
        /// A virtual method about get hit event
        /// </summary>
        /// <param name="attacker"></param>
        public virtual void GetHit(ActorController attacker)
        {

        }
    }
}
