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

namespace DataIniXmlFile
{


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
