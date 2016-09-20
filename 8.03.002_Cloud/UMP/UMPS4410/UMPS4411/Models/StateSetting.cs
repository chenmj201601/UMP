//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6c273214-cfb2-4d1b-819b-24a6dcba7a9b
//        CLR Version:              4.0.30319.18408
//        Name:                     StateSetting
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411.Models
//        File Name:                StateSetting
//
//        created by Charley at 2016/6/30 16:15:15
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;


namespace UMPS4411.Models
{
    [XmlRoot]
    public class StateSetting
    {
        public const string FILE_NAME = "StateSetting.xml";

        [XmlAttribute]
        public string Name { get; set; }

        private List<StateSettingItem> mListItemBorderStates = new List<StateSettingItem>();
        [XmlArray(ElementName = "ItemBorder")]
        [XmlArrayItem(ElementName = "State")]
        public List<StateSettingItem> ListItemBorderStates
        {
            get { return mListItemBorderStates; }
        }

        private List<StateSettingItem> mListHeadBackgroundStates = new List<StateSettingItem>();

        [XmlArray(ElementName = "HeadBackground")]
        [XmlArrayItem(ElementName = "State")]
        public List<StateSettingItem> ListHeadBackgroundStates
        {
            get { return mListHeadBackgroundStates; }
        }

        private List<StateSettingItem> mListContentBackgroundStates = new List<StateSettingItem>();

        [XmlArray(ElementName = "ContentBackground")]
        [XmlArrayItem(ElementName = "State")]
        public List<StateSettingItem> ListContentBackgroundStates
        {
            get { return mListContentBackgroundStates; }
        }

        private List<StateSettingItem> mListLabelMainStates = new List<StateSettingItem>();

        [XmlArray(ElementName = "LabelMain")]
        [XmlArrayItem(ElementName = "State")]
        public List<StateSettingItem> ListLabelMainStates
        {
            get { return mListLabelMainStates; }
        }

        private List<StateSettingItem> mListLabelSubStates = new List<StateSettingItem>();

        [XmlArray(ElementName = "LabelSub")]
        [XmlArrayItem(ElementName = "State")]
        public List<StateSettingItem> ListLabelSubStates
        {
            get { return mListLabelSubStates; }
        }
    }
}
