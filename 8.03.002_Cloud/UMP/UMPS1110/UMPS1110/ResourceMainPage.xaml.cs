//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    210213b1-1505-44e4-a0cb-8e04b4c17bf6
//        CLR Version:              4.0.30319.18444
//        Name:                     ResourceMainPage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110
//        File Name:                ResourceMainPage
//
//        created by Charley at 2014/12/18 14:17:08
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UMPS1110.Models;
using UMPS1110.Models.ConfigObjects;
using UMPS1110.Wcf11012;
using UMPS1110.Wcf11101;
using UMPS1110.Wcf11102;
using UMPS1110.Wizard;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1110
{
    /// <summary>
    /// ResourceMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class ResourceMainPage
    {

        #region Memebers

        public static ObservableCollection<OperationInfo> ListOperations = new ObservableCollection<OperationInfo>();

        private BackgroundWorker mWorker;
        private ObjectItem mRootItem;
        private ObservableCollection<OperationInfo> mListBasicOperations;
        private List<ResourceTypeParam> mListResourceTypeParams;
        private List<ResourceGroupParam> mListResourceGroupParams;
        private List<ObjectPropertyInfo> mListResourcePropertyInfos;
        private List<BasicResourceInfo> mListBasicResourceInfos;
        private List<ResourceProperty> mListResourcePropertyValues;
        private List<ResourceProperty> mListSingleResourcePropertyValues;
        private List<ObjectItem> mListObjectItems;
        private List<ConfigObject> mListConfigObjects;
        private List<long> mListRemoveObjectIDs;
        private List<BasicUserInfo> mListSftpUsers;
        private List<BasicInfoData> mListBasicInfoDatas;
        private ConfigObject mCurrentConfigObject;
        private ConfigGroup mCurrentConfigGroup;
        private ObjectItem mCurrentObjectItem;
        private DescriptionInfo mCurrentDescription;
        private MultiSelectedItem mMultiSelectedItem;
        private WizardHelper mWizardHelper;

        #endregion

        public ResourceMainPage()
        {
            InitializeComponent();

            mRootItem = new ObjectItem();
            mMultiSelectedItem = new MultiSelectedItem();
            mListBasicOperations = new ObservableCollection<OperationInfo>();
            mListResourceTypeParams = new List<ResourceTypeParam>();
            mListResourceGroupParams = new List<ResourceGroupParam>();
            mListResourcePropertyInfos = new List<ObjectPropertyInfo>();
            mListBasicResourceInfos = new List<BasicResourceInfo>();
            mListResourcePropertyValues = new List<ResourceProperty>();
            mListSingleResourcePropertyValues = new List<ResourceProperty>();
            mListObjectItems = new List<ObjectItem>();
            mListConfigObjects = new List<ConfigObject>();
            mListRemoveObjectIDs = new List<long>();
            mListSftpUsers = new List<BasicUserInfo>();
            mListBasicInfoDatas = new List<BasicInfoData>();

            TreeResourceObject.SelectedItemChanged += TreeResourceObject_SelectedItemChanged;
        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "ResourceMainPage";
                StylePath = "UMPS1110/ResourceMainPage.xaml";

                base.Init();

                TreeResourceObject.ItemsSource = mRootItem.Children;

                MyWaiter.Visibility = Visibility.Visible;
                SetStatuMessage(App.GetMessageLanguageInfo("001", "Loading config information..."));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    //触发Loaded消息
                    SendLoadedMessage();

                    Parallel.Invoke(InitOperations,
                        LoadSftpUsers,
                        LoadBasicInfoDatas,
                        LoadResourceTypeParams,
                        LoadResourceGroupParams,
                        LoadResourcePropertyInfos,
                        LoadBasicResourceInfos);
                    //初始化数据
                    InitConfigObjects();
                    InitWizardHelper();

                    LoadLicenseInfo();
                    //InitOperations();
                    //LoadSftpUsers();
                    //LoadBasicInfoDatas();
                    //LoadResourceTypeParams();
                    //LoadResourceGroupParams();
                    //LoadResourcePropertyInfos();
                    //LoadResourcePropertyValues();
                    //InitConfigObjects();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Hidden;
                    SetStatuMessage(string.Empty);

                    CreateObjectItems();
                    if (mRootItem.Children.Count > 0)
                    {
                        var child = mRootItem.Children[0] as ObjectItem;
                        if (child != null)
                        {
                            child.IsExpanded = true;
                            child.IsSelected = true;
                        }
                    }
                    ResetOperationButtons();
                    CreateOptButtons();
                    //App.GetLicenses("192.168.4.182");
                    ChangeLanguage();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitOperations()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("11");
                webRequest.ListData.Add("1110");
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                ListOperations.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo optInfo = optReturn.Data as OperationInfo;
                    if (optInfo != null)
                    {
                        optInfo.Display = App.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                        optInfo.Description = optInfo.Display;
                        ListOperations.Add(optInfo);
                        InitOperations(optInfo.ID);
                    }
                }

                App.WriteLog("PageLoad", string.Format("Init Operations"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitOperations(long parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("11");
                webRequest.ListData.Add(parentID.ToString());  //根据父级操作ID获取下级操作
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebRequest(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo optInfo = optReturn.Data as OperationInfo;
                    if (optInfo != null)
                    {
                        optInfo.Display = App.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                        optInfo.Description = optInfo.Display;
                        ListOperations.Add(optInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadResourceTypeParams()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1110Codes.GetResourceTypeParamList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11101Client client = new Service11101Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service11101"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                List<ResourceTypeParam> listTypeParams = new List<ResourceTypeParam>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<ResourceTypeParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceTypeParam info = optReturn.Data as ResourceTypeParam;
                    if (info == null)
                    {
                        App.ShowExceptionMessage(string.Format("ResourceTypeParam is null"));
                        return;
                    }
                    listTypeParams.Add(info);
                }
                listTypeParams = listTypeParams.OrderBy(t => t.ParentID).ThenBy(t => t.OrderID).ToList();
                mListResourceTypeParams.Clear();
                foreach (var typeParam in listTypeParams)
                {
                    mListResourceTypeParams.Add(typeParam);
                }

                App.WriteLog("PageLoad", string.Format("Init ResourceTypeParam"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadResourceGroupParams()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1110Codes.GetResourceGroupParamList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11101Client client = new Service11101Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service11101"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                List<ResourceGroupParam> listGroupParams = new List<ResourceGroupParam>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<ResourceGroupParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceGroupParam info = optReturn.Data as ResourceGroupParam;
                    if (info == null)
                    {
                        App.ShowExceptionMessage(string.Format("ResourceGroupParam is null"));
                        return;
                    }
                    if (!info.IsShow) { continue; }
                    listGroupParams.Add(info);
                }
                listGroupParams =
                    listGroupParams.OrderBy(g => g.TypeID).ThenBy(g => g.ParentGroup).ThenBy(g => g.SortID).ToList();
                mListResourceGroupParams.Clear();
                foreach (var groupParam in listGroupParams)
                {
                    mListResourceGroupParams.Add(groupParam);
                }

                App.WriteLog("PageLoad", string.Format("Init ResourceGroupParam"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadResourcePropertyInfos()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1110Codes.GetResourcePropertyInfoList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                App.MonitorHelper.AddWebRequest(webRequest);

                Service11101Client client = new Service11101Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service11101"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                List<ObjectPropertyInfo> listPropertyInfos = new List<ObjectPropertyInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<ObjectPropertyInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ObjectPropertyInfo info = optReturn.Data as ObjectPropertyInfo;
                    if (info == null)
                    {
                        App.ShowExceptionMessage(string.Format("ResourcePropertyInfo is null"));
                        return;
                    }
                    listPropertyInfos.Add(info);
                }
                listPropertyInfos =
                    listPropertyInfos.OrderBy(p => p.ObjType).ThenBy(p => p.GroupID).ThenBy(p => p.SortID).ToList();
                mListResourcePropertyInfos.Clear();
                foreach (var propertyInfo in listPropertyInfos)
                {
                    mListResourcePropertyInfos.Add(propertyInfo);
                }

                App.WriteLog("PageLoad", string.Format("Init ResourcePropertyInfo"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadBasicResourceInfos()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1110Codes.GetBasicResourceInfoList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11101Client client = new Service11101Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service11101"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                List<BasicResourceInfo> listItems = new List<BasicResourceInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<BasicResourceInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicResourceInfo info = optReturn.Data as BasicResourceInfo;
                    if (info == null)
                    {
                        App.ShowExceptionMessage(string.Format("BasicResourceInfo is null"));
                        return;
                    }
                    listItems.Add(info);
                }
                listItems =
                    listItems.OrderBy(i => i.ObjectID).ToList();
                mListBasicResourceInfos.Clear();
                foreach (var item in listItems)
                {
                    mListBasicResourceInfos.Add(item);
                }

                App.WriteLog("PageLoad", string.Format("Init BasicResourceInfos"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadResourcePropertyValues()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1110Codes.GetResourcePropertyValueList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                //获取所有配置对象的属性值
                webRequest.ListData.Add("0");
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11101Client client = new Service11101Client(
                     WebHelper.CreateBasicHttpBinding(App.Session),
                     WebHelper.CreateEndpointAddress(
                         App.Session.AppServerInfo,
                         "Service11101"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                List<ResourceProperty> listPropertyValues = new List<ResourceProperty>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<ResourceProperty>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceProperty info = optReturn.Data as ResourceProperty;
                    if (info == null)
                    {
                        App.ShowExceptionMessage(string.Format("ResourceProperty is null"));
                        return;
                    }
                    listPropertyValues.Add(info);
                }
                listPropertyValues = listPropertyValues.OrderBy(p => p.ObjID).ThenBy(p => p.PropertyID).ToList();
                mListResourcePropertyValues.Clear();
                foreach (var propertyValue in listPropertyValues)
                {
                    mListResourcePropertyValues.Add(propertyValue);
                }

                App.WriteLog("PageLoad", string.Format("Init ResourcePropertyValue"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadSingleResourcePropertyValues(BasicResourceInfo resourceInfo)
        {
            try
            {
                if (resourceInfo == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1110Codes.GetResourcePropertyValueList;
                webRequest.ListData.Add("1");
                //获取指定配置对象的属性值
                webRequest.ListData.Add(resourceInfo.ObjectID.ToString());
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11101Client client = new Service11101Client(
                     WebHelper.CreateBasicHttpBinding(App.Session),
                     WebHelper.CreateEndpointAddress(
                         App.Session.AppServerInfo,
                         "Service11101"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                List<ResourceProperty> listPropertyValues = new List<ResourceProperty>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<ResourceProperty>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceProperty info = optReturn.Data as ResourceProperty;
                    if (info == null)
                    {
                        App.ShowExceptionMessage(string.Format("ResourceProperty is null"));
                        return;
                    }
                    listPropertyValues.Add(info);
                }
                listPropertyValues = listPropertyValues.OrderBy(p => p.ObjID).ThenBy(p => p.PropertyID).ToList();
                foreach (var propertyValue in listPropertyValues)
                {
                    mListSingleResourcePropertyValues.Add(propertyValue);
                }

                //App.WriteLog("PageLoad", string.Format("Init ResourcePropertyValue"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitConfigObjects()
        {
            try
            {
                mListResourcePropertyValues.Clear();
                //初始化所有配置对象，将属性值放入配置对象的ListProperties集合中，
                //这样方便后面快速的取得属性值，而不必从mListResourcePropertyValues集合中获取
                mListConfigObjects.Clear();
                //初始化根对象，这是一个ObjectType和ObjectID都是0的特殊配置对象
                ConfigObject rootObject = ConfigObject.CreateObject(0);
                rootObject.ObjectID = 0;
                rootObject.Name = string.Empty;
                rootObject.Description = string.Empty;
                rootObject.ListAllTypeParams = mListResourceTypeParams;
                rootObject.ListAllObjects = mListConfigObjects;
                rootObject.ListAllBasicInfos = mListBasicInfoDatas;
                mListConfigObjects.Add(rootObject);

                int count = mListBasicResourceInfos.Count;
                for (int i = 0; i < count; i++)
                {
                    BasicResourceInfo resourceInfo = mListBasicResourceInfos[i];
                    long objID = resourceInfo.ObjectID;
                    mListSingleResourcePropertyValues.Clear();
                    double per = i / (count * 1.0) * 100;
                    SetStatuMessage(string.Format("{0}\t{1}\t{2}%",
                        App.GetMessageLanguageInfo("001", "Loading config information..."),
                        objID,
                        per.ToString("00.0")));
                    LoadSingleResourcePropertyValues(resourceInfo);
                    if (mListSingleResourcePropertyValues.Count <= 0) { continue; }
                    //加载资源对象的属性信息
                    ResourceProperty propertyValue = mListSingleResourcePropertyValues[0];
                    int objType = propertyValue.ObjType;
                    List<ObjectPropertyInfo> listPropertyInfos =
                        mListResourcePropertyInfos.Where(p => p.ObjType == objType).ToList();
                    ConfigObject configObject = ConfigObject.CreateObject(objType);
                    configObject.ObjectID = objID;
                    configObject.ListAllTypeParams = mListResourceTypeParams;
                    configObject.ListAllObjects = mListConfigObjects;
                    configObject.ListAllBasicInfos = mListBasicInfoDatas;
                    ResourceTypeParam typeParam = mListResourceTypeParams.FirstOrDefault(t => t.TypeID == objType);
                    if (typeParam != null)
                    {
                        configObject.Icon = typeParam.Icon;
                        configObject.TypeParam = typeParam;
                    }
                    for (int j = 0; j < listPropertyInfos.Count; j++)
                    {
                        ObjectPropertyInfo propertyInfo = listPropertyInfos[j];
                        propertyValue =
                            mListSingleResourcePropertyValues.FirstOrDefault(p => p.PropertyID == propertyInfo.PropertyID);
                        //如果不存对应PropertyID的属性值，根据PropertyInfo创建一个，并且使用默认值填充
                        if (propertyValue == null)
                        {
                            propertyValue = new ResourceProperty();
                            propertyValue.ObjID = objID;
                            propertyValue.ObjType = objType;
                            propertyValue.PropertyID = propertyInfo.PropertyID;
                            propertyValue.Value = propertyInfo.DefaultValue;
                            propertyValue.EncryptMode = propertyInfo.EncryptMode;
                            propertyValue.MultiValueMode = propertyInfo.MultiValueMode;
                        }
                        configObject.ListProperties.Add(propertyValue);
                        mListResourcePropertyValues.Add(propertyValue);
                    }
                    mListConfigObjects.Add(configObject);
                }
                //对每个资源对象调用一次GetBasicPropertyValues初始化模型属性
                for (int i = 0; i < mListConfigObjects.Count; i++)
                {
                    mListConfigObjects[i].GetBasicPropertyValues();
                }

                App.WriteLog("PageLoad", string.Format("Init ConfigObject"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitWizardHelper()
        {
            mWizardHelper = new WizardHelper();
            mWizardHelper.ListAllConfigObjects = mListConfigObjects;
            mWizardHelper.ListAllTypeParams = mListResourceTypeParams;
            mWizardHelper.ListAllGroupParams = mListResourceGroupParams;
            mWizardHelper.ListAllPropertyInfos = mListResourcePropertyInfos;
            mWizardHelper.ListAllPropertyValues = mListResourcePropertyValues;
            mWizardHelper.ListAllBasicInfos = mListBasicInfoDatas;
            mWizardHelper.ListAllObjectItem = mListObjectItems;
            mWizardHelper.mListSftpUsers = mListSftpUsers;
            mWizardHelper.mRootObjectItem = mRootItem;
            List<OperationInfo> TempListOptInfo = new List<OperationInfo>();
            foreach (OperationInfo optInfo in ListOperations)
            {
                TempListOptInfo.Add(optInfo);
            }
            mWizardHelper.WizListOperations = TempListOptInfo;
        }

        private void LoadSftpUsers()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1110Codes.GetSftpUserList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11101Client client = new Service11101Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service11101"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                List<BasicUserInfo> listTypeParams = new List<BasicUserInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<BasicUserInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicUserInfo info = optReturn.Data as BasicUserInfo;
                    if (info == null)
                    {
                        App.ShowExceptionMessage(string.Format("ResourceTypeParam is null"));
                        return;
                    }
                    listTypeParams.Add(info);
                }
                listTypeParams = listTypeParams.OrderBy(t => t.UserID).ToList();
                mListSftpUsers.Clear();
                foreach (var typeParam in listTypeParams)
                {
                    mListSftpUsers.Add(typeParam);
                }

                App.WriteLog("PageLoad", string.Format("Init SftpUsers"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadBasicInfoDatas()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1110Codes.GetBasicInfoDataList;
                webRequest.ListData.Add("3");
                webRequest.ListData.Add("111000000");
                webRequest.ListData.Add(string.Empty);
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11101Client client = new Service11101Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service11101"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                List<BasicInfoData> listItems = new List<BasicInfoData>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<BasicInfoData>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicInfoData info = optReturn.Data as BasicInfoData;
                    if (info == null)
                    {
                        App.ShowExceptionMessage(string.Format("ResourceTypeParam is null"));
                        return;
                    }
                    listItems.Add(info);
                }
                listItems = listItems.OrderBy(i => i.InfoID).ThenBy(i => i.SortID).ToList();
                mListBasicInfoDatas.Clear();
                foreach (var typeParam in listItems)
                {
                    mListBasicInfoDatas.Add(typeParam);
                }

                App.WriteLog("PageLoad", string.Format("Init BasicInfoData"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadLicenseInfo()
        {
            //获取License所在机器ip211
            List<ConfigObject> TempLicenses = mListConfigObjects.Where(p => p.ObjectType == 211).ToList();
            if (TempLicenses != null && TempLicenses.Count() != 0)
            {
                string IPName = TempLicenses[0].ListProperties.FirstOrDefault(p => p.PropertyID == S1110Consts.PROPERTYID_HOSTADDRESS).Value;
                if (IPName != null && IPName != string.Empty)
                    App.GetLicenses(IPName);
            }
        }
        #endregion


        #region Create TreeItem

        private void CreateObjectItems()
        {
            try
            {
                mRootItem.Children.Clear();
                mListObjectItems.Clear();
                ConfigObject rootObject = mListConfigObjects.FirstOrDefault(o => o.ObjectID == 0 && o.ObjectType == 0);
                if (rootObject == null) { return; }
                mRootItem.Type = S1110Consts.OBJECTITEMTYPE_CONFIGOBJECT;
                mRootItem.Name = rootObject.Name;
                mRootItem.Description = rootObject.Description;
                mRootItem.Data = rootObject;
                rootObject.ObjectItem = mRootItem;
                //创建子对象组
                CreateConfigGroupItems(mRootItem);
                mListObjectItems.Add(mRootItem);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateConfigGroupItems(ObjectItem parentItem)
        {
            try
            {
                int type = parentItem.Type;
                if (type == S1110Consts.OBJECTITEMTYPE_CONFIGOBJECT)
                {
                    ConfigObject parentObject = parentItem.Data as ConfigObject;
                    if (parentObject == null) { return; }
                    //只取父级组编码为0的组
                    List<ResourceGroupParam> groupParams =
                        mListResourceGroupParams.Where(g => g.TypeID == parentObject.ObjectType && g.ParentGroup == 0)
                            .ToList();
                    for (int i = 0; i < groupParams.Count; i++)
                    {
                        ResourceGroupParam groupParam = groupParams[i];
                        ConfigGroup configGroup = new ConfigGroup();
                        configGroup.ConfigObject = parentObject;
                        configGroup.TypeID = groupParam.TypeID;
                        configGroup.GroupID = groupParam.GroupID;
                        configGroup.ParentGroupID = groupParam.ParentGroup;
                        configGroup.GroupType = groupParam.GroupType;
                        configGroup.ChildType = groupParam.ChildType;
                        configGroup.Icon = groupParam.Icon;
                        configGroup.Name = groupParam.Description;
                        configGroup.Description = groupParam.Description;
                        configGroup.GroupInfo = groupParam;
                        ObjectItem item = new ObjectItem();
                        item.Type = S1110Consts.OBJECTITEMTYPE_CONFIGGROUP;
                        item.Name =
                            App.GetLanguageInfo(
                                string.Format("1110GRP{0}{1}", configGroup.TypeID.ToString("000"),
                                configGroup.GroupID.ToString("000")),
                                configGroup.Name);
                        item.Description = App.GetLanguageInfo(
                            string.Format("1110GRPD{0}{1}", configGroup.TypeID.ToString("000"),
                            configGroup.GroupID.ToString("000")),
                            configGroup.Name);
                        item.Icon = string.Format("Images/{0}", configGroup.Icon);
                        item.Data = configGroup;
                        configGroup.ObjectItem = item;
                        if (groupParam.GroupType == ResourceGroupType.Group)
                        {
                            CreateConfigGroupItems(item);
                        }
                        if (groupParam.GroupType == ResourceGroupType.ChildList)
                        {
                            CreateConfigObjectItems(item);
                        }
                        parentItem.AddChild(item);
                        mListObjectItems.Add(item);
                    }
                }
                if (type == S1110Consts.OBJECTITEMTYPE_CONFIGGROUP)
                {
                    ConfigGroup parentGroup = parentItem.Data as ConfigGroup;
                    if (parentGroup == null) { return; }
                    //取父级组编码为上级组的编码
                    List<ResourceGroupParam> groupParams =
                       mListResourceGroupParams.Where(g => g.TypeID == parentGroup.TypeID && g.ParentGroup == parentGroup.GroupID)
                           .ToList();
                    for (int i = 0; i < groupParams.Count; i++)
                    {
                        ResourceGroupParam groupParam = groupParams[i];
                        ConfigGroup configGroup = new ConfigGroup();
                        configGroup.ConfigObject = parentGroup.ConfigObject;
                        configGroup.TypeID = groupParam.TypeID;
                        configGroup.GroupID = groupParam.GroupID;
                        configGroup.ParentGroupID = groupParam.ParentGroup;
                        configGroup.GroupType = groupParam.GroupType;
                        configGroup.ChildType = groupParam.ChildType;
                        configGroup.Icon = groupParam.Icon;
                        configGroup.Name = groupParam.Description;
                        configGroup.Description = groupParam.Description;
                        configGroup.GroupInfo = groupParam;
                        ObjectItem item = new ObjectItem();
                        item.Type = S1110Consts.OBJECTITEMTYPE_CONFIGGROUP;
                        item.Name =
                            App.GetLanguageInfo(
                                string.Format("1110GRP{0}{1}", configGroup.TypeID.ToString("000"),
                                configGroup.GroupID.ToString("000")),
                                configGroup.Name);
                        item.Description = App.GetLanguageInfo(
                            string.Format("1110GRPD{0}{1}", configGroup.TypeID.ToString("000"),
                            configGroup.GroupID.ToString("000")),
                            configGroup.Name);
                        item.Icon = string.Format("Images/{0}", configGroup.Icon);
                        item.Data = configGroup;
                        configGroup.ObjectItem = item;
                        if (groupParam.GroupType == ResourceGroupType.Group)
                        {
                            CreateConfigGroupItems(item);
                        }
                        if (groupParam.GroupType == ResourceGroupType.ChildList)
                        {
                            CreateConfigObjectItems(item);
                        }
                        parentItem.AddChild(item);
                        mListObjectItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateConfigObjectItems(ObjectItem parentItem)
        {
            try
            {
                ConfigObject parentObject = null;
                ConfigGroup parentGroup;
                ResourceTypeParam typeParam;
                List<ConfigObject> listConfigObjects = null;
                int childType;
                long parentObjID;
                if (parentItem.Type == S1110Consts.OBJECTITEMTYPE_CONFIGOBJECT)
                {
                    parentObject = parentItem.Data as ConfigObject;
                    if (parentObject == null) { return; }
                    parentObjID = parentObject.ObjectID;
                    typeParam = mListResourceTypeParams.FirstOrDefault(t => t.TypeID == parentObject.ObjectType);
                    if (typeParam == null) { return; }
                    if (!typeParam.HasChildList) { return; }
                    childType = typeParam.ChildType;
                    typeParam = mListResourceTypeParams.FirstOrDefault(t => t.TypeID == childType);
                    if (typeParam == null) { return; }
                    if (typeParam.ParentID == 0)
                    {
                        int type = childType;
                        listConfigObjects = mListConfigObjects.Where(o => o.ObjectType == type).ToList();
                    }
                    else
                    {
                        int type = childType;
                        long id = parentObjID;
                        listConfigObjects = mListConfigObjects.Where(o => o.ObjectType == type && o.ParentID == id).ToList();
                    }
                }
                if (parentItem.Type == S1110Consts.OBJECTITEMTYPE_CONFIGGROUP)
                {
                    parentGroup = parentItem.Data as ConfigGroup;
                    if (parentGroup == null) { return; }
                    parentObject = parentGroup.ConfigObject;
                    if (parentObject == null) { return; }
                    parentObjID = parentObject.ObjectID;
                    childType = parentGroup.ChildType;
                    typeParam = mListResourceTypeParams.FirstOrDefault(t => t.TypeID == childType);
                    if (typeParam == null) { return; }
                    if (typeParam.ParentID == 0)
                    {
                        listConfigObjects = mListConfigObjects.Where(o => o.ObjectType == childType).ToList();
                    }
                    else
                    {
                        listConfigObjects = mListConfigObjects.Where(o => o.ObjectType == childType && o.ParentID == parentObjID).ToList();
                    }
                }
                if (parentObject == null) { return; }
                for (int i = 0; i < listConfigObjects.Count; i++)
                {
                    ConfigObject configObject = listConfigObjects[i];
                    ObjectItem item = new ObjectItem();
                    item.Type = S1110Consts.OBJECTITEMTYPE_CONFIGOBJECT;
                    item.Name = configObject.Name;
                    item.Description = configObject.Description;
                    item.Icon = string.Format("Images/{0}", configObject.Icon);
                    item.Data = configObject;
                    configObject.ObjectItem = item;
                    parentObject.ListChildObjects.Add(configObject);
                    CreateConfigGroupItems(item);
                    CreateConfigObjectItems(item);
                    parentItem.AddChild(item);
                    mListObjectItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region Create New ConfigObject

        private void PreCreateNewConfigObject(ObjectItem parentItem, int objType)
        {
            try
            {
                if (objType == S1110Consts.RESOURCE_CHANNEL)
                {
                    UCAddChannel uc = new UCAddChannel();
                    uc.MainPage = this;
                    uc.ParentItem = parentItem;
                    uc.ListResourceTypeParams = mListResourceTypeParams;
                    uc.ListResourceGroupParams = mListResourceGroupParams;
                    uc.ListPropertyInfos = mListResourcePropertyInfos;
                    uc.ListPropertyValues = mListResourcePropertyValues;
                    uc.ListConfigObjects = mListConfigObjects;
                    uc.ListAllBasicInfos = mListBasicInfoDatas;
                    PopupPanel.Title = "Add Channels";
                    PopupPanel.Content = uc;
                    PopupPanel.IsOpen = true;
                }
                else if (objType == S1110Consts.RESOURCE_SCREENCHANNEL)
                {
                    UCAddScreenChannel uc = new UCAddScreenChannel();
                    uc.MainPage = this;
                    uc.ParentItem = parentItem;
                    uc.ListResourceTypeParams = mListResourceTypeParams;
                    uc.ListResourceGroupParams = mListResourceGroupParams;
                    uc.ListPropertyInfos = mListResourcePropertyInfos;
                    uc.ListPropertyValues = mListResourcePropertyValues;
                    uc.ListConfigObjects = mListConfigObjects;
                    uc.ListAllBasicInfos = mListBasicInfoDatas;
                    PopupPanel.Title = "Add Channels";
                    PopupPanel.Content = uc;
                    PopupPanel.IsOpen = true;
                }
                else if (objType == S1110Consts.RESOURCE_PBXDEVICE)
                {
                    UCAddPBXDevice uc = new UCAddPBXDevice();
                    uc.MainPage = this;
                    uc.ParentItem = parentItem;
                    uc.ListResourceTypeParams = mListResourceTypeParams;
                    uc.ListResourceGroupParams = mListResourceGroupParams;
                    uc.ListPropertyInfos = mListResourcePropertyInfos;
                    uc.ListPropertyValues = mListResourcePropertyValues;
                    uc.ListConfigObjects = mListConfigObjects;
                    uc.ListAllCTITypeInfos = mListBasicInfoDatas;
                    PopupPanel.Title = "Add PBXDevice";
                    PopupPanel.Content = uc;
                    PopupPanel.IsOpen = true;
                }
                else
                {
                    CreateNewConfigObject(parentItem, objType);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateNewConfigObject(ObjectItem parentItem, int objType)
        {
            try
            {
                //检查最大个数限制
                if (!CheckChildObjectMaxinum(parentItem, objType)) { return; }
                ConfigObject parentObject;
                ConfigGroup parentGroup;
                ResourceTypeParam typeParam = mListResourceTypeParams.FirstOrDefault(t => t.TypeID == objType);
                if (typeParam == null) { return; }
                int parentType = typeParam.ParentID;
                parentObject = parentItem.Data as ConfigObject;
                if (parentObject == null)
                {
                    parentGroup = parentItem.Data as ConfigGroup;
                    if (parentGroup == null) { return; }
                    parentObject = parentGroup.ConfigObject;
                    if (parentObject == null) { return; }
                }
                List<ConfigObject> listConfigObject = mListConfigObjects.Where(o => o.ObjectType == objType).OrderBy(o => o.ObjectID).ToList();
                long objID = objType * (long)Math.Pow(10, 16) + 1;
                string strMasterSlaver = "1";
                //Key是资源在整个系统中的序号（从零开始），而ID是同一父对象下资源的序号（从零开始）
                //确定ObjID
                for (int i = 0; i < listConfigObject.Count; i++)
                {
                    //如果被占用，递增1
                    ConfigObject temp = listConfigObject[i];
                    if (objID == temp.ObjectID)
                    {
                        objID++;
                    }
                    var prop =
                        temp.ListProperties.FirstOrDefault(p => p.PropertyID == S1110Consts.PROPERTYID_MASTERSLAVER);
                    if (prop != null)
                    {
                        if (prop.Value == "1")
                        {
                            strMasterSlaver = "2";
                        }
                    }
                }
                //Key实际上就是ObjID的尾数
                int key = (int)(objID - (objType * (long)Math.Pow(10, 16) + 1));
                //确定ID
                int id = 0;
                //如果类型的父级类型为0，则他的ID与Key一致
                if (parentType == 0)
                {
                    id = key;
                }
                else
                {
                    listConfigObject =
                        parentObject.ListChildObjects.Where(o => o.ObjectType == objType).OrderBy(o => o.ID).ToList();
                    //遍历同一父对象下该类型的资源
                    for (int i = 0; i < listConfigObject.Count; i++)
                    {
                        ConfigObject temp = listConfigObject[i];
                        if (id == temp.ID)
                        {
                            //如果被占用，递增1
                            id++;
                        }
                    }
                }
                ConfigObject configObject = ConfigObject.CreateObject(objType);
                configObject.ObjectID = objID;
                configObject.Icon = typeParam.Icon;
                configObject.TypeParam = typeParam;
                configObject.ListAllTypeParams = mListResourceTypeParams;
                configObject.ListAllObjects = mListConfigObjects;
                configObject.ListAllBasicInfos = mListBasicInfoDatas;
                List<ObjectPropertyInfo> listPropertyInfos =
                       mListResourcePropertyInfos.Where(p => p.ObjType == objType).ToList();
                for (int i = 0; i < listPropertyInfos.Count; i++)
                {
                    ObjectPropertyInfo info = listPropertyInfos[i];
                    int propertyID = info.PropertyID;
                    ResourceProperty propertyValue = new ResourceProperty();
                    propertyValue.ObjID = configObject.ObjectID;
                    propertyValue.ObjType = configObject.ObjectType;
                    propertyValue.PropertyID = info.PropertyID;
                    propertyValue.Value = info.DefaultValue;
                    propertyValue.EncryptMode = info.EncryptMode;
                    propertyValue.MultiValueMode = info.MultiValueMode;
                    if (propertyID == S1110Consts.PROPERTYID_KEY)
                    {
                        propertyValue.Value = key.ToString();
                    }
                    if (propertyID == S1110Consts.PROPERTYID_ID)
                    {
                        propertyValue.Value = id.ToString();
                    }
                    if (typeParam.ParentID != 0)
                    {
                        if (propertyID == S1110Consts.PROPERTYID_PARENTOBJID)
                        {
                            propertyValue.Value = parentObject.ObjectID.ToString();
                        }
                    }
                    if (typeParam.IsMasterSlaver)
                    {
                        if (propertyID == S1110Consts.PROPERTYID_MASTERSLAVER)
                        {
                            propertyValue.Value = strMasterSlaver;
                        }
                    }
                    configObject.ListProperties.Add(propertyValue);
                    mListResourcePropertyValues.Add(propertyValue);
                }
                configObject.GetBasicPropertyValues();
                mListConfigObjects.Add(configObject);
                parentObject.ListChildObjects.Add(configObject);
                CreateNewConfigObjectItem(parentItem, configObject);

                #region 写操作日志

                var optID = string.Format("1110{0}06", configObject.ObjectType);
                string strOptLog = string.Format("{0}({1})", configObject.Name,
                    Utils.FormatOptLogString(string.Format("OBJ{0}", configObject.ObjectType)));
                App.WriteOperationLog(optID, ConstValue.OPT_RESULT_SUCCESS, strOptLog);

                #endregion
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateNewConfigObjectItem(ObjectItem parentItem, ConfigObject configObject)
        {
            try
            {
                ObjectItem item = new ObjectItem();
                item.Type = S1110Consts.OBJECTITEMTYPE_CONFIGOBJECT;
                item.Name = configObject.Name;
                item.Description = configObject.Description;
                item.Icon = string.Format("Images/{0}", configObject.Icon);
                item.Data = configObject;
                configObject.ObjectItem = item;
                CreateConfigGroupItems(item);
                parentItem.AddChild(item);
                mListObjectItems.Add(item);
                parentItem.IsExpanded = true;
                item.IsSelected = true;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        public ObjectItem CreateNewObjectItem(ObjectItem parentItem, ConfigObject configObject, bool isSelected)
        {
            try
            {
                ObjectItem item = new ObjectItem();
                item.Type = S1110Consts.OBJECTITEMTYPE_CONFIGOBJECT;
                item.Name = configObject.Name;
                item.Description = configObject.Description;
                item.Icon = string.Format("Images/{0}", configObject.Icon);
                item.Data = configObject;
                configObject.ObjectItem = item;
                CreateConfigGroupItems(item);
                parentItem.AddChild(item);
                mListObjectItems.Add(item);
                parentItem.IsExpanded = true;
                if (isSelected)
                {
                    item.IsSelected = true;
                }
                return item;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
                return null;
            }
        }

        public void CreateNewChannelConfigObjectItem(ObjectItem parentItem, ConfigObject configObject)
        {
            try
            {
                ObjectItem item = new ObjectItem();
                item.Type = S1110Consts.OBJECTITEMTYPE_CONFIGOBJECT;
                item.Name = configObject.Name;
                item.Description = configObject.Description;
                item.Icon = string.Format("Images/{0}", configObject.Icon);
                item.Data = configObject;
                configObject.ObjectItem = item;
                CreateConfigGroupItems(item);
                parentItem.AddChild(item);
                mListObjectItems.Add(item);
                parentItem.IsExpanded = true;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region Remove ConfigObject

        private void RemoveConfigObject(ConfigObject configObject, bool ask, bool check)
        {
            try
            {
                if (configObject == null) { return; }
                int type = configObject.ObjectType;
                long id = configObject.ObjectID;
                if (check)
                {
                    //检查最小对象个数
                    int minNumber;
                    var group = mListResourceGroupParams.FirstOrDefault(g => g.ChildType == type);
                    if (group == null)
                    {
                        var typeParam = mListResourceTypeParams.FirstOrDefault(t => t.ChildType == configObject.ObjectType);
                        if (typeParam == null) { return; }
                        if (!typeParam.HasChildList) { return; }
                        minNumber = typeParam.IntValue01;
                    }
                    else
                    {
                        minNumber = group.IntValue01;
                    }
                    var objs = mListConfigObjects.Where(o => o.ObjectType == type).ToList();
                    int count = objs.Count;
                    if (count <= minNumber)
                    {
                        App.ShowExceptionMessage(string.Format("{0}\r\n\r\n{1}\t{2}",
                            App.GetMessageLanguageInfo("007", "Can not Less than least of this type of resource"),
                            App.GetLanguageInfo(string.Format("OBJ{0}", type), type.ToString()),
                            minNumber));
                        return;
                    }
                }
                if (ask)
                {
                    //确认删除
                    var result = MessageBox.Show(string.Format("{0}\r\n\r\n{1}\t{2}",
                        App.GetMessageLanguageInfo("009", "Confirm remove this resource?"),
                        App.GetLanguageInfo(string.Format("OBJ{0}", type), type.ToString()),
                        configObject.Name),
                        App.AppName,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes) { return; }
                }
                //先删除子级资源对象
                for (int i = configObject.ListChildObjects.Count - 1; i >= 0; i--)
                {
                    //不用确认以及检查最小数
                    RemoveConfigObject(configObject.ListChildObjects[i], false, false);
                }
                //加入到待删除列表中，以便提交数据库的时候进行删除
                if (!mListRemoveObjectIDs.Contains(id))
                {
                    mListRemoveObjectIDs.Add(id);
                }
                //从集合中移除相关的属性值和资源对象
                mListResourcePropertyValues.RemoveAll(p => p.ObjID == id);
                mListConfigObjects.RemoveAll(o => o.ObjectID == id);

                #region 写操作日志

                var optID = string.Format("1110{0}07", configObject.ObjectType);
                string strOptLog = string.Format("{0}({1})", configObject.Name,
                    Utils.FormatOptLogString(string.Format("OBJ{0}", configObject.ObjectType)));
                App.WriteOperationLog(optID, ConstValue.OPT_RESULT_SUCCESS, strOptLog);

                #endregion

                //特殊处理
                RemoveConfigObject(configObject);
                var item = configObject.ObjectItem;
                if (item == null) { return; }
                var parentItem = item.Parent as ObjectItem;
                if (parentItem == null) { return; }
                ConfigObject parentObject;
                ConfigGroup parentGroup;
                parentObject = parentItem.Data as ConfigObject;
                //从上级对象的ListChildObjects集合中移除此对象
                if (parentObject == null)
                {
                    parentGroup = parentItem.Data as ConfigGroup;
                    if (parentGroup == null) { return; }
                    parentObject = parentGroup.ConfigObject;
                    if (parentObject == null) { return; }
                    parentObject.ListChildObjects.RemoveAll(o => o.ObjectID == id);
                }
                else
                {
                    parentObject.ListChildObjects.RemoveAll(o => o.ObjectID == id);
                }
                //从上级节点的孩子中移除此节点
                parentItem.Children.Remove(item);
                mListObjectItems.Remove(item);
                parentItem.IsSelected = true;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void RemoveConfigObject(ConfigObject configObject)
        {
            ResourceProperty propertyValue;
            List<ConfigObject> listObjects;
            switch (configObject.ObjectType)
            {
                //主备服务器的，删除其中之一，另外一个自动变为主服务器
                case S1110Consts.RESOURCE_LICENSESERVER:
                    listObjects =
                        mListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_LICENSESERVER).ToList();
                    for (int i = 0; i < listObjects.Count; i++)
                    {
                        propertyValue = listObjects[i].ListProperties.FirstOrDefault(p => p.PropertyID == S1110Consts.PROPERTYID_MASTERSLAVER);
                        if (propertyValue != null)
                        {
                            propertyValue.Value = "1";
                        }
                        listObjects[i].GetBasicPropertyValues();
                        listObjects[i].RefreshObjectItem();
                    }
                    break;
                case S1110Consts.RESOURCE_ALARMSERVER:
                    listObjects =
                        mListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_ALARMSERVER).ToList();
                    for (int i = 0; i < listObjects.Count; i++)
                    {
                        propertyValue = listObjects[i].ListProperties.FirstOrDefault(p => p.PropertyID == S1110Consts.PROPERTYID_MASTERSLAVER);
                        if (propertyValue != null)
                        {
                            propertyValue.Value = "1";
                        }
                        listObjects[i].GetBasicPropertyValues();
                        listObjects[i].RefreshObjectItem();
                    }
                    break;
                //对于通道，删除了一个通道，之后的通道的ID需要减1，保证通道号从0开始，并且连续
                case S1110Consts.RESOURCE_CHANNEL:
                case S1110Consts.RESOURCE_SCREENCHANNEL:
                    int objType = configObject.ObjectType;
                    int channelID = configObject.ID;
                    var voice = mListConfigObjects.FirstOrDefault(o => o.ObjectID == configObject.ParentID);
                    if (voice != null)
                    {
                        listObjects = voice.ListChildObjects.Where(o => o.ObjectType == objType).OrderBy(o => o.ID).ToList();
                        for (int i = 0; i < listObjects.Count; i++)
                        {
                            ConfigObject temp = listObjects[i];
                            if (i >= channelID)
                            {
                                temp.SetPropertyValue(S1110Consts.PROPERTYID_ID, (i - 1).ToString());
                                temp.GetNameAndDescription();
                                temp.RefreshObjectItem();
                            }
                        }
                    }
                    break;
            }
        }

        #endregion


        #region Save ConfigObject Data

        private void SaveCurrentConfigObject()
        {
            try
            {
                if (mCurrentConfigObject == null) { return; }
                //保存参数前先检查一下配置参数
                if (!CheckObjectConfig(mCurrentConfigObject)) { return; }

                string optID = string.Format("1110{0}08", mCurrentConfigObject.ObjectType);
                string parentID = string.Format("1110{0}", mCurrentConfigObject.ObjectType);
                OperationInfo optInfo =
                    ListOperations.FirstOrDefault(o => o.ID.ToString() == optID && o.ParentID.ToString() == parentID);
                if (optInfo == null) { return; }
                MyWaiter.Visibility = Visibility.Visible;
                SetStatuMessage(App.GetMessageLanguageInfo("002", "Saving config information"));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    PreSaveConfigObject();
                    SaveConfigObjectPropertyInfo(mCurrentConfigObject);
                    DoRemoveConfigObject();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Hidden;
                    SetStatuMessage(string.Empty);

                    #region 写操作日志

                    string strOptLog = string.Format("{0}({1})", mCurrentConfigObject.Name,
                        Utils.FormatOptLogString(string.Format("OBJ{0}", mCurrentConfigObject.ObjectType)));
                    App.WriteOperationLog(S1110Consts.OPT_SAVECURRENTCONFIG.ToString(), ConstValue.OPT_RESULT_SUCCESS, strOptLog);

                    #endregion

                    App.ShowInfoMessage(App.GetMessageLanguageInfo("012", "Save Data End"));
                    //GenerateResourceXml(mCurrentConfigObject);
                    NotifyResourceChanged();
                };
                mWorker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void SaveAllConfigObject()
        {
            try
            {
                if (mRootItem == null) { return; }
                var rootObject = mRootItem.Data as ConfigObject;
                if (rootObject == null) { return; }
                //保存参数前先检查一下配置参数
                if (!CheckObjectConfig(rootObject)) { return; }

                MyWaiter.Visibility = Visibility.Visible;
                SetStatuMessage(App.GetMessageLanguageInfo("002", "Saving config information"));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    PreSaveConfigObject();
                    SaveConfigObjectPropertyInfo(rootObject);
                    DoRemoveConfigObject();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Hidden;
                    SetStatuMessage(string.Empty);

                    #region 写操作日志

                    App.WriteOperationLog(S1110Consts.OPT_SAVEALLCONFIG.ToString(),
                        ConstValue.OPT_RESULT_SUCCESS, string.Empty);

                    #endregion

                    App.ShowInfoMessage(App.GetMessageLanguageInfo("012", "Save Data End"));
                    //GenerateResourceXml(rootObject);
                    NotifyResourceChanged();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void PreSaveConfigObject()
        {
            try
            {
                //保存参数前的准备工作
                for (int i = 0; i < mListConfigObjects.Count; i++)
                {
                    ConfigObject configObject = mListConfigObjects[i];
                    //重新设置一下特殊属性值
                    configObject.SetBasicPropertyValues();
                    //其他准备前的操作
                    switch (configObject.ObjectType)
                    {
                        case S1110Consts.RESOURCE_VOICESERVER:

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void SaveConfigObjectPropertyInfo(ConfigObject configObject)
        {
            try
            {
                //保存对象自身属性信息
                if (configObject == null) { return; }
                //如果待删除列表中存在此资源，表示此资源又重新添加回来，不应该再被删除了
                if (mListRemoveObjectIDs.Contains(configObject.ObjectID))
                {
                    mListRemoveObjectIDs.Remove(configObject.ObjectID);
                }
                //设置一下特殊属性值
                configObject.SetBasicPropertyValues();
                //判断保存权限
                if (configObject.ObjectType != 0)
                {
                    string optID = string.Format("1110{0}08", configObject.ObjectType);
                    string parentID = string.Format("1110{0}", configObject.ObjectType);
                    OperationInfo optInfo =
                        ListOperations.FirstOrDefault(o => o.ID.ToString() == optID && o.ParentID.ToString() == parentID);
                    if (optInfo == null) { return; }
                }
                OperationReturn optReturn;
                int count = configObject.ListProperties.Count;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1110Codes.SaveResourcePropertyData;
                webRequest.ListData.Add(configObject.ObjectID.ToString());
                webRequest.ListData.Add(count.ToString());
                for (int i = 0; i < count; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(configObject.ListProperties[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11101Client client = new Service11101Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo, "Service11101"));
                WebHelper.SetServiceClient(client);
                SetStatuMessage(string.Format("{0}\t{1}\t{2}", App.GetMessageLanguageInfo("002", "Saving..."),
                    App.GetLanguageInfo(string.Format("OBJ{0}", configObject.ObjectType),
                        configObject.ObjectType.ToString()),
                    configObject.Name));
                //App.WriteLog("Test", string.Format("Saving... {0}\t{1}", configObject.ObjectType, configObject.ObjectID));
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                //保存子对象信息
                SaveChildObjectData(configObject);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void SaveChildObjectData(ConfigObject configObject)
        {
            try
            {
                //保存对象的子对象信息
                if (configObject == null) { return; }
                int objType = configObject.ObjectType;
                var typeParam = mListResourceTypeParams.FirstOrDefault(t => t.TypeID == objType);
                if (typeParam != null)
                {
                    //如果对象本身具有子级对象
                    if (typeParam.HasChildList)
                    {
                        int childType = typeParam.ChildType;
                        SaveChildObjectData(configObject, childType);
                    }
                }
                //保存各子对象组
                var groups = (from pgs in mListResourceGroupParams
                              where pgs.TypeID == configObject.ObjectType
                                    && pgs.GroupType == ResourceGroupType.ChildList
                              select pgs).ToList();
                for (int i = 0; i < groups.Count; i++)
                {
                    ResourceGroupParam group = groups[i];
                    int childType = group.ChildType;
                    SaveChildObjectData(configObject, childType);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void SaveChildObjectData(ConfigObject configObject, int childType)
        {
            try
            {
                //保存子对象信息
                if (configObject == null) { return; }
                int objType = configObject.ObjectType;
                long objID = configObject.ObjectID;
                List<ConfigObject> listChildObjects =
                        configObject.ListChildObjects.Where(o => o.ObjectType == childType).ToList();
                int count = listChildObjects.Count;
                for (int j = 0; j < count; j++)
                {
                    //保存子对象的属性信息
                    SaveConfigObjectPropertyInfo(listChildObjects[j]);
                }
                string strIsParent = "0";
                var typeParam = mListResourceTypeParams.FirstOrDefault(t => t.TypeID == childType);
                if (typeParam != null)
                {
                    if (typeParam.ParentID > 0)
                    {
                        strIsParent = "1";
                    }
                }
#pragma warning disable 219
                string strLog = string.Empty;
#pragma warning restore 219
                strLog += string.Format("{0};", objType);
                strLog += string.Format("{0};", objID);
                strLog += string.Format("{0};", strIsParent);
                strLog += string.Format("{0};", childType);
                strLog += string.Format("{0};", count);
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1110Codes.SaveResourceChildObjectData;
                webRequest.ListData.Add(objType.ToString());
                webRequest.ListData.Add(objID.ToString());
                webRequest.ListData.Add(strIsParent);
                webRequest.ListData.Add(childType.ToString());
                webRequest.ListData.Add(count.ToString());
                for (int j = 0; j < count; j++)
                {
                    webRequest.ListData.Add(listChildObjects[j].ObjectID.ToString());
                    strLog += string.Format("{0};", listChildObjects[j].ObjectID);
                }
                App.MonitorHelper.AddWebRequest(webRequest);
                //App.WriteLog("Test", string.Format("SavingChildObj...\t{0}",strLog));
                Service11101Client client = new Service11101Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service11101"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void DoRemoveConfigObject()
        {
            try
            {
                if (mListRemoveObjectIDs.Count > 0)
                {
                    //将待删除列表中的资源对象彻底删除，然后清空待删除列表
                    int count = mListRemoveObjectIDs.Count;
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = App.Session;
                    webRequest.Code = (int)S1110Codes.RemoveResourceObjectData;
                    webRequest.ListData.Add(count.ToString());
                    for (int i = 0; i < count; i++)
                    {
                        webRequest.ListData.Add(mListRemoveObjectIDs[i].ToString());
                    }
                    App.MonitorHelper.AddWebRequest(webRequest);
                    SetStatuMessage(string.Format("{0}", App.GetMessageLanguageInfo("015", "Remove resource object...")));
                    Service11101Client client = new Service11101Client(
                        WebHelper.CreateBasicHttpBinding(App.Session),
                        WebHelper.CreateEndpointAddress(
                            App.Session.AppServerInfo, "Service11101"));
                    WebHelper.SetServiceClient(client);
                    WebReturn webReturn = client.DoOperation(webRequest);
                    App.MonitorHelper.AddWebReturn(webReturn);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    }
                    mListRemoveObjectIDs.Clear();
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region Sync ConfigObject

        private void SyncConfigObjects(ObjectItem parentItem, int intObjType)
        {
            try
            {
                switch (intObjType)
                {
                    case S1110Consts.RESOURCE_PBXDEVICE:
                        SyncPBXDevices(parentItem);
                        break;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        public void SynPBXDevice(ObjectItem parentItem)
        {
            SyncPBXDevices(parentItem);
        }

        private void SyncPBXDevices(ObjectItem parentItem)
        {
            try
            {
                //获取ParentObject
                ConfigObject parentObject;
                ConfigGroup parentGroup;
                parentObject = parentItem.Data as ConfigObject;
                if (parentObject == null)
                {
                    parentGroup = parentItem.Data as ConfigGroup;
                    if (parentGroup == null) { return; }
                    parentObject = parentGroup.ConfigObject;
                }
                if (parentObject == null) { return; }

                //获取已经存在的PBXDevices
                List<ConfigObject> listExistPBxDeviceObjs =
                    mListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_PBXDEVICE).ToList();
                List<PBXDeviceObject> listExistPBXDevices = new List<PBXDeviceObject>();
                for (int i = 0; i < listExistPBxDeviceObjs.Count; i++)
                {
                    var pbx = listExistPBxDeviceObjs[i] as PBXDeviceObject;
                    if (pbx != null)
                    {
                        listExistPBXDevices.Add(pbx);
                    }
                }

                //获取所有已经配置的通道,去除重复分机号的通道
                List<ConfigObject> listObjs =
                    mListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_CHANNEL).ToList();
                List<VoiceChannelObject> listChannels = new List<VoiceChannelObject>();
                for (int i = 0; i < listObjs.Count; i++)
                {
                    var channel = listObjs[i] as VoiceChannelObject;
                    if (channel == null) { continue; }
                    var temp = listChannels.FirstOrDefault(c => c.Extension == channel.Extension);
                    if (temp != null) { continue; }         //分机号重复的忽略调
                    listChannels.Add(channel);
                }

                //遍历每个通道，根据启动方式创建PBX对象
                int count = 0;
                for (int i = 0; i < listChannels.Count; i++)
                {
                    var channel = listChannels[i];
                    string strExtension = channel.Extension;
                    int startType = channel.StartType;
                    if (startType != 1005
                        && startType != 1011
                        && startType != 1014
                        && startType != 1015
                        && startType != 1103
                        && startType != 1104
                        && startType != 1105
                        && startType != 1106
                        && startType != 1107
                        && startType != 1113)
                    {
                        //非CTI相关的启动方式的忽略掉
                        continue;
                    }
                    var temp = listExistPBXDevices.FirstOrDefault(o => o.DeviceName == channel.Extension);
                    if (temp != null)
                    {
                        //已经存在的跳过
                        continue;
                    }

                    //新增PBXDevice
                    ConfigObject configObject = GetNewConfigObject(S1110Consts.RESOURCE_PBXDEVICE);
                    var pbxDevice = configObject as PBXDeviceObject;
                    if (pbxDevice != null)
                    {
                        pbxDevice.CTIType = 0;
                        pbxDevice.DeviceName = strExtension;
                        switch (startType)
                        {
                            case 1005:      //模拟线，CTI
                            case 1011:      //数字线，CTI
                            case 1103:      //Voip，CTI
                            case 1104:      //模拟线，摘挂机，CTI收消息
                            case 1105:      //数字线，D信道，CTI收消息
                            case 1107:      //Voip，D信道，CTI收消息
                                pbxDevice.DeviceType = 1;
                                pbxDevice.MonitorMode = 2;
                                break;
                            case 1014:      //E1/T1中继线，CTI
                            case 1106:      //E1/T1中继线，D信道，CTI收消息
                                pbxDevice.DeviceType = 4;
                                pbxDevice.MonitorMode = 3;
                                break;
                            case 1015:      //E1/T1中继线，单步会议
                                pbxDevice.DeviceType = 5;
                                pbxDevice.MonitorMode = 4;
                                break;
                            case 1113:      //Voip，IP会议
                                pbxDevice.DeviceType = 7;
                                pbxDevice.MonitorMode = 4;
                                break;
                        }
                        pbxDevice.SetBasicPropertyValues();
                        pbxDevice.GetBasicPropertyValues();
                    }
                    parentObject.ListChildObjects.Add(configObject);
                    //创建节点
                    CreateNewObjectItem(parentItem, configObject, false);
                    count++;
                }

                #region 写操作日志

                var optID = string.Format("1110{0}11", S1110Consts.RESOURCE_PBXDEVICE);
                string strOptLog = string.Format("{0}", count);
                App.WriteOperationLog(optID, ConstValue.OPT_RESULT_SUCCESS, strOptLog);

                #endregion

                var lister = PanelPropertyList.Child as UCResourceObjectLister;
                if (lister != null)
                {
                    lister.ReloadData();
                }

            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region Check Config

        private bool CheckObjectConfig(bool alert)
        {
            try
            {
                var lister = PanelPropertyList.Child as UCResourcePropertyLister;
                if (lister == null) { return false; }
                CheckResult result = lister.CheckConfig();
                if (result.Result)
                {
                    if (alert)
                    {
                        string strMsg = string.Format("{0}", App.GetMessageLanguageInfo("010", "Check config end."));
                        var configObject = result.ConfigObject;
                        if (configObject != null)
                        {
                            strMsg += string.Format("\r\n\r\n[{0}]{1}",
                               App.GetLanguageInfo(string.Format("OBJ{0}", configObject.ObjectType.ToString("000")),
                                   configObject.ObjectType.ToString()), configObject.Name);
                        }
                        App.ShowInfoMessage(strMsg);
                    }
                }
                else
                {
                    string strMsg = string.Format("{0}", App.GetMessageLanguageInfo("014", "Check config fail."));
                    ConfigObject configObject = result.ConfigObject;
                    if (configObject != null)
                    {
                        strMsg += string.Format("\r\n\r\n[{0}]{1}",
                            App.GetLanguageInfo(string.Format("OBJ{0}", configObject.ObjectType.ToString("000")),
                                configObject.ObjectType.ToString()), configObject.Name);

                        int propertyID = result.PropertyID;
                        if (propertyID > 0)
                        {
                            strMsg += string.Format("\r\n\r\n{0}",
                               App.GetLanguageInfo(
                                   string.Format("PRO{0}{1}", configObject.ObjectType.ToString("000"),
                                       propertyID.ToString("000")), propertyID.ToString()));
                        }
                    }
                    strMsg += string.Format("\r\n\r\n{0}",
                        App.GetMessageLanguageInfo(string.Format("014{0}", result.Code.ToString("000")),
                            string.Format("[{0}]{1}", result.Code, result.Message)));
                    App.ShowExceptionMessage(strMsg);
                    return false;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
            return true;
        }

        private bool CheckObjectConfig(ConfigObject obj)
        {
            var result = CheckSingObjectConfig(obj);
            if (!result.Result)
            {
                string strMsg = string.Format("{0}", App.GetMessageLanguageInfo("014", "Check config fail."));
                ConfigObject configObject = result.ConfigObject;
                if (configObject != null)
                {
                    strMsg += string.Format("\r\n\r\n[{0}]{1}",
                        App.GetLanguageInfo(string.Format("OBJ{0}", configObject.ObjectType.ToString("000")),
                            configObject.ObjectType.ToString()), configObject.Name);

                    int propertyID = result.PropertyID;
                    if (propertyID > 0)
                    {
                        strMsg += string.Format("\r\n\r\n{0}",
                           App.GetLanguageInfo(
                               string.Format("PRO{0}{1}", configObject.ObjectType.ToString("000"),
                                   propertyID.ToString("000")), propertyID.ToString()));
                    }
                }
                strMsg += string.Format("\r\n\r\n{0}",
                    App.GetMessageLanguageInfo(string.Format("014{0}", result.Code.ToString("000")),
                        string.Format("[{0}]{1}", result.Code, result.Message)));
                App.ShowExceptionMessage(strMsg);
                return false;
            }
            return true;
        }

        private CheckResult CheckSingObjectConfig(ConfigObject configObject)
        {
            var result = configObject.CheckConfig();
            if (!result.Result)
            {
                return result;
            }
            for (int i = 0; i < configObject.ListChildObjects.Count; i++)
            {
                result = CheckSingObjectConfig(configObject.ListChildObjects[i]);
                if (!result.Result)
                {
                    return result;
                }
            }
            return result;
        }

        #endregion


        #region Others

        private void SetMultiSelectedItems(ObjectItem objectItem)
        {
            try
            {
                if (objectItem == null) { return; }
                ConfigObject configObject = objectItem.Data as ConfigObject;
                //当前不是对象节点，清空
                if (configObject == null)
                {
                    mMultiSelectedItem.Clear();
                    return;
                }
                ObjectItem parentItem = objectItem.Parent as ObjectItem;
                if (parentItem == null) { return; }
                //如果当前节点的父节点不等于列表中对象的父节点，重置
                if (parentItem != mMultiSelectedItem.ParentItem)
                {
                    mMultiSelectedItem.Reset(objectItem);
                    return;
                }
                //类型不同，重置
                if (configObject.ObjectType != mMultiSelectedItem.ObjType)
                {
                    mMultiSelectedItem.Reset(objectItem);
                    return;
                }
                //如果没有按下Ctrl或Shift键,重置
                if (!Keyboard.IsKeyDown(Key.LeftCtrl)
                    && !Keyboard.IsKeyDown(Key.LeftShift)
                    && !Keyboard.IsKeyDown(Key.RightCtrl)
                    && !Keyboard.IsKeyDown(Key.RightShift))
                {
                    mMultiSelectedItem.Reset(objectItem);
                    return;
                }
                //按下了Ctrl键或Shift键
                //追加当前节点
                mMultiSelectedItem.AddItem(objectItem);
                //如果按下的是Shift键，追加一系列节点
                if (Keyboard.IsKeyDown(Key.LeftShift)
                    || Keyboard.IsKeyDown(Key.RightShift))
                {
                    //找到最小与最大索引
                    int current = parentItem.Children.IndexOf(objectItem);
                    int min = current;
                    int max = current;
                    for (int i = 0; i < parentItem.Children.Count; i++)
                    {
                        var child = parentItem.Children[i] as ObjectItem;
                        if (child == null) { continue; }
                        if (child.IsMultiSelected && i < min)
                        {
                            min = i;
                        }
                        if (child.IsMultiSelected && i > max)
                        {
                            max = i;
                        }
                    }
                    //最小到最大索引之间的节点全部加入列表中
                    for (int i = min; i < max; i++)
                    {
                        mMultiSelectedItem.AddItem(parentItem.Children[i] as ObjectItem);
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void ResetOperationButtons()
        {
            try
            {
                mListBasicOperations.Clear();
                OperationInfo info = ListOperations.FirstOrDefault(o => o.ID == S1110Consts.OPT_SAVECURRENTCONFIG);
                if (info != null)
                {
                    mListBasicOperations.Add(info);
                }
                info = ListOperations.FirstOrDefault(o => o.ID == S1110Consts.OPT_SAVEALLCONFIG);
                if (info != null)
                {
                    mListBasicOperations.Add(info);
                }
                info = ListOperations.FirstOrDefault(o => o.ID == S1110Consts.OPT_WIZARD);
                if (info != null)
                {
                    mListBasicOperations.Add(info);
                }
                if (mCurrentObjectItem != null)
                {
                    ConfigObject configObject;
                    ConfigGroup configGroup;
                    switch (mCurrentObjectItem.Type)
                    {
                        case S1110Consts.OBJECTITEMTYPE_CONFIGOBJECT:
                            configObject = mCurrentObjectItem.Data as ConfigObject;
                            if (configObject != null)
                            {
                                string optID = string.Format("1110{0}07", configObject.ObjectType);
                                info = ListOperations.FirstOrDefault(o => o.ID.ToString() == optID);
                                if (info != null)
                                {
                                    mListBasicOperations.Add(info);
                                }
                                optID = string.Format("1110{0}99", configObject.ObjectType);
                                info = ListOperations.FirstOrDefault(o => o.ID.ToString() == optID);
                                if (info != null)
                                {
                                    mListBasicOperations.Add(info);
                                }
                                ResourceTypeParam typeParam =
                                    mListResourceTypeParams.FirstOrDefault(t => t.TypeID == configObject.ObjectType);
                                if (typeParam == null) { return; }
                                if (!typeParam.HasChildList) { return; }
                                int childType = typeParam.ChildType;
                                optID = string.Format("1110{0}06", childType);
                                info = ListOperations.FirstOrDefault(o => o.ID.ToString() == optID);
                                if (info != null)
                                {
                                    mListBasicOperations.Add(info);
                                }
                            }
                            break;
                        case S1110Consts.OBJECTITEMTYPE_CONFIGGROUP:
                            configGroup = mCurrentObjectItem.Data as ConfigGroup;
                            if (configGroup != null)
                            {
                                if (configGroup.GroupType == ResourceGroupType.ChildList)
                                {
                                    int childType = configGroup.ChildType;
                                    string optID = string.Format("1110{0}06", childType);
                                    info = ListOperations.FirstOrDefault(o => o.ID.ToString() == optID);
                                    if (info != null)
                                    {
                                        mListBasicOperations.Add(info);
                                    }
                                    optID = string.Format("1110{0}11", childType);
                                    info = ListOperations.FirstOrDefault(o => o.ID.ToString() == optID);
                                    if (info != null)
                                    {
                                        mListBasicOperations.Add(info);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateOptButtons()
        {
            PanelOperationButtons.Children.Clear();
            OperationInfo item;
            Button btn;
            for (int i = 0; i < mListBasicOperations.Count; i++)
            {
                item = mListBasicOperations[i];
                //基本操作按钮
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelOperationButtons.Children.Add(btn);
            }
            PanelOjectListButtons.Children.Clear();
            item = ListOperations.FirstOrDefault(o => o.ID == S1110Consts.OPT_RELOADCONFIGDATA);
            if (item != null)
            {
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptObjectListButtonStyle");
                PanelOjectListButtons.Children.Add(btn);
            }
        }

        private void CreatePropertyList()
        {
            try
            {
                PanelPropertyList.Child = null;
                if (mCurrentConfigObject == null) { return; }
                string optID, parentID;
                parentID = string.Format("1110{0}", mCurrentConfigObject.ObjectType);
                optID = string.Format("1110{0}00", mCurrentConfigObject.ObjectType);
                OperationInfo optInfo = ListOperations.FirstOrDefault(o => o.ID.ToString() == optID && o.ParentID.ToString() == parentID);
                if (optInfo == null) { return; }
                UCResourcePropertyLister lister = new UCResourcePropertyLister();
                lister.PropertyListerEvent += PropertyLister_PropertyListerEvent;
                lister.ObjectItem = mCurrentObjectItem;
                lister.ListObjectPropertyInfos = mListResourcePropertyInfos;
                lister.ListResourceGroupParams = mListResourceGroupParams;
                lister.ListPropertyValues = mListResourcePropertyValues;
                lister.ListConfigObjects = mListConfigObjects;
                lister.ListSftpUsers = mListSftpUsers;
                lister.ListBasicInfoDatas = mListBasicInfoDatas;
                lister.MainPage = this;
                PanelPropertyList.Child = lister;
                optID = string.Format("1110{0}02", mCurrentConfigObject.ObjectType);
                optInfo = ListOperations.FirstOrDefault(o => o.ID.ToString() == optID && o.ParentID.ToString() == parentID);
                if (optInfo == null)
                {
                    lister.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateMultiPropertyList()
        {
            try
            {
                PanelPropertyList.Child = null;
                if (mCurrentConfigObject == null) { return; }
                string optID, parentID;
                parentID = string.Format("1110{0}", mCurrentConfigObject.ObjectType);
                optID = string.Format("1110{0}00", mCurrentConfigObject.ObjectType);
                OperationInfo optInfo = ListOperations.FirstOrDefault(o => o.ID.ToString() == optID && o.ParentID.ToString() == parentID);
                if (optInfo == null) { return; }
                UCMultiResourcePropertyLister lister = new UCMultiResourcePropertyLister();
                //lister.PropertyListerEvent += PropertyLister_PropertyListerEvent;
                //lister.ObjectItem = mCurrentObjectItem;
                lister.ListObjectPropertyInfos = mListResourcePropertyInfos;
                lister.ListResourceGroupParams = mListResourceGroupParams;
                lister.MultiSelectedItem = mMultiSelectedItem;
                //lister.ListPropertyValues = mListResourcePropertyValues;
                //lister.ListConfigObjects = mListConfigObjects;
                //lister.ListSftpUsers = mListSftpUsers;
                //lister.MainPage = this;
                PanelPropertyList.Child = lister;
                optID = string.Format("1110{0}02", mCurrentConfigObject.ObjectType);
                optInfo = ListOperations.FirstOrDefault(o => o.ID.ToString() == optID && o.ParentID.ToString() == parentID);
                if (optInfo == null)
                {
                    lister.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        public void CreateChildObjectList()
        {
            try
            {
                //if (mCurrentObjectItem == null || mCurrentConfigGroup == null) { return; }
                //if (mCurrentConfigGroup.GroupType != ResourceGroupType.ChildList) { return; }
                //switch (mCurrentConfigGroup.ChildType)
                //{
                //    case S1110Consts.RESOURCE_MACHINE:
                //        UCResourceObjectLister lister210 = new UCResourceObjectLister();
                //        lister210.ObjectItem = mCurrentObjectItem;
                //        PanelPropertyList.Child = lister210;
                //        break;
                //    default:
                //        UCChildObjectLister lister = new UCChildObjectLister();
                //        lister.ChildObjectListerEvent += ChildObjectLister_ChildObjectListerEvent;
                //        lister.ObjectItem = mCurrentObjectItem;
                //        lister.ListConfigObjects = mListConfigObjects;
                //        lister.ListResourceTypeParams = mListResourceTypeParams;
                //        PanelPropertyList.Child = lister;
                //        break;
                //}
                UCResourceObjectLister lister = new UCResourceObjectLister();
                lister.ResourceObjectListerEvent += ResourceObjectLister_ResourceObjectListerEvent;
                lister.ObjectItem = mCurrentObjectItem;
                lister.ListAllConfigObjects = mListConfigObjects;
                lister.ListAllTypeParams = mListResourceTypeParams;
                PanelPropertyList.Child = lister;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void ResetNameDescription()
        {
            LbCurrentObject.DataContext = null;
            if (mCurrentConfigObject != null)
            {
                if (mCurrentConfigObject.ObjectID != 0)
                {
                    LbCurrentObject.DataContext = mCurrentConfigObject;
                }
                else
                {
                    if (mCurrentConfigGroup != null)
                    {
                        LbCurrentObject.DataContext = mCurrentConfigGroup;
                    }
                }
            }
            TxtDescription.DataContext = mCurrentDescription;
        }

        private void ReloadResourceData()
        {
            try
            {
                MyWaiter.Visibility = Visibility.Visible;
                SetStatuMessage(App.GetMessageLanguageInfo("001", "Loading config information..."));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    LoadBasicResourceInfos();
                    InitConfigObjects();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Hidden;
                    SetStatuMessage(string.Empty);

                    #region 写操作日志

                    App.WriteOperationLog(S1110Consts.OPT_RELOADCONFIGDATA.ToString(), ConstValue.OPT_RESULT_SUCCESS, string.Empty);

                    #endregion

                    mRootItem.Children.Clear();
                    CreateObjectItems();
                    if (mRootItem.Children.Count > 0)
                    {
                        var child = mRootItem.Children[0] as ObjectItem;
                        if (child != null)
                        {
                            child.IsExpanded = true;
                            child.IsSelected = true;
                        }
                    }
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private long GetConfigObjectObjID(int objType)
        {
            //Key是资源在整个系统中的序号（从零开始），而ID是同一父对象下资源的序号（从零开始）
            long objID = objType * (long)Math.Pow(10, 16) + 1;
            List<ConfigObject> listObjs =
                mListConfigObjects.Where(o => o.ObjectType == objType).OrderBy(o => o.ObjectID).ToList();
            for (int i = 0; i < listObjs.Count; i++)
            {
                var temp = listObjs[i];
                if (objID == temp.ObjectID)
                {
                    //如果被占用，递增1
                    objID++;
                }
            }
            return objID;
        }

        public ConfigObject GetNewConfigObj(int objType)
        {
            return GetNewConfigObject(objType);
        }

        private ConfigObject GetNewConfigObject(int objType)
        {
            ConfigObject configObject = ConfigObject.CreateObject(objType);
            try
            {
                var typeParam =
               mListResourceTypeParams.FirstOrDefault(t => t.TypeID == S1110Consts.RESOURCE_PBXDEVICE);
                if (typeParam == null)
                {
                    App.WriteLog("GetNewConfigObject", string.Format("TypeParam is null"));
                    return configObject;
                }
                long objID = GetConfigObjectObjID(S1110Consts.RESOURCE_PBXDEVICE);
                int key = (int)(objID - (S1110Consts.RESOURCE_PBXDEVICE * (long)Math.Pow(10, 16) + 1));
                int id = key;
                configObject.ObjectID = objID;
                configObject.Icon = typeParam.Icon;
                configObject.TypeParam = typeParam;
                configObject.ListAllTypeParams = mListResourceTypeParams;
                configObject.ListAllObjects = mListConfigObjects;
                configObject.ListAllBasicInfos = mListBasicInfoDatas;
                List<ObjectPropertyInfo> listPropertyInfos =
                   mListResourcePropertyInfos.Where(p => p.ObjType == objType).ToList();
                for (int i = 0; i < listPropertyInfos.Count; i++)
                {
                    ObjectPropertyInfo info = listPropertyInfos[i];
                    int propertyID = info.PropertyID;
                    ResourceProperty propertyValue = new ResourceProperty();
                    propertyValue.ObjID = configObject.ObjectID;
                    propertyValue.ObjType = configObject.ObjectType;
                    propertyValue.PropertyID = info.PropertyID;
                    propertyValue.Value = info.DefaultValue;
                    propertyValue.EncryptMode = info.EncryptMode;
                    propertyValue.MultiValueMode = info.MultiValueMode;
                    if (propertyID == S1110Consts.PROPERTYID_KEY)
                    {
                        propertyValue.Value = key.ToString();
                    }
                    if (propertyID == S1110Consts.PROPERTYID_ID)
                    {
                        propertyValue.Value = id.ToString();
                    }
                    configObject.ListProperties.Add(propertyValue);
                    mListResourcePropertyValues.Add(propertyValue);
                }
                configObject.GetBasicPropertyValues();
                mListConfigObjects.Add(configObject);
            }
            catch (Exception ex)
            {
                App.WriteLog("GetNewConfigObject", string.Format("Fail.\t{0}", ex.Message));
                return configObject;
            }
            return configObject;
        }

        private bool CheckChildObjectMaxinum(ObjectItem parentItem, int childType)
        {
            int intMaxValue;
            ConfigObject parentObject = parentItem.Data as ConfigObject;
            if (parentObject == null)
            {
                ConfigGroup parentGroup = parentItem.Data as ConfigGroup;
                if (parentGroup == null)
                {
                    return false;
                }
                ResourceGroupParam groupParam = parentGroup.GroupInfo;
                if (groupParam == null)
                {
                    return false;
                }
                intMaxValue = groupParam.IntValue02;
            }
            else
            {
                ResourceTypeParam typeParam =
                    mListResourceTypeParams.FirstOrDefault(t => t.TypeID == parentObject.ObjectType);
                if (typeParam == null)
                {
                    return false;
                }
                if (!typeParam.HasChildList)
                {
                    return false;
                }
                intMaxValue = typeParam.IntValue02;
            }
            var temp = (from pvs in mListResourcePropertyValues
                        where pvs.ObjType == childType
                        group pvs by pvs.ObjID).ToList();
            int count = temp.Count;
            if (count >= intMaxValue)
            {
                App.ShowExceptionMessage(string.Format("{0}\r\n\r\n{1}\t{2}",
                    App.GetMessageLanguageInfo("008", "Over maxinum value"),
                    App.GetLanguageInfo(string.Format("OBJ{0}", childType), childType.ToString()),
                    intMaxValue));
                return false;
            }
            return true;
        }

        private bool CheckRemoveOperation(ObjectItem item)
        {
            //检查删除权限
            ConfigObject configObject = item.Data as ConfigObject;
            if (configObject == null)
            {
                return false;
            }
            OperationInfo optInfo;
            string optID = string.Format("1110{0}07", configObject.ObjectType);
            string parentID = string.Format("1110{0}", configObject.ObjectType);
            optInfo = ListOperations.FirstOrDefault(o => o.ID.ToString() == optID && o.ParentID.ToString() == parentID);
            return optInfo != null;
        }

        private void GenerateResourceXml(ConfigObject configObject)
        {
            try
            {
                //生成资源的Xml文档
                if (configObject == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1110Codes.GenerateResourceXml;
                webRequest.ListData.Add(configObject.ObjectID.ToString());
                Service11101Client client = new Service11101Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service11101"));
                MyWaiter.Visibility = Visibility.Visible;
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    App.ShowInfoMessage(string.Format("Generator Resource xml end.\t{0}", webReturn.Message));
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Hidden;
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void NotifyResourceChanged()
        {
            try
            {
                //通知Service00服务资源更新
                if (mRootItem == null) { return; }
                ConfigObject rootObject = mRootItem.Data as ConfigObject;
                if (rootObject == null) { return; }
                List<NotifyObjectInfo> listNotifyObjects = new List<NotifyObjectInfo>();
                //取得待通知的资源集合
                NotifyResourceChanged(rootObject, listNotifyObjects);
                if (listNotifyObjects.Count > 0)
                {
                    MyWaiter.Visibility = Visibility.Visible;
                    SetStatuMessage(App.GetMessageLanguageInfo("006", "Nortify resource changed"));
                    mWorker = new BackgroundWorker();
                    mWorker.DoWork += (s, de) => NotifyResourceChanged(listNotifyObjects);
                    mWorker.RunWorkerCompleted += (s, re) =>
                    {
                        mWorker.Dispose();
                        MyWaiter.Visibility = Visibility.Hidden;
                        SetStatuMessage(string.Empty);
                    };
                    mWorker.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void NotifyResourceChanged(ConfigObject configObject, List<NotifyObjectInfo> listNotifyObjects)
        {
            //递归遍历所有资源，如果资源的HostAddress不在列表中就加入
            if (configObject == null) { return; }
            for (int i = 0; i < configObject.ListChildObjects.Count; i++)
            {
                ConfigObject child = configObject.ListChildObjects[i];
                ResourceProperty propertyAddress =
                    child.ListProperties.FirstOrDefault(p => p.PropertyID == S1110Consts.PROPERTYID_HOSTADDRESS);
                ResourceProperty propertyPort =
                    child.ListProperties.FirstOrDefault(p => p.PropertyID == S1110Consts.PROPERTYID_HOSTPORT);
                //无效的地址过滤调
                if (propertyAddress != null
                    && !string.IsNullOrEmpty(propertyAddress.Value)
                    && propertyAddress.Value != "0.0.0.0")
                {
                    var temp = listNotifyObjects.FirstOrDefault(o => o.Address == propertyAddress.Value);
                    if (temp == null)
                    {
                        NotifyObjectInfo item = new NotifyObjectInfo();
                        item.ObjID = child.ObjectID;
                        item.ObjType = child.ObjectType;
                        item.Address = propertyAddress.Value;
                        item.Port = propertyPort != null ? propertyPort.Value : string.Empty;
                        listNotifyObjects.Add(item);
                    }
                }
                NotifyResourceChanged(child, listNotifyObjects);
            }
        }

        private void NotifyResourceChanged(List<NotifyObjectInfo> listNotifyObjects)
        {
            try
            {
                //向UMP服务器上的Service00发送通知请求
                OperationReturn optReturn;
                string address = App.Session.AppServerInfo.Address;
                int port = 8009;
                int count = listNotifyObjects.Count;
                ServerRequestInfo request = new ServerRequestInfo();
                request.ServerHost = address;
                request.ServerPort = port;
                request.Command = (int)ServerRequestCommand.SetResourceChanged;
                request.ListData.Add(count.ToString());
                for (int i = 0; i < count; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(listNotifyObjects[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    request.ListData.Add(optReturn.Data.ToString());
                }
                optReturn = XMLHelper.SeriallizeObject(request);
                if (!optReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S1110Codes.GetServerInfo;
                webRequest.Data = optReturn.Data.ToString();
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11102Client client = new Service11102Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service11102"));
                SetStatuMessage(string.Format("{0}\t{1}\t{2}",
                    App.GetMessageLanguageInfo("006", "Nortify resource changed"),
                    address, port));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                App.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        public void SetBusy(bool isBusy, string msg)
        {
            if (isBusy)
            {
                MyWaiter.Visibility = Visibility.Visible;
                SetStatuMessage(msg);
            }
            else
            {
                MyWaiter.Visibility = Visibility.Hidden;
                SetStatuMessage(string.Empty);
            }
        }

        public void SetBusy(bool isBusy)
        {
            SetBusy(false, string.Empty);
        }

        #endregion


        #region Refresh ConfigItem Data

        private void RefreshConfigItemData(ObjectItem objectItem)
        {
            try
            {
                //更新资源属性信息
                ConfigObject configObject;
                ConfigGroup configGroup;
                configObject = objectItem.Data as ConfigObject;
                if (configObject == null)
                {
                    configGroup = objectItem.Data as ConfigGroup;
                    if (configGroup != null)
                    {
                        configObject = configGroup.ConfigObject;
                    }
                }
                if (configObject == null) { return; }
                switch (configObject.ObjectType)
                {
                    case S1110Consts.RESOURCE_LICENSESERVER:
                    case S1110Consts.RESOURCE_ALARMSERVER:
                        RefreshMasterSlaverConfigItemData(configObject);
                        break;
                    default:
                        configObject.GetBasicPropertyValues();
                        configObject.RefreshObjectItem();
                        break;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        public void RefreshConfigObjectItem(ObjectItem objectItem)
        {
            RefreshConfigItemData(objectItem);
        }

        public void RefreshCenterView()
        {
            var uc= this.PanelPropertyList.Child as UCResourceObjectLister;
            if (uc != null)
            {
                uc.ReloadData();
            }
        }

        private void RefreshMasterSlaverConfigItemData(ConfigObject configObject)
        {
            try
            {
                //对于主备关系的资源，其中一个主备性质变了，另一个资源的主备性质要跟着变
                var objItem = configObject.ObjectItem;
                if (objItem == null) { return; }
                var parentItem = objItem.Parent as ObjectItem;
                if (parentItem == null) { return; }
                for (int i = 0; i < parentItem.Children.Count; i++)
                {
                    objItem = parentItem.Children[i] as ObjectItem;
                    if (objItem == null) { continue; }
                    configObject = objItem.Data as ConfigObject;
                    if (configObject == null) { continue; }
                    configObject.GetBasicPropertyValues();
                    configObject.RefreshObjectItem();
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        protected override void PageHead_PageHeadEvent(object sender, PageHeadEventArgs e)
        {
            base.PageHead_PageHeadEvent(sender, e);

            switch (e.Code)
            {
                //切换主题
                case 100:
                    ThemeInfo themeInfo = e.Data as ThemeInfo;
                    if (themeInfo != null)
                    {
                        ThemeInfo = themeInfo;
                        App.Session.ThemeInfo = themeInfo;
                        App.Session.ThemeName = themeInfo.Name;
                        ChangeTheme();
                        SendThemeChangeMessage();
                    }
                    break;
                //切换语言
                case 110:
                    LangTypeInfo langType = e.Data as LangTypeInfo;
                    if (langType != null)
                    {
                        LangTypeInfo = langType;
                        App.Session.LangTypeInfo = langType;
                        App.Session.LangTypeID = langType.LangID;
                        MyWaiter.Visibility = Visibility.Visible;
                        mWorker = new BackgroundWorker();
                        mWorker.DoWork += (s, de) => App.InitAllLanguageInfos();
                        mWorker.RunWorkerCompleted += (s, re) =>
                        {
                            mWorker.Dispose();
                            MyWaiter.Visibility = Visibility.Hidden;
                            ChangeLanguage();
                            PopupPanel.ChangeLanguage();
                            SendLanguageChangeMessage();
                        };
                        mWorker.RunWorkerAsync();
                    }
                    break;
            }
        }

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem == null) { return; }
                switch (optItem.ID)
                {
                    //px
                    case S1110Consts.OPT_WIZARD:
                        UCWizardMachineNumber ucwizard = new UCWizardMachineNumber();
                        ucwizard.MainPage = this;
                        ucwizard.WizardHelper = mWizardHelper;
                        ucwizard.ListConfigObjects = this.mListConfigObjects;
                        ucwizard.mListResourcePropertyInfos = mListResourcePropertyInfos;
                        PopupPanel.Title = "Config Wizard";
                        PopupPanel.Content = ucwizard;
                        PopupPanel.IsOpen = true;
                        break;
                    //px-end
                    case S1110Consts.OPT_SAVECURRENTCONFIG:
                        SaveCurrentConfigObject();
                        break;
                    case S1110Consts.OPT_SAVEALLCONFIG:
                        SaveAllConfigObject();
                        break;
                    case S1110Consts.OPT_RELOADCONFIGDATA:
                        ReloadResourceData();
                        break;
                    default:
                        string strOptID = optItem.ID.ToString();
                        if (strOptID.Length == 9)
                        {
                            string objType = strOptID.Substring(4, 3);
                            strOptID = strOptID.Substring(7, 2);
                            int intObjType, intOptID;
                            if (!int.TryParse(objType, out intObjType) || !int.TryParse(strOptID, out intOptID))
                            {
                                return;
                            }
                            ObjectItem objectItem = TreeResourceObject.SelectedItem as ObjectItem;
                            if (objectItem == null) { return; }
                            switch (intOptID)
                            {
                                case 6:
                                    //CreateNewConfigObject(objectItem, intObjType);
                                    PreCreateNewConfigObject(objectItem, intObjType);
                                    break;
                                case 7:
                                    RemoveConfigObject(objectItem.Data as ConfigObject, true, true);
                                    break;
                                case 11:
                                    SyncConfigObjects(objectItem, intObjType);
                                    break;
                                case 99:
                                    CheckObjectConfig(true);
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        void TreeResourceObject_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var objectItem = TreeResourceObject.SelectedItem as ObjectItem;
            if (objectItem == null) { return; }
            objectItem.IsExpanded = true;

            //设置当前节点及当前组，当前对象
            mCurrentObjectItem = objectItem;
            int flag = 0;       //0：当前节点是资源对象，1：当前节点是子对象列表节点
            ConfigObject configObject = objectItem.Data as ConfigObject;
            if (configObject != null)
            {
                mCurrentConfigObject = configObject;
                flag = 0;
            }
            ConfigGroup configGroup = objectItem.Data as ConfigGroup;
            if (configGroup != null)
            {
                mCurrentConfigGroup = configGroup;
                mCurrentConfigObject = configGroup.ConfigObject;
                if (configGroup.GroupType == ResourceGroupType.ChildList)
                {
                    flag = 1;
                }
            }

            ResetOperationButtons();
            CreateOptButtons();

            //设置选择的节点列表
            //SetMultiSelectedItems(objectItem);
            if (mMultiSelectedItem.ListObjectItems.Count > 1)
            {
                //如果当前选择的节点不止一个
                CreateMultiPropertyList();
            }
            else
            {
                switch (flag)
                {
                    case 0:
                        CreatePropertyList();
                        break;
                    case 1:
                        CreateChildObjectList();
                        break;
                    default:
                        PanelPropertyList = null;
                        break;
                }
            }

            mCurrentDescription = null;
            ResetNameDescription();
        }

        void PropertyLister_PropertyListerEvent(object sender, RoutedPropertyChangedEventArgs<PropertyListerEventEventArgs> e)
        {
            try
            {
                var lister = sender as UCResourcePropertyLister;
                if (lister == null) { return; }
                var args = e.NewValue;
                if (args != null)
                {
                    var code = args.Code;
                    switch (code)
                    {
                        //PropertyItemChangedEvent
                        case 1:
                            var itemArgs = args.Data as PropertyItemChangedEventArgs;
                            if (itemArgs != null)
                            {
                                var item = itemArgs.PropertyItem;
                                if (item != null)
                                {
                                    var propertyInfo = item.PropertyInfo;
                                    if (propertyInfo != null)
                                    {
                                        mCurrentDescription = new DescriptionInfo(0, propertyInfo);
                                        ResetNameDescription();
                                    }
                                }
                            }
                            break;
                        //PropertyValueChangedEvent
                        case 2:
                            var objectItem = lister.ObjectItem;
                            if (objectItem != null)
                            {
                                //当资源属性改变时立即刷新对应的ConfigObject
                                RefreshConfigItemData(objectItem);
                                ResetNameDescription();
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        void ResourceObjectLister_ResourceObjectListerEvent(object sender, RoutedPropertyChangedEventArgs<ResourceObjectListerEventEventArgs> e)
        {
            try
            {
                var lister = sender as UCResourceObjectLister;
                if (lister == null) { return; }
                var args = e.NewValue;
                if (args == null) { return; }
                ConfigObject item;
                List<ConfigObject> listItems;
                ConfigObject configObject;
                ObjectItem objItem;
                var code = args.Code;
                switch (code)
                {
                    case 1:
                        item = args.Data as ConfigObject;
                        if (item == null) { return; }
                        mCurrentDescription = new DescriptionInfo(1, item);
                        ResetNameDescription();
                        break;
                    case 2:
                        item = args.Data as ConfigObject;
                        if (item == null) { return; }
                        configObject = item;
                        objItem = configObject.ObjectItem;
                        if (objItem == null) { return; }
                        objItem.IsSelected = true;
                        break;
                    case 3:
                        listItems = args.Data as List<ConfigObject>;
                        if (listItems == null) { return; }
                        for (int i = 0; i < listItems.Count; i++)
                        {
                            item = listItems[i];
                            configObject = item;
                            if (configObject == null) { return; }
                            objItem = configObject.ObjectItem;
                            if (objItem == null) { return; }
                            if (!CheckRemoveOperation(objItem)) { return; }
                            RemoveConfigObject(objItem.Data as ConfigObject, false, false);
                        }
                        lister.ReloadData();
                        break;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region ChangTheme

        public override void ChangeTheme()
        {
            base.ChangeTheme();

            bool bPage = false;
            if (AppServerInfo != null)
            {
                //优先从服务器上加载资源文件
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                        AppServerInfo.Protocol,
                        AppServerInfo.Address,
                        AppServerInfo.Port,
                        ThemeInfo.Name
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception)
                {
                    //App.ShowExceptionMessage("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //App.ShowExceptionMessage("2" + ex.Message);
                }
            }

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/Themes/Default/UMPS1110/MainPageStatic.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //App.ShowExceptionMessage("3" + ex.Message);
            }
        }

        #endregion


        #region ChangLanguage

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                //Operation
                for (int i = 0; i < ListOperations.Count; i++)
                {
                    ListOperations[i].Display = App.GetLanguageInfo(string.Format("FO{0}", ListOperations[i].ID),
                        ListOperations[i].ID.ToString());
                }
                CreateOptButtons();

                //Other
                if (PageHead != null)
                {
                    PageHead.AppName = App.GetLanguageInfo("1110003", "UMP Resource Management");
                }
                LbObjectList.Text = App.GetLanguageInfo("1110101", "Object List");
                LbOperations.Text = App.GetLanguageInfo("1110103", "Operations");
                LbDescriptions.Text = App.GetLanguageInfo("1110104", "Description");

                //Object List
                for (int i = 0; i < mListObjectItems.Count; i++)
                {
                    ObjectItem item = mListObjectItems[i];
                    ConfigObject configObject = item.Data as ConfigObject;
                    if (configObject != null)
                    {
                        configObject.RefreshObjectItem();
                    }
                    ConfigGroup configGroup = item.Data as ConfigGroup;
                    if (configGroup != null)
                    {
                        configGroup.Name =
                            App.GetLanguageInfo(
                                string.Format("1110GRP{0}{1}", configGroup.TypeID.ToString("000"), configGroup.GroupID.ToString("000")),
                                configGroup.Name);
                        configGroup.Description = App.GetLanguageInfo(
                            string.Format("1110GRPD{0}{1}", configGroup.TypeID.ToString("000"), configGroup.GroupID.ToString("000")),
                            configGroup.Name);
                        configGroup.RefreshData();
                    }
                }
                if (mCurrentDescription != null)
                {
                    mCurrentDescription.RefreshData();
                }
                ResetNameDescription();

                //Popup
                PopupPanel.ChangeLanguage();

                if (PanelPropertyList != null)
                {
                    var lister = PanelPropertyList.Child as ILanguagePage;
                    if (lister != null)
                    {
                        lister.ChangeLanguage();
                    }
                }
            }
            catch (Exception ex)
            {
                App.WriteLog("ChangeLang", string.Format("ChangeLang fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region NetPipeMessage

        protected override void App_NetPipeEvent(WebRequest webRequest)
        {
            base.App_NetPipeEvent(webRequest);

            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    var code = webRequest.Code;
                    var session = webRequest.Session;
                    var strData = webRequest.Data;
                    switch (code)
                    {
                        case (int)RequestCode.SCLanguageChange:
                            LangTypeInfo langTypeInfo =
                               App.Session.SupportLangTypes.FirstOrDefault(l => l.LangID.ToString() == strData);
                            if (langTypeInfo != null)
                            {
                                LangTypeInfo = langTypeInfo;
                                App.Session.LangTypeInfo = langTypeInfo;
                                App.Session.LangTypeID = langTypeInfo.LangID;
                                if (MyWaiter != null)
                                {
                                    MyWaiter.Visibility = Visibility.Visible;
                                }
                                mWorker = new BackgroundWorker();
                                mWorker.DoWork += (s, de) => App.InitAllLanguageInfos();
                                mWorker.RunWorkerCompleted += (s, re) =>
                                {
                                    mWorker.Dispose();
                                    if (MyWaiter != null)
                                    {
                                        MyWaiter.Visibility = Visibility.Hidden;
                                    }
                                    ChangeLanguage();
                                    if (PopupPanel != null)
                                    {
                                        PopupPanel.ChangeLanguage();
                                    }
                                };
                                mWorker.RunWorkerAsync();
                            }
                            break;
                        case (int)RequestCode.SCThemeChange:
                            ThemeInfo themeInfo = App.Session.SupportThemes.FirstOrDefault(t => t.Name == strData);
                            if (themeInfo != null)
                            {
                                ThemeInfo = themeInfo;
                                App.Session.ThemeInfo = themeInfo;
                                App.Session.ThemeName = themeInfo.Name;
                                ChangeTheme();
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    App.ShowExceptionMessage(ex.Message);
                }
            }));
        }

        #endregion


        #region Monitor    什么鬼？

        public OperationReturn GetAllConfigObjectInfos()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //这里使用StringBuilder，效率会大大的提高
                StringBuilder sbInfo = new StringBuilder();
                for (int i = 0; i < mListConfigObjects.Count; i++)
                {
                    ConfigObject configObject = mListConfigObjects[i];
                    sbInfo.Append(string.Format("OBJ:[{0}]{1}({2})\r\n", configObject.ObjectType, configObject.Name,
                        configObject.ObjectID));
                    for (int j = 0; j < configObject.ListProperties.Count; j++)
                    {
                        ResourceProperty propertyValue = configObject.ListProperties[j];
                        string strValue = propertyValue.Value;
                        if (propertyValue.ListOtherValues.Count > 0)
                        {
                            for (int k = 0; k < propertyValue.ListOtherValues.Count; k++)
                            {
                                strValue += string.Format(";{0}", propertyValue.ListOtherValues[k]);
                            }
                        }
                        sbInfo.Append(string.Format("[{0}]{1}:{2}\r\n", propertyValue.ObjID, propertyValue.PropertyID,
                            strValue));
                    }
                }
                optReturn.Data = sbInfo.ToString();
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        #endregion

    }
}
