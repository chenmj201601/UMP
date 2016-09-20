using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common11011
{
    public class DomainInfoOrg
    {
        /// <summary>
        /// 域编号C001
        /// </summary>
        public long DomainID { get; set; }
        /// <summary>
        /// 租户号C002
        /// </summary>
        public long RentID { get; set; }
        /// <summary>
        /// 域名城C003
        /// </summary>
        public string DomainName { get; set; }
        /// <summary>
        /// 域排序编号C004
        /// </summary>
        public int DomainCode { get; set; }
        /// <summary>
        /// 拥有浏览该域权限的用户名C005
        /// </summary>
        public string DomainUserName { get; set; }
        /// <summary>
        /// 拥有浏览该域权限的用户密码C006
        /// </summary>
        public string DomainUserPassWord { get; set; }
        /// <summary>
        /// 浏览根目录C007
        /// </summary>
        public string RootDirectory { get; set; }
        /// <summary>
        /// 是否活动C008
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 是否自动登录C010
        /// </summary>
        public bool IsActiveLogin { get; set; }
        /// <summary>
        /// 是否删除C009
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 创建人C011
        /// </summary>
        public long Creator { get; set; }
        /// <summary>
        /// 创建时间C012
        /// </summary>
        public string CreatTime { get; set; }
        /// <summary>
        /// 备注或描述C099
        /// </summary>
        public string Description { get; set; }

        public override string ToString()
        {
            if (DomainName != null)
                return this.DomainName;
            else
                return string.Empty;
        }
    }
}
