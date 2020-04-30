using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CombatDesigner
{
    public class AttackBase
    {
        /// <summary>
        /// Is this attack only hit one target
        /// </summary>
        [Tooltip("Is this attack only hit one target")]
        public bool singleTarget;

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
        public float hitImpact;

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
        public AttackImpactType atkImpactType;

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
    }
}










