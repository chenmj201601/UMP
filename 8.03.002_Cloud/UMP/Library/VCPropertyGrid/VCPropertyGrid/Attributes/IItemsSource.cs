//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ed86af35-899f-4467-ae1b-8d3c216d0da6
//        CLR Version:              4.0.30319.18444
//        Name:                     IItemsSource
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Attributes
//        File Name:                IItemsSource
//
//        created by Charley at 2014/7/23 11:53:15
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.PropertyGrids.Attributes
{
    public interface IItemsSource
    {
        ItemCollection GetValues();
    }

    public class Item
    {
        public string DisplayName
        {
            get;
            set;
        }
        public object Value
        {
            get;
            set;
        }
    }

    public class ItemCollection : List<Item>
    {
        public void Add(object value)
        {
            Item item = new Item();
            item.DisplayName = value.ToString();
            item.Value = value;
            base.Add(item);
        }

        public void Add(object value, string displayName)
        {
            Item newItem = new Item();
            newItem.DisplayName = displayName;
            newItem.Value = value;
            base.Add(newItem);
        }
    }
}
