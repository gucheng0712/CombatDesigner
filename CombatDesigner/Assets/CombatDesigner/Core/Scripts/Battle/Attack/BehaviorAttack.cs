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

    /// <summary>
    ///  Behavior Attack is a class that contains attack information of a behavior
    /// </summary>
    [System.Serializable]
    public class BehaviorAttack : AttackBase
    {
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
        /// the delay to execute next attack behavior
        /// </summary>
        public float cancelDelay;

        /// <summary>
        /// Disable the Attack
        /// </summary>
        /// <param name="model"></param>
        public void DeactiveHitBox(ActorModel model)
        {
            model.hitBox.SetLocalScale(Vector3.zero);
            model.hitBox.SetActive(false);
        }

        /// <summary>
        /// Enable the Attack
        /// </summary>
        /// <param name="model"></param>
        /// <param name="atkIndex">the current attackinfo index</param>
        public void ActiveHitBox(ActorModel model, int atkIndex)
        {
            model.hitBox.SetActive(false);
            model.hitBox.SetActive(true);

            model.hitBox.SetLocalScale(hitboxInfo.hitBoxScale);
            model.hitBox.SetLocalPosition(hitboxInfo.hitBoxPos);
            model.hitBox.SetLocalRotation(hitboxInfo.hitBoxRot);
            model.CurrentAtkIndex = atkIndex;
        }
    }
}
