using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{
    public abstract class GameManager : MonoBehaviour { }

    /// <summary>
    ///  Create an instance for all GameManager_XXXX which inherited from this GameManager<T>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GameManager<T> : GameManager where T : GameManager<T>
    {
        /// <summary>
        /// Access GameManager singleton instance through this propriety.
        /// </summary>
        public static T Instance { get; private set; }

        
        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = (T)this;
            }
        }

      

        protected virtual void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
