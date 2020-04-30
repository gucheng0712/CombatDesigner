
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
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        public ActorFSM(ActorModel model)
        {
            this.model = model;
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
            //Debug.LogError("Can't find the Behavior named " + name);
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

            ResetBehaviorActions();// Reset the behavior actions' params for prevent some behaior enter new behavior without exiting the behavior

            model.currentFrame = 0; // reset current frame
            model.currentBehavior = newBehavior; // update current behavior to new behavior

            //if (model.currentBehavior == GetBehavior("Neutral"))
            //{
            //    model.currentChainIndex = 0;
            //}
            model.CanCancel = false;

            // Reset hit info
            model.IsHitBoxActive = false;
            model.HitConfirm = false;

            model.EnterBehaviorEvent?.Invoke(model); // all the Enter Behavior Events
        }

        /// <summary>
        ///  Reset all behavior actions
        /// </summary>
        void ResetBehaviorActions()
        {
            if (model.currentBehavior != null)
            {
                foreach (IBehaviorAction e in model.currentBehavior.behaviorActions)
                {
                    if (e != null)
                    {
                        e.Exit(model);
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
        }

        /// <summary>
        /// End the Behavior, automaticly go back to the Neutral Behavior
        /// </summary>
        public void EndBehavior()
        {
            StartBehavior(GetBehavior("Neutral"));
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
            // increase the frame based on local time scale
            model.currentFrame += 1 * model.objectTimeScale;

            UpdateBehaviorActions();

            // Convert current frame to a integer (need to call after UpdateBehaviorActions)
            int currentFrame = (int)model.currentFrame;

            // if the current frame reach to the current behavior's end frame
            if (currentFrame >= (model.currentBehavior.frameLength))
            {
                // if this behavior is toggled loop
                if (model.currentBehavior.loopBehavior)
                    model.LoopBehavior();
                else
                    model.EndBehavior();
            }

            model.UpdateBehaviorEvent?.Invoke(model);// all the update behavior events

            // set the previouse frame to current frame at the end 
            model.previousFrame = currentFrame;
        }

        /// <summary>
        ///  Update all Behavior Actions in current behavior
        /// </summary>
        void UpdateBehaviorActions()
        {
            // Convert current frame to a integer
            int currentFrame = (int)model.currentFrame;

            // loop through all behaviors
            foreach (IBehaviorAction e in model.currentBehavior.behaviorActions)
            {
                if (e != null)
                {
                    int scaledStartFrame = (int)(e.startFrame);
                    int scaledEndFrame = (int)(e.endFrame);

                    // when current frame equal start frame of this action
                    if (currentFrame == scaledStartFrame)
                    {
                        e.Enter(model);
                    }
                    // when current frame in between start frame and end frame
                    if (currentFrame >= scaledStartFrame && currentFrame <= scaledEndFrame)
                    {
                        e.Execute(model);
                    }
                    // when current frame equal end frame of this action
                    if (currentFrame >= scaledEndFrame)
                    {
                        e.Exit(model);
                    }
                }
            }
        }
    }
}