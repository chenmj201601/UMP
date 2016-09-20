//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8ccc2df9-4ede-452f-85bb-3c170f25f602
//        CLR Version:              4.0.30319.18444
//        Name:                     StaticConverters
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Ribbon.Converters
//        File Name:                StaticConverters
//
//        created by Charley at 2014/5/28 11:22:02
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace VoiceCyber.Ribbon.Converters
{
    public static class StaticConverters
    {
        public static readonly InvertNumericConverter InvertNumericConverter = new InvertNumericConverter();

        public static readonly ThicknessConverter ThicknessConverter = new ThicknessConverter();
    }
}
