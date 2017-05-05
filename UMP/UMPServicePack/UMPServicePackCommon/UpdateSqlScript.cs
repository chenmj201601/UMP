//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    aed43298-eaeb-4dcd-822d-af739bfc65d8
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateSqlScript
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UpdateSqlScript
//
//        created by Charley at 2016/5/9 10:16:29
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;


namespace UMPServicePackCommon
{
    /// <summary>
    /// Sql脚本信息
    /// </summary>
    [XmlRoot(ElementName = "UpdateSqlScript", Namespace = "http://www.voicecyber.com/UMP/Updates/2016/05")]
    public class UpdateSqlScript : NodeItem
    {
        /// <summary>
        /// Sql类型（DDL/DML/脚本文件）
        /// </summary>
        [XmlAttribute]
        public int SqlType { get; set; }
        /// <summary>
        /// 脚本种类（select/insert/update等）
        /// </summary>
        [XmlAttribute]
        public int ScriptType { get; set; }

        private List<UpdateScript> mListScripts = new List<UpdateScript>();
        /// <summary>
        /// 脚本列表（不同数据库类型对应不同的脚本）
        /// </summary>
        [XmlArray(ElementName = "Scripts")]
        [XmlArrayItem(ElementName = "Script")]
        public List<UpdateScript> ListScripts
        {
            get { return mListScripts; }
        }
    }
}
