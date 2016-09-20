//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cf183f46-ccc1-412e-835b-2ae0c415fb14
//        CLR Version:              4.0.30319.18408
//        Name:                     MonitorObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService10
//        File Name:                MonitorObject
//
//        created by Charley at 2016/6/27 17:36:17
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService10
{
    /// <summary>
    /// 监视对象
    /// </summary>
    public class MonitorObject
    {
        /// <summary>
        /// 监视ID，当对象加入监视队列后会生成唯一的ID（GUID）
        /// </summary>
        public string MonID { get; set; }
        /// <summary>
        /// 监视对象的编码
        /// </summary>
        public long ObjID { get; set; }
        /// <summary>
        /// 监视对象的类型（分机，坐席，真实分机，通道等）
        /// </summary>
        public int ObjType { get; set; }
        /// <summary>
        /// 监视对象名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 保留
        /// </summary>
        public string Other01 { get; set; }
        /// <summary>
        /// 保留
        /// </summary>
        public string Other02 { get; set; }
        /// <summary>
        /// 保留
        /// </summary>
        public string Other03 { get; set; }
        /// <summary>
        /// 保留
        /// </summary>
        public string Other04 { get; set; }
        /// <summary>
        /// 保留
        /// </summary>
        public string Other05 { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}][{3}]",
               MonID,
               ObjID,
               ObjType,
               Name);
        }
    }
}
