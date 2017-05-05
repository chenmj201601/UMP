//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    f9268e35-e294-4c56-af45-e82efd72b8c8
//        CLR Version:              4.0.30319.42000
//        Name:                     IEvaluationName
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula
//        File Name:                IEvaluationName
//
//        Created by Charley at 2016/9/30 16:05:49
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace VoiceCyber.NPOI.SS.Formula
{
    /**
      * Abstracts a name record for formula evaluation.<br/>
      * 
      * For POI internal use only
      * 
      * @author Josh Micich
      */
    public interface IEvaluationName
    {

        String NameText { get; }

        bool IsFunctionName { get; }

        bool HasFormula { get; }

        Ptg[] NameDefinition { get; }

        bool IsRange { get; }
        NamePtg CreatePtg();
    }
}
