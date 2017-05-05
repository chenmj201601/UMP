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
using UMPServicePack.PublicClasses;
using VoiceCyber.Common;
using UMPServicePackCommon;
using System.ComponentModel;

namespace UMPServicePack.UserControls
{
    /// <summary>
    /// UC_Done.xaml 的交互逻辑
    /// </summary>
    public partial class UC_Done : UserControl
    {
        public bool bIsSuccess = false;
        public MainWindow main = null;
        BackgroundWorker mBackgroundWorker = null;
        bool bIsloaded = false;

        public UC_Done()
        {
            InitializeComponent();
            Loaded += UC_Done_Loaded;
        }

        void UC_Done_Loaded(object sender, RoutedEventArgs e)
        {
            btnExit.Click += btnExit_Click;
            if (bIsSuccess)
            {
                App.bIsUpgrageSuccess = true;
                lblResult.Content = App.GetLanguage("string49", "string49");
                //将更新后的版本号写入数据库
                OperationReturn optReturn = DatabaseOperator.WriteVersionToDB();
                if (!optReturn.Result)
                {
                    App.WriteLog("Error code :" + optReturn.Code + ". WriteVersionToDB failed. " + optReturn.Message);
                }
                else
                {
                    App.WriteLog("WriteVersionToDB success");
                }
                bIsloaded = true;

                 mBackgroundWorker = new BackgroundWorker();
                 mBackgroundWorker.DoWork += (s, de) =>
                     {
                         while (true)
                         {
                             if (bIsloaded)
                             {
                                 //FOLLOW
                                 Follow();
                                 break;
                             }
                         }
                     };
                 mBackgroundWorker.RunWorkerCompleted += mBackgroundWorker_RunWorkerCompleted;
                 mBackgroundWorker.RunWorkerAsync();
            }
            else
            {
                lblResult.Content = App.GetLanguage("string51","string51");
            }
            InitUpdateResultInfo(bIsSuccess);

        }

        void mBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            mBackgroundWorker.Dispose();
        }

        void btnExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        /// <summary>
        /// 加载升级结果的详细信息
        /// </summary>
        void InitUpdateResultInfo(bool bResult)
        {
            TextBlock txt = new TextBlock();
            txt.TextWrapping = TextWrapping.Wrap;
            txt.HorizontalAlignment = HorizontalAlignment.Stretch;
            txt.VerticalAlignment = VerticalAlignment.Stretch;
            spResult.Children.Add(txt);
            if (bResult)
            {
                string text = App.GetLanguage("string50", "string50");
                text = string.Format(text, App.updateInfo.Version);
            }
            else
            {
                txt.Text = App.GetLanguage("string51", "string51");
            }
        }

        private OperationReturn Follow()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                List<UpdateFollow> lstFollows = App.updateInfo.ListFollows;
                if (lstFollows.Count > 0)
                {
                    foreach (UpdateFollow item in lstFollows)
                    {
                        switch (item.Key)
                        {
                            case 101:
                                if (App.dicAppInstalled.Keys.Contains(ConstDefines.UMP))
                                {
                                    optReturn = CommonFuncs.StartMAMT(App.dicAppInstalled[ConstDefines.UMP].AppInstallPath);
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return optReturn;
            }
            return optReturn;
        }

    }
}
