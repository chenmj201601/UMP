//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    77c180fe-ab21-49f8-bf7c-cad464f76c48
//        CLR Version:              4.0.30319.18063
//        Name:                     OperationReturn
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPSetup.Common
//        File Name:                OperationReturn
//
//        created by Charley at 2015/12/29 12:02:36
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace UMPSetup.Common
{
    /// <summary>
    /// 操作返回值
    /// </summary>
    public class OperationReturn
    {
        /// <summary>
        /// 操作结果
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        /// 返回代码，参考Defines中的定义
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回值，整型
        /// </summary>
        public int IntValue { get; set; }
        /// <summary>
        /// 返回值，数值型
        /// </summary>
        public decimal NumericValue { get; set; }
        /// <summary>
        /// 返回值，文本型
        /// </summary>
        public string StringValue { get; set; }
        /// <summary>
        /// 返回值，使用的时候可通过 as 转换成对应的对象
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 操作异常
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// 以字符串形式返回
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string strReturn = string.Empty;
            if (Exception != null)
            {
                strReturn = Exception.Message;
            }
            if (string.IsNullOrEmpty(Message))
            {
                return string.Format("{0}-{1}", Code.ToString("0000"), strReturn);
            }
            return string.Format("{0}-{1}", Code.ToString("0000"), Message);
        }
    }
}
