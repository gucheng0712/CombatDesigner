using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{  
    /// <summary>
    /// Game Manager for GamePlay Functionalities
    /// </summary>
    public  class GameManager_GamePlay:GameManager<GameManager_GamePlay>
    {      
        /// <summary>
        /// A collection of enemies exist in current stage
        /// </summary>
        List<GameObject> enemies = new List<GameObject>();

        /// <summary>
        /// The enemy who is closest to player character
        /// </summary>
       [HideInInspector] public GameObject nearestTarget;

        /// <summary>
        /// The player GameObject
        /// </summary>
        GameObject player;

        protected override void Awake()
        {
            base.Awake();
         
            player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Update()
        {
            DetectEnemies(2f);
        }

        /// <summary>
        /// Detect Enemys every detectCD seconds
        /// </summary>
        /// <param name="detectCD"></param>
        void DetectEnemies(float detectCD)
        {
            StartCoroutine(RecordNearbyTargetsToArray(detectCD));
        }

        /// <summary>
        /// A routine to add all the nearby enemies into array, and find the nearest target
        /// </summary>
        /// <param name="detectCD"></param>
        /// <returns></returns>
        IEnumerator RecordNearbyTargetsToArray(float detectCD)
        {
            yield return new WaitForSeconds(detectCD);
            GetTargetsFromArray();
            GetNearestTarget();
        }

        /// <summary>
        /// Get all the targets
        /// </summary>
        void GetTargetsFromArray()
        {
            if (GameObject.FindWithTag("AI")== null)
            {
                return;
            }
            GameObject[] gos = GameObject.FindGameObjectsWithTag("AI");
            if (gos != null)
            {
                foreach (GameObject go in gos)
                {
                    if (!go.GetComponent<ActorController>().model.isDead)
                    {
                        enemies.Add(go);
                    }
                }
            }
        }

        /// <summary>
        /// Get the nearest target
        /// </summary>
        void GetNearestTarget()
        {
            if (enemies != null && enemies.Count > 0)
            {
                float closestDistance = float.MaxValue;
                foreach (GameObject go in enemies)
                {
                    Vector3 playerPos = player.transform.position;
                    playerPos.y = 0;
                    Vector3 enemyPos = go.transform.position;
                    enemyPos.y = 0;
                    float dist = Vector3.Distance(playerPos, enemyPos);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        nearestTarget = go;
                    }
                }
            }
        }
    }
}
