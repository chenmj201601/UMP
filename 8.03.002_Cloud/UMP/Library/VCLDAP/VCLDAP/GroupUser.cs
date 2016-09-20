using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.VCLDAP
{
    public class GroupUser
    {
        public GroupUser(string disName, string name)
        {
            this.DisplayName = disName;
            this.Name = name;

        }
        private string _DisplayName;
        public string DisplayName
        {
            get
            {
                return _DisplayName;
            }
            set
            {
                _DisplayName = value;
            }
        }
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
    }
}
