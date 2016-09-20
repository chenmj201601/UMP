//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    612cfe5e-33a2-4a6a-bcb4-78326a88a33c
//        CLR Version:              4.0.30319.18444
//        Name:                     TargetPropertyType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids
//        File Name:                TargetPropertyType
//
//        created by Charley at 2014/7/23 12:11:00
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.PropertyGrids
{
    /// <summary>
    /// This class is intended to provide the "Type" target
    /// for property definitions or editor definitions when
    /// using Property Element Syntax.
    /// </summary>
    public sealed class TargetPropertyType
    {
        private Type _type;
        private bool _sealed;

        public Type Type
        {
            get { return _type; }
            set
            {
                if (_sealed)
                    throw new InvalidOperationException(
                      string.Format(
                      "{0}.Type property cannot be modified once the instance is used",
                      typeof(TargetPropertyType)));

                _type = value;
            }
        }

        internal void Seal()
        {
            if (_type == null)
                throw new InvalidOperationException(
                  string.Format("{0}.Type property must be initialized", typeof(TargetPropertyType)));

            _sealed = true;
        }
    }
}
