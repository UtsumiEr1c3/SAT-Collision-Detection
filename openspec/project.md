# Project Context

## Purpose
本项目是一个基于分离轴定理（SAT - Separating Axis Theorem）的高性能碰撞检测系统，使用 Unity 引擎开发。项目的主要目标是：

- 实现高效的 2D/3D 几何体碰撞检测算法
- 利用 Unity Burst Compiler 和 Jobs System 实现多线程并行计算
- 提供可扩展的碰撞检测框架，支持多种几何形状
- 优化内存使用，减少 GC 分配，提升运行时性能

## Tech Stack
- **Unity 引擎**: Unity 2022.3.62f1
- **编程语言**: C# (.NET Framework)
- **性能优化**:
  - Unity Burst Compiler - 将 C# 代码编译为优化的原生代码
  - Unity Jobs System - 多线程并行计算框架
  - Unity Collections - 高性能原生集合类型（NativeArray, NativeBuffer）
- **内存管理**: 
  - Unity.Collections.LowLevel.Unsafe - 底层内存操作
  - 自定义 NativeBuffer 和 NativeArrayNoLeakDetection 实现

## Project Conventions

### Code Style
- **命名空间**: 使用 `Common` 命名空间组织通用工具类
- **命名约定**: 
  - 接口以 `I` 开头（如 `IBurstOperation`, `IDebugDrawing`）
  - 结构体使用 PascalCase（如 `BurstRefAction`, `NativeBuffer`）
  - 私有字段使用 `_` 前缀（如 `_maxIndex`, `_lastFrame`）
- **注释规范**:
  - 使用 XML 文档注释（`/// <summary>`）为公共 API 提供文档
  - 关键算法和复杂逻辑使用中文注释说明
  - 使用 `[Conditional]` 特性标记仅在特定条件下编译的代码
- **代码组织**:
  - 相关功能组织在同一命名空间下
  - 使用 `#if UNITY_EDITOR` 条件编译区分编辑器代码

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
- 当前项目尚未建立完整的测试框架
- 建议添加：
  - 单元测试验证碰撞检测算法的正确性
  - 性能测试确保 Burst 编译后的性能提升
  - 边界情况测试（空集合、零向量、重叠几何体等）

### Git Workflow
- 当前未明确指定 Git 工作流
- 建议采用：
  - 功能分支开发（feature branches）
  - 提交信息使用中文或英文，清晰描述变更内容
  - 重要功能变更前创建 OpenSpec 提案（参考 `openspec/AGENTS.md`）

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
- **Unity 包**:
  - `Unity.Burst` - Burst 编译器
  - `Unity.Collections` - 原生集合类型
  - `Unity.Jobs` - Jobs 系统
  - `Unity.Mathematics` - 高性能数学库（如使用）
- **Unity 编辑器功能**:
  - `UnityEditor.Handles` - 场景视图绘制（仅编辑器）
  - `UnityEditor.SceneView` - 场景视图事件（仅编辑器）
- **系统依赖**:
  - .NET Framework（Unity 2022.3 使用）
  - 支持 Burst 编译的目标平台运行时
