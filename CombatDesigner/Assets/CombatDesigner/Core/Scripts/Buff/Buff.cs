using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{
    public abstract class Buff
    {
        public string buffName;
        [HideInInspector] public float stackedDuration;
        protected int effectStacks;

        public float duration;
        public bool isEffectStacked;
        public bool isDurationStacked;
        public bool IsFinished;
        [HideInInspector] public ActorModel affectedModel;

        public Buff() { }
        public Buff(string buffName, ActorModel model)
        {
            this.affectedModel = model;
            this.buffName = buffName;
        }

        public void Tick(float delta)
        {
            stackedDuration -= delta;
            if (stackedDuration <= 0)
            {
                End();
                IsFinished = true;
            }
            Debug.Log("Ticking Buff ");
        }

        public void Activate()
        {
            if (isEffectStacked || stackedDuration <= 0)
            {
                ApplyEffect(effectStacks);
                effectStacks++;
            }

            if (isDurationStacked || stackedDuration <= 0)
            {
                stackedDuration += duration;
            }
        }

        protected abstract void ApplyEffect(int effectStacks);

        public abstract void End();
    }
}












