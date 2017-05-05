using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using UMPS1110.Models;
using UMPS1110.Wcf11102;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Communications;

namespace UMPS1110.Editors
{
    /// <summary>
    /// UCPropertyEditor430.xaml 的交互逻辑
    /// </summary>
    public partial class UCPropertyEditor430
    {
        #region MainPageProperty

        public static readonly DependencyProperty MainPageProperty =
            DependencyProperty.Register("MainPage", typeof (ResourceMainView), typeof (UCPropertyEditor400), new PropertyMetadata(default(ResourceMainView)));

        public ResourceMainView MainPage
        {
            get { return (ResourceMainView) GetValue(MainPageProperty); }
            set { SetValue(MainPageProperty, value); }
        }

        #endregion

        #region PropertyInfoItemProperty

        public static readonly DependencyProperty PropertyInfoItemProperty =
          DependencyProperty.Register("PropertyInfoItem", typeof(ResourcePropertyInfoItem), typeof(UCPropertyEditor400), new PropertyMetadata(default(ResourcePropertyInfoItem), OnPropertyInfoItemChanged));

        public ResourcePropertyInfoItem PropertyInfoItem
        {
            get { return (ResourcePropertyInfoItem)GetValue(PropertyInfoItemProperty); }
            set { SetValue(PropertyInfoItemProperty, value); }
        }

        private static void OnPropertyInfoItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as UCPropertyEditor400;
            if (editor != null)
            {
                editor.OnPropertyInfoItemChanged((ResourcePropertyInfoItem)e.OldValue,
                    (ResourcePropertyInfoItem)e.NewValue);
            }
        }

        public void OnPropertyInfoItemChanged(ResourcePropertyInfoItem oldValue, ResourcePropertyInfoItem newValue)
        {
            RefreshValue();
        }


        #endregion

        #region Members

        private UCPathLister mUCPathLister;
        private ObjectPropertyInfo mPropertyInfo;
        private ResourceProperty mPropertyValue;
        private ConfigObject mConfigObject;
        private BackgroundWorker mWorker;

        #endregion

        public UCPropertyEditor430()
        {
            InitializeComponent();

            Loaded += UCPropertyEditor430_Loaded;
            TxtUserID.TextChanged += TxtUserID_TextChanged;
            BtnBrowser.Click += BtnBrowser_Click;
        }

        private void UCPropertyEditor430_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        #region Event Handler

        private void TxtUserID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mPropertyValue != null)
            {
                mPropertyValue.Value = TxtUserID.Text;
            }
            if (mConfigObject != null)
            {
                mConfigObject.GetBasicPropertyValues();
            }
            OnPropertyValueChanged();
        }

        void BtnBrowser_Click(object sender, RoutedEventArgs e)
        {
            //加载用户机构树
            //if (mConfigObject != null)
            //{
            //    if (mConfigObject.ObjectType == S1110Consts.RESOURCE_STORAGEDEVICE)
            //    {
            //        var prop =
            //            mConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == StorageDeviceObject.PRO_TYPE);
            //        if (prop != null)
            //        {
            //            var value = prop.Value;
            //            //除本地磁盘外，其他类型存储设备不能通过浏览目录结构的方式设置路径
            //            if (value != "0")
            //            {
            //                return;
            //            }
            //        }
            //    }
            //}
            if (MainPage != null)
            {
                var popup = MainPage.PopupPanel;
                if (popup != null)
                {
                    popup.Title = string.Format("Select directory");
                    mUCPathLister = new UCPathLister();
                    mUCPathLister.PathListerEvent += mUCPathLister_PathListerEvent;
                    mUCPathLister.CurrentApp = CurrentApp;
                    popup.Content = mUCPathLister;
                    popup.IsOpen = true;
                    GetDriverList();
                }
            }
        }

        void mUCPathLister_PathListerEvent(object sender, RoutedPropertyChangedEventArgs<PathListerEventEventArgs> e)
        {
            var args = e.NewValue;
            if (args == null) { return; }
            ObjectItem item;
            switch (args.Code)
            {
                case 1:
                    item = args.Data as ObjectItem;
                    if (item == null) { return; }
                    string strValue = string.Empty;
                    var dirInfo = item.Data as DirInfo;
                    if (dirInfo == null)
                    {
                        var fileInfo = item.Data as FileInfo;
                        if (fileInfo != null)
                        {
                            strValue = fileInfo.Path;
                        }
                    }
                    else
                    {
                        strValue = dirInfo.Path;
                    }
                    TxtUserID.Text = strValue;
                    if (mPropertyValue != null)
                    {
                        mPropertyValue.Value = strValue;
                    }
                    if (mConfigObject != null)
                    {
                        mConfigObject.GetBasicPropertyValues();
                    }
                    OnPropertyValueChanged();
                    break;
                case 3:
                    item = args.Data as ObjectItem;
                    if (item == null) { return; }
                    GetChildDirItems(item);
                    break;
            }
        }

        #endregion

        #region Others

        private void GetDriverList()
        {
            try
            {
                string strAddress = string.Empty;
                if (string.IsNullOrEmpty(strAddress))
                {
                    ShowException(string.Format("Server address is empty"));
                    return;
                }
                if (mUCPathLister == null) { return; }
                ObjectItem rootItem = mUCPathLister.RootItem;
                if (rootItem == null) { return; }
                ServerRequestInfo requestInfo = new ServerRequestInfo();
                requestInfo.ServerHost = strAddress;
                requestInfo.ServerPort = 8009;
                requestInfo.Command = (int)ServerRequestCommand.GetDiskDriverList;
                string strGetSystemDisk = "0";
                if (mConfigObject != null)
                {
                    switch (mConfigObject.ObjectType)
                    {
                        case S1110Consts.RESOURCE_VOICESERVER:
                        case S1110Consts.RESOURCE_NTIDRVPATH:
                            strGetSystemDisk = "1";
                            break;
                    }
                }
                requestInfo.ListData.Add(strGetSystemDisk);
                OperationReturn optReturn = XMLHelper.SeriallizeObject(requestInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1110Codes.GetServerInfo;
                webRequest.Data = optReturn.Data.ToString();
                Service11102Client client = new Service11102Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11102"));
                WebReturn webReturn = null;
                if (MainPage != null)
                {
                    MainPage.SetBusy(true, CurrentApp.GetMessageLanguageInfo("003", "Getting server disk information..."));
                }
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    try
                    {
                        webReturn = client.DoOperation(webRequest);
                        client.Close();
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    if (MainPage != null)
                    {
                        MainPage.SetBusy(false, string.Empty);
                    }
                    if (webReturn == null) { return; }
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    if (webReturn.ListData == null)
                    {
                        ShowException(string.Format("ListData is null"));
                        return;
                    }
                    rootItem.Children.Clear();
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        string info = webReturn.ListData[i];
                        string[] arrInfo = info.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                        if (arrInfo.Length < 2) { continue; }
                        DirInfo dirInfo = new DirInfo();
                        dirInfo.Name = arrInfo[0];
                        dirInfo.Path = arrInfo[0] + "\\";
                        ObjectItem item = new ObjectItem();
                        item.Type = S1110Consts.OBJECTITEMTYPE_DIRINFO;
                        item.Name = dirInfo.Name;
                        item.Icon = string.Format("../Themes/Default/UMPS1110/Images/{0}", "driver.png");
                        item.Description = dirInfo.Path;
                        item.Data = dirInfo;
                        rootItem.AddChild(item);
                    }
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetChildDirItems(ObjectItem parentItem)
        {
            try
            {
                string strAddress = string.Empty;
                if (string.IsNullOrEmpty(strAddress))
                {
                    ShowException(string.Format("Server address is empty"));
                    return;
                }
                if (parentItem == null) { return; }
                var parentDir = parentItem.Data as DirInfo;
                if (parentDir == null) { return; }
                ServerRequestInfo requestInfo = new ServerRequestInfo();
                requestInfo.ServerHost = strAddress;
                requestInfo.ServerPort = 8009;
                requestInfo.Command = (int)ServerRequestCommand.GetChildDirectoryList;
                requestInfo.ListData.Add(parentDir.Path);
                OperationReturn optReturn = XMLHelper.SeriallizeObject(requestInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1110Codes.GetServerInfo;
                webRequest.Data = optReturn.Data.ToString();
                Service11102Client client = new Service11102Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11102"));
                WebReturn webReturn = null;
                if (MainPage != null)
                {
                    MainPage.SetBusy(true, CurrentApp.GetMessageLanguageInfo("004", "Getting server directory information"));
                }
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    try
                    {
                        webReturn = client.DoOperation(webRequest);
                        client.Close();
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    if (MainPage != null)
                    {
                        MainPage.SetBusy(false, string.Empty);
                    }
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    if (webReturn.ListData == null)
                    {
                        ShowException(string.Format("ListData is null"));
                        return;
                    }
                    parentItem.Children.Clear();
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        string info = webReturn.ListData[i];
                        string[] arrInfo = info.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                        if (arrInfo.Length < 2) { continue; }
                        DirInfo dirInfo = new DirInfo();
                        dirInfo.Name = arrInfo[0];
                        dirInfo.Path = arrInfo[1];
                        ObjectItem item = new ObjectItem();
                        item.Type = S1110Consts.OBJECTITEMTYPE_DIRINFO;
                        item.Name = dirInfo.Name;
                        item.Icon = string.Format("../Themes/Default/UMPS1110/Images/{0}", "folder.png");
                        item.Description = dirInfo.Path;
                        item.Data = dirInfo;
                        parentItem.AddChild(item);
                    }
                    if (mConfigObject != null)
                    {
                        switch (mConfigObject.ObjectType)
                        {
                            case S1110Consts.RESOURCE_VOICESERVER:
                            case S1110Consts.RESOURCE_NTIDRVPATH:
                                //GetChildFileItems(parentItem);
                                break;
                        }
                    }
                    parentItem.IsExpanded = true;
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        #endregion

        #region init

        private void Init()
        {

        }
        #endregion

        private void RefreshValue()
        {
            Init();
        }

        #region PropertyValueChanged

        public event RoutedPropertyChangedEventHandler<PropertyValueChangedEventArgs> PropertyValueChanged;

        private void OnPropertyValueChanged(PropertyValueChangedEventArgs args)
        {
            RoutedPropertyChangedEventArgs<PropertyValueChangedEventArgs> p =
               new RoutedPropertyChangedEventArgs<PropertyValueChangedEventArgs>(null, args);
            if (PropertyValueChanged != null)
            {
                PropertyValueChanged(this, p);
            }
        }

        private void OnPropertyValueChanged()
        {
            if (mConfigObject != null)
            {
                mConfigObject.GetBasicPropertyValues();
            }
            PropertyValueChangedEventArgs args = new PropertyValueChangedEventArgs();
            args.PropertyItem = PropertyInfoItem;
            args.ConfigObject = mConfigObject;
            args.PropertyInfo = mPropertyInfo;
            args.PropetyValue = mPropertyValue;
            if (mPropertyValue != null)
            {
                args.Value = mPropertyValue.Value;
            }
            OnPropertyValueChanged(args);
        }

        #endregion
    }
}
