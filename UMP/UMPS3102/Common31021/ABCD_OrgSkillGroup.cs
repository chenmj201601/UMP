using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31021
{
    public class ABCD_OrgSkillGroup
    {
        /// <summary>
        /// 组织机构或技能组的ID 从T_31_052 的 C003
        /// </summary>
        public long OrgSkillGroupID { get; set; }
        /// <summary>
        /// 参数大项ID 从T_31_051 的 C002
        /// </summary>
        public long ParamID { get; set; }
        /// <summary>
        /// 在 T_31_054 的多少列  从T_31_052 的 C008
        /// </summary>
        public int InColumn { get; set; }
        /// <summary>
        /// 组织机构或技能组的名字
        /// </summary>
        public string OrgSkillGroupName { get; set; }
    }
}
