//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3420b94f-b15a-4257-b4bb-6641334e696c
//        CLR Version:              4.0.30319.18444
//        Name:                     DescriptionInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                DescriptionInfo
//
//        created by Charley at 2015/2/1 22:28:45
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Controls;

namespace UMPS1110.Models
{
    public class DescriptionInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 类型
        /// 0       Property Description, Data is ObjectPropertyInfo
        /// 1       Object Description, Data is ConfigObject
        /// </summary>
        public int Type { get; set; }
        private string mDescription;

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }

        public object Data { get; set; }

        public UMPApp CurrentApp;

        public DescriptionInfo()
        {

        }

        public DescriptionInfo(int type, object data,UMPApp currentApp)
        {
            Type = type;
            switch (type)
            {
                case 0:
                    ObjectPropertyInfo propertyInfo = data as ObjectPropertyInfo;
                    if (propertyInfo == null) { return; }
                    mDescription =
                        currentApp.GetLanguageInfo(
                            string.Format("PROD{0}{1}", propertyInfo.ObjType.ToString("000"),
                                propertyInfo.PropertyID.ToString("000")), propertyInfo.Description);
                    break;
                case 1:
                    ConfigObject configObject = data as ConfigObject;
                    if (configObject == null) { return; }
                    mDescription = configObject.Description;
                    break;
            }
            Data = data;
            CurrentApp = currentApp;
        }

        public void RefreshData()
        {
            switch (Type)
            {
                case 0:
                    ObjectPropertyInfo propertyInfo = Data as ObjectPropertyInfo;
                    if (propertyInfo == null) { return; }
                    Description =
                        CurrentApp.GetLanguageInfo(
                            string.Format("PROD{0}{1}", propertyInfo.ObjType.ToString("000"),
                                propertyInfo.PropertyID.ToString("000")), propertyInfo.Description);
                    break;
                case 1:
                    ConfigObject configObject = Data as ConfigObject;
                    if (configObject == null) { return; }
                    Description = configObject.Name;
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
