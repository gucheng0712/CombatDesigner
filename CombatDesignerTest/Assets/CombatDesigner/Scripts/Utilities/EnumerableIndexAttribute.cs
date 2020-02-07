using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{
    /// <summary>
    /// A custom attribute to convert index to enum representation
    /// </summary>
    public class EnumerableIndexAttribute : PropertyAttribute
    {
        /// <summary>
        /// The type of the Enum popup
        /// </summary>
        public enum ItemType
        {
            Behavior,
            ChainBehavior,
            Action
        }

        public ItemType itemType;

        public EnumerableIndexAttribute(ItemType type)
        {
            this.itemType = type;
        }
    }
}