//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e5c1e595-e4be-45db-9267-3226aacb13aa
//        CLR Version:              4.0.30319.18444
//        Name:                     CategoryOrderAttribute
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Attributes
//        File Name:                CategoryOrderAttribute
//
//        created by Charley at 2014/7/23 11:52:29
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.PropertyGrids.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CategoryOrderAttribute : Attribute
    {
        #region Properties

        #region Order

        public int Order
        {
            get;
            set;
        }

        #endregion

        #region Category

        public virtual string Category
        {
            get
            {
                return CategoryValue;
            }
        }

        #endregion

        #region CategoryValue

        public string CategoryValue
        {
            get;
            private set;
        }

        #endregion

        #endregion

        #region constructor

        public CategoryOrderAttribute()
        {
        }

        public CategoryOrderAttribute(string categoryName, int order)
            : this()
        {
            CategoryValue = categoryName;
            Order = order;
        }

        #endregion
    }
}
