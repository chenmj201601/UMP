using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMP.PF.MAMT.Classes
{
    public class Enums
    {
        public enum Model
        {
            DBModel = 0,        
            IISModel,
            LanguageModel
        }

        public enum DBType 
        {
            MySql = 1,
            MSSQL ,
            Oracle
        }

        public enum ImportOperationType
        {
            Refresh=1,
            Append,
            Update
        }

        /// <summary>
        /// 操作
        /// </summary>
        public enum OperationStatus
        {
            Wait=1,     //等待操作
            Success,    //操作成功
            Error       //操作失败
        }
    }
}
