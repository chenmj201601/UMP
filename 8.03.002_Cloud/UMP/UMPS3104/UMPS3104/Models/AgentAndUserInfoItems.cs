using System.ComponentModel;
using VoiceCyber.UMP.Common31041;

namespace UMPS3104.Models
{
    public class AgentAndUserInfoItems : INotifyPropertyChanged
    {
        public long ID { set; get; }
        public string Name { set; get; }
        public string FullName { set; get; }
        public AgentAndUserInfoList AuInfoList { set; get; }

        public AgentAndUserInfoItems()
        {
            
        }

        public AgentAndUserInfoItems(AgentAndUserInfoList AuList)
        {
            ID = AuList.ID;
            Name = AuList.Name;
            FullName = AuList.FullName;
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
