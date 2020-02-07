using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{
    /// <summary>
    /// The base class of hitbox
    /// </summary>
    public class HitBox : MonoBehaviour
    {
        // The ActorController Object from the attacker
        ActorController attacker;

        // The collider of current hitbox
        BoxCollider hitBoxCollider;

        void Start()
        {
            attacker = transform.root.GetComponent<ActorController>();
            hitBoxCollider = GetComponent<BoxCollider>();
        }

        // Hit detection by Unity built-in function OnTriggerEnter
        void OnTriggerEnter(Collider col)
        {
            if (IsHitBoxActive() && !IsCollidingParent(col.gameObject))
            {
                switch (attacker.model.actorType)
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
            if (col.gameObject.tag == tag)
            {
                // Debug.Log("Hit");
                ActorController victim = col.transform.root.GetComponent<ActorController>();
                victim.GetHit(attacker);
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
            return attacker.model.IsHitBoxActive;
        }

        /// <summary>
        ///  Enable/Disable the hitbox collider
        /// </summary>
        /// <param name="enable"></param>
        public void SetActive(bool enable)
        {
            if (hitBoxCollider != null)
                hitBoxCollider.enabled = enable;
        }

        /// <summary>
        /// A method to set hitbox scale
        /// </summary>
        /// <param name="scale"></param>
        public void SetLocalScale(Vector3 scale)
        {
            transform.localScale = scale;
        }
        /// <summary>
        /// A method to set hitbox position
        /// </summary>
        /// <param name="pos"></param>
        public void SetLocalPosition(Vector3 pos)
        {
            transform.localPosition = pos;
        }
    }
}
