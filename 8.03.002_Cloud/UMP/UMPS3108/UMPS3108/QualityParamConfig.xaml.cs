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
    /// QualityParamConfig.xaml 的交互逻辑
    /// </summary>
    public partial class QualityParamConfig
    {
        #region menber

        public List<QualityParam> Display;
        public SCMainView mainPage;

        #endregion

        public QualityParamConfig()
        {
            InitializeComponent();

        }

        public void SetCheck()
        {
            if (Display.Count() < 3)
            {
                CheckB3.Visibility = Visibility.Collapsed;
                TxtbParam3.Visibility = Visibility.Collapsed;
                CheckB4.Visibility = Visibility.Collapsed;
                TxtbParam4.Visibility = Visibility.Collapsed;
            }
            if (Display.Count() >= 3)
            {
                CheckB3.Visibility = Visibility.Visible;
                TxtbParam3.Visibility = Visibility.Visible;
                CheckB4.Visibility = Visibility.Collapsed;
                TxtbParam4.Visibility = Visibility.Collapsed;
                if (Display[2].ParamValue.Substring(8) == "1")
                {
                    this.CheckB3.IsChecked = true;
                }
                else
                {
                    this.CheckB3.IsChecked = false;
                }
            }
             if (Display.Count() == 4)
            {
                CheckB4.Visibility = Visibility.Visible;
                TxtbParam4.Visibility = Visibility.Visible;
                if (Display[3].ParamValue.Substring(8) == "1")
                {
                    this.CheckB4.IsChecked = true;
                }
                else
                {
                    this.CheckB4.IsChecked = false;
                }
            }
            if (Display[0].ParamValue.Substring(8) == "1")
            {
                this.CheckB1.IsChecked = true;
            }
            else
            {
                this.CheckB1.IsChecked = false;
            }
            if (Display[1].ParamValue.Substring(8) == "1")
            {
                this.CheckB2.IsChecked = true;
            }
            else
            {
                this.CheckB2.IsChecked = false;
            }
            ChangeLanguage();
        }
        //public void LanguageChange()
        //{
        //    this.TxtbParam1.Text = Display1.Name;
        //    this.TxtbParam2.Text = Display2.Name;
        //}

        public void CollectionData()
        {
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

            if (Display.Count() >= 3)
            {
                if (CheckB3.IsChecked == true)
                {
                    Display[2].ParamValue = Display[2].ParamValue.Substring(0, 8) + "1";
                }
                else
                {
                    Display[2].ParamValue = Display[2].ParamValue.Substring(0, 8) + "0";
                }
                Display[2].ModifyTime = DateTime.Now.ToUniversalTime().ToString();
                Display[2].ModifyMan = CurrentApp.Session.UserID;
            }

            if (Display.Count() == 4)
            {
                if (CheckB4.IsChecked == true)
                {
                    Display[3].ParamValue = Display[3].ParamValue.Substring(0, 8) + "1";
                }
                else
                {
                    Display[3].ParamValue = Display[3].ParamValue.Substring(0, 8) + "0";
                }
                Display[3].ModifyTime = DateTime.Now.ToUniversalTime().ToString();
                Display[3].ModifyMan = CurrentApp.Session.UserID;
            }
        }

        public override void ChangeLanguage()
        {
            this.TxtbParam1.Text = Display[0].Name;
            this.TxtbParam2.Text = Display[1].Name;
            if (Display.Count() >= 3)
            {
                TxtbParam3.Text = Display[2].Name;
            }
            if (Display.Count() == 4)
            {
                TxtbParam4.Text = Display[3].Name;
            }
        }
    }
}
