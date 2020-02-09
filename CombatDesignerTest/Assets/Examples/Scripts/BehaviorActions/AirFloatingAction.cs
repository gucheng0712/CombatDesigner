using UnityEngine;

namespace CombatDesigner
{
    public class AirFloatingAction : IBehaviorAction
    {


        public override void Execute(ActorModel model)
        {                       model.velocity = new Vector3(model.velocity.x, 0, model.velocity.z);
        }
    }
}