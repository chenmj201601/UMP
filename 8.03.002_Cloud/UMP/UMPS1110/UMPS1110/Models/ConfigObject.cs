//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    22f6bf06-e509-4cd4-b0ee-5fbd7017330c
//        CLR Version:              4.0.30319.18444
//        Name:                     ConfigObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                ConfigObject
//
//        created by Charley at 2015/1/12 11:13:32
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using UMPS1110.Models.ConfigObjects;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Controls;

namespace UMPS1110.Models
{
    /// <summary>
    /// 配置对象
    /// </summary>
    public class ConfigObject : INotifyPropertyChanged
    {

        #region Members

        public long ObjectID { get; set; }
        public int ObjectType { get; set; }
        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }
        public string Description { get; set; }
        private string mIcon;

        public string Icon
        {
            get { return mIcon; }
            set { mIcon = value; OnPropertyChanged("Icon"); }
        }

        private string mStrIcon;

        public string StrIcon
        {
            get { return mStrIcon; }
            set { mStrIcon = value; OnPropertyChanged("StrIcon"); }
        }
        private Brush mBackground;

        public Brush Background
        {
            get { return mBackground; }
            set { mBackground = value; OnPropertyChanged("Background"); }
        }

        public int Key { get; set; }
        public int ID { get; set; }
        public long ParentID { get; set; }

        /// <summary>
        /// 所有基本信息数据
        /// </summary>
        public List<BasicInfoData> ListAllBasicInfos { get; set; }
        /// <summary>
        /// 所有配置对象列表
        /// </summary>
        public List<ConfigObject> ListAllObjects { get; set; }
        /// <summary>
        /// 所在的资源树上对应的节点
        /// </summary>
        public ObjectItem ObjectItem { get; set; }
        /// <summary>
        /// 所有资源类型参数
        /// </summary>
        public List<ResourceTypeParam> ListAllTypeParams { get; set; }
        /// <summary>
        /// 资源类型参数
        /// </summary>
        public ResourceTypeParam TypeParam { get; set; }

        public UMPApp CurrentApp;

        #endregion


        public ConfigObject()
        {
            mListProperties = new List<ResourceProperty>();
            mListChildObjects = new List<ConfigObject>();
        }


        #region ListProperties

        private List<ResourceProperty> mListProperties;
        /// <summary>
        /// 属性列表
        /// </summary>
        public List<ResourceProperty> ListProperties
        {
            get { return mListProperties; }
        }

        #endregion


        #region ListChildObjects

        private List<ConfigObject> mListChildObjects;
        /// <summary>
        /// 子对象列表
        /// </summary>
        public List<ConfigObject> ListChildObjects
        {
            get { return mListChildObjects; }
        }

        #endregion


        #region Public Functions

        /// <summary>
        /// 更新树节点上的显示内容
        /// </summary>
        public void RefreshObjectItem()
        {
            if (ObjectItem != null)
            {
                ObjectItem.Name = Name;
                ObjectItem.Description = Description;
            }
        }
        /// <summary>
        /// 获取指定属性ID的属性值，从属性列表中获取
        /// </summary>
        /// <param name="propertyID">属性编号</param>
        /// <returns></returns>
        public string GetPropertyStringValue(int propertyID)
        {
            string strReturn = string.Empty;
            ResourceProperty propertyValue = GetPropertyValue(propertyID);
            if (propertyValue != null)
            {
                strReturn = propertyValue.Value;
            }
            return strReturn;
        }
        /// <summary>
        /// 获取指定属性ID的属性值，从属性列表中获取
        /// </summary>
        /// <param name="propertyID">属性编号</param>
        /// <returns></returns>
        public ResourceProperty GetPropertyValue(int propertyID)
        {
            return mListProperties.FirstOrDefault(p => p.PropertyID == propertyID);
        }
        /// <summary>
        /// 设置指定属性ID的属性值，同时设置属性列表的值和模型的属性值
        /// </summary>
        /// <param name="propertyID"></param>
        /// <param name="value"></param>
        public virtual void SetPropertyValue(int propertyID, string value)
        {
            //设置属性列表中的属性值
            ResourceProperty propertyValue = mListProperties.FirstOrDefault(p => p.PropertyID == propertyID);
            if (propertyValue != null)
            {
                propertyValue.Value = value;
            }

            //设置模型属性值
            int intValue;
            long longValue;
            switch (propertyID)
            {
                case S1110Consts.PROPERTYID_KEY:
                    if (int.TryParse(value, out intValue))
                    {
                        Key = intValue;
                    }
                    break;
                case S1110Consts.PROPERTYID_ID:
                    if (int.TryParse(value, out intValue))
                    {
                        ID = intValue;
                    }
                    break;
                case S1110Consts.PROPERTYID_PARENTOBJID:
                    if (long.TryParse(value, out longValue))
                    {
                        ParentID = longValue;
                    }
                    break;
            }
        }
        /// <summary>
        /// 获取特殊属性的值，从属性列表到模型
        /// </summary>
        public virtual void GetBasicPropertyValues()
        {
            ResourceProperty propertyValue;
            int intValue;
            long longValue;
            for (int i = 0; i < mListProperties.Count; i++)
            {
                propertyValue = mListProperties[i];
                switch (propertyValue.PropertyID)
                {
                    case S1110Consts.PROPERTYID_KEY:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            Key = intValue;
                        }
                        break;
                    case S1110Consts.PROPERTYID_ID:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            ID = intValue;
                        }
                        break;
                    case S1110Consts.PROPERTYID_PARENTOBJID:
                        if (long.TryParse(propertyValue.Value, out longValue))
                        {
                            ParentID = longValue;
                        }
                        break;
                }
            }

            GetNameAndDescription();
        }
        /// <summary>
        /// 设置特殊属性的值，从模型到属性列表
        /// 有些属性的值是通过计算得来的，保存参数之前需要先调用一下此方法
        /// </summary>
        public virtual void SetBasicPropertyValues()
        {
            //默认XmlKey的值有ID决定
            int xmlKey = ID;
            long xmlObjID = ObjectID;
            ResourceProperty propertyValue;
            for (int i = 0; i < mListProperties.Count; i++)
            {
                propertyValue = mListProperties[i];
                switch (propertyValue.PropertyID)
                {
                    case S1110Consts.PROPERTYID_KEY:
                        propertyValue.Value = Key.ToString();
                        break;
                    case S1110Consts.PROPERTYID_ID:
                        propertyValue.Value = ID.ToString();
                        break;
                    case S1110Consts.PROPERTYID_PARENTOBJID:
                        propertyValue.Value = ParentID.ToString();
                        break;
                    case S1110Consts.PROPERTYID_XMLKEY:
                        propertyValue.Value = xmlKey.ToString();
                        break;
                    case S1110Consts.PROPERTYID_XMLOBJID:
                        propertyValue.Value = xmlObjID.ToString();
                        break;
                }
            }
        }
        /// <summary>
        /// 获取名称与描述
        /// </summary>
        public virtual void GetNameAndDescription()
        {
            Name = string.Empty;
            Description = string.Empty;
            string typeDesc;
            typeDesc = CurrentApp.GetLanguageInfo(string.Format("OBJ{0}", ObjectType), string.Empty);
            if (string.IsNullOrEmpty(typeDesc))
            {
                typeDesc = ObjectType.ToString();
                if (TypeParam != null)
                {
                    typeDesc = TypeParam.Description;
                }
            }
            Name = string.Format("[{0}] {1}", typeDesc, ID);
            Description = string.Format("[{0}] {1}", typeDesc, ObjectID);
        }
        /// <summary>
        /// 检查配置
        /// </summary>
        /// <returns></returns>
        public virtual CheckResult CheckConfig()
        {
            CheckResult result = new CheckResult();
            result.ConfigObject = this;
            result.Result = true;
            result.Code = 0;
            return result;
        }

        #endregion


        #region Static Functions
        /// <summary>
        /// 根据指定的资源类型创建一个资源对象
        /// </summary>
        /// <param name="objType">资源类型编码</param>
        /// <returns></returns>
        public static ConfigObject CreateObject(int objType)
        {
            ConfigObject configObject;
            switch (objType)
            {
                case S1110Consts.RESOURCE_MACHINE:
                    configObject = new MachineObject();
                    break;
                case S1110Consts.RESOURCE_LICENSESERVER:
                    configObject = new LicenseServiceObject();
                    break;
                case S1110Consts.RESOURCE_ALARMSERVER:
                    configObject = new AlarmServerObject();
                    break;
                case S1110Consts.RESOURCE_VOICESERVER:
                    configObject = new VoiceServiceObject();
                    break;
                case S1110Consts.RESOURCE_DATATRANSFERSERVER:
                case S1110Consts.RESOURCE_CTIHUBSERVER:
                case S1110Consts.RESOURCE_DBBRIDGE:
                case S1110Consts.RESOURCE_ALARMMONITOR:
                case S1110Consts.RESOURCE_SFTP:
                case S1110Consts.RESOURCE_SCREENSERVER:
                case S1110Consts.RESOURCE_ISASERVER:
                case S1110Consts.RESOURCE_CMSERVER:
                case S1110Consts.RESOURCE_KEYGENERATOR:
                case S1110Consts.RESOURCE_FILEOPERATOR:
                case S1110Consts.RESOURCE_SPEECHANALYSISPARAM:
                case S1110Consts.RESOURCE_RECOVERSERVER:
                case S1110Consts.RESOURCE_CAPTURESERVER:
                    configObject = new ServiceObject();
                    break;
                case S1110Consts.RESOURCE_STORAGEDEVICE:
                    configObject = new StorageDeviceObject();
                    break;
                case S1110Consts.RESOURCE_PBXDEVICE:
                    configObject = new PBXDeviceObject();
                    break;
                case S1110Consts.RESOURCE_CHANNEL:
                    configObject = new VoiceChannelObject();
                    break;
                case S1110Consts.RESOURCE_SCREENCHANNEL:
                    configObject = new ChannelObject();
                    break;
                case S1110Consts.RESOURCE_NETWORKCARD:
                    configObject = new NetworkCardObject();
                    break;
                case S1110Consts.RESOURCE_VOIPPROTOCAL:
                    configObject = new VoipProtocalObject();
                    break;
                case S1110Consts.RESOURCE_CONCURRENT:
                    configObject = new ConcurrentObject();
                    break;
                case S1110Consts.RESOURCE_CTICONNECTION:
                    configObject=new CTIConnectionObject();
                    break;
                case S1110Consts.RESOURCE_CTICONNECTIONGROUP:
                    configObject = new CTIConnectionGroupObject();
                    break;
                case S1110Consts.RESOURCE_CTICONNECTIONGROUPCOLLECTION:
                    configObject = new CTIConnectionGroupCollectionObject();
                    break;
                case S1110Consts.RESOURCE_DOWNLOADPARAM:
                    configObject = new DownloadParamObject();
                    break;
                case S1110Consts.RESOURCE_ALARMMONITORPARAM:
                    configObject = new AlarmMonitorParamObject();
                    break;
                case S1110Consts.RESOURCE_CTIDBBRIDGE:
                    configObject = new CTIDBBServerObject();
                    break;
                case S1110Consts.RESOURCE_ALARMSERVICE:
                    configObject = new AlarmServiceObject();
                    break;
                case S1110Consts.RESOURCE_ALARMPROCESS:
                    configObject = new AlarmProcessObject();
                    break;
                default:
                    configObject = new ConfigObject();
                    break;
            }
            configObject.ObjectType = objType;
            return configObject;
        }

        #endregion


        #region Others

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", ObjectID, Name);
        }

        #endregion


        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

    }
}
