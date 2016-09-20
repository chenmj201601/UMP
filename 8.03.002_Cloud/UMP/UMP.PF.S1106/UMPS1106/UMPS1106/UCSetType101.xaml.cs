using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace UMPS1106
{
    public partial class UCSetType101 : UserControl
    {
        public Page00000A IPageParent = null;

        private DataRow IDataRowCurrent = null;

        public UCSetType101()
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
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                RadioButtonValue1.Content = App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V1");
                RadioButtonValue0.Content = App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V0");
                LStrParameterValueDB = ADataRowParamInfo["C006"].ToString();
                LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrParameterValueDB = LStrParameterValueDB.Substring(8);
                if (LStrParameterValueDB == "1")
                {
                    RadioButtonValue1.IsChecked = true;
                }
                else
                {
                    RadioButtonValue0.IsChecked = true;
                }
                IPageParent.IChangeLanguageEvent += IPageParent_IChangeLanguageEvent;
            }
            catch { }
        }

        private void IPageParent_IChangeLanguageEvent(object sender, OperationEventArgs e)
        {
            string LStr11001003 = string.Empty;     //参数编码

            LStr11001003 = IDataRowCurrent["C003"].ToString();
            RadioButtonValue1.Content = App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V1");
            RadioButtonValue0.Content = App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V0");
        }

        private void IPageParent_IOperationEvent(object sender, OperationEventArgs e)
        {
            string LStrNewValue = string.Empty;

            if (RadioButtonValue1.IsChecked == true)
            { LStrNewValue = "1"; }
            else { LStrNewValue = "0"; }
            if (e.StrObjectTag == "Save") { IPageParent.SaveEditedParameterValue(LStrNewValue); }
        }
    }
}
