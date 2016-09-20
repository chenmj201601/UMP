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
    public partial class UCSetType106 
    {
        public MainView00000A IPageParent = null;

        private DataRow IDataRowCurrent = null;

        public UCSetType106()
        {
            InitializeComponent();
        }

        public void ShowParameterEditStyle(DataRow ADataRowParamInfo)
        {
            string LStr11001003 = string.Empty;     //参数编码
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;

            int LIntMinValue = 0, LIntMaxValue = 0, LIntDefValue = 0;

            try
            {
                IDataRowCurrent = ADataRowParamInfo;
                if (IPageParent != null) { IPageParent.IOperationEvent += IPageParent_IOperationEvent; }

                LStr11001003 = ADataRowParamInfo["C003"].ToString();
                LStrVerificationCode104 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LStrParameterValueDB = ADataRowParamInfo["C006"].ToString();
                LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrParameterValueDB = LStrParameterValueDB.Substring(8);
                if (S1106App.GetParameterMinMaxDefValue(LStr11001003, ref LIntMinValue, ref LIntMaxValue, ref LIntDefValue))
                {
                    TextBoxPositiveInteger.SetMinMaxDefaultValue(LIntMinValue, LIntMaxValue, LIntDefValue);
                }

                TextBoxPositiveInteger.SetElementData(LStrParameterValueDB);
                IPageParent.IChangeLanguageEvent += IPageParent_IChangeLanguageEvent;

            }
            catch { }
        }

        private void IPageParent_IChangeLanguageEvent(object sender, OperationEventArgs e)
        {
        }

        private void IPageParent_IOperationEvent(object sender, OperationEventArgs e)
        {
            string LStrNewValue = string.Empty;

            LStrNewValue = TextBoxPositiveInteger.GetElementData();

            if (e.StrObjectTag == "Save") { IPageParent.SaveEditedParameterValue(LStrNewValue); }
        }
    }
}

