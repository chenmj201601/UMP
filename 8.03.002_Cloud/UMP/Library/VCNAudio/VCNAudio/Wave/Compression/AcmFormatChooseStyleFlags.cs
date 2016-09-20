//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    bed2b81b-2cc1-49b5-9c69-79510bd54d1b
//        CLR Version:              4.0.30319.34003
//        Name:                     AcmFormatChooseStyleFlags
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.Compression
//        File Name:                AcmFormatChooseStyleFlags
//
//        created by Charley at 2013/12/1 13:18:30
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave.Compression
{
    [Flags]
    enum AcmFormatChooseStyleFlags
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// ACMFORMATCHOOSE_STYLEF_SHOWHELP
        /// </summary>
        ShowHelp = 0x00000004,
        /// <summary>
        /// ACMFORMATCHOOSE_STYLEF_ENABLEHOOK
        /// </summary>
        EnableHook = 0x00000008,
        /// <summary>
        /// ACMFORMATCHOOSE_STYLEF_ENABLETEMPLATE
        /// </summary>
        EnableTemplate = 0x00000010,
        /// <summary>
        /// ACMFORMATCHOOSE_STYLEF_ENABLETEMPLATEHANDLE
        /// </summary>
        EnableTemplateHandle = 0x00000020,
        /// <summary>
        /// ACMFORMATCHOOSE_STYLEF_INITTOWFXSTRUCT
        /// </summary>
        InitToWfxStruct = 0x00000040,
        /// <summary>
        /// ACMFORMATCHOOSE_STYLEF_CONTEXTHELP
        /// </summary>
        ContextHelp = 0x00000080
    }
}
