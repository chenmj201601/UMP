//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0ff00931-b699-4724-9449-53ded66ef335
//        CLR Version:              4.0.30319.18444
//        Name:                     DefinitionBase
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Definitions
//        File Name:                DefinitionBase
//
//        created by Charley at 2014/7/23 11:56:49
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using VoiceCyber.Wpf.PropertyGrids.Utilities;

namespace VoiceCyber.Wpf.PropertyGrids.Definitions
{
    public abstract class DefinitionBase : DependencyObject
    {
        private bool _isLocked;

        internal bool IsLocked
        {
            get { return _isLocked; }
        }

        internal void ThrowIfLocked<TMember>(Expression<Func<TMember>> propertyExpression)
        {
            if (this.IsLocked)
            {
                string propertyName = ReflectionHelper.GetPropertyOrFieldName(propertyExpression);
                string message = string.Format(
                    @"Cannot modify {0} once the definition has beed added to a collection.",
                    propertyName);
                throw new InvalidOperationException(message);
            }
        }

        internal virtual void Lock()
        {
            if (!_isLocked)
            {
                _isLocked = true;
            }
        }
    }
}
