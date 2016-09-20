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

namespace UMPS6101.SharingClasses
{
    /// <summary>
    /// UserControlDatepick.xaml 的交互逻辑
    /// </summary>
    public partial class UserControlDatepick : UserControl
    {
        public UserControlDatepick()
        {
            InitializeComponent();
        }

        public void InitDateTime(string as_datetime)
        {
            try
            {
                System.Windows.Forms.DateTimePicker DateControl = DateFormsHostBegin.Child as System.Windows.Forms.DateTimePicker;
                System.Windows.Forms.DateTimePicker TimeControl = TimeFormsHostBegin.Child as System.Windows.Forms.DateTimePicker;
                if (as_datetime == string.Empty || as_datetime.Length == 0) { as_datetime = DateTime.Now.ToString(); }
                DateControl.Value = Convert.ToDateTime(as_datetime);
                TimeControl.Value = Convert.ToDateTime(as_datetime);
            }
            catch (Exception ex)
            {
                //ShowExceptionMessage(ex.Message);
            }
        }

        public string GetDateTimeAsString()
        {
            string ls_return = string.Empty;
            string ls_date, ls_time;
            try
            {
                ls_date = ((System.Windows.Forms.DateTimePicker)DateFormsHostBegin.Child).Value.ToString("yyyy-MM-dd");
                ls_time = ((System.Windows.Forms.DateTimePicker)TimeFormsHostBegin.Child).Value.ToString("HH:mm:ss");
                ls_return = ls_date + " " + ls_time;
            }
            catch (Exception ex)
            {
                ls_return = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //ShowExceptionMessage(ex.Message);
            }

            return ls_return;
        }
    }
}
