using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Palmmedia.ReportGenerator.Core;
using Unity.Mathematics;
using UnityEngine;

namespace UnityNativeHull
{
    /// <summary>
    /// 三维平面，用于表示一个三维空间中的平面
    /// </summary>
    [DebuggerDisplay("NativePlane: Normal = {Normal}, Offset = {Offset}")]
    public unsafe struct NativePlane
    {
        // 平面方程可以表示为 Normal · P = Offset
        // 其中 Normal 是平面的法向量，P 是平面上的任意一点，Offset 是平面到原点的距离

        /// <summary>
        /// 平面的法向量，表示平面相对于 hull 原点的方向
        /// </summary>
        public float3 Normal;

        /// <summary>
        /// 平面到 hull 原点的距离
        /// </summary>
        public float Offset;

        /// <summary>
        /// 构造一个三维平面
        /// </summary>
        /// <param name="normal">平面的法向量</param>
        /// <param name="offset">平面到 hull 原点的距离</param>
        public NativePlane(float3 normal, float offset)
        {
            Normal = normal;
            Offset = offset;
        }

        /// <summary>
        /// 计算点到平面的距离
        /// </summary>
        /// <param name="point">点</param>
        /// <returns>点到平面的距离</returns>
        public float Distance(float3 point)
        {
            return math.dot(point, Normal) - Offset;
        }

        /// <summary>
        /// 计算某一点到平面的最近点
        /// </summary>
        /// <param name="point">任意点</param>
        /// <returns>点到平面的最近点</returns>
        public float3 ClosestPoint(float3 point)
        {
            return point - Distance(point) * math.normalize(Normal);
        }

        /// <summary>
        /// 通过刚体变换来转换平面
        /// </summary>
        /// <param name="transform">刚体变换</param>
        /// <param name="plane">平面</param>
        /// <returns>转换后的平面</returns>
        public static NativePlane operator *(RigidTransform transform, NativePlane plane)
        {
            float3 normal = math.mul(transform.rot, plane.Normal);
            return new NativePlane(normal, plane.Offset + math.dot(normal, transform.pos));
        }
    }
}
