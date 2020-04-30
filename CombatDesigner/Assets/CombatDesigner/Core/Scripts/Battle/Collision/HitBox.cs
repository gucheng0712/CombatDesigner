using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{
    /// <summary>
    /// The base class of hitbox
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(ColliderVisualizer))]
    public class HitBox : IBox
    {
        private void Start()
        {
           // transform.localScale = Vector3.zero;
        }

        // Hit detection by Unity built-in function OnTriggerEnter
        void OnTriggerEnter(Collider col)
        {
            if (model == null)
                return;
            if (IsHitBoxActive() && !IsCollidingParent(col.gameObject))
            {
                switch (model.actorType)
                {
                    case ActorType.AI:
                        HitByTag(col, "Player");

                        break;
                    case ActorType.PLAYER:
                        HitByTag(col, "AI");
                        break;
                }
            }
        }

        /// <summary>
        /// Victim will Get Hit based on the tag
        /// </summary>
        /// <param name="col"></param>
        /// <param name="tag">The tag that give to the Collider GameObject </param>
        void HitByTag(Collider col, string tag)
        {
            if (col.transform.root.CompareTag(tag))
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("HurtBox"))
                {
                    ActorController victim = col.transform.root.GetComponent<ActorController>();
                    HurtBox hurtBox = victim.model.hurtBox;
                    BehaviorAttack currentAtk = model.GetCurrentAttack();
                    if (currentAtk != null)
                    {
                        hurtBox.hitPoint = new Vector3(victim.transform.position.x, transform.position.y, victim.model.hurtBox.transform.position.z);

                        hurtBox.GetHit(model, transform, currentAtk);
                        if (currentAtk.singleTarget)
                        {
                            currentAtk.DeactiveHitBox(model);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// A method to check if the colliding gameObject is the parent gameObject
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        bool IsCollidingParent(GameObject go)
        {
            return go == transform.root.gameObject;
        }

        /// <summary>
        /// A method to check if the hitbox should be active
        /// </summary>
        /// <returns></returns>
        bool IsHitBoxActive()
        {
            return true;// model.IsHitBoxActive;
        }
    }
}
