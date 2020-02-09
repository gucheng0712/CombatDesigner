using UnityEngine;

namespace CombatDesigner
{
    public class SFXAction : IBehaviorAction
    {
        public AudioClip sfx;


        public override void Execute(ActorModel model)
        {
            if (sfx == null) return;
            if ((int)model.currentFrame == model.previousFrame)return;

            model.audioSource.pitch = model.objectTimeScale;
            model.audioSource.outputAudioMixerGroup.audioMixer.SetFloat("pitchBend",1f/model.objectTimeScale);
            model.audioSource.PlayOneShot(sfx);
            //Debug.Log("Play Audio");
        }
    }
}