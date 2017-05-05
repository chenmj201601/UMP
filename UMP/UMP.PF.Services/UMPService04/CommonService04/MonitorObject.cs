//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    019b665f-ecfe-4e89-a711-e43e8669547d
//        CLR Version:              4.0.30319.18063
//        Name:                     MonitorObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService04
//        File Name:                MonitorObject
//
//        created by Charley at 2015/6/26 9:32:42
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService04
{
    /// <summary>
    /// 监视对象
    /// </summary>
    public class MonitorObject
    {
        /// <summary>
        /// 监视方式
        /// </summary>
        public MonitorType MonType { get; set; }
        /// <summary>
        /// 监控唯一标识，一个GUID
        /// </summary>
        public string MonID { get; set; }
        /// <summary>
        /// 对象的资源编码
        /// </summary>
        public long ObjID { get; set; }
        /// <summary>
        /// 对象类型
        /// </summary>
        public int ObjType { get; set; }
        /// <summary>
        /// 对象角色
        /// 0：无
        /// 1：录音通道
        /// 2：录屏通道
        /// </summary>
        public int Role { get; set; }
        /// <summary>
        /// 对象的值（可以是通道ID，分机号，坐席ID等）
        /// </summary>
        public string ObjValue { get; set; }
        /// <summary>
        /// 所属通道资源编码
        /// </summary>
        public long ChanObjID { get; set; }
        /// <summary>
        /// 保留（ChanID，通道号）
        /// </summary>
        public string Other01 { get; set; }
        /// <summary>
        /// 保留(ServerID,所在服务器的ID）
        /// </summary>
        public string Other02 { get; set; }
        /// <summary>
        /// 保留（ServerAddress，所在服务器地址）
        /// </summary>
        public string Other03 { get; set; }
        /// <summary>
        /// 保留（ServerObjID，服务器的资源编码）
        /// </summary>
        public string Other04 { get; set; }
        /// <summary>
        /// 保留
        /// </summary>
        public string Other05 { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}][{3}][{4}][{5}][{6}]",
                MonType,
                MonID,
                ObjID,
                ObjType,
                Role,
                ChanObjID,
                ObjValue);
        }
        /// <summary>
        /// 更新信息
        /// </summary>
        /// <param name="monObj"></param>
        public void UpdateInfo(MonitorObject monObj)
        {
            ChanObjID = monObj.ChanObjID;
            ObjValue = monObj.ObjValue;
            Other01 = monObj.Other01;
            Other02 = monObj.Other02;
            Other03 = monObj.Other03;
            Other04 = monObj.Other04;
            Other05 = monObj.Other05;
        }
    }
}
