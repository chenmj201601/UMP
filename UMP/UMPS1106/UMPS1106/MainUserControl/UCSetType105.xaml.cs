using PFShareClassesC;
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
    public partial class UCSetType105
    {
        public MainView00000A IPageParent = null;

        private DataRow IDataRowCurrent = null;

        public UCSetType105()
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
                CheckBoxExtension.IsChecked = false; CheckBoxAgentID.IsChecked = false;
                LStr11001003 = ADataRowParamInfo["C003"].ToString();
                LStrVerificationCode104 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                CheckBoxExtension.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "VE");
                CheckBoxAgentID.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "VA");
                CheckRealExtension.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "VR");
                LStrParameterValueDB = ADataRowParamInfo["C006"].ToString();
                LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrParameterValueDB = LStrParameterValueDB.Substring(8);
                string[] LStrGroupType = LStrParameterValueDB.Split(IPageParent.IStrSpliterChar.ToCharArray());
                foreach (string LStrType in LStrGroupType)
                {
                    if (LStrType == "E") { CheckBoxExtension.IsChecked = true; continue; }
                    if (LStrType == "A") { CheckBoxAgentID.IsChecked = true; continue; }
                    if (LStrType == "R") { CheckRealExtension.IsChecked = true; continue; }
                }

                IPageParent.IChangeLanguageEvent += IPageParent_IChangeLanguageEvent;
            }
            catch { }
        }

        private void IPageParent_IChangeLanguageEvent(object sender, OperationEventArgs e)
        {
            string LStr11001003 = string.Empty;     //参数编码

            LStr11001003 = IDataRowCurrent["C003"].ToString();
            CheckBoxExtension.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "VE");
            CheckBoxAgentID.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "VA");
            CheckRealExtension.Content = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "VR");
        }

        private void IPageParent_IOperationEvent(object sender, OperationEventArgs e)
        {
            string LStrNewValue = string.Empty;
            List<string> LListStrNewValue = new List<string>();

            if (CheckBoxExtension.IsChecked == true) { LListStrNewValue.Add("E"); }
            if (CheckBoxAgentID.IsChecked == true) { LListStrNewValue.Add("A"); }
            if (CheckRealExtension.IsChecked == true) { LListStrNewValue.Add("R"); }
            if (LListStrNewValue.Count <= 0) { return; }
            LStrNewValue = string.Join(IPageParent.IStrSpliterChar, LListStrNewValue.ToArray());
            if (e.StrObjectTag == "Save") { IPageParent.SaveEditedParameterValue(LStrNewValue); }
        }
    }
}
