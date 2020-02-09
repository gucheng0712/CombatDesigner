
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatDesigner.EditorTool
{
    /// <summary>    ///  Helper Rect extension methods for the Rect class. This class must be static.    /// </summary>
    public static class RectExtensions
    {
        /// <summary>
        /// An extension methods to return the top left position of a Rect
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Vector2 TopLeft(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMin);
        }
        /// <summary>
        /// An extension methods to resize the rect based on one value
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Rect ScaleSizeBy(this Rect rect, float scale)
        {
            return rect.ScaleSizeBy(scale, rect.center);
        }
        /// <summary>
        /// An extension methods to resize the rect based on one value through a pivot point
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scale"></param>
        /// <param name="pivotPoint"></param>
        /// <returns></returns>
        public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
        {
            Rect result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale;
            result.xMax *= scale;
            result.yMin *= scale;
            result.yMax *= scale;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }

        /// <summary>
        /// An extension methods to resize the rect based on x y values
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
        {
            return rect.ScaleSizeBy(scale, rect.center);
        }

        /// <summary>
        /// An extension methods to resize the rect based on on x y values through a pivot point
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scale"></param>
        /// <param name="pivotPoint"></param>
        /// <returns></returns>
        public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
        {
            Rect result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale.x;
            result.xMax *= scale.x;
            result.yMin *= scale.y;
            result.yMax *= scale.y;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }
    }
}#endif