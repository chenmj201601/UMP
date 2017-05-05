using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PFShareClassesC
{
    public class IntermediateArgs
    {
        /// <summary>
        /// 用户登录UMP系统后，自动分配的流水号
        /// </summary>
        public string StrLoginSerialID { get; set; }

        /// <summary>
        /// 用户内部编码，对应数据库C001
        /// </summary>
        public long LongUserID { get; set; }

        /// <summary>
        /// 当前登录账户号
        /// </summary>
        public string StrLoginAccount { get; set; }

        /// <summary>
        /// 当前登录账户全名
        /// </summary>
        public string StrLoginUserName { get; set; }

        /// <summary>
        /// 登录用户的角色编号。如果该用户拥有多个角色，0：全部角色，其他：具体的角色编号
        /// </summary>
        public long LongLoginRoleID { get; set; }

        /// <summary>
        /// 登录用户的角色名称
        /// </summary>
        public string StrLoginRoleName { get; set; }

        /// <summary>
        /// 使用的样式。Style01、Style02、Style03、Style04
        /// </summary>
        public string StrUseStyle { get; set; }

        /// <summary>
        /// 用户选择的语言ID,如：1033，2052，1028，1041等
        /// </summary>
        public string StrLanguageID { get; set; }
        
    }

    public class IntermediateArgsAppServer
    {
        /// <summary>
        /// 使用的协议，如：HTTP/HTTPS等
        /// </summary>
        public string StrUseProtol { get; set; }

        /// <summary>
        /// 应用服务器IP或名称
        /// </summary>
        public string StrServerHost { get; set; }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public string StrServerPort { get; set; }

        /// <summary>
        /// 是否启用了Net.tcp 协议
        /// </summary>
        public bool BoolEnableNettcp { get; set; }

        /// <summary>
        /// Net.tcp 端口
        /// </summary>
        public int IntNettcpPort { get; set; }
    }

}
