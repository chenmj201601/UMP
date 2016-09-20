using VoiceCyber.Wpf.CustomControls;

namespace UMPS3105.Models
{
    public class CtrolAgent : CheckableItemBase
    {
        public long ObjID { get; set; }
        public string Name { get; set; }
        public int ObjType { get; set; }
        public string Description { get; set; }
        public object Data { get; set; }
    }
}
