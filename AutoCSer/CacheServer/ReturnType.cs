﻿using System;

namespace AutoCSer.CacheServer
{
    /// <summary>
    /// 返回值类型
    /// </summary>
    public enum ReturnType : byte
    {
        /// <summary>
        /// 未知错误
        /// </summary>
        Unknown,
        /// <summary>
        /// 操作成功
        /// </summary>
        Success,
        /// <summary>
        /// TCP 通讯错误
        /// </summary>
        TcpError,
        /// <summary>
        /// 反序列化错误
        /// </summary>
        DeSerializeError,
        /// <summary>
        /// 缺少必要的输入参数
        /// </summary>
        NullArgument,
        /// <summary>
        /// 操作类型与返回值匹配错误
        /// </summary>
        OperationTypeReturnValueError,
        /// <summary>
        /// 数据结构定义节点缺少指定的构造函数信息
        /// </summary>
        DataStructureNotFoundConstructor,
        /// <summary>
        /// 不允许创建数据节点
        /// </summary>
        CanNotCreateValueNode,
        /// <summary>
        /// 节点的父节点不匹配
        /// </summary>
        NodeParentSetError,

        /// <summary>
        /// 数据结构定义冲突
        /// </summary>
        DataStructureNameExists,

        /// <summary>
        /// 服务端数据结构定义信息初始化错误
        /// </summary>
        ServerDataStructureCreateError,
        /// <summary>
        /// 没有找到数据结构定义
        /// </summary>
        DataStructureNameNotFound,
        /// <summary>
        /// 数据结构标识错误
        /// </summary>
        DataStructureIdentityError,
        /// <summary>
        /// 不支持的操作类型
        /// </summary>
        OperationTypeError,
        /// <summary>
        /// 参数数据加载错误
        /// </summary>
        ValueDataLoadError,
        /// <summary>
        /// 数组索引超出范围
        /// </summary>
        ArrayIndexOutOfRange,
        /// <summary>
        /// 访问的数组节点为 null
        /// </summary>
        NullArrayNode,
        /// <summary>
        /// 没有找到字典关键字
        /// </summary>
        NotFoundDictionaryKey,
        /// <summary>
        /// 链表索引超出范围
        /// </summary>
        LinkIndexOutOfRange,

        /// <summary>
        /// 缓存已经释放
        /// </summary>
        Disposed,
        /// <summary>
        /// 没有找到可用的文件流
        /// </summary>
        NotFoundFileStream,
        /// <summary>
        /// 文件流已经存在
        /// </summary>
        FileStreamExists,
        /// <summary>
        /// 快照文件建立错误
        /// </summary>
        SnapshotFileStartError,
        /// <summary>
        /// 缓存处于不可写状态
        /// </summary>
        CanNotWrite,
        /// <summary>
        /// 缓存从服务不可写
        /// </summary>
        SlaveCanNotWrite,
        /// <summary>
        /// 没有找到缓存管理
        /// </summary>
        NotFoundSlaveCache,
    }
}