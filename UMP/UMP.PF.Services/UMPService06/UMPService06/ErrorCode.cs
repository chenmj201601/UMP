using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMPService06
{
    public enum Service06ErrorCode 
    {
        /// <summary>
        /// 验证HelloService06错误
        /// </summary>
        ERROR01 = 10001,

        /// <summary>
        /// 验证32位随机数错误
        /// </summary>
        ERROR02 = 10002,

        /// <summary>
        /// 验证方法码错误
        /// </summary>
        ERROR03 = 10003,
      

        /// <summary>
        /// 方法处理错误
        /// </summary>
        ERROR04 = 10004,


          /// <summary>
        /// 方法参数错误
        /// </summary>
        ERROR05 = 10005,


        /// <summary>
        /// 方法运行错误
        /// </summary>
        ERROR06 = 10006,

        /// <summary>
        ///无匹配的数据
        /// </summary>
        ERROR07 = 10007,

        /// <summary>
        /// 系统不识别命令指令
        /// </summary>
        ERROR08 = 10008
        ,
        /// <summary>
        /// G002数量不对
        /// </summary>
        ERROR09=10009
    }
}
