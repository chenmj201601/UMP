//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f3f61d03-56d9-45af-8399-2ca83dd1b2f5
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyOrderAttribute
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Attributes
//        File Name:                PropertyOrderAttribute
//
//        created by Charley at 2014/7/23 11:54:19
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.PropertyGrids.Attributes
{
    public enum UsageContextEnum
    {
        Alphabetical,
        Categorized,
        Both
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class PropertyOrderAttribute : Attribute
    {
        #region Properties

        public int Order
        {
            get;
            set;
        }

        public UsageContextEnum UsageContext
        {
            get;
            set;
        }

        public override object TypeId
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region Initialization

        public PropertyOrderAttribute(int order)
            : this(order, UsageContextEnum.Both)
        {
        }

        public PropertyOrderAttribute(int order, UsageContextEnum usageContext)
        {
            Order = order;
            UsageContext = usageContext;
        }

        #endregion
    }
}
