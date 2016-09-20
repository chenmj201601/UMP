//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b79759ce-9966-4c0e-9bd2-78f139bdd8df
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyDefinitionBaseCollection
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids
//        File Name:                PropertyDefinitionBaseCollection
//
//        created by Charley at 2014/7/23 12:08:20
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using VoiceCyber.Wpf.PropertyGrids.Definitions;

namespace VoiceCyber.Wpf.PropertyGrids
{
    public class PropertyDefinitionCollection : PropertyDefinitionBaseCollection<PropertyDefinition> { }
    public class EditorDefinitionCollection : PropertyDefinitionBaseCollection<EditorDefinitionBase> { }


    public abstract class PropertyDefinitionBaseCollection<T> : DefinitionCollectionBase<T> where T : PropertyDefinitionBase
    {
        internal PropertyDefinitionBaseCollection() { }

        public T this[object propertyId]
        {
            get
            {
                foreach (var item in Items)
                {
                    if (item.TargetProperties.Contains(propertyId))
                        return item;
                }

                return null;
            }
        }

        internal T GetRecursiveBaseTypes(Type type)
        {
            // If no definition for the current type, fall back on base type editor recursively.
            T ret = null;
            while (ret == null && type != null)
            {
                ret = this[type];
                type = type.BaseType;
            }
            return ret;
        }
    }

    public abstract class DefinitionCollectionBase<T> : ObservableCollection<T> where T : DefinitionBase
    {
        internal DefinitionCollectionBase() { }

        protected override void InsertItem(int index, T item)
        {
            if (item == null)
                throw new InvalidOperationException(@"Cannot insert null items in the collection.");

            item.Lock();
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            if (item == null)
                throw new InvalidOperationException(@"Cannot insert null items in the collection.");

            item.Lock();
            base.SetItem(index, item);
        }
    }
}
