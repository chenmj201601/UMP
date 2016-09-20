using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Updates;

namespace LoggingUpdater
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private List<string> mListProductGUIDs;
        private List<InstallProduct> mListProductInfos;
        private bool mIsInited;
        private InstallInfo mInstallInfo;

        private ObservableCollection<ProductItem> mListProductItems; 

        public MainWindow()
        {
            InitializeComponent();

            mListProductGUIDs = new List<string>();
            mListProductInfos = new List<InstallProduct>();

            mListProductItems = new ObservableCollection<ProductItem>();

            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        private void Init()
        {
            try
            {
                ListInstalledProducts.ItemsSource = mListProductItems;

                InitProductGUIDs();
                LoadInstallInfo();
                InitProductInfos();

                CreateProductItems();

                WSearchPackage win = new WSearchPackage();
                win.PageParent = this;
                var result = win.ShowDialog();
                if (result != true) { return; }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitProductGUIDs()
        {
            try
            {
                mListProductGUIDs.Clear();
                mListProductGUIDs.Add(UpdateConsts.PACKAGE_GUID_ALARMSERVER);
                mListProductGUIDs.Add(UpdateConsts.PACKAGE_GUID_LICENSESERVER);
                mListProductGUIDs.Add(UpdateConsts.PACKAGE_GUID_CMSERVER);
                mListProductGUIDs.Add(UpdateConsts.PACKAGE_GUID_CTIHUB);
                mListProductGUIDs.Add(UpdateConsts.PACKAGE_GUID_DEC);
                mListProductGUIDs.Add(UpdateConsts.PACKAGE_GUID_SCREENSERVER);
                mListProductGUIDs.Add(UpdateConsts.PACKAGE_GUID_SOFTRECORD);
                mListProductGUIDs.Add(UpdateConsts.PACKAGE_GUID_SPEECHANALYSIS);
                mListProductGUIDs.Add(UpdateConsts.PACKAGE_GUID_IXPATCH);
                mListProductGUIDs.Add(UpdateConsts.PACKAGE_GUID_WEBSDK);
                mListProductGUIDs.Add(UpdateConsts.PACKAGE_GUID_ASM);
                mListProductGUIDs.Add(UpdateConsts.PACKAGE_GUID_SFTP);
                mListProductGUIDs.Add(UpdateConsts.PACKAGE_GUID_VOICE);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadInstallInfo()
        {
            try
            {
                string strPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    ConstValue.VCT_COMPANY_SHORTNAME, ConstValue.UMP_PRODUCTER_SHORTNAME);
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                string strFile = Path.Combine(strPath, InstallInfo.FILE_NAME);
                if (!File.Exists(strFile))
                {
                    App.WriteLog("LoadInstallInfo", string.Format("InstallInfo file not exist.\t{0}", strFile));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<InstallInfo>(strFile);
                if (!optReturn.Result)
                {
                    App.WriteLog("LoadInstallInfo",
                        string.Format("InstallInfo file invalid.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                InstallInfo installInfo = optReturn.Data as InstallInfo;
                if (installInfo == null)
                {
                    App.WriteLog("LoadInstallInfo", string.Format("InstallInfo is null"));
                    return;
                }
                mInstallInfo = installInfo;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitProductInfos()
        {
            try
            {
                mListProductInfos.Clear();
                if (mInstallInfo == null)
                {
                    mInstallInfo = new InstallInfo();
                    //从注册表获取已经安装的产品
                    for (int i = 0; i < mListProductGUIDs.Count; i++)
                    {
                        string strGUID = mListProductGUIDs[i];
                        string path;
                        if (Environment.Is64BitOperatingSystem)
                        {
                            path = string.Format(
                                @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{0}", strGUID);
                        }
                        else
                        {
                            path = string.Format(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{0}", strGUID);
                        }
                        try
                        {
                            RegistryKey rootKey = Registry.LocalMachine;
                            RegistryKey umpKey = rootKey.OpenSubKey(path);
                            if (umpKey != null)
                            {
                                InstallProduct product = new InstallProduct();
                                product.ProductGuid = strGUID;
                                product.ProductName = umpKey.GetValue("DisplayName").ToString();
                                product.DisplayName = umpKey.GetValue("DisplayName").ToString();
                                product.InstallPath = umpKey.GetValue("InstallLocation").ToString();
                                product.Version = umpKey.GetValue("DisplayVersion").ToString();
                                mInstallInfo.ListProducts.Add(product);
                            }
                        }
                        catch (Exception ex)
                        {
                            App.WriteLog("InitProductInfos", string.Format("Get product info fail.\t{0}", ex.Message));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateProductItems()
        {
            try
            {
                mListProductItems.Clear();
                if (mInstallInfo == null) { return;}
                for (int i = 0; i < mInstallInfo.ListProducts.Count; i++)
                {
                    var product = mInstallInfo.ListProducts[i];
                    ProductItem item=new ProductItem();
                    item.Name = product.ProductName;
                    item.Info = product;
                    mListProductItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ShowException(string msg)
        {
            MessageBox.Show(msg, App.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
