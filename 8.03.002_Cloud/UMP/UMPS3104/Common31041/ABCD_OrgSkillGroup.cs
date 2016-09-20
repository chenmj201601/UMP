using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31041
{
    public class ABCD_OrgSkillGroup
    {
        /// <summary>
        /// 组织机构或技能组的ID (T_31_052.C003)
        /// </summary>
        public long OrgSkillGroupID { get; set; }
        /// <summary>
        /// 参数大项ID (T_31_051.C002)
        /// </summary>
        public long ParamID { get; set; }
        /// <summary>
        /// 在 T_31_054 的多少列  (T_31_052.C008)
        /// </summary>
        public int InColumn { get; set; }
        /// <summary>
        /// 组织机构或技能组的名字
        /// </summary>
        public string OrgSkillGroupName { get; set; }
        /// <summary>
        /// 机构下属的座席号（T21.C039)
        /// </summary>
        public string ManageAgent { get; set; }
    }
}
