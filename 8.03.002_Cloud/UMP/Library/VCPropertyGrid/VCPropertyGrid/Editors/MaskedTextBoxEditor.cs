//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    618acad4-b524-4832-983b-0d8b06838270
//        CLR Version:              4.0.30319.18444
//        Name:                     MaskedTextBoxEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Editors
//        File Name:                MaskedTextBoxEditor
//
//        created by Charley at 2014/7/23 12:02:00
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using VoiceCyber.Wpf.CustomControls;

namespace VoiceCyber.Wpf.PropertyGrids.Editors
{
    public class MaskedTextBoxEditor : TypeEditor<MaskedTextBox>
    {
        public string Mask
        {
            get;
            set;
        }

        public Type ValueDataType
        {
            get;
            set;
        }

        protected override void SetControlProperties()
        {
            Editor.BorderThickness = new System.Windows.Thickness(0);
            this.Editor.ValueDataType = this.ValueDataType;
            this.Editor.Mask = this.Mask;
        }

        protected override void SetValueDependencyProperty()
        {
            this.ValueProperty = MaskedTextBox.ValueProperty;
        }
    }
}
