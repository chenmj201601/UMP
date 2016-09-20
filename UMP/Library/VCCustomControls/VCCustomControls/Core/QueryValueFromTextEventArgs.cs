//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5792b255-6719-43b6-96df-5d5e49bc4b01
//        CLR Version:              4.0.30319.18444
//        Name:                     QueryValueFromTextEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core
//        File Name:                QueryValueFromTextEventArgs
//
//        created by Charley at 2014/7/21 10:27:30
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.CustomControls.Core
{
    public class QueryValueFromTextEventArgs : EventArgs
    {
        public QueryValueFromTextEventArgs(string text, object value)
        {
            m_text = text;
            m_value = value;
        }

        #region Text Property

        private string m_text;

        public string Text
        {
            get { return m_text; }
        }

        #endregion Text Property

        #region Value Property

        private object m_value;

        public object Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        #endregion Value Property

        #region HasParsingError Property

        private bool m_hasParsingError;

        public bool HasParsingError
        {
            get { return m_hasParsingError; }
            set { m_hasParsingError = value; }
        }

        #endregion HasParsingError Property

    }
}
