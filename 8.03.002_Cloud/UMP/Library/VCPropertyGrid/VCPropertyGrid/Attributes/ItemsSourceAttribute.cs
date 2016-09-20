//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    77d530de-2aec-46bb-8a63-e7dcf3873314
//        CLR Version:              4.0.30319.18444
//        Name:                     ItemsSourceAttribute
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Attributes
//        File Name:                ItemsSourceAttribute
//
//        created by Charley at 2014/7/23 11:53:42
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.PropertyGrids.Attributes
{
    public class ItemsSourceAttribute : Attribute
    {
        public Type Type
        {
            get;
            set;
        }

        public ItemsSourceAttribute(Type type)
        {
            var valueSourceInterface = type.GetInterface(typeof(IItemsSource).FullName);
            if (valueSourceInterface == null)
                throw new ArgumentException("Type must implement the IItemsSource interface.", "type");

            Type = type;
        }
    }
}
