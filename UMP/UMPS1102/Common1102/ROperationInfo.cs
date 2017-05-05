

namespace VoiceCyber.UMP.Common11021
{
    public class ROperationInfo
    {
        public long ID { get; set; }
        public string Display { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public long ParentID { get; set; }

        public string IsCanUse { get; set; }
        public string IsCanDownAssign { get; set; }
        public string IsCanCascadeRecycle { get; set; }
        public string IsHide { get; set; }
    }
}
