//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4e22a0e3-2c74-4bf7-b6b6-7fe86665bdce
//        CLR Version:              4.0.30319.18408
//        Name:                     UCAlarmInfoModify
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4415
//        File Name:                UCAlarmInfoModify
//
//        created by Charley at 2016/7/12 18:31:15
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using UMPS4415.Models;
using UMPS4415.Wcf11012;
using UMPS4415.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4415
{
    /// <summary>
    /// UCAlarmInfoModify.xaml 的交互逻辑
    /// </summary>
    public partial class UCAlarmInfoModify
    {

        #region Members

        public AlarmSettingMainView PageParent;
        public AlarmMessageItem AlarmItem;
        public bool IsModify;
        public List<AgentStateInfo> ListAgentStateInfos;

        private ObservableCollection<ComboItem> mListTypeItems;
        private ObservableCollection<ComboItem> mListRelativeStateItems;
        private bool mIsInited;
        private string mAlarmIcon;

        #endregion


        public UCAlarmInfoModify()
        {
            InitializeComponent();

            mListTypeItems = new ObservableCollection<ComboItem>();
            mListRelativeStateItems = new ObservableCollection<ComboItem>();

            Loaded += UCAlarmInfoModify_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            BtnBrowseImage.Click += BtnIcon_Click;
        }

        void UCAlarmInfoModify_Loaded(object sender, RoutedEventArgs e)
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
                ComboType.ItemsSource = mListTypeItems;
                ComboRelativeState.ItemsSource = mListRelativeStateItems;

                InitTypeItems();
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {

                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    CreateRelativeStateItems();
                    InitInfo();

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitInfo()
        {
            try
            {
                mAlarmIcon = string.Empty;
                if (IsModify)
                {
                    if (AlarmItem == null) { return; }
                    var alarmInfo = AlarmItem.Info;
                    if (alarmInfo == null) { return; }
                    TxtName.Text = alarmInfo.Name;
                    var typeItem = mListTypeItems.FirstOrDefault(t => t.IntValue == alarmInfo.Type);
                    ComboType.SelectedItem = typeItem;
                    RadioStateEnable.IsChecked = alarmInfo.State == 0;
                    RadioStateDisable.IsChecked = alarmInfo.State != 0;
                    TxtRank.Value = alarmInfo.Rank;
                    TxtColor.SelectedColor = GetColorFromString(alarmInfo.Color);

                    string strIcon = alarmInfo.Icon;
                    if (!string.IsNullOrEmpty(strIcon))
                    {
                        string strUrl = string.Format("{0}://{1}:{2}/{3}/UMPS4415/{4}",
                          CurrentApp.Session.AppServerInfo.Protocol,
                          CurrentApp.Session.AppServerInfo.Address,
                          CurrentApp.Session.AppServerInfo.Port,
                          ConstValue.TEMP_DIR_UPLOADFILES,
                          strIcon);
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.UriSource = new Uri(strUrl, UriKind.Absolute);
                        image.EndInit();
                        ImageIcon.Source = image;

                        Image imgTip = new Image();
                        imgTip.Source = new BitmapImage(new Uri(strUrl, UriKind.Absolute));
                        imgTip.Stretch = Stretch.Uniform;
                        ImageIcon.ToolTip = imgTip;
                    }

                    TxtHoldTime.Value = alarmInfo.HoldTime;
                    typeItem = mListRelativeStateItems.FirstOrDefault(s => s.LongValue == alarmInfo.StateID);
                    ComboRelativeState.SelectedItem = typeItem;
                    TxtValue.Text = alarmInfo.Value;
                    TxtContent.Text = alarmInfo.Content;
                }
                else
                {
                    TxtName.Text = string.Empty;
                    var typeItem = mListTypeItems.FirstOrDefault(t => t.IntValue == 0);
                    ComboType.SelectedItem = typeItem;
                    RadioStateEnable.IsChecked = true;
                    RadioStateDisable.IsChecked = false;
                    TxtRank.Value = 0;
                    TxtColor.SelectedColor = Brushes.Transparent.Color;
                    TxtHoldTime.Value = 0;
                    typeItem = mListRelativeStateItems.FirstOrDefault(s => s.LongValue == 0);
                    ComboRelativeState.SelectedItem = typeItem;
                    TxtValue.Text = string.Empty;
                    TxtContent.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitTypeItems()
        {
            try
            {
                mListTypeItems.Clear();
                ComboItem item = new ComboItem();
                item.Name = "Traditional";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4415015{0}", 0.ToString("000")), "Traditional");
                item.IntValue = 0;
                mListTypeItems.Add(item);
                item = new ComboItem();
                item.Name = "State Timeout";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4415015{0}", 1.ToString("000")), "State Timeout");
                item.IntValue = 1;
                mListTypeItems.Add(item);
                item = new ComboItem();
                item.Name = "Keyword";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4415015{0}", 2.ToString("000")), "Keyword");
                item.IntValue = 2;
                mListTypeItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateRelativeStateItems()
        {
            try
            {
                if (ListAgentStateInfos == null) { return; }
                mListRelativeStateItems.Clear();
                for (int i = 0; i < ListAgentStateInfos.Count; i++)
                {
                    var info = ListAgentStateInfos[i];
                    ComboItem item = new ComboItem();
                    item.Data = info;
                    item.Name = info.Name;
                    item.Display = info.Name;
                    item.LongValue = info.ObjID;
                    mListRelativeStateItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Event Handlers

        void BtnIcon_Click(object sender, RoutedEventArgs e)
        {
            BrowseIconImage();
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (IsModify)
            {
                ModifyAlarmMessage();
            }
            else
            {
                AddAlarmMessage();
            }
        }

        #endregion


        #region Others

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


        #region Operations

        private void AddAlarmMessage()
        {
            try
            {
                AlarmMessageInfo alarmInfo = new AlarmMessageInfo();
                if (string.IsNullOrEmpty(TxtName.Text))
                {
                    ShowException(CurrentApp.GetLanguageInfo("4415N003", string.Format("Name empty")));
                    return;
                }
                alarmInfo.Name = TxtName.Text;
                var typeItem = ComboType.SelectedItem as ComboItem;
                if (typeItem == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("4415N004", string.Format("Type invalid")));
                    return;
                }
                alarmInfo.Type = typeItem.IntValue;
                alarmInfo.State = RadioStateEnable.IsChecked == true ? 0 : 2;
                if (TxtRank.Value == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("4415N006", string.Format("Rank invalid")));
                    return;
                }
                int intValue = (int)TxtRank.Value;
                if (intValue < 0 || intValue > 10)
                {
                    ShowException(CurrentApp.GetLanguageInfo("4415N006", string.Format("Rank invalid")));
                    return;
                }
                alarmInfo.Rank = intValue;
                alarmInfo.Color = TxtColor.SelectedColor.ToString();
                alarmInfo.Icon = mAlarmIcon;
                if (TxtHoldTime.Value == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("4415N007", string.Format("HoldTime invalid")));
                    return;
                }
                intValue = (int)TxtHoldTime.Value;
                if (intValue < 0)
                {
                    ShowException(CurrentApp.GetLanguageInfo("4415N007", string.Format("HoldTime invalid")));
                    return;
                }
                alarmInfo.HoldTime = intValue;
                typeItem = ComboRelativeState.SelectedItem as ComboItem;
                if (typeItem == null)
                {
                    alarmInfo.StateID = 0;
                }
                else
                {
                    alarmInfo.StateID = typeItem.LongValue;
                }
                alarmInfo.Value = TxtValue.Text;
                alarmInfo.Content = TxtContent.Text;

                bool isFail = true;
                string strMsg = string.Empty;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.SaveAlarmMessage;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add("1");//拷贝图标文件
                webRequest.ListData.Add("1");
                OperationReturn optReturn = XMLHelper.SeriallizeObject(alarmInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        Service44101Client client =
                            new Service44101Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        if (!webReturn.Result)
                        {
                            strMsg = string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message);
                            return;
                        }
                        if (webReturn.ListData == null)
                        {
                            strMsg = string.Format("Fail.ListData is null");
                            return;
                        }
                        for (int i = 0; i < webReturn.ListData.Count; i++)
                        {
                            string str = webReturn.ListData[i];

                            CurrentApp.WriteLog("AddAlarmMessage", string.Format("{0}", str));
                        }
                        isFail = false;
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    if (!isFail)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("4415N008", string.Format("Add successful")));

                        if (PageParent != null)
                        {
                            PageParent.ReloadData();
                        }
                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                    }
                    else
                    {
                        ShowException(strMsg);
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ModifyAlarmMessage()
        {
            try
            {
                if (AlarmItem == null) { return; }
                var alarmInfo = AlarmItem.Info;
                if (alarmInfo == null) { return; }
                if (string.IsNullOrEmpty(TxtName.Text))
                {
                    ShowException(CurrentApp.GetLanguageInfo("4415N003", string.Format("Name empty")));
                    return;
                }
                alarmInfo.Name = TxtName.Text;
                var typeItem = ComboType.SelectedItem as ComboItem;
                if (typeItem == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("4415N004", string.Format("Type invalid")));
                    return;
                }
                alarmInfo.Type = typeItem.IntValue;
                alarmInfo.State = RadioStateEnable.IsChecked == true ? 0 : 2;
                if (TxtRank.Value == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("4415N006", string.Format("Rank invalid")));
                    return;
                }
                int intValue = (int)TxtRank.Value;
                if (intValue < 0 || intValue > 10)
                {
                    ShowException(CurrentApp.GetLanguageInfo("4415N006", string.Format("Rank invalid")));
                    return;
                }
                alarmInfo.Rank = intValue;
                alarmInfo.Color = TxtColor.SelectedColor.ToString();
                bool isCopy = false;
                if (!string.IsNullOrEmpty(mAlarmIcon))
                {
                    alarmInfo.Icon = mAlarmIcon;
                    isCopy = true;
                }
                if (TxtHoldTime.Value == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("4415N007", string.Format("HoldTime invalid")));
                    return;
                }
                intValue = (int)TxtHoldTime.Value;
                if (intValue < 0)
                {
                    ShowException(CurrentApp.GetLanguageInfo("4415N007", string.Format("HoldTime invalid")));
                    return;
                }
                alarmInfo.HoldTime = intValue;
                typeItem = ComboRelativeState.SelectedItem as ComboItem;
                if (typeItem == null)
                {
                    alarmInfo.StateID = 0;
                }
                else
                {
                    alarmInfo.StateID = typeItem.LongValue;
                }
                alarmInfo.Value = TxtValue.Text;
                alarmInfo.Content = TxtContent.Text;

                bool isFail = true;
                string strMsg = string.Empty;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.SaveAlarmMessage;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(isCopy ? "1" : "0");
                webRequest.ListData.Add("1");
                OperationReturn optReturn = XMLHelper.SeriallizeObject(alarmInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        Service44101Client client =
                       new Service44101Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                           WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        if (!webReturn.Result)
                        {
                            strMsg = string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message);
                            return;
                        }
                        if (webReturn.ListData == null)
                        {
                            strMsg = string.Format("Fail.ListData is null");
                            return;
                        }
                        for (int i = 0; i < webReturn.ListData.Count; i++)
                        {
                            string str = webReturn.ListData[i];

                            CurrentApp.WriteLog("ModifyAlarmMessage", string.Format("{0}", str));
                        }
                        isFail = false;
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    if (!isFail)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("4415N009",string.Format("Modify successful")));

                        if (PageParent != null)
                        {
                            PageParent.ReloadData();
                        }
                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                    }
                    else
                    {
                        ShowException(strMsg);
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void BrowseIconImage()
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = string.Format("Select a image as Alarm icon");
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
                request.ListData.Add("4415");
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
                mAlarmIcon = strTemp;

                CurrentApp.WriteLog("UploadIconImage", string.Format("End.\t{0}", mAlarmIcon));

                string strUrl = string.Format("{0}://{1}:{2}/{3}/{4}",
                    CurrentApp.Session.AppServerInfo.Protocol,
                    CurrentApp.Session.AppServerInfo.Address,
                    CurrentApp.Session.AppServerInfo.Port,
                    ConstValue.TEMP_DIR_MEDIADATA,
                    mAlarmIcon);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(strUrl, UriKind.Absolute);
                bitmap.EndInit();
                ImageIcon.Source = bitmap;

                Image imgTip = new Image();
                imgTip.Source = new BitmapImage(new Uri(strUrl, UriKind.Absolute));
                imgTip.Stretch = Stretch.Uniform;
                ImageIcon.ToolTip = imgTip;

                #endregion

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
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
                        parent.Title = CurrentApp.GetLanguageInfo("4415004", "Modify Alarm");
                    }
                    else
                    {
                        parent.Title = CurrentApp.GetLanguageInfo("4415003", "Add Alarm");
                    }
                }

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("COM002", "Close");

                LbName.Text = CurrentApp.GetLanguageInfo("4415005", "Alarm Name");
                LbType.Text = CurrentApp.GetLanguageInfo("4415006", "Type");
                LbState.Text = CurrentApp.GetLanguageInfo("4415020", "Alarm Type");
                LbRank.Text = CurrentApp.GetLanguageInfo("4415008", "Rank");
                LbColor.Text = CurrentApp.GetLanguageInfo("4415009", "Color");
                LbIcon.Text = CurrentApp.GetLanguageInfo("4415010", "Icon");
                LbHoldTime.Text = CurrentApp.GetLanguageInfo("4415011", "Hold Time");
                LbRelativeState.Text = CurrentApp.GetLanguageInfo("4415012", "Relative State");
                LbValue.Text = CurrentApp.GetLanguageInfo("4415013", "Value");
                LbContent.Text = CurrentApp.GetLanguageInfo("4415014", "Content");

                RadioStateEnable.Content = CurrentApp.GetLanguageInfo("4415021000", "Enable");
                RadioStateDisable.Content = CurrentApp.GetLanguageInfo("4415021002", "Disable");

                for (int i = 0; i < mListTypeItems.Count; i++)
                {
                    var item = mListTypeItems[i];
                    int intValue = item.IntValue;

                    item.Display = CurrentApp.GetLanguageInfo(string.Format("4415015{0}", intValue.ToString("000")),
                        intValue.ToString());
                }
            }
            catch (Exception ex) { }
        }

        #endregion

    }
}
