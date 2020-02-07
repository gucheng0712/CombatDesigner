using UnityEngine;

namespace CombatDesigner
{
    public class VelocityYAction : IBehaviorAction
    {
        public float yDirForce;

        public override void Execute(ActorModel model)
        {
            model.velocity.y = yDirForce;
        }
    }
}
