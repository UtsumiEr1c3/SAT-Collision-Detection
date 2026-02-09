# Project Context

## Purpose
本项目是一个基于分离轴定理（SAT - Separating Axis Theorem）的高性能碰撞检测系统，使用 Unity 引擎开发。项目的主要目标是：

- 实现高效的 2D/3D 几何体碰撞检测算法
- 利用 Unity Burst Compiler 和 Jobs System 实现多线程并行计算
- 提供可扩展的碰撞检测框架，支持多种几何形状（凸包、多面体）
- 优化内存使用，减少 GC 分配，提升运行时性能

### 当前项目状态
- 已实现核心数据结构：NativeHull（原生凸包）、NativeFace（面）、NativePlane（平面）、NativeHalfEdge（半边）
- 已实现通用工具：BurstAction（Burst 编译的操作）、DebugDrawer（场景调试可视化）、NativeBuffer（自定义原生缓冲区）
- 测试类：TestShape、HullTester 用于验证功能

## Tech Stack
- **Unity 引擎**: Unity 2022.3.62f1
- **编程语言**: C# (.NET Framework)
- **性能优化**:
  - Unity Burst Compiler (v1.8.25) - 将 C# 代码编译为优化的原生代码
  - Unity Jobs System - 多线程并行计算框架
  - Unity Collections - 高性能原生集合类型（NativeArray, NativeBuffer）
  - Unity Mathematics - 高性能数学库（float3, 四元数等）
- **内存管理**:
  - Unity.Collections.LowLevel.Unsafe - 底层内存操作
  - 自定义 NativeBuffer 和 NativeArrayNoLeakDetection 实现

## Project Structure

```
Assets/Scripts/
├── Common/                      # 通用工具类命名空间
│   ├── BurstAction.cs          # Burst 编译的泛型操作执行器
│   ├── DebugDrawer.cs          # 场景视图调试绘制工具
│   ├── NativeBuffer.cs         # 自定义原生缓冲区（NativeList 替代品）
│   ├── NativeArrayNoLeakDetection.cs  # 跳过泄漏检测的 NativeArray
│   └── UnityColors.cs          # Unity 颜色工具
├── UnityNativeHull/            # 原生几何体数据结构
│   ├── NativeHull.cs           # 原生凸包结构（顶点、面、边）
│   ├── NativeFace.cs           # 面数据结构
│   ├── NativePlane.cs          # 平面数据结构
│   └── NativeHalfEdge.cs       # 半边数据结构
├── TestShape.cs                # 测试形状结构体
└── HullTester.cs              # 凸包测试器
```

## Project Conventions

### Code Style
- **命名空间**:
  - `Common` - 通用工具类（BurstAction, DebugDrawer, NativeBuffer, NativeArrayNoLeakDetection, UnityColors）
  - `UnityNativeHull` - 原生几何体相关结构（NativeHull, NativeFace, NativePlane, NativeHalfEdge）
- **命名约定**:
  - 接口以 `I` 开头（如 `IBurstOperation`, `IDebugDrawing`）
  - 结构体使用 PascalCase（如 `BurstRefAction`, `NativeBuffer`, `TestShape`）
  - 私有字段使用 `_` 前缀（如 `_maxIndex`, `_lastFrame`, `_isCreated`）
- **注释规范**:
  - 使用 XML 文档注释（`/// <summary>`）为公共 API 提供文档
  - 关键算法和复杂逻辑使用中文注释说明
  - 使用 `[Conditional]` 特性标记仅在特定条件下编译的代码
- **代码组织**:
  - 相关功能组织在同一命名空间下
  - 使用 `#if UNITY_EDITOR` 条件编译区分编辑器代码
  - Assets/Scripts/Common - 通用工具类
  - Assets/Scripts/UnityNativeHull - 凸包和几何体数据结构
  - Assets/Scripts/ - 测试和示例代码

### Architecture Patterns
- **Burst 编译优化**: 
  - 关键计算路径使用 `[BurstCompile]` 特性标记
  - 结构体实现 `IJob` 接口以支持 Jobs 系统并行执行
  - 使用泛型约束确保类型安全（`where T : unmanaged`, `where T : struct`）
- **接口驱动设计**:
  - `IBurstOperation` - 标识所有 Burst 操作的基础接口
  - `IBurstRefAction<T1, T2, T3, T4, T5>` - 定义带引用参数的操作接口
  - `IDebugDrawing` - 定义可绘制调试信息的接口
- **内存管理**:
  - 使用 `NativeBuffer<T>` 替代 `NativeList<T>`（在 Burst Job 中更稳定）
  - 使用 `NativeArrayNoLeakDetection<T>` 减少内存泄漏检测开销
  - 所有 Native 集合必须正确 Dispose，避免内存泄漏
- **调试支持**:
  - `DebugDrawer` 静态类提供场景视图中的可视化调试功能
  - 支持绘制多边形、球体、箭头、圆形等调试图形

### Testing Strategy
- **当前测试方法**:
  - 使用 `TestShape` 结构体进行形状数据测试
  - 使用 `HullTester` 类进行凸包功能测试
  - 场景视图中使用 `DebugDrawer` 可视化验证
- **未来测试计划**:
  - 添加 Unity Test Framework 单元测试验证碰撞检测算法的正确性
  - 性能测试确保 Burst 编译后的性能提升
  - 边界情况测试（空集合、零向量、重叠几何体、退化情况等）
  - 压力测试（大量并发碰撞检测）

### Git Workflow
- **提交信息格式**: 使用约定式提交（Conventional Commits）格式：
  - 格式：`<type>(scope) description`
  - 示例：`<feat>(test) add TestShape`、`<feat>(NativeHull) add NativeHull struct`、`<style>(space)delete space`
  - 常见类型：`feat`（新功能）、`update`（更新）、`style`（代码格式）、`fix`（修复）
- **分支策略**:
  - 当前在 `main` 分支上进行开发
  - 建议：功能分支开发（feature branches）用于大型功能
- **重要变更**:
  - 重大功能变更前创建 OpenSpec 提案（参考 `openspec/` 目录）

## Domain Context
- **分离轴定理（SAT）**: 
  - 用于检测凸多边形/多面体之间的碰撞
  - 核心思想：如果两个凸形状不相交，则存在一条分离轴
  - 需要计算形状在每条可能轴上的投影并检查重叠
- **Burst 编译限制**:
  - 只能使用 blittable 类型（值类型或包含 blittable 字段的结构体）
  - 不能使用托管引用、字符串操作、异常处理等
  - 需要使用 `[BurstDiscard]` 标记无法在 Burst 中执行的代码
- **Jobs 系统**:
  - `IJob` - 单线程作业
  - `IJobParallelFor` - 并行作业（需要确保线程安全）
  - 使用 `NativeDisableUnsafePtrRestriction` 允许不安全指针操作

## Important Constraints
- **性能要求**: 
  - 碰撞检测必须在高频率下运行（每帧可能检测数百/数千次）
  - 必须最小化 GC 分配，避免帧率波动
  - 充分利用多核 CPU 进行并行计算
- **内存安全**:
  - 所有 Native 集合必须正确 Dispose
  - 使用 `unsafe` 代码时需要确保指针操作的安全性
  - 注意 `AtomicSafetyHandle` 的读写权限检查
- **平台兼容性**:
  - Burst 编译的代码在不同平台（Windows, macOS, Linux, 移动平台）上行为一致
  - 编辑器调试功能仅在使用 `#if UNITY_EDITOR` 时编译
- **类型约束**:
  - Burst 编译的结构体必须满足 `unmanaged` 约束
  - 泛型类型参数需要明确约束以确保类型安全

## External Dependencies
- **Unity 包**（来自 Packages/manifest.json）:
  - `com.unity.burst` (v1.8.25) - Burst 编译器
  - `Unity.Collections` - 原生集合类型
  - `Unity.Jobs` - Jobs 系统
  - `Unity.Mathematics` - 高性能数学库
  - `com.unity.textmeshpro` (v3.0.7) - 文本渲染
  - `com.unity.ugui` (v1.0.0) - Unity UI 系统
  - `com.unity.visualscripting` (v1.9.4) - 可视化脚本
  - `com.boxqkrtm.ide.cursor` - 自定义 IDE 集成（本地包）
- **Unity 编辑器功能**:
  - `UnityEditor.Handles` - 场景视图绘制（仅编辑器）
  - `UnityEditor.SceneView` - 场景视图事件（仅编辑器）
- **系统依赖**:
  - .NET Framework（Unity 2022.3 使用）
  - 支持 Burst 编译的目标平台运行时
