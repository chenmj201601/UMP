//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    44b31130-d5b5-4b9b-8dff-1d98960bf408
//        CLR Version:              4.0.30319.18444
//        Name:                     QueryTextFromValueEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core
//        File Name:                QueryTextFromValueEventArgs
//
//        created by Charley at 2014/7/21 10:26:22
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.CustomControls.Core
{
    public class QueryTextFromValueEventArgs : EventArgs
    {
        public QueryTextFromValueEventArgs(object value, string text)
        {
            m_value = value;
            m_text = text;
        }

        #region Value Property

        private object m_value;

        public object Value
        {
            get { return m_value; }
        }

        #endregion Value Property

        #region Text Property

        private string m_text;

        public string Text
        {
            get { return m_text; }
            set { m_text = value; }
        }

        #endregion Text Property
    }
}
