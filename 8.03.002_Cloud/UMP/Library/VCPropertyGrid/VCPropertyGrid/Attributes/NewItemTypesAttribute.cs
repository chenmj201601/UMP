//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5f339d8c-3266-497a-826b-d4bd3d41f033
//        CLR Version:              4.0.30319.18444
//        Name:                     NewItemTypesAttribute
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Attributes
//        File Name:                NewItemTypesAttribute
//
//        created by Charley at 2014/7/23 11:54:02
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.PropertyGrids.Attributes
{
    /// <summary>
    /// This attribute can decorate the collection properties (i.e., IList) 
    /// of your selected object in order to control the types that will be allowed
    /// to be instantiated in the CollectionControl.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NewItemTypesAttribute : Attribute
    {
        public IList<Type> Types
        {
            get;
            set;
        }

        public NewItemTypesAttribute(params Type[] types)
        {
            this.Types = new List<Type>(types);
        }

        public NewItemTypesAttribute()
        {
        }
    }
}
