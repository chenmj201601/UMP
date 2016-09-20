//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8afacb9c-2d0d-4320-a8bc-2a819958729f
//        CLR Version:              4.0.30319.18444
//        Name:                     IValidateInput
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core.Input
//        File Name:                IValidateInput
//
//        created by Charley at 2014/7/17 16:11:47
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.Wpf.CustomControls.Core.Input
{
    public interface IValidateInput
    {
        event InputValidationErrorEventHandler InputValidationError;
        bool CommitInput();
    }
}
