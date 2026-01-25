using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Common;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityNativeHull
{
    /// <summary>
    /// 一个原生的几何体, 比如多面体或凸包
    /// </summary>
    public unsafe struct NativeHull : IDisposable
    {
        /// <summary>
        /// 顶点数量
        /// </summary>
        public int VertexCount; 

        /// <summary>
        /// 面数量
        /// </summary>
        public int FaceCount; 

        /// <summary>
        /// 边数量
        /// </summary>
        public int EdgeCount; 
        
        // Unity 为 NativeArray, NativeList 等容器提供了安全检查机制(LeakDetection), 用来防止内存泄漏和越界访问
        // 但是这些检查机制会带来一定的性能损失, 特别是在大量数据操作时
        // 因此, 我们可以通过 NativeDisableUnsafePtrRestriction 特性来绕过这些安全检查
        // 从而提高性能
        // 但是需要注意的是, 使用原生指针(unsafe指针)时, 需要手动管理内存泄漏和越界访问
        // 因此, 在使用原生指针时, 需要手动管理内存泄漏和越界访问
        // 告知 NativeArray 在构造时跳过 LeakDetection 注册, 用来提高速度
        /// <summary>
        /// 顶点数组
        /// </summary>
        public NativeArrayNoLeakDetection<float3> VerticesNative; 

        /// <summary>
        /// 面数组
        /// </summary>
        public NativeArrayNoLeakDetection<NativeFace> FacesNative; 

        /// <summary>
        /// 平面数组
        /// </summary>
        public NativeArrayNoLeakDetection<NativePlane> PlanesNative;

        /// <summary>
        /// 半边数组
        /// </summary>
        public NativeArrayNoLeakDetection<NativeHalfEdge> HalfEdgesNative; 

        // 允许在 Native 容器中使用原生指针(unsafe指针), 绕过安全检查限制
        /// <summary>
        /// 顶点指针
        /// </summary>
        [NativeDisableUnsafePtrRestriction]
        public unsafe float3* Vertices; 

        /// <summary>
        /// 面指针
        /// </summary>
        [NativeDisableUnsafePtrRestriction]
        public unsafe NativeFace* Faces; // 面指针

        /// <summary>
        /// 平面指针
        /// </summary>
        [NativeDisableUnsafePtrRestriction]
        public unsafe NativePlane* Planes; 

        /// <summary>
        /// 半边指针
        /// </summary>
        [NativeDisableUnsafePtrRestriction]
        public unsafe NativeHalfEdge* HalfEdges; 

        private int _isCreated; // 标记结构体是否已创建
        private int _isDisposed; // 标记结构体是否已释放

        /// <summary>
        /// 判断结构体是否已经创建
        /// </summary>
        public bool IsCreated { get => _isCreated == 1; set => _isCreated = value ? 1 : 0; }

        /// <summary>
        /// 判断结构体是否已经释放
        /// </summary>
        public bool IsDisposed { get => _isDisposed == 1; set => _isDisposed = value ? 1 : 0; }
    
        /// <summary>
        /// 判断结构体是否有效
        /// </summary>
        public bool IsValid => IsCreated && !IsDisposed;

        public void Dispose()
        {
            if (_isDisposed == 1)
            {
                return;
            }

            _isDisposed = 1;

            if (VerticesNative.IsCreated)
            {
                VerticesNative.Dispose();
            }
            if (FacesNative.IsCreated)
            {
                FacesNative.Dispose();
            }
            if (PlanesNative.IsCreated)
            {
                PlanesNative.Dispose();
            }
            if (HalfEdgesNative.IsCreated)
            {
                HalfEdgesNative.Dispose();
            }

            Vertices = null;
            Faces = null;
            Planes = null;
            HalfEdges = null;
        }
    }
}
