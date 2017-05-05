using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml;
using PFShareClassesS;

namespace UMPService08
{
    class ABCDManager
    {
        #region  日志检查
        private bool IBoolIsThreadLogCheckWorking;

        private Thread IThreadLogCheck;
        #endregion

        #region ABCD统计
        private bool IBoolIsThreadABCDStatisticsWorking;

        private Thread IThreadABCDStatistics;
        #endregion

        #region 数据库信息
        private bool IBoolIsDBInfoWorking;
        private Thread IThreadDBInfo;

        public static int IIntDatabaseType = 0;
        public static string IStrDatabaseProfile = string.Empty;
        #endregion


        public ABCDManager() 
        {
            IBoolIsThreadABCDStatisticsWorking = true;
            IBoolIsThreadLogCheckWorking = true;
            IBoolIsDBInfoWorking = true;
            IIntDatabaseType = 0;
            IStrDatabaseProfile = string.Empty;
        }


        public void ABCDManagerStartup() 
        {
            FileLog.WriteInfo("ABCDManagerStartup() ", "start");
            IBoolIsThreadLogCheckWorking = false;
            IBoolIsThreadABCDStatisticsWorking = false;
            IBoolIsDBInfoWorking = false;

            if(IThreadABCDStatistics !=null)
            {
                IThreadABCDStatistics.Abort();
            }
            if(IThreadLogCheck !=null)
            {
                IThreadLogCheck.Abort();
            }
            if (IThreadDBInfo != null)
            {
                IThreadDBInfo.Abort();
            }

            IThreadDBInfo = new Thread(new ParameterizedThreadStart(ABCDManager.AutoDBCheckThread));
            IBoolIsDBInfoWorking = true;
            IThreadDBInfo.Start(this);
            FileLog.WriteInfo("ABCDManagerStartup()", "IThreadDBInfo Start");


            IThreadLogCheck = new Thread(new ParameterizedThreadStart(ABCDManager.AutoLogCheckThread));
            IBoolIsThreadLogCheckWorking = true;
            IThreadLogCheck.Start(this);
            FileLog.WriteInfo("ABCDManagerStartup()", "IThreadLogCheck Start");


            IThreadABCDStatistics = new Thread(new ParameterizedThreadStart(ABCDManager.AutoABCDStatisticsThread));
            IBoolIsThreadABCDStatisticsWorking = true;
            IThreadABCDStatistics.Start(this);
            FileLog.WriteInfo("ABCDManagerStartup()", "IThreadABCDStatistics Start");
        }


        private static void AutoDBCheckThread(object o) 
        {
            ABCDManager autoDBInfo = o as ABCDManager;
            while (autoDBInfo.IBoolIsDBInfoWorking)
            {
                try
                {
                    if (IIntDatabaseType == 0 || string.IsNullOrEmpty(IStrDatabaseProfile))
                    {
                        autoDBInfo.GetDatabaseConnectionProfile();
                        Thread.Sleep(1000);
                    }
                    Thread.Sleep(1000*15);
                }
                catch (Exception e)
                {   
                    FileLog.WriteError("AutoDBCheckThread() " , e.Message);
                    Thread.Sleep(5 * 60 * 1000);
                }
            }
        }


        /// <summary>
        /// 日志检查线程 
        /// </summary>
        private static void AutoLogCheckThread(object o)
        {
            ABCDManager autoLogCheck = o as ABCDManager;
            LogOperation logOperation = new LogOperation();

            while (autoLogCheck.IBoolIsThreadLogCheckWorking)
            {
                try
                {
                    logOperation.LogCompressionAndDelete();
                    Thread.Sleep(100);
                }
                catch (Exception e)
                {
                    
                    FileLog.WriteError("AutoLogCheckThread() " , e.Message);
                    Thread.Sleep(5 * 60 * 1000);
                }
            }
        }


        /// <summary>
        /// 统计服务线程 
        /// </summary>
        /// <param name="o"></param>
        private static void AutoABCDStatisticsThread(object o) 
        {
            ABCDManager autoABCDStatistics = o as ABCDManager;
            ABCDStatistics abcdStatistics = new ABCDStatistics();
            while(autoABCDStatistics.IBoolIsThreadABCDStatisticsWorking)
            {
                //+++++++++++++++++++++++++++++++++++++++++++++++++++
                //IIntDatabaseType = 3;
                //IStrDatabaseProfile = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.4.182) (PORT=1521)))(CONNECT_DATA=(SERVICE_NAME= PFOrcl)));User Id=PFDEV831; Password=pfdev831";
                //pmFirstStatistics.IDatabaseConfig.IntDatabaseType = 3;

                //pmFirstStatistics.IDatabaseConfig.StrDatabaseProfile = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.4.182) (PORT=1521)))(CONNECT_DATA=(SERVICE_NAME= PFOrcl)));User Id=PFDEV831; Password=pfdev831";



                //IIntDatabaseType = 2;
                //IStrDatabaseProfile = "Data Source=192.168.7.101,1433;Initial Catalog=UMPDataDB0509;User Id=sa;Password=voicecodes";
                //abcdStatistics.IIntDatabaseType = 2;
                //abcdStatistics.IStrDatabaseProfile = "Data Source=192.168.7.101,1433;Initial Catalog=UMPDataDB0509;User Id=sa;Password=voicecodes";
                //////+++++++++++++++++++++++++++++++++++++++++++++++++++

                 //读数据库连接串              
                if (IIntDatabaseType == 0 || string.IsNullOrEmpty(IStrDatabaseProfile))
                { 
                        Thread.Sleep(1000*1);
                        continue;
                } 

                try
                {
                    //读取所有的运行周期的
                    abcdStatistics.IIntDatabaseType = IIntDatabaseType;
                    abcdStatistics.IStrDatabaseProfile = IStrDatabaseProfile;
                    abcdStatistics.RunABCDStatistics();
                    Thread.Sleep(1000 * 15);
                }
                catch (Exception e)
                {
                    FileLog.WriteError("AutoLogCheckThread() ", e.Message);
                    Thread.Sleep(1000 * 15);
                }
            }

        }

        public void ABCDManagerStop() 
        {
            FileLog.WriteInfo("ABCDManagerStop() ", "start");
            IBoolIsThreadLogCheckWorking = false;
            IBoolIsThreadABCDStatisticsWorking = false;

            if (IThreadABCDStatistics != null)
            {
                IThreadABCDStatistics.Abort();
                IThreadABCDStatistics = null;
            }

            if (IThreadLogCheck != null)
            {
                IThreadLogCheck.Abort();
                IThreadLogCheck = null;
            }            
        }



        #region 创建加密解密验证字符串
        private string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
        {
            string LStrReturn = string.Empty;
            int LIntRand = 0;
            string LStrTemp = string.Empty;

            try
            {
                Random LRandom = new Random();
                LStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = LRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "VCT");
                LIntRand = LRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "UMP");
                LIntRand = LRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, ((int)AKeyIVID).ToString("000"));

                LStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + LStrReturn);
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }
        #endregion

        #region 读取数据库连接信息
        public void GetDatabaseConnectionProfile()
        {
            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode = string.Empty;

            string LStrAttributesData = string.Empty;
            //0:数据库服务器；1：端口；2：数据库名或服务名；3：登录用户；4：登录密码；5：其他参数
            List<string> LListStrDBProfile = new List<string>();

            try
            {
                IIntDatabaseType = 0;
                IStrDatabaseProfile = string.Empty;

                LStrVerificationCode = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LStrXmlFileName = 
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);  
                   // System.AppDomain.CurrentDomain.BaseDirectory;//测试用
                     
                    
                LStrXmlFileName = System.IO.Path.Combine(LStrXmlFileName, @"UMP.Server\Args01.UMP.xml");

                XmlDocument LXmlDocArgs01 = new XmlDocument();
                LXmlDocArgs01.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListDatabase = LXmlDocArgs01.SelectSingleNode("DatabaseParameters").ChildNodes;

                #region 读取数据库连接参数
                foreach (XmlNode LXmlNodeSingleDatabase in LXmlNodeListDatabase)
                {
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P03"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    if (LStrAttributesData != "1") { continue; }

                    //数据库类型
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P02"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    IIntDatabaseType = int.Parse(LStrAttributesData);

                    //数据库服务器名或IP地址
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P04"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LListStrDBProfile.Add(LStrAttributesData);

                    //数据库服务端口
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P05"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LListStrDBProfile.Add(LStrAttributesData);

                    //数据库名或Service Name
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P06"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LListStrDBProfile.Add(LStrAttributesData);

                    //登录用户
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P07"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LListStrDBProfile.Add(LStrAttributesData);

                    //登录密码
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P08"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LListStrDBProfile.Add(LStrAttributesData);

                    //其他参数
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P09"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LListStrDBProfile.Add(LStrAttributesData);

                    break;
                }
                #endregion

                #region 创建数据库连接字符串
                string LStrDBConnectProfile = string.Empty;

                if (IIntDatabaseType == 2)
                {
                    IStrDatabaseProfile = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], LListStrDBProfile[4]);
                    LStrDBConnectProfile = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], "******");
                    LStrDBConnectProfile = "DataBase Type : MS SQL Server\n" + LStrDBConnectProfile;
                }
                if (IIntDatabaseType == 3)
                {
                    IStrDatabaseProfile = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], LListStrDBProfile[4]);
                    LStrDBConnectProfile = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], "******");
                    LStrDBConnectProfile = "DataBase Type : Oracle\n" + LStrDBConnectProfile;
                }
                FileLog.WriteInfo("GetDatabaseConnectionProfile()", LStrDBConnectProfile);

                #endregion
            }
            catch (Exception)
            {
                Thread.Sleep(1000 * 10);
                IIntDatabaseType = 0;
                IStrDatabaseProfile = string.Empty;
            }
        }

        #endregion

    }
}
