using UnityEngine;

namespace CombatDesigner
{
    public class FaceTargetAction : IBehaviorAction
    {
        public override void Execute(ActorModel model)
        {
            if (model.hasLockdeTarget == false)
            {
                model.target = GameManager_Input.Instance.nearestTarget;
                model.hasLockdeTarget = true;
            }
            if (model.target != null)
            {
                Vector3 lookDir = model.target.transform.position - model.character.transform.position;
                lookDir.y = 0;
                if (lookDir.sqrMagnitude > 0.25f)
                {
                    model.character.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
                }
            }
        }
    }
}