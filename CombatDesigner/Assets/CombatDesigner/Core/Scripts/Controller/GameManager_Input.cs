using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{
    /// <summary>
    /// GameManager for Inputs
    /// </summary>
    public class GameManager_Input : GameManager<GameManager_Input>
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
        }

        #region InputBuffer
        // todo put them into the seperate class called InputBuffer

        // Detect if the keys are pressed

        public List<string> inputNames;
        public List<string> buffer;
        void DetectPressedInputs(ref List<string> inputs, float keyLifetime)
        {
            // todo switch to all necessary to optimize
            foreach (string input in inputNames)
            {

                if (Input.GetButtonDown(input))
                    if (!string.IsNullOrEmpty(input))
                    {
                        Debug.Log(input);
                        buffer.Add(input);
                        //StartCoroutine(RemoveKeyFromBuffer(input, keyLifetime));
                    }
            }
        }


        void DetectPressedKeys(ref List<KeyCode> keys, float keyLifetime)
        {
            // todo switch to all necessary to optimize
            foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                    if (kcode != KeyCode.None)
                    {
                        CombatDebugger.Log(kcode.ToString(), LogDomain.Input);
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

    }
}
