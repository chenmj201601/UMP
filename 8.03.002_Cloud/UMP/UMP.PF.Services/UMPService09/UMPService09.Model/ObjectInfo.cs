using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService09.Model
{

    //技能组就直接用它了
     public class ObjectInfo
    {
        public long ObjID { set; get; }
        public String ObjName { set; get; }

        public int ObjType { set; get; } // 1座席 2分机  3用户 4真实分机 5机构  6 技能组

         //机构专用
        public long ParentOrgID { set; get; }

         //座席和分机和用户专用
        public long BeyondOrgID { set; get; }
        public long BeyondSkillID { set; get; }


         //分机专用
        public string ExtensionIP { set; get; }
    }
}
