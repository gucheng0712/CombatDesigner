using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{
    //[System.Serializable]
    //public class Stat
    //{
    //    [SerializeField] float baseValue;

    //    List<float> modifiers = new List<float>();

    //    public float GetValue()
    //    {
    //        float finalValue = baseValue;
    //        modifiers.ForEach(x => finalValue += x);
    //        return finalValue;
    //    }

    //    public void AddModifier(float modifier)
    //    {
    //        if (modifier != 0)
    //            modifiers.Add(modifier);
    //    }

    //    public void RemoveModifier(float modifier)
    //    {
    //        if (modifier != 0)
    //            modifiers.Remove(modifier);
    //    }
    //}

    [System.Serializable]
    public class ActorStats
    {
        [Range(1, 99)]
        public int level = 1;
        public int exp;

        [HideInInspector] public int currentHealth { get; private set; }
        public int maxHealth = 100;

        [HideInInspector] public int currentEnergy { get; private set; }
        public int maxEnergy = 100;
        [HideInInspector] public int currentAirJumpPoint { get; private set; }
        public int maxAirJumpPoint = 2;
        public int atkDmg;
        public float criticalChance;
        public float atkSpd;
        public float moveSpd;


        [Range(0, 100)] public float hardStraight;// Hit recovery Speed, the higher hard Straight the faster to recover from attack the impact of the attack can not move can not attack time, the higher the harder the shorter the time
        [Range(0, 100)] public float weight;

        public void Init()
        {
            currentHealth = maxHealth;
            currentEnergy = 0;
            RefillAirJumpPoint();
        }


        /// <summary>
        /// Increase the energy at power meter of the character
        /// </summary>
        /// <param name="healthPoint"></param>
        public void IncreaseHealth(int healthPoint)
        {
            currentHealth += healthPoint;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }
        /// <summary>
        /// Increase the energy at power meter of the character
        /// </summary>
        /// <param name="healthPoint"></param>
        public void ReduceHealth(int healthPoint)
        {
            currentHealth -= healthPoint;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }

        /// <summary>
        /// Increase the energy at power meter of the character
        /// </summary>
        /// <param name="power"></param>
        public void IncreaseEnergy(int power)
        {
            currentEnergy += power;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        }

        /// <summary>
        /// Decrease the energy at power meter of the character
        /// </summary>
        /// <param name="power"></param>
        public void ReduceEnergy(int power)
        {
            currentEnergy -= power;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        }

        /// <summary>
        ///  Reset the Initial Air Jump Point
        /// </summary>
        public void RefillAirJumpPoint()
        {
            currentAirJumpPoint = maxAirJumpPoint;
        }
        public void ReduceJumpPoint(int reducedPoints)
        {
            currentAirJumpPoint -= reducedPoints;
            currentAirJumpPoint = Mathf.Max(0, currentAirJumpPoint);
        }
    }
}
