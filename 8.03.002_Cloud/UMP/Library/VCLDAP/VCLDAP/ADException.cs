using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceCyber.VCLDAP
{
    public class ADException : Exception
    {
        private string m_strMessage;

        public ADException(string message)
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

        public ADException(string message, Exception ex)
        {

        }
    }
}
