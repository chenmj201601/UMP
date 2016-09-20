//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3b09239c-20ef-46a7-874c-023f96fc5f93
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyDefinitionBase
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Definitions
//        File Name:                PropertyDefinitionBase
//
//        created by Charley at 2014/7/23 11:57:52
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using VoiceCyber.Wpf.PropertyGrids.Converters;

namespace VoiceCyber.Wpf.PropertyGrids.Definitions
{
    public abstract class PropertyDefinitionBase : DefinitionBase
    {
        private IList _targetProperties;

        internal PropertyDefinitionBase()
        {
            _targetProperties = new List<object>();
        }

        [TypeConverter(typeof(ListConverter))]
        public IList TargetProperties
        {
            get { return _targetProperties; }
            set
            {
                this.ThrowIfLocked(() => this.TargetProperties);
                _targetProperties = value;
            }
        }

        internal override void Lock()
        {
            if (this.IsLocked)
                return;

            base.Lock();

            // Just create a new copy of the properties target to ensure 
            // that the list doesn't ever get modified.

            List<object> newList = new List<object>();
            if (_targetProperties != null)
            {
                foreach (object p in _targetProperties)
                {
                    object prop = p;
                    // Convert all TargetPropertyType to Types
                    var targetType = prop as TargetPropertyType;
                    if (targetType != null)
                    {
                        prop = targetType.Type;
                    }
                    newList.Add(prop);
                }
            }

            //In Designer Mode, the Designer is broken if using a ReadOnlyCollection
            _targetProperties = DesignerProperties.GetIsInDesignMode(this)
                                ? new Collection<object>(newList)
                                : new ReadOnlyCollection<object>(newList) as IList;
        }
    }
}
