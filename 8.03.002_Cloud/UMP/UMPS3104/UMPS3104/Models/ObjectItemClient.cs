using System;
using VoiceCyber.Wpf.CustomControls;


namespace UMPS3104.Models
{
    public class ObjectItemClient: CheckableItemBase
    {
        public long ObjID { get; set; }
        public string Name { get; set; }
        public int ObjType { get; set; }
        public string Description { get; set; }
        public object Data { get; set; }
    }
}
