//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    666a09f0-122d-4fd5-b6d4-3a816ccb2101
//        CLR Version:              4.0.30319.18444
//        Name:                     ListUtilities
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Utilities
//        File Name:                ListUtilities
//
//        created by Charley at 2014/7/23 15:31:17
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.Wpf.PropertyGrids.Utilities
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
