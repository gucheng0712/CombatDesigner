using System;
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
    /// The attack reaction type
    /// </summary>
    public enum AttackImpactType
    {
        Normal,
        KnockDown,
        HitFly,
        Stun,
    }

    [Flags]
    public enum TargetRelativeLocation
    {
        None = 0,
        Front = 1 << 0,
        Back = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
    }
}