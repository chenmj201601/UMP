using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UMPService09.Utility;
using UMPService09.Utility.InterFace;
using UMPService09.DALFactory;
using UMPService09.Log;
using UMPService09.Model;
using System.IO;
using System.Xml;
using UMPService09.DAL;
using System.Configuration;


namespace UMPService09
{
     public  class DataDelete:BasicMethod
    {
        public DataBaseConfig IDatabaseConfig;
        //全局参数
        protected GlobalSetting IGlobalSetting;


        public DataDelete() 
        {
            IDatabaseConfig = new DataBaseConfig();
            IGlobalSetting = new GlobalSetting();
        }


        public void RunDataDelete() 
        {
            if (IDatabaseConfig != null)
            {
                List<string> listRentInfo = new List<string>();
                string strSql = string.Empty;
                long tempDataTime = 0;
                string tableName=string.Empty;
                List<string> listTableName = new List<string>();
                tempDataTime = Convert.ToInt64(DateTime.Now.ToString("HHmm"));
                if (tempDataTime < 200 || tempDataTime>210) { return; }
                //获取租户信息
                DALCommon.ObtainRentList(IDatabaseConfig, ref  listRentInfo);
                if (listRentInfo.Count() <= 0) { return; }
                foreach (string strRent in listRentInfo)
                {
                    tempDataTime = Convert.ToInt64(DateTime.Now.AddDays(-7).ToString("yyyyMMddHHmmss"));//天之后的切片只保存7天
                    tableName = string.Format("T_46_011_{0}", strRent);
                    strSql = string.Format("DELETE FROM {0} WHERE C007<{1}", tableName, tempDataTime);
                    RunDataDelete(strSql, tableName);

                    tempDataTime = Convert.ToInt64(DateTime.Now.AddMonths(-6).ToString("yyyyMMddHHmmss"));//天的切片只保存6个月
                    tableName = string.Format("T_46_012_{0}", strRent);
                    strSql = string.Format("DELETE FROM {0} WHERE C005<{1}", tableName, tempDataTime);
                    RunDataDelete(strSql, tableName);

                    tempDataTime = Convert.ToInt64(DateTime.Now.AddYears(-3).ToString("yyyyMMddHHmmss"));//周的切片只保存3年
                    tableName = string.Format("T_46_013_{0}", strRent);
                    strSql = string.Format("DELETE FROM {0} WHERE C005<{1}", tableName, tempDataTime);
                    RunDataDelete(strSql, tableName);

                    tempDataTime = Convert.ToInt64(DateTime.Now.AddYears(-3).ToString("yyyyMMddHHmmss"));//月的切片只保存3年
                    tableName = string.Format("T_46_014_{0}", strRent);
                    strSql = string.Format("DELETE FROM {0} WHERE C005<{1}", tableName, tempDataTime);
                    RunDataDelete(strSql, tableName);

                    tempDataTime = Convert.ToInt64(DateTime.Now.AddYears(-10).ToString("yyyyMMddHHmmss"));//年的切片只保存10年
                    tableName = string.Format("T_46_015_{0}", strRent);
                    strSql = string.Format("DELETE FROM {0} WHERE C005<{1}", tableName, tempDataTime);
                    RunDataDelete(strSql, tableName);
                }
                listTableName.Add("T_46_022");
                listTableName.Add("T_46_023");
                listTableName.Add("T_46_024");
                listTableName.Add("T_46_025");
                listTableName.Add("T_46_031");
                listTableName.Add("T_46_032");
                listTableName.Add("T_46_033");
                listTableName.Add("T_46_034");
                listTableName.Add("T_46_035");
                listTableName.Add("T_46_041");
                listTableName.Add("T_46_042");
                listTableName.Add("T_46_043");
                listTableName.Add("T_46_044");
                listTableName.Add("T_46_045");
                foreach(string tempString in listTableName)
                {
                    string intString = tempString.Substring(tempString.Length-1,1);
                    switch (intString)
                    {
                        case"1":
                            tempDataTime = Convert.ToInt64(DateTime.Now.AddDays(-7).ToString("yyyyMMddHHmmss"));//天之后的切片只保存7天
                            break;
                        case "2":
                            tempDataTime = Convert.ToInt64(DateTime.Now.AddMonths(-6).ToString("yyyyMMddHHmmss"));//天的切片只保存6个月
                            break;
                        case "3":
                            tempDataTime = Convert.ToInt64(DateTime.Now.AddYears(-3).ToString("yyyyMMddHHmmss"));//周的切片只保存3年
                            break;
                        case "4":
                            tempDataTime = Convert.ToInt64(DateTime.Now.AddYears(-3).ToString("yyyyMMddHHmmss"));//月的切片只保存3年
                            break;
                        case "5":
                            tempDataTime = Convert.ToInt64(DateTime.Now.AddYears(-10).ToString("yyyyMMddHHmmss"));//年的切片只保存10年
                            break;
                    }
                    tableName = tempString;
                    strSql = string.Format("DELETE FROM {0} WHERE C006<{1}", tableName, tempDataTime);
                    RunDataDelete(strSql, tableName);
                }
            }
        }

        private void RunDataDelete(string strSql,string tableName)
        {
            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
                LDatabaseOperationReturn = LDataOperations.ExecuteDynamicSQL(IDatabaseConfig.IntDatabaseType, IDatabaseConfig.StrDatabaseProfile, strSql);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    FileLog.WriteInfo("DeleteData(),Error", strSql);
                    return;
                }
                if (!string.IsNullOrWhiteSpace(LDatabaseOperationReturn.StrReturn) && Convert.ToInt32(LDatabaseOperationReturn.StrReturn) > 0)
                {
                    FileLog.WriteInfo(string.Format("DeleteData({0})", tableName), LDatabaseOperationReturn.StrReturn);
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("DeleteData()", "Error :" + ex.Message);
            }
        }

    }
}
