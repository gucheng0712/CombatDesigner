using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner
{
    public static class CombatDesignerUtils
    {

        public static bool IsWithinRange(this int value, float minimum, float maximum)
        {
            return value >= minimum && value <= maximum;
        }



        public static TargetRelativeLocation GetTargetRelativePosition(Transform character, Transform target)
        {
            TargetRelativeLocation targetRelativeLocation = TargetRelativeLocation.None;
            Vector3 dirToTarget = (target.transform.position - character.transform.position).normalized;
            float dot = Vector3.Dot(dirToTarget, character.transform.forward);
            float cross = Vector3.Cross(character.transform.forward, target.transform.position).y;
            if (dot < 0) // at back side,set animation to gethit_back
            {

                targetRelativeLocation = TargetRelativeLocation.Back;
                // Debug.Log("back");
            }
            else if (dot > 0)// at front side,set animation to gethit_front
            {

                targetRelativeLocation = TargetRelativeLocation.Front;
                //Debug.Log("front");
            }
            if (cross < 0)// at left side,set animation to gethit_left
            {

                targetRelativeLocation = TargetRelativeLocation.Left;
                // Debug.Log("left");
            }
            else if (cross > 0)// at right side,set animation to gethit_right
            {
                targetRelativeLocation = TargetRelativeLocation.Right;
                // Debug.Log("right");
            }
            return targetRelativeLocation;
        }
    }
}






