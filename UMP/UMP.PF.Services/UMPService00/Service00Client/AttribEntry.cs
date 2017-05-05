using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService00
{
    /// <summary>
    /// 属性实体类
    /// </summary>
    public class AttribEntry
    {
        public AttribEntry(string strName, string strContent)
        {
            AttribName = strName;
            AttribContent = strContent;
        }

        private string _AttribName;

        /// <summary>
        /// 属性名 
        /// </summary>
        public string AttribName
        {
            get { return _AttribName; }
            set { _AttribName = value; }
        }
        private string _AttribContent;

        /// <summary>
        /// 属性值
        /// </summary>
        public string AttribContent
        {
            get { return _AttribContent; }
            set { _AttribContent = value; }
        }
    }

    /// <summary>
    /// PBXDevice实体类
    /// </summary>
    public class PBXDeviceEntry
    {
        private string _Key;

        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }
        private string _Enable;

        public string Enable
        {
            get { return _Enable; }
            set { _Enable = value; }
        }
        private string _CTIType;

        public string CTIType
        {
            get { return _CTIType; }
            set { _CTIType = value; }
        }
        private string _DeviceType;

        public string DeviceType
        {
            get { return _DeviceType; }
            set { _DeviceType = value; }
        }
        private string _MonitorMode;

        public string MonitorMode
        {
            get { return _MonitorMode; }
            set { _MonitorMode = value; }
        }
        private string _Value;

        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
    }

    /// <summary>
    /// LicenseServer实体类
    /// </summary>
    public class LicenseServer
    {
        private string _Host;

        public string Host
        {
            get { return _Host; }
            set { _Host = value; }
        }
        private int _Port;

        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }
        private int _IsMain;

        public int IsMain
        {
            get { return _IsMain; }
            set { _IsMain = value; }
        }
    }
}
