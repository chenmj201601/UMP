//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e8d20d5a-ea2b-4c78-b61d-36484f32cc76
//        CLR Version:              4.0.30319.18444
//        Name:                     ColorItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.ColorPicker.Implementation
//        File Name:                ColorItem
//
//        created by Charley at 2014/7/18 12:16:18
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls
{
    public class ColorItem
    {
        public Color Color
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }

        public ColorItem(Color color, string name)
        {
            Color = color;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var ci = obj as ColorItem;
            if (ci == null)
                return false;
            return (ci.Color.Equals(Color) && ci.Name.Equals(Name));
        }

        public override int GetHashCode()
        {
            return this.Color.GetHashCode() ^ this.Name.GetHashCode();
        }
    }
}
