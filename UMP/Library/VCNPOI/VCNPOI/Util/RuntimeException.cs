//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    e0718776-6138-417f-98e1-253fcc2aba10
//        CLR Version:              4.0.30319.42000
//        Name:                     RuntimeException
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.Util
//        File Name:                RuntimeException
//
//        Created by Charley at 2016/9/30 15:53:05
//        http://www.voicecyber.com 
//
//======================================================================

using System;


namespace VoiceCyber.NPOI.Util
{
    [Serializable]
    public class RuntimeException : Exception
    {
        public RuntimeException()
        {

        }
        public RuntimeException(string message)
            : base(message)
        {
        }
        public RuntimeException(Exception e)
            : base("", e)
        {
        }
        public RuntimeException(string exception, Exception ex)
            : base(exception, ex)
        {

        }
    }
}
