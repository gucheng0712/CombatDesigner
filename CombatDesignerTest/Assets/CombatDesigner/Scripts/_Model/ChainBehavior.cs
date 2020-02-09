using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{
    /// <summary>
    /// A class type of character's chain behavior tree
    /// </summary>
    [System.Serializable]
    public class ChainBehavior
    {
        /// <summary>
        /// The name of the ChainBehavior
        /// </summary>
        [Header("====================================")]
        [Space(10)]
        public string name;

        /// <summary>
        /// The ID of the ChainBehavior
        /// </summary>
        public int idIndex;

        /// <summary>
        /// The behavior this ChainBehavior represents.
        /// </summary>
        public ActorBehavior behavior;

        /// <summary>
        /// the index of leaf nodes.
        /// </summary>
        public List<int> followUps = new List<int>();

        //public bool strict; //Alternatively I could make a whole new Command State

        /// <summary>
        /// A flag to determine the ChainBehavior is active or not.
        /// </summary>
        public bool activated;

        /// <summary>
        /// The priority of ChainBehavior. From large to small
        /// </summary>
        public int priority;

        /// <summary>
        /// Constructor with no parameters
        /// </summary>
        public ChainBehavior()
        {

        }

        /// <summary>
        /// Constructor with id behavior and followUps
        /// </summary>
        /// <param name="id"></param>
        /// <param name="behavior"></param>
        /// <param name="_followUps"></param>
        public ChainBehavior(int id, ActorBehavior behavior, List<int> _followUps)
        {
            
            this.idIndex = id;
            this.behavior = behavior;

            // because its reference type
            this.followUps.Clear();
            foreach (var followUp in _followUps)
            {
                this.followUps.Add(followUp);
            }
        }
    }
}
