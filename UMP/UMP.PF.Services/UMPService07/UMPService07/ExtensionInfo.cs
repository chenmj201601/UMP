//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5bdac5b3-7ee8-4596-9edb-e6af3c76df19
//        CLR Version:              4.0.30319.18063
//        Name:                     ExtensionInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService07
//        File Name:                ExtensionInfo
//
//        created by Charley at 2015/11/24 14:00:05
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService07
{
    public class ExtensionInfo : ResourceConfigInfo
    {
        public long OrgID { get; set; }
        /// <summary>
        /// 状态
        /// 0：删除
        /// 1：正常
        /// 2：禁用
        /// </summary>
        public string Status { get; set; }
        public bool IsNew { get; set; }
        public bool IsLock { get; set; }
        public string LockMethod { get; set; }
        /// <summary>
        /// 来源类型
        /// 00：手动输入
        /// 21：录音表导入
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// Ext+ServerIP1+Role+ServerIP2
        /// 其中后面三个是可选的
        /// 如果Role为3，前一个是录音IP，后一个是录屏IP
        /// 两个IP可以一样，也可以不一样
        /// char27隔开
        /// </summary>
        public string Name { get; set; }
        public string Extension { get; set; }
        public string ChanName { get; set; }
        /// <summary>
        /// 角色，按位（1bit：录音分机；2bit：录屏分机）
        /// </summary>
        public int Role { get; set; }
        
        public long VoiceObjID { get; set; }
        public int VoiceChanID { get; set; }
        public long VoiceChanObjID { get; set; }
        public long ScreenObjID { get; set; }
        public int ScreenChanID { get; set; }
        public long ScreenChanObjID { get; set; }

        public string VoiceIP { get; set; }
        public string ScreenIP { get; set; }

        public override string LogInfo()
        {
            string strInfo = base.LogInfo();
            strInfo =
                string.Format(
                    "{0};Name:{1};ChanName:{2};OrgID:{3};Status:{4};VoiceObjID:{5};VoiceChanID:{6};ScreenObjID:{7};ScreenChanID:{8};VoiceIP:{9};ScreenIP:{10}",
                    strInfo,
                    Name,
                    ChanName,
                    OrgID,
                    Status,
                    VoiceObjID,
                    VoiceChanID,
                    ScreenObjID,
                    ScreenChanID,
                    VoiceIP,
                    ScreenIP);
            return strInfo;
        }
    }
}
