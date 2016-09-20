//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    be0c2433-012d-43f6-bdfd-4e11c40cd1e4
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyDefinition
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids
//        File Name:                PropertyDefinition
//
//        created by Charley at 2014/7/23 12:08:05
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using VoiceCyber.Wpf.PropertyGrids.Definitions;

namespace VoiceCyber.Wpf.PropertyGrids
{
    public class PropertyDefinition : PropertyDefinitionBase
    {
        private string _name;
        private bool? _isBrowsable = true;
        private bool? _isExpandable = null;
        private string _displayName = null;
        private string _description = null;
        private string _category = null;
        private int? _displayOrder = null;

        [Obsolete(@"Use 'TargetProperties' instead of 'Name'")]
        public string Name
        {
            get { return _name; }
            set
            {
                const string usageError = "{0}: \'Name\' property is obsolete. Instead use \'TargetProperties\'. (XAML example: <t:PropertyDefinition TargetProperties=\"FirstName,LastName\" />)";
                System.Diagnostics.Trace.TraceWarning(usageError, typeof(PropertyDefinition));
                _name = value;
            }
        }

        public string Category
        {
            get { return _category; }
            set
            {
                this.ThrowIfLocked(() => this.Category);
                _category = value;
            }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                this.ThrowIfLocked(() => this.DisplayName);
                _displayName = value;
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                this.ThrowIfLocked(() => this.Description);
                _description = value;
            }
        }

        public int? DisplayOrder
        {
            get { return _displayOrder; }
            set
            {
                this.ThrowIfLocked(() => this.DisplayOrder);
                _displayOrder = value;
            }
        }

        public bool? IsBrowsable
        {
            get { return _isBrowsable; }
            set
            {
                this.ThrowIfLocked(() => this.IsBrowsable);
                _isBrowsable = value;
            }
        }

        public bool? IsExpandable
        {
            get { return _isExpandable; }
            set
            {
                this.ThrowIfLocked(() => this.IsExpandable);
                _isExpandable = value;
            }
        }

        internal override void Lock()
        {
            if (_name != null
              && this.TargetProperties != null
              && this.TargetProperties.Count > 0)
            {
                throw new InvalidOperationException(
                  string.Format(
                    @"{0}: When using 'TargetProperties' property, do not use 'Name' property.",
                    typeof(PropertyDefinition)));
            }

            if (_name != null)
            {
                this.TargetProperties = new List<object>() { _name };
            }
            base.Lock();
        }
    }
}
