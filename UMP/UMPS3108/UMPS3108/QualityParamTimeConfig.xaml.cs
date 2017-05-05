using System;
using System.Collections.Generic;
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
using UMPS3108.Common31081;

namespace UMPS3108
{
    /// <summary>
    /// ParamsItemsTimeConfigPage.xaml 的交互逻辑
    /// </summary>
    public partial class QualityParamTimeConfig
    {
        #region menber

        public List<QualityParam> Display;
        public SCMainView mainPage;

        #endregion

        public QualityParamTimeConfig()
        {
            InitializeComponent();
        }
        public void SetCheck()
        {
            if (Display[0].ParamValue.Substring(8, 1) == "1")
            {
                this.CheckB1.IsChecked = true;
            }
            else
            {
                this.CheckB1.IsChecked = false;
            }
            if (Display[1].ParamValue.Substring(8, 1) == "1")
            {
                this.CheckB2.IsChecked = true;
            }
            else
            {
                this.CheckB2.IsChecked = false;
            }
            if (Display[2].ParamValue.Substring(8, 1) == "1")
            {
                this.CheckB3.IsChecked = true;
            }
            else
            {
                this.CheckB3.IsChecked = false;
            }
            if (Display[3].ParamValue.Substring(8, 1) == "1")
            {
                this.CheckB4.IsChecked = true;
            }
            else
            {
                this.CheckB4.IsChecked = false;
            }
            this.TextQM.Text = Display[0].ParamValue.Substring(9, 4);
            this.TextReply.Text = Display[1].ParamValue.Substring(9, 4);
            this.QMTime3.Text = Display[2].ParamValue.Substring(9, 4);
            this.QMTime4.Text = Display[3].ParamValue.Substring(9, 4);
            ChangeLanguage();
        }

        //31010402(c003) 1 (被勾选) 0024(时间) 1(时间单位，默认--小时 1)
        public bool CollectionData()
        {
            if (string.IsNullOrWhiteSpace(TextQM.Text.Trim()) || string.IsNullOrWhiteSpace(TextReply.Text.Trim()) || string.IsNullOrWhiteSpace(QMTime3.Text.Trim()) || string.IsNullOrWhiteSpace(QMTime4.Text.Trim()))
            { return false; }
            int hour1 = int.Parse(this.TextQM.Text);
            int hour2 = int.Parse(this.TextReply.Text);
            int hour3 = int.Parse(this.QMTime3.Text);
            int hour4 = int.Parse(this.QMTime4.Text);
            if (hour1 > 9999 || hour2 > 9999 || hour3 > 9999 || hour4 > 9999 || hour1 < 0 || hour2 < 0 || hour3 < 0 || hour4 < 0) { ShowInformation(CurrentApp.GetLanguageInfo("", "设置时间不符合要求")); return false; }

            string Strhour1 = hour1.ToString();
            string Strhour2 = hour2.ToString();
            string Strhour3 = hour3.ToString();
            string Strhour4 = hour4.ToString();
            if (Strhour1.Length != 4)
            {
                int zeronum = 4 - Strhour1.Length;
                for (int i = 0; i < zeronum; i++)
                    Strhour1 = "0" + Strhour1;
            }
            if (Strhour2.Length != 4)
            {
                int zeronum = 4 - Strhour2.Length;
                for (int i = 0; i < zeronum; i++)
                    Strhour2 = "0" + Strhour2;
            }
            if (Strhour3.Length != 4)
            {
                int zeronum = 4 - Strhour3.Length;
                for (int i = 0; i < zeronum; i++)
                    Strhour3 = "0" + Strhour3;
            }
            if (Strhour4.Length != 4)
            {
                int zeronum = 4 - Strhour4.Length;
                for (int i = 0; i < zeronum; i++)
                    Strhour4 = "0" + Strhour4;
            }

            if (this.CheckB1.IsChecked == true)
            {
                Display[0].ParamValue = Display[0].ParamValue.Substring(0, 8) + "1";
            }
            else
            {
                Display[0].ParamValue = Display[0].ParamValue.Substring(0, 8) + "0";
            }
            Display[0].ModifyTime = DateTime.Now.ToUniversalTime().ToString();
            Display[0].ModifyMan = CurrentApp.Session.UserID;
            Display[0].ParamValue += Strhour1 + "1"; 

            if (this.CheckB2.IsChecked == true)
            {
                Display[1].ParamValue = Display[1].ParamValue.Substring(0, 8) + "1";
            }
            else
            {
                Display[1].ParamValue = Display[1].ParamValue.Substring(0, 8) + "0";
            }
            Display[1].ModifyTime = DateTime.Now.ToUniversalTime().ToString();
            Display[1].ModifyMan = CurrentApp.Session.UserID;
            Display[1].ParamValue += Strhour2 + "1";

            if (this.CheckB3.IsChecked == true)
            {
                Display[2].ParamValue = Display[2].ParamValue.Substring(0, 8) + "1";
            }
            else
            {
                Display[2].ParamValue = Display[2].ParamValue.Substring(0, 8) + "0";
            }
            Display[2].ModifyTime = DateTime.Now.ToUniversalTime().ToString();
            Display[2].ModifyMan = CurrentApp.Session.UserID;
            Display[2].ParamValue += Strhour3 + "1";

            if (this.CheckB4.IsChecked == true)
            {
                Display[3].ParamValue = Display[3].ParamValue.Substring(0, 8) + "1";
            }
            else
            {
                Display[3].ParamValue = Display[3].ParamValue.Substring(0, 8) + "0";
            }
            Display[3].ModifyTime = DateTime.Now.ToUniversalTime().ToString();
            Display[3].ModifyMan = CurrentApp.Session.UserID;
            Display[3].ParamValue += Strhour4 + "1";
            return true;
        }

        public override void ChangeLanguage()
        {
            this.TxtbParam1.Text = Display[0].Name;
            this.TxtbParam2.Text = Display[1].Name;
            TxtbParam3.Text = Display[2].Name;
            TxtbParam4.Text = Display[3].Name;
        }

    }
}
