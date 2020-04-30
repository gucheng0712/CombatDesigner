using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{
    public interface IUnderAttack
    {
        void OnGetHit(ActorModel attacker, ActorModel victim, AttackBase atk, Transform dmgTransform);
    }
}












