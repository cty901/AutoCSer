﻿using System;
using AutoCSer.Extension;
using System.Reflection;
using AutoCSer.Metadata;
using AutoCSer.CodeGenerator.Metadata;
using System.Collections.Generic;

namespace AutoCSer.CodeGenerator.TemplateGenerator
{
    /// <summary>
    /// TCP 服务代码生成
    /// </summary>
    internal sealed partial class TcpOpenServer : TcpServer
    {
        /// <summary>
        /// TCP 服务代码生成
        /// </summary>
        [Generator(Name = "TCP 开放服务", DependType = typeof(CSharper), IsAuto = true)]
        internal sealed partial class Generator : Generator<AutoCSer.Net.TcpOpenServer.ServerAttribute, AutoCSer.Net.TcpOpenServer.MethodAttribute, AutoCSer.Net.TcpOpenServer.ServerSocketSender>
        {
            /// <summary>
            /// 服务类名称
            /// </summary>
            public string ServerNameOnly
            {
                get
                {
                    return string.IsNullOrEmpty(Attribute.ServerName) ? Type.TypeOnlyName : Attribute.ServerName;
                }
            }
            /// <summary>
            /// 服务类名称
            /// </summary>
            public override string ServerName
            {
                get
                {
                    if (Attribute.ServerName == null) return TypeName;
                    int index = TypeName.IndexOf('<');
                    if (index == -1) return ServerNameOnly;
                    return ServerNameOnly + TypeName.Substring(index);
                }
            }
            /// <summary>
            /// 服务注册名称
            /// </summary>
            public override string ServerRegisterName { get { return Attribute.ServerName ?? Type.FullName; } }
            /// <summary>
            /// 是否默认时间验证服务
            /// </summary>
            public bool IsTimeVerify
            {
                get { return typeof(AutoCSer.Net.TcpOpenServer.TimeVerifyServer).IsAssignableFrom(Type); }
            }
            /// <summary>
            /// 是否生成客户端代码
            /// </summary>
            public bool IsClientCode;
            /// <summary>
            /// 是否生成服务器端代码
            /// </summary>
            public bool IsServerCode;
            /// <summary>
            /// 是否存在 AutoCSer.Net.TcpServer.ISetTcpServer 接口函数
            /// </summary>
            public bool IsSetTcpServer
            {
                get
                {
#if NOJIT
                    return isSetTcpServer
#else
                    return typeof(AutoCSer.Net.TcpServer.ISetTcpServer<AutoCSer.Net.TcpOpenServer.Server>).IsAssignableFrom(Type.Type)
#endif
                        || Type.Type.GetMethod("SetTcpServer", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(AutoCSer.Net.TcpOpenServer.Server) }, null) != null;
                }
            }
            /// <summary>
            /// 安装下一个类型
            /// </summary>
            protected unsafe override void nextCreate()
            {
                if (Type.Type.IsClass && !Type.Type.IsAbstract)
                {
                    LeftArray<TcpMethod> methodArray = new LeftArray<TcpMethod>(Metadata.MethodIndex.GetMethods<AutoCSer.Net.TcpOpenServer.MethodAttribute>(Type, Attribute.GetMemberFilters, false, Attribute.IsAttribute, Attribute.IsBaseTypeAttribute)
                        .getFind(value => !value.Method.IsGenericMethod)
                        .getArray(value => new TcpMethod
                        {
                            Method = value,
                            MethodType = Type,
                            ServiceAttribute = Attribute
                        }));
                    foreach (MemberIndexInfo member in MemberIndexGroup.Get<AutoCSer.Net.TcpOpenServer.MethodAttribute>(Type, Attribute.GetMemberFilters, false, Attribute.IsAttribute, Attribute.IsBaseTypeAttribute))
                    {
                        if (member.IsField)
                        {
                            FieldInfo field = (FieldInfo)member.Member;
                            TcpMethod getMethod = new TcpMethod
                            {
                                Method = new Metadata.MethodIndex(field, true),
                                MemberIndex = member,
                                MethodType = Type,
                                ServiceAttribute = Attribute
                            };
                            if (!getMethod.Attribute.IsOnlyGetMember)
                            {
                                getMethod.SetMethod = new TcpMethod { Method = new MethodIndex(field, false), MemberIndex = member, MethodType = Type, ServiceAttribute = Attribute };
                            }
                            methodArray.Add(getMethod);
                            if (getMethod.SetMethod != null) methodArray.Add(getMethod.SetMethod);
                        }
                        else if (member.CanGet)
                        {
                            PropertyInfo property = (PropertyInfo)member.Member;
                            TcpMethod getMethod = new TcpMethod
                            {
                                Method = new MethodIndex(property, true),
                                MemberIndex = member,
                                MethodType = Type,
                                ServiceAttribute = Attribute
                            };
                            if (member.CanSet && !getMethod.Attribute.IsOnlyGetMember)
                            {
                                getMethod.SetMethod = new TcpMethod { Method = new MethodIndex(property, false), MemberIndex = member, MethodType = Type, ServiceAttribute = Attribute };
                            }
                            methodArray.Add(getMethod);
                            if (getMethod.SetMethod != null) methodArray.Add(getMethod.SetMethod);
                        }
                    }
                    MethodIndexs = methodArray.ToArray();
                    MethodIndexs = TcpMethod.CheckIdentity(MethodIndexs, Attribute.CommandIdentityEnmuType, getRememberIdentityName(Attribute.CommandIdentityEnmuType == null ? Attribute.GenericType ?? Type : null), method => method.Method.MethodKeyFullName);
                    if (MethodIndexs == null) return;
                    int methodIndex = CallQueueCount = 0;
                    IsVerifyMethod = false;
                    IsCallQueueLink = ServiceAttribute.GetRemoteExpressionServerTask == Net.TcpServer.ServerTaskType.QueueLink;
                    if (ServiceAttribute.GetRemoteExpressionServerTask == Net.TcpServer.ServerTaskType.Queue || ServiceAttribute.GetRemoteExpressionServerTask == Net.TcpServer.ServerTaskType.QueueLink)
                    {
                        CallQueueCount = (int)ServiceAttribute.GetRemoteExpressionCallQueueIndex + 1;
                    }
                    ParameterBuilder parameterBuilder = new ParameterBuilder { IsSimpleSerialize = Attribute.IsSimpleSerialize };
                    QueueTypeBuilder queueTypeBuilder = new QueueTypeBuilder();
                    foreach (TcpMethod method in MethodIndexs)
                    {
                        method.MethodIndex = methodIndex++;
                        if (!method.IsNullMethod)
                        {
                            if (IsVerifyMethod) method.Attribute.IsVerifyMethod = false;
                            else if (method.IsVerifyMethod)
                            {
                                IsVerifyMethod = true;
                                IsSynchronousVerifyMethod = method.Attribute.ServerTaskType == AutoCSer.Net.TcpServer.ServerTaskType.Synchronous && !method.IsAsynchronousCallback;
                                //method.Attribute.ServerTaskType = Net.TcpServer.ServerTaskType.Synchronous;
                            }
                            parameterBuilder.Add(method);
                            queueTypeBuilder.Add(method);

                            IsCallQueueLink |= method.Attribute.ServerTaskType == Net.TcpServer.ServerTaskType.QueueLink;
                            if (method.Attribute.ServerTaskType == Net.TcpServer.ServerTaskType.Queue || method.Attribute.ServerTaskType == Net.TcpServer.ServerTaskType.QueueLink)
                            {
                                CallQueueCount = Math.Max((int)method.Attribute.GetServerQueueIndex + 1, CallQueueCount);
                            }
                            MaxTimeoutSeconds = Math.Max(MaxTimeoutSeconds, method.Attribute.GetClientTimeoutSeconds);

                            //if (method.IsAsynchronousCallback && method.Attribute.ServerTaskType != Net.TcpServer.ServerTaskType.Synchronous)
                            //{
                            //    Messages.Message("异步函数警告" + method.MemberFullName);
                            //}
                        }
                    }
                    ParameterTypes = parameterBuilder.Get();
                    ServerCallQueueTypes = queueTypeBuilder.Get();
                    //TcpMethod[] methodIndexs = MethodIndexs.getFindArray(value => !value.IsNullMethod);
                    if (ServiceAttribute.GetIsSegmentation)
                    {
                        IsClientCode = false;
                        create(IsServerCode = true);
                        CSharpTypeDefinition definition = new CSharpTypeDefinition(Type, IsClientCode = true, false, Type.Type.Namespace + ".TcpClient");
                        _code_.Length = 0;
                        _code_.Add(definition.Start);
                        create(IsServerCode = false);
                        _code_.Add(definition.End);
                        string fileName = AutoParameter.ProjectPath + "{" + AutoParameter.DefaultNamespace + "}.TcpOpenServer." + ServerName + ".Client.cs";
                        string clientCode = Coder.WarningCode + _code_.ToString() + Coder.FileEndCode;
                        if (Coder.WriteFile(fileName, clientCode))
                        {
                            if (ServiceAttribute.ClientSegmentationCopyPath != null)
                            {
                                string copyFileName = ServiceAttribute.ClientSegmentationCopyPath + "{" + AutoParameter.DefaultNamespace + "}.TcpOpenServer." + ServerName + ".Client.cs";
                                if (Coder.WriteFile(copyFileName, clientCode)) Messages.Message(copyFileName + " 被修改");
                            }
                            Messages.Message(fileName + " 被修改");
                        }
                    }
                    else create(IsServerCode = IsClientCode = true);
                }
            }
            /// <summary>
            /// 安装完成处理
            /// </summary>
            protected override void onCreated() { }
        }
    }
}
