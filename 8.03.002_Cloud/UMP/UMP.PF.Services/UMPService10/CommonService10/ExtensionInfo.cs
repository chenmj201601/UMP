//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a4a8bc02-d070-4e59-8928-d66a573b148b
//        CLR Version:              4.0.30319.18408
//        Name:                     ExtensionInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService10
//        File Name:                ExtensionInfo
//
//        created by Charley at 2016/6/28 18:04:28
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;


namespace VoiceCyber.UMP.CommonService10
{
    /// <summary>
    /// 分机信息（虚拟分机或真实分机）,是被监视的对象
    /// </summary>
    public class ExtensionInfo
    {
        public const int RESOURCE_EXTENSION = 104;
        public const int RESOURCE_REALEXT = 105;

        public const int PRO_ORG = 1;
        public const int PRO_STATE = 2;
        public const int PRO_EXTENSION = 7;

        public long ObjID { get; set; }
        public int ObjType { get; set; }
        public long ParentObjID { get; set; }
        public string Extension { get; set; }


        #region 呼叫信息

        public string CallerID { get; set; }
        public string CalledID { get; set; }
        public string AgentID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public int TalkLength { get; set; }
        public double TalkTimeDeviation { get; set; }

        #endregion


        #region 录音信息

        public string VocRecordReference { get; set; }
        public DateTime VocStartRecordTime { get; set; }
        public DateTime VocStopRecordTime { get; set; }
        public int VocRecordLength { get; set; }
        public double VocTimeDeviation { get; set; }

        #endregion


        #region 录屏信息

        public string ScrRecordReference { get; set; }
        public DateTime ScrStartRecordTime { get; set; }
        public DateTime ScrStopRecordTime { get; set; }
        public int ScrRecordLength { get; set; }
        public double ScrTimeDiviation { get; set; }

        #endregion


        #region 状态信息

        private List<ExtStateInfo> mListStateInfos = new List<ExtStateInfo>();

        public List<ExtStateInfo> ListStateInfos
        {
            get { return mListStateInfos; }
        }

        #endregion


        #region 其他信息

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
        /// <summary>
        /// 保留
        /// </summary>
        public string Other06 { get; set; }
        /// <summary>
        /// 保留
        /// </summary>
        public string Other07 { get; set; }
        /// <summary>
        /// 保留
        /// </summary>
        public string Other08 { get; set; }
        /// <summary>
        /// 保留
        /// </summary>
        public string Other09 { get; set; }
        /// <summary>
        /// 保留
        /// </summary>
        public string Other10 { get; set; }

        #endregion


        public string LogInfo()
        {
            string strInfo = string.Format("ObjID:{0};ObjType:{1};Parent:{2};Extension:{3};",
               ObjID,
               ObjType,
               ParentObjID,
               Extension);
            return strInfo;
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", ObjID, ObjType, Extension);
        }
    }
}
