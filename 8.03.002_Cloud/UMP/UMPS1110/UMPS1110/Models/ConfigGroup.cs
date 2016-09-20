//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e869f1ff-f3fc-42eb-9c30-139663a929cb
//        CLR Version:              4.0.30319.18444
//        Name:                     ConfigGroup
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                ConfigGroup
//
//        created by Charley at 2015/1/12 11:18:42
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models
{
    /// <summary>
    /// 属性组
    /// </summary>
    public class ConfigGroup
    {
        public ConfigObject ConfigObject { get; set; }
        public int TypeID { get; set; }
        public int GroupID { get; set; }
        public int ParentGroupID { get; set; }
        public ResourceGroupType GroupType { get; set; }
        public int ChildType { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ObjectItem ObjectItem { get; set; }
        public ResourceGroupParam GroupInfo { get; set; }

        public void RefreshData()
        {
            if (ObjectItem != null)
            {
                ObjectItem.Name = Name;
                ObjectItem.Description = Description;
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", TypeID, GroupID, Name);
        }
    }
}
