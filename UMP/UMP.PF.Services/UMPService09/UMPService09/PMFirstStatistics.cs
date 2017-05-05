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
    public class PMFirstStatistics :BasicMethod
    {

        public DataBaseConfig IDatabaseConfig;
        protected static List<ServiceConfigInfo> IListServiceConfigInfo;
        //全局参数
        protected GlobalSetting IGlobalSetting;


        public PMFirstStatistics() 
        {
            IListServiceConfigInfo = new List<ServiceConfigInfo>();
            IDatabaseConfig = new DataBaseConfig();
            IGlobalSetting = new GlobalSetting();
        }


        public void RunFirstStatistics()
        {
            string LPath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
            string LFilePath = System.IO.Path.Combine(LPath,StatisticsConstDefine.XmlFileName);
            if(ReadXml(LFilePath))
            {
                if (IListServiceConfigInfo !=null)
                {
                    //服务启动时间
                    string LStrStartTime = ConfigurationManager.AppSettings["StartTime"] != null ? ConfigurationManager.AppSettings["StartTime"] : "020000";
                    LStrStartTime = DateTime.Now.Date.ToString("yyyyMMdd") + LStrStartTime;
                    DateTime LStartTime = StringToDateTime(LStrStartTime);

                    //初次统计
                    foreach (ServiceConfigInfo serviceconfiginfo in IListServiceConfigInfo)
                    {

                        //serviceconfiginfo.StartTime这个时间写统计结束的时间
                        if (LStartTime < DateTime.Now && serviceconfiginfo.StartTime < DateTime.Today && serviceconfiginfo.IsStart)
                        {
                            //读取全局参数
                            List<string> LListStrRent = new List<string>();
                            List<string> LListTableNameInfo = new List<string>();
                            string LStrTemp = string.Empty;
                            //得到租户信息
                            DALCommon.ObtainRentList(IDatabaseConfig, ref  LListStrRent);
                            foreach (string strRent in LListStrRent)
                            {
                                IGlobalSetting.StrRent = strRent;
                                int LlogicPartMark = DALCommon.ObtainRentLogicTable(IDatabaseConfig, strRent, StatisticsConstDefine.Const_ColumnName_LPRecord);

                                //1为有分表 2为无分表
                                if (LlogicPartMark == 0) { FileLog.WriteInfo("DoAction()", "LlogicPartMark == 0"); continue; }
                                IGlobalSetting.IlogicPartMark = LlogicPartMark;

                                if (LlogicPartMark == 1)
                                {
                                    LListTableNameInfo = new List<string>();
                                    DALCommon.ObtainRentExistLogicPartitionTables(IDatabaseConfig, strRent, StatisticsConstDefine.Const_TableName_Record, ref LListTableNameInfo);
                                }

                                IGlobalSetting.LStrRecordName = LListTableNameInfo;

                                //全局参数里座席分机真实分机
                                LStrTemp = string.Empty;
                                DALGlobalSetting.GetGlobalSetting(IDatabaseConfig, ref LStrTemp, strRent, StatisticsConstDefine.Const_Agent_Extension);
                                if (LStrTemp.Contains('R')) //如有真实分机则统计座席和真实分机的信息
                                {
                                    IGlobalSetting.StrConfigAER = "R";
                                }
                                else  //其它则统计座席和分机的信息
                                {
                                    IGlobalSetting.StrConfigAER = "A";
                                }

                                //全局周的配置
                                LStrTemp = string.Empty;
                                DALGlobalSetting.GetGlobalSetting(IDatabaseConfig, ref LStrTemp, strRent, StatisticsConstDefine.Const_Week_Config);
                                IGlobalSetting.StrWeekStart = LStrTemp;

                                //全局月的配置
                                LStrTemp = string.Empty;
                                DALGlobalSetting.GetGlobalSetting(IDatabaseConfig, ref LStrTemp, strRent, StatisticsConstDefine.Const_Month_Config);
                                IGlobalSetting.StrMonthStart = LStrTemp;

                                if (serviceconfiginfo.StatistcsName != StatisticsConstDefine.KPIStatisticsName && serviceconfiginfo.IsStart)
                                {
                                    IStatistics iStatistics = DALFactory.DataAccess.CreateStatistic(serviceconfiginfo.StatistcsName);
                                    iStatistics.DoAction(IDatabaseConfig, serviceconfiginfo, IGlobalSetting);
                                }

                                if (serviceconfiginfo.StatistcsName == StatisticsConstDefine.KPIStatisticsName && serviceconfiginfo.IsStart)
                                {
                                    //kpi计算
                                    KPIStatistics kpiStatistics = new KPIStatistics();
                                    kpiStatistics.DoAction(IDatabaseConfig, serviceconfiginfo, IGlobalSetting);
                                }

                                Thread.Sleep(10);
                            }
                        }
                    }
                }
                //保存统计时间到xml
                SaveXml(LFilePath);
                Thread.Sleep(60*1000);
            }
        }

        /// <summary>
        /// 将统计时间写入xml里
        /// </summary>
        /// <returns></returns>
        protected void SaveXml(string AFileName) 
        {
            if (File.Exists(AFileName))
            {
                try
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.Load(AFileName);
                    XmlNodeList lstXmlNodeList = xdoc.SelectNodes("PMStatisticsTime/StatisticsName");
                    foreach (XmlNode xnode in lstXmlNodeList)
                    {
                        if (xnode != null)
                        {
                            xnode.Attributes["StartTime"].Value = DateTime.Now.Date.ToString();
                        }
                    }
                    xdoc.Save(AFileName);
                    xdoc = null;
                }
                catch (Exception e)
                {
                    FileLog.WriteInfo("SaveXml()", e.Message.ToString());                    
                }
            }
            FileLog.WriteInfo("SaveXml", "");
        }


        /// <summary>
        /// 将配置信息读到类里
        /// </summary>
        /// <param name="AFileName"></param>
        /// <param name="AXmlNodeName"></param>
        /// <returns></returns>
        protected bool ReadXml(string AFileName)
        {
            bool flag = true;
            if (IListServiceConfigInfo==null)
            {
                IListServiceConfigInfo = new List<ServiceConfigInfo>();
            }
            else
            {
                IListServiceConfigInfo.Clear();
            }
                
           
            if (File.Exists(AFileName))
            {
                try
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.Load(AFileName);
                    XmlNodeList lstXmlNodeList = xdoc.SelectNodes("PMStatisticsTime/StatisticsName");
                    foreach (XmlNode xnode in lstXmlNodeList)
                    {
                        if (xnode != null)
                        {
                            ServiceConfigInfo serviceConfigInfo = new ServiceConfigInfo();
                            serviceConfigInfo.StatistcsName = xnode.Attributes["name"].Value ?? StatisticsConstDefine.QMStatisticsName;
                            serviceConfigInfo.IsStart = xnode.Attributes["IsActive"].Value == "1";
                            serviceConfigInfo.StartTime = BasicMethod.DateTimeParse(xnode.Attributes["StartTime"].Value, new DateTime(DateTime.Today.Year, 1, 1));
                            if (IListServiceConfigInfo == null)
                            {
                                IListServiceConfigInfo = new List<ServiceConfigInfo>();
                            }
                            IListServiceConfigInfo.Add(serviceConfigInfo);
                        }
                    }
                }
                catch (Exception e)
                {
                    FileLog.WriteInfo("ReadXml()", e.Message.ToString());
                    return false;
                }
            }
            else 
            {
                return false;
            }
            return flag;
        }
  
    }
}
 