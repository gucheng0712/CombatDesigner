using UnityEngine;

namespace CombatDesigner
{
    [CreateAssetMenu(fileName = "NewActorStats", menuName = "ACT-System/Stats")]
    public class ActorStats : ScriptableObject
    {
        [Range(1, 99)]
        public int level = 1;
        public int exp;
        public int maxExp;
        public int health;
        public int maxHealth;
        public int energy;
        public int maxEnergy;
        public int atkDmg;
        public float criticalChance;
        public float atkSpd;
        public float moveSpd;
        public float hardStraight;// Hit recovery Speed, the higher hard Straight the faster to recover from attack the impact of the attack can not move can not attack time, the higher the harder the shorter the time

    }
}
