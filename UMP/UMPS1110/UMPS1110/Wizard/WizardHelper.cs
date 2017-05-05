using System;
using System.Collections.Generic;
using System.Linq;
using UMPS1110.Models;
using VoiceCyber.SDKs.Licenses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Controls;

namespace UMPS1110.Wizard
{
    public class WizardHelper
    {
        public List<ConfigObject> ListAllConfigObjects;
        public List<ResourceTypeParam> ListAllTypeParams;
        public List<ResourceGroupParam> ListAllGroupParams;
        public List<ObjectPropertyInfo> ListAllPropertyInfos;
        public List<ResourceProperty> ListAllPropertyValues;
        public List<BasicInfoData> ListAllBasicInfos;
        public List<ObjectItem> ListAllObjectItem;
        public List<BasicUserInfo> mListSftpUsers;

        public List<ConfigObject> ListRecords;
        public List<ConfigObject> ListScreens;
        public ObjectItem mRootObjectItem;

        public UCWizardRecordConfig UCRecordConfig;
        public UCWizardScreenConfig UCScreenConfig;

        public List<OperationInfo> WizListOperations;

        public UMPApp CurrentApp;

        public List<License> ListLicenses;

        public ConfigObject CreateNewConfigObject(ConfigObject parentObject, int objType)
        {
            try
            {
                //检查最大个数限制
                //if (!CheckChildObjectMaxinum(parentItem, objType)) { return; }
                ConfigGroup parentGroup;
                ResourceTypeParam typeParam = ListAllTypeParams.FirstOrDefault(t => t.TypeID == objType);
                if (typeParam == null) { return null; }
                int parentType = typeParam.ParentID;
                List<ConfigObject> listConfigObject = ListAllConfigObjects.Where(o => o.ObjectType == objType).OrderBy(o => o.ObjectID).ToList();
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
                configObject.CurrentApp = CurrentApp;
                configObject.Icon = typeParam.Icon;
                configObject.TypeParam = typeParam;
                configObject.ListAllTypeParams = ListAllTypeParams;
                configObject.ListAllObjects = ListAllConfigObjects;
                configObject.ListAllBasicInfos = ListAllBasicInfos;
                List<ObjectPropertyInfo> listPropertyInfos =
                       ListAllPropertyInfos.Where(p => p.ObjType == objType).ToList();
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
                    ListAllPropertyValues.Add(propertyValue);
                }
                configObject.GetBasicPropertyValues();
                ListAllConfigObjects.Add(configObject);
                parentObject.ListChildObjects.Add(configObject);
               
                #region 写操作日志

                //var optID = string.Format("1110{0}06", configObject.ObjectType);
                //string strOptLog = string.Format("{0}({1})", configObject.Name,
                //    Utils.FormatOptLogString(string.Format("OBJ{0}", configObject.ObjectType)));
                //App.WriteOperationLog(optID, ConstValue.OPT_RESULT_SUCCESS, strOptLog);

                #endregion

                return configObject;
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
                return null;
            }
        }

    }
}
