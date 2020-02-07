using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{
    /// <summary>
    /// A class of Attack View
    /// </summary>
    public class BehaviorView_Attack
    {
        /// <summary>
        /// A method to update all Behavior AttackInfo in current hehavior
        /// </summary>
        /// <param name="model"></param>
        public void UpdateAttackInfos(ActorModel model)
        {
            int atkIndex = 0;
            int delayFrames = 10;
            int currentFrame = (int)model.currentFrame;
            //for each attackinfo, check the frame
            foreach (BehaviorAttack atkInfo in model.currentBehavior.attackInfos)
            {
                int scaledStartFrame = (int)(atkInfo.frameInfo.startFrame);
                int scaledEndFrame = (int)((atkInfo.frameInfo.startFrame + atkInfo.frameInfo.length));
                // only when current behavior's frame is euqal to the attackinfo's startframe
                if (currentFrame == scaledStartFrame)
                {
                    atkInfo.ActiveAttack(model, atkIndex);
                    //ActiveAttack(model, atkInfo, atkIndex);
                }

                // check if turn off the hitbox
                if (currentFrame == scaledEndFrame)
                {
                    atkInfo.DeactiveAttack(model);
                }

                // if hit something then be able to cancel the attack immediately
                // if character didn't attack during the cancancel frames, then need extra delay frames to cancel the attackBehavior 
                model.CanCancel = CanCancelAttack(model, atkInfo, delayFrames);

                atkIndex++;
            }
        }

        /// <summary>
        /// If current attack can be canceled
        /// </summary>
        /// <param name="model"></param>
        /// <param name="atkInfo"></param>
        /// <param name="delayFrames"></param>
        /// <returns></returns>
        bool CanCancelAttack(ActorModel model, BehaviorAttack atkInfo, int delayFrames)
        {
            float currentFrame = model.currentFrame;

            int cancelFrames = (int)(atkInfo.frameInfo.startFrame + atkInfo.cancelDelay);
            return (currentFrame >= cancelFrames / model.objectTimeScale && model.HitConfirm) || currentFrame >= (cancelFrames + delayFrames) / model.objectTimeScale;
        }

    }
}
