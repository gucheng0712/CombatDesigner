using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CombatDesigner.EditorTool
{
    public class ActorCreator : EditorWindow
    {
        public GameObject actorMeshPrefab;

        GameObject actor;

        [MenuItem("CombatDesigner/Actor Creator")]
        static void Open()
        {
            ActorCreator win = EditorWindow.GetWindow<ActorCreator>();
            win.minSize = win.maxSize = new Vector2(215f, 64f);
            win.titleContent = new GUIContent("Actor Creator");
            win.Show();

        }

        private void OnGUI()
        {
            actorMeshPrefab = (GameObject)EditorGUILayout.ObjectField(actorMeshPrefab, typeof(GameObject), false);

            EditorGUILayout.Space(10);

            if (actorMeshPrefab == null)
                return;

            if (GUILayout.Button("Create Player", GUILayout.Height(30)))
            {
                actor = new GameObject(actorMeshPrefab.name);
                ActorController controller = actor.AddComponent<DefaultActorController>();
                controller.model = actor.GetComponent<ActorModel>();

                // Add Character Mesh object
                GameObject meshObj = PrefabUtility.InstantiatePrefab(actorMeshPrefab) as GameObject;
                meshObj.transform.SetParent(actor.transform);
                if (meshObj.GetComponent<Animator>())
                {
                    meshObj.GetComponent<Animator>().applyRootMotion = false;
                }
                else
                {
                    meshObj.AddComponent<Animator>().applyRootMotion = false;
                }


                // Add HitbBox
                GameObject hitBoxGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                hitBoxGO.name = "HitBox";
                hitBoxGO.transform.SetParent(actor.transform);
                hitBoxGO.AddComponent<HitBox>();
                hitBoxGO.GetComponent<Rigidbody>().isKinematic = true;
                hitBoxGO.GetComponent<LineRenderer>().enabled = false;
                ColliderVisualizer hitBoxVisualizer = hitBoxGO.GetComponent<ColliderVisualizer>();
                hitBoxVisualizer.lineColor = new Color(0, 0.72f, 1f, 1f);
                MeshRenderer hitBoxRenderer = hitBoxGO.GetComponent<MeshRenderer>();
                hitBoxRenderer.material = (Material)AssetDatabase.LoadAssetAtPath("Assets/Plugins/CombatDesigner/Artworks/Materials/HitBoxMat.mat", typeof(Material));
                hitBoxRenderer.enabled = false;
                SettingUtils.AddLayer("HitBox");
                hitBoxGO.layer = LayerMask.NameToLayer("HitBox");

                // Add HurtBox
                GameObject hurtBoxGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                hurtBoxGO.name = "HurtBox";
                hurtBoxGO.transform.SetParent(actor.transform);
                hurtBoxGO.AddComponent<HurtBox>();
                hurtBoxGO.GetComponent<LineRenderer>().enabled = false;
                ColliderVisualizer hurtBoxVisualizer = hurtBoxGO.GetComponent<ColliderVisualizer>();
                hurtBoxVisualizer.lineColor = new Color(1, 0.06f, 0.06f, 1);
                MeshRenderer hurtBoxRenderer = hurtBoxGO.GetComponent<MeshRenderer>();
                hurtBoxRenderer.material = (Material)AssetDatabase.LoadAssetAtPath("Assets/Plugins/CombatDesigner/Artworks/Materials/HurtBoxMat.mat", typeof(Material));
                hurtBoxRenderer.enabled = false;
                SettingUtils.AddLayer("HurtBox");
                hurtBoxGO.layer = LayerMask.NameToLayer("HurtBox");
            }
        }
    }
}












