using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMP.PF.MAMT.Classes;

namespace UMP.PF.MAMT.Classes
{
    public class DBInfo
    {
        int _dbType;

        public int DbType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }
        string _Host;

        public string Host
        {
            get { return _Host; }
            set { _Host = value; }
        }
        string _Port;

        public string Port
        {
            get { return _Port; }
            set { _Port = value; }
        }
        string _ServiceName;

        public string ServiceName
        {
            get { return _ServiceName; }
            set { _ServiceName = value; }
        }
        string _LoginName;

        public string LoginName
        {
            get { return _LoginName; }
            set { _LoginName = value; }
        }
        string _Password;

        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }
    }

    /// <summary>
    /// 数据库对象实体类
    /// </summary>
    public class DatabaseObject
    {
        public DatabaseObject(string dboName, string dboType, Enums.OperationStatus dboStatus, string dboOperationMessage)
        {
            DatabaseObjectName = dboName;
            DatabaseObjectType = dboType;
            switch (dboStatus)
            {
                case Enums.OperationStatus.Wait:
                    DatabaseIconPath = System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000033.ico");
                    break;
                case Enums.OperationStatus.Success:
                    DatabaseIconPath = System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000032.ico");
                    break;
                case Enums.OperationStatus.Error:
                    DatabaseIconPath = System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000031.ico");
                    break;
            }
            DatabaseObjectOperationMessage = dboOperationMessage;
        }

        private string _DatabaseIconPath;

        public string DatabaseIconPath
        {
            get { return _DatabaseIconPath; }
            set { _DatabaseIconPath = value; }
        }
        private string _DatabaseObjectName;

        public string DatabaseObjectName
        {
            get { return _DatabaseObjectName; }
            set { _DatabaseObjectName = value; }
        }

        private string _DatabaseObjectType;

        public string DatabaseObjectType
        {
            get { return _DatabaseObjectType; }
            set { _DatabaseObjectType = value; }
        }

        private string _DatabaseObjectStatus;

        public string DatabaseObjectStatus
        {
            get { return _DatabaseObjectStatus; }
            set { _DatabaseObjectStatus = value; }
        }

        private string _DatabaseObjectOperationMessage;

        public string DatabaseObjectOperationMessage
        {
            get { return _DatabaseObjectOperationMessage; }
            set { _DatabaseObjectOperationMessage = value; }
        }
    }
}
