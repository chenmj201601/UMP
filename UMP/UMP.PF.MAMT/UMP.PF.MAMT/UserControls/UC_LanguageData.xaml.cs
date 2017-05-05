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
using System.Data;
using UMP.PF.MAMT.Classes;
using UMP.PF.MAMT.WCF_LanPackOperation;
using System.ComponentModel;

namespace UMP.PF.MAMT.UserControls
{
    /// <summary>
    /// UC_LanguageData.xaml 的交互逻辑
    /// </summary>
    public partial class UC_LanguageData : UserControl
    {
        public DataTable dtAllLanguage;
        public UC_LanManager LanManagerWindow;
        public List<LanguageInfo> lstViewrSource;

        public UC_LanguageData(DataTable dtLan,UC_LanManager _mainWin)
        {
            InitializeComponent();
            dtAllLanguage = dtLan;
            LanManagerWindow = _mainWin;
            this.Loaded += UC_LanguageData_Loaded;
        }

        void UC_LanguageData_Loaded(object sender, RoutedEventArgs e)
        {
            lstViewrSource = new List<LanguageInfo>();
            lstViewrSource = Common.ConvertDataTableToLanInfoList(dtAllLanguage);
            lvLanguage.ItemsSource = lstViewrSource;
            lvLanguage.SelectionChanged += lvLanguage_SelectionChanged;
        }

        void lvLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            #region 准备数据
            LanguageInfo row = lvLanguage.SelectedItem as LanguageInfo;
            if (row == null)
            {
                return;
            }
            string strMessageId = row.MessageID;

            List<LanguageInfo> lstLans = lstViewrSource.Where(p => p.MessageID == strMessageId).ToList();
            App.GLstLanguageItemInEdit.Clear();
            App.GLstLanguageItemInEdit = lstLans;

            #endregion
            #region 加载控件
            UC_LanguageInfo uc_Lan;
            if (LanManagerWindow.dpDetail.Children.Count > 0)
            {
                try
                {
                    //如果dpDetail中加载的是UC_LanguageInfo控件 则直接填充控件 而不New一个新的UC_LanguageInfo
                    uc_Lan = LanManagerWindow.dpDetail.Children[0] as UC_LanguageInfo;
                    uc_Lan.lstLanguageInfos = lstLans;
                    uc_Lan.InitControl();
                }
                catch
                {
                    //反之 则new一个新的 加载到dpDetail
                    uc_Lan = new UC_LanguageInfo(lstLans,LanManagerWindow);
                    LanManagerWindow.dpDetail.Children.Clear();
                    LanManagerWindow.dpDetail.Children.Add(uc_Lan);
                }
                LanManagerWindow.spOperator.Children.RemoveAt(1);
                UC_DBOpeartionDefault edit = new UC_DBOpeartionDefault(LanManagerWindow);
                LanManagerWindow.spOperator.Children.Add(edit);
            }
            else
            {
                uc_Lan = new UC_LanguageInfo(lstLans,LanManagerWindow);
                LanManagerWindow.dpDetail.Children.Clear();
                LanManagerWindow.dpDetail.Children.Add(uc_Lan);
            }
            #endregion
        }

        //public void Refresh()
        //{
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        DBInfo dbInfo = App.GCurrentDBServer;
        //        ReturnResult result = AboutLanguagesInServer.WCFOperationMthodA("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port, 2, dbInfo);
        //        if (result.BoolReturn)
        //        {
        //            if (result.DataSetReturn.Tables.Count > 0)
        //            {
        //                lvLanguage.ItemsSource = result.DataSetReturn.Tables[0].DefaultView;

        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show(result.StringReturn,
        //               this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message,
        //               this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}
    }
}
