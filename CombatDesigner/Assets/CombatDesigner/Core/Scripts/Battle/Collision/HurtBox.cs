
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace CombatDesigner
{
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(ColliderVisualizer))]
    public class HurtBox : IBox
    {
        [HideInInspector] public Vector3 hitPoint;


        public override void Init()
        {
            base.Init();

        }


        /// <summary>
        /// When receive a attack, this method will be called
        /// An Override method from ActorController.
        /// </summary>
        /// <param name="attacker"></param>
        public void GetHit(ActorModel attacker, Transform atkTransform, AttackBase atk)
        {
            if (model.isDead||model.IsInvincible)
                return;

            Vector3 dirToAttacker = (attacker.character.transform.position - model.character.transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToAttacker) * 2;

            if (model.currentBehavior.atkBlockingAngle <= angle)
            {
                foreach (var e in model.currentBehavior.getHurtEvents)
                {
                    e.OnGetHit(attacker, model, atk, atkTransform);
                }
            }
            else
            {              
                foreach (var e in model.currentBehavior.blockHitEvents)
                {
                    e.OnGetHit(attacker, model, atk, atkTransform);
                }
            }
            attacker.IncreaseEnergy(atk.powerMeterGrowthRate); // attack increase 10 energy;
            attacker.HitConfirm = true;
            DeathCheck();
        }

        private void DeathCheck()
        {
            if (model.actorStats.currentHealth <= 0)
            {
                model.StartBehavior(model.GetBehavior("Die"));
                model.isDead = true;
            }
        }


        /// <summary>
        /// A Method to apply Behavior Attack infomation
        /// </summary>
        /// <param name="attacker"></param>
        void ApplyHitInfo(ActorModel attacker, Transform atkTransform, AttackBase currentAtk)
        {
            // get the attacker's attackinfo
            ExecuteHurtBehavior(currentAtk, atkTransform);
            //ApplyHurtFX(hitPoint);

            // reset hit pause Time
            ApplyHitPauseTime(attacker, currentAtk);

            // Apply the hit force that character received from attacker
            ApplyHitForce(currentAtk, atkTransform.forward);

            // apply the get hit vfx such as blood, impact...
            ApplyHitFx(currentAtk, hitPoint);

            model.DecreaseHealth(currentAtk.hitDmg);
        }

        /// <summary>
        /// The Reaction of Getting hit based on the attack type
        /// </summary>
        /// <param name="attacker"> the character that attacking</param>
        /// <param name="currentAtk"></param>
        void ExecuteHurtBehavior(AttackBase currentAtk, Transform atkTransform)
        {
            ActorBehavior getHitBehavior = null;
            if (!model.cc.isGrounded)
            {
                switch (currentAtk.atkImpactType)
                {
                    case AttackImpactType.Normal:
                        getHitBehavior = model.GetBehavior("Hurt_Grounded"); // todo change get hit to a standalone behavior
                        break;
                    case AttackImpactType.KnockDown:
                        getHitBehavior = model.GetBehavior("Hurt_Falldown"); // todo change get hit to a standalone behavior
                        break;
                    case AttackImpactType.HitFly:
                        getHitBehavior = model.GetBehavior("Hurt_Aerial"); // todo change get hit to a standalone behavior
                        break;
                }
            }
            else
            {
                getHitBehavior = model.GetBehavior("HitFly"); // todo change get hit to a standalone behavior
            }

            model.StartBehavior(getHitBehavior);

            // update the get hit behavior frame length based on the attackinfo's hitstun
            ApplyHitImpactFrames(currentAtk.hitImpact);
            // The hit animation conditions
            model.animHit = GetEnemyRelativePosition(model.character.transform, atkTransform);
        }

        void ApplyHitImpactFrames(float frames)
        {
            model.currentBehavior.frameLength = frames * (1 - model.actorStats.hardStraight * 0.01f);
        }

        /// <summary>
        ///  Play the vfx based on the attacker
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="currentAtk"></param>
        void ApplyHitFx(AttackBase currentAtk, Vector3 spawnPoint)
        {
            if (currentAtk.hitVFX != null)
            {
                GameObject hitVFX = Instantiate(currentAtk.hitVFX, spawnPoint, transform.rotation, model.character.transform);
                Destroy(hitVFX, 3);
            }
        }

        // void ApplyHurtFX(Vector3 spawnPoint)
        // {
        //     // todo put into pool manager
        //     StartCoroutine(HitShaderEffect());

        //     if (model.hurtVFX != null)
        //     {
        //         GameObject hurtVFX = Instantiate(model.hurtVFX, spawnPoint, transform.rotation, model.character.transform);
        //         Destroy(hurtVFX, 3);
        //     }
        // }

        /// <summary>
        ///  Reset the hitpause time to self or target independently
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="currentAtk"></param>
        void ApplyHitPauseTime(ActorModel attacker, AttackBase currentAtk)
        {
            if (currentAtk.hitPauseSelf > attacker.CurrentHitPauseFrames)
            {
                attacker.CurrentHitPauseFrames = currentAtk.hitPauseSelf;
            }

            if (currentAtk.hitPauseTarget > model.CurrentHitPauseFrames)
            {
                model.CurrentHitPauseFrames = currentAtk.hitPauseTarget;
            }
        }

        /// <summary>
        ///  Apply the hit force that character received from attacker
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="currentAtk"></param>
        void ApplyHitForce(AttackBase currentAtk, Vector3 forceOrigin)
        {
            Vector3 nextKnockback = (model.isAerial) ? currentAtk.hitForceToAerialTarget : currentAtk.hitForceToGroundedTarget;
            //Debug.Log("hitinfo");
            Vector3 knockOrientation = forceOrigin;
            nextKnockback = Quaternion.LookRotation(knockOrientation) * nextKnockback;
            model.velocity = nextKnockback * (1 - model.actorStats.weight * 0.01f);
        }

        /// <summary>
        ///  Get the relative position from target to perform different get hit animation
        /// </summary>
        /// <param name="character"></param>
        /// <param name="attacker"></param>
        /// <returns></returns>
        Vector2 GetEnemyRelativePosition(Transform character, Transform attacker)
        {
            Vector2 hitAnimPos = new Vector2();

            float dot = Vector3.Dot(character.transform.position, attacker.transform.forward);
            float cross = Vector3.Cross(character.transform.forward, attacker.transform.position).y;
            if (dot < 0) // at back side,set animation to gethit_back
            {
                hitAnimPos.y = 1;
                // Debug.Log("back");
            }
            else if (dot > 0)// at front side,set animation to gethit_front
            {
                hitAnimPos.y = -1;
                //Debug.Log("front");
            }
            if (cross < 0)// at left side,set animation to gethit_left
            {
                hitAnimPos.x = -1;
                // Debug.Log("left");
            }
            else if (cross > 0)// at right side,set animation to gethit_right
            {
                hitAnimPos.x = 1;
                // Debug.Log("right");
            }
            return hitAnimPos;
        }

    }
}