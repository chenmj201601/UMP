using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService09.Model
{
    //数据库信息类
    public  class DataBaseConfig
    {
        //本数据库信息
        public int IntDatabaseType { set; get; }
        public string StrDatabaseProfile { set; get; }


        //分库后PM专用数据库信息
        public int IntDatabaseTypeBackup { set; get; }
        public string StrDatabaseProfileBackup { set; get; }

    }
}
