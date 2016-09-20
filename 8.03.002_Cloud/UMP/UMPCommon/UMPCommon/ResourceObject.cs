//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e18e3475-0ba5-4db0-9068-f1d1a5e7c100
//        CLR Version:              4.0.30319.18063
//        Name:                     ResourceObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                ResourceObject
//
//        created by Charley at 2015/7/8 17:49:01
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /*
     * 各类资源类型名称及保留字段的定义
     * 机构：
     *      Name：机构全名
     * 用户：
     *      Name：帐号
     *      FullName：全名
     * 分机：
     *      Name：分机号
     *      FullName：通道名
     *      01：服务器编码
     *      02：通道号
     *      03：通道编码
     *      04：服务器地址
     *      05：角色（0：无角色，1：录音分机；2：录屏分机；3：录音录屏分机）
     *      ***
     *          注：当Other05的值为 3 时，Other01，Other02，Other03，Other04的值都是分号分割的，前面代表录音的，后面代表录屏的
     *      ***
     * 坐席：
     *      Name：坐席工号
     *      FullName：坐席名
     */
    /// <summary>
    /// 通用资源对象，可以是机构，用户，分机，坐席......
    /// </summary>
    [DataContract]
    public class ResourceObject
    {
        /// <summary>
        /// 资源编号
        /// </summary>
        [DataMember]
        public long ObjID { get; set; }
        /// <summary>
        /// 资源类型
        /// </summary>
        [DataMember]
        public int ObjType { get; set; }
        /// <summary>
        /// 资源名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 全名/描述
        /// </summary>
        [DataMember]
        public string FullName { get; set; }
        /// <summary>
        /// 对象状态(按位）
        /// 0：正常
        /// 1：删除
        /// 2：禁用
        /// </summary>
        [DataMember]
        public int State { get; set; }
        /// <summary>
        /// 所在部门编码
        /// </summary>
        [DataMember]
        public long OrgObjID { get; set; }
        /// <summary>
        /// 上级资源编码
        /// </summary>
        [DataMember]
        public long ParentObjID { get; set; }
        /// <summary>
        /// 其他信息1
        /// </summary>
        [DataMember]
        public string Other01 { get; set; }
        /// <summary>
        /// 其他信息2
        /// </summary>
        [DataMember]
        public string Other02 { get; set; }
        /// <summary>
        /// 其他信息3
        /// </summary>
        [DataMember]
        public string Other03 { get; set; }
        /// <summary>
        /// 其他信息4
        /// </summary>
        [DataMember]
        public string Other04 { get; set; }
        /// <summary>
        /// 其他信息5
        /// </summary>
        [DataMember]
        public string Other05 { get; set; }
        /// <summary>
        /// 其他信息6
        /// </summary>
        [DataMember]
        public string Other06 { get; set; }
        /// <summary>
        /// 其他信息7
        /// </summary>
        [DataMember]
        public string Other07 { get; set; }
        /// <summary>
        /// 其他信息8
        /// </summary>
        [DataMember]
        public string Other08 { get; set; }
        /// <summary>
        /// 其他信息9
        /// </summary>
        [DataMember]
        public string Other09 { get; set; }
        /// <summary>
        /// 其他信息10
        /// </summary>
        [DataMember]
        public string Other10 { get; set; }
        /// <summary>
        /// 其他信息11
        /// </summary>
        [DataMember]
        public string Other11 { get; set; }
        /// <summary>
        /// 其他信息12
        /// </summary>
        [DataMember]
        public string Other12 { get; set; }
        /// <summary>
        /// 其他信息13
        /// </summary>
        [DataMember]
        public string Other13 { get; set; }
        /// <summary>
        /// 其他信息14
        /// </summary>
        [DataMember]
        public string Other14 { get; set; }
        /// <summary>
        /// 其他信息15
        /// </summary>
        [DataMember]
        public string Other15 { get; set; }
        /// <summary>
        /// 其他信息16
        /// </summary>
        [DataMember]
        public string Other16 { get; set; }
        /// <summary>
        /// 其他信息17
        /// </summary>
        [DataMember]
        public string Other17 { get; set; }
        /// <summary>
        /// 其他信息18
        /// </summary>
        [DataMember]
        public string Other18 { get; set; }
        /// <summary>
        /// 其他信息19
        /// </summary>
        [DataMember]
        public string Other19 { get; set; }
        /// <summary>
        /// 其他信息20
        /// </summary>
        [DataMember]
        public string Other20 { get; set; }
    }
}
