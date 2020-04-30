
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{
    public class ActorFSM
    {
        /// <summary>
        ///  Core Data of each character model
        /// </summary>
        ActorModel model;

        /// <summary>
        ///  Locked Framerate
        /// </summary>
        float fps;

        /// <summary>
        ///  Accumilated delta time
        /// </summary>
        float accumilatedTime;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        public ActorFSM(ActorModel model)
        {
            this.model = model;
            fps = 1.0f / GameManager_Settings.targetFrameRate;
        }

        /// <summary>
        /// Get behavior based on its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActorBehavior GetBehavior(string name)
        {
            foreach (var behavior in model.behaviors)
            {
                if (behavior.name == name)
                {
                    return behavior;
                }
            }
            Debug.LogError("Can't find the Behavior named " + name);
            return null;
        }

        /// <summary>
        ///  Get Behavior based on its index in behavior list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ActorBehavior GetBehavior(int index)
        {
            return model.behaviors[index];
        }

        /// <summary>
        ///  Start a new behavior
        /// </summary>
        /// <param name="newBehavior"></param>
        public void StartBehavior(ActorBehavior newBehavior)
        {
            if (newBehavior == null)
                return;
            // Reset the behavior actions' params for prevent some behaior enter new behavior before exiting the behavior
            ResetBehaviorActions(model.currentBehavior);
            model.currentFrame = 0; // reset current frame
            model.previousFrame = -1;
            model.currentBehavior = newBehavior; // update current behavior to new behavior

            if (model.currentBehavior == GetBehavior("Default") || model.currentBehavior.isHurtBehavior || model.currentBehavior.canForceExecute)
            {
                model.currentChainIndex = 0;
            }
            model.CanCancel = false;

            // Reset hit info
            model.HitConfirm = false;
            model.friction = model.currentBehavior.behaviorFriction;
            UpdateHurtBoxTransformation(model.currentBehavior); // update current behavior's position, rotation, and scale
            model.anim?.CrossFadeInFixedTime(model.currentBehavior.name, model.currentBehavior.blendRate);
        }

        /// <summary>
        /// Update  behavior's position, rotation, and scale
        /// </summary>
        void UpdateHurtBoxTransformation(ActorBehavior behavior)
        {
            if (model.hurtBox != null)
            {
                HurtboxTransform hurtbox = behavior.hurtBoxInfo;
                model.hurtBox.SetLocalPosition(hurtbox.position);
                model.hurtBox.SetLocalRotation(hurtbox.rotation);
                model.hurtBox.SetLocalScale(hurtbox.scale);
            }
        }

        /// <summary>
        ///  Reset all behavior actions
        /// </summary>
        void ResetBehaviorActions(ActorBehavior behavior)
        {
            if (behavior != null)
            {
                foreach (IBehaviorAction act in behavior.behaviorActions)
                {
                    if (act != null)
                    {
                        act.Exit(model);
                    }
                }
            }
        }

        /// <summary>
        /// Loop the behavior by reset the frame
        /// </summary>
        public void LoopBehavior()
        {
            model.currentFrame = 0;
            //  accumilatedTime = 0;
        }

        /// <summary>
        /// End the Behavior, automaticly go back to the Default Behavior
        /// </summary>
        public void EndBehavior()
        {
            StartBehavior(GetBehavior("Default"));
            //  accumilatedTime = 0;
        }

        /// <summary>
        /// Get the names of the behavior list
        /// </summary>
        /// <returns></returns>
        public string[] GetBehaviorNames()
        {
            string[] names = new string[model.behaviors.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = model.behaviors[i].name;
            }
            return names;
        }

        /// <summary>
        ///  Get the chainbehavior names
        /// </summary>
        /// <returns></returns>
        public string[] GetChainBehaviorNames()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the behavior action names
        /// </summary>
        /// <returns></returns>
        public string[] GetBehaviorActionNames()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Initialize the behavior actions
        /// </summary>
        public void InitBehaviorActions()
        {
            foreach (ActorBehavior behavior in model.behaviors)
            {
                foreach (IBehaviorAction e in behavior.behaviorActions)
                {
                    if (e != null)
                    {
                        e.Init(model);
                    }
                }
            }
        }


        /// <summary>
        ///  Update the behavior
        /// </summary>
        public void UpdateBehavior()
        {
            if (model.currentBehavior == null)
            {
                return;
            }

            accumilatedTime += Time.deltaTime;
            while (accumilatedTime > fps)
            {
                // if the current frame reach to the current behavior's end frame
                int scaledFrameLength = (int)(model.currentBehavior.frameLength / model.objectTimeScale);
                if (model.currentFrame >= scaledFrameLength)
                {
                    // if this behavior is toggled loop
                    if (model.currentBehavior.loopBehavior)
                        model.LoopBehavior();
                    else
                        model.EndBehavior();
                }
                model.currentFrame += 1;

                CombatDebugger.Log(scaledFrameLength.ToString(), LogDomain.FrameInfo);
                UpdateBehaviorActions();
                UpdateBehaviorAttack();

                // set the previouse frame to current frame at the end 
                model.previousFrame = model.currentFrame;
                accumilatedTime -= fps;
            }
        }

        /// <summary>
        ///  Update all Behavior Actions in current behavior
        /// </summary>
        void UpdateBehaviorActions()
        {
            // loop through all behaviors
            foreach (IBehaviorAction act in model.currentBehavior.behaviorActions)
            {
                if (act != null)
                {
                    int scaledStartFrame = (int)((act.startFrame + 1) / model.objectTimeScale);
                    int scaledEndFrame = (int)((act.endFrame+1) / model.objectTimeScale);
                    // when current frame equal start frame of this action
                    if (model.currentFrame == scaledStartFrame)
                    {
                        act.Enter(model);
                    }
                    // when current frame in between start frame and end frame
                    if (model.currentFrame >= scaledStartFrame && model.currentFrame <= scaledEndFrame)
                    {
                        act.Execute(model);
                    }
                    // when current frame equal end frame of this action
                    if (model.currentFrame >= scaledEndFrame)
                    {
                        act.Exit(model);
                    }
                }
            }
        }

        /// <summary>
        /// A method to update all Behavior AttackInfo in current hehavior
        /// </summary>
        public void UpdateBehaviorAttack()
        {
            int atkIndex = 0;
            int delayFrames = 5;
            //for each attackinfo, check the frame
            foreach (BehaviorAttack atkInfo in model.currentBehavior.attackInfos)
            {
                int scaledStartFrame = (int)((atkInfo.frameInfo.startFrame + 1) / model.objectTimeScale);
                int scaledEndFrame = scaledStartFrame + (int)((atkInfo.frameInfo.length+1) / model.objectTimeScale);
                // only when current behavior's frame is euqal to the attackinfo's startframe
                if (model.currentFrame == scaledStartFrame)
                {
                    atkInfo.ActiveHitBox(model, atkIndex);
                    //ActiveAttack(model, atkInfo, atkIndex);
                }

                // check if turn off the hitbox
                if (model.currentFrame == scaledEndFrame)
                {
                    atkInfo.DeactiveHitBox(model);
                }

                // if hit something then be able to cancel the attack immediately
                // if character didn't attack during the cancancel frames, then need extra delay frames to cancel the attackBehavior 
                model.CanCancel = CanCancelAttack(atkInfo, delayFrames);

                atkIndex++;
            }
        }

        /// <summary>
        /// If current attack can be canceled
        /// </summary>
        /// <param name="atkInfo"></param>
        /// <param name="delayFrames"></param>
        /// <returns></returns>
        bool CanCancelAttack(BehaviorAttack atkInfo, int delayFrames)
        {
            float currentFrame = model.currentFrame;

            int cancelFrames = (int)(atkInfo.frameInfo.startFrame + atkInfo.cancelDelay);

            return (currentFrame >= cancelFrames / model.objectTimeScale && model.HitConfirm) || currentFrame >= (cancelFrames + delayFrames) / model.objectTimeScale;
        }
    }
}