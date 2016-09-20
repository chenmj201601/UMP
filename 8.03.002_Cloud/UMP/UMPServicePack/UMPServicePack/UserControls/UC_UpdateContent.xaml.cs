using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using UMPServicePackCommon;
using VoiceCyber.Common;

namespace UMPServicePack.UserControls
{
    /// <summary>
    /// UC_UpdateContent.xaml 的交互逻辑
    /// </summary>
    public partial class UC_UpdateContent : UserControl
    {
        public MainWindow main = null;
        BackgroundWorker mBackgroundWorker = null;

        public UC_UpdateContent()
        {
            InitializeComponent();
            Loaded += UC_UpdateContent_Loaded;
        }

        #region 控件事件
        void UC_UpdateContent_Loaded(object sender, RoutedEventArgs e)
        {
            #region 关联事件
            btnUpgrade.Click += btnUpgrade_Click;
            btnTermination.Click += btnTermination_Click;
            #endregion

            #region Init

            mBackgroundWorker = new BackgroundWorker();
            mBackgroundWorker.DoWork += mBackgroundWorker_DoWork;
            mBackgroundWorker.RunWorkerAsync();

            #endregion

        }

        void mBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            InitUpdateContent();
        }

        void btnTermination_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        void btnUpgrade_Click(object sender, RoutedEventArgs e)
        {
            //判断是否可以升级
            if (string.Compare(App.gStrLastVersion, App.updateInfo.Version) > 0)
            {
                App.ShowException(App.GetLanguage("string22", "string22"));
                App.WriteLog("Has been installed above the patch package.");
                return;
            }
            //再次弹框确认升级 
            MessageBoxResult result = MessageBox.Show(App.GetLanguage("string31", "string31"), "UMP Service Pack",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                //进入许可协议界面
                UC_License uc_License = new UC_License();
                main.borderUpdater.Child = uc_License;
                uc_License.main = main;
            }
        }

        #endregion

        #region Init
        //此函数公开 以便在切换语言时调用
        public void InitUpdateContent()
        {
            Dispatcher.Invoke(new Action(() =>
                {
                    main.SetBusy(true);
                    splContent.Children.Clear();
                    List<UpdateModule> lstModules = App.updateInfo.ListModules;

                    for (int i = 0; i < App.lstAllModules.Count; i++)
                    {
                        string strModuleID = App.lstAllModules[i];
                        List<UpdateModule> lstModulesByID = lstModules.Where(p => p.ModuleID == strModuleID).ToList();
                        if (lstModulesByID.Count > 0)
                        {
                            Label lblModule = new Label();
                            lblModule.Content = App.GetLanguage(lstModulesByID[0].ModuleLangID, lstModulesByID[0].ModuleName);
                            lblModule.Style = this.FindResource("LabelInContent") as Style;
                            splContent.Children.Add(lblModule);

                            for (int j = 0; j < lstModulesByID.Count; j++)
                            {
                                DockPanel dpanel = CreateContentLabel(lstModulesByID[j]);
                                splContent.Children.Add(dpanel);
                            }
                        }
                    }
                    InitServiceDepend();
                    main.SetBusy(false);
                }));

        }

        private DockPanel CreateContentLabel(UpdateModule module)
        {
            DockPanel dpanel = null;
            Dispatcher.Invoke(new Action(() =>
              {
                  dpanel = new DockPanel();
                  dpanel.Style = this.FindResource("DockPanelInUpdateContent") as Style;
                  DockPanel panImages = new DockPanel();
                  panImages.Width = 120;
                  panImages.HorizontalAlignment = HorizontalAlignment.Right;
                  panImages.SetValue(DockPanel.DockProperty, Dock.Right);
                  dpanel.Children.Add(panImages);

                  string strVersion = string.Empty;
                  try
                  {
                      strVersion = module.SerialNo.Substring(0, 12);
                      string str1 = strVersion.Substring(0, 1);
                      string str2 = strVersion.Substring(1, 2);
                      string str3 = strVersion.Substring(3, 3);
                      string str4 = strVersion.Substring(6, 3);
                      string str5 = strVersion.Substring(9);
                      strVersion = str1 + "." + str2 + "." + str3 + "." + str4 + "." + str5;
                  }
                  catch { }

                  TextBlock lbl = new TextBlock();
                  if (string.IsNullOrEmpty(strVersion))
                  {
                      lbl.Text = App.GetLanguage(module.SerialNo, module.Content);
                  }
                  else
                  {
                      lbl.Text = App.GetLanguage(module.SerialNo, module.Content) + " [" + strVersion + "]";
                  }
                  lbl.HorizontalAlignment = HorizontalAlignment.Left;
                  lbl.VerticalAlignment = VerticalAlignment.Top;
                  lbl.TextWrapping = TextWrapping.Wrap;
                  dpanel.Children.Add(lbl);
                  lbl.SetValue(DockPanel.DockProperty, Dock.Left);

                  Image img = null;

                  for (int i = 0; i < module.Level; i++)
                  {
                      img = new Image();
                      img.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/Level.ico", UriKind.Absolute));
                      img.HorizontalAlignment = HorizontalAlignment.Left;
                      img.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                      img.Width = 12;
                      img.Height = 12;

                      if (i == module.Level - 1)
                      {
                          img.Margin = new Thickness(5, 0, 15, 0);
                      }
                      else
                      {
                          img.Margin = new Thickness(5, 0, 0, 0);
                      }
                      panImages.Children.Add(img);
                  }

              }));
            return dpanel;
        }

        /// <summary>
        /// 加载受影响的服务
        /// </summary>
        private void InitServiceDepend()
        {
            Dispatcher.Invoke(new Action(() =>
             {
                 UpdateInfo upInfo = App.updateInfo;
                 if (upInfo.ListServices.Count > 0)
                 {
                     Label lblServices = new Label();
                     lblServices.Content = App.GetLanguage("string30", "string30");
                     lblServices.Style = this.FindResource("LabelInContent") as Style;
                     splContent.Children.Add(lblServices);
                     for (int i = 0; i < upInfo.ListServices.Count; i++)
                     {
                         Label lblService = new Label();
                         lblService.Content = upInfo.ListServices[i].ServiceName;
                         lblService.Margin = new Thickness(40, 0, 0, 0);
                         lblService.HorizontalAlignment = HorizontalAlignment.Left;
                         lblService.VerticalAlignment = VerticalAlignment.Top;
                         lblService.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                         lblService.Foreground = Brushes.OrangeRed;
                         splContent.Children.Add(lblService);
                     }
                 }
             }));
        }
        #endregion

    }
}
