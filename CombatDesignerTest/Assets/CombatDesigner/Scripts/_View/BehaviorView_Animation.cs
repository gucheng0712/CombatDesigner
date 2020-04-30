using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatDesigner;


/// <summary>
/// A class to the animation view
/// </summary>
public class BehaviorView_Animation
{
    /// <summary>
    /// A method to update character Animator. 
    /// Called in <b>Update()</b>
    /// </summary>
    /// <param name="model"></param>
    public void UpdateAnimation(ActorModel model)
    {
        model.animSpeed = 1; // Set overall animation speed to 1
        model.anim.speed = model.objectTimeScale;
        // Check if is in attack Pausing Frame
        if (model.CurrentHitPauseFrames > 0)
        {
            model.anim.speed = 0;
        }

        // todo falling animation based on the velocity
        model.animFallSpeed = model.velocity.y * 30f;

        model.anim.SetFloat("MoveSpeed", model.animMoveSpeed);
    }

    /// <summary>
    /// A callback method to play animation
    /// </summary>
    /// <param name="model"></param>
    public void CrossFadeAnimationInFixedTime(ActorModel model)
    {
        ActorBehavior currentBehavior = model.currentBehavior;
        model.anim.CrossFadeInFixedTime(currentBehavior.name, currentBehavior.animBlend);
    }
}

