using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{
    public class IBox : MonoBehaviour
    {
        protected ActorModel model;
        protected BoxCollider col;
        protected LineRenderer lr;
        protected MeshRenderer mr;

        public bool visualizeBox;

        public virtual void Init()
        {
            model = transform.root.GetComponent<ActorController>().model;
            col = GetComponent<BoxCollider>();
            if (visualizeBox)
            {
                lr = GetComponent<LineRenderer>();
                mr = GetComponent<MeshRenderer>();
            }
        }

        public virtual void SetActive(bool enable)
        {
            if (col != null)
            {
                col.enabled = enable;
            }
            if (visualizeBox)
            {
                if (lr != null)
                    lr.enabled = enable;
                if (mr != null)
                    mr.enabled = enable;
            }
        }
        /// <summary>
        /// A method to set hitbox scale
        /// </summary>
        /// <param name="scale"></param>
        public void SetLocalScale(Vector3 scale)
        {
            transform.localScale = scale;
        }
        /// <summary>
        /// A method to set hitbox position
        /// </summary>
        /// <param name="pos"></param>
        public void SetLocalPosition(Vector3 pos)
        {
            transform.localPosition = pos;
        }
        /// <summary>
        /// A method to set hitbox position
        /// </summary>
        /// <param name="pos"></param>
        public void SetLocalRotation(Quaternion rot)
        {
            transform.localRotation = rot;
        }
    }
}










