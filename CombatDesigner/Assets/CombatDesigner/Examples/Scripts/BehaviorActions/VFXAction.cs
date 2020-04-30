#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Sirenix.OdinInspector;

namespace CombatDesigner
{
    public class VFXAction : IBehaviorAction
    {
        public GameObject vfxPrefab;
        public Vector3 posOffset;
        public float speedMultiplier = 1;

        public bool attachSelf;
        [EnableIf("@this.attachSelf")]
        public string selfParentName;

        public bool attachTarget;
        [EnableIf("@this.attachTarget")]
        public string targetParentName;

        public bool disableWhenExit;

        //float normalizedTime;
        ParticleSystem[] particles;

        public override void Enter(ActorModel model)
        {
            if (vfxPrefab == null) return;

            if ((int)model.currentFrame == model.previousFrame) return;

            Vector3 forward = model.character.transform.forward * posOffset.z;
            Vector3 right = model.character.transform.right * posOffset.x;
            Vector3 up = model.character.transform.up * posOffset.y;
            Vector3 spawnPos = model.character.transform.position + forward + right + up;

            Transform parent = CheckForAttachment(model);

            // todo Use FX Manager to control the particle pool, so that no need to get component all the time
            GameObject particleGo = GameObject.Instantiate(vfxPrefab, spawnPos, model.character.transform.rotation, parent);
            particles = particleGo.GetComponentsInChildren<ParticleSystem>();

            GameObject.Destroy(particleGo, 5);
            CombatDebugger.Log("Play vfx", LogDomain.BehaviorAcrion);
            foreach (var particle in particles)
            {
                var main = particle.main;
                main.simulationSpeed = model.objectTimeScale * speedMultiplier;
                particle.Play();
            }
        }

        public override void Execute(ActorModel model)
        {
            if (vfxPrefab == null) return;
            if ((int)model.currentFrame == model.previousFrame) return;
            //normalizedTime += 1 / (endFrame - startFrame);
            //if (particle != null)
            //{
            //    particle.Simulate(normalizedTime, true, true, false);
            //}

        }

        public override void Exit(ActorModel model)
        {
            // if ((int)model.currentFrame == model.previousFrame) return;
            if (disableWhenExit)
            {
                if (particles != null)
                {
                    foreach (var particle in particles)
                    {
                        if (particle != null)
                            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    }
                }
            }
        }

#if UNITY_EDITOR
        public override void ExecuteInEditor(ActorModel model, float normalizedTime)
        {
            //Debug.Log(normalizedTime);
            //if (vfxPrefab == null)
            //    return;

            //if (model.vfxGO == null)
            //    model.vfxGO = PrefabUtility.InstantiatePrefab(vfxPrefab) as GameObject;

            //Vector3 spawnPos = model.character.transform.position;
            //model.vfxGO.transform.position = spawnPos;
            //Quaternion spawnRot = Quaternion.LookRotation(model.character.transform.forward);
            //model.vfxGO.transform.rotation = spawnRot;
            //Transform parent = CheckForAttachment(model);
            //model.vfxGO.transform.SetParent(parent);

            //model.e_vfxInitialTime = model.e_currentStateTime;

            //float vfxNormalizedTime = normalizedTime - model.e_vfxInitialTime;
            //vfxNormalizedTime = Mathf.Max(0, vfxNormalizedTime);

            //ParticleSystem vfxParticle = model.vfxGO.GetComponentInChildren<ParticleSystem>();

            //vfxParticle.Simulate(vfxNormalizedTime, true);

            //if (model.e_currentStateTime >= endFrame)
            //{
            //    GameObject.DestroyImmediate(model.vfxGO);
            //}
        }
#endif

        Transform CheckForAttachment(ActorModel model)
        {
            Transform parent = null;
            if (attachSelf)
            {
                if (string.IsNullOrEmpty(selfParentName))
                {
                    parent = model.character.transform;
                }
                else
                {
                    parent = model.character.transform.Find(selfParentName);
                }
            }
            else if (attachTarget)
            {
                parent = model.target.character.transform;
            }
            return parent;
        }
    }



}