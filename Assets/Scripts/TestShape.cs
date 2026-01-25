using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Unity.Burst;
using System;
using JetBrains.Annotations;
using UnityNativeHull;

/// <summary>
/// 一个测试形状, 包含唯一标识符和外壳(例如多面体)
/// </summary>
[DebuggerDisplay("TestSHape: Id = {Id}")]
[BurstCompile] //启用Burst编译器优化此结构体的性能
public struct TestShape : IEquatable<TestShape>, IComparable<TestShape> // IEquatable是为了重载 Equals 判等操作, IComparable是为了重载 CompareTo 排序操作
{
    /// <summary>
    /// 形状的唯一标识符
    /// </summary>
    public int Id;

    /// <summary>
    /// 形状的外壳(例如多面体)
    /// </summary>
    public NativeHull Hull;

    /// <summary>
    /// 实现IEquatable<TestShape>接口, 重载 Equals 判等操作, 用于比较两个TestShape对象是否相等
    /// </summary>
    /// <param name="other">另一个TestShape对象</param>
    /// <returns>如果两个TestShape对象的Id相等, 则返回true, 否则返回false</returns>
    public bool Equals([CanBeNull] TestShape other)
    {
        return Id == other.Id; // 直接比较Id字段
    }

    /// <summary>
    /// 实现IComparable<TestShape>接口, 重载 CompareTo 排序操作, 用于根据 Id 对 TestShape 进行比较
    /// </summary>
    /// <param name="other">另一个TestShape对象</param>
    /// <returns>如果当前TestShape对象的Id小于other对象的Id, 则返回-1, 如果等于则返回0, 如果大于则返回1</returns>
    public int CompareTo([CanBeNull] TestShape other)
    {
        return Id.CompareTo(other.Id);
    }

    /// <summary>
    /// 重载 GetHashCode 方法, 用于为 TestShape 生成哈希码
    /// </summary>
    /// <returns>TestShape 对象的哈希值</returns>
    public override int GetHashCode()
    {
        return Id;
    }
}

