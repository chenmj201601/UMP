//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f67913d7-5d17-46a3-a3d7-5e83f88d0148
//        CLR Version:              4.0.30319.42000
//        Name:                     AppControlService
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1201
//        File Name:                AppControlService
//
//        created by Charley at 2016/1/28 17:59:17
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using VoiceCyber.UMP.Common;

namespace UMPS1201
{
    public class AppControlService : IAppControlService
    {
        private IList<IAppConfigInfo> mListAppConfigs;

        public IList<IAppConfigInfo> ListAppConfigs
        {
            get { return mListAppConfigs; }
        }

        public AppControlService()
        {
            mListAppConfigs = new List<IAppConfigInfo>();
        }
    }
}
