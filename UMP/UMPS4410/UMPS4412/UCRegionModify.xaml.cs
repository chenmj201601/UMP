//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    543906e9-d6c9-4f25-b541-246c0e471394
//        CLR Version:              4.0.30319.18408
//        Name:                     UCRegionModify
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4412
//        File Name:                UCRegionModify
//
//        created by Charley at 2016/5/11 10:18:23
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using UMPS4412.Models;
using UMPS4412.Wcf11012;
using UMPS4412.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4412
{
    /// <summary>
    /// UCRegionModify.xaml 的交互逻辑
    /// </summary>
    public partial class UCRegionModify
    {

        #region Members

        public RegionManageMainView PageParent;
        public bool IsModify;
        public ObjItem RegionItem;

        private bool mIsInited;
        private ObservableCollection<RegionTypeItem> mListRegionTypes;
        private string mBgFileName;

        #endregion


        public UCRegionModify()
        {
            InitializeComponent();

            mListRegionTypes = new ObservableCollection<RegionTypeItem>();

            Loaded += UCRegionModify_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            BtnBrowseImage.Click += BtnBrowseImage_Click;
        }

        void UCRegionModify_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                ComboRegionTypes.ItemsSource = mListRegionTypes;
                InitRegionTypes();

                mBgFileName = string.Empty;
                if (RegionItem == null) { return; }
                if (IsModify)
                {
                    var info = RegionItem.Data as RegionInfo;
                    if (info == null) { return; }
                    TxtRegionName.Text = info.Name;
                    ComboRegionTypes.SelectedItem = mListRegionTypes.FirstOrDefault(r => r.Value == info.Type);
                    RadioStateEnable.IsChecked = info.State == 1;
                    RadioStateDisable.IsChecked = info.State == 0;
                    RadioDefaultYes.IsChecked = info.IsDefault;
                    RadioDefaultNo.IsChecked = !info.IsDefault;
                    TxtRegionWidth.Value = info.Width;
                    TxtRegionHeight.Value = info.Height;
                    TxtBgColor.SelectedColor = GetColorFromString(info.BgColor);
                    string strBgImage = info.BgImage;
                    if (!string.IsNullOrEmpty(strBgImage))
                    {
                        string strUrl = string.Format("{0}://{1}:{2}/{3}/UMPS4412/{4}",
                            CurrentApp.Session.AppServerInfo.Protocol,
                            CurrentApp.Session.AppServerInfo.Address,
                            CurrentApp.Session.AppServerInfo.Port,
                            ConstValue.TEMP_DIR_UPLOADFILES,
                            strBgImage);
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.UriSource = new Uri(strUrl, UriKind.Absolute);
                        image.EndInit();
                        ImageBgImage.Source = image;

                        Image imgTip = new Image();
                        imgTip.Source = new BitmapImage(new Uri(strUrl, UriKind.Absolute));
                        imgTip.Stretch = Stretch.Uniform;
                        ImageBgImage.ToolTip = imgTip;
                    }
                }
                else
                {
                    TxtRegionName.Text = string.Empty;
                    ComboRegionTypes.SelectedItem = mListRegionTypes.FirstOrDefault(r => r.Value == 0);
                    RadioStateEnable.IsChecked = true;
                    RadioStateDisable.IsChecked = false;
                    RadioDefaultYes.IsChecked = true;
                    RadioDefaultNo.IsChecked = false;
                    TxtRegionWidth.Value = 0;
                    TxtRegionHeight.Value = 0;
                    TxtBgColor.SelectedColor = Brushes.Transparent.Color;
                }

                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitRegionTypes()
        {
            try
            {
                mListRegionTypes.Clear();
                RegionTypeItem item = new RegionTypeItem();
                item.Name = "WorkRegion";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4412013{0}", 0.ToString("000")), "Work Region");
                item.Value = 0;
                mListRegionTypes.Add(item);
                item = new RegionTypeItem();
                item.Name = "PlaceRegion";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4412013{0}", 1.ToString("000")), "Place Region");
                item.Value = 1;
                mListRegionTypes.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void AddRegion()
        {
            try
            {
                RegionInfo regionInfo = new RegionInfo();
                if (RegionItem == null) { return; }
                var parentRegion = RegionItem.Data as RegionInfo;
                if (parentRegion == null) { return; }
                regionInfo.ParentObjID = parentRegion.ObjID;
                if (string.IsNullOrEmpty(TxtRegionName.Text))
                {
                    ShowException(CurrentApp.GetLanguageInfo("4412N003", string.Format("Region name can not be empty.")));
                    return;
                }
                regionInfo.Name = TxtRegionName.Text;
                var regionType = ComboRegionTypes.SelectedItem as RegionTypeItem;
                if (regionType == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("4412N004", string.Format("Region type invalid.")));
                    return;
                }
                regionInfo.Type = regionType.Value;
                regionInfo.State = RadioStateEnable.IsChecked == true ? 1 : 0;
                regionInfo.IsDefault = RadioDefaultYes.IsChecked == true;
                int intValue = 0;
                if (TxtRegionWidth.Value != null && TxtRegionWidth.Value > 0)
                {
                    intValue = (int)TxtRegionWidth.Value;
                }
                regionInfo.Width = intValue;
                intValue = 0;
                if (TxtRegionHeight.Value != null && TxtRegionHeight.Value > 0)
                {
                    intValue = (int)TxtRegionHeight.Value;
                }
                regionInfo.Height = intValue;
                regionInfo.BgColor = TxtBgColor.SelectedColor.ToString();
                string strBgFile = mBgFileName;
                regionInfo.BgImage = strBgFile;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(regionInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                string strInfo = optReturn.Data.ToString();

                bool isSuccess = false;
                string strMsg = string.Empty;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S4410Codes.SaveRegionInfo;
                        webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                        webRequest.ListData.Add("0");
                        webRequest.ListData.Add(strInfo);
                        webRequest.ListData.Add("1");       //需要拷贝背景图片
                        Service44101Client client =
                            new Service44101Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            strMsg = string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message);
                            return;
                        }

                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        strMsg = ex.Message;
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    try
                    {
                        if (isSuccess)
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("4412N005", string.Format("Add region end")));

                            var parent = Parent as PopupPanel;
                            if (parent != null)
                            {
                                parent.IsOpen = false;
                            }
                            if (PageParent != null)
                            {
                                RegionItem.IsExpanded = true;
                                PageParent.ReloadRegionInfo(RegionItem);
                            }
                        }
                        else
                        {
                            ShowException(strMsg);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ModifyRegion()
        {
            try
            {
                RegionInfo regionInfo = RegionItem.Data as RegionInfo;
                if (regionInfo == null) { return; }
                if (string.IsNullOrEmpty(TxtRegionName.Text))
                {
                    ShowException(CurrentApp.GetLanguageInfo("4412N003", string.Format("Region name can not be empty.")));
                    return;
                }
                regionInfo.Name = TxtRegionName.Text;
                var regionType = ComboRegionTypes.SelectedItem as RegionTypeItem;
                if (regionType == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("4412N004", string.Format("Region type invalid.")));
                    return;
                }
                regionInfo.Type = regionType.Value;
                regionInfo.State = RadioStateEnable.IsChecked == true ? 1 : 0;
                regionInfo.IsDefault = RadioDefaultYes.IsChecked == true;
                int intValue = 0;
                if (TxtRegionWidth.Value != null && TxtRegionWidth.Value > 0)
                {
                    intValue = (int)TxtRegionWidth.Value;
                }
                regionInfo.Width = intValue;
                intValue = 0;
                if (TxtRegionHeight.Value != null && TxtRegionHeight.Value > 0)
                {
                    intValue = (int)TxtRegionHeight.Value;
                }
                regionInfo.Height = intValue;
                bool isCopy = false;
                regionInfo.BgColor = TxtBgColor.SelectedColor.ToString();
                if (!string.IsNullOrEmpty(mBgFileName))
                {
                    regionInfo.BgImage = mBgFileName;
                    isCopy = true;
                }
                OperationReturn optReturn = XMLHelper.SeriallizeObject(regionInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                string strInfo = optReturn.Data.ToString();

                bool isSuccess = false;
                string strMsg = string.Empty;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S4410Codes.SaveRegionInfo;
                        webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                        webRequest.ListData.Add(regionInfo.ObjID.ToString());
                        webRequest.ListData.Add(strInfo);
                        webRequest.ListData.Add(isCopy ? "1" : "0");
                        Service44101Client client =
                            new Service44101Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            strMsg = string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message);
                            return;
                        }
                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        strMsg = ex.Message;
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    try
                    {
                        if (isSuccess)
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("4412N006", string.Format("Modify region end")));

                            var parent = Parent as PopupPanel;
                            if (parent != null)
                            {
                                parent.IsOpen = false;
                            }
                            if (PageParent != null)
                            {
                                var parentItem = RegionItem.Parent as ObjItem;
                                if (parentItem != null)
                                {
                                    parentItem.IsExpanded = true;
                                    PageParent.ReloadRegionInfo(parentItem);
                                }
                            }
                        }
                        else
                        {
                            ShowException(strMsg);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void BrowseBgImage()
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = string.Format("Select a image as region background");
                dialog.Multiselect = false;
                dialog.Filter = "Support Image|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                var result = dialog.ShowDialog();
                if (result != true) { return; }
                string strFile = dialog.FileName;
                if (!File.Exists(strFile)) { return; }


                #region 上传背景图片

                FileInfo fileInfo = new FileInfo(strFile);
                string strExt = fileInfo.Extension;
                strExt = strExt.TrimStart('.');
                if (fileInfo.Length > 1024 * 1024 * 5)
                {
                    //图片文件超过5M，不允许上传
                    ShowException(string.Format("Image file too big, can not upload."));
                    return;
                }
                int length = (int)fileInfo.Length;
                byte[] buffer = File.ReadAllBytes(strFile);
                UploadRequest request = new UploadRequest();
                request.Session = CurrentApp.Session;
                request.Code = (int)RequestCode.WSUploadFile;
                request.ListData.Add(length.ToString());
                request.ListData.Add("4412");
                request.ListData.Add("1");      //先上传到MediaData临时目录
                request.ListData.Add(string.Empty); //自动生成文件名
                request.ListData.Add(strExt);       //扩展名
                request.Content = buffer;
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.UploadOperation(request);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                string strTemp = webReturn.Data;
                if (string.IsNullOrEmpty(strTemp))
                {
                    ShowException(string.Format("Fail.\tFileName empty!"));
                    return;
                }
                mBgFileName = strTemp;

                CurrentApp.WriteLog("UploadBgImage", string.Format("End.\t{0}", mBgFileName));

                string strUrl = string.Format("{0}://{1}:{2}/{3}/{4}",
                    CurrentApp.Session.AppServerInfo.Protocol,
                    CurrentApp.Session.AppServerInfo.Address,
                    CurrentApp.Session.AppServerInfo.Port,
                    ConstValue.TEMP_DIR_MEDIADATA,
                    mBgFileName);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(strUrl, UriKind.Absolute);
                bitmap.EndInit();
                ImageBgImage.Source = bitmap;

                Image imgTip = new Image();
                imgTip.Source = new BitmapImage(new Uri(strUrl, UriKind.Absolute));
                imgTip.Stretch = Stretch.Uniform;
                ImageBgImage.ToolTip = imgTip;

                #endregion

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private Color GetColorFromString(string strColor)
        {
            Color color = Brushes.Transparent.Color;
            try
            {
                string strA = strColor.Substring(1, 2);
                string strR = strColor.Substring(3, 2);
                string strG = strColor.Substring(5, 2);
                string strB = strColor.Substring(7, 2);
                color = Color.FromArgb((byte)Convert.ToInt32(strA, 16), (byte)Convert.ToInt32(strR, 16), (byte)Convert.ToInt32(strG, 16),
                    (byte)Convert.ToInt32(strB, 16));
            }
            catch { }
            return color;
        }

        #endregion


        #region Event Handler

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (IsModify)
            {
                ModifyRegion();
            }
            else
            {
                AddRegion();
            }
        }

        void BtnBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            BrowseBgImage();
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    if (IsModify)
                    {
                        parent.Title = CurrentApp.GetLanguageInfo("4412004", "Modify Region");
                    }
                    else
                    {
                        parent.Title = CurrentApp.GetLanguageInfo("4412003", "Modify Region");
                    }
                }

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("COM002", "Close");

                LbRegionName.Text = CurrentApp.GetLanguageInfo("4412005", "Region Name");
                LbRegionType.Text = CurrentApp.GetLanguageInfo("4412006", "Region Type");
                LbRegionState.Text = CurrentApp.GetLanguageInfo("4412007", "State");
                LbIsDefault.Text = CurrentApp.GetLanguageInfo("4412008", "Is Default");
                LbRegionWidth.Text = CurrentApp.GetLanguageInfo("4412009", "Width");
                LbRegionHeight.Text = CurrentApp.GetLanguageInfo("4412010", "Height");
                LbBgColor.Text = CurrentApp.GetLanguageInfo("4412011", "Background Color");
                LbBgImage.Text = CurrentApp.GetLanguageInfo("4412012", "Background Image");

                for (int i = 0; i < mListRegionTypes.Count; i++)
                {
                    var item = mListRegionTypes[i];
                    int intValue = item.Value;

                    item.Display = CurrentApp.GetLanguageInfo(string.Format("4412013{0}", intValue.ToString("000")),
                        intValue.ToString());
                }

                RadioStateEnable.Content = CurrentApp.GetLanguageInfo("4412014001", "Enable");
                RadioStateDisable.Content = CurrentApp.GetLanguageInfo("4412014000", "Disable");
                RadioDefaultYes.Content = CurrentApp.GetLanguageInfo("4412015001", "Yes");
                RadioDefaultNo.Content = CurrentApp.GetLanguageInfo("4412015000", "No");

            }
            catch { }
        }

        #endregion

    }
}
