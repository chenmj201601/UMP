//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b7d046c2-04aa-4b79-8184-98b20ed15d30
//        CLR Version:              4.0.30319.18444
//        Name:                     ResourceXmlHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ResourceXmls
//        File Name:                ResourceXmlHelper
//
//        created by Charley at 2015/3/5 14:02:59
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.ResourceXmls.Wcf11101;

namespace VoiceCyber.UMP.ResourceXmls
{
    /// <summary>
    /// 帮助类
    /// 1、将资源对象数据从数据库中读出，生成指定的Xml文件
    /// 2、从指定的xml文件中解析出资源信息（暂未实现）
    /// 
    /// 注：
    /// 1、生成xml属性中如果是加密的，可以指定EncryptObject来自定义加密内容，请参考IEncryptObject的说明
    /// 2、调用RefreshData方法（首次调用Init方法后会自动调用RefreshData方法）后，就可以从ListResourceObject集合中获取资源的信息
    /// 3、可以通过设置GenerateOption来灵活的自定义生成xml的方式，请参考GenerateOperation的说明
    /// </summary>
    public class ResourceXmlHelper
    {
        #region  Public Property
        /// <summary>
        /// SessionInfo，会话信息
        /// </summary>
        public SessionInfo Session
        {
            set { mSession = value; }
        }
        /// <summary>
        /// 设置加密解密的处理对象
        /// </summary>
        public IEncryptable EncryptObject
        {
            set { mEncryptObject = value; }
        }
        /// <summary>
        /// 资源类型参数
        /// </summary>
        public List<ResourceTypeParam> ListResourceTypeParams
        {
            set
            {
                if (value != null)
                {
                    mListResourceTypeParams = value;
                }
            }
        }
        /// <summary>
        /// 属性分组参数
        /// </summary>
        public List<ResourceGroupParam> ListResourceGroupParams
        {
            set
            {
                if (value != null)
                {
                    mListResourceGroupParams = value;
                }
            }
        }
        /// <summary>
        /// 属性参数
        /// </summary>
        public List<ObjectPropertyInfo> ListResourcePropertyInfos
        {
            set
            {
                if (value != null)
                {
                    mListResourcePropertyInfos = value;
                }
            }
        }
        /// <summary>
        /// 属性值列表
        /// </summary>
        public List<ResourceProperty> ListResourcePropertyValues
        {
            set
            {
                if (value != null)
                {
                    mListResourcePropertyValues = value;
                }
            }
        }
        /// <summary>
        /// 获取资源对象列表
        /// </summary>
        public List<ResourceObject> ListResourceObjects
        {
            get { return mListResourceObjects; }
        }

        #endregion


        #region Memebers

        private SessionInfo mSession;
        private IEncryptable mEncryptObject;
        private List<ResourceTypeParam> mListResourceTypeParams;
        private List<ResourceGroupParam> mListResourceGroupParams;
        private List<ObjectPropertyInfo> mListResourcePropertyInfos;
        private List<ResourceProperty> mListResourcePropertyValues;
        private List<ResourceObject> mListResourceObjects;
        private List<BasicResourceInfo> mListBasicResourceInfos;
        private List<ResourceProperty> mListSingleResourcePropertyValues;

        #endregion


        /// <summary>
        /// 创建一个用于资源xml处理的帮助类
        /// </summary>
        public ResourceXmlHelper()
        {
            mListResourceTypeParams = new List<ResourceTypeParam>();
            mListResourceGroupParams = new List<ResourceGroupParam>();
            mListResourcePropertyInfos = new List<ObjectPropertyInfo>();
            mListResourcePropertyValues = new List<ResourceProperty>();
            mListResourceObjects = new List<ResourceObject>();
            mListBasicResourceInfos = new List<BasicResourceInfo>();
            mListSingleResourcePropertyValues = new List<ResourceProperty>();
        }
        /// <summary>
        /// 指定SessionInfo创建一个用于资源xml处理的帮助类
        /// </summary>
        /// <param name="session"></param>
        public ResourceXmlHelper(SessionInfo session)
            : this()
        {
            mSession = session;
        }


        #region Public Functions

        /// <summary>
        /// 初始化参数信息
        /// 1、加载资源类型参数
        /// 2、加载属性组参数
        /// 3、加载属性参数
        /// </summary>
        /// <returns></returns>
        public OperationReturn Init()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mSession == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("SessionInfo is null");
                }
                optReturn = LoadResourceTypeParams();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = LoadResourceGroupParams();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = LoadResourcePropertyInfos();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = RefreshData();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
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
        /// <summary>
        /// 刷新资源数据
        /// </summary>
        /// <returns></returns>
        public OperationReturn RefreshData()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mSession == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("SessionInfo is null");
                }
                //optReturn = LoadResourcePropertyValues();
                //if (!optReturn.Result)
                //{
                //    return optReturn;
                //}
                optReturn = LoadBasicResourceInfos();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = InitResourceObjects();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
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
        /// <summary>
        /// 清理配置参数，释放内存
        /// </summary>
        public void CleanData()
        {
            try
            {
                for (int i = 0; i < mListResourcePropertyValues.Count; i++)
                {
                    mListResourcePropertyValues[i] = null;
                }
                for (int i = 0; i < mListResourceObjects.Count; i++)
                {
                    mListResourceObjects[i] = null;
                }
                mListResourcePropertyValues.Clear();
                mListResourceObjects.Clear();
                GC.Collect();
            }
            catch { }
        }
        /// <summary>
        /// 生成指定资源对象的Xml文件
        /// </summary>
        /// <param name="resourceID"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public OperationReturn GenerateResourceXmlFile(long resourceID, string path)
        {
            return GenerateResourceXmlFile(resourceID, path, GenerateOption.Default);
        }
        /// <summary>
        /// 指定生成选项生成指定资源对象的Xml文件
        /// </summary>
        /// <param name="resourceID"></param>
        /// <param name="path"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public OperationReturn GenerateResourceXmlFile(long resourceID, string path, GenerateOption option)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                XmlDocument doc = new XmlDocument();
                //声明
                XmlNode declare = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
                doc.AppendChild(declare);
                optReturn = GenerateResourceXmlNode(resourceID, doc, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                doc.Save(path);
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
        /// <summary>
        /// 生成指定资源对象的Xml节点
        /// </summary>
        /// <param name="resourceID"></param>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        public OperationReturn GenerateResourceXmlNode(long resourceID, XmlNode parentNode)
        {
            return GenerateResourceXmlNode(resourceID, parentNode, GenerateOption.Default);
        }
        /// <summary>
        /// 指定生成选项生成指定资源对象的Xml节点
        /// </summary>
        /// <param name="resourceID"></param>
        /// <param name="parentNode"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public OperationReturn GenerateResourceXmlNode(long resourceID, XmlNode parentNode, GenerateOption option)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                ResourceObject resourceObject = mListResourceObjects.FirstOrDefault(r => r.ObjID == resourceID);
                if (resourceObject == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = string.Format("ResourceObject not exist.\t{0}", resourceID);
                    return optReturn;
                }
                optReturn = GenerateResourceAttribute(resourceObject, parentNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
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
        /// <summary>
        /// 生成该类型下所有资源的Xml文件
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public OperationReturn GenerateTypeXmlFile(int typeID, string path)
        {
            return GenerateTypeXmlFile(typeID, path, GenerateOption.Default);
        }
        /// <summary>
        /// 指定生成选项生成该类型下所有资源的Xml文件
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="path"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public OperationReturn GenerateTypeXmlFile(int typeID, string path, GenerateOption option)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                XmlDocument doc = new XmlDocument();
                //声明
                XmlNode declare = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
                doc.AppendChild(declare);
                optReturn = GenerateTypeXmlNode(typeID, doc, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                doc.Save(path);
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
        /// <summary>
        /// 生成该类型下所有资源的xml节点
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        public OperationReturn GenerateTypeXmlNode(int typeID, XmlNode parentNode)
        {
            return GenerateTypeXmlNode(typeID, parentNode, GenerateOption.Default);
        }
        /// <summary>
        /// 指定生成选项生成该类型下所有资源的xml节点
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="parentNode"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public OperationReturn GenerateTypeXmlNode(int typeID, XmlNode parentNode, GenerateOption option)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                optReturn = GenerateResourceType(typeID, parentNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
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
        /// <summary>
        /// 生成所有资源的Xml文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public OperationReturn GenerateAllResourceXmlFile(string path)
        {
            return GenerateAllResourceXmlFile(path, GenerateOption.Default);
        }
        /// <summary>
        /// 指定生成选项生成所有资源的Xml文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public OperationReturn GenerateAllResourceXmlFile(string path, GenerateOption option)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                XmlDocument doc = new XmlDocument();
                //声明
                XmlNode declare = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
                doc.AppendChild(declare);
                optReturn = GenerateAllResourceXmlNode(doc, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                doc.Save(path);
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
        /// <summary>
        /// 生成所有资源的Xml节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        public OperationReturn GenerateAllResourceXmlNode(XmlNode parentNode)
        {
            return GenerateAllResourceXmlNode(parentNode, GenerateOption.Default);
        }
        /// <summary>
        /// 指定生成选项生成所有资源的Xml节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public OperationReturn GenerateAllResourceXmlNode(XmlNode parentNode, GenerateOption option)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                XmlDocument doc = parentNode as XmlDocument;
                if (doc == null)
                {
                    doc = parentNode.OwnerDocument;
                }
                if (doc == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format("XmlDocument is null");
                    return optReturn;
                }
                //Configurations
                XmlElement configsNode = doc.CreateElement("Configurations");
                configsNode.SetAttribute("Version", "2.0");
                //Configuration
                XmlElement configNode = doc.CreateElement("Configuration");
                //LocalMachine
                XmlElement localMachineNode = doc.CreateElement("LocalMachine");
                localMachineNode.SetAttribute("SiteID", "0");
                configNode.AppendChild(localMachineNode);
                //Sites
                XmlElement sitesNode = doc.CreateElement("Sites");
                //Site
                XmlElement siteNode = doc.CreateElement("Site");
                siteNode.SetAttribute("ID", "0");
                //Resources
                XmlElement resourcesNode = doc.CreateElement("Resources");

                optReturn = GenerateResourceType(S1110Consts.RESOURCE_MACHINE, resourcesNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_LICENSESERVER, resourcesNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_DATATRANSFERSERVER, resourcesNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_CTIHUBSERVER, resourcesNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                //px
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_CTIDBBRIDGE, resourcesNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                optReturn = GenerateResourceType(S1110Consts.RESOURCE_ARCHIVESTRATEGY, resourcesNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_STORAGEDEVICE, resourcesNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_PBXDEVICE, resourcesNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMMONITOR, resourcesNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMSERVER, resourcesNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_DBBRIDGE, resourcesNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_SFTP, resourcesNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                siteNode.AppendChild(resourcesNode);

                optReturn = GenerateResourceType(S1110Consts.RESOURCE_CTICONNECTIONGROUPCOLLECTION, siteNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_CTIPOLICY, siteNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_FILEOPERATOR, siteNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMMONITORPARAM, siteNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMSERVERPARAM, siteNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                //XmlElement alarmServerParam = doc.CreateElement("AlarmServerParam");
                //optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMEMAILSERVER, alarmServerParam, option);
                //if (!optReturn.Result)
                //{
                //    return optReturn;
                //}
                //optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMABNORMALRECORD, alarmServerParam, option);
                //if (!optReturn.Result)
                //{
                //    return optReturn;
                //}
                //optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMSENDRULE, alarmServerParam, option);
                //if (!optReturn.Result)
                //{
                //    return optReturn;
                //}
                //optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMRECORDNUMBER, alarmServerParam, option);
                //if (!optReturn.Result)
                //{
                //    return optReturn;
                //}
                //siteNode.AppendChild(alarmServerParam);

                optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMLOGGINGPARAM, siteNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                optReturn = GenerateResourceType(S1110Consts.RESOURCE_VOICESERVER, siteNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_CMSERVER, siteNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_SCREENSERVER, siteNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_SPEECHANALYSISPARAM, siteNode, option);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                sitesNode.AppendChild(siteNode);
                configNode.AppendChild(sitesNode);
                configsNode.AppendChild(configNode);
                parentNode.AppendChild(configsNode);
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


        #region Init and Load

        private OperationReturn LoadResourceTypeParams()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mSession == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("SessionInfo is null");
                    return optReturn;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = mSession;
                webRequest.Code = (int)S1110Codes.GetResourceTypeParamList;
                webRequest.ListData.Add("0");
                Service11101Client client = new Service11101Client(
                    WebHelper.CreateBasicHttpBinding(mSession),
                    WebHelper.CreateEndpointAddress(
                        mSession.AppServerInfo,
                        "Service11101"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                if (webReturn.ListData == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("WebReturn ListData is null");
                    return optReturn;
                }
                List<ResourceTypeParam> listItems = new List<ResourceTypeParam>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<ResourceTypeParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    ResourceTypeParam item = optReturn.Data as ResourceTypeParam;
                    if (item == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Item is null");
                        return optReturn;
                    }
                    listItems.Add(item);
                }
                listItems = listItems.OrderBy(t => t.ParentID).ThenBy(t => t.OrderID).ToList();
                mListResourceTypeParams.Clear();
                for (int i = 0; i < listItems.Count; i++)
                {
                    mListResourceTypeParams.Add(listItems[i]);
                }
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

        private OperationReturn LoadResourceGroupParams()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mSession == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("SessionInfo is null");
                    return optReturn;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = mSession;
                webRequest.Code = (int)S1110Codes.GetResourceGroupParamList;
                webRequest.ListData.Add("0");
                Service11101Client client = new Service11101Client(
                    WebHelper.CreateBasicHttpBinding(mSession),
                    WebHelper.CreateEndpointAddress(
                        mSession.AppServerInfo,
                        "Service11101"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                if (webReturn.ListData == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("WebReturn ListData is null");
                    return optReturn;
                }
                List<ResourceGroupParam> listItems = new List<ResourceGroupParam>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<ResourceGroupParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    ResourceGroupParam item = optReturn.Data as ResourceGroupParam;
                    if (item == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Item is null");
                        return optReturn;
                    }
                    listItems.Add(item);
                }
                listItems = listItems.OrderBy(g => g.TypeID).ThenBy(g => g.ParentGroup).ThenBy(g => g.SortID).ToList();
                mListResourceGroupParams.Clear();
                for (int i = 0; i < listItems.Count; i++)
                {
                    mListResourceGroupParams.Add(listItems[i]);
                }
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

        private OperationReturn LoadResourcePropertyInfos()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mSession == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("SessionInfo is null");
                    return optReturn;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = mSession;
                webRequest.Code = (int)S1110Codes.GetResourcePropertyInfoList;
                webRequest.ListData.Add("0");
                Service11101Client client = new Service11101Client(
                    WebHelper.CreateBasicHttpBinding(mSession),
                    WebHelper.CreateEndpointAddress(
                        mSession.AppServerInfo,
                        "Service11101"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                if (webReturn.ListData == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("WebReturn ListData is null");
                    return optReturn;
                }
                List<ObjectPropertyInfo> listItems = new List<ObjectPropertyInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<ObjectPropertyInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    ObjectPropertyInfo item = optReturn.Data as ObjectPropertyInfo;
                    if (item == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Item is null");
                        return optReturn;
                    }
                    listItems.Add(item);
                }
                listItems = listItems.OrderBy(p => p.ObjType).ThenBy(p => p.GroupID).ThenBy(p => p.SortID).ToList();
                mListResourcePropertyInfos.Clear();
                for (int i = 0; i < listItems.Count; i++)
                {
                    mListResourcePropertyInfos.Add(listItems[i]);
                }
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

        private OperationReturn LoadBasicResourceInfos()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mSession == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("SessionInfo is null");
                    return optReturn;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = mSession;
                webRequest.Code = (int)S1110Codes.GetBasicResourceInfoList;
                webRequest.ListData.Add(mSession.UserID.ToString());
                webRequest.ListData.Add("0");
                Service11101Client client = new Service11101Client(
                    WebHelper.CreateBasicHttpBinding(mSession),
                    WebHelper.CreateEndpointAddress(
                        mSession.AppServerInfo,
                        "Service11101"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                if (webReturn.ListData == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("WebReturn ListData is null");
                    return optReturn;
                }
                List<BasicResourceInfo> listItems = new List<BasicResourceInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<BasicResourceInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    BasicResourceInfo item = optReturn.Data as BasicResourceInfo;
                    if (item == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Item is null");
                        return optReturn;
                    }
                    listItems.Add(item);
                }
                listItems = listItems.OrderBy(p => p.ObjectID).ToList();
                mListBasicResourceInfos.Clear();
                for (int i = 0; i < listItems.Count; i++)
                {
                    mListBasicResourceInfos.Add(listItems[i]);
                }
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

        private OperationReturn LoadResourcePropertyValues()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mSession == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("SessionInfo is null");
                    return optReturn;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = mSession;
                webRequest.Code = (int)S1110Codes.GetResourcePropertyValueList;
                webRequest.ListData.Add("0");
                webRequest.ListData.Add("0");
                Service11101Client client = new Service11101Client(
                    WebHelper.CreateBasicHttpBinding(mSession),
                    WebHelper.CreateEndpointAddress(
                        mSession.AppServerInfo,
                        "Service11101"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                if (webReturn.ListData == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("WebReturn ListData is null");
                    return optReturn;
                }
                List<ResourceProperty> listItems = new List<ResourceProperty>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<ResourceProperty>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    ResourceProperty item = optReturn.Data as ResourceProperty;
                    if (item == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Item is null");
                        return optReturn;
                    }
                    listItems.Add(item);
                }
                listItems = listItems.OrderBy(p => p.ObjID).ThenBy(p => p.PropertyID).ToList();
                mListResourcePropertyValues.Clear();
                for (int i = 0; i < listItems.Count; i++)
                {
                    mListResourcePropertyValues.Add(listItems[i]);
                }
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

        private OperationReturn LoadSingleResourcePropertyValues(BasicResourceInfo resourceInfo)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mSession == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("SessionInfo is null");
                    return optReturn;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = mSession;
                webRequest.Code = (int)S1110Codes.GetResourcePropertyValueList;
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(resourceInfo.ObjectID.ToString());
                Service11101Client client = new Service11101Client(
                    WebHelper.CreateBasicHttpBinding(mSession),
                    WebHelper.CreateEndpointAddress(
                        mSession.AppServerInfo,
                        "Service11101"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                if (webReturn.ListData == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("WebReturn ListData is null");
                    return optReturn;
                }
                List<ResourceProperty> listItems = new List<ResourceProperty>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<ResourceProperty>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    ResourceProperty item = optReturn.Data as ResourceProperty;
                    if (item == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Item is null");
                        return optReturn;
                    }
                    listItems.Add(item);
                }
                listItems = listItems.OrderBy(p => p.ObjID).ThenBy(p => p.PropertyID).ToList();
                for (int i = 0; i < listItems.Count; i++)
                {
                    mListSingleResourcePropertyValues.Add(listItems[i]);
                }
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

        private OperationReturn InitResourceObjects()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                mListResourcePropertyValues.Clear();
                mListResourceObjects.Clear();
                for (int i = 0; i < mListBasicResourceInfos.Count; i++)
                {
                    var resourceInfo = mListBasicResourceInfos[i];
                    long objID = resourceInfo.ObjectID;
                    mListSingleResourcePropertyValues.Clear();
                    optReturn = LoadSingleResourcePropertyValues(resourceInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    if (mListSingleResourcePropertyValues.Count <= 0) { continue; }
                    ResourceProperty propertyValue = mListSingleResourcePropertyValues[0];
                    int objType = propertyValue.ObjType;
                    List<ObjectPropertyInfo> listPropertyInfos =
                        mListResourcePropertyInfos.Where(p => p.ObjType == objType).ToList();
                    ResourceObject resourceObject = new ResourceObject();
                    resourceObject.ObjID = objID;
                    resourceObject.ObjType = objType;
                    for (int j = 0; j < listPropertyInfos.Count; j++)
                    {
                        var propertyInfo = listPropertyInfos[j];
                        propertyValue =
                            mListSingleResourcePropertyValues.FirstOrDefault(
                                p => p.PropertyID == propertyInfo.PropertyID);
                        if (propertyValue == null)
                        {
                            propertyValue = new ResourceProperty();
                            propertyValue.ObjID = objID;
                            propertyValue.ObjType = objType;
                            propertyValue.PropertyID = propertyInfo.PropertyID;
                            propertyValue.EncryptMode = propertyInfo.EncryptMode;
                            propertyValue.MultiValueMode = propertyInfo.MultiValueMode;
                            propertyValue.Value = propertyInfo.DefaultValue;
                        }
                        resourceObject.ListPropertyValues.Add(propertyValue);
                        mListResourcePropertyValues.Add(propertyValue);
                    }
                    optReturn = GetResourcePropertyValues(resourceObject);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    mListResourceObjects.Add(resourceObject);
                }
                //var temp = from pvs in mListResourcePropertyValues
                //           group pvs by pvs.ObjID;
                //foreach (var resource in temp)
                //{
                //    long objID = resource.Key;
                //    ResourceObject resourceObject = new ResourceObject();
                //    resourceObject.ObjType = (int)(objID / (long)Math.Pow(10, 16));
                //    resourceObject.ObjID = objID;
                //    foreach (var propertyValue in resource)
                //    {
                //        resourceObject.ListPropertyValues.Add(propertyValue);
                //    }
                //    optReturn = GetResourcePropertyValues(resourceObject);
                //    if (!optReturn.Result)
                //    {
                //        return optReturn;
                //    }
                //    mListResourceObjects.Add(resourceObject);
                //}

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


        #region Operations

        private OperationReturn GenerateResourceType(int objType, XmlNode parentNode, GenerateOption option)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                XmlDocument doc = parentNode as XmlDocument;
                if (doc == null)
                {
                    doc = parentNode.OwnerDocument;
                }
                if (doc == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format("XmlDocument is null");
                    return optReturn;
                }
                ResourceGroupParam groupParam =
                    mListResourceGroupParams.FirstOrDefault(
                        g => g.GroupType == ResourceGroupType.ChildList && g.ChildType == objType);
                if (groupParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("GroupParam is null.\t{0}", objType);
                    return optReturn;
                }
                if (string.IsNullOrEmpty(groupParam.NodeName))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_STRING_EMPTY;
                    optReturn.Message = string.Format("GroupParam NodeName is empty.\tObjType:{0}\tGroupID:{1}", objType, groupParam.GroupID);
                    return optReturn;
                }
                XmlElement element = doc.CreateElement(groupParam.NodeName);
                List<ResourceObject> listObjects = mListResourceObjects.Where(o => o.ObjType == objType).ToList();
                for (int i = 0; i < listObjects.Count; i++)
                {
                    ResourceObject resource = listObjects[i];
                    optReturn = GenerateResourceAttribute(resource, element, option);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                }
                parentNode.AppendChild(element);
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

        private OperationReturn GenerateResourceAttribute(ResourceObject resourceObject, XmlNode parentNode, GenerateOption option)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                XmlDocument doc = parentNode as XmlDocument;
                if (doc == null)
                {
                    doc = parentNode.OwnerDocument;
                }
                if (doc == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format("XmlDocument is null");
                    return optReturn;
                }
                int objType = resourceObject.ObjType;
                ResourceTypeParam typeParam = mListResourceTypeParams.FirstOrDefault(t => t.TypeID == objType);
                if (typeParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = string.Format("TypeParam not exist.\t{0}", objType);
                    return optReturn;
                }
                if (string.IsNullOrEmpty(typeParam.NodeName))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_STRING_EMPTY;
                    optReturn.Message = string.Format("TypeParam NodeName empty.\t{0}", objType);
                    return optReturn;
                }
                XmlElement xmlElement = doc.CreateElement(typeParam.NodeName);


                #region 写入直接属性

                if ((option & GenerateOption.Property) > 0)
                {
                    //直接属性是组编号为0的属性
                    List<ObjectPropertyInfo> listPropertyInfos =
                        mListResourcePropertyInfos.Where(p => p.ObjType == objType && p.GroupID == 0).ToList();
                    for (int i = 0; i < listPropertyInfos.Count; i++)
                    {
                        ObjectPropertyInfo propertyInfo = listPropertyInfos[i];
                        //属性名为空的跳过
                        if (string.IsNullOrEmpty(propertyInfo.AttributeName)) { continue; }
                        //登录名，登录密码需特殊处理，跳过
                        if (propertyInfo.PropertyID == 911 || propertyInfo.PropertyID == 912) { continue; }
                        ResourceProperty propertyValue =
                            resourceObject.ListPropertyValues.FirstOrDefault(p => p.PropertyID == propertyInfo.PropertyID);
                        if (propertyValue == null)
                        {
                            //不存在属性值的使用属性的默认值填充
                            xmlElement.SetAttribute(propertyInfo.AttributeName, propertyInfo.DefaultValue);
                        }
                        else
                        {
                            string strValue = propertyValue.Value;
                            //如果是加密的，判断加密版本及编码方式，然后以加密的方式写加密的节点
                            if (propertyInfo.EncryptMode != 0)
                            {
                                switch (propertyInfo.EncryptMode)
                                {
                                    case ObjectPropertyEncryptMode.E2Hex:
                                        XmlElement encryptElement = doc.CreateElement(propertyInfo.AttributeName);
                                        encryptElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCRYPTION, "2");
                                        encryptElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCODING, "hex");
                                        if (mEncryptObject != null)
                                        {
                                            strValue = mEncryptObject.EncryptString(strValue,
                                                (int)EncryptionMode.AES256V01Hex);
                                        }
                                        else
                                        {
                                            strValue = propertyValue.OriginalValue;
                                        }
                                        encryptElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, strValue);
                                        xmlElement.AppendChild(encryptElement);
                                        break;
                                    case ObjectPropertyEncryptMode.SHA256:
                                        if (mEncryptObject != null)
                                        {
                                            strValue = mEncryptObject.EncryptString(strValue,
                                                (int)EncryptionMode.SHA256V00Hex);
                                        }
                                        else
                                        {
                                            strValue = propertyValue.OriginalValue;
                                        }
                                        xmlElement.SetAttribute(propertyInfo.AttributeName, strValue);
                                        break;
                                }
                            }
                            else
                            {
                                xmlElement.SetAttribute(propertyInfo.AttributeName, strValue);
                            }
                        }
                    }
                }

                #endregion


                #region 写入组或子对象集合组

                if ((option & GenerateOption.Group) > 0
                  || (option & GenerateOption.Children) > 0)
                {
                    optReturn = GenerateResourceGroup(resourceObject, 0, xmlElement, option);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                }

                #endregion


                #region 写入认证

                if ((option & GenerateOption.Authention) > 0)
                {
                    optReturn = GenerateAuthention(resourceObject, xmlElement);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                }

                #endregion


                #region 写入子资源，有些资源对象本身就包含某一类型的子对象

                if ((option & GenerateOption.Children) > 0)
                {
                    if (typeParam.HasChildList)
                    {
                        //先找出指定类型的资源对象，然后判断子对象的父对象是不是本资源对象
                        var temp = mListResourceObjects.Where(r => r.ObjType == typeParam.ChildType);
                        foreach (var resource in temp)
                        {
                            ResourceProperty propertyValue =
                                resource.ListPropertyValues.FirstOrDefault(
                                    p => p.PropertyID == S1110Consts.PROPERTYID_PARENTOBJID);

                            if (propertyValue == null) { continue; }
                            if (propertyValue.Value == resourceObject.ObjID.ToString())
                            {
                                optReturn = GenerateResourceAttribute(resource, xmlElement, option);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                            }
                        }
                    }
                }

                #endregion


                parentNode.AppendChild(xmlElement);
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

        private OperationReturn GenerateResourceGroup(ResourceObject resourceObject, int parentGroup, XmlNode parentNode, GenerateOption option)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                XmlDocument doc = parentNode as XmlDocument;
                if (doc == null)
                {
                    doc = parentNode.OwnerDocument;
                }
                if (doc == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format("XmlDocument is null");
                    return optReturn;
                }
                int objType = resourceObject.ObjType;
                //找到所有下一级的组，不包含下下级
                List<ResourceGroupParam> listGroups =
                    mListResourceGroupParams.Where(g => g.TypeID == objType && g.ParentGroup == parentGroup).ToList();
                XmlElement xmlElement;
                List<ObjectPropertyInfo> listPropertyInfos;
                for (int i = 0; i < listGroups.Count; i++)
                {
                    ResourceGroupParam groupParam = listGroups[i];
                    if (string.IsNullOrEmpty(groupParam.NodeName)) { continue; }
                    switch (groupParam.GroupType)
                    {
                        case ResourceGroupType.Group:

                            #region 普通组

                            if ((option & GenerateOption.Group) > 0)
                            {

                                #region 先判断是否存在认证字段，如果存在，需要创建一个Authention节点

                                listPropertyInfos =
                                    mListResourcePropertyInfos.Where(
                                        p => p.ObjType == objType && p.GroupID == groupParam.GroupID && p.AuthField > 0)
                                        .ToList();
                                if (listPropertyInfos.Count > 0)
                                {
                                    xmlElement = doc.CreateElement(ResourceXmlsConsts.NODENAME_AUTHENTION);
                                    for (int j = 0; j < listPropertyInfos.Count; j++)
                                    {
                                        var propertyInfo = listPropertyInfos[j];
                                        if (string.IsNullOrEmpty(propertyInfo.AttributeName)) { continue; }
                                        if (propertyInfo.AuthField == 1)
                                        {
                                            //Username
                                            var propertyValue =
                                                resourceObject.ListPropertyValues.FirstOrDefault(
                                                    p => p.PropertyID == propertyInfo.PropertyID);
                                            var usernameElement = doc.CreateElement(ResourceXmlsConsts.NODENAME_USERNAME);
                                            usernameElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCODING, "hex");
                                            usernameElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCRYPTION, "2");
                                            if (propertyValue == null)
                                            {
                                                string strValue = propertyInfo.DefaultValue;
                                                if (mEncryptObject != null)
                                                {
                                                    strValue = mEncryptObject.EncryptString(strValue,
                                                        (int)EncryptionMode.AES256V01Hex);
                                                }
                                                usernameElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, strValue);
                                            }
                                            else
                                            {
                                                string strValue = propertyValue.Value;
                                                if (mEncryptObject != null)
                                                {
                                                    strValue = mEncryptObject.EncryptString(strValue,
                                                        (int)EncryptionMode.AES256V01Hex);
                                                }
                                                else
                                                {
                                                    strValue = propertyValue.OriginalValue;
                                                }
                                                usernameElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, strValue);
                                            }
                                            xmlElement.AppendChild(usernameElement);
                                        }
                                        if (propertyInfo.AuthField == 2)
                                        {
                                            //Password
                                            var propertyValue =
                                               resourceObject.ListPropertyValues.FirstOrDefault(
                                                   p => p.PropertyID == propertyInfo.PropertyID);
                                            var passwordElement = doc.CreateElement(ResourceXmlsConsts.NODENAME_PASSWORD);
                                            passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCODING, "hex");
                                            passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCRYPTION, "2");
                                            if (propertyValue == null)
                                            {
                                                string strValue = propertyInfo.DefaultValue;
                                                if (mEncryptObject != null)
                                                {
                                                    strValue = mEncryptObject.EncryptString(strValue,
                                                        (int)EncryptionMode.AES256V01Hex);
                                                }
                                                passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, strValue);
                                            }
                                            else
                                            {
                                                string strValue = propertyValue.Value;
                                                if (mEncryptObject != null)
                                                {
                                                    strValue = mEncryptObject.EncryptString(strValue,
                                                        (int)EncryptionMode.AES256V01Hex);
                                                }
                                                else
                                                {
                                                    strValue = propertyValue.OriginalValue;
                                                }
                                                passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, strValue);
                                            }
                                            xmlElement.AppendChild(passwordElement);
                                        }
                                    }
                                }

                                #endregion


                                #region 处理非认证字段的属性

                                listPropertyInfos =
                                    mListResourcePropertyInfos.Where(
                                        p => p.ObjType == objType && p.GroupID == groupParam.GroupID && p.AuthField <= 0)
                                        .ToList();
                                xmlElement = doc.CreateElement(groupParam.NodeName);
                                if (listPropertyInfos.Count > 0)
                                {
                                    for (int j = 0; j < listPropertyInfos.Count; j++)
                                    {
                                        ObjectPropertyInfo propertyInfo = listPropertyInfos[j];
                                        if (string.IsNullOrEmpty(propertyInfo.AttributeName)) { continue; }
                                        ResourceProperty propertyValue =
                                            resourceObject.ListPropertyValues.FirstOrDefault(p => p.PropertyID == propertyInfo.PropertyID);
                                        if (propertyValue == null)
                                        {

                                            #region 使用资源属性的默认值

                                            if (propertyInfo.EncryptMode != 0)
                                            {
                                                string strValue = propertyInfo.DefaultValue;
                                                switch (propertyInfo.EncryptMode)
                                                {
                                                    case ObjectPropertyEncryptMode.E2Hex:
                                                        XmlElement encryptElement = doc.CreateElement(propertyInfo.AttributeName);
                                                        encryptElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCRYPTION, "2");
                                                        encryptElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCODING, "hex");
                                                        if (mEncryptObject != null)
                                                        {
                                                            strValue = mEncryptObject.EncryptString(strValue,
                                                                (int)EncryptionMode.AES256V01Hex);

                                                        }
                                                        encryptElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, strValue);
                                                        xmlElement.AppendChild(encryptElement);
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                xmlElement.SetAttribute(propertyInfo.AttributeName, propertyInfo.DefaultValue);
                                            }

                                            #endregion

                                        }
                                        else
                                        {

                                            #region 使用PropertyValue的值

                                            string strValue = propertyValue.Value;
                                            if (propertyInfo.EncryptMode != 0)
                                            {
                                                switch (propertyInfo.EncryptMode)
                                                {
                                                    case ObjectPropertyEncryptMode.E2Hex:
                                                        XmlElement encryptElement = doc.CreateElement(propertyInfo.AttributeName);
                                                        encryptElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCRYPTION, "2");
                                                        encryptElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCODING, "hex");
                                                        if (mEncryptObject != null)
                                                        {
                                                            strValue = mEncryptObject.EncryptString(strValue,
                                                                (int)EncryptionMode.AES256V01Hex);
                                                        }
                                                        else
                                                        {
                                                            strValue = propertyValue.OriginalValue;
                                                        }
                                                        encryptElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, strValue);
                                                        xmlElement.AppendChild(encryptElement);
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                xmlElement.SetAttribute(propertyInfo.AttributeName, propertyValue.Value);
                                            }

                                            #endregion

                                        }
                                    }
                                }
                                //递归到下一级
                                optReturn = GenerateResourceGroup(resourceObject, groupParam.GroupID, xmlElement, option);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                parentNode.AppendChild(xmlElement);
                                

                                #endregion

                            }

                            #endregion

                            break;
                        case ResourceGroupType.ChildList:

                            #region 子对象集合组

                            if ((option & GenerateOption.Children) > 0)
                            {
                                var typeParam = mListResourceTypeParams.FirstOrDefault(t => t.TypeID == groupParam.ChildType);
                                if (typeParam == null) { continue; }
                                //忽略下级通道
                                if ((option & GenerateOption.IgnoreChannel) > 0)
                                {
                                    if (typeParam.TypeID == S1110Consts.RESOURCE_CHANNEL
                                        || typeParam.TypeID == S1110Consts.RESOURCE_SCREENCHANNEL)
                                    {
                                        continue;
                                    }
                                }
                                int count = 0;
                                xmlElement = doc.CreateElement(groupParam.NodeName);
                                if (typeParam.ParentID == 0)
                                {
                                    var temp = mListResourceObjects.Where(o => o.ObjType == groupParam.ChildType).ToList();
                                    foreach (var resource in temp)
                                    {
                                        optReturn = GenerateResourceAttribute(resource, xmlElement, option);
                                        if (!optReturn.Result)
                                        {
                                            return optReturn;
                                        }
                                        count++;
                                    }
                                }
                                else
                                {
                                    var temp = mListResourceObjects.Where(o => o.ObjType == groupParam.ChildType).ToList();
                                    foreach (var resource in temp)
                                    {
                                        ResourceProperty propertyValue =
                                            resource.ListPropertyValues.FirstOrDefault(
                                                p => p.PropertyID == S1110Consts.PROPERTYID_PARENTOBJID);
                                        if (propertyValue == null) { continue; }
                                        if (propertyValue.Value == resourceObject.ObjID.ToString())
                                        {
                                            optReturn = GenerateResourceAttribute(resource, xmlElement, option);
                                            if (!optReturn.Result)
                                            {
                                                return optReturn;
                                            }
                                            count++;
                                        }
                                    }
                                }
                                if (groupParam.IsCaculateCount)
                                {
                                    xmlElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_COUNT, count.ToString());
                                }
                                parentNode.AppendChild(xmlElement);
                            }

                            #endregion

                            break;
                    }
                }
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

        private OperationReturn GenerateAuthention(ResourceObject resourceObject, XmlNode parentNode)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                XmlDocument doc = parentNode as XmlDocument;
                if (doc == null)
                {
                    doc = parentNode.OwnerDocument;
                }
                if (doc == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format("XmlDocument is null");
                    return optReturn;
                }
                int objType = resourceObject.ObjType;
                ResourceTypeParam typeParam = mListResourceTypeParams.FirstOrDefault(t => t.TypeID == objType);
                if (typeParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = string.Format("TypeParam not exist.\t{0}", objType);
                    return optReturn;
                }
                if (!typeParam.IsAuthention)
                {
                    return optReturn;
                }
                XmlElement xmlElement = doc.CreateElement(ResourceXmlsConsts.NODENAME_AUTHENTION);
                ObjectPropertyInfo userNameProperty =
                    mListResourcePropertyInfos.FirstOrDefault(
                        p => p.ObjType == objType && p.PropertyID == S1110Consts.PROPERTYID_AUTHUSERNAME);
                ObjectPropertyInfo passwordProperty =
                    mListResourcePropertyInfos.FirstOrDefault(
                        p => p.ObjType == objType && p.PropertyID == S1110Consts.PROPERTYID_AUTHPASSWORD);
                if (userNameProperty != null)
                {
                    XmlElement userNameElement = doc.CreateElement(ResourceXmlsConsts.NODENAME_USERNAME);
                    ResourceProperty propertyValue =
                        resourceObject.ListPropertyValues.FirstOrDefault(p => p.PropertyID == S1110Consts.PROPERTYID_AUTHUSERNAME);
                    if (propertyValue == null)
                    {
                        switch (userNameProperty.EncryptMode)
                        {
                            case ObjectPropertyEncryptMode.E2Hex:
                                userNameElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCRYPTION, "2");
                                userNameElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCODING, "hex");
                                if (mEncryptObject != null)
                                {
                                    userNameElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE,
                                        mEncryptObject.EncryptString(userNameProperty.DefaultValue));
                                }
                                else
                                {
                                    userNameElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, userNameProperty.DefaultValue);
                                }
                                xmlElement.AppendChild(userNameElement);
                                break;
                        }
                    }
                    else
                    {
                        string strValue = propertyValue.Value;
                        switch (userNameProperty.EncryptMode)
                        {
                            case ObjectPropertyEncryptMode.E2Hex:
                                userNameElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCRYPTION, "2");
                                userNameElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCODING, "hex");
                                if (mEncryptObject != null)
                                {
                                    strValue = mEncryptObject.EncryptString(strValue, (int)EncryptionMode.AES256V01Hex);
                                }
                                else
                                {
                                    strValue = propertyValue.OriginalValue;
                                }
                                userNameElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, strValue);
                                xmlElement.AppendChild(userNameElement);
                                break;
                        }
                    }
                }
                if (passwordProperty != null)
                {
                    XmlElement passwordElement = doc.CreateElement(ResourceXmlsConsts.NODENAME_PASSWORD);
                    ResourceProperty propertyValue =
                        resourceObject.ListPropertyValues.FirstOrDefault(p => p.PropertyID == S1110Consts.PROPERTYID_AUTHPASSWORD);
                    if (propertyValue == null)
                    {
                        switch (passwordProperty.EncryptMode)
                        {
                            case ObjectPropertyEncryptMode.E2Hex:
                                passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCRYPTION, "2");
                                passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCODING, "hex");
                                if (mEncryptObject != null)
                                {
                                    passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE,
                                        mEncryptObject.EncryptString(passwordProperty.DefaultValue));
                                }
                                else
                                {
                                    passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, passwordProperty.DefaultValue);
                                }
                                xmlElement.AppendChild(passwordElement);
                                break;
                        }
                    }
                    else
                    {
                        string strValue = propertyValue.Value;
                        switch (passwordProperty.EncryptMode)
                        {
                            case ObjectPropertyEncryptMode.E2Hex:
                                passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCRYPTION, "2");
                                passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCODING, "hex");
                                if (mEncryptObject != null)
                                {
                                    strValue = mEncryptObject.EncryptString(strValue, (int)EncryptionMode.AES256V01Hex);
                                }
                                else
                                {
                                    strValue = propertyValue.OriginalValue;
                                }
                                passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, strValue);
                                xmlElement.AppendChild(passwordElement);
                                break;
                        }
                    }
                }
                parentNode.AppendChild(xmlElement);
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

        private OperationReturn GetResourcePropertyValues(ResourceObject resourceObject)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                ResourceProperty propertyValue;
                int intValue;
                long longValue;
                for (int i = 0; i < resourceObject.ListPropertyValues.Count; i++)
                {
                    propertyValue = resourceObject.ListPropertyValues[i];
                    switch (propertyValue.PropertyID)
                    {
                        case S1110Consts.PROPERTYID_KEY:
                            if (int.TryParse(propertyValue.Value, out intValue))
                            {
                                resourceObject.Key = intValue;
                            }
                            break;
                        case S1110Consts.PROPERTYID_ID:
                            if (int.TryParse(propertyValue.Value, out intValue))
                            {
                                resourceObject.ID = intValue;
                            }
                            break;
                        case S1110Consts.PROPERTYID_PARENTOBJID:
                            if (long.TryParse(propertyValue.Value, out longValue))
                            {
                                resourceObject.ParentID = longValue;
                            }
                            break;
                        case S1110Consts.PROPERTYID_MODULENUMBER:
                            if (int.TryParse(propertyValue.Value, out intValue))
                            {
                                resourceObject.ModuleNumber = intValue;
                            }
                            break;
                        case S1110Consts.PROPERTYID_HOSTADDRESS:
                            resourceObject.HostAddress = propertyValue.Value;
                            break;
                        case S1110Consts.PROPERTYID_HOSTNAME:
                            resourceObject.HostName = propertyValue.Value;
                            break;
                        case S1110Consts.PROPERTYID_HOSTPORT:
                            if (int.TryParse(propertyValue.Value, out intValue))
                            {
                                resourceObject.HostPort = intValue;
                            }
                            break;
                    }
                }
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
