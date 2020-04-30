
using UnityEngine;

namespace CombatDesigner
{
    public class SFXAction : IBehaviorAction
    {
        public AudioClip sfx;

        public override void Execute(ActorModel model)
        {
            if (sfx == null)
            {
                Debug.Log("Can't find sfx audio clip!");
                return;
            }
            if ((int)model.currentFrame == model.previousFrame)
            {
                Debug.Log("Is previous frame");
                return;
            }
            model.audioSource.pitch = model.objectTimeScale;
            model.audioSource.outputAudioMixerGroup.audioMixer.SetFloat("pitchBend", 1f / model.objectTimeScale);
            model.audioSource.PlayOneShot(sfx);
            CombatDebugger.Log("Play Audio", LogDomain.BehaviorAcrion);
        }
    }
}