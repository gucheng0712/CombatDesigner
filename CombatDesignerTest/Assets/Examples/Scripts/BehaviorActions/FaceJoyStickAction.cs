using UnityEngine;

namespace CombatDesigner
{
    public class FaceJoyStickAction : IBehaviorAction
    {
        public float rotateTime = 0.5f;

        public override void Execute(ActorModel model)
        {
            model.moveInputDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (model.moveInputDir != Vector2.zero)
            {
                Vector3 lookDir = new Vector3(0, 0, 0);
                Vector3 camFwd = Camera.main.transform.forward;
                camFwd.y = 0;
                camFwd.Normalize();
                lookDir += camFwd * model.moveInputDir.y;

                lookDir += Camera.main.transform.right * model.moveInputDir.x;
                lookDir.y = 0;

                model.character.transform.rotation =  Quaternion.LookRotation(new Vector3(lookDir.x, 0, lookDir.z), Vector3.up);
            }
        }
    }
}