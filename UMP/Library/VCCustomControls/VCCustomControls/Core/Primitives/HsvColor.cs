//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1e5b052b-f217-44fb-909f-ec88bc265af6
//        CLR Version:              4.0.30319.18444
//        Name:                     HsvColor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core.Primitives
//        File Name:                HsvColor
//
//        created by Charley at 2014/7/18 12:07:52
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.CustomControls.Primitives
{
    internal struct HsvColor
    {
        public double H;
        public double S;
        public double V;

        public HsvColor(double h, double s, double v)
        {
            H = h;
            S = s;
            V = v;
        }
    }
}
