//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8457704a-4b61-4a03-a44d-754d813cec52
//        CLR Version:              4.0.30319.18444
//        Name:                     GenerateOption
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ResourceXmls
//        File Name:                GenerateOption
//
//        created by Charley at 2015/3/14 17:16:37
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.ResourceXmls
{
    /// <summary>
    /// 生成Xml的选项
    /// 通过此选项或选项的组合，可以根据需要生成资源的属性，组，子资源等
    /// 这是一个Flags类型的枚举，枚举值可以自由组合
    /// </summary>
    [Flags]
    public enum GenerateOption
    {
        /// <summary>
        /// 生成直接属性
        /// </summary>
        Property = 1,
        /// <summary>
        /// 生成组节点及组下属性
        /// </summary>
        Group = 2,
        /// <summary>
        /// 生成子对象节点
        /// </summary>
        Children = 4,
        /// <summary>
        /// 生成认证节点及认证属性
        /// </summary>
        Authention = 8,
        /// <summary>
        /// 忽略下级通道
        /// </summary>
        IgnoreChannel = 16,
        /// <summary>
        /// 默认属性，组，子对象，认证都生成
        /// </summary>
        Default = Property | Group | Children | Authention
    }
}
