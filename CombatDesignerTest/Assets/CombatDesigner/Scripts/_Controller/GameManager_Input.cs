using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{  
    /// <summary>
    /// GameManager for Inputs
    /// </summary>
    public  class GameManager_Input:GameManager<GameManager_Input>
    {      
        /// <summary>
        /// The collection of all input key keycode
        /// </summary>
        //public List<KeyCode> allKeys;

        /// <summary>
        /// The length of each buffer
        /// </summary>
        public int bufferFrameCount = 25;
        /// <summary>
        /// The keys existing inside the buffer
        /// </summary>
        public List<KeyCode> bufferKeys = new List<KeyCode>();

        /// <summary>
        /// The time that key will exist inside the buffer
        /// </summary>
        public float keycodeLifeTime = 0.1f;

        /// <summary>
        /// The default frame rate of the system
        /// </summary>
        public int targetFrameRate = 60;

        /// <summary>
        /// A multiplier for the frame rate
        /// </summary>
        public float frameRangeMultiplier = 1;

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
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFrameRate;
            frameRangeMultiplier = (float)targetFrameRate / 60f;
         
            player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Update()
        {
            DetectPressedKeys(ref bufferKeys, keycodeLifeTime);
            DetectEnemies(2f);
        }

        #region InputBuffer
        // todo put them into the seperate class called InputBuffer

        // Detect if the keys are pressed
        void DetectPressedKeys(ref List<KeyCode> keys, float keyLifetime)
        {
            // todo switch to all necessary to optimize
            foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                    if (kcode != KeyCode.None)
                    {
                        keys.Add(kcode);
                        StartCoroutine(RemoveKeyFromBuffer(kcode, keyLifetime));
                    }
            }
        }

        // Keycode from the input buffer will be destroyed in certain time.
        IEnumerator RemoveKeyFromBuffer(KeyCode keycode, float time)
        {
            yield return new WaitForSeconds(time);
            bufferKeys.Remove(keycode);
        }

        // Keycode from the input buffer will be destroyed at the end of the frame.
        IEnumerator RemoveKeyFromBuffer(KeyCode keycode)
        {
            yield return new WaitForEndOfFrame();
            bufferKeys.Remove(keycode);
        }

        // Keycode from the input buffer will be destroyed immediately.
        void RemoveKeyFromBufferImmediately(KeyCode keycode)
        {
            bufferKeys.Remove(keycode);
        }
        #endregion


        void DetectEnemies(float detectCD)
        {
            StartCoroutine(RecordNearbyTargetsToArray(detectCD));
        }

        // Add all the nearby enemies into array
        IEnumerator RecordNearbyTargetsToArray(float detectCD)
        {
            yield return new WaitForSeconds(detectCD);
            GetTargetsFromArray();
            GetNearestTarget();
        }

        // Get all the targets
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

        // Get the nearest target
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
