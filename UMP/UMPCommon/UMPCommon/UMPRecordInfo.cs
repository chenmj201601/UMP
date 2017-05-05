//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    93a52a4e-12be-41a0-a76a-32e283539933
//        CLR Version:              4.0.30319.18063
//        Name:                     UMPRecordInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                UMPRecordInfo
//
//        created by Charley at 2015/9/7 9:30:45
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 录音（录屏）记录信息
    /// </summary>
    [DataContract]
    public class UMPRecordInfo
    {

        #region 基本字段

        /// <summary>
        /// 行号（C001）
        /// </summary>
        [DataMember]
        public long RowID { get; set; }
        /// <summary>
        /// 流水号（C002）
        /// </summary>
        [DataMember]
        public string SerialID { get; set; }
        /// <summary>
        /// 记录流水号
        /// </summary>
        [DataMember]
        public string RecordReference { get; set; }
        /// <summary>
        /// 录音（录屏）开始时间（UTC时间，C005）
        /// </summary>
        [DataMember]
        public DateTime StartRecordTime { get; set; }
        /// <summary>
        /// 录音（录屏）结束时间（UTC时间，C009）
        /// </summary>
        [DataMember]
        public DateTime StopRecordTime { get; set; }
        /// <summary>
        /// 分机号（C042）
        /// </summary>
        [DataMember]
        public string Extension { get; set; }
        /// <summary>
        /// 坐席（工号）（C039）
        /// </summary>
        [DataMember]
        public string Agent { get; set; }
        /// <summary>
        /// 服务器ID（C037）
        /// </summary>
        [DataMember]
        public int ServerID { get; set; }
        /// <summary>
        /// 服务器IP（C020）
        /// </summary>
        [DataMember]
        public string ServerIP { get; set; }
        /// <summary>
        /// 通道号（C038）
        /// </summary>
        [DataMember]
        public int ChannelID { get; set; }
        /// <summary>
        /// 媒体类型（0：录音（带录屏）；1：录音；2：录屏）（C014）
        /// </summary>
        [DataMember]
        public int MediaType { get; set; }
        /// <summary>
        /// 加密标识（C025）
        /// 0：未加密（无需加密）
        /// 2：二代加密
        /// E：待加密
        /// F：加密失败
        /// </summary>
        [DataMember]
        public string EncryptFlag { get; set; }
        /// <summary>
        /// 以字符串形式表示的记录信息，可以是Json格式，Xml格式等
        /// 字段名参考UMPRecordInfo的静态定义
        /// 如：{"RowID"=1,"SerialID"="1509021000000000001"...}
        /// </summary>
        [DataMember]
        public string StringInfo { get; set; }

        #endregion


        #region 保留字段

        /// <summary>
        /// 其他信息
        /// </summary>
        [DataMember]
        public string Other01 { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        [DataMember]
        public string Other02 { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        [DataMember]
        public string Other03 { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        [DataMember]
        public string Other04 { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        [DataMember]
        public string Other05 { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        [DataMember]
        public string Other06 { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        [DataMember]
        public string Other07 { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        [DataMember]
        public string Other08 { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        [DataMember]
        public string Other09 { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        [DataMember]
        public string Other10 { get; set; }

        #endregion


        #region 字段定义

        public const string PRO_ROWID = "RowID";
        public const string PRO_SERIALID = "SerialID";
        public const string PRO_RECORDREFERENCE = "RecordReference";
        public const string PRO_STARTRECORDTIME = "StartRecordTime";
        public const string PRO_STOPRECORDTIME = "StopRecordTime";
        public const string PRO_EXTENSION = "Extension";
        public const string PRO_AGENT = "Agent";
        public const string PRO_SERVERID = "ServerID";
        public const string PRO_SERVERIP = "ServerIP";
        public const string PRO_CHANNELID = "ChannelID";
        public const string PRO_MEDIATYPE = "MediaType";
        public const string PRO_ENCRYPTFLAG = "EncryptFlag";

        public const string PRO_STRINGINFO = "StringInfo";

        public const string PRO_CALLERID = "CallerID";
        public const string PRO_CALLEDID = "CalledID";
        public const string PRO_DIRECTION = "Direction";
        public const string PRO_WAVEFORMAT = "WaveFormat";

        #endregion

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", SerialID, StartRecordTime, MediaType);
        }
    }
}
