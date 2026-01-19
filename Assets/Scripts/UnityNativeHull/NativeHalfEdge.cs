using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace UnityNativeHull
{
    [DebuggerDisplay("NativeHalfEdge: Origin = {Origin}, Face = {Face}, Twin = {Twin}, Prev = {Prev}, Next = {Next}")]
    public struct NativeHalfEdge
    {
        /// <summary>
        /// 面环中前一条边的索引
        /// </summary>
        public int Prev;

        /// <summary>
        /// 面环中后一条边的索引
        /// </summary>
        public int Next;

        /// <summary>
        /// 与此边相对的的另一条边的索引, 在不同的面环中
        /// </summary>
        public int Twin;

        /// <summary>
        /// 该边起点的顶点索引
        /// </summary>
        public int Origin;
    }
}
