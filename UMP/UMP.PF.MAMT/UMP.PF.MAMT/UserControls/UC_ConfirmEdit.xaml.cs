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
using UMP.PF.MAMT.Classes;
using UMP.PF.MAMT.WCF_LanPackOperation;
using System.Data;

namespace UMP.PF.MAMT.UserControls
{
    /// <summary>
    /// UC_ConfirmEdit.xaml 的交互逻辑
    /// </summary>
    public partial class UC_ConfirmEdit : UserControl
    {
        public UC_LanManager LanManagerWindow;

        public UC_ConfirmEdit(UC_LanManager _main)
        {
            InitializeComponent();
            LanManagerWindow = _main;
            this.Loaded += UC_ConfirmEdit_Loaded;
        }

        void UC_ConfirmEdit_Loaded(object sender, RoutedEventArgs e)
        {
            imgSave.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000024.ico"), UriKind.RelativeOrAbsolute));
            imgOperation.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000018.ico"), UriKind.RelativeOrAbsolute));
            imgCancel.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000026.ico"), UriKind.RelativeOrAbsolute));
            dpSave.MouseLeftButtonDown += dpSave_MouseLeftButtonDown;
            dpCancel.MouseLeftButtonDown += dpCancel_MouseLeftButtonDown;
        }

        /// <summary>
        /// 取消编辑语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dpCancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LanManagerWindow.dpDetail.Children.Clear();
            LanManagerWindow.spOperator.Children.RemoveAt(1);
            UC_DBOpeartionDefault defaultWin2 = new UC_DBOpeartionDefault(LanManagerWindow);
            LanManagerWindow.spOperator.Children.Add(defaultWin2);
            UC_LanguageInfo lanInfo = new UC_LanguageInfo(App.GLstLanguageItemInEdit, LanManagerWindow);
            LanManagerWindow.dpDetail.Children.Add(lanInfo);
        }

        /// <summary>
        /// 保存编辑语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dpSave_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            List<string> lstParams;
            List<LanguageInfo> lstLans = GetLanguagesEdited();
            ReturnResult result = null;
            for (int i = 0; i < lstLans.Count; i++)
            {
                lstParams = new List<string>();
                lstParams.Add(App.GCurrentDBServer.DbType.ToString());
                lstParams.Add(App.GCurrentDBServer.Host);
                lstParams.Add(App.GCurrentDBServer.Port);
                lstParams.Add(App.GCurrentDBServer.ServiceName);
                lstParams.Add(App.GCurrentDBServer.LoginName);
                lstParams.Add(App.GCurrentDBServer.Password);
                lstParams.Add(lstLans[i].LanguageCode);
                lstParams.Add(lstLans[i].MessageID);
                if (lstLans[i].DisplayMessage.Length > 1024)
                {
                    lstParams.Add(lstLans[i].DisplayMessage.Substring(0, 1024));
                    lstParams.Add(lstLans[i].DisplayMessage.Substring(1024));
                }
                else
                {
                    lstParams.Add(lstLans[i].DisplayMessage);
                    lstParams.Add(string.Empty);
                }
                if (lstLans[i].TipMessage.Length > 1024)
                {
                    lstParams.Add(lstLans[i].TipMessage.Substring(0, 1024));
                    lstParams.Add(lstLans[i].TipMessage.Substring(1024));
                }
                else
                {
                    lstParams.Add(lstLans[i].TipMessage);
                    lstParams.Add(string.Empty);
                }

                result = AboutLanguagesInServer.WCFOperationMthodA("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port
                    , 3, lstParams);
                if (!result.BoolReturn)
                {
                    MessageBox.Show(lstLans[i].LanguageCode+this.TryFindResource("SaveLanguageItemError").ToString(),
                    this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    UC_LanguageInfo uc_Info = new UC_LanguageInfo(lstLans, LanManagerWindow);
                    LanManagerWindow.dpDetail.Children.Clear();
                    LanManagerWindow.dpDetail.Children.Add(uc_Info);

                    LanManagerWindow.spOperator.Children.RemoveAt(1);
                    UC_DBOpeartionDefault uc_Default = new UC_DBOpeartionDefault(LanManagerWindow);
                    LanManagerWindow.spOperator.Children.Add(uc_Default);
                }
            }

            TreeView tree = LanManagerWindow.spContent.Children[2] as TreeView;
            TreeViewItem item = tree.SelectedItem as TreeViewItem;
            string strContextValue = item.DataContext.ToString();
            string strSelectedLanCode = string.Empty;
            if (strContextValue.StartsWith("Lan-"))
            {
                DataRow row = item.Tag as DataRow;
                strSelectedLanCode = row["C002"].ToString();
            }
            UC_LanguageData data = LanManagerWindow.dpData.Children[0] as UC_LanguageData;
            Common.RefershData(data.lvLanguage,strSelectedLanCode);
            data.lstViewrSource = data.lvLanguage.ItemsSource as List<LanguageInfo>;
            List<LanguageInfo> lstSingleLans = null;
            if (!string.IsNullOrEmpty(strSelectedLanCode))
            {
                lstSingleLans = data.lstViewrSource.Where(p => p.LanguageCode == strSelectedLanCode).ToList();
                data.lvLanguage.ItemsSource=null;
                data.lvLanguage.ItemsSource = lstSingleLans;
            }
        }

        /// <summary>
        /// 将修改完成的语言整理成languageinfo类对象 便于传入wcf进行更新
        /// </summary>
        /// <returns></returns>
        List<LanguageInfo> GetLanguagesEdited()
        {
            List<LanguageInfo> lstEditedLans = new List<LanguageInfo>();
            UC_EditLanguageItem uc_EditLan = LanManagerWindow.dpDetail.Children[0] as UC_EditLanguageItem;
            if (uc_EditLan.spDisplayContent.Children.Count > 0)
            {
                TextBox txtDisPlay = null;
                LanguageInfo lan = null;
                TextBox txtTip = null;
                StackPanel spDis = null;
                for (int i = 0; i < uc_EditLan.spDisplayContent.Children.Count; i++)
                {
                    spDis = uc_EditLan.spDisplayContent.Children[i] as StackPanel;
                    if (spDis.Children.Count > 0)
                    {
                        for (int iChild = 0; iChild < spDis.Children.Count; iChild++)
                        {
                            try
                            {
                                txtDisPlay = spDis.Children[iChild] as TextBox;
                                lan = new LanguageInfo();
                                lan.ChildModuleID = uc_EditLan.lblChildModuleID.Content.ToString();
                                lan.DisplayMessage = txtDisPlay.Text;
                                lan.InFrame = uc_EditLan.lblInPageFrame.Content.ToString();
                                lan.InObject = null;
                                lan.LanguageCode = txtDisPlay.Tag.ToString();
                                lan.MessageID = uc_EditLan.lblMessageID.Content.ToString();
                                lan.ModuleID = uc_EditLan.lblModuleID.Content.ToString();
                                //遍历spTipContet 根据lan.LanguageCode加载提示信息
                                StackPanel spTip = null;
                                for (int j = 0; j < uc_EditLan.spTipContent.Children.Count; j++)
                                {
                                    spTip = uc_EditLan.spTipContent.Children[j] as StackPanel;
                                    if (spTip.Children.Count > 0)
                                    {
                                        for (int jChild = 0; jChild < spTip.Children.Count; jChild++)
                                        {
                                            try
                                            {
                                                txtTip = spTip.Children[jChild] as TextBox;
                                                if (txtTip.Tag.ToString().Equals(lan.LanguageCode))
                                                {
                                                    lan.TipMessage = txtTip.Text;
                                                }
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                        }
                                    }

                                }
                                lstEditedLans.Add(lan);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            return lstEditedLans;
        }
    }
}
