using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.VCLDAP
{
    public class ADPropertyException : Exception
    {
        private string m_strMessage;
        public ADPropertyException()
            : base()
        {
            m_strMessage = "修改属性时出错";
        }

        /**/
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="message">错误信息</param>
        public ADPropertyException(string message)
            : base(message)
        {
            this.m_strMessage = message;
        }

        /**/
        /// <summary>
        /// 指定出错显示的信息
        /// </summary>
        public override string Message
        {
            get
            {
                return this.m_strMessage;
            }
        }

        /**/
        /// <summary>
        /// 返回它的字符表示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
