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
    public partial class UCSetType107
    {
        public MainView00000A IPageParent = null;

        private DataRow IDataRowCurrent = null;

        public UCSetType107()
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
                LoadThisDataEnumeration(LStr11001003);
                LStrVerificationCode104 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrParameterValueDB = ADataRowParamInfo["C006"].ToString();
                LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrParameterValueDB = LStrParameterValueDB.Substring(8);

                foreach (RadioButton LRadioButtonSingleItem in StackPanelDataEnumeration.Children)
                {
                    if (LRadioButtonSingleItem.DataContext.ToString() == LStrParameterValueDB)
                    {
                        LRadioButtonSingleItem.IsChecked = true; break;
                    }
                }

                IPageParent.IChangeLanguageEvent += IPageParent_IChangeLanguageEvent;
            }
            catch { }
        }

        private void LoadThisDataEnumeration(string AStr11001003)
        {
            string LStr00003001 = string.Empty;

            try
            {
                StackPanelDataEnumeration.Children.Clear();

                DataRow[] LDataRowRoot = S1106App.GDataTable00003.Select("C003 = " + AStr11001003);
                LStr00003001 = LDataRowRoot[0]["C001"].ToString();
                DataRow[] LDataRowDataEnumeration = S1106App.GDataTable00003.Select("C003 = " + LStr00003001, "C002 ASC");
                foreach (DataRow LDataRowSingleData in LDataRowDataEnumeration)
                {
                    RadioButton LRadioButtonSingleItem = new RadioButton();
                    LRadioButtonSingleItem.Content = string.Format("{0:N0}", int.Parse(LDataRowSingleData["C006"].ToString()));
                    LRadioButtonSingleItem.DataContext = LDataRowSingleData["C006"].ToString();
                    LRadioButtonSingleItem.Style = (Style)App.Current.Resources["RadioButtonNormalStyle"];
                    LRadioButtonSingleItem.IsChecked = false;
                    StackPanelDataEnumeration.Children.Add(LRadioButtonSingleItem);
                }
            }
            catch { }
        }

        private void IPageParent_IChangeLanguageEvent(object sender, OperationEventArgs e)
        {

        }

        private void IPageParent_IOperationEvent(object sender, OperationEventArgs e)
        {
            string LStrNewValue = string.Empty;

            foreach (RadioButton LRadioButtonSingleItem in StackPanelDataEnumeration.Children)
            {
                if (LRadioButtonSingleItem.IsChecked == true)
                {
                    LStrNewValue = LRadioButtonSingleItem.DataContext.ToString(); break;
                }
            }
            if (string.IsNullOrEmpty(LStrNewValue)) { return; }
            if (e.StrObjectTag == "Save") { IPageParent.SaveEditedParameterValue(LStrNewValue); }
        }
    }
}

