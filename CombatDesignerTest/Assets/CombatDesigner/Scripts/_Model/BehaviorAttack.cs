using UnityEngine;

namespace CombatDesigner
{
    /// <summary>
    /// Hitbox position, rotation, and scale infomation
    /// </summary>
    [System.Serializable]
    public struct HitboxInfo
    {
        public Vector3 hitBoxPos;
        public Quaternion hitBoxRot;
        public Vector3 hitBoxScale;
    }

    /// <summary>
    /// The start frame and the length of this attackInfo
    /// </summary>
    [System.Serializable]
    public struct FrameInfo
    {
        public float startFrame;
        public float length;
    }

    //[System.Serializable]
    //public struct DamageInfo
    //{

    //}


    /// <summary>
    ///  Behavior Attack is a class that contains attack information of a behavior
    /// </summary>
    [System.Serializable]
    public class BehaviorAttack
    {
        /// <summary>
        /// Is this attack only hit one target
        /// </summary>
        [Tooltip("Is this attack only hit one target")]
        public bool singleTarget;

        /// <summary>
        /// The start frame and the length of this attackInfo
        /// </summary>
        public FrameInfo frameInfo;

        /// <summary>
        /// Hitbox position, rotation, and scale infomation
        /// </summary>
        [Space(2)]
        public HitboxInfo hitboxInfo;

        /// <summary>
        /// how many frame to pause self. Improve combat feeling.
        /// </summary>
        [Space(2)]
        public float hitPauseSelf;
        /// <summary>
        /// how many frame to pause target. Improve combat feeling.
        /// </summary>
        public float hitPauseTarget;

        /// <summary>
        /// how many frame to stun target. 
        /// </summary>
        public float hitStun;

        /// <summary>
        /// The force that the attack will give to grouneded targets
        /// </summary>
        public Vector3 hitForceToGroundedTarget;

        /// <summary>
        /// The force that the attack will give to aerial targets
        /// </summary>
        public Vector3 hitForceToAerialTarget;

        /// <summary>
        /// The attack type of this attack
        /// </summary>
        public AtkType atkType;

        /// <summary>
        /// The damage that this attack give to the target
        /// </summary>
        public int hitDmg;

        /// <summary>
        /// A multiplier of power meter increasement
        /// </summary>
        public int powerMeterGrowthRate;

        /// <summary>
        /// The hit effect prefab
        /// </summary>
        public GameObject hitVFX;

        /// <summary>
        /// the delay to execute next attack behavior
        /// </summary>
        public float cancelDelay;

        /// <summary>
        /// Disable the Attack
        /// </summary>
        /// <param name="model"></param>
        public void DeactiveAttack(ActorModel model)
        {

            model.IsHitBoxActive = false;
            model.hitBox.SetLocalScale(Vector3.zero);
            model.hitBox.SetActive(false);
        }

        /// <summary>
        /// Enable the Attack
        /// </summary>
        /// <param name="model"></param>
        /// <param name="atkIndex">the current attackinfo index</param>
        public void ActiveAttack(ActorModel model, int atkIndex)
        {
            model.hitBox.SetActive(false);
            model.IsHitBoxActive = false;
            model.hitBox.SetActive(true);
            model.IsHitBoxActive = true;

            model.hitBox.SetLocalScale(hitboxInfo.hitBoxScale);
            model.hitBox.SetLocalPosition(hitboxInfo.hitBoxPos);
            model.CurrentAtkIndex = atkIndex;
        }
    }
}
