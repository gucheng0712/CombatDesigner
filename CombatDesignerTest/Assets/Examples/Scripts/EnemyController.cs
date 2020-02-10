using System;
using UnityEngine;
using CombatDesigner;

namespace ShowCaseDemo
{
    public class EnemyController : ActorController
    {
        #region Fields
        /// <summary>
        /// A spawn point of hit particle
        /// </summary>
        public Transform hitParticlePoint;

        /// <summary>
        /// Collision normal on other object
        /// </summary>
        Vector3 hitNormal;

        /// <summary>
        /// The platform current stands on
        /// </summary>
        Transform activePlatform;

        /// <summary>
        /// The platform's move direction
        /// </summary>
        Vector3 moveDirection;
        /// <summary>
        /// The active platform positon in world space
        /// </summary>
        Vector3 activeGlobalPlatformPos;
        /// <summary>
        /// The active platform positon in local space
        /// </summary>
        Vector3 activeLocalPlatformPos;
        /// <summary>
        ///  The active platform rotation in world space
        /// </summary>
        Quaternion activeGlobalPlatformRot;
        /// <summary>
        /// The active platform rotation in local space
        /// </summary>
        Quaternion activeLocalPlatformRotation;

        #endregion


        /// <summary>
        /// Initialization Function. 
        /// An override method from <b>ActorController</b>. Called in <b>Awake()</b>
        /// </summary>
        public override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// An override method from <b>ActorController</b> 
        /// </summary>
        public override void Update()
        {
            // Check for Death
            if (model.isDead)
                return;

            // Check for Hit Pause 
            if (IsInHitPauseFrames())
                return;

            base.Update();

            UpdateMovingPlatform();
        }
        /// <summary>
        /// Updating the AI control logics. 
        /// An override method from ActorController. Called in <b>Update()</b>
        /// </summary>
        protected override void UpdateAILogic()
        {
            base.UpdateAILogic(); // todo add enemy behavior
        }

        /// <summary>
        ///  Return a boolean to determine if current character is in hit pause frames.
        /// </summary>
        /// <returns></returns>
        bool IsInHitPauseFrames()
        {
            if (model.CurrentHitPauseFrames > 0)
            {
                model.CurrentHitPauseFrames -= Time.fixedDeltaTime * 50;
            }

            return model.CurrentHitPauseFrames > 0;
        }

        /// <summary>
        /// Update the character's movements. 
        /// An override method from ActorController. Called in <b>Update()</b>
        /// </summary>
        public override void UpdatePhysics()
        {
            base.UpdatePhysics();
        }

        /// <summary>
        /// Update the velocity information of character.
        /// An override method from ActorController. Called in <b>UpdatePhysics().</b>
        /// </summary>
        protected override void UpdateVelocity()
        {
            base.UpdateVelocity();
        }

        /// <summary>
        /// Update the Infomation based on grounded or not. 
        /// An override method from ActorController. Called in <b>UpdatePhysics()</b>
        /// </summary>
        protected override void UpdateGrounndCheckInfo()
        {
            base.UpdateGrounndCheckInfo();
        }

        /// <summary>
        /// Update the character's grounded information.
        /// An override method from ActorController. Called in <b>UpdateGrounndCheckInfo().</b>
        /// </summary>
        protected override void UpdateGrounededInfo()
        {
            RaycastHit hit;

            // Check if is in Grounded layer, prevent stand on character's head
            if (Physics.SphereCast(this.transform.position + Vector3.up, model.cc.radius * 2, -Vector3.up, out hit, 1.5f, model.groundLayer))
            {
                // todo remove later, for debuging purpose
                Debug.DrawRay(this.transform.position + Vector3.up, -Vector3.up, Color.red, 1);
                // move actor based on the hitNormal
                model.cc.SimpleMove(new Vector3(hitNormal.x, -100, hitNormal.z) * 10f);
            }
            else// Reset Ground Property
            {
                model.velocity.y = 0;
            }
            model.isAerial = false;
            model.aerialTimer = 0;
            model.animAir *= 0.75f;

            // if is HitFly State perform a stand up behavior after fall down
            if (model.currentBehavior.name == "HitFly")
            {
                model.StartBehavior(model.GetBehavior("StandUp"));
            }
        }

        /// <summary>
        /// Update the character's ungrounded information.
        /// An override method from ActorController. Called in <b>UpdateGrounndCheckInfo()</b>
        /// </summary>
        protected override void UpdateUngrounededInfo()
        {
            // if is in falling threhold
            if (!model.isAerial)
            {
                model.RefillAirJumpPoint();
                model.aerialTimer++;// start counting time to be in Aerial state
            }

            if (model.aerialTimer >= aerialThreahold)
            {
                model.isAerial = true;

                // update the falling animation
                if (model.animAir <= 1f)
                {
                    model.animAir += 0.1f;
                }
            }
        }

        /// <summary>
        /// Update the character's friction.
        /// An override method from ActorController. Called in <b>UpdatePhysics()</b>
        /// </summary>
        protected override void UpdateFriction()
        {
            base.UpdateFriction();
        }

        /// <summary>
        /// Calculate the movement when moving on the platform
        /// </summary>
        void UpdateMovingPlatform()
        {
            if (activePlatform != null)
            {
                Vector3 newGlobalPlatformPoint = activePlatform.TransformPoint(activeLocalPlatformPos);
                moveDirection = newGlobalPlatformPoint - activeGlobalPlatformPos;
                if (moveDirection.magnitude > 0.01f)
                {
                    model.cc.Move(moveDirection);
                }
                if (activePlatform)
                {
                    // Support moving platform rotation
                    Quaternion newGlobalPlatformRotation = activePlatform.rotation * activeLocalPlatformRotation;
                    Quaternion rotationDiff = newGlobalPlatformRotation * Quaternion.Inverse(activeGlobalPlatformRot);
                    // Prevent rotation of the local up vector
                    rotationDiff = Quaternion.FromToRotation(rotationDiff * Vector3.up, Vector3.up) * rotationDiff;
                    transform.rotation = rotationDiff * transform.rotation;
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                    UpdatePlatformTransformation();
                }
            }
        }

        /// <summary>
        /// When CharacterController hit sth, this method will be called
        /// </summary>
        /// <param name="hit"></param>
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            hitNormal = hit.normal;

            // Make sure we are really standing on a straight platform *NEW*
            // Not on the underside of one and not falling down from it either!
            if (hit.gameObject.tag == "MovingPlatform")
            {
                if (hit.moveDirection.y < -0.9 && hit.normal.y > 0.41)
                {
                    if (activePlatform != hit.collider.transform)
                    {
                        activePlatform = hit.collider.transform;
                        UpdatePlatformTransformation();
                    }
                }
                else
                {
                    activePlatform = null;
                }
            }
            else
            {
                activePlatform = null;
            }
        }

        /// <summary>
        /// Update the local and global platform point
        /// </summary>
        void UpdatePlatformTransformation()
        {
            activeGlobalPlatformPos = transform.position;
            activeLocalPlatformPos = activePlatform.InverseTransformPoint(transform.position);
            // Support moving platform rotation
            activeGlobalPlatformRot = transform.rotation;
            activeLocalPlatformRotation = Quaternion.Inverse(activePlatform.rotation) * transform.rotation;
        }

        /// <summary>
        /// When receive a attack, this method will be called
        /// An Override method from ActorController.
        /// </summary>
        /// <param name="attacker"></param>
        public override void GetHit(ActorController attacker)
        {
            ApplyAttackInfo(attacker);

            attacker.model.HitConfirm = true;
            attacker.model.IncreaseEnergy(10); // attack increase 10 energy;

            // if this actor is dead, then perform a death behavior    todo implement receive dmg 
            if (model.isDead)
            {
                model.StartBehavior(model.GetBehavior("Die"));
                // model.OnDeathEvent?.Invoke(model); // disable all the colliders and scripts
            }

            //model.GetHitEvent?.Invoke(model); // todo get hit events such as vfx sfx,energy,mp,hp
        }

        /// <summary>
        /// A Method to apply Behavior Attack infomation
        /// </summary>
        /// <param name="attacker"></param>
        void ApplyAttackInfo(ActorController attacker)
        {
            // get the attacker's attackinfo
            BehaviorAttack currentAtk = attacker.model.currentBehavior.attackInfos[attacker.model.CurrentAtkIndex];
            ApplyHitReaction(attacker, currentAtk);

            // apply the get hit vfx such as blood, impact...
            ApplyHitFx(attacker, currentAtk);
            // Apply the hit force that character received from attacker
            ApplyHitForce(attacker, currentAtk);

            // reset hit pause Time
            ResetHitPauseTime(attacker, currentAtk);

            // stun
            model.HitRecoverFrames = currentAtk.hitStun;

            // deactive the hitbox when the attackinfo require single target
            if (currentAtk.singleTarget)
            {
                currentAtk.DeactiveAttack(model);
            }
        }

        /// <summary>
        /// The Reaction of Getting hit based on the attack type
        /// </summary>
        /// <param name="attacker"> the character that attacking</param>
        /// <param name="currentAtk"></param>
        void ApplyHitReaction(ActorController attacker, BehaviorAttack currentAtk)
        {
            if (!model.isAerial)
            {
                switch (currentAtk.atkType)
                {
                    case AtkType.Normal:
                        model.StartBehavior(model.GetBehavior("HitStun")); // todo change get hit to a standalone behavior
                        break;
                    case AtkType.KnockDown:
                        model.StartBehavior(model.GetBehavior("HitDown")); // todo change get hit to a standalone behavior
                        break;
                    case AtkType.HitFly:
                        model.StartBehavior(model.GetBehavior("HitFly")); // todo change get hit to a standalone behavior
                        break;
                }
            }
            else
            {
                model.StartBehavior(model.GetBehavior("HitFly")); // todo change get hit to a standalone behavior
            }

            // update the get hit behavior frame length based on the attackinfo's hitstun
            model.currentBehavior.frameLength = currentAtk.hitStun;
            // The hit animation conditions
            model.animHit = GetEnemyRelativePosition(model.character.transform, attacker.model.character.transform);
        }

        /// <summary>
        ///  Play the vfx based on the attacker
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="currentAtk"></param>
        void ApplyHitFx(ActorController attacker, BehaviorAttack currentAtk)
        {
            if (currentAtk.hitVFX != null)
            {
                if (hitParticlePoint.position != null)
                {
                    Vector3 hitFxSpawnPos = hitParticlePoint.position;
                    GameObject go = Instantiate(currentAtk.hitVFX, hitFxSpawnPos, transform.rotation, model.character.transform);
                    Destroy(go, 3);
                }
            }

        }

        /// <summary>
        ///  Reset the hitpause time to self or target independently
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="currentAtk"></param>
        void ResetHitPauseTime(ActorController attacker, BehaviorAttack currentAtk)
        {
            if (currentAtk.hitPauseSelf > attacker.model.CurrentHitPauseFrames)
            {
                attacker.model.CurrentHitPauseFrames = currentAtk.hitPauseSelf;
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
        void ApplyHitForce(ActorController attacker, BehaviorAttack currentAtk)
        {
            Vector3 nextKnockback = (model.isAerial) ? currentAtk.hitForceToAerialTarget : currentAtk.hitForceToGroundedTarget;
            //Debug.Log("hitinfo");
            Vector3 knockOrientation = attacker.model.character.transform.forward;
            nextKnockback = Quaternion.LookRotation(knockOrientation) * nextKnockback;
            model.velocity = nextKnockback;
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
