using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityNativeHull;


#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode] // 在编辑器模式下执行
public class HullTester : MonoBehaviour
{
    /// <summary>
    /// 要测试的节点列表
    /// </summary>
    public List<Transform> Transforms;

    [Header("可视化选项")]
    /// <summary>
    /// 是否绘制碰撞状态
    /// </summary>
    public bool IsDrawCollided;

    /// <summary>
    /// 是否绘制相交区域
    /// </summary>
    public bool IsDrawIntersection;

    [Header("控制台日志")]
    /// <summary>
    /// 是否记录接触日志
    /// </summary>
    public bool IsLogContact;

    /// <summary>
    /// 测试凸包字典, 用于存储测试形状
    /// </summary>
    private Dictionary<int, TestShape> _testHullsDic;

    void Update()
    {
        HandleTransformChanged();
        HandleHullCollisions();
    }

    void OnDestroy()
    {
        EnsureDestroyed();
    }

    void OnDisable()
    {
        EnsureDestroyed();
    }

    /// <summary>
    /// 处理Transform变化, 判断是否需要更新测试凸包字典
    /// </summary>
    private void HandleTransformChanged()
    {
        // Transforms.ToList() 为了得到副本不影响原来的, Distinct() 为了去重, Where(t => t.gameObject.activeSelf) 为了过滤掉不活跃的Transform, ToList() 为了转换回List
        // TODO: 这里有 GC 的问题, 后续需要优化
        var transforms = Transforms.ToList().Distinct().Where(t => t.gameObject.activeSelf).ToList();
        var isNewTransformFound = false; // 是否找到新的Transform
        var transformCount = 0; // 当前Transform数量
        
        // 检测是否有新的 Transform
        // 如果 _testHullsDic 已存在，遍历过滤后的列表
        if (_testHullsDic != null)
        {
            for(var i = 0; i < transforms.Count; i++)
            {
                var t = transforms[i];
                if (t == null)
                {
                    continue;
                }

                transformCount++;

                var isFoundNewHull = !_testHullsDic.ContainsKey(t.GetInstanceID());

                // 如果找到新的凸包，则设置标志并退出循环
                if (isFoundNewHull)
                {
                    isNewTransformFound = true;
                    break;
                }
            }

            // 如果没有新 Transform 且数量相同，直接返回，避免不必要的更新
            if (!isNewTransformFound && transformCount == _testHullsDic.Count)
            {
                return;
            }
        }

        Debug.Log("重建对象");

        // 安全的释放资源
        EnsureDestroyed();

        // 保存不为空的节点, InstanceID 作为 key, 创建的 TestShape 作为 value 记录在字典里
        _testHullsDic = transforms.Where(t => t != null).ToDictionary(k => k.GetInstanceID(), CreateShape);

        // Unity 编辑器中的一个方法, 强制刷新 Scene 视图以反映更改
        SceneView.RepaintAll();
    }

    /// <summary>
    /// 创建测试形状
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private TestShape CreateShape(Transform t)
    {
        var hull = CreateHull(t);

        return new TestShape
        {
            Id = t.GetInstanceID(),
            Hull = hull,
        };
    }

    /// <summary>
    /// 创建凸包
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private NativeHull CreateHull(Transform t)
    {
        var collider = t.GetComponent<Collider>();
        if (collider is MeshCollider meshCollider)
        {
            // TODO: return 一个 NativeHull, 然后 create 用 meshCollider 的 sharedMesh
        }

        var meshFilter = t.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            // TODO: return 一个 NativeHull, 然后 create 用 meshCollider 的 sharedMesh
        }

        throw new Exception($"无法为对象 {t.name} 创建凸包, 缺少 MeshCollider 或 MeshFilter 组件");
    }

    /// <summary>
    /// 处理凸包碰撞检测
    /// </summary>
    private void HandleHullCollisions()
    {
        // TODO: 实现凸包碰撞检测逻辑
    }
    
    /// <summary>
    /// 确保销毁所有测试凸包
    /// </summary>
    private void EnsureDestroyed()
    {
        if (_testHullsDic == null)
        {
            return;
        }

        foreach(var kv in _testHullsDic)
        {
            if (kv.Value.Hull.IsValid)
            {
                kv.Value.Hull.Dispose();
            }
        }

        _testHullsDic.Clear();
    }
}
