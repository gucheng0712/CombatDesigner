#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace CombatDesigner
{
    public class VFXAction : IBehaviorAction
    {
        public GameObject vfxPrefab;
        public bool attachSelf;
        public bool attachTarget;

        //float normalizedTime;
        ParticleSystem[] particles;

        public override void Enter(ActorModel model)
        {
            //Debug.Log("Play vfx");
            if (vfxPrefab == null) return;
            if ((int)model.currentFrame == model.previousFrame) return;
            Vector3 spawnPos = model.character.transform.position;
            Quaternion spawnRot = Quaternion.LookRotation(model.character.transform.forward);

            Transform parent = CheckForAttachment(model);

            // todo Use FX Manager to control the particle pool, so that no need to get component all the time
            GameObject particleGo = GameObject.Instantiate(vfxPrefab, spawnPos, spawnRot, parent);
            particles = particleGo.GetComponentsInChildren<ParticleSystem>();

            GameObject.Destroy(particleGo, 5);
            // normalizedTime = 0;
        }

        public override void Execute(ActorModel model)
        {
            if ((int)model.currentFrame == model.previousFrame) return;
            //normalizedTime += 1 / (endFrame - startFrame);
            //if (particle != null)
            //{
            //    particle.Simulate(normalizedTime, true, true, false);
            //}
            foreach (var particle in particles)
            {
                var main = particle.main;
                main.simulationSpeed = model.objectTimeScale;
                particle.Play();
            }
        }

        public override void Exit(ActorModel model)
        {
            if ((int)model.currentFrame == model.previousFrame) return;
            // Debug.Log("Exist particle action");
            //normalizedTime += 1 / (endFrame - startFrame);
            //if (particles != null)
            //{
            //    particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            //}
        }

#if UNITY_EDITOR
        public override void ExecuteInEditor(ActorModel model)
        {
            if (vfxPrefab == null) return;
            // todo change to enable and disable
            if (model.vfxGO == null)
                model.vfxGO = PrefabUtility.InstantiatePrefab(vfxPrefab) as GameObject;

            Vector3 spawnPos = model.character.transform.position;
            model.vfxGO.transform.position = spawnPos;
            Quaternion spawnRot = Quaternion.LookRotation(model.character.transform.forward);
            model.vfxGO.transform.rotation = spawnRot;
            Transform parent = CheckForAttachment(model);
            model.vfxGO.transform.SetParent(parent);

            model.e_playVFX = true;
            model.e_vfxInitialTime = model.e_currentStateTime;
        }
#endif

        Transform CheckForAttachment(ActorModel model)
        {
            Transform parent = null;
            if (attachSelf)
            {
                parent = model.character.transform;
            }
            else if (attachTarget)
            {
                parent = model.target.transform;
            }

            return parent;
        }
    }



}