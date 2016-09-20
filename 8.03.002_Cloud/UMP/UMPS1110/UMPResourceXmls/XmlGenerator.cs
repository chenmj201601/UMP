//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4baf95a8-6f57-4246-a15a-5536dafa34d3
//        CLR Version:              4.0.30319.18444
//        Name:                     XmlGenerator
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ResourceXmls
//        File Name:                XmlGenerator
//
//        created by Charley at 2015/2/12 17:27:30
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
    /// 用于生成资源对象的xml文件
    /// </summary>
    public class XmlGenerator
    {

        #region  Set only Property
        /// <summary>
        /// SessionInfo，初始化时需指定
        /// </summary>
        public SessionInfo Session
        {
            set { mSession = value; }
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

        #endregion


        #region Memebers

        private SessionInfo mSession;
        private List<ResourceTypeParam> mListResourceTypeParams;
        private List<ResourceGroupParam> mListResourceGroupParams;
        private List<ObjectPropertyInfo> mListResourcePropertyInfos;
        private List<ResourceProperty> mListResourcePropertyValues;
        private List<ResourceObject> mListResourceObjects;

        #endregion

        public XmlGenerator()
        {
            mListResourceTypeParams = new List<ResourceTypeParam>();
            mListResourceGroupParams = new List<ResourceGroupParam>();
            mListResourcePropertyInfos = new List<ObjectPropertyInfo>();
            mListResourcePropertyValues = new List<ResourceProperty>();
            mListResourceObjects = new List<ResourceObject>();
        }

        public XmlGenerator(SessionInfo session)
            : this()
        {
            mSession = session;
        }


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
                    WebHelper.CreateBasicHttpBingding(),
                    WebHelper.CreateEndpointAddress(
                        mSession.AppServerInfo,
                        "Service11101"));
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
                    WebHelper.CreateBasicHttpBingding(),
                    WebHelper.CreateEndpointAddress(
                        mSession.AppServerInfo,
                        "Service11101"));
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
                    WebHelper.CreateBasicHttpBingding(),
                    WebHelper.CreateEndpointAddress(
                        mSession.AppServerInfo,
                        "Service11101"));
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
                    WebHelper.CreateBasicHttpBingding(),
                    WebHelper.CreateEndpointAddress(
                        mSession.AppServerInfo,
                        "Service11101"));
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
                mListResourceObjects.Clear();
                var temp = from pvs in mListResourcePropertyValues
                           group pvs by pvs.ObjID;
                foreach (var resource in temp)
                {
                    long objID = resource.Key;
                    ResourceObject resourceObject = new ResourceObject();
                    resourceObject.ObjType = (int)(objID / (long)Math.Pow(10, 16));
                    resourceObject.ObjID = objID;
                    foreach (var propertyValue in resource)
                    {
                        resourceObject.ListPropertyValues.Add(propertyValue);
                    }
                    mListResourceObjects.Add(resourceObject);
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        #endregion


        #region Public Functions

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
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 生成单个资源的xml的文件
        /// </summary>
        /// <param name="resourceID"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public OperationReturn GenerateResourceXmlFile(long resourceID, string path)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                optReturn = GenerateResourceXmlNode(resourceID);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                XmlDocument doc = optReturn.Data as XmlDocument;
                if (doc == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("XmlDocument is null");
                    return optReturn;
                }
                doc.Save(path);
                optReturn.Message = path;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 当文件存在时 生成单个资源的xml的文件
        /// </summary>
        /// <param name="resourceID"></param>
        /// <returns></returns>
        public OperationReturn GenerateResourceXmlNode(long resourceID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                optReturn = LoadResourcePropertyValues();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = InitResourceObjects();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                ResourceObject resourceObject = mListResourceObjects.FirstOrDefault(r => r.ObjID == resourceID);
                if (resourceObject == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = string.Format("ResourceObject not exist.\t{0}", resourceID);
                    return optReturn;
                }
                XmlDocument doc = new XmlDocument();
                XmlNode declare = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
                doc.AppendChild(declare);
                optReturn = GenerateResourceAttribute(resourceObject, doc, doc);
                if (optReturn.Result)
                {
                    optReturn.Data = doc;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 生成所有资源的xml文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public OperationReturn GenerateAllResourceXmlFile(string path)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                optReturn = GenerateAllResourceXmlNode();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                XmlDocument doc = optReturn.Data as XmlDocument;
                if (doc == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("XmlDocument is null");
                    return optReturn;
                }
                doc.Save(path);
                optReturn.Message = path;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 当文件存在时 生成所有资源的xml文件
        /// </summary>
        /// <returns></returns>
        public OperationReturn GenerateAllResourceXmlNode()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                optReturn = LoadResourcePropertyValues();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = InitResourceObjects();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                XmlDocument doc = new XmlDocument();
                //声明
                XmlNode declare = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
                doc.AppendChild(declare);
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

                optReturn = GenerateResourceType(S1110Consts.RESOURCE_LICENSESERVER, doc, resourcesNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_DATATRANSFERSERVER, doc, resourcesNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_CTIHUBSERVER, doc, resourcesNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_ARCHIVESTRATEGY, doc, resourcesNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_STORAGEDEVICE, doc, resourcesNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMMONITOR, doc, resourcesNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMSERVER, doc, resourcesNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_DBBRIDGE, doc, resourcesNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_SFTP, doc, resourcesNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                siteNode.AppendChild(resourcesNode);

                optReturn = GenerateResourceType(S1110Consts.RESOURCE_CTICONNECTIONGROUPCOLLECTION, doc, siteNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_CTIPOLICY, doc, siteNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_FILEOPERATOR, doc, siteNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMMONITORPARAM, doc, siteNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                XmlElement alarmServerParam = doc.CreateElement("AlarmServerParam");
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMEMAILSERVER, doc, alarmServerParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMABNORMALRECORD, doc, alarmServerParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMSENDRULE, doc, alarmServerParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_ALARMRECORDNUMBER, doc, alarmServerParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                siteNode.AppendChild(alarmServerParam);

                optReturn = GenerateResourceType(S1110Consts.RESOURCE_VOICESERVER, doc, siteNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_CMSERVER, doc, siteNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_SCREENSERVER, doc, siteNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn = GenerateResourceType(S1110Consts.RESOURCE_SPEECHANALYSISPARAM, doc, siteNode);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                sitesNode.AppendChild(siteNode);
                configNode.AppendChild(sitesNode);
                configsNode.AppendChild(configNode);
                doc.AppendChild(configsNode);
                optReturn.Data = doc;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        #endregion


        #region Operations

        public OperationReturn GenerateResourceType(int objType, XmlDocument doc, XmlNode parentNode)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
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
                    optReturn = GenerateResourceAttribute(resource, doc, element);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    parentNode.AppendChild(element);
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn GenerateResourceAttribute(ResourceObject resourceObject, XmlDocument doc, XmlNode parentNode)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                optReturn = SetResourcePropertyValue(resourceObject);
                if (!optReturn.Result)
                {
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
                //写入属性
                List<ObjectPropertyInfo> listPropertyInfos =
                    mListResourcePropertyInfos.Where(p => p.ObjType == objType && p.GroupID == 0).ToList();
                for (int i = 0; i < listPropertyInfos.Count; i++)
                {
                    ObjectPropertyInfo propertyInfo = listPropertyInfos[i];
                    if (string.IsNullOrEmpty(propertyInfo.AttributeName)) { continue; }
                    //登录名，登录密码需特殊处理，过滤
                    if (propertyInfo.PropertyID == 911 || propertyInfo.PropertyID == 912) { continue; }
                    ResourceProperty propertyValue =
                        resourceObject.ListPropertyValues.FirstOrDefault(p => p.PropertyID == propertyInfo.PropertyID);
                    if (propertyValue == null)
                    {
                        xmlElement.SetAttribute(propertyInfo.AttributeName, propertyInfo.DefaultValue);
                    }
                    else
                    {
                        if (propertyInfo.EncryptMode != 0)
                        {
                            switch (propertyInfo.EncryptMode)
                            {
                                case ObjectPropertyEncryptMode.E2Hex:
                                    XmlElement encryptElement = doc.CreateElement(propertyInfo.AttributeName);
                                    encryptElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCRYPTION, "2");
                                    encryptElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCODING, "hex");
                                    encryptElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, propertyValue.OriginalValue);
                                    xmlElement.AppendChild(encryptElement);
                                    break;
                            }
                        }
                        else
                        {
                            xmlElement.SetAttribute(propertyInfo.AttributeName, propertyValue.Value);
                        }
                    }
                }
                //写入组
                optReturn = GenerateResourceGroup(resourceObject, 0, doc, xmlElement);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                //写入认证
                optReturn = GenerateAuthention(resourceObject, doc, xmlElement);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                //写入子资源
                if (typeParam.HasChildList)
                {
                    var temp = mListResourceObjects.Where(r => r.ObjType == typeParam.ChildType);
                    foreach (var resource in temp)
                    {
                        ResourceProperty propertyValue =
                            resource.ListPropertyValues.FirstOrDefault(
                                p => p.PropertyID == S1110Consts.PROPERTYID_PARENTOBJID);
                        if (propertyValue == null) { continue; }
                        if (propertyValue.Value == resourceObject.ObjID.ToString())
                        {
                            optReturn = GenerateResourceAttribute(resource, doc, xmlElement);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
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
            }
            return optReturn;
        }

        private OperationReturn GenerateResourceGroup(ResourceObject resourceObject, int parentGroup, XmlDocument doc, XmlNode parentNode)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                int objType = resourceObject.ObjType;
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
                            xmlElement = doc.CreateElement(groupParam.NodeName);
                            listPropertyInfos = mListResourcePropertyInfos.Where(p => p.ObjType == objType && p.GroupID == groupParam.GroupID).ToList();
                            for (int j = 0; j < listPropertyInfos.Count; j++)
                            {
                                ObjectPropertyInfo propertyInfo = listPropertyInfos[j];
                                if (string.IsNullOrEmpty(propertyInfo.AttributeName)) { continue; }
                                ResourceProperty propertyValue =
                                    resourceObject.ListPropertyValues.FirstOrDefault(p => p.PropertyID == propertyInfo.PropertyID);
                                if (propertyValue == null)
                                {
                                    xmlElement.SetAttribute(propertyInfo.AttributeName, propertyInfo.DefaultValue);
                                }
                                else
                                {
                                    xmlElement.SetAttribute(propertyInfo.AttributeName, propertyValue.Value);
                                    if (propertyInfo.EncryptMode != 0)
                                    {
                                        xmlElement.SetAttribute(propertyInfo.AttributeName, propertyValue.OriginalValue);
                                    }
                                }
                            }
                            optReturn = GenerateResourceGroup(resourceObject, groupParam.GroupID, doc, xmlElement);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            parentNode.AppendChild(xmlElement);
                            break;
                        case ResourceGroupType.ChildList:
                            var typeParam = mListResourceTypeParams.FirstOrDefault(t => t.TypeID == groupParam.ChildType);
                            if (typeParam == null) { continue; }
                            xmlElement = doc.CreateElement(groupParam.NodeName);
                            if (typeParam.ParentID == 0)
                            {
                                var temp = mListResourceObjects.Where(o => o.ObjType == groupParam.ChildType).ToList();
                                foreach (var resource in temp)
                                {
                                    GenerateResourceAttribute(resource, doc, xmlElement);
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
                                        optReturn = GenerateResourceAttribute(resource, doc, xmlElement);
                                        if (!optReturn.Result)
                                        {
                                            return optReturn;
                                        }
                                    }
                                }
                            }
                            parentNode.AppendChild(xmlElement);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn GenerateAuthention(ResourceObject resourceObject, XmlDocument doc, XmlNode parentNode)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
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
                                userNameElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, userNameProperty.DefaultValue);
                                xmlElement.AppendChild(userNameElement);
                                break;
                        }
                    }
                    else
                    {
                        switch (userNameProperty.EncryptMode)
                        {
                            case ObjectPropertyEncryptMode.E2Hex:
                                userNameElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCRYPTION, "2");
                                userNameElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCODING, "hex");
                                userNameElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, propertyValue.OriginalValue);
                                xmlElement.AppendChild(userNameElement);
                                break;
                        }
                    }
                }
                if (passwordProperty != null)
                {
                    XmlElement passwordElement = doc.CreateElement(ResourceXmlsConsts.NODENAME_PASSWORD);
                    ResourceProperty propertyValue =
                        resourceObject.ListPropertyValues.FirstOrDefault(p => p.PropertyID == S1110Consts.PROPERTYID_AUTHUSERNAME);
                    if (propertyValue == null)
                    {
                        switch (passwordProperty.EncryptMode)
                        {
                            case ObjectPropertyEncryptMode.E2Hex:
                                passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCRYPTION, "2");
                                passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCODING, "hex");
                                passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, passwordProperty.DefaultValue);
                                xmlElement.AppendChild(passwordElement);
                                break;
                        }
                    }
                    else
                    {
                        switch (passwordProperty.EncryptMode)
                        {
                            case ObjectPropertyEncryptMode.E2Hex:
                                passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCRYPTION, "2");
                                passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_ENCODING, "hex");
                                passwordElement.SetAttribute(ResourceXmlsConsts.ATTRNAME_VALUE, propertyValue.OriginalValue);
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
            }
            return optReturn;
        }

        private OperationReturn SetResourcePropertyValue(ResourceObject resourceObject)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                ResourceProperty propertyValue1, PropertyValue2;

                //资源的ModuleNumber默认为资源的ID
                propertyValue1 =
                    resourceObject.ListPropertyValues.FirstOrDefault(
                        p => p.PropertyID == S1110Consts.PROPERTYID_MODULENUMBER);
                PropertyValue2 =
                    resourceObject.ListPropertyValues.FirstOrDefault(p => p.PropertyID == S1110Consts.PROPERTYID_ID);
                if (propertyValue1 != null && PropertyValue2 != null)
                {
                    propertyValue1.Value = PropertyValue2.Value;
                }

                //特殊处理
                switch (resourceObject.ObjType)
                {
                    case S1110Consts.RESOURCE_LICENSESERVER:
                        //LicenseServer的ModuleNumber有主备性质得来，主机：0 备机：1
                        propertyValue1 =
                            resourceObject.ListPropertyValues.FirstOrDefault(
                                p => p.PropertyID == S1110Consts.PROPERTYID_MODULENUMBER);
                        PropertyValue2 =
                            resourceObject.ListPropertyValues.FirstOrDefault(p => p.PropertyID == S1110Consts.PROPERTYID_MASTERSLAVER);
                        if (propertyValue1 != null && PropertyValue2 != null)
                        {
                            if (PropertyValue2.Value == "1")
                            {
                                propertyValue1.Value = "0";
                            }
                            else
                            {
                                propertyValue1.Value = "1";
                            }
                        }
                        break;
                    case S1110Consts.RESOURCE_ALARMSERVER:

                        break;
                    case S1110Consts.RESOURCE_DATATRANSFERSERVER:
                    case S1110Consts.RESOURCE_CTIHUBSERVER:
                    case S1110Consts.RESOURCE_ALARMMONITOR:
                    case S1110Consts.RESOURCE_DBBRIDGE:
                    case S1110Consts.RESOURCE_VOICESERVER:
                    case S1110Consts.RESOURCE_SFTP:
                    case S1110Consts.RESOURCE_SCREENSERVER:
                    case S1110Consts.RESOURCE_CTICONNECTION:
                    case S1110Consts.RESOURCE_CMSERVER:
                    case S1110Consts.RESOURCE_ALARMEMAILSERVER:

                        break;
                    case S1110Consts.RESOURCE_DBBRIDGENAME:
                    case S1110Consts.RESOURCE_STORAGEDEVICE:
                    case S1110Consts.RESOURCE_NETWORKCARD:
                    case S1110Consts.RESOURCE_VOIPPROTOCAL:

                        break;
                    case S1110Consts.RESOURCE_CONCURRENT:
                        //并发的Key值由Number值得来
                        propertyValue1 =
                            resourceObject.ListPropertyValues.FirstOrDefault(p => p.PropertyID == S1110Consts.PROPERTYID_KEY);
                        PropertyValue2 =
                            resourceObject.ListPropertyValues.FirstOrDefault(p => p.PropertyID == 11);
                        if (propertyValue1 != null && PropertyValue2 != null)
                        {
                            propertyValue1.Value = PropertyValue2.Value;
                        }
                        break;
                    case S1110Consts.RESOURCE_CTICONNECTIONGROUP:
                    case S1110Consts.RESOURCE_CTICONNECTIONGROUPCOLLECTION:
                    case S1110Consts.RESOURCE_CTIPOLICY:
                    case S1110Consts.RESOURCE_FILEOPERATOR:
                    case S1110Consts.RESOURCE_CMSERVERVOICE:
                    case S1110Consts.RESOURCE_ARCHIVESTRATEGY:
                    case S1110Consts.RESOURCE_ALARMMONITORPARAM:
                    case S1110Consts.RESOURCE_ALARMHARDDISK:

                        break;
                    case S1110Consts.RESOURCE_ALARMPROCESS:
                    case S1110Consts.RESOURCE_ALARMSERVICE:
                        //进程和服务的Key值由Name值得来
                        propertyValue1 =
                            resourceObject.ListPropertyValues.FirstOrDefault(p => p.PropertyID == 11);
                        PropertyValue2 =
                            resourceObject.ListPropertyValues.FirstOrDefault(p => p.PropertyID == 12);
                        if (propertyValue1 != null && PropertyValue2 != null)
                        {
                            propertyValue1.Value = PropertyValue2.Value;
                        }
                        break;
                    case S1110Consts.RESOURCE_ALARMABNORMALRECORD:
                    case S1110Consts.RESOURCE_ALARMSENDRULE:
                    case S1110Consts.RESOURCE_ALARMRECORDNUMBER:
                    case S1110Consts.RESOURCE_ALARMRECORDNUMBERCHECKTIME:
                    case S1110Consts.RESOURCE_ALARMRECORDNUMBERDEVICE:
                    case S1110Consts.RESOURCE_SPEECHANALYSISPARAM:

                        break;
                    case S1110Consts.RESOURCE_CHANNEL:
                    case S1110Consts.RESOURCE_SCREENCHANNEL:

                        break;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        #endregion

    }
}
