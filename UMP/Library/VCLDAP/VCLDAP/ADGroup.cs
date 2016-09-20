using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;

namespace VoiceCyber.VCLDAP
{
    public class ADGroup : ADObject
    {
         public ADGroup(string groupName)
        {
            this.DisplayName = groupName;
        }

        public ADGroup(DirectoryEntry directoryEntry)
        {
            this.MyDirectoryEntry = directoryEntry;
        }


        /**/
        /// <summary>
        /// 中文显示名
        /// </summary>
        public string DisplayName
        {
            get
            {
                return GetProperty("DisplayName").ToString();
            }
            set
            {
                SetProperty("DisplayName", value);
            }
        }
        /**/
        /// <summary>        
        /// </summary>
        public string Name
        {
            get
            {
                return GetProperty("Name").ToString();
            }
            set
            {
                SetProperty("Name", value);
            }
        }
    }
}
