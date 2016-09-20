using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31021
{
    public class SkillGroupInfo
    {
        /// <summary>
        /// 技能组ID 19位的
        /// </summary>
        public string SkillGroupID { get; set; }
        /// <summary>
        /// 技能组编码,T_21_001的C107字段
        /// </summary>
        public string SkillGroupCode { get; set; }
        /// <summary>
        /// 技能组名称
        /// </summary>
        public string SkillGroupName{ get; set; }

    }
}
