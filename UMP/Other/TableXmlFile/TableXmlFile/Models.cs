//======================================================================
//
//        Copyright (C) 2013 仙剑
//        All rights reserved
//        guid1:                    b0179056-c0c7-41fe-af4b-5f9e2692e98e
//        CLR Version:              4.0.30319.18063
//        Name:                     Models
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VCLogRecordGenerator
//        File Name:                Models
//
//        created by Charley at 2014/3/13 16:05:26
//        http://blog.csdn.net/jian200801 
//
//======================================================================
using System;
using System.Collections.Generic;

namespace FunctionTree
{
    public class IMPRecordDataInfo
    {
        public string RecordReference { get; set; }
        public string VoiceIP { get; set; }
        public string RootDisk { get; set; }
        public int VoiceID { get; set; }
        public int Channel { get; set; }
        public DateTime StartRecordTime { get; set; }
        public DateTime StopRecordTime { get; set; }
        public int RecordLength { get; set; }
        public string AgentID { get; set; }
        public string CallerID { get; set; }
        public string CalledID { get; set; }
        public string Extension { get; set; }
        public string DirectionFlag { get; set; }
        public string CallerDTMF { get; set; }
        public string CalledDTMF { get; set; }
        public int FileFormat { get; set; }
        public string EncryFlag { get; set; }
        public DateTime InsertTime { get; set; }
        public int BackupCount { get; set; }
        public string DeleteFlag { get; set; }
        public DateTime DeleteTime { get; set; }
        public string DeleteIdentify { get; set; }
        public string ReservedOne { get; set; }
        public string ReservedTwo { get; set; }
        public string ReservedThree { get; set; }
        public string ReservedFour { get; set; }
        public string SynchFlag { get; set; }

        public int VoiceSite { get; set; }
        public int MediaType { get; set; }
        public string CTISerialID { get; set; }
        public string ChannelName { get; set; }
        public string TransferFlag { get; set; }
        public string TransferFrom { get; set; }
        public string TransferTo { get; set; }
        public string ConferenceFlag { get; set; }
        public string ConferenceFrom { get; set; }
        public string ConferenceTo { get; set; }
        public string OtherParticipate { get; set; }
        public int CRCCheck { get; set; }
        public string OtherCallID { get; set; }
        public string RealExtension { get; set; }
        public int HoldTimes { get; set; }
        public int HoldLength { get; set; }
        public int RingLength { get; set; }
        public int AgentTalkLength { get; set; }
        public int UserTalkLength { get; set; }
        public int MuteLength { get; set; }
        public string FileSavePath { get; set; }
        public int UTCOffset { get; set; }
        public string AreaCodeContinent { get; set; }
        public string AreaCodeCountry { get; set; }
        public int TransferCount { get; set; }
        public int KeyWorkFlag { get; set; }
        public int PossibleDisputationDuration { get; set; }

        public int IMPField00 { get; set; }
        public string IMPField01 { get; set; }
        public string IMPField02 { get; set; }
        public string IMPField03 { get; set; }
        public string IMPField04 { get; set; }
        public string IMPField05 { get; set; }
        public string IMPField09 { get; set; }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4}", RecordReference, StartRecordTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Channel, Extension, AgentID);
        }
    }

    public class VoiceInfo
    {
        public string VoiceIP { get; set; }
        public int VoiceID { get; set; }
        public List<ChannelInfo> ListChannelInfos { get; set; }

        public VoiceInfo()
        {
            ListChannelInfos = new List<ChannelInfo>();
        }
    }

    public class ChannelInfo
    {
        public int ChannelID { get; set; }
        public string Extension { get; set; }
        public string AgentID { get; set; }
    }

    public class CaculateInfo
    {
        public int TotalNum { get; set; }
        public int Successed { get; set; }
        public int Failed { get; set; }
    }

    public class ConfigInfo
    {
        public string DBServer { get; set; }
        public int DBPort { get; set; }
        public string DBName { get; set; }
        public string DBUser { get; set; }
        public string DBPassword { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    internal class PropertyNodeItem
    {
        public string Icon { get; set; }
        public string EditIcon { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public int id { get; set; }  //c002
        public int parentId { get; set; }//c003 
        public bool IsExpanded { get; set; }
        public List<PropertyNodeItem> Children { get; set; }
        public PropertyNodeItem()
        {
            Children = new List<PropertyNodeItem>();
        }
    }
}
