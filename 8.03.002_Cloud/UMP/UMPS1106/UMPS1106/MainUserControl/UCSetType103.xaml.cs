using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PFShareClassesC;
using System.Data;
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
    public partial class UCSetType103
    {
        public MainView00000A IPageParent = null;

        private DataRow IDataRowCurrent = null;

        public UCSetType103()
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

                RadioButtonValue0.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V0");
                RadioButtonValue1.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V1");
                RadioButtonValue2.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V2");
                RadioButtonValue3.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V3");
                RadioButtonValue4.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V4");
                RadioButtonValue5.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V5");
                RadioButtonValue6.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V6");

                LStrParameterValueDB = ADataRowParamInfo["C006"].ToString();
                LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrParameterValueDB = LStrParameterValueDB.Substring(8);
                switch (LStrParameterValueDB)
                {
                    case "0":
                        RadioButtonValue0.IsChecked = true;
                        break;
                    case "1":
                        RadioButtonValue1.IsChecked = true;
                        break;
                    case "2":
                        RadioButtonValue2.IsChecked = true;
                        break;
                    case "3":
                        RadioButtonValue3.IsChecked = true;
                        break;
                    case "4":
                        RadioButtonValue4.IsChecked = true;
                        break;
                    case "5":
                        RadioButtonValue5.IsChecked = true;
                        break;
                    case "6":
                        RadioButtonValue6.IsChecked = true;
                        break;
                    default:
                        break;
                }

                IPageParent.IChangeLanguageEvent += IPageParent_IChangeLanguageEvent;
            }
            catch { }
        }

        private void IPageParent_IChangeLanguageEvent(object sender, OperationEventArgs e)
        {
            string LStr11001003 = string.Empty;     //参数编码

            LStr11001003 = IDataRowCurrent["C003"].ToString();
            RadioButtonValue0.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V0");
            RadioButtonValue1.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V1");
            RadioButtonValue2.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V2");
            RadioButtonValue3.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V3");
            RadioButtonValue4.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V4");
            RadioButtonValue5.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V5");
            RadioButtonValue6.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V6");
        }

        private void IPageParent_IOperationEvent(object sender, OperationEventArgs e)
        {
            string LStrNewValue = string.Empty;

            if (RadioButtonValue0.IsChecked == true)
            {
                LStrNewValue = "0";
            }
            else if (RadioButtonValue1.IsChecked == true)
            {
                LStrNewValue = "1";
            }
            else if (RadioButtonValue2.IsChecked == true)
            {
                LStrNewValue = "2";
            }
            else if (RadioButtonValue3.IsChecked == true)
            {
                LStrNewValue = "3";
            }
            else if (RadioButtonValue4.IsChecked == true)
            {
                LStrNewValue = "4";
            }
            else if (RadioButtonValue5.IsChecked == true)
            {
                LStrNewValue = "5";
            }
            else if (RadioButtonValue6.IsChecked == true)
            {
                LStrNewValue = "6";
            }
            if (string.IsNullOrEmpty(LStrNewValue)) { return; }
            if (e.StrObjectTag == "Save") { IPageParent.SaveEditedParameterValue(LStrNewValue); }
        }
    }
}

