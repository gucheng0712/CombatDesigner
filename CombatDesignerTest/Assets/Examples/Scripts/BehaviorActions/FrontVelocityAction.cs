using UnityEngine;

namespace CombatDesigner
{
    public class FrontVelocityAction : IBehaviorAction
    {
        public float velocityPower = 1f;
        public override void Execute(ActorModel model)
        {
            model.velocity.x = (model.character.transform.forward * velocityPower).x;
            model.velocity.z = (model.character.transform.forward * velocityPower).z;
            model.velocity.y = 0;
        }
    }
}
