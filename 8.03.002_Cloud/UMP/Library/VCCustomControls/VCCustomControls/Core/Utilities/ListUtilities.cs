//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9f87a0e9-0b75-459f-917b-f14626faadcb
//        CLR Version:              4.0.30319.18444
//        Name:                     ListUtilities
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core.Utilities
//        File Name:                ListUtilities
//
//        created by Charley at 2014/7/21 10:35:31
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.Wpf.CustomControls.Core.Utilities
{
    internal class ListUtilities
    {
        internal static Type GetListItemType(Type listType)
        {
            Type iListOfT = listType.GetInterfaces().FirstOrDefault(
              (i) => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>));

            return (iListOfT != null)
              ? iListOfT.GetGenericArguments()[0]
              : null;
        }
    }
}
