
using UnityEngine;

namespace CombatDesigner
{
    public class CastPrefabAction : IBehaviorAction
    {
        public GameObject prefab;
        public Vector3 firePosOffset;
        public bool attachParent;

        public override void Enter(ActorModel model)
        {
            if (prefab == null) return;

            Vector3 forward = model.character.transform.forward * firePosOffset.z;
            Vector3 right = model.character.transform.right * firePosOffset.x;
            Vector3 up = model.character.transform.up * firePosOffset.y;

            Vector3 firePos = model.character.transform.position + forward + right + up;
            // todo Use FX Manager to control the particle pool, so that no need to get component all the time

            GameObject go = null;
            if (attachParent)
            {
                go = GameObject.Instantiate(prefab, firePos, model.character.transform.rotation, model.character.transform);
            }
            else
            {
                go = GameObject.Instantiate(prefab, firePos, model.character.transform.rotation);
            }
            go.AddComponent<ModelReference>().model = model;
            GameObject.Destroy(go, 3f);
        }

    }
}