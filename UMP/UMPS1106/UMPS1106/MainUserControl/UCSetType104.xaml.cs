﻿using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UMPS1106.MainUserControl
{
    public partial class UCSetType104
    {
        public MainView00000A IPageParent = null;

        private DataRow IDataRowCurrent = null;

        public UCSetType104()
        {
            InitializeComponent();
        }

        public void ShowParameterEditStyle(DataRow ADataRowParamInfo)
        {
            string LStr11001003 = string.Empty;     //参数编码
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;

            try
            {
                IDataRowCurrent = ADataRowParamInfo;
                if (IPageParent != null) { IPageParent.IOperationEvent += IPageParent_IOperationEvent; }
                LStr11001003 = ADataRowParamInfo["C003"].ToString();
                LStrVerificationCode104 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                RadioButtonValueX.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "VX");
                RadioButtonValue1.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V1");
                LStrParameterValueDB = ADataRowParamInfo["C006"].ToString();
                LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrParameterValueDB = LStrParameterValueDB.Substring(8);
                if (LStrParameterValueDB != "1")
                {
                    RadioButtonValueX.IsChecked = true;
                    foreach (ComboBoxItem LComboBoxItemSingle in ComboBoxFirstDay.Items)
                    {
                        if (LComboBoxItemSingle.DataContext.ToString() == LStrParameterValueDB)
                        {
                            LComboBoxItemSingle.IsSelected = true; break;
                        }
                    }
                }
                else
                {
                    RadioButtonValue1.IsChecked = true;
                }
                IPageParent.IChangeLanguageEvent += IPageParent_IChangeLanguageEvent;
            }
            catch { }
        }

        private void IPageParent_IChangeLanguageEvent(object sender, OperationEventArgs e)
        {
            string LStr11001003 = string.Empty;     //参数编码

            LStr11001003 = IDataRowCurrent["C003"].ToString();
            RadioButtonValueX.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "VX");
            RadioButtonValue1.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V1");

        }

        private void IPageParent_IOperationEvent(object sender, OperationEventArgs e)
        {
            string LStrNewValue = string.Empty;

            if (RadioButtonValue1.IsChecked == true)
            { LStrNewValue = "1"; }
            else
            {
                ComboBoxItem LComboBoxItemSeleted = ComboBoxFirstDay.SelectedItem as ComboBoxItem;
                LStrNewValue = LComboBoxItemSeleted.DataContext.ToString();
            }
            if (e.StrObjectTag == "Save") { IPageParent.SaveEditedParameterValue(LStrNewValue); }
        }
    }
}
