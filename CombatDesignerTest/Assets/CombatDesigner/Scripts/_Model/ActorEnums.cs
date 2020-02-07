using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{
    /// <summary>
    /// The Control Type of the actor
    /// </summary>
    public enum ActorType
    {
        AI,
        PLAYER
    }

    /// <summary>
    /// The damage type of the behavior attack infomation
    /// </summary>
    public enum DamageType
    {
        Physical,
        Magical
    }

    /// <summary>
    /// The attack reaction type
    /// </summary>
    public enum AtkType
    {
        Normal,
        KnockDown,
        HitFly,
    }
}