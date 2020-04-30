using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CombatDesigner
{
    [System.Serializable]
    public class ActorBuffSystem
    {
        public Dictionary<string, Buff> buffs = new Dictionary<string, Buff>();

        public void UpdateBuff()
        {
            if (buffs.Values.Count < 1)
                return;
            foreach (var b in buffs.Values.ToList())
            {
                b.Tick(Time.deltaTime);

                if (b.IsFinished)
                {
                    buffs.Remove(b.buffName);
                    Debug.Log("Buff Finished "+b.buffName);
                }
            }
        }

        public void AddBuff(Buff buffToAdd)
        {
            if (buffs.ContainsKey(buffToAdd.buffName))
            {
                buffs[buffToAdd.buffName].Activate();
            }
            else
            {
                buffs.Add(buffToAdd.buffName, buffToAdd);
                buffToAdd.Activate();
            }
            Debug.Log("Add Buff " + buffToAdd.buffName+" with Duration "+buffToAdd.duration+" Seconds");
        }

        public void RemoveBuff(Buff buffToRemove)
        {
            if (buffs.ContainsKey(buffToRemove.buffName))
            {
                buffs.Remove(buffToRemove.buffName);
            }
        }

        public void ClearBuff()
        {

        }
    }
}













