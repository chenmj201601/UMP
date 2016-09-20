//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2e60e70e-09d1-48a0-984b-ac0139155803
//        CLR Version:              4.0.30319.18444
//        Name:                     IPropertyContainer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids
//        File Name:                IPropertyContainer
//
//        created by Charley at 2014/7/23 12:07:10
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.PropertyGrids
{
    internal interface IPropertyContainer
    {

        ContainerHelperBase ContainerHelper { get; }

        Style PropertyContainerStyle { get; }

        EditorDefinitionCollection EditorDefinitions { get; }

        PropertyDefinitionCollection PropertyDefinitions { get; }

        bool IsCategorized { get; }

        bool AutoGenerateProperties { get; }

        FilterInfo FilterInfo { get; }
    }
}
