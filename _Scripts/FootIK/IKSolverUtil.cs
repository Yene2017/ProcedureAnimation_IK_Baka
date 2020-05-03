using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FootIK
{
    public class Bone
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    public class IKSolverUtil
    {
        public static void SolveTrigonometric(Bone[] bones, int first, int second, int third, Vector3 targetPosition, Vector3 bendNormal, float weight)
        {
            if (weight <= 0f) return;

            // 解算三角形斜边向量
            targetPosition = Vector3.Lerp(bones[third].position, targetPosition, weight);
            Vector3 dir = targetPosition - bones[first].position;

            // 解算三角形斜边大小

            float sqrMag1 = SquareDistance(bones[second].position, bones[first].position);
            float sqrMag2 = SquareDistance(bones[third].position, bones[second].position);

            // 构成解算三角形的运动方向
            Vector3 bendDir = Vector3.Cross(dir, bendNormal);

            // 构成解算三角形的中间点的运动方向
            Vector3 toBendPoint = GetDirectionToBendPoint(dir, dir.magnitude, bendDir, sqrMag1, sqrMag2);
            Debug.DrawLine(bones[first].position, bones[second].position + bendDir * 4, Color.red);
            Debug.DrawLine(bones[first].position, toBendPoint, Color.red);

            // 计算中间点运动对应的旋转
            Quaternion q1 = Quaternion.FromToRotation(bones[second].position - bones[first].position, toBendPoint);
            // 混合配置权重
            if (weight < 1f) q1 = Quaternion.Lerp(Quaternion.identity, q1, weight);

            //更新所有位置，旋转数据
            RotateAroundPoint(bones, first, bones[first].position, q1);

            //第二次旋转，旋转Calf骨骼，从而构成三角形
            Quaternion q2 = Quaternion.FromToRotation(bones[third].position - bones[second].position, targetPosition - bones[second].position);
            if (weight < 1f) q2 = Quaternion.Lerp(Quaternion.identity, q2, weight);

            //更新所有位置，旋转数据
            RotateAroundPoint(bones, second, bones[second].position, q2);
        }

        public static float SquareDistance(Vector3 p1, Vector3 p2)
        {
            return (p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y) + (p1.z - p2.z) * (p1.z - p2.z);
        }

        public static void RotateAroundPoint(Bone[] bones, int index, Vector3 point, Quaternion rotation)
        {
            for (int i = index; i < bones.Length; i++)
            {
                Vector3 dir = bones[i].position - point;
                bones[i].position = point + rotation * dir;
                bones[i].rotation = rotation * bones[i].rotation;
            }
        }

        //Calculates the bend direction based on the law of cosines. 
        //NB! Magnitude of the returned vector does not equal to the length of the first bone!
        private static Vector3 GetDirectionToBendPoint(Vector3 direction, float directionMag, Vector3 bendDirection, float sqrMag1, float sqrMag2)
        {
            if (direction == Vector3.zero) return Vector3.zero;

            float x = ((directionMag * directionMag) + (sqrMag1 - sqrMag2)) / 2f / directionMag;
            float y = (float)Mathf.Sqrt(Mathf.Clamp(sqrMag1 - x * x, 0, Mathf.Infinity));

            return Quaternion.LookRotation(direction, bendDirection) * new Vector3(0, y, x);
        }

    }
}
