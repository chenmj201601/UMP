//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    282c196f-e6d4-4e49-bb3a-734700e1bfba
//        CLR Version:              4.0.30319.18063
//        Name:                     ViewItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                LicenseMonitor
//        File Name:                ViewItem
//
//        created by Charley at 2015/9/13 20:21:22
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using System.Reflection;
using VoiceCyber.SDKs.Licenses;

namespace LicenseMonitor
{
    public class ViewItem : INotifyPropertyChanged
    {
        public object Data { get; set; }

        public void OnPropertyChanged()
        {
            PropertyInfo[] propertyInfos = GetType().GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                OnPropertyChanged(propertyInfo.Name);
            }
        }

        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    public class ClientViewItem : ViewItem
    {
        public string Session { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public ClientType Type { get; set; }
        public string ModuleNumber { get; set; }
        public string Name { get; set; }
        public string ConnTime { get; set; }
        public string Protocol { get; set; }
        public string Expiration { get; set; }
        public string Endpoint { get; set; }
    }

    public class LicensePoolViewItem : ViewItem
    {
        public string Name { get; set; }
        public string Expiration { get; set; }
    }

    public class SoftdogViewItem : ViewItem
    {
        public string SerialNumber { get; set; }
        public SoftdogType Type { get; set; }
        public string Master { get; set; }
        public string Periods { get; set; }
        public string Expiration { get; set; }
        public bool IsCurrent { get; set; }
        public string CurTag { get; set; }
    }

    public class LicenseServerViewItem : ViewItem
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int ModuleNumber { get; set; }
    }

    public class PropertyViewItem : ViewItem
    {
        public string Name { get; set; }
        public string SerialNo { get; set; }
        public string Catetory { get; set; }
        public string LicType { get; set; }
        public string Value { get; set; }
        public string Expiration { get; set; }
        public string MajorID { get; set; }
        public string MinorID { get; set; }
    }
}
