
using System;
using UnityEngine;

namespace CombatDesigner
{
    /// <summary>
    /// A class to visualize the collider in the game scene
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ColliderVisualizer : MonoBehaviour
    {
        /// <summary>
        /// Color of the wireframe
        /// </summary>
        public Color lineColor = Color.blue;

        /// <summary>
        /// width of the wireframe's wire
        /// </summary>
        public float lineWidth = 0.3f;

        void Start()
        {
            Init();
        }

        public void EnableColliderVisualizer(bool enable)
        {
            LineRenderer lr = GetComponent<LineRenderer>();
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (lr != null)
            {
                lr.enabled = enable;
            }
            if (mr != null)
            {
                mr.enabled = enable;
            }
            if (enable)
                Init();
        }

        /// <summary>
        /// Initialization
        /// </summary>
        public void Init()
        {
            Collider col = GetComponent<Collider>();
            LineRenderer lr = GetComponent<LineRenderer>();

            if (lr == null)
            {
                lr = gameObject.AddComponent<LineRenderer>();
            }


            if (col is BoxCollider)
            {
                // Draw box
                DrawBoxCollider(col as BoxCollider, lr, lineWidth);
            }
            else if (col is SphereCollider)
            {
                // Draw sphere
                DrawSphereCollider(col as SphereCollider, lr, lineWidth);
            }
            else if (col is CapsuleCollider)
            {
                // Draw capsule
                DrawCapsuleCollider(col as CapsuleCollider, lr, lineWidth);
            }
            else
            {
                Debug.LogError("do not support that:" + col);
                return;
            }

            // change the material and color
            lr.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
            lr.sharedMaterial.color = this.lineColor;
        }

        /// <summary>
        /// A method to draw box
        /// </summary>
        /// <param name="col"></param>
        /// <param name="lr"></param>
        /// <param name="lineWidth"></param>
        void DrawBoxCollider(BoxCollider col, LineRenderer lr, float lineWidth)
        {
            Vector3 pos = col.center;
            Vector3 halfSize = col.size / 2;
            Vector3 offset = new Vector3(-halfSize.x, halfSize.y, halfSize.z);

            lr.startWidth = halfSize.magnitude / 20.0f * lineWidth;
            lr.endWidth = halfSize.magnitude / 20.0f * lineWidth;
            lr.useWorldSpace = false;
            Vector3[] corners = GetBoxCornerPos(pos, halfSize, offset);
            SetupBoxLineRenderer(lr, corners);
        }

        /// <summary>
        /// A method to get all conner position of the box
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="halfSize"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Vector3[] GetBoxCornerPos(Vector3 pos, Vector3 halfSize, Vector3 offset)
        {
            Vector3[] corners = new Vector3[8];
            // Calculate Box Corner Position
            corners[0] = pos + (offset); //(0.5,0.5,0.5)+(-0.5,0.5,0.5) = (0,1,1)

            offset.x = halfSize.x;
            corners[1] = pos + (offset); //(0.5,0.5,0.5)+(0.5,0.5,0.5) = (1,1,1)

            offset.z = -halfSize.z;
            corners[2] = pos + (offset); //(0.5,0.5,0.5)+(0.5,0.5,-0.5) = (1,1,0)

            offset.x = -halfSize.x;
            corners[3] = pos + (offset); //(0.5,0.5,0.5)+(-0.5,0.5,-0.5) = (0,1,0)

            offset.y = -halfSize.y;
            offset.z = halfSize.z;
            corners[4] = pos + (offset); //(0.5,0.5,0.5)+(-0.5,-0.5,0.5) = (0,0,1)

            offset.x = halfSize.x;
            corners[5] = pos + (offset); //(0.5,0.5,0.5)+(0.5,-0.5,0.5) = (1,0,1)

            offset.z = -halfSize.z;
            corners[6] = pos + (offset); //(0.5,0.5,0.5)+(0.5,-0.5,-0.5) = (1,0,0)

            offset.x = -halfSize.x;
            corners[7] = pos + (offset); //(0.5,0.5,0.5)+(-0.5,-0.5,-0.5) = (0,0,0)

            return corners;
        }

        /// <summary>
        /// A method to set up the line renderer for box
        /// </summary>
        /// <param name="lr"></param>
        /// <param name="corners"></param>
        void SetupBoxLineRenderer(LineRenderer lr, Vector3[] corners)
        {
            int seek = 0;
            lr.positionCount = 46;
            //top //13
            lr.SetPosition(seek, corners[0]); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[0], corners[1], 0.05f)); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[0], corners[1], 0.95f)); seek++;
            lr.SetPosition(seek, corners[1]); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[1], corners[2], 0.05f)); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[1], corners[2], 0.95f)); seek++;
            lr.SetPosition(seek, corners[2]); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[2], corners[3], 0.05f)); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[2], corners[3], 0.95f)); seek++;
            lr.SetPosition(seek, corners[3]); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[3], corners[0], 0.05f)); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[3], corners[0], 0.95f)); seek++;
            lr.SetPosition(seek, corners[0]); seek++;
            //door1 //9
            lr.SetPosition(seek, Vector3.Lerp(corners[0], corners[4 + 0], 0.05f)); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[0], corners[4 + 0], 0.95f)); seek++;
            lr.SetPosition(seek, corners[4 + 0]); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[4 + 0], corners[4 + 1], 0.05f)); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[4 + 0], corners[4 + 1], 0.95f)); seek++;
            lr.SetPosition(seek, corners[4 + 1]); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[4 + 1], corners[1], 0.05f)); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[4 + 1], corners[1], 0.95f)); seek++;
            lr.SetPosition(seek, corners[1]); seek++;
            //door2 //9
            lr.SetPosition(seek, Vector3.Lerp(corners[1], corners[4 + 1], 0.05f)); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[1], corners[4 + 1], 0.95f)); seek++;
            lr.SetPosition(seek, corners[4 + 1]); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[4 + 1], corners[4 + 2], 0.05f)); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[4 + 1], corners[4 + 2], 0.95f)); seek++;
            lr.SetPosition(seek, corners[4 + 2]); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[4 + 2], corners[2], 0.05f)); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[4 + 2], corners[2], 0.95f)); seek++;
            lr.SetPosition(seek, corners[2]); seek++;
            //door3 //9
            lr.SetPosition(seek, Vector3.Lerp(corners[2], corners[4 + 2], 0.05f)); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[2], corners[4 + 2], 0.95f)); seek++;
            lr.SetPosition(seek, corners[4 + 2]); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[4 + 2], corners[4 + 3], 0.05f)); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[4 + 2], corners[4 + 3], 0.95f)); seek++;
            lr.SetPosition(seek, corners[4 + 3]); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[4 + 3], corners[3], 0.05f)); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[4 + 3], corners[3], 0.95f)); seek++;
            lr.SetPosition(seek, corners[3]); seek++;
            //door4 //6
            lr.SetPosition(seek, Vector3.Lerp(corners[3], corners[4 + 3], 0.05f)); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[3], corners[4 + 3], 0.95f)); seek++;
            lr.SetPosition(seek, corners[4 + 3]); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[4 + 3], corners[4 + 0], 0.05f)); seek++;
            lr.SetPosition(seek, Vector3.Lerp(corners[4 + 3], corners[4 + 0], 0.95f)); seek++;
            lr.SetPosition(seek, corners[4 + 0]); seek++;
        }

        /// <summary>
        /// A method to draw sphere
        /// </summary>
        /// <param name="col"></param>
        /// <param name="lr"></param>
        /// <param name="lineWidth"></param>
        void DrawSphereCollider(SphereCollider col, LineRenderer lr, float lineWidth)
        {
            lr.startWidth = col.radius / 10.0f * lineWidth;
            lr.endWidth = col.radius / 10.0f * lineWidth;
            lr.useWorldSpace = false;

            int sides = 40;
            lr.positionCount = sides + 2 + sides + sides / 4 + 1 + sides + 2;

            int seek = 0;

            //��һȦ
            for (int i = 0; i < sides + 2; i++)
            {
                float phi = Mathf.PI * 2.0f * (i / (float)sides);
                Vector3 offset = new Vector3(
                    col.radius * Mathf.Sin(phi), //left/right
                    col.radius * Mathf.Cos(phi),           //up/down
                    0   //forward/backward
                    );
                lr.SetPosition(seek, offset + col.center);
                seek++;
            }
            //�ڶ�Ȧ����1/4
            for (int i = 0; i < sides + sides / 4 + 1; i++)
            {
                float phi = Mathf.PI * 2.0f * (i / (float)sides);
                Vector3 offset = new Vector3(
                    //left/right
                    0,
                    col.radius * Mathf.Cos(phi),       //up/down
                    col.radius * Mathf.Sin(phi)//forward/backward
                    );
                lr.SetPosition(seek, offset + col.center); seek++;

            }
            //����Ȧ
            for (int i = sides / 4; i < sides + 2 + sides / 4; i++)
            {
                float phi = Mathf.PI * 2.0f * (i / (float)sides);
                Vector3 offset = new Vector3(
                    //left/right

                    col.radius * Mathf.Cos(phi),       //up/down
                      0,
                    col.radius * Mathf.Sin(phi)//forward/backward
                    );
                lr.SetPosition(seek, offset + col.center); seek++;
            }

        }

        /// <summary>
        ///  A method to draw capsule
        /// </summary>
        /// <param name="col"></param>
        /// <param name="lr"></param>
        /// <param name="lineWidth"></param>
        void DrawCapsuleCollider(CapsuleCollider col, LineRenderer lr, float lineWidth)
        {
            lr.startWidth = col.radius / 10.0f * lineWidth;
            lr.endWidth = col.radius / 10.0f * lineWidth;
            lr.useWorldSpace = false;

            Quaternion rot = Quaternion.identity;
            if (col.direction == 0)
            {
                rot = Quaternion.Euler(0f, 0f, 90f);
            }
            else if (col.direction == 2)
            {
                rot = Quaternion.Euler(90f, 0f, 0f);
            }
            int sides = 40;
            //int p = 3;
            lr.positionCount = (sides + 2) + (sides + sides / 4 + 1) + (sides + 2) + 1 + (sides + 2);
            int seek = 0;

            //��һȦ
            for (int i = 0; i < sides + 2; i++)
            {
                float phi = Mathf.PI * 2.0f * (i / (float)sides);
                Vector3 offset = new Vector3(
                    col.radius * Mathf.Sin(phi), //left/right
                    col.radius * Mathf.Cos(phi),           //up/down
                    0   //forward/backward
                    );
                if (offset.y < 0) offset.y -= col.height / 2 - col.radius;
                else offset.y += (col.height / 2) - col.radius;
                lr.SetPosition(seek, rot * offset + col.center); seek++;

            }
            //�ڶ�Ȧ����1/4
            for (int i = 0; i < sides + sides / 4 + 1; i++)
            {
                float phi = Mathf.PI * 2.0f * (i / (float)sides);
                Vector3 offset = new Vector3(
                    //left/right
                    0,
                    col.radius * Mathf.Cos(phi),       //up/down
                    col.radius * Mathf.Sin(phi)//forward/backward
                    );
                if (offset.y < 0) offset.y -= col.height / 2 - col.radius;
                else offset.y += (col.height / 2) - col.radius;

                lr.SetPosition(seek, rot * offset + col.center); seek++;

            }
            //����Ȧ
            for (int i = sides / 4; i < sides + 2 + sides / 4; i++)
            {
                float phi = Mathf.PI * 2.0f * (i / (float)sides);
                Vector3 offset = new Vector3(
                    //left/right

                    col.radius * Mathf.Cos(phi),       //up/down
                      0,
                    col.radius * Mathf.Sin(phi)//forward/backward
                    );
                offset.y -= col.height / 2 - col.radius;
                lr.SetPosition(seek, rot * offset + col.center); seek++;

            }
            {
                float phi = Mathf.PI * 2.0f * 0.25f;
                Vector3 offset = new Vector3(
                   //left/right

                   col.radius * Mathf.Cos(phi),        //up/down
                     0,
                   col.radius * Mathf.Sin(phi)//forward/backward
                   );
                offset.y -= col.height / 2 - col.radius;
                lr.SetPosition(seek, rot * offset + col.center); seek++;
            }
            //����Ȧ
            for (int i = sides / 4; i < sides + 2 + sides / 4; i++)
            {
                float phi = Mathf.PI * 2.0f * (i / (float)sides);
                Vector3 offset = new Vector3(
                    //left/right

                    col.radius * Mathf.Cos(phi),       //up/down
                      0,
                    col.radius * Mathf.Sin(phi)//forward/backward
                    );
                offset.y += col.height / 2 - col.radius;
                lr.SetPosition(seek, rot * offset + col.center); seek++;
            }
        }
    }
}