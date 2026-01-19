using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace UnityNativeHull
{
    // 在调试器中显示该结构体的信息
    [DebuggerDisplay("NativeFace: Edge = {Edge}")]
    public struct NativeFace
    {
        /// <summary>
        /// 该面（face）起始边的索引
        /// </summary>
        public int Edge;
    }
}
