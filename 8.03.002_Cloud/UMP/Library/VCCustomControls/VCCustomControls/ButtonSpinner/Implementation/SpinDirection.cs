//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    bbdba80d-edd6-4312-bf29-00271b264ab2
//        CLR Version:              4.0.30319.18444
//        Name:                     SpinDirection
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.ButtonSpinner.Implementation
//        File Name:                SpinDirection
//
//        created by Charley at 2014/7/17 16:18:53
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// Represents spin directions that could be initiated by the end-user.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum SpinDirection
    {
        /// <summary>
        /// Represents a spin initiated by the end-user in order to Increase a value.
        /// </summary>
        Increase = 0,

        /// <summary>
        /// Represents a spin initiated by the end-user in order to Decrease a value.
        /// </summary>
        Decrease = 1
    }
}
