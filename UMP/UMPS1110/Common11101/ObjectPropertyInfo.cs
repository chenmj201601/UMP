//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3e649e66-0f25-42e5-82cf-d81d58591cba
//        CLR Version:              4.0.30319.18444
//        Name:                     ObjectParameter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                Common11101
//        File Name:                ObjectParameter
//
//        created by Charley at 2014/12/19 10:28:59
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11101
{
    /// <summary>
    /// 资源属性信息
    /// </summary>
    public class ObjectPropertyInfo
    {
        /// <summary>
        /// 资源类型ID
        /// </summary>
        public int ObjType { get; set; }
        /// <summary>
        /// 属性编号
        /// 特定的编号：
        /// 1       Key（全系统下的唯一序号，即ObjectID的尾数）
        /// 2       ID（同一父资源下同类型资源的唯一序号）
        /// 3       Parent ObjectID（父资源的ObjectID）
        /// 4       ModuleNumber
        /// 5       MasterSlaver（Master：1；Slaver：2）
        /// 6       EnableDisable（Enable：1；Disable：0）
        /// 7       HostIP
        /// 8       HostName
        /// 9       HostPort
        /// 10      Machine ObjectID（所在机器的ObjectID）
        /// .....
        /// 901 ~ 999 保留（也是特定属性）
        /// 901     州代码
        /// 902     国家代码
        /// 911     认证的UserName
        /// 912     认证的Password
        /// 921     Key（xml文件里的Key，由其他属性计算得来）
        /// 922     ObjID（xml文件里的ObjID，值与ConfigObject的ObjectID相同）
        /// </summary>
        public int PropertyID { get; set; }
        /// <summary>
        /// 数据类型
        /// 2   数值
        /// 14  文本
        /// </summary>
        public ObjectPropertyDataType DataType { get; set; }
        /// <summary>
        /// 格式类型，指定在配置视图中以怎样的形式显示
        /// </summary>
        public ObjectPropertyConvertFormat ConvertFormat { get; set; }
        /// <summary>
        /// 所在组编号
        /// </summary>
        public int GroupID { get; set; }
        /// <summary>
        /// 组内排列序号
        /// </summary>
        public int SortID { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 是否是配置参数，只有配置参数才在界面上显示
        /// </summary>
        public bool IsParam { get; set; }
        /// <summary>
        /// 枚举时BasicInfo中的ID
        /// </summary>
        public int SourceID { get; set; }
        /// <summary>
        /// 参数最小值
        /// </summary>
        public string MinValue { get; set; }
        /// <summary>
        /// 参数最大值
        /// </summary>
        public string MaxValue { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 属性值的加密模式
        /// </summary>
        public ObjectPropertyEncryptMode EncryptMode { get; set; }
        /// <summary>
        /// 属性未开放，（部分属性虽然在界面上显示，但由于未开放，暂时不能配置)
        /// </summary>
        public bool IsLocked { get; set; }
        /// <summary>
        /// 属性是否被其他属性控制
        /// 0   仅被属性6（启用禁用）控制
        /// -1  不被启用禁用控制
        /// >0  被控制的属性的编号
        /// </summary>
        public int ControledPropID { get; set; }
        /// <summary>
        /// 复合值类型
        /// 0   单值
        /// 1   以char30char30char30开头char27分割的多值，其中第一个值是实际值，第二个值通常为配置界面上显示值
        /// ...
        /// </summary>
        public int MultiValueMode { get; set; }
        /// <summary>
        /// 是否关键属性，关键属性在配置界面上用红色*标注
        /// </summary>
        public bool IsKeyProperty { get; set; }
        /// <summary>
        /// Xml文件中属性的名称
        /// </summary>
        public string AttributeName { get; set; }
        /// <summary>
        /// 认证字段类型，认证字段需要包裹在Authention节点中
        /// 0   非认证字段
        /// 1   UserName
        /// 2   Password
        /// </summary>
        public int AuthField { get; set; }
        /// <summary>
        /// 批量修改，某些属性允许批量修改，如通道的大多数属性
        /// 0   不能批量修改
        /// 1   允许批量修改
        /// </summary>
        public int BatchModify { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", ObjType, PropertyID, Description);
        }
    }
}
