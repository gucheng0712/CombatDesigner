
using System.Collections;
using UnityEngine;

namespace CombatDesigner
{
    public class FlashMovementAction : IBehaviorAction
    {
        public float moveSpeed = 2.5f;
        float moveSmoothVelocity;
        float turnSmoothVelocity;

        public int disappearFrame = 3;
        public int appearFrame = 25;

        SkinnedMeshRenderer[] renderers;

        public override void Execute(ActorModel model)
        {
            GroundMovementAction(moveSpeed, model);

            double currentFrame = model.currentFrame;

            if (disappearFrame == currentFrame)
            {
                renderers = model.character.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var r in renderers)
                {
                    r.enabled = false;
                }
            }
            if (appearFrame == currentFrame)
            {
                foreach (var r in renderers)
                {
                    r.enabled = true;
                }
            }
        }

        void GroundMovementAction(float power, ActorModel model)
        {
            if (model.HitRecoverFrames <= 0)
            {
                Vector3 moveDir = -model.character.transform.forward;
                model.animMoveSpeed = model.moveInputDir.magnitude;

                if (model.moveInputDir.magnitude > 0.1f)
                {
                    moveDir = model.character.transform.forward;
                }

                model.CurrentSpeed = Mathf.SmoothDamp(model.CurrentSpeed, model.animMoveSpeed, ref moveSmoothVelocity, 0.1f);

                model.velocity += moveDir * power * Time.fixedDeltaTime;
                HandleRotation(model.moveInputDir, model);
            }
        }
        void HandleRotation(Vector2 _inputDir, ActorModel model)
        {
            if (_inputDir != Vector2.zero)
            {
                float targetRotation = Mathf.Atan2(_inputDir.x, _inputDir.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                model.character.transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(model.character.transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, 0.1f, 2000f, Time.deltaTime);
            }
        }
    }
}