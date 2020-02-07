using UnityEngine;

namespace CombatDesigner
{
    public class CastPrefabAction : IBehaviorAction
    {
        public GameObject prefab;
        public Transform firePoint;

        public override void Execute(ActorModel model)
        {
            Debug.Log("Play vfx");
            if (prefab == null) return;


            // todo Use FX Manager to control the particle pool, so that no need to get component all the time
            GameObject go = GameObject.Instantiate(prefab, firePoint.position, firePoint.rotation);
            GameObject.Destroy(go, 3f);
        }
    }
}