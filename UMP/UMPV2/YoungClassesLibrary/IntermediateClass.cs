using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YoungClassesLibrary
{
    public class IntermediateSingleFeature
    {
        /// <summary>
        /// 用户登录UMP系统后，自动分配的流水号
        /// </summary>
        public string StrLoginSerialID { get; set; }

        /// <summary>
        /// 当前登录账户号
        /// </summary>
        public string StrLoginAccount { get; set; }

        /// <summary>
        /// 当前登录账户名
        /// </summary>
        public string StrLoginUserName { get; set; }

        /// <summary>
        /// 功能编号
        /// </summary>
        public string StrFeatureID { get; set; }

        /// <summary>
        /// 功能名称
        /// </summary>
        public string StrFeatureName { get; set; }

        /// <summary>
        /// 功能图片，png格式，建议是 96 dpi
        /// </summary>
        public string StrFeatureImage { get; set; }

        /// <summary>
        /// 显示图片的大小。V1：小；V2：中；V3：大；V3：宽；VL：加载；VF：进入模块后的Logo
        /// </summary>
        public string StrImageSize { get; set; }

        /// <summary>
        /// 使用的协议，如：HTTP/HTTPS/NET.TCP等
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
        /// 使用的样式。Style01、Style02、Style03、Style04
        /// </summary>
        public string StrUseStyle { get; set; }

        /// <summary>
        /// 打开的XBAP部署清单
        /// </summary>
        public string StrOpenXbap { get; set; }

        /// <summary>
        /// 传入的参数
        /// </summary>
        public string StrXbapArgs { get; set; }
    }
}
