using UnityEngine;

namespace CombatDesigner
{
    public class ChargedAttackAction : IBehaviorAction
    {

        public ActorBehavior chargeAttack;

        public override void Execute(ActorModel model)
        {

            if (!Input.GetKey(model.currentBehavior.inputKey))
            {
                //  Debug.Log(model.CurrentBehaviorFrame);
                model.StartBehavior(chargeAttack);
                //Debug.Log(model.CurrentBehaviorFrame);
            }
        }
    }
}