//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2e0876bb-6de9-4157-ab5d-944cbe81f8fe
//        CLR Version:              4.0.30319.18408
//        Name:                     UCAgentStateModify
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4414
//        File Name:                UCAgentStateModify
//
//        created by Charley at 2016/6/23 09:48:32
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
using System.Windows.Media;
using Microsoft.Win32;
using UMPS4414.Models;
using UMPS4414.Wcf11012;
using UMPS4414.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4414
{
    /// <summary>
    /// UCAgentStateModify.xaml 的交互逻辑
    /// </summary>
    public partial class UCAgentStateModify
    {

        #region Members

        public StateSettingMainView PageParent;
        public bool IsModify;
        public ObjItem StateItem;
        public List<AgentStateInfo> ListAllStateInfos;

        private bool mIsInited;
        private AgentStateInfo mStateInfo;
        private ObservableCollection<StateTypeItem> mListStateTypeItems;
        private string mStateIcon;

        #endregion
      

        public UCAgentStateModify()
        {
            InitializeComponent();

            mListStateTypeItems = new ObservableCollection<StateTypeItem>();

            Loaded += UCAgentStateModify_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            //BtnBrowseImage.Click += BtnBrowseImage_Click;
        }

        void UCAgentStateModify_Loaded(object sender, RoutedEventArgs e)
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
                mStateIcon = string.Empty;
                ComboStateTypes.ItemsSource = mListStateTypeItems;
                InitStateTypeItems();
                if (IsModify)
                {
                    if (StateItem == null) { return; }
                    var stateInfo = StateItem.Data as AgentStateInfo;
                    mStateInfo = stateInfo;
                    if (mStateInfo == null) { return; }
                    TxtName.Text = mStateInfo.Name;
                    var typeItem = mListStateTypeItems.FirstOrDefault(t => t.Value == mStateInfo.Type);
                    ComboStateTypes.SelectedItem = typeItem;
                    RadioStateEnable.IsChecked = mStateInfo.State == 1;
                    RadioStateDisable.IsChecked = mStateInfo.State != 1;
                    TxtColor.SelectedColor = GetColorFromString(mStateInfo.Color);

                    string strIcon = mStateInfo.Icon;
                    if (!string.IsNullOrEmpty(strIcon))
                    {
                        //string strUrl = string.Format("{0}://{1}:{2}/{3}/UMPS4414/{4}",
                        //  CurrentApp.Session.AppServerInfo.Protocol,
                        //  CurrentApp.Session.AppServerInfo.Address,
                        //  CurrentApp.Session.AppServerInfo.Port,
                        //  ConstValue.TEMP_DIR_UPLOADFILES,
                        //  strIcon);
                        //BitmapImage image = new BitmapImage();
                        //image.BeginInit();
                        //image.UriSource = new Uri(strUrl, UriKind.Absolute);
                        //image.EndInit();
                        //ImageIcon.Source = image;

                        //Image imgTip = new Image();
                        //imgTip.Source = new BitmapImage(new Uri(strUrl, UriKind.Absolute));
                        //imgTip.Stretch = Stretch.Uniform;
                        //ImageIcon.ToolTip = imgTip;
                    }

                    //RadioIsWorkTimeYes.IsChecked = mStateInfo.IsWorkTime;
                    TxtValue.Text = mStateInfo.Value.ToString();
                    //RadioIsWorkTimeNo.IsChecked = !mStateInfo.IsWorkTime;
                    TxtDescription.Text = mStateInfo.Description;
                }
                else
                {
                    TxtName.Text = string.Empty;
                    var typeItem = mListStateTypeItems.FirstOrDefault();
                    ComboStateTypes.SelectedItem = typeItem;
                    RadioStateEnable.IsChecked = true;
                    RadioStateDisable.IsChecked = false;
                    TxtColor.SelectedColor = Brushes.Transparent.Color;
                    //RadioIsWorkTimeYes.IsChecked = true;
                    TxtValue.Text = "0";
                    //RadioIsWorkTimeNo.IsChecked = false;
                    TxtDescription.Text = string.Empty;
                }

                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitStateTypeItems()
        {
            try
            {
                mListStateTypeItems.Clear();
                StateTypeItem item = new StateTypeItem();
                item.Name = "None";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", 0.ToString("000")), "None");
                item.Value = 0;
                mListStateTypeItems.Add(item);
                item = new StateTypeItem();
                item.Name = "Login";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", 1.ToString("000")), "Login");
                item.Value = 1;
                mListStateTypeItems.Add(item);
                item = new StateTypeItem();
                item.Name = "Call";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", 2.ToString("000")), "Call");
                item.Value = 2;
                mListStateTypeItems.Add(item);
                item = new StateTypeItem();
                item.Name = "Record";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", 3.ToString("000")), "Record");
                item.Value = 3;
                mListStateTypeItems.Add(item);
                item = new StateTypeItem();
                item.Name = "Direction";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", 4.ToString("000")), "Direction");
                item.Value = 4;
                mListStateTypeItems.Add(item);
                item = new StateTypeItem();
                item.Name = "Agent";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", 5.ToString("000")), "Agent");
                item.Value = 5;
                mListStateTypeItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void AddAgentState()
        {
            try
            {
                AgentStateInfo stateInfo = new AgentStateInfo();
                if (ListAllStateInfos == null) { return; }
                int number = 10;        //自定义的状态的编码从10开始累加
                for (int i = 0; i < ListAllStateInfos.Count; i++)
                {
                    var info = ListAllStateInfos[i];
                    number = Math.Max(info.Number, number);
                }
                number++;
                stateInfo.Number = number;
                string strName = TxtName.Text;
                if (string.IsNullOrEmpty(strName))
                {
                    ShowException(CurrentApp.GetLanguageInfo("4414N003",string.Format("Name can not be empty.")));
                    return;
                }
                stateInfo.Name = strName;
                var typeItem = ComboStateTypes.SelectedItem as StateTypeItem;
                if (typeItem != null)
                {
                    stateInfo.Type = typeItem.Value;
                }
                else
                {
                    stateInfo.Type = S4410Consts.STATE_TYPE_UNKOWN;
                }
                string strValue = TxtValue.Text;
                int intValue;
                if (!int.TryParse(strValue, out intValue)
                    || intValue < 0)
                {
                    ShowException(CurrentApp.GetLanguageInfo("4414N004",string.Format("Value invalid")));
                    return;
                }
                stateInfo.Value = intValue;
                stateInfo.State = RadioStateEnable.IsChecked == true ? 1 : 0;
                stateInfo.Color = TxtColor.SelectedColor.ToString();
                stateInfo.Icon = mStateIcon;
                //stateInfo.IsWorkTime = RadioIsWorkTimeYes.IsChecked == true;
                stateInfo.IsWorkTime = true;
                stateInfo.Description = TxtDescription.Text;
                stateInfo.Creator = CurrentApp.Session.UserID;
                stateInfo.CreateTime = DateTime.Now.ToUniversalTime();
                stateInfo.Modifier = CurrentApp.Session.UserID;
                stateInfo.ModifyTime = DateTime.Now.ToUniversalTime();

                OperationReturn optReturn;
                optReturn = XMLHelper.SeriallizeObject(stateInfo);
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
                        webRequest.Code = (int)S4410Codes.SaveAgentStateInfo;
                        webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                        webRequest.ListData.Add("0");
                        webRequest.ListData.Add("1");//拷贝图标文件
                        webRequest.ListData.Add("1");
                        webRequest.ListData.Add(strInfo);
                        Service44101Client client =
                            new Service44101Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            strMsg=string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message);
                            return;
                        }
                        if (webReturn.ListData != null)
                        {
                            for (int i = 0; i < webReturn.ListData.Count; i++)
                            {
                                CurrentApp.WriteLog("DeleteAgentState", string.Format("{0}", webReturn.ListData[i]));
                            }
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
                            ShowInformation(CurrentApp.GetLanguageInfo("4414N005", string.Format("Add AgentState end")));

                            var parent = Parent as PopupPanel;
                            if (parent != null)
                            {
                                parent.IsOpen = false;
                            }

                            if (PageParent != null)
                            {
                                PageParent.ReloadData();
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

        private void ModifyAgentState()
        {
            try
            {
                if (StateItem == null) { return; }
                var stateInfo = StateItem.Data as AgentStateInfo;
                if (stateInfo == null) { return; }

                string strName = TxtName.Text;
                if (string.IsNullOrEmpty(strName))
                {
                    ShowException(CurrentApp.GetLanguageInfo("4414N003", string.Format("Name can not be empty.")));
                    return;
                }
                stateInfo.Name = strName;
                var typeItem = ComboStateTypes.SelectedItem as StateTypeItem;
                if (typeItem != null)
                {
                    stateInfo.Type = typeItem.Value;
                }
                else
                {
                    stateInfo.Type = S4410Consts.STATE_TYPE_UNKOWN;
                }
                string strValue = TxtValue.Text;
                int intValue;
                if (!int.TryParse(strValue, out intValue)
                    || intValue < 0)
                {
                    ShowException(CurrentApp.GetLanguageInfo("4414N004", string.Format("Value invalid")));
                    return;
                }
                stateInfo.Value = intValue;
                stateInfo.State = RadioStateEnable.IsChecked == true ? 1 : 0;
                stateInfo.Color = TxtColor.SelectedColor.ToString();

                bool isCopy = false;
                if (!string.IsNullOrEmpty(mStateIcon))
                {
                    stateInfo.Icon = mStateIcon;
                    isCopy = true;
                }

                //stateInfo.IsWorkTime = RadioIsWorkTimeYes.IsChecked == true;
                stateInfo.IsWorkTime = true;
                stateInfo.Description = TxtDescription.Text;
                stateInfo.Modifier = CurrentApp.Session.UserID;
                stateInfo.ModifyTime = DateTime.Now.ToUniversalTime();

                OperationReturn optReturn;
                optReturn = XMLHelper.SeriallizeObject(stateInfo);
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
                        webRequest.Code = (int)S4410Codes.SaveAgentStateInfo;
                        webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                        webRequest.ListData.Add("0");
                        webRequest.ListData.Add(isCopy ? "1" : "0");
                        webRequest.ListData.Add("1");
                        webRequest.ListData.Add(strInfo);
                        Service44101Client client =
                            new Service44101Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            strMsg=string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message);
                            return;
                        }
                        if (webReturn.ListData != null)
                        {
                            for (int i = 0; i < webReturn.ListData.Count; i++)
                            {
                                CurrentApp.WriteLog("DeleteAgentState", string.Format("{0}", webReturn.ListData[i]));
                            }
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
                            ShowInformation(CurrentApp.GetLanguageInfo("4414N006",
                                string.Format("Modify AgentState end")));

                            var parent = Parent as PopupPanel;
                            if (parent != null)
                            {
                                parent.IsOpen = false;
                            }

                            if (PageParent != null)
                            {
                                PageParent.ReloadData();
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

        private void BrowseIconImage()
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = string.Format("Select a image as AgentState icon");
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
                request.ListData.Add("4414");
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
                mStateIcon = strTemp;

                CurrentApp.WriteLog("UploadIconImage", string.Format("End.\t{0}", mStateIcon));

                //string strUrl = string.Format("{0}://{1}:{2}/{3}/{4}",
                //    CurrentApp.Session.AppServerInfo.Protocol,
                //    CurrentApp.Session.AppServerInfo.Address,
                //    CurrentApp.Session.AppServerInfo.Port,
                //    ConstValue.TEMP_DIR_MEDIADATA,
                //    mStateIcon);
                //BitmapImage bitmap = new BitmapImage();
                //bitmap.BeginInit();
                //bitmap.UriSource = new Uri(strUrl, UriKind.Absolute);
                //bitmap.EndInit();
                //ImageIcon.Source = bitmap;

                //Image imgTip = new Image();
                //imgTip.Source = new BitmapImage(new Uri(strUrl, UriKind.Absolute));
                //imgTip.Stretch = Stretch.Uniform;
                //ImageIcon.ToolTip = imgTip;

                #endregion

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                ModifyAgentState();
            }
            else
            {
                AddAgentState();
            }
        }

        void BtnBrowseImage_Click(object sender, RoutedEventArgs e)
        {
           BrowseIconImage();
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
                        parent.Title = CurrentApp.GetLanguageInfo("4414004", "Modify State");
                    }
                    else
                    {
                        parent.Title = CurrentApp.GetLanguageInfo("4414003", "Add State");
                    }
                }

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("COM002", "Close");

                LbName.Text = CurrentApp.GetLanguageInfo("4414005", "State Name");
                LbType.Text = CurrentApp.GetLanguageInfo("4414006", "Type");
                LbState.Text = CurrentApp.GetLanguageInfo("4414007", "State");
                LbColor.Text = CurrentApp.GetLanguageInfo("4414008", "Color");
                //LbIcon.Text = CurrentApp.GetLanguageInfo("4414009", "Icon");
                //LbIsWorkTime.Text = CurrentApp.GetLanguageInfo("4414010", "Is work time");
                LbValue.Text = CurrentApp.GetLanguageInfo("4414011", "Value");
                LbDescription.Text = CurrentApp.GetLanguageInfo("4414012", "Description");

                for (int i = 0; i < mListStateTypeItems.Count; i++)
                {
                    var item = mListStateTypeItems[i];
                    int intValue = item.Value;

                    item.Display = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", intValue.ToString("000")),
                        intValue.ToString());
                }

                RadioStateEnable.Content = CurrentApp.GetLanguageInfo("4414014001", "Enable");
                RadioStateDisable.Content = CurrentApp.GetLanguageInfo("4414014000", "Disable");
                //RadioIsWorkTimeYes.Content = CurrentApp.GetLanguageInfo("4414015001", "Yes");
                //RadioIsWorkTimeNo.Content = CurrentApp.GetLanguageInfo("4414015000", "No");
            }
            catch { }
        }

        #endregion

    }
}
