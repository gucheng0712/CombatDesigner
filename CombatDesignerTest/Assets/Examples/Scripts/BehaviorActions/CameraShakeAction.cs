using UnityEngine;
using Cinemachine;

namespace CombatDesigner
{
    public class CameraShakeAction : IBehaviorAction
    {

        CinemachineFreeLook freelook;
        CinemachineBasicMultiChannelPerlin topRigNoise;
        CinemachineBasicMultiChannelPerlin midRigNoise;
        CinemachineBasicMultiChannelPerlin botRigNoise;

        public float shakeAmplitude = 1.2f;
        public float shakeFrequency = 2.0f;

        public override void Init(ActorModel model)
        {
            if (model.cmGO != null)
            {
                freelook = model.cmGO.GetComponent<CinemachineFreeLook>();

                topRigNoise = freelook.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                topRigNoise.m_AmplitudeGain = 0;
                topRigNoise.m_FrequencyGain = 0;
                midRigNoise = freelook.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                midRigNoise.m_AmplitudeGain = 0;
                midRigNoise.m_FrequencyGain = 0;
                botRigNoise = freelook.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                botRigNoise.m_AmplitudeGain = 0;
                botRigNoise.m_FrequencyGain = 0;
            }
        }


        public override void Enter(ActorModel model)
        {
            SetCameraShakeValue(shakeAmplitude, shakeFrequency);
            Debug.Log("Set camera shake");
        }

        public override void Exit(ActorModel model)
        {
            SetCameraShakeValue(0, 0);
            // Debug.Log("reset camera shake");
        }

        private void SetCameraShakeValue(float amplitude, float frequency)
        {
            if (topRigNoise != null)
            {
                topRigNoise.m_AmplitudeGain = amplitude;
                topRigNoise.m_FrequencyGain = frequency;
            }
            if (midRigNoise != null)
            {
                midRigNoise.m_AmplitudeGain = amplitude;
                midRigNoise.m_FrequencyGain = frequency;
            }
            if (botRigNoise != null)
            {
                botRigNoise.m_AmplitudeGain = amplitude;
                botRigNoise.m_FrequencyGain = frequency;
            }
        }
    }
}