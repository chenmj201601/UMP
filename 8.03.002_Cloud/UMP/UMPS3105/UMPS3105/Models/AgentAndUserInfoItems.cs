using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Common3105;

namespace UMPS3105.Models
{
    public class AgentAndUserInfoItems : INotifyPropertyChanged
    {
        public long ID { set; get; }
        public string Name { set; get; }
        public string FullName { set; get; }
        public long OrgID { set; get; } 
        public AgentAndUserInfoList AuInfoList { set; get; }

        public AgentAndUserInfoItems()
        {

        }

        public AgentAndUserInfoItems(AgentAndUserInfoList AuList)
        {
            ID = AuList.ID;
            Name = AuList.Name;
            FullName = AuList.FullName;
            OrgID = AuList.OrgID;
            AuInfoList = AuList;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnProeprtyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
