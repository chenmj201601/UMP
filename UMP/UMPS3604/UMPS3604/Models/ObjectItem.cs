using VoiceCyber.Wpf.CustomControls;

namespace UMPS3604.Models
{
    public class ObjectItem : CheckableItemBase
    {
        public long ObjID { get; set; }
        public string Name { get; set; }
        public int ObjType { get; set; }
        public string Description { get; set; }
        public object Data { get; set; }
    }
}
