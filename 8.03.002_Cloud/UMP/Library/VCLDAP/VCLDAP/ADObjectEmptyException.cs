using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.VCLDAP
{
    public class ADObjectEmptyException : Exception
    {
        private string m_strMessage;
        public ADObjectEmptyException()
            : base()
        {
            m_strMessage = "实例化出错。";
        }

        public ADObjectEmptyException(string message)
            : base(message)
        {
            this.m_strMessage = message;
        }

        public override string Message
        {
            get
            {
                return this.m_strMessage;
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
