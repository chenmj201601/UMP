using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml;
using PFShareClassesS;
using System.Data;
using PFShareClasses01;
using System.Text.RegularExpressions;
using VoiceCyber.Common;
using System.Data.Common;

namespace UMPService08
{
    public class ABCDStatistics
    {

        public int IIntDatabaseType
        {
            set;
            get;
        }

        public string IStrDatabaseProfile 
        {
            set; get; 
        }

        public static string IStrSpliterChar = string.Empty;        
        private string IStrVerificationCode102 = string.Empty;
        private string IStrVerificationCode002 = string.Empty;
        private List<OrgInfo> IListOrgInfo = null;
        private List<AgentInfo> IListAgentInfo = null;
        private List<UserInfo> IListUserInfo = null;
        private List<string> IListStringTableInfo = null;


        public ABCDStatistics() 
        {
            IStrSpliterChar = AscCodeToChr(27);
            IStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
            IStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
            IListOrgInfo = new List<OrgInfo>();
            IListAgentInfo = new List<AgentInfo>();
            IListUserInfo = new List<UserInfo>();
            IListStringTableInfo = new List<string>();
        }


        public void RunABCDStatistics() 
        {
            List<string> LListStrRentExist = new List<string>();
            ObtainRentList(ref  LListStrRentExist);
            if(LListStrRentExist.Count>0)
            {
                foreach(string StrRent in LListStrRentExist)
                {
                    //1、获取租户下所有的梆定以及运行周期
                    List<StatisticsMapping> LListStatisticsMapping = new List<StatisticsMapping>();
                    FileLog.WriteInfo("RunABCDStatistics()", "GetAllBinding() StrRent :" + StrRent);
                    GetAllBinding(ref LListStatisticsMapping, StrRent);
                    if (LListStatisticsMapping.Count == 0) { continue; }

                    List<string> LListStrFrequenceID = new List<string>();
                    LListStrFrequenceID.Add(string.Empty);
                    LListStrFrequenceID.Add(LListStatisticsMapping.Count.ToString());
                    foreach (StatisticsMapping FreMap in LListStatisticsMapping)
                    {
                        string strInfo = string.Format("{0}{1}", FreMap.FrequencyID, AscCodeToChr(27));
                        LListStrFrequenceID.Add(strInfo);
                    }

                    FileLog.WriteInfo("RunABCDStatistics()", "InsertTempData()");
                    OperationReturn operationTemp = InsertTempData(LListStrFrequenceID, StrRent);
                    if (!operationTemp.Result)  { continue; }
                    string LSeriID = operationTemp.Data.ToString();

                    List<RunFrequence> LListRunFrequence = new List<RunFrequence>();
                    FileLog.WriteInfo("RunABCDStatistics()", "GetAllRunFrequency()");
                    GetAllRunFrequency(ref LListRunFrequence, StrRent, LSeriID);
                    if (LListRunFrequence.Count == 0) { continue; }

                    //2、   录音表有无年月分表1、有按月分表 2、 无按月分表 0、 运行错误
                    FileLog.WriteInfo("RunABCDStatistics()", "ObtainRentLogicTable()");
                    int LlogicPartMark = ObtainRentLogicTable(StrRent, UMPService08Const.Const_ColumnName_LPRecord);
                    if (LlogicPartMark == 0) { continue; }

                    //2.0 如果分表将录音按月查询出来
                    if(LlogicPartMark==1)
                    {
                        IListStringTableInfo = new List<string>();
                        ObtainRentExistLogicPartitionTables(StrRent, UMPService08Const.Const_TableName_Record,ref IListStringTableInfo);
                    }


                    //2.1 读取统计大项
                    List<StatisticsCatogary> LListStatisticsCatogary = new List<StatisticsCatogary>();
                    FileLog.WriteInfo("RunABCDStatistics()", "GetAllStatisticsCatogary()");
                    GetAllStatisticsCatogary(ref LListStatisticsCatogary, StrRent);

                    //2.2读取统计小项
                    List<StatisticsItem> LListStatisticsItem= new List<StatisticsItem>();
                    FileLog.WriteInfo("RunABCDStatistics()", "GetAllStatisticsItem()");
                    GetAllStatisticsItem(ref LListStatisticsItem, StrRent);

                    //2.3读取统计项梆定机构或技能组的配置值 
                    List<StatisticsSetValue> LListStatisticsSetValue = new List<StatisticsSetValue>();
                    GetStatisticsSetValue(StrRent, "314", "315", "C002", ref LListStatisticsSetValue);
                    //MappingIDOrStatisticsID  如果是311 则是默认写的值 如是312开头则是mapping时写的值



                    //2.4读取周，月配置值
                    string LStrWeekConfig = string.Empty;
                    string LStrMonthConfig = string.Empty;

                    GetGlobalSetting(ref LStrWeekConfig, StrRent, UMPService08Const.Const_Week_Config);
                    GetGlobalSetting(ref LStrMonthConfig, StrRent, UMPService08Const.Const_Month_Config);

                    string LStrAgentExtensionConfig = string.Empty;
                    GetGlobalSetting(ref LStrAgentExtensionConfig, StrRent, UMPService08Const.Const_Agent_Extension);

                    IListOrgInfo.Clear();
                    IListAgentInfo.Clear();
                    IListUserInfo.Clear();
                    GetAllOrgInfo(ref IListOrgInfo, StrRent);
                    GetAllAgentInfo(ref IListAgentInfo, StrRent);
                    GetAllUserInfo(ref IListUserInfo, StrRent);
                    
                    //3、检查梆定运行时间是否要执行统计
                    foreach(RunFrequence LRunFrequence in LListRunFrequence)
                    {
                        bool LBoolIsCanRun = false;
                        IsCanStatistics(ref LBoolIsCanRun, LRunFrequence);

                        if (!LBoolIsCanRun) { Thread.Sleep(10); continue; }


                        //4、根据梆定信息读取T_31_044表的数据
                       StatisticsMapping LStatisticsMapping=null;
                       LStatisticsMapping = LListStatisticsMapping.Where(p => p.FrequencyID == LRunFrequence.FrequencyID).Count() == 0 ? null : LListStatisticsMapping.Where(p => p.FrequencyID == LRunFrequence.FrequencyID).First();
                       if (LStatisticsMapping != null) 
                       {
                           //5、根据StatisticsID，读取大项和小项信息
                           //6、运算统计
                           //7、将结果写入数据库表中T_31_054(录音统计表)和T_31_055(成绩统计表)
                           StatisticsCatogary LStatisticsCatogary = null;
                           LStatisticsCatogary = LListStatisticsCatogary.Where(p => p.StatisticsID == LStatisticsMapping.StatisticsID).Count() == 0 ? null : LListStatisticsCatogary.Where(p => p.StatisticsID == LStatisticsMapping.StatisticsID).First();
                           if (LStatisticsCatogary == null) { continue; }
                           switch (LStatisticsCatogary.IsCanCombin)
                           {
                               case 1:
                                   {
                                       switch (LStatisticsCatogary.StatisticsID)
                                       {
                                           case UMPService08Const.Const_ServiceAttitude: //服务态度
                                           case UMPService08Const.Const_ProfessionalLevel://专业水平
                                               {
                                                   //机构下的人或者技能组下的人
                                                   List<AgentInfo> LListAgentInfoTemp = new List<AgentInfo>();
                                                   List<OrgInfo> LListOrgInfo = new List<OrgInfo>();
                                                   if (LStatisticsMapping.DropDown == 1) //平行时可能有技能组
                                                   {
                                                       if (LStatisticsMapping.ApplayAll == 2)//不应用到所有
                                                       {
                                                           GetAgentInfoInOrg(ref LListAgentInfoTemp, LStatisticsMapping.OrgIDOrSkillID, 1, StrRent);
                                                           Do_Combine_Statistics(StrRent, LStatisticsMapping, LStatisticsCatogary, LListStatisticsItem, LListStatisticsSetValue, LlogicPartMark, LStrWeekConfig, LStrMonthConfig, LListAgentInfoTemp);
                                                       }
                                                       else if (LStatisticsMapping.ApplayAll == 1)//应用所有
                                                       {
                                                           GetOrgInfoSon(ref  LListOrgInfo, LStatisticsMapping.OrgIDOrSkillID);
                                                           foreach (OrgInfo LOrgInfoTemp in LListOrgInfo)
                                                           {
                                                               LListAgentInfoTemp = new List<AgentInfo>();
                                                               GetAgentInfoInOrg(ref LListAgentInfoTemp, LStatisticsMapping.OrgIDOrSkillID, 1, StrRent);
                                                               Do_Combine_Statistics(StrRent, LStatisticsMapping, LStatisticsCatogary, LListStatisticsItem, LListStatisticsSetValue, LlogicPartMark, LStrWeekConfig, LStrMonthConfig, LListAgentInfoTemp);
                                                           }
                                                       }

                                                   }
                                                   else if (LStatisticsMapping.DropDown == 2)//钻取 只能是机构
                                                   {
                                                       GetAgentInfoInOrg(ref LListAgentInfoTemp, LStatisticsMapping.OrgIDOrSkillID, 2, StrRent);
                                                       Do_Combine_Statistics(StrRent, LStatisticsMapping, LStatisticsCatogary, LListStatisticsItem, LListStatisticsSetValue, LlogicPartMark, LStrWeekConfig, LStrMonthConfig, LListAgentInfoTemp);
                                                   }
                                                   Update_RunFrequence(StrRent, LRunFrequence);
                                               }
                                               break;
                                           default:
                                               break;
                                       }
                                   }
                                   break;
                               case 2:
                                   {
                                        switch (LStatisticsCatogary.StatisticsID)
	                                    {
                                            case UMPService08Const.Const_RepeatedCallin://重复呼入
                                                {
                                                    Do_RepeatedCallin(StrRent, LStatisticsMapping, LStatisticsCatogary, LListStatisticsItem, LListStatisticsSetValue, LlogicPartMark, LStrWeekConfig, LStrMonthConfig);
                                                    Update_RunFrequence(StrRent, LRunFrequence);
                                                }
                                                break;
                                            case UMPService08Const.Const_CallPeak://呼叫高峰期
                                                {
                                                    Do_CallPeak(StrRent, LStatisticsMapping, LStatisticsCatogary, LListStatisticsItem, LListStatisticsSetValue, LlogicPartMark, LStrWeekConfig, LStrMonthConfig);
                                                    Update_RunFrequence(StrRent, LRunFrequence);
                                                }
                                                break;
                                            case UMPService08Const.Const_ACSpeExceptProportion://座席/客户讲话时长比例异常
                                                { 
                                                }
                                                break;
                                            case UMPService08Const.Const_RecordDurationExcept://录音时长异常
                                                {
                                                    Do_RecordDurationExcept(StrRent, LStatisticsMapping, LStatisticsCatogary, LListStatisticsItem,LListStatisticsSetValue, LlogicPartMark, LStrWeekConfig, LStrMonthConfig);
                                                    Update_RunFrequence(StrRent, LRunFrequence);
                                                }
                                                break;
                                            case UMPService08Const.Const_AfterDealDurationExcept: //事后处理时长异常
                                                { 
                                                }
                                                break;
                                            case UMPService08Const.Const_ExceptionScore://异常分数
                                                { 
                                                }
                                                break;
		                                    default:
                                                break;
	                                    }
                                   }
                                   break;
                               default:
                                   break;
                           }
                       }
                       Thread.Sleep( 1000);  
                    }
                }
            }
        }


        /// <summary>
        /// 专业水平和服务态度的统计
        /// </summary>
        /// <param name="AStrRent"></param>
        /// <param name="AStatisticsMapping"></param>
        /// <param name="AStatisticsCatogary"></param>
        /// <param name="ALListStatisticsItem"></param>
        /// <param name="AListStatisticsSetValue"></param>
        /// <param name="ALlogicPartMark"></param>
        /// <param name="AStrWeekConfig"></param>
        /// <param name="AStrMontConfig"></param>
        private void Do_Combine_Statistics(string AStrRent, StatisticsMapping AStatisticsMapping, StatisticsCatogary AStatisticsCatogary, List<StatisticsItem> ALListStatisticsItem, List<StatisticsSetValue> AListStatisticsSetValue, int ALlogicPartMark, string AStrWeekConfig, String AStrMontConfig,List<AgentInfo> AListAgentInfo) 
        {
            try
            {              
 
                //先找小项是不是带平均的, 然后拆分统计周期, 按月查询数据并统计
               StatisticsSetValue StatisticsSetValueSpecial = AListStatisticsSetValue.Where(p => p.MappingIDOrStatisticsID == AStatisticsMapping.StatisticsMappingID && p.IsStart.Equals("Y") && (p.StatisticsItemID == UMPService08Const.Const_Item_AfDeDurMoreAvaDeDurSec
                    || p.StatisticsItemID == UMPService08Const.Const_Item_CallDurationCompareAva)).Count()==0? null:AListStatisticsSetValue.Where(p => p.MappingIDOrStatisticsID == AStatisticsMapping.StatisticsMappingID && p.IsStart.Equals("Y") && (p.StatisticsItemID == UMPService08Const.Const_Item_AfDeDurMoreAvaDeDurSec
                    || p.StatisticsItemID == UMPService08Const.Const_Item_CallDurationCompareAva)).First();
               List<DateTimeSplite> LListDateTimeSplite = new List<DateTimeSplite>();
               if (StatisticsSetValueSpecial == null)
               {
                   //全不带平均
                   GetSpliteStatisticsSplit(AStatisticsMapping, null, ref LListDateTimeSplite, AStrWeekConfig, AStrMontConfig);
               }
               else 
               {
                   StatisticsItem LStatisticsItemTemp = ALListStatisticsItem.Where(p => p.StatisticsItemID == StatisticsSetValueSpecial.StatisticsItemID).Count() > 0 ? ALListStatisticsItem.Where(p => p.StatisticsItemID == StatisticsSetValueSpecial.StatisticsItemID).First() : null;
                   if (LStatisticsItemTemp == null) { return; }
                   GetSpliteStatisticsSplit(AStatisticsMapping, LStatisticsItemTemp, ref LListDateTimeSplite, AStrWeekConfig, AStrMontConfig);
               }
                            
               if (LListDateTimeSplite.Count == 0) { return; }

               //先将座席写入临时表
               List<string> LListStrAgentName = new List<string>();
               LListStrAgentName.Add(string.Empty);
               LListStrAgentName.Add(AListAgentInfo.Count.ToString());
               foreach (AgentInfo AgentTemp in AListAgentInfo)
               {
                   string strInfo = string.Format("{0}{1}", AgentTemp.ObjName, AscCodeToChr(27));
                   LListStrAgentName.Add(strInfo);
               }

               FileLog.WriteInfo("Do_Combine_Statistics()", "InsertTempData()");
               OperationReturn operationTemp = InsertTempData(LListStrAgentName, AStrRent);
               if (!operationTemp.Result) { return; }
               string LSeriID = operationTemp.Data.ToString();

                //拼连接串
               //统计数据的List
               List<ObjInfo> LListAllObjInfo = null;
               DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
               DataOperations01 LDataOperations = new DataOperations01();
               string LStrDynamicSQL = string.Empty;
               DataTable LDataTableReturn = new DataTable();
               long ILongNumber = 0;

               //C012为录音时长,C059 为Hold次数  C060 Hold总时长 C063总静音时长 C064座席讲话时长 C065客人讲话时长 C066座席转接次数  C103座席先挂机 C108事后处理时长
               foreach (DateTimeSplite LDateTimeSingle in LListDateTimeSplite)
               {
                   LDatabaseOperationReturn = new DatabaseOperation01Return();
                   LDataOperations = new DataOperations01();
                   LStrDynamicSQL = string.Empty;
                   LDataTableReturn = new DataTable();
                   LListAllObjInfo = new List<ObjInfo>();

                   ILongNumber = 0;
                   bool LBoolContinuteFlag = true;
                   if(ALlogicPartMark==2)//不分表
                    {
                        while (LBoolContinuteFlag) 
                        {
                            try
                            {
                                switch (IIntDatabaseType)
                                {
                                    case 2:
                                        {
                                            LStrDynamicSQL = string.Format("SELECT TOP 1000 C001,C002,C004,C006,C012,C059,C060,C063,C064,C065,C066,C103,C108 FROM T_21_001_{0} WHERE C001>{2} AND  C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {1}) AND C004>='{3}' AND C004<'{4}' ORDER BY C001 ", AStrRent, LSeriID, ILongNumber, LDateTimeSingle.StartStatisticsTime, LDateTimeSingle.StopStatisticsTime);
                                        }
                                        break;
                                    case 3:
                                        {
                                            LStrDynamicSQL = String.Format("SELECT  C001,C002,C004,C006,C012,C059,C060,C063,C064,C065,C066,C103,C108 FROM T_21_001_{0} WHERE C001>{2} AND  C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {1}) AND C004>='{3}' AND C004<'{4}' AND ROWNUM <=1000   ORDER BY C001  ", AStrRent, LSeriID, ILongNumber, LDateTimeSingle.StartStatisticsTime, LDateTimeSingle.StopStatisticsTime);
                                        }
                                        break;
                                    default:
                                        break;
                                }

                                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                                if (!LDatabaseOperationReturn.BoolReturn)
                                {
                                    LBoolContinuteFlag = false;
                                    continue;
                                }
                                else
                                {
                                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                                    if (LDataTableReturn.Rows.Count < 1000) { LBoolContinuteFlag = false; }
                                    foreach (DataRow dr in LDataTableReturn.Rows)
                                    {
                                        ObjInfo objInfo = new ObjInfo();
                                        objInfo.ObjID = LongParse(dr["C001"].ToString(), 0);
                                        objInfo.ObjName = dr["C002"].ToString();
                                        objInfo.UTCTime = LongParse(dr["C006"].ToString(), 0);
                                        objInfo.LocalTime = LongParse(DateTime.Parse(dr["C004"].ToString()).ToString("yyyyMMddHHmmss"), 0);
                                        objInfo.ObjValue01 =DoubleParse( dr["C012"].ToString(),0);
                                        objInfo.ObjValue02 = DoubleParse(dr["C059"].ToString(), 0);
                                        objInfo.ObjValue03 = DoubleParse(dr["C060"].ToString(), 0);
                                        objInfo.ObjValue04 = DoubleParse(dr["C063"].ToString(), 0);
                                        objInfo.ObjValue05 = DoubleParse(dr["C064"].ToString(), 0);
                                        objInfo.ObjValue06 = DoubleParse(dr["C065"].ToString(), 0);
                                        objInfo.ObjValue07 = DoubleParse(dr["C066"].ToString(), 0);
                                        objInfo.ObjValue08 = DoubleParse(dr["C103"].ToString(), 0);
                                        objInfo.ObjValue09 = DoubleParse(dr["C108"].ToString(), 0);
                                        objInfo.ObjIsFit = true;
                                        LListAllObjInfo.Add(objInfo);
                                        ILongNumber = objInfo.ObjID;
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                LBoolContinuteFlag = false;
                                FileLog.WriteInfo("", LStrDynamicSQL + "|" + ex.Message);
                            }
                        }

                        GetCombineNotFill(LListAllObjInfo, AStatisticsMapping, AStatisticsCatogary, ALListStatisticsItem, AListStatisticsSetValue);

                        foreach (ObjInfo obj in LListAllObjInfo) 
                        {
                            if (obj.ObjIsFit == false)
                            {
                                UpdateStatisticsValue(string.Format("T_31_054_{0}", AStrRent), AStatisticsMapping.MappingColumnID.ToString(), UMPService08Const.Const_Statistics_NotFill, UMPService08Const.Const_Statistics_MarkTable_Record, obj);
                            }
                            else 
                            {

                                UpdateStatisticsValue(string.Format("T_31_054_{0}", AStrRent), AStatisticsMapping.MappingColumnID.ToString(), UMPService08Const.Const_Statistics_Fill, UMPService08Const.Const_Statistics_MarkTable_Record, obj);
                            }
                        }
                    }
                    else if (ALlogicPartMark == 1) //按月分表
                    {
                        //按月分表，再继续将时间分段。
                        List<DateTimeSplite> ListDateTimeSplite2 = new List<DateTimeSplite>();
                        GetSpliteTime(LDateTimeSingle.StartStatisticsTime, LDateTimeSingle.StopStatisticsTime, ref ListDateTimeSplite2);
                        string strPartInfo = string.Empty;
                        foreach (DateTimeSplite DateTimeSpliteTemp in ListDateTimeSplite2)
                        {
                            ILongNumber = 0;
                            LBoolContinuteFlag = true;

                            strPartInfo = string.Format("T_21_001_{0}_{1}{2}",AStrRent, DateTimeSpliteTemp.StartStatisticsTime.ToString("yy"), DateTimeSpliteTemp.StartStatisticsTime.ToString("MM"));
                            if (!IListStringTableInfo.Contains(strPartInfo)) { continue; }
                            while (LBoolContinuteFlag)
                            {
                                try
                                {
                                    switch (IIntDatabaseType)
                                    {
                                        case 2:
                                            {
                                                LStrDynamicSQL = string.Format("SELECT TOP 1000 C001,C002,C004,C006,C012,C059,C060,C063,C064,C065,C066,C103,C108 FROM {0} WHERE C001>{2} AND  C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {1}) AND C004>='{3}' AND C004<'{4}' ORDER BY C001 ", strPartInfo, LSeriID, ILongNumber, DateTimeSpliteTemp.StartStatisticsTime, DateTimeSpliteTemp.StopStatisticsTime);
                                            }
                                            break;
                                        case 3:
                                            {
                                                LStrDynamicSQL = String.Format("SELECT  C001,C002,C004,C006,C012,C059,C060,C063,C064,C065,C066,C103,C108 FROM {0} WHERE C001>{2} AND  C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {1}) AND C004>='{3}' AND C004<'{4}' AND ROWNUM <=1000   ORDER BY C001  ", strPartInfo, LSeriID, ILongNumber, DateTimeSpliteTemp.StartStatisticsTime, DateTimeSpliteTemp.StopStatisticsTime);
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                                    if (!LDatabaseOperationReturn.BoolReturn)
                                    {
                                        LBoolContinuteFlag = false;
                                        continue;
                                    }
                                    else
                                    {
                                        LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                                        if (LDataTableReturn.Rows.Count < 1000) { LBoolContinuteFlag = false; }
                                        foreach (DataRow dr in LDataTableReturn.Rows)
                                        {
                                            ObjInfo objInfo = new ObjInfo();
                                            objInfo.ObjID = LongParse(dr["C001"].ToString(), 0);
                                            objInfo.ObjName = dr["C002"].ToString();
                                            objInfo.UTCTime = LongParse(dr["C006"].ToString(), 0);
                                            objInfo.LocalTime = LongParse(DateTime.Parse(dr["C004"].ToString()).ToString("yyyyMMddHHmmss"), 0);
                                            objInfo.ObjValue01 = DoubleParse(dr["C012"].ToString(), 0);
                                            objInfo.ObjValue02 = DoubleParse(dr["C059"].ToString(), 0);
                                            objInfo.ObjValue03 = DoubleParse(dr["C060"].ToString(), 0);
                                            objInfo.ObjValue04 = DoubleParse(dr["C063"].ToString(), 0);
                                            objInfo.ObjValue05 = DoubleParse(dr["C064"].ToString(), 0);
                                            objInfo.ObjValue06 = DoubleParse(dr["C065"].ToString(), 0);
                                            objInfo.ObjValue07 = DoubleParse(dr["C066"].ToString(), 0);
                                            objInfo.ObjValue08 = DoubleParse(dr["C103"].ToString(), 0);
                                            objInfo.ObjValue09 = DoubleParse(dr["C108"].ToString(), 0);
                                            objInfo.ObjIsFit = true;
                                            LListAllObjInfo.Add(objInfo);
                                            ILongNumber = objInfo.ObjID;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LBoolContinuteFlag = false;
                                    FileLog.WriteInfo("", LStrDynamicSQL + "|" + ex.Message);
                                }
                            }
                        }
                        GetCombineNotFill(LListAllObjInfo, AStatisticsMapping, AStatisticsCatogary, ALListStatisticsItem, AListStatisticsSetValue);
                        foreach (ObjInfo obj in LListAllObjInfo)
                        {
                            string LTableAfter = string.Empty;
                            LTableAfter = obj.ObjName.Substring(0, 4);
                            if (obj.ObjIsFit == false)
                            {
                                UpdateStatisticsValue(string.Format("T_31_054_{0}_{1}", AStrRent, LTableAfter), AStatisticsMapping.MappingColumnID.ToString(), UMPService08Const.Const_Statistics_NotFill, UMPService08Const.Const_Statistics_MarkTable_Record, obj);
                            }
                            else
                            {

                                UpdateStatisticsValue(string.Format("T_31_054_{0}_{1}", AStrRent, LTableAfter), AStatisticsMapping.MappingColumnID.ToString(), UMPService08Const.Const_Statistics_Fill, UMPService08Const.Const_Statistics_MarkTable_Record, obj);
                            }
                        }

                    }
               }

            }
            catch (Exception e)
            {
                FileLog.WriteInfo("Do_RecordDurationExcept()", "Error:" + e.Message);
            }
        }

        /// <summary>
        /// 对可合并统计的统计
        /// </summary>
        /// <param name="ALListAllObjInfo"></param>
        /// <param name="LListObjInfoNotFit"></param>
        /// <param name="LListObjInfoFit"></param>
        /// <param name="AStatisticsMapping"></param>
        /// <param name="AStatisticsCatogary"></param>
        /// <param name="ALListStatisticsItem"></param>
        /// <param name="AListStatisticsSetValue"></param>
        private void GetCombineNotFill(List<ObjInfo> ALListAllObjInfo, StatisticsMapping AStatisticsMapping, StatisticsCatogary AStatisticsCatogary, List<StatisticsItem> ALListStatisticsItem, List<StatisticsSetValue> AListStatisticsSetValue) 
        {
            List<StatisticsSetValue> LListStatisticsSetValue = AListStatisticsSetValue.Where(p => p.MappingIDOrStatisticsID == AStatisticsMapping.StatisticsMappingID && p.IsStart.Equals("Y")).ToList();
            foreach(StatisticsSetValue LSingleStatisticsSetValue in LListStatisticsSetValue)
            {
                //C012为录音时长1,C059 为Hold次数2,  C060 Hold总时长3, C063总静音时长4, C064座席讲话时长5, C065客人讲话时长6, C066座席转接次数7,  C103座席先挂机8, C108事后处理时长 9，
               double LDoubleSetValue = DoubleParse(LSingleStatisticsSetValue.SetValue004, 0);
               switch(LSingleStatisticsSetValue.StatisticsItemID)
               {
                       
                   case UMPService08Const.Const_Item_CollisionDuration: //冲突时长
                       {                          
                           foreach(ObjInfo obj in ALListAllObjInfo)
                           {
                               if((obj.ObjValue04+obj.ObjValue05+obj.ObjValue06)>LDoubleSetValue)
                               {
                                   if(obj.ObjIsFit!=false)
                                   {
                                       obj.ObjIsFit = false;
                                   }
                               }
                           }
                       }
                       break;
                   case UMPService08Const.Const_Item_CollisionPercent://冲突时长百分比
                       {
                           foreach (ObjInfo obj in ALListAllObjInfo)
                           {
                               double CollisionLength=obj.ObjValue04+obj.ObjValue05+obj.ObjValue06;
                               if ((1 - CollisionLength / obj.ObjValue01) * 100 > LDoubleSetValue)
                               {
                                   if (obj.ObjIsFit != false)
                                   {
                                       obj.ObjIsFit = false;
                                   }
                               }
                           }
                       }
                       break;
                   case UMPService08Const.Const_Item_HoldDuration: //Hold时长
                       {
                           foreach (ObjInfo obj in ALListAllObjInfo)
                           {
                               if(obj.ObjValue03>LDoubleSetValue)
                               {
                                   if (obj.ObjIsFit != false)
                                   {
                                       obj.ObjIsFit = false;
                                   }
                               }
                           }
                       }
                       break;
                   case UMPService08Const.Const_Item_HoldPercent://Hold时长占百分比
                       {
                           foreach (ObjInfo obj in ALListAllObjInfo)
                           {
                               if(obj.ObjValue03/obj.ObjValue01*100 > LDoubleSetValue)
                               {
                                   if (obj.ObjIsFit != false)
                                   {
                                       obj.ObjIsFit = false;
                                   }
                               }
                           }
                       }
                       break;
                   case UMPService08Const.Const_Item_HoldTimes://Hold次数
                       {
                           foreach (ObjInfo obj in ALListAllObjInfo)
                           {
                               if(obj.ObjValue02>LDoubleSetValue)
                               {
                                   if (obj.ObjIsFit != false)
                                   {
                                       obj.ObjIsFit = false;
                                   }
                               }
                           }
                       }
                       break;
                   case UMPService08Const.Const_Item_TransferTimes://转接次数
                       {
                           foreach (ObjInfo obj in ALListAllObjInfo)
                           {
                               if (obj.ObjValue07 > LDoubleSetValue)
                               {
                                   if (obj.ObjIsFit != false)
                                   {
                                       obj.ObjIsFit = false;
                                   }
                               }
                           }
                       }
                       break;
                   case UMPService08Const.Const_Item_IsAgentHanged://是否座席先挂机
                       {
                           foreach (ObjInfo obj in ALListAllObjInfo)
                           {
                               if (obj.ObjValue08.Equals("Y"))
                               {
                                   if (obj.ObjIsFit != false)
                                   {
                                       obj.ObjIsFit = false;
                                   }
                               }
                           }
                       }
                       break;
                   case UMPService08Const.Const_Item_AfterDealDurationSec://事后处理时长
                       {
                           foreach (ObjInfo obj in ALListAllObjInfo)
                           {
                               if (obj.ObjValue09> LDoubleSetValue)
                               {
                                   if (obj.ObjIsFit != false)
                                   {
                                       obj.ObjIsFit = false;
                                   }
                               }
                           }
                       }
                       break;
                   case UMPService08Const.Const_Item_AfDeDurMoreAvaDeDurSec://事后处理时长大于平均处理时长多少(s)
                       {
                           double AVG = 0; double SUM = 0;
                           SUM = ALListAllObjInfo.Sum(p => p.ObjValue09);
                           if (ALListAllObjInfo.Count == 0)
                           {
                               continue;
                           }
                           AVG = SUM / ALListAllObjInfo.Count;
                           foreach (ObjInfo obj in ALListAllObjInfo)
                           {
                               if ((1 - obj.ObjValue09 / AVG) * 100 > LDoubleSetValue)
                               {
                                   if (obj.ObjIsFit != false)
                                   {
                                       obj.ObjIsFit = false;
                                   }
                               }
                           }
                       }
                       break;
                   case UMPService08Const.Const_Item_CallDurationCompareAva://通话时长比较平均值（绝对值）
                       {
                           double AVG = 0; double SUM = 0;
                           SUM = ALListAllObjInfo.Sum(p => p.ObjValue01);
                           if (ALListAllObjInfo.Count==0)
                           {
                               continue;
                           }
                           AVG = SUM / ALListAllObjInfo.Count;
                           foreach (ObjInfo obj in ALListAllObjInfo)
                           {
                               if (Math.Abs( obj.ObjValue01 - AVG)> LDoubleSetValue)
                               {
                                   if (obj.ObjIsFit != false)
                                   {
                                       obj.ObjIsFit = false;
                                   }
                               }
                           }
                       }
                       break;
                   case UMPService08Const.Const_Item_CallDurationComparePec://通话时长比较平均值（百分比）
                       {
                           double AVG = 0; double SUM = 0;
                           SUM = ALListAllObjInfo.Sum(p => p.ObjValue01);
                           if (ALListAllObjInfo.Count == 0)
                           {
                               continue;
                           }
                           AVG = SUM / ALListAllObjInfo.Count;
                           foreach (ObjInfo obj in ALListAllObjInfo)
                           {
                               if ((1 - obj.ObjValue01 / AVG) * 100 > LDoubleSetValue)
                               {
                                   if (obj.ObjIsFit != false)
                                   {
                                       obj.ObjIsFit = false;
                                   }
                               }
                           }
                       }
                       break;
                   default:
                       break;
               }
            }
        }





        //事后处理时长异常(标准差)
        private void Do_AfterDealDurationExcept(string AStrRent, StatisticsMapping AStatisticsMapping, StatisticsCatogary AStatisticsCatogary, List<StatisticsItem> ALListStatisticsItem, List<StatisticsSetValue> AListStatisticsSetValue, int ALlogicPartMark, string AStrWeekConfig, String AStrMontConfig)
        {

        }
        //异常分数(标准差)
        private void Do_ExceptionScore()
        {
        }

        //重复呼入(主叫号C040)
        private void Do_RepeatedCallin(string AStrRent, StatisticsMapping AStatisticsMapping, StatisticsCatogary AStatisticsCatogary, List<StatisticsItem> ALListStatisticsItem, List<StatisticsSetValue> AListStatisticsSetValue, int ALlogicPartMark, string AStrWeekConfig, String AStrMontConfig)
        {
            FileLog.WriteInfo("Do_RepeatedCallin","start");
            try
            {
                StatisticsItem LStatisticsItemTemp = ALListStatisticsItem.Where(p => p.StatisticsID == AStatisticsCatogary.StatisticsID).Count() == 0 ? null : ALListStatisticsItem.Where(p => p.StatisticsID == AStatisticsCatogary.StatisticsID).First();
                if (LStatisticsItemTemp == null) { return; }
                StatisticsSetValue LStatisticsSetValue = AListStatisticsSetValue.Where(p => p.MappingIDOrStatisticsID == AStatisticsMapping.StatisticsMappingID && p.StatisticsItemID == LStatisticsItemTemp.StatisticsItemID && p.IsStart.Equals("Y")).Count() == 0 ? null : AListStatisticsSetValue.Where(p => p.MappingIDOrStatisticsID == AStatisticsMapping.StatisticsMappingID && p.StatisticsItemID == LStatisticsItemTemp.StatisticsItemID).First();
                if (LStatisticsSetValue == null) { return; }
                
                string LStrIsStart = LStatisticsSetValue.IsStart;
                string LStrSetValue004 = LStatisticsSetValue.SetValue004;
                string [] Value004= LStrSetValue004.Split(';');
                int PreHour= IntParse(Value004[0],0);
                int StatisNumber= IntParse(Value004[1],0);

                DateTime LDateTimeNow = DateTime.Now.Date;
                DateTime LDateTimeStart = LDateTimeNow;
                switch (AStatisticsMapping.UpdateTimeUnit)//1年，2月，3周，4天
                {
                    case 1:
                        {
                            LDateTimeStart = LDateTimeNow.AddYears(AStatisticsMapping.UpdateValueTime*(-1));
                        }
                        break;
                    case 2://月 
                        {
                            LDateTimeStart = LDateTimeNow.AddMonths(AStatisticsMapping.UpdateValueTime * (-1));
                        }
                        break;
                    case 3://周
                        {
                            LDateTimeStart = LDateTimeNow.AddDays(AStatisticsMapping.UpdateValueTime * 7 * (-1));
                        }
                        break;
                    case 4://更新天数据
                        {
                            LDateTimeStart = LDateTimeNow.AddDays(AStatisticsMapping.UpdateValueTime * (-1));
                        }
                        break;
                    default:
                        break;
                }
            
                DateTime StartStatisticsTime = LDateTimeStart.Date;
                DateTime StopStatisticsTime = LDateTimeNow.Date;

                //统计数据的List
                List<ObjInfo> LListAllObjInfo = null;
                List<ObjInfo> LListObjInfoFit = null;
                List<ObjInfo> LListObjInfoNotFit = null;
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
                string LStrDynamicSQL = string.Empty;
                DataTable LDataTableReturn = new DataTable();
                long ILongNumber = 0;
                #region
                //每次统计时按天开始时间往前走
                while (StartStatisticsTime < StopStatisticsTime)
                {
                    LDatabaseOperationReturn = new DatabaseOperation01Return();
                    LDataOperations = new DataOperations01();
                    LStrDynamicSQL = string.Empty;
                    LDataTableReturn = new DataTable();
                    LListAllObjInfo = new List<ObjInfo>();
                    LListObjInfoFit = new List<ObjInfo>();
                    LListObjInfoNotFit = new List<ObjInfo>(); 

                    bool LBoolContinuteFlag = true;
                    if (ALlogicPartMark == 2)//不分表
                    {
                        while (LBoolContinuteFlag)
                        {
                            try
                            {
                                switch (IIntDatabaseType)
                                {
                                    case 2:
                                        {
                                            LStrDynamicSQL = string.Format("SELECT TOP 1000 C001,C002,C004,C005,C006,{2} FROM T_21_001_{0} WHERE C001>{1}  AND C004>='{3}' AND C004<'{4}' ORDER BY C001 ", AStrRent, ILongNumber, "C040", StartStatisticsTime, StartStatisticsTime.AddDays(1));
                                        }
                                        break;
                                    case 3:
                                        {
                                            LStrDynamicSQL = String.Format("SELECT C001,C002,C004,C005,C006,{2} FROM T_21_001_{0} WHERE C001>{1} AND C004>='{3}' AND C004<'{4}' AND ROWNUM <=1000   ORDER BY C001  ", AStrRent, ILongNumber, "C040", StartStatisticsTime, StartStatisticsTime.AddDays(1));
                                        }
                                        break;
                                    default:
                                        break;
                                }

                                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                                if (!LDatabaseOperationReturn.BoolReturn)
                                {
                                    LBoolContinuteFlag = false;
                                    continue;
                                }
                                else
                                {
                                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                                    if (LDataTableReturn.Rows.Count < 1000) { LBoolContinuteFlag = false; }
                                    foreach (DataRow dr in LDataTableReturn.Rows)
                                    {
                                        ObjInfo objInfo = new ObjInfo();
                                        objInfo.ObjID = LongParse(dr["C001"].ToString(), 0);
                                        objInfo.ObjName = dr["C002"].ToString();
                                        objInfo.UTCTime = LongParse(dr["C006"].ToString(), 0);
                                        objInfo.LocalTime = LongParse(DateTime.Parse(dr["C004"].ToString()).ToString("yyyyMMddHHmmss"),0);
                                        objInfo.ObjValue00 = DoubleParse(dr["C040"].ToString(), 0);
                                        LListAllObjInfo.Add(objInfo);
                                        ILongNumber = objInfo.ObjID;
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                LBoolContinuteFlag = false;
                                FileLog.WriteInfo("", LStrDynamicSQL + "|" + ex.Message);
                            }
                            GetNotFillRepeatedCallin(LListAllObjInfo, ref LListObjInfoNotFit, ref LListObjInfoFit, PreHour, StatisNumber, ALlogicPartMark, AStrRent);

                            //更新时间段数据未处理
                            foreach (ObjInfo obj in LListObjInfoNotFit)
                            {
                                UpdateStatisticsValue(string.Format("T_31_054_{0}", AStrRent), AStatisticsMapping.MappingColumnID.ToString(), UMPService08Const.Const_Statistics_NotFill,UMPService08Const.Const_Statistics_MarkTable_Record, obj);
                            }

                            foreach (ObjInfo obj in LListObjInfoFit)
                            {
                                UpdateStatisticsValue(string.Format("T_31_054_{0}", AStrRent), AStatisticsMapping.MappingColumnID.ToString(), UMPService08Const.Const_Statistics_Fill, UMPService08Const.Const_Statistics_MarkTable_Record, obj);
                            }


                        }//while LBoolContinuteFlag
                    }
                    else if (ALlogicPartMark==1)
                    {
                         //按月分表，再继续将时间分段。
                        List<DateTimeSplite> ListDateTimeSplite2 = new List<DateTimeSplite>();
                        GetSpliteTime(StartStatisticsTime,StopStatisticsTime.AddDays(1), ref ListDateTimeSplite2);
                        string strPartInfo = string.Empty;
                        foreach (DateTimeSplite DateTimeSpliteTemp in ListDateTimeSplite2)
                        {
                            ILongNumber = 0;
                            LBoolContinuteFlag = true;
                            strPartInfo = string.Format("T_21_001_{0}_{1}{2}", AStrRent, DateTimeSpliteTemp.StartStatisticsTime.ToString("yy"), DateTimeSpliteTemp.StartStatisticsTime.ToString("MM"));
                            if (!IListStringTableInfo.Contains(strPartInfo)) { continue; }
                            //存的表要判断了.是那张表的要将表给记下来，用于写数据
                            while (LBoolContinuteFlag)
                            {
                                try
                                {
                                    switch (IIntDatabaseType)
                                    {
                                        case 2:
                                            {
                                                LStrDynamicSQL = string.Format("SELECT TOP 1000 C001,C002,C004,C005,C006,{2} FROM T_21_001_{0} WHERE C001>{1}  AND C004>='{3}' AND C004<'{4}' ORDER BY C001 ", AStrRent, ILongNumber, "C040", StartStatisticsTime, StartStatisticsTime.AddDays(1));
                                            }
                                            break;
                                        case 3:
                                            {
                                                LStrDynamicSQL = String.Format("SELECT C001,C002,C004,C005,C006,{2} FROM T_21_001_{0} WHERE C001>{1} AND C004>='{3}' AND C004<'{4}' AND ROWNUM <=1000   ORDER BY C001  ", AStrRent, ILongNumber, "C040", StartStatisticsTime, StartStatisticsTime.AddDays(1));
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                                    if (!LDatabaseOperationReturn.BoolReturn)
                                    {
                                        LBoolContinuteFlag = false;
                                        continue;
                                    }
                                    else
                                    {
                                        LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                                        if (LDataTableReturn.Rows.Count < 1000) { LBoolContinuteFlag = false; }
                                        foreach (DataRow dr in LDataTableReturn.Rows)
                                        {
                                            ObjInfo objInfo = new ObjInfo();
                                            objInfo.ObjID = LongParse(dr["C001"].ToString(), 0);
                                            objInfo.ObjName = dr["C002"].ToString();
                                            objInfo.UTCTime = LongParse(dr["C006"].ToString(), 0);
                                            objInfo.LocalTime = LongParse(DateTime.Parse(dr["C004"].ToString()).ToString("yyyyMMddHHmmss"), 0);
                                            objInfo.ObjValue00 = DoubleParse(dr["C040"].ToString(), 0);
                                            LListAllObjInfo.Add(objInfo);
                                            ILongNumber = objInfo.ObjID;
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    LBoolContinuteFlag = false;
                                    FileLog.WriteInfo("", LStrDynamicSQL + "|" + ex.Message);
                                }

                            }//while LBoolContinuteFlag

                        }
                        GetNotFillRepeatedCallin(LListAllObjInfo, ref LListObjInfoNotFit, ref LListObjInfoFit, PreHour, StatisNumber, ALlogicPartMark, AStrRent);
                        //更新时间段数据
                        string LTableAfter = string.Empty;
                        foreach (ObjInfo obj in LListObjInfoNotFit)
                        {
                            LTableAfter = obj.ObjName.Substring(0, 4);
                            UpdateStatisticsValue(string.Format("T_31_054_{0}_{1}", AStrRent, LTableAfter), AStatisticsMapping.MappingColumnID.ToString(), UMPService08Const.Const_Statistics_NotFill,UMPService08Const.Const_Statistics_MarkTable_Record, obj);
                        }

                        foreach (ObjInfo obj in LListObjInfoFit)
                        {
                            LTableAfter = obj.ObjName.Substring(0, 4);
                            UpdateStatisticsValue(string.Format("T_31_054_{0}_{1}", AStrRent, LTableAfter), AStatisticsMapping.MappingColumnID.ToString(), UMPService08Const.Const_Statistics_Fill,UMPService08Const.Const_Statistics_MarkTable_Record, obj);
                        }
                    }

                    StartStatisticsTime=StartStatisticsTime.AddDays(1);
                }
                #endregion

            }
            catch (Exception e)
            {       
                 FileLog.WriteInfo("Do_RepeatedCallin()", "Error:"+e.Message);
            }
        }

        /// <summary>
        /// 得到是否要打标
        /// </summary>
        private void GetNotFillRepeatedCallin(List<ObjInfo> AListAllObjInfo, ref List<ObjInfo> AListObjInfoNotFit, ref List<ObjInfo> AListObjInfoFit, int PreHours, int ACallNumberValue, int ALlogicPartMark, string AStrRent)
        {

            DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
            DataOperations01 LDataOperations = new DataOperations01();
            string LStrDynamicSQL = string.Empty;
            DataTable LDataTableReturn = new DataTable();
            int CountNumber = 0;

            foreach(ObjInfo SigleObjetInfo in AListAllObjInfo)
            {
                DateTime StartTime = StringToDateTime(SigleObjetInfo.LocalTime.ToString());

                DateTime TrueStartTime = StartTime.AddHours(PreHours * (-1));
                DateTime TrueStopTime = StartTime.AddHours(PreHours*1);

                LDatabaseOperationReturn = new DatabaseOperation01Return();
                LDataOperations = new DataOperations01();
                LStrDynamicSQL = string.Empty;
                LDataTableReturn = new DataTable();
                CountNumber = 0;
                try
                {
                    if(ALlogicPartMark==2)
                    {

                        try
                        {
                            switch (IIntDatabaseType)
                            {
                                case 2:
                                    {
                                        LStrDynamicSQL = string.Format("SELECT Count(C001) AS NumCount FROM T_21_001_{0} WHERE  C004>='{1}' AND C004<'{2}'  ", AStrRent, TrueStartTime, TrueStopTime);
                                    }
                                    break;
                                case 3:
                                    {
                                        LStrDynamicSQL = String.Format("SELECT Count(C001) AS NumCount FROM T_21_001_{0} WHERE  C004>='{1}' AND C004<'{2}'  ", AStrRent, TrueStartTime, TrueStopTime);
                                    }
                                    break;
                                default:
                                    break;
                            }

                            LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                            if (!LDatabaseOperationReturn.BoolReturn)
                            {                               
                                continue;
                            }
                            else
                            {
                                LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];                              
                                foreach (DataRow dr in LDataTableReturn.Rows)
                                {
                                    CountNumber += IntParse(dr["NumCount"].ToString(), 0);
                                }

                                if (CountNumber >= ACallNumberValue)
                                {
                                    AListObjInfoFit.Add(SigleObjetInfo);
                                }
                                else 
                                {
                                    AListObjInfoNotFit.Add(SigleObjetInfo);
                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            FileLog.WriteInfo("", LStrDynamicSQL + "|" + ex.Message);
                        }

                    }
                    else if (ALlogicPartMark==1) 
                    {
                         List<DateTimeSplite> ListDateTimeSplite2 = new List<DateTimeSplite>();
                         GetSpliteTime(TrueStartTime, TrueStopTime, ref ListDateTimeSplite2);
                         string strPartInfo = string.Empty;
                         foreach (DateTimeSplite DateTimeSpliteTemp in ListDateTimeSplite2)
                         {
                             strPartInfo = string.Format("T_21_001_{0}_{1}{2}", AStrRent, DateTimeSpliteTemp.StartStatisticsTime.ToString("yy"), DateTimeSpliteTemp.StartStatisticsTime.ToString("MM"));
                             if (!IListStringTableInfo.Contains(strPartInfo)) { continue; }

                             try
                             {
                                 switch (IIntDatabaseType)
                                 {
                                     case 2:
                                         {
                                             LStrDynamicSQL = string.Format("SELECT Count(C001) AS NumCount FROM {0} WHERE  C004>='{1}' AND C004<'{2}'  ", strPartInfo,  DateTimeSpliteTemp.StartStatisticsTime, DateTimeSpliteTemp.StopStatisticsTime);
                                         }
                                         break;
                                     case 3:
                                         {
                                             LStrDynamicSQL = String.Format("SELECT Count(C001) AS NumCount FROM {0} WHERE  C004>='{1}' AND C004<'{2}' ", strPartInfo,  DateTimeSpliteTemp.StartStatisticsTime, DateTimeSpliteTemp.StopStatisticsTime);
                                         }
                                         break;
                                     default:
                                         break;
                                 }

                                 LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                                 if (!LDatabaseOperationReturn.BoolReturn)
                                 {
                                     continue;
                                 }
                                 else
                                 {
                                     LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                                     foreach (DataRow dr in LDataTableReturn.Rows)
                                     {
                                         CountNumber += IntParse(dr["NumCount"].ToString(), 0);
                                     }

                                     if (CountNumber >= ACallNumberValue)
                                     {
                                         AListObjInfoFit.Add(SigleObjetInfo);
                                     }
                                     else
                                     {
                                         AListObjInfoNotFit.Add(SigleObjetInfo);
                                     }

                                 }
                             }
                             catch (Exception ex)
                             {
                                 FileLog.WriteInfo("", LStrDynamicSQL + "|" + ex.Message);
                             }
                         }
                    }

                }
                catch (Exception e)
                {
                    FileLog.WriteInfo("GetNotFillRepeatedCallin",e.Message.ToString());
                }
            
            }

        }



        #region
        //呼叫高峰期
        private void Do_CallPeak(string AStrRent, StatisticsMapping AStatisticsMapping, StatisticsCatogary AStatisticsCatogary, List<StatisticsItem> ALListStatisticsItem, List<StatisticsSetValue> AListStatisticsSetValue, int ALlogicPartMark, string AStrWeekConfig, String AStrMontConfig) 
        {
             FileLog.WriteInfo("Do_CallPeak", "start");
             try
             {
                 StatisticsItem LStatisticsItemTemp = ALListStatisticsItem.Where(p => p.StatisticsID == AStatisticsCatogary.StatisticsID).Count() == 0 ? null : ALListStatisticsItem.Where(p => p.StatisticsID == AStatisticsCatogary.StatisticsID).First();
                 if (LStatisticsItemTemp == null) { return; }
                 StatisticsSetValue LStatisticsSetValue = AListStatisticsSetValue.Where(p => p.MappingIDOrStatisticsID == AStatisticsMapping.StatisticsMappingID && p.StatisticsItemID == LStatisticsItemTemp.StatisticsItemID && p.IsStart.Equals("Y")).Count() == 0 ? null : AListStatisticsSetValue.Where(p => p.MappingIDOrStatisticsID == AStatisticsMapping.StatisticsMappingID && p.StatisticsItemID == LStatisticsItemTemp.StatisticsItemID).First();
                 if (LStatisticsSetValue == null) { return; }
                 List<DateTimeSplite> LListDateTimeSplite = new List<DateTimeSplite>();
                 GetSpliteStatisticsSplit(AStatisticsMapping, LStatisticsItemTemp, ref LListDateTimeSplite, AStrWeekConfig, AStrMontConfig);
                if (LListDateTimeSplite.Count == 0) { return; }

                 //妆一天分成多少片
                string LStrSetValue004 = LStatisticsSetValue.SetValue004;
                string[] Value004 = LStrSetValue004.Split(';');
                int LIntSTD = IntParse(Value004[0], 0);
                int LSpliteNumber = IntParse(Value004[1], 0);
                int LSpliteType = LSpliteNumber;
                switch (LSpliteNumber)
                {
                    case 0: //5分钟
                        LSpliteNumber = LSpliteNumber * 12 * 24;
                        break;
                    case 1://15分钟
                        LSpliteNumber = LSpliteNumber * 4 * 24;
                        break;
                    case 2://半小时
                        LSpliteNumber = LSpliteNumber * 2 * 24;
                        break;
                    case 3://1小时
                        LSpliteNumber = LSpliteNumber * 1* 24;
                        break;
                    case 4: //2小时
                        LSpliteNumber = LSpliteNumber * 12;
                        break;
                    default:
                        break;
                }
             

                // 1平行 2 钻取 
                //1应用到所有 2为不
                List<AgentInfo> LListAgentInfoTemp = new List<AgentInfo>();
                List<OrgInfo> LListOrgInfo = new List<OrgInfo>();
                if (AStatisticsMapping.DropDown == 1) //平行时可能有技能组
                {
                    if(AStatisticsMapping.ApplayAll==2)//不应用所有机构
                    {
                        GetAgentInfoInOrg(ref LListAgentInfoTemp, AStatisticsMapping.OrgIDOrSkillID, 1, AStrRent);

                        //将座席信息写入临时表
                        Do_CallPeak_Statistics(LListAgentInfoTemp, LListDateTimeSplite, AStrRent, ALlogicPartMark, AStatisticsMapping.MappingColumnID, LIntSTD, UMPService08Const.Const_Statistics_MarkTable_Record, LSpliteNumber, LStatisticsItemTemp, LSpliteType);
                    }
                    else if (AStatisticsMapping.ApplayAll == 1)//应用到所有机构
                    {
                        GetOrgInfoSon(ref  LListOrgInfo, AStatisticsMapping.OrgIDOrSkillID);
                        foreach (OrgInfo LOrgInfoTemp in LListOrgInfo)
                        {
                            LListAgentInfoTemp = new List<AgentInfo>();
                            GetAgentInfoInOrg(ref LListAgentInfoTemp, AStatisticsMapping.OrgIDOrSkillID, 1, AStrRent);
                            //将座席信息写入临时表
                            Do_CallPeak_Statistics(LListAgentInfoTemp, LListDateTimeSplite, AStrRent, ALlogicPartMark, AStatisticsMapping.MappingColumnID, LIntSTD, UMPService08Const.Const_Statistics_MarkTable_Record, LSpliteNumber, LStatisticsItemTemp, LSpliteType);
                        }
                    }
                }
                else if (AStatisticsMapping.DropDown == 2)//只能是机构
                {
                    GetAgentInfoInOrg(ref LListAgentInfoTemp, AStatisticsMapping.OrgIDOrSkillID, 2, AStrRent);
                    //将座席信息写入临时表
                    Do_CallPeak_Statistics(LListAgentInfoTemp, LListDateTimeSplite, AStrRent, ALlogicPartMark,AStatisticsMapping.MappingColumnID, LIntSTD, UMPService08Const.Const_Statistics_MarkTable_Record, LSpliteNumber, LStatisticsItemTemp, LSpliteType);
                }
             }
             catch (Exception e)
             {
                 FileLog.WriteInfo("Do_CallPeak()", "Error:" + e.Message);
             }
        }

       /// <summary>
        /// 
        //实际的统计
       /// </summary>
       /// <param name="AListAgentInfo"></param>
       /// <param name="AListDateTimeSplite"></param>
       /// <param name="AStrRent"></param>
       /// <param name="ALlogicPartMark">是否有逻辑分区</param>
       /// <param name="AMappingColumnID">写入表的那一列</param>
       /// <param name="ALIntAVGRate"> 高于平均数的比率</param>
       /// <param name="AMarkTable">标志写入那表</param>
       /// <param name="ASpliteNumber">统计周期被时间片分成多少份</param>
       /// <param name="ALStatisticsItemTemp"></param>
       /// <param name="ASpliteType">切片类型</param>
        private void Do_CallPeak_Statistics(List<AgentInfo> AListAgentInfo, List<DateTimeSplite> AListDateTimeSplite, string AStrRent, int ALlogicPartMark,  int AMappingColumnID, int ALIntAVGRate, int AMarkTable, int ASpliteNumber, StatisticsItem ALStatisticsItemTemp, int ASpliteType) 
        {
            //先将座席写入临时表
            List<string> LListStrAgentName = new List<string>();
            LListStrAgentName.Add(string.Empty);
            LListStrAgentName.Add(AListAgentInfo.Count.ToString());
            foreach (AgentInfo AgentTemp in AListAgentInfo)
            {
                string strInfo = string.Format("{0}{1}", AgentTemp.ObjID, AscCodeToChr(27)); 
                //此处不一样，以前存的是AgentName,此处存agentID
                LListStrAgentName.Add(strInfo);
            }

            FileLog.WriteInfo("Do_Common_Statistics()", "InsertTempData()");
            OperationReturn operationTemp = InsertTempData(LListStrAgentName, AStrRent);
            if (!operationTemp.Result) { return; }
            string LSeriID = operationTemp.Data.ToString();


           
            List<CallPeakSpliteObject> LListCallPeakSpliteObject = new List<CallPeakSpliteObject>();
            foreach (DateTimeSplite LDateTimeSingle in AListDateTimeSplite)
            {
                #region //周期共分多少片
                int month = LDateTimeSingle.StartStatisticsTime.Month;
                int year = LDateTimeSingle.StartStatisticsTime.Year;
                int maxday = 0;

                switch (ALStatisticsItemTemp.SliceTimeUnit)
                {
                    case 1: //1年
                        {
                            if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0))
                            {
                                ASpliteNumber = ASpliteNumber * 365;
                            }
                            else
                            {
                                ASpliteNumber = ASpliteNumber * 366;
                            }
                        }
                        break;
                    case 2: //月
                        {
                            switch (month)
                            {
                                case 2:
                                    {
                                        if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0))
                                        {
                                            maxday = 29;
                                        }
                                        else
                                            maxday = 28;
                                    }
                                    break;
                                case 1:
                                case 3:
                                case 5:
                                case 7:
                                case 8:
                                case 10:
                                case 12:
                                    maxday = 31;
                                    break;
                                case 4:
                                case 6:
                                case 9:
                                case 11:
                                    maxday = 30;
                                    break;

                            }
                            ASpliteNumber = ASpliteNumber * maxday;
                        }
                        break;
                    case 3: //周
                        ASpliteNumber = ASpliteNumber * 7;
                        break;
                    case 4://天
                        break;
                }
                #endregion;

                //先统计T-21_101表总数，再除以片段数
                #region               

                /// 租户ID（默认为‘00000’）
                 string @Param01 = string.Empty;

                  ///  座席或分机
                 string @Param02 = string.Empty;   
              
                ///  T_00_901中的C001根据这个来查询座席或分机的记录  
                 string @Param03 = string.Empty;

                 ///  开始时间
                 string @Param04 = string.Empty;

                ///   结束时间
                 string @Param05 = string.Empty;

                ///   时间类型 L U
                 string @Param06 = string.Empty;

                  ///   统计列的字符串c001,c002,c003....
                 string @Param07 = string.Empty;


                 @Param01 = AStrRent;
                 @Param02= "3";
                 @Param03 = LSeriID.ToString();
                 @Param04 = LDateTimeSingle.StartStatisticsTime.Date.ToString("yyyyMMdd");
                 @Param05 = LDateTimeSingle.StartStatisticsTime.Date.ToString("yyyyMMdd");
                 @Param06 = "L";
                
                 while (LDateTimeSingle.StartStatisticsTime.AddDays(1) <= LDateTimeSingle.StopStatisticsTime) 
                 {
                     
                     switch (ASpliteType)
                     {
                         case 0: //5分钟
                             {
                                 for (int i = 1; i <= 288; i++) 
                                 {
                                     GetColumnNameOfCallPeak(ref @Param07, 0, i);
                                     GetCallPeakValue(ref LListCallPeakSpliteObject, @Param01, @Param02, @Param03, @Param04, @Param05, @Param06, @Param07, AStrRent, ASpliteType, i, LDateTimeSingle.StartStatisticsTime);
                                 }
                             }
                             break;
                         case 1://15分钟
                             {
                                 for (int i = 1; i <= 96; i++) 
                                 {
                                     GetColumnNameOfCallPeak(ref @Param07, 1, i);
                                     GetCallPeakValue(ref LListCallPeakSpliteObject, @Param01, @Param02, @Param03, @Param04, @Param05, @Param06, @Param07, AStrRent, ASpliteType, i, LDateTimeSingle.StartStatisticsTime);
                                 }
                             }
                             break;
                         case 2://半小时
                             {
                                 for (int i = 1; i <= 48; i++) 
                                 {
                                     GetColumnNameOfCallPeak(ref @Param07, 2, i);
                                     GetCallPeakValue(ref LListCallPeakSpliteObject, @Param01, @Param02, @Param03, @Param04, @Param05, @Param06, @Param07, AStrRent, ASpliteType, i, LDateTimeSingle.StartStatisticsTime);
                                 }
                             }
                             break;
                         case 3://1小时
                             {
                                 for (int i = 1; i <= 24; i++) 
                                 {
                                     GetColumnNameOfCallPeak(ref @Param07, 3, i);
                                     GetCallPeakValue(ref LListCallPeakSpliteObject, @Param01, @Param02, @Param03, @Param04, @Param05, @Param06, @Param07, AStrRent, ASpliteType, i, LDateTimeSingle.StartStatisticsTime);
                                 }
                             }
                             break;
                         case 4: //2小时
                             {
                                 for (int i = 1; i <= 12; i++) 
                                 {
                                     GetColumnNameOfCallPeak(ref @Param07, 4, i);
                                     GetCallPeakValue(ref LListCallPeakSpliteObject, @Param01, @Param02, @Param03, @Param04, @Param05, @Param06, @Param07, AStrRent, ASpliteType, i, LDateTimeSingle.StartStatisticsTime);
                                 }
                             }
                             break;
                         default:
                             break;
                     }
                     LDateTimeSingle.StartStatisticsTime = LDateTimeSingle.StartStatisticsTime.AddDays(1);
                 }

                #endregion



                //再查询录音表T_21_001表查出来如果在那个片段内则当算高峰期录音
                #region
                 double AVGValue = GetAVGValue(LListCallPeakSpliteObject, ASpliteNumber);
                 foreach (CallPeakSpliteObject o in LListCallPeakSpliteObject)
                {
                    bool IsCallPeak = false;
                    IsCallPeak = CompleteMethod((double)o.StatistcsValue, AVGValue, ALIntAVGRate);


                    //先将座席号写入临时表
                    //先将座席写入临时表
                    LListStrAgentName = new List<string>();
                    LListStrAgentName.Add(string.Empty);
                    LListStrAgentName.Add(AListAgentInfo.Count.ToString());
                    foreach (AgentInfo AgentTemp in AListAgentInfo)
                    {
                        string strInfo = string.Format("{0}{1}", AgentTemp.ObjName, AscCodeToChr(27));
                        LListStrAgentName.Add(strInfo);
                    }

                    FileLog.WriteInfo("Do_Common_Statistics()", "InsertTempData()");
                    operationTemp = InsertTempData(LListStrAgentName, AStrRent);
                    if (!operationTemp.Result) { return; }
                    LSeriID = operationTemp.Data.ToString();


                    //统计数据的List
                    List<ObjInfo> LListObjInfoFit = null;
                    List<ObjInfo> LListObjInfoNotFit = null;
                     //如果为true 则为高峰期录音
                    if (IsCallPeak)
                    {
                        LListObjInfoFit = new List<ObjInfo>();

                        //日期还原真实时间
                        DateTime LStartTime=DateTime.Parse("2100-1-1 00:00:00");
                        DateTime LStopTime = LStartTime;
                        ReplaceDatetime(o, ref  LStartTime, ref  LStopTime);
                        GetRecordObject(LStartTime, LStopTime, AStrRent, ALlogicPartMark, LSeriID, UMPService08Const.Const_Statistics_Fill, AMappingColumnID);
                    }
                    else 
                    {
                        LListObjInfoNotFit = new List<ObjInfo>();
                        LListCallPeakSpliteObject = new List<CallPeakSpliteObject>();

                        //日期还原真实时间
                        DateTime LStartTime = DateTime.Parse("2100-1-1 00:00:00");
                        DateTime LStopTime = LStartTime;
                        ReplaceDatetime(o, ref  LStartTime, ref  LStopTime);
                        GetRecordObject(LStartTime, LStopTime, AStrRent, ALlogicPartMark, LSeriID, UMPService08Const.Const_Statistics_NotFill, AMappingColumnID);
                    }
                }


                #endregion


            }
        }
        

        
        /// <summary>
        /// ///根据时间段查录音
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="StopTime"></param>
        /// <param name="AStrRent"></param>
        /// <param name="ALlogicPartMark">是否有逻辑分区</param>
        public void GetRecordObject(DateTime StartTime, DateTime StopTime, string AStrRent, int ALlogicPartMark, string ALSeriID, string IsFit, int AMappingColumnID) 
        {
            DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
            DataOperations01 LDataOperations = new DataOperations01();
            string LStrDynamicSQL = string.Empty;
            DataTable LDataTableReturn = new DataTable();
            long ILongNumber = 0;
            List<ObjInfo> LListAllObjInfo = new List<ObjInfo>();
            bool LBoolContinuteFlag = true;
            if (ALlogicPartMark == 2)// 录音表不按月表
            {
                while (LBoolContinuteFlag)
                {
                    try
                    {
                        switch (IIntDatabaseType)
                        {
                            case 2:
                                {
                                    LStrDynamicSQL = string.Format("SELECT TOP 1000 C001,C002,C004,C005,C006 FROM T_21_001_{0} WHERE C001>{2} AND  C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {1}) AND C004>='{3}' AND C004<'{4}' ORDER BY C001 ", AStrRent, ALSeriID, ILongNumber, StartTime, StopTime);
                                }
                                break;
                            case 3:
                                {
                                    LStrDynamicSQL = String.Format("SELECT C001,C002,C004,C005,C006  FROM T_21_001_{0} WHERE C001>{2} AND  C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {1}) AND C004>='{3}' AND C004<'{4}' AND ROWNUM <=1000   ORDER BY C001  ", AStrRent, ALSeriID, ILongNumber, StartTime, StopTime);
                                }
                                break;
                            default:
                                break;
                        }

                        LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                        if (!LDatabaseOperationReturn.BoolReturn)
                        {
                            LBoolContinuteFlag = false;
                            continue;
                        }
                        else
                        {
                            LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                            if (LDataTableReturn.Rows.Count < 1000) { LBoolContinuteFlag = false; }
                            foreach (DataRow dr in LDataTableReturn.Rows)
                            {
                                ObjInfo objInfo = new ObjInfo();
                                objInfo.ObjID = LongParse(dr["C001"].ToString(), 0);
                                objInfo.ObjName = dr["C002"].ToString();
                                objInfo.UTCTime = LongParse(dr["C006"].ToString(), 0);
                                objInfo.LocalTime = LongParse(DateTime.Parse(dr["C004"].ToString()).ToString("yyyyMMddHHmmss"), 0);
                                LListAllObjInfo.Add(objInfo);
                                ILongNumber = objInfo.ObjID;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LBoolContinuteFlag = false;
                        FileLog.WriteInfo("", LStrDynamicSQL + "|" + ex.Message);
                    }
                }


                //更新时间段数据未处理
                foreach (ObjInfo obj in LListAllObjInfo)
                {
                    if(IsFit=="1")
                    {
                        UpdateStatisticsValue(string.Format("T_31_054_{0}", AStrRent), AMappingColumnID.ToString(), UMPService08Const.Const_Statistics_Fill, UMPService08Const.Const_Statistics_MarkTable_Record, obj);
                    }
                    else
                    {
                        UpdateStatisticsValue(string.Format("T_31_054_{0}", AStrRent), AMappingColumnID.ToString(), UMPService08Const.Const_Statistics_NotFill, UMPService08Const.Const_Statistics_MarkTable_Record, obj);
                    }
                   
                }
            }
            else if (ALlogicPartMark==1)//录音表按月分
            {
                string strPartInfo = string.Empty;

                LBoolContinuteFlag = true;
                strPartInfo = string.Format("T_21_001_{0}_{1}{2}", AStrRent, StartTime.ToString("yy"), StartTime.ToString("MM"));
                if (!IListStringTableInfo.Contains(strPartInfo)) { return; }
                //存的表要判断了.是那张表的要将表给记下来，用于写数据
                while (LBoolContinuteFlag)
                {
                    try
                    {
                        switch (IIntDatabaseType)
                        {
                            case 2:
                                {
                                    LStrDynamicSQL = string.Format("SELECT TOP 1000 C001,C002,C004,C005 FROM {0} WHERE C001>{2} AND  C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {1}) AND C004>='{3}' AND C004<'{4}' ORDER BY C001 ",strPartInfo , ALSeriID, ILongNumber, StartTime, StopTime);
                                }
                                break;
                            case 3:
                                {
                                    LStrDynamicSQL = String.Format("SELECT C001,C002,C004,C005  FROM {0} WHERE C001>{2} AND  C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {1}) AND C004>=TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND C004<TO_DATE ('{4}','YYYY-MM-DD HH24:MI:SS') AND ROWNUM <=1000   ORDER BY C001  ",strPartInfo, ALSeriID, ILongNumber, StartTime, StopTime);
                                }
                                break;
                            default:
                                break;
                        }

                        LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                        if (!LDatabaseOperationReturn.BoolReturn)
                        {
                            LBoolContinuteFlag = false;
                            continue;
                        }
                        else
                        {
                            LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                            if (LDataTableReturn.Rows.Count < 1000) { LBoolContinuteFlag = false; }
                            foreach (DataRow dr in LDataTableReturn.Rows)
                            {
                                ObjInfo objInfo = new ObjInfo();
                                objInfo.ObjID = LongParse(dr["C001"].ToString(), 0);
                                objInfo.ObjName = dr["C002"].ToString();
                                LListAllObjInfo.Add(objInfo);
                                ILongNumber = objInfo.ObjID;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LBoolContinuteFlag = false;
                        FileLog.WriteInfo("", LStrDynamicSQL + "|" + ex.Message);
                    }
                }

                //更新时间段数据未处理
                string LTableAfter = string.Empty;
                foreach (ObjInfo obj in LListAllObjInfo)
                {
                    if (IsFit == "1")
                    {
                        LTableAfter = obj.ObjName.Substring(0, 4);
                        UpdateStatisticsValue(string.Format("T_31_054_{0}_{1}", AStrRent, LTableAfter), AMappingColumnID.ToString(), UMPService08Const.Const_Statistics_Fill, UMPService08Const.Const_Statistics_MarkTable_Record, obj);
                    }
                    else
                    {
                        LTableAfter = obj.ObjName.Substring(0, 4);
                        UpdateStatisticsValue(string.Format("T_31_054_{0}_{1}", AStrRent, LTableAfter), AMappingColumnID.ToString(), UMPService08Const.Const_Statistics_NotFill, UMPService08Const.Const_Statistics_MarkTable_Record, obj);
                    }
                }
            }
        }


        //根据片断类别还原成开始时间和结束时间
        private void ReplaceDatetime(CallPeakSpliteObject ACallPeakSpliteObject, ref DateTime StartTime, ref DateTime StopTime) 
        {
            switch (ACallPeakSpliteObject.SliceType)
            {
                case 0:
                    {
                        StartTime = ACallPeakSpliteObject.DateValue.AddMinutes((ACallPeakSpliteObject.SliceOrder - 1) * 5);
                        StopTime = ACallPeakSpliteObject.DateValue.AddMinutes(ACallPeakSpliteObject.SliceOrder * 5);
                    }
                    break;
                case 1:
                    {
                        StartTime = ACallPeakSpliteObject.DateValue.AddMinutes((ACallPeakSpliteObject.SliceOrder - 1) * 15);
                        StopTime = ACallPeakSpliteObject.DateValue.AddMinutes(ACallPeakSpliteObject.SliceOrder * 15);
                    }
                    break;
                case 2:
                    {
                        StartTime = ACallPeakSpliteObject.DateValue.AddMinutes((ACallPeakSpliteObject.SliceOrder - 1) * 30);
                        StopTime = ACallPeakSpliteObject.DateValue.AddMinutes(ACallPeakSpliteObject.SliceOrder * 30);
                    }
                    break;
                case 3:
                    {
                        StartTime = ACallPeakSpliteObject.DateValue.AddMinutes((ACallPeakSpliteObject.SliceOrder - 1) * 60);
                        StopTime = ACallPeakSpliteObject.DateValue.AddMinutes(ACallPeakSpliteObject.SliceOrder * 60);
                    }
                    break;
                case 4:
                    {
                        StartTime = ACallPeakSpliteObject.DateValue.AddMinutes((ACallPeakSpliteObject.SliceOrder - 1) * 120);
                        StopTime = ACallPeakSpliteObject.DateValue.AddMinutes(ACallPeakSpliteObject.SliceOrder * 120);
                    }
                    break;
                default:
                    break;
            }

        }


        //比较大于平均值
        public bool CompleteMethod(double TrueValue, double AVGValue,int Rate) 
        {
            bool flag = false;

            if ((TrueValue - AVGValue) * 100 > AVGValue * Rate)
            {
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 得到List中ObjValue的平均值
        /// </summary>
        /// <param name="AListObjInfoAll"></param>
        /// <returns></returns>
        private double GetAVGValue( List<CallPeakSpliteObject> ACallPeakSpliteObject,int SlipeNumber)
        {
            double LDoubleAVG = 0;
            double LDoubleSUM = 0;
            if (ACallPeakSpliteObject.Count > 0)
            {
                foreach (CallPeakSpliteObject o in ACallPeakSpliteObject)
                {
                    LDoubleSUM += o.StatistcsValue;
                }
                LDoubleAVG = LDoubleSUM / SlipeNumber;
            }
            return LDoubleAVG;
        }


        //传参数得到每天各段的统计对象list
        private void GetCallPeakValue(ref List<CallPeakSpliteObject> AListCallPeakSpliteObject,string @Param01,string @Param02,string @Param03,string @Param04,string @Param05,string @Param06,string @Param07 ,string ARent,int ASpliteType,int ASpliteOrder,DateTime ADateTimeValue) 
        {

            CallPeakSpliteObject LCallPeakSpliteObject = new CallPeakSpliteObject();
            LCallPeakSpliteObject.SliceType = ASpliteType;
            LCallPeakSpliteObject.SliceOrder = ASpliteOrder;
            LCallPeakSpliteObject.DateValue = ADateTimeValue;

            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            int errNum = 0;
            string errMsg = string.Empty;
            string AOutParam01 = string.Empty;
            try
            {
                switch (IIntDatabaseType)
                {
                    //MSSQL
                    case 2:
                        {
                            DbParameter[] mssqlParameters =
                                {
                                    MssqlOperation.GetDbParameter("@ainparam01",MssqlDataType.Varchar ,5),
                                    MssqlOperation.GetDbParameter("@ainparam02",MssqlDataType.Varchar,5),
                                    MssqlOperation.GetDbParameter("@ainparam03",MssqlDataType.Varchar,20),
                                    MssqlOperation.GetDbParameter("@ainparam04",MssqlDataType.Varchar,5 ),
                                    MssqlOperation.GetDbParameter("@ainparam05",MssqlDataType.Varchar,20),
                                    MssqlOperation.GetDbParameter("@ainparam06",MssqlDataType.Varchar,20),
                                    MssqlOperation.GetDbParameter("@ainparam07",MssqlDataType.Varchar,5000),
                                    MssqlOperation.GetDbParameter("@AOutParam01",MssqlDataType.Varchar,100),
                                    MssqlOperation.GetDbParameter("@aouterrornumber",MssqlDataType.Bigint,0),
                                    MssqlOperation.GetDbParameter("@aouterrorstring",MssqlDataType.Varchar,200)
                                };
                            mssqlParameters[0].Value = @Param01;
                            mssqlParameters[1].Value = @Param02;
                            mssqlParameters[2].Value = @Param03;
                            mssqlParameters[3].Value = @Param04;
                            mssqlParameters[4].Value = @Param05;
                            mssqlParameters[5].Value = @Param06;
                            mssqlParameters[6].Value = @Param07;

                            mssqlParameters[7].Value = AOutParam01;
                            mssqlParameters[8].Value = errNum;
                            mssqlParameters[9].Value = errMsg;
                            mssqlParameters[7].Direction = ParameterDirection.Output;
                            mssqlParameters[8].Direction = ParameterDirection.Output;
                            mssqlParameters[9].Direction = ParameterDirection.Output;
                            optReturn = MssqlOperation.ExecuteStoredProcedure(IStrDatabaseProfile, "P_21_003",
                               mssqlParameters);

                            if (mssqlParameters[8].Value.ToString() != "0")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = mssqlParameters[9].Value.ToString();
                            }
                            else
                            {
                                LCallPeakSpliteObject.StatistcsValue = LongParse(mssqlParameters[7].Value.ToString(), 0);
                                AListCallPeakSpliteObject.Add(LCallPeakSpliteObject);
                            }
                        }
                        break;
                    //ORCL
                    case 3:
                        {
                            DbParameter[] orclParameters =
                                {
                                    OracleOperation.GetDbParameter("ainparam01",OracleDataType.Varchar2,5 ),
                                    OracleOperation.GetDbParameter("ainparam02",OracleDataType.Varchar2,5),
                                    OracleOperation.GetDbParameter("ainparam03",OracleDataType.Varchar2,20),
                                    OracleOperation.GetDbParameter("ainparam04",OracleDataType.Varchar2,5 ),
                                    OracleOperation.GetDbParameter("ainparam05",OracleDataType.Varchar2,20),
                                    OracleOperation.GetDbParameter("ainparam06",OracleDataType.Varchar2,20),
                                    OracleOperation.GetDbParameter("ainparam07",OracleDataType.Varchar2,5000 ),
                                    OracleOperation.GetDbParameter("AOutParam01",OracleDataType.Varchar2,100),
                                    OracleOperation.GetDbParameter("aouterrornumber",OracleDataType.Int32,0),
                                    OracleOperation.GetDbParameter("aouterrorstring",OracleDataType.Varchar2,200)
                                };
                            orclParameters[0].Value = @Param01;
                            orclParameters[1].Value = @Param02;
                            orclParameters[2].Value = @Param03;
                            orclParameters[3].Value = @Param04;
                            orclParameters[4].Value = @Param05;
                            orclParameters[5].Value = @Param06;
                            orclParameters[6].Value = @Param07;

                            orclParameters[7].Value = AOutParam01;
                            orclParameters[8].Value = errNum;
                            orclParameters[9].Value = errMsg;
                            orclParameters[7].Direction = ParameterDirection.Output;
                            orclParameters[8].Direction = ParameterDirection.Output;
                            orclParameters[9].Direction = ParameterDirection.Output;
                            optReturn = OracleOperation.ExecuteStoredProcedure(IStrDatabaseProfile, "P_21_003",
                                orclParameters);
                            if (orclParameters[8].Value.ToString() != "0")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = orclParameters[9].Value.ToString();
                            }
                            else
                            {
                                LCallPeakSpliteObject.StatistcsValue = LongParse(orclParameters[7].Value.ToString(), 0);
                                AListCallPeakSpliteObject.Add(LCallPeakSpliteObject);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("GetCallPeakValue()", "Error:"+ex.Message.ToString());
            }
            
        }


        /// <summary>
        /// 返回的列名语句以逗号隔开
        /// </summary>
        /// <param name="AStrWhere">返回的列名语句以逗号隔开</param>
        /// <param name="ASliceType"> 分段类别</param>
        /// <param name="AOrderID">第几段</param>
        private void GetColumnNameOfCallPeak(ref string AStrWhere ,int ASliceType,int AOrderID) 
        {
            AStrWhere = string.Empty;
            switch (ASliceType)
            {
                case 0: //5分钟
                    {
                        int tempH = AOrderID / 12;
                        int tempM = AOrderID % 12;
                        if (tempH > 0 && tempM == 0)
                        {
                            tempH--;
                            tempM = 12;
                        }
                        AStrWhere = "C" + tempH.ToString("00") + ((tempM - 1) * 5).ToString("00");
                    }
                    break;
                case 1://15分钟
                    {
                        int tempH = AOrderID / 4;
                        int tempM = AOrderID % 4;
                        if (tempH > 0 && tempM == 0)
                        {
                            tempH--;
                            tempM = 4;
                        }
                        int temp15M = (tempM - 1) * 15;
                        AStrWhere += "C" + tempH.ToString("00") + temp15M.ToString("00");
                        AStrWhere += "," + "C" + tempH.ToString("00") + (temp15M + 5).ToString("00");
                        AStrWhere += "," + "C" + tempH.ToString("00") + (temp15M + 10).ToString("00");
                    }
                    break;
                case 2://30分钟
                    {
                        int tempH = AOrderID / 2;
                        int tempM = AOrderID % 2;
                        if (tempH > 0 && tempM == 0)
                        {
                            tempH--;
                            tempM = 2;
                        }
                        int temp30M = (tempM - 1) * 30;
                        AStrWhere += "C" + tempH.ToString("00") + temp30M.ToString("00");
                        AStrWhere += "," + "C" + tempH.ToString("00") + (temp30M + 5).ToString("00");
                        AStrWhere += "," + "C" + tempH.ToString("00") + (temp30M + 10).ToString("00");
                        AStrWhere += "," + "C" + tempH.ToString("00") + (temp30M + 15).ToString("00");
                        AStrWhere += "," + "C" + tempH.ToString("00") + (temp30M + 20).ToString("00");
                        AStrWhere += "," + "C" + tempH.ToString("00") + (temp30M + 25).ToString("00");
                    }
                    break;
                case 3: //1小时
                    {
                        int tempH = AOrderID - 1;
                        AStrWhere += "C" + tempH.ToString("00") + "00";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "05";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "10";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "15";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "20";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "25";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "30";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "35";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "40";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "45";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "50";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "55";
                    }
                    break;
                case 4://2小时
                    {
                        int tempH = AOrderID * 2 - 2;
                        AStrWhere += "C" + tempH.ToString("00") + "00";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "05";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "10";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "15";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "20";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "25";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "30";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "35";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "40";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "45";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "50";
                        AStrWhere += "," + "C" + tempH.ToString("00") + "55";
                        AStrWhere += "," + "C" + (tempH + 1).ToString("00") + "00";
                        AStrWhere += "," + "C" + (tempH + 1).ToString("00") + "05";
                        AStrWhere += "," + "C" + (tempH + 1).ToString("00") + "10";
                        AStrWhere += "," + "C" + (tempH + 1).ToString("00") + "15";
                        AStrWhere += "," + "C" + (tempH + 1).ToString("00") + "20";
                        AStrWhere += "," + "C" + (tempH + 1).ToString("00") + "25";
                        AStrWhere += "," + "C" + (tempH + 1).ToString("00") + "30";
                        AStrWhere += "," + "C" + (tempH + 1).ToString("00") + "35";
                        AStrWhere += "," + "C" + (tempH + 1).ToString("00") + "40";
                        AStrWhere += "," + "C" + (tempH + 1).ToString("00") + "45";
                        AStrWhere += "," + "C" + (tempH + 1).ToString("00") + "50";
                        AStrWhere += "," + "C" + (tempH + 1).ToString("00") + "55";
                    }
                    break;
            }
        }



        #endregion

        //录音时长C012异常 (标准差)
        private void Do_RecordDurationExcept(string AStrRent, StatisticsMapping AStatisticsMapping, StatisticsCatogary AStatisticsCatogary, List<StatisticsItem> ALListStatisticsItem, List<StatisticsSetValue> AListStatisticsSetValue, int ALlogicPartMark,string AStrWeekConfig,String AStrMontConfig) 
        {
            try
            {
                StatisticsItem LStatisticsItemTemp = ALListStatisticsItem.Where(p => p.StatisticsID == AStatisticsCatogary.StatisticsID).Count() == 0 ? null : ALListStatisticsItem.Where(p => p.StatisticsID == AStatisticsCatogary.StatisticsID).First();
                if (LStatisticsItemTemp == null) { return; }

                StatisticsSetValue LStatisticsSetValue = AListStatisticsSetValue.Where(p => p.MappingIDOrStatisticsID == AStatisticsMapping.StatisticsMappingID && p.StatisticsItemID == LStatisticsItemTemp.StatisticsItemID && p.IsStart.Equals("Y")).Count() ==0 ? null : AListStatisticsSetValue.Where(p => p.MappingIDOrStatisticsID == AStatisticsMapping.StatisticsMappingID && p.StatisticsItemID == LStatisticsItemTemp.StatisticsItemID).First();
                if (LStatisticsSetValue == null) { return; }
                string LStrIsStart = LStatisticsSetValue.IsStart;
                string LStrSetValue004 = LStatisticsSetValue.SetValue004;
                int LIntSTD = IntParse(LStrSetValue004,0);

                List<DateTimeSplite> LListDateTimeSplite = new List<DateTimeSplite>();
                GetSpliteStatisticsSplit(AStatisticsMapping, LStatisticsItemTemp, ref LListDateTimeSplite, AStrWeekConfig, AStrMontConfig);
                if (LListDateTimeSplite.Count == 0) { return; }
                // 1平行 2 钻取 
                //1应用到所有 2为不
                List<AgentInfo> LListAgentInfoTemp = new List<AgentInfo>();
                List<OrgInfo> LListOrgInfo = new List<OrgInfo>();
                if (AStatisticsMapping.DropDown == 1) //平行时可能有技能组
                {
                    if(AStatisticsMapping.ApplayAll==2)//不应用所有机构
                    {
                        GetAgentInfoInOrg(ref LListAgentInfoTemp, AStatisticsMapping.OrgIDOrSkillID, 1, AStrRent);

                        //将座席信息写入临时表
                        Do_Common_Statistics(LListAgentInfoTemp, LListDateTimeSplite, AStrRent, ALlogicPartMark, "C012", AStatisticsMapping.MappingColumnID, LIntSTD, UMPService08Const.Const_Statistics_MarkTable_Record);
                    }
                    else if(AStatisticsMapping.ApplayAll==1)//应用到所有机构
                    {
                        GetOrgInfoSon(ref  LListOrgInfo, AStatisticsMapping.OrgIDOrSkillID);
                        foreach(OrgInfo LOrgInfoTemp in LListOrgInfo)
                        {
                            LListAgentInfoTemp = new List<AgentInfo>();
                            GetAgentInfoInOrg(ref LListAgentInfoTemp, AStatisticsMapping.OrgIDOrSkillID, 1, AStrRent);
                            //将座席信息写入临时表
                            Do_Common_Statistics(LListAgentInfoTemp, LListDateTimeSplite, AStrRent, ALlogicPartMark, "C012", AStatisticsMapping.MappingColumnID, LIntSTD, UMPService08Const.Const_Statistics_MarkTable_Record);
                        }
                    }

                }
                else if (AStatisticsMapping.DropDown == 2)//只能是机构
                {
                    GetAgentInfoInOrg(ref LListAgentInfoTemp, AStatisticsMapping.OrgIDOrSkillID, 2,AStrRent);
                    //将座席信息写入临时表
                    Do_Common_Statistics(LListAgentInfoTemp, LListDateTimeSplite, AStrRent, ALlogicPartMark, "C012", AStatisticsMapping.MappingColumnID, LIntSTD, UMPService08Const.Const_Statistics_MarkTable_Record);

                }
            }
            catch (Exception e)
            {
                
                 FileLog.WriteInfo("Do_RecordDurationExcept()", "Error:"+e.Message);
            }
        }

        private void Do_Common_Statistics(List<AgentInfo> AListAgentInfo, List<DateTimeSplite> AListDateTimeSplite, string AStrRent, int ALlogicPartMark, String AStrColumnName, int AMappingColumnID, int ALIntSTD,int AMarkTable) 
        {
            //先将座席写入临时表
            List<string> LListStrAgentName = new List<string>();
            LListStrAgentName.Add(string.Empty);
            LListStrAgentName.Add(AListAgentInfo.Count.ToString());
            foreach (AgentInfo AgentTemp in AListAgentInfo)
            {
                string strInfo = string.Format("{0}{1}", AgentTemp.ObjName, AscCodeToChr(27));
                LListStrAgentName.Add(strInfo);
            }

            FileLog.WriteInfo("Do_Common_Statistics()", "InsertTempData()");
            OperationReturn operationTemp = InsertTempData(LListStrAgentName, AStrRent);
            if (!operationTemp.Result) { return; }
            string LSeriID = operationTemp.Data.ToString();


           //统计数据的List
            List<ObjInfo> LListAllObjInfo = null;
            List<ObjInfo> LListObjInfoFit = null;
            List<ObjInfo> LListObjInfoNotFit = null;
            DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
            DataOperations01 LDataOperations = new DataOperations01();
            string LStrDynamicSQL = string.Empty;
            DataTable LDataTableReturn = new DataTable();
            long ILongNumber=0;
            foreach(DateTimeSplite LDateTimeSingle in AListDateTimeSplite)
            {
                 LDatabaseOperationReturn = new DatabaseOperation01Return();
                 LDataOperations = new DataOperations01();
                 LStrDynamicSQL = string.Empty;
                 LDataTableReturn = new DataTable();
                 LListAllObjInfo = new List<ObjInfo>();
                 LListObjInfoFit = new List<ObjInfo>(); 
                 LListObjInfoNotFit = new List<ObjInfo>(); 

                ILongNumber = 0;
                bool LBoolContinuteFlag = true;
                if(ALlogicPartMark==2)
                {
                    while (LBoolContinuteFlag) 
                    {
                        try
                        {
                            switch (IIntDatabaseType)
                            {
                                case 2:
                                    {
                                        LStrDynamicSQL = string.Format("SELECT TOP 1000 C001,C002,C004,C005,C006,{3} FROM T_21_001_{0} WHERE C001>{2} AND  C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {1}) AND C004>='{4}' AND C004<'{5}' ORDER BY C001 ", AStrRent, LSeriID, ILongNumber, AStrColumnName, LDateTimeSingle.StartStatisticsTime, LDateTimeSingle.StopStatisticsTime);
                                    }
                                    break;
                                case 3:
                                    {
                                        LStrDynamicSQL = String.Format("SELECT C001,C002,C004,C005,C006,{3} FROM T_21_001_{0} WHERE C001>{2} AND  C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {1}) AND C004>='{4}' AND C004<'{5}' AND ROWNUM <=1000   ORDER BY C001  ", AStrRent, LSeriID, ILongNumber, AStrColumnName, LDateTimeSingle.StartStatisticsTime, LDateTimeSingle.StopStatisticsTime);
                                    }
                                    break;
                                default:
                                    break;
                            }

                            LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                            if (!LDatabaseOperationReturn.BoolReturn)
                            {
                                LBoolContinuteFlag = false;
                                continue;
                            }
                            else
                            {
                                LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                                if (LDataTableReturn.Rows.Count < 1000) { LBoolContinuteFlag = false; }
                                foreach (DataRow dr in LDataTableReturn.Rows)
                                {
                                    ObjInfo objInfo = new ObjInfo();
                                    objInfo.ObjID = LongParse(dr["C001"].ToString(), 0);
                                    objInfo.ObjName = dr["C002"].ToString();
                                    objInfo.UTCTime = LongParse(dr["C006"].ToString(), 0);
                                    objInfo.LocalTime = LongParse(DateTime.Parse(dr["C004"].ToString()).ToString("yyyyMMddHHmmss"), 0);
                                    objInfo.ObjValue00 = DoubleParse(dr[AStrColumnName].ToString(), 0);
                                    LListAllObjInfo.Add(objInfo);
                                    ILongNumber = objInfo.ObjID;
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            LBoolContinuteFlag = false;
                            FileLog.WriteInfo("", LStrDynamicSQL + "|" + ex.Message);
                        }
                    }

                    GetNotFillStandardDeviation(LListAllObjInfo, ref LListObjInfoNotFit, ref LListObjInfoFit, (double)ALIntSTD);

                    //更新时间段数据未处理
                    foreach (ObjInfo obj in LListObjInfoNotFit)
                    {
                        UpdateStatisticsValue(string.Format("T_31_054_{0}", AStrRent), AMappingColumnID.ToString(), UMPService08Const.Const_Statistics_NotFill, AMarkTable, obj);
                    }

                    foreach (ObjInfo obj in LListObjInfoFit)
                    {
                        UpdateStatisticsValue(string.Format("T_31_054_{0}", AStrRent), AMappingColumnID.ToString(), UMPService08Const.Const_Statistics_Fill, AMarkTable, obj);
                    }
                    
                }
                else if (ALlogicPartMark == 1) //按月分表
                {                   
                   
                    //按月分表，再继续将时间分段。
                    List<DateTimeSplite> ListDateTimeSplite2 = new List<DateTimeSplite>();
                    GetSpliteTime(LDateTimeSingle.StartStatisticsTime, LDateTimeSingle.StopStatisticsTime, ref ListDateTimeSplite2);
                    string strPartInfo = string.Empty;
                    foreach(DateTimeSplite DateTimeSpliteTemp in ListDateTimeSplite2)
                    {
                        ILongNumber = 0;
                        LBoolContinuteFlag = true;
                        strPartInfo = string.Format("T_21_001_{0}_{1}{2}", AStrRent, DateTimeSpliteTemp.StartStatisticsTime.ToString("yy"), DateTimeSpliteTemp.StartStatisticsTime.ToString("MM"));
                        if (!IListStringTableInfo.Contains(strPartInfo)) { continue; }
                        //存的表要判断了.是那张表的要将表给记下来，用于写数据
                        while (LBoolContinuteFlag) 
                        {
                            try
                            {
                                switch (IIntDatabaseType)
                                {
                                    case 2:
                                        {
                                            LStrDynamicSQL = string.Format("SELECT TOP 1000 C001,C002,C004,C005,{3} FROM {0} WHERE C001>{2} AND  C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {1}) AND C004>='{4}' AND C004<'{5}' ORDER BY C001 ", strPartInfo, LSeriID, ILongNumber, AStrColumnName, DateTimeSpliteTemp.StartStatisticsTime, DateTimeSpliteTemp.StopStatisticsTime);
                                        }
                                        break;
                                    case 3:
                                        {
                                            LStrDynamicSQL = String.Format("SELECT C001,C002,C004,C005,{3} FROM {0} WHERE C001>{2} AND  C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {1}) AND C004>=TO_DATE ('{4}','YYYY-MM-DD HH24:MI:SS') AND C004<TO_DATE ('{5}','YYYY-MM-DD HH24:MI:SS') AND ROWNUM <=1000   ORDER BY C001  ", strPartInfo, LSeriID, ILongNumber, AStrColumnName, DateTimeSpliteTemp.StartStatisticsTime, DateTimeSpliteTemp.StopStatisticsTime);
                                        }
                                        break;
                                    default:
                                        break;
                                }

                                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                                if (!LDatabaseOperationReturn.BoolReturn)
                                {
                                    LBoolContinuteFlag = false;
                                    continue;
                                }
                                else
                                {
                                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                                    if (LDataTableReturn.Rows.Count < 1000) { LBoolContinuteFlag = false; }
                                    foreach (DataRow dr in LDataTableReturn.Rows)
                                    {
                                        ObjInfo objInfo = new ObjInfo();
                                        objInfo.ObjID = LongParse(dr["C001"].ToString(), 0);
                                        objInfo.ObjName = dr["C002"].ToString();
                                        objInfo.ObjValue00 = DoubleParse(dr[AStrColumnName].ToString(), 0);
                                        LListAllObjInfo.Add(objInfo);
                                        ILongNumber = objInfo.ObjID;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LBoolContinuteFlag = false;
                                FileLog.WriteInfo("", LStrDynamicSQL + "|" + ex.Message);
                            }
                        }
                    }
                    GetNotFillStandardDeviation(LListAllObjInfo, ref LListObjInfoNotFit, ref LListObjInfoFit, (double)ALIntSTD);

                    //更新时间段数据
                    string LTableAfter = string.Empty;
                    foreach (ObjInfo obj in LListObjInfoNotFit)
                    {
                        LTableAfter = obj.ObjName.Substring(0, 4);
                        UpdateStatisticsValue(string.Format("T_31_054_{0}_{1}", AStrRent, LTableAfter), AMappingColumnID.ToString(), UMPService08Const.Const_Statistics_NotFill, AMarkTable, obj);
                    }

                    foreach (ObjInfo obj in LListObjInfoFit)
                    {
                        LTableAfter = obj.ObjName.Substring(0, 4);
                        UpdateStatisticsValue(string.Format("T_31_054_{0}_{1}", AStrRent, LTableAfter), AMappingColumnID.ToString(), UMPService08Const.Const_Statistics_Fill, AMarkTable, obj);
                    }
                }
            }
        }

        private void GetSpliteTime(DateTime AStartTime, DateTime AStopTime, ref List<DateTimeSplite> AListDateTimeSplite)
        {
            DateTime LDateTimeStartUTC = AStartTime.ToUniversalTime();
            DateTime LDateTimeStopUTC = AStopTime.ToUniversalTime();
            DateTimeSplite LDateTimeSpliteTemp;
            if (new DateTime(LDateTimeStartUTC.Year, LDateTimeStartUTC.Month, 1) != new DateTime(LDateTimeStopUTC.Year, LDateTimeStopUTC.Month, 1))
            {
                DateTime LDateTimeUTCTemp = new DateTime(LDateTimeStartUTC.Year, LDateTimeStartUTC.Month + 1, 1);
                while (LDateTimeUTCTemp <= LDateTimeStopUTC)
                {
                    LDateTimeSpliteTemp = new DateTimeSplite();
                    LDateTimeSpliteTemp.StartStatisticsTime = LDateTimeStartUTC;
                    LDateTimeSpliteTemp.StopStatisticsTime = LDateTimeUTCTemp;

                    AListDateTimeSplite.Add(LDateTimeSpliteTemp);
                    LDateTimeStartUTC = LDateTimeUTCTemp;
                    LDateTimeUTCTemp= LDateTimeUTCTemp.AddMonths(1);                   
                }

                if (LDateTimeUTCTemp > LDateTimeStopUTC) 
                {
                    LDateTimeSpliteTemp = new DateTimeSplite();
                    LDateTimeSpliteTemp.StartStatisticsTime = LDateTimeStartUTC;
                    LDateTimeSpliteTemp.StopStatisticsTime = LDateTimeStopUTC;
                    AListDateTimeSplite.Add(LDateTimeSpliteTemp);
                }
            }
            else 
            {
                LDateTimeSpliteTemp = new DateTimeSplite();
                LDateTimeSpliteTemp.StartStatisticsTime = LDateTimeStartUTC;
                LDateTimeSpliteTemp.StopStatisticsTime = LDateTimeStopUTC;
                AListDateTimeSplite.Add(LDateTimeSpliteTemp);
            }
        }

        /// <summary>
        /// 更新统计数据到统计表
        /// </summary>
        /// <param name="AStrTableName">更新的表名</param>
        /// <param name="AColumnNumber">更新到那列</param>
        /// <param name="AStrValue">写入列值</param>
        private void UpdateStatisticsValue( string AStrTableName, string AColumnNumber,string AStrValue,int AIntMarkTable,ObjInfo AObjInfo)
        {
            try
            {
                string LStrDynamicSQL = string.Empty;
                int LIntLength=AColumnNumber.Length;
                string LStrColumnName= string.Empty;
                switch (LIntLength)
	            {
                    case 1:
                        LStrColumnName="C00"+AColumnNumber;
                        break;
                    case 2:
                        LStrColumnName="C0"+AColumnNumber;
                        break;
                    case 3:
                        LStrColumnName="C"+AColumnNumber;
                        break;
		            default:
                    break;
	            }

                switch (IIntDatabaseType)                    
                {
                    case 2:
                        { 
                            if(AIntMarkTable ==1) //更新录音统计表
                            {
                                 LStrDynamicSQL= string.Format(" IF NOT EXISTS (SELECT * FROM {0} WHERE C000={1})	INSERT INTO {0} (C000,C201,{2},C202,C203)VALUES({1},{3},'{4}',{5},{6}) ELSE 	UPDATE {0} SET C201={3},{2}='{4}',C202={5},C203={6} WHERE C000={1}",
                                     AStrTableName, 
                                     AObjInfo.ObjID, 
                                     LStrColumnName,
                                     AObjInfo.ObjName, 
                                     AStrValue,
                                     AObjInfo.LocalTime,
                                     AObjInfo.UTCTime);
                            }
                            else if(AIntMarkTable ==2)//更新成绩统计表
                            {
                                 LStrDynamicSQL= string.Format("IF NOT EXISTS (SELECT * FROM {0} WHERE C000={1}) INSERT INTO {0} (C000,{2})VALUES({1},'{3}') ELSE 	UPDATE {0} SET {2}='{3}' WHERE C000={1}",
                                    AStrTableName,
                                    AObjInfo.ObjID,
                                    LStrColumnName,
                                    AStrValue);
                            }                   
                        }
                        break;
                    case 3:
                        { 
                            if(AIntMarkTable ==1) //更新录音统计表
                            {
                                LStrDynamicSQL = string.Format("BEGIN   UPDATE {0} SET {1} = '{2}' ,C201={3},C202={5},C203={6} WHERE C000={4} ;   IF SQL%NOTFOUND THEN   INSERT INTO {0}(C000,{1},C201) VALUES ({4},{2}',{3});  END IF; COMMIT; END;", AStrTableName, LStrColumnName, AStrValue, AObjInfo.ObjName, AObjInfo.ObjID, AObjInfo.LocalTime,
                                     AObjInfo.UTCTime);
                            }
                            else if(AIntMarkTable ==2)//更新成绩统计表
                            {
                                LStrDynamicSQL = string.Format("BEGIN   UPDATE {0} SET {1} = '{2}' ,C201={3} WHERE C000={4} ;   IF SQL%NOTFOUND THEN   INSERT INTO {0}(C000,{1},C201) VALUES ({4},{2}',{3});  END IF;COMMIT;  END;", AStrTableName, LStrColumnName, AStrValue, AObjInfo.ObjName, AObjInfo.ObjID);
                            } 
                        }
                        break;
                    default:
                        {
                            return;
                        }
                }

                FileLog.WriteInfo("UpdateStatisticsValue()", LStrDynamicSQL);
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
                LDatabaseOperationReturn = LDataOperations.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    FileLog.WriteInfo("UpdateStatisticsValue()",LStrDynamicSQL+"Error");
                }

            }
            catch (Exception ex)
            {

                FileLog.WriteInfo("UpdateStatisticsValue()", "Error :"+ ex.Message);
            }

        }

        /// <summary>
        ///  得到机构以及其下的机构信息
        /// </summary>
        /// <param name="AListOrgInfo"></param>
        /// <param name="AOrgID"></param>
        private void GetOrgInfoSon(ref List<OrgInfo> AListOrgInfo, long AOrgID) 
        {
            OrgInfo LOrgInfoTemp = IListOrgInfo.Where(p => p.ObjID == AOrgID).Count() > 0 ? IListOrgInfo.Where(p => p.ObjID == AOrgID).First() : null;
            if(LOrgInfoTemp !=null)
            {
                if (!AListOrgInfo.Contains(LOrgInfoTemp))
                {
                    AListOrgInfo.Add(LOrgInfoTemp);
                    List<OrgInfo> LListOrgInfoTemp = IListOrgInfo.Where(p => p.ParentOrgID == AOrgID).ToList();
                    foreach (OrgInfo LOrgInfo in LListOrgInfoTemp)
                    {
                        GetOrgInfoSon(ref AListOrgInfo, LOrgInfo.ObjID);
                    }
           
                }
            }

           
        }

        /// <summary>
        ///  得到机构下座席
        /// </summary>
        /// <param name="AListAgentInfo"></param>
        /// <param name="AOrgID"></param>
        /// <param name="AType">1平行  2 为钻取 </param>
        private void GetAgentInfoInOrg(ref List<AgentInfo> AListAgentInfo,long AOrgIDOrSkillID, int AType,string AStrRent ) 
        {
            if(AOrgIDOrSkillID>9060000000000000000 && AOrgIDOrSkillID<9070000000000000000)//技能组
            {
                // GetObjInfoMapping( string AResourceCodeBegin,string AResourceCodeEnd, string AStrRent, ref List<long> AListObjMappingID)
                List<long> LLongSkillMappingID = new List<long>();
                GetObjInfoMapping("906", "907", AStrRent,ref LLongSkillMappingID);
                foreach (long AgentID in LLongSkillMappingID) 
                {
                    AgentInfo agentInfo = IListAgentInfo.Where(p => p.ObjID == AgentID).Count() > 0 ? IListAgentInfo.Where(p => p.ObjID == AgentID).First() : null;
                    if(!AListAgentInfo.Contains(agentInfo))
                    {
                        AListAgentInfo.Add(agentInfo);
                    }
                }

            }
            else if (AOrgIDOrSkillID > 1010000000000000000 && AOrgIDOrSkillID < 1020000000000000000)//机构 
            {
                if (AType == 1)
                {
                    AListAgentInfo = IListAgentInfo.Where(p => p.BeyondOrgID == AOrgIDOrSkillID).ToList();
                }
                else if (AType == 2)
                {
                    List<AgentInfo> LListAgentInfo = IListAgentInfo.Where(p => p.BeyondOrgID == AOrgIDOrSkillID).ToList();
                    foreach (AgentInfo LAgent in LListAgentInfo)
                    {
                        if (!AListAgentInfo.Contains(LAgent))
                        {
                            AListAgentInfo.Add(LAgent);
                        }
                    }
                    List<OrgInfo> LListOrgInfo = IListOrgInfo.Where(p => p.ParentOrgID == AOrgIDOrSkillID).ToList();
                    foreach (OrgInfo LOrg in LListOrgInfo)
                    {
                        GetAgentInfoInOrg(ref AListAgentInfo, LOrg.ObjID, AType, AStrRent);
                    }
                }
            }
        }


        /// <summary>
        /// 根据时间来判断是否要执行统计
        /// </summary>
        /// <param name="ABoolIsCanRun"></param>
        /// <param name="ARunFrequence"></param>
        private void IsCanStatistics(ref bool ABoolIsCanRun,RunFrequence  ARunFrequence)
        {
            DateTime LDateTimeNow = DateTime.Now;
            int LIntStatisticsTime = IntParse(ARunFrequence.StrDayTime.Substring(0, 2), 0) * 3600 + IntParse(ARunFrequence.StrDayTime.Substring(3, 2), 0) * 60 + IntParse(ARunFrequence.StrDayTime.Substring(6, 2), 0);
            int LIntConvertSecond = LDateTimeNow.Hour * 3600 + LDateTimeNow.Minute * 60 + LDateTimeNow.Second;
            switch (ARunFrequence.RunFreq.ToUpper())
            {
                case "D":
                    {
                        if (LIntStatisticsTime <= LIntConvertSecond && (ARunFrequence.LastRunTime.Date != LDateTimeNow.Date))
                            ABoolIsCanRun = true;
                    }
                    break;
                case "W"://周
                    {
                        string dayOfWeek = string.Empty;
                        switch (ARunFrequence.DayOfWeek)
                        {
                            case 1:
                                dayOfWeek = DayOfWeek.Monday.ToString();
                                break;
                            case 2:
                                dayOfWeek = DayOfWeek.Tuesday.ToString();
                                break;
                            case 3:
                                dayOfWeek = DayOfWeek.Wednesday.ToString();
                                break;
                            case 4:
                                dayOfWeek = DayOfWeek.Thursday.ToString();
                                break;
                            case 5:
                                dayOfWeek = DayOfWeek.Friday.ToString();
                                break;
                            case 6:
                                dayOfWeek = DayOfWeek.Saturday.ToString();
                                break;
                            case 7:
                                dayOfWeek = DayOfWeek.Sunday.ToString();
                                break;
                        }
                        if (LDateTimeNow.DayOfWeek.ToString() == dayOfWeek && LIntStatisticsTime <= LIntConvertSecond && (ARunFrequence.LastRunTime.Date != LDateTimeNow.Date))
                        {
                            ABoolIsCanRun = true;
                        }
                    }
                    break;
                case "P"://旬
                    ABoolIsCanRun = false;
                    break;
                case "M"://月
                    {
                        int maxday = GetMonthMaxDay();
                        int setday = 0;
                        switch (ARunFrequence.DayOfMonth)
                        {
                            case -1:
                                {
                                    setday=maxday;
                                }
                                break;
                            case -2:
                                {
                                    setday=maxday-1;
                                }
                                break;
                            case -3:
                                {
                                    setday= maxday-2;
                                }
                                break;
                            default:
                                setday = ARunFrequence.DayOfMonth;
                                break;
                        }
                        if (LDateTimeNow.Day <= setday && LIntStatisticsTime <= LIntConvertSecond && ARunFrequence.LastRunTime.Month != LDateTimeNow.Month)
                        {
                            ABoolIsCanRun = true;
                        }
                    }
                    break;
                case "S"://季
                    ABoolIsCanRun = false;
                    break;
                case "Y"://年
                    {
                        if (LDateTimeNow.Day <= ARunFrequence.DayOfYear && LIntStatisticsTime <= LIntConvertSecond && ARunFrequence.LastRunTime.Year != LDateTimeNow.Year)
                        {
                            ABoolIsCanRun = true;
                        }
                    }
                    break;
                default:
                    ABoolIsCanRun = false;
                    break;
            }
        }



        /// <summary>
        /// 更新运行时间的lastRunTime
        /// </summary>
        /// <param name="AStrRent"></param>
        /// <param name="ARunFrequence"></param>
        private void Update_RunFrequence(string AStrRent, RunFrequence ARunFrequence)
        {

            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("UPDATE  T_31_026_{1} SET C018='{0}'    WHERE C001={2}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), AStrRent, ARunFrequence.FrequencyID);
                LDatabaseOperationReturn = LDataOperations.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                    FileLog.WriteInfo("Update_RunFrequence()", "Fail");
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("Update_RunFrequence()", ex.Message);
            }

        }



        private void GetSpliteStatisticsSplit(StatisticsMapping AStatisticsMapping, StatisticsItem AStatisticsItemTemp, ref List<DateTimeSplite> AListDateTimeSplite, string AStrWeekConfig, String AStrMonthConfig) 
        {
            AListDateTimeSplite.Clear();
            DateTime LDateTimeNow = DateTime.Now.Date;
            DateTime LDateTimeStart = LDateTimeNow;
            switch (AStatisticsMapping.UpdateTimeUnit)//1年，2月，3周，4天
            {
                case 1:
                    {
                        LDateTimeStart = LDateTimeNow.AddYears(AStatisticsMapping.UpdateValueTime*(-1));
                    }
                    break;
                case 2://月 
                    {
                        LDateTimeStart = LDateTimeNow.AddMonths(AStatisticsMapping.UpdateValueTime * (-1));
                    }
                    break;
                case 3://周
                    {
                        LDateTimeStart = LDateTimeNow.AddDays(AStatisticsMapping.UpdateValueTime * 7 * (-1));
                    }
                    break;
                case 4://更新天数据
                    {

                        LDateTimeStart = LDateTimeNow.AddDays(AStatisticsMapping.UpdateValueTime * (-1));
                    }
                    break;
                default:
                    break;
            }
            if (LDateTimeStart >= LDateTimeNow) { return; }
            if(AStatisticsItemTemp ==null)
            {
                DateTimeSplite LDateTimeSpliteTemp = new DateTimeSplite();
                LDateTimeSpliteTemp.StartStatisticsTime = LDateTimeStart;
                LDateTimeSpliteTemp.StopStatisticsTime = LDateTimeNow;
                LDateTimeSpliteTemp.UpdateStartTime = LDateTimeStart;
                LDateTimeSpliteTemp.UpdateStopTime = LDateTimeNow;
                AListDateTimeSplite.Add(LDateTimeSpliteTemp);
            }
            if (AStatisticsItemTemp !=null)
            {
                if (AStatisticsItemTemp.IsAVGOrStandarDev == 2 || AStatisticsItemTemp.IsAVGOrStandarDev == 3)//带平均或标准差切片
                {
                    //切片
                    DateTime LDateTimeItemStop = LDateTimeNow;
                    DateTime LDateTimeItemStart = LDateTimeNow;
                    DateTimeSplite LDateTimeSpliteTemp = null;
                    switch (AStatisticsItemTemp.SliceTimeUnit)
                    {
                        case 1: //1年
                            {
                                LDateTimeItemStop = new DateTime(LDateTimeNow.Year, 1, 1);

                                while (LDateTimeItemStop > LDateTimeStart)
                                {
                                    LDateTimeItemStart = LDateTimeItemStop.AddYears(AStatisticsItemTemp.SliceTimeValue * (-1));
                                    LDateTimeSpliteTemp = new DateTimeSplite();
                                    LDateTimeSpliteTemp.StartStatisticsTime = LDateTimeItemStart;
                                    LDateTimeSpliteTemp.StopStatisticsTime = LDateTimeItemStop;
                                    if (LDateTimeItemStart >= LDateTimeStart)
                                    {
                                        LDateTimeSpliteTemp.UpdateStartTime = LDateTimeItemStart;
                                        LDateTimeSpliteTemp.UpdateStopTime = LDateTimeItemStop;
                                    }
                                    else
                                    {
                                        LDateTimeSpliteTemp.UpdateStartTime = LDateTimeStart;
                                        LDateTimeSpliteTemp.UpdateStopTime = LDateTimeItemStop;
                                    }
                                    AListDateTimeSplite.Add(LDateTimeSpliteTemp);
                                    LDateTimeItemStop = LDateTimeItemStart;
                                }
                            }
                            break;
                        case 2://2月
                            {
                                int LIntMonthStartSet = IntParse(AStrMonthConfig, 0);//1为1号 2为2号 26为26号
                                if (LIntMonthStartSet == 0) { return; }
                                int LIntNowDate = LDateTimeItemStop.Day;
                                if (LIntMonthStartSet < LIntNowDate)
                                {
                                    LDateTimeItemStop = new DateTime(LDateTimeItemStop.Year, LDateTimeItemStop.Month, LIntMonthStartSet);
                                }
                                else if (LIntMonthStartSet > LIntNowDate)//启动日期小于月设置的开始时间，则要往前推一个月
                                {
                                    LDateTimeItemStop = new DateTime(LDateTimeItemStop.Year, LDateTimeItemStop.Month, LIntMonthStartSet).AddMonths(-1);
                                }

                                while (LDateTimeItemStop > LDateTimeStart)
                                {
                                    LDateTimeItemStart = LDateTimeItemStop.AddMonths(AStatisticsItemTemp.SliceTimeValue * (-1));
                                    LDateTimeSpliteTemp = new DateTimeSplite();
                                    LDateTimeSpliteTemp.StartStatisticsTime = LDateTimeItemStart;
                                    LDateTimeSpliteTemp.StopStatisticsTime = LDateTimeItemStop;
                                    if (LDateTimeItemStart >= LDateTimeStart)
                                    {
                                        LDateTimeSpliteTemp.UpdateStartTime = LDateTimeItemStart;
                                        LDateTimeSpliteTemp.UpdateStopTime = LDateTimeItemStop;
                                    }
                                    else
                                    {
                                        LDateTimeSpliteTemp.UpdateStartTime = LDateTimeStart;
                                        LDateTimeSpliteTemp.UpdateStopTime = LDateTimeItemStop;
                                    }
                                    AListDateTimeSplite.Add(LDateTimeSpliteTemp);
                                    LDateTimeItemStop = LDateTimeItemStart;
                                }
                            }
                            break;
                        case 3://3周
                            {
                                int LIntWeekStartSet = IntParse(AStrWeekConfig, 0);//0为星期天，1为星期1，6为星期6
                                int LIntWeek = 0;

                                switch (LDateTimeItemStop.DayOfWeek)
                                {
                                    case DayOfWeek.Monday:
                                        LIntWeek = 1;
                                        break;
                                    case DayOfWeek.Tuesday:
                                        LIntWeek = 2;
                                        break;
                                    case DayOfWeek.Wednesday:
                                        LIntWeek = 3;
                                        break;
                                    case DayOfWeek.Thursday:
                                        LIntWeek = 4;
                                        break;
                                    case DayOfWeek.Friday:
                                        LIntWeek = 5;
                                        break;
                                    case DayOfWeek.Saturday:
                                        LIntWeek = 6;
                                        break;
                                    case DayOfWeek.Sunday:
                                        LIntWeek = 0;
                                        break;
                                    default:
                                        break;
                                }

                                if (LIntWeekStartSet < LIntWeek)
                                {
                                    LDateTimeItemStop = LDateTimeItemStop.AddDays(LIntWeekStartSet - LIntWeek);
                                }
                                else if (LIntWeekStartSet > LIntWeek)
                                {
                                    LDateTimeItemStop = LDateTimeItemStop.AddDays(LIntWeekStartSet - LIntWeek).AddDays(-7);
                                }

                                while (LDateTimeItemStop > LDateTimeStart)
                                {
                                    LDateTimeItemStart = LDateTimeItemStop.AddDays(AStatisticsItemTemp.SliceTimeValue * (-7));
                                    LDateTimeSpliteTemp = new DateTimeSplite();
                                    LDateTimeSpliteTemp.StartStatisticsTime = LDateTimeItemStart;
                                    LDateTimeSpliteTemp.StopStatisticsTime = LDateTimeItemStop;
                                    if (LDateTimeItemStart >= LDateTimeStart)
                                    {
                                        LDateTimeSpliteTemp.UpdateStartTime = LDateTimeItemStart;
                                        LDateTimeSpliteTemp.UpdateStopTime = LDateTimeItemStop;
                                    }
                                    else
                                    {
                                        LDateTimeSpliteTemp.UpdateStartTime = LDateTimeStart;
                                        LDateTimeSpliteTemp.UpdateStopTime = LDateTimeItemStop;
                                    }
                                    AListDateTimeSplite.Add(LDateTimeSpliteTemp);
                                    LDateTimeItemStop = LDateTimeItemStart;
                                }
                            }
                            break;
                        case 4://4天
                            {
                                while (LDateTimeItemStop > LDateTimeStart)
                                {
                                    LDateTimeItemStart = LDateTimeItemStop.AddDays(AStatisticsItemTemp.SliceTimeValue * (-1));
                                    LDateTimeSpliteTemp = new DateTimeSplite();
                                    LDateTimeSpliteTemp.StartStatisticsTime = LDateTimeItemStart;
                                    LDateTimeSpliteTemp.StopStatisticsTime = LDateTimeItemStop;
                                    if (LDateTimeItemStart >= LDateTimeStart)
                                    {
                                        LDateTimeSpliteTemp.UpdateStartTime = LDateTimeItemStart;
                                        LDateTimeSpliteTemp.UpdateStopTime = LDateTimeItemStop;
                                    }
                                    else
                                    {
                                        LDateTimeSpliteTemp.UpdateStartTime = LDateTimeStart;
                                        LDateTimeSpliteTemp.UpdateStopTime = LDateTimeItemStop;
                                    }
                                    AListDateTimeSplite.Add(LDateTimeSpliteTemp);
                                    LDateTimeItemStop = LDateTimeItemStart;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        
        //得到本月最多天数
        protected int GetMonthMaxDay()
        {
            int maxday = 0;
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            switch (month)
            {
                case 2:
                    {
                        if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0))
                        {
                            maxday = 29;
                        }
                        else
                            maxday = 28;
                    }
                    break;
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    maxday = 31;
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    maxday = 30;
                    break;

            }
            return maxday;
        }

        /// <summary>
        /// 得到所有运行周期
        /// </summary>
        private void GetAllRunFrequency(ref  List<RunFrequence> AListRunFrequence, String AStrRent, string AStrSerialID)
        {

            AListRunFrequence.Clear();

            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT   *  FROM T_31_026_{0}  WHERE C001 IN (SELECT C011 FROM T_00_901 WHERE C001 = {1}) ", AStrRent, AStrSerialID);
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow dr in LDataTableReturn.Rows)
                    {
                        RunFrequence runFrequenceTemp = new RunFrequence();
                        runFrequenceTemp.FrequencyID = LongParse(dr["C001"].ToString(), 0);
                        runFrequenceTemp.RunFreq = dr["C002"].ToString();
                        runFrequenceTemp.StrDayTime = dr["C003"].ToString();
                        runFrequenceTemp.DayOfWeek = IntParse(dr["C004"].ToString(),0);
                        runFrequenceTemp.DayOfMonth = IntParse(dr["C005"].ToString(), 0);
                        runFrequenceTemp.DayOfYear = IntParse(dr["C017"].ToString(), 0);
                        if(dr["C018"] !=DBNull.Value)
                        {
                            runFrequenceTemp.LastRunTime = DateTime.Parse(dr["C018"].ToString());
                        }
                        AListRunFrequence.Add(runFrequenceTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetAllBinding()", ex.Message);
            } 
        }

        /// <summary>
        /// 将运行周期写入临时表
        /// </summary>
        /// <param name="listParams"></param>
        /// <param name="AStrRent"></param>
        /// <returns></returns>
        private OperationReturn InsertTempData( List<string> listParams,String AStrRent)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                //ListParams
                //0         tempID
                //1         count
                //2..       tempData(tempData property split by char 27, less than 5)
                string strTempID = listParams[0];
                string strCount = listParams[1];
                int intCount;
                if (!int.TryParse(strCount, out intCount) || intCount <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Resource count invalid");
                    return optReturn;
                }
                if (listParams.Count < 2 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Resource count invalid");
                    return optReturn;
                }
                if (string.IsNullOrEmpty(strTempID))
                {
                    List<string> listGetSerialIDParams = new List<string>();
                    listGetSerialIDParams.Add("11");
                    listGetSerialIDParams.Add("911");
                    listGetSerialIDParams.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    OperationReturn getSerialIDReturn = GetSerialID(listGetSerialIDParams, AStrRent);
                    if (!getSerialIDReturn.Result)
                    {
                        return getSerialIDReturn;
                    }
                    strTempID = getSerialIDReturn.Data.ToString();
                }
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (IIntDatabaseType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("select * from t_00_901 where c001 = {0}", strTempID);
                        objConn = MssqlOperation.GetConnection(IStrDatabaseProfile);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("select * from t_00_901 where c001 = {0}", strTempID);
                        objConn = OracleOperation.GetConnection(IStrDatabaseProfile);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not support");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    int number = 0;
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        number = Math.Max(number, Convert.ToInt32(objDataSet.Tables[0].Rows[i]["C002"]));
                    }
                    for (int i = 2; i < listParams.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].NewRow();
                        string strTempData = listParams[i];
                        string[] arrTempData = strTempData.Split(new[] { AscCodeToChr(27) },
                            StringSplitOptions.RemoveEmptyEntries);
                        dr["C001"] = strTempID;
                        dr["C002"] = number + i - 1;
                        if (arrTempData.Length > 0)
                        {
                            dr["C011"] = arrTempData[0];
                        }
                        if (arrTempData.Length > 1)
                        {
                            dr["C012"] = arrTempData[1];
                        }
                        if (arrTempData.Length > 2)
                        {
                            dr["C013"] = arrTempData[2];
                        }
                        if (arrTempData.Length > 3)
                        {
                            dr["C014"] = arrTempData[3];
                        }
                        if (arrTempData.Length > 4)
                        {
                            dr["C015"] = arrTempData[4];
                        }
                        objDataSet.Tables[0].Rows.Add(dr);
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }

                optReturn.Data = strTempID;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }



        /// <summary>
        /// 获取是否表有逻辑分表
        /// </summary>
        /// <param name="ARent"></param>
        /// <returns> 0、运行错误1、有按月分表 2、 无按月分表 </returns>
        private int ObtainRentLogicTable( string ARentToken,string ATableNameField) 
        {
            int Flag = 2;
            string LStrDynamicSQL = string.Empty;
            DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
            DataOperations01 LDataOperations = new DataOperations01();
            LStrDynamicSQL = "SELECT * FROM T_00_000 WHERE C000 = '" + ARentToken + "' AND C001 = '" + ATableNameField + "' AND C004 = '1'";
            LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
            if (!LDatabaseOperationReturn.BoolReturn)
            {
               return  Flag = 0;
            }

            if (LDatabaseOperationReturn.StrReturn == "1") 
            {
               return  Flag = 1;
            }

            return Flag;
        }


        /// <summary>
        /// 得到所有的租户
        /// </summary>
        private void ObtainRentList(ref List<string> AListStrRentExistObjects)
        {
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = "SELECT * FROM T_00_121 ORDER BY C001 ASC";
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow LDataRowSingleRent in LDataTableReturn.Rows)
                    {
                        LStrRentToken = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["C021"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        while (!Regex.IsMatch(LStrRentToken, @"^\d{5}$"))
                        {
                            LStrRentToken = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["C021"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        }
                        LDataRowSingleRent["C021"] = LStrRentToken;
                        LStrRentToken = LDataRowSingleRent["C021"].ToString();
                        AListStrRentExistObjects.Add(LStrRentToken);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("ObtainRentList()", ex.Message);
            }           
        }

        /// <summary>
        /// 得到所有的统计大项
        /// </summary>
        /// <param name="AListStatisticsCatogary"></param>
        /// <param name="AStrRent"></param>
        private void GetAllStatisticsCatogary(ref List<StatisticsCatogary> AListStatisticsCatogary,string AStrRent)
        {
            AListStatisticsCatogary.Clear();

            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT   *  FROM T_31_050_{0} WHERE C006 =1 AND C008=0", AStrRent);
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow dr in LDataTableReturn.Rows)
                    {
                        StatisticsCatogary statisticsCatogaryTemp = new StatisticsCatogary();
                        statisticsCatogaryTemp.StatisticsID = LongParse(dr["C001"].ToString(), 0);
                        statisticsCatogaryTemp.StatisticsName = dr["C002"].ToString();
                        statisticsCatogaryTemp.Active = IntParse(dr["C006"].ToString(), 1);
                        statisticsCatogaryTemp.SystemType = IntParse(dr["C007"].ToString(), 1);
                        statisticsCatogaryTemp.IsDelete = IntParse(dr["C008"].ToString(), 1);
                        statisticsCatogaryTemp.ValueType = IntParse(dr["C009"].ToString(), 0);
                        statisticsCatogaryTemp.IsCanCombin = IntParse(dr["C010"].ToString(), 0);
                        statisticsCatogaryTemp.MarkTable = IntParse(dr["C011"].ToString(), 0);
                        AListStatisticsCatogary.Add(statisticsCatogaryTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetAllStatisticsCatogary()", ex.Message);
            } 
        }

        /// <summary>
        /// 得到所有的统计小项
        /// </summary>
        /// <param name="AListStatisticsCatogary"></param>
        /// <param name="AStrRent"></param>
        private void GetAllStatisticsItem(ref List<StatisticsItem> AListStatisticsItem, string AStrRent) 
        {
            AListStatisticsItem.Clear();

            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT   *  FROM T_31_051_{0}", AStrRent);
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow dr in LDataTableReturn.Rows)
                    {
                        StatisticsItem statisticsItemTemp = new StatisticsItem();
                        statisticsItemTemp.StatisticsItemID = LongParse(dr["C001"].ToString(), 0);
                        statisticsItemTemp.StatisticsItemName = dr["C002"].ToString();
                        statisticsItemTemp.SourceTable = IntParse(dr["C003"].ToString(), 0);
                        statisticsItemTemp.IsCanCombine = IntParse(dr["C004"].ToString(), 0);
                        statisticsItemTemp.ShowType = IntParse(dr["C007"].ToString(), 0);
                        statisticsItemTemp.OrderID = IntParse(dr["C008"].ToString(), 0);
                        statisticsItemTemp.SystemType = IntParse(dr["C009"].ToString(), 0);
                        statisticsItemTemp.StatisticsID = LongParse(dr["C010"].ToString(), 0);
                        statisticsItemTemp.IsAVGOrStandarDev = IntParse(dr["C011"].ToString(), 0);
                        statisticsItemTemp.SliceTimeValue = IntParse(dr["C012"].ToString(), 0);
                        statisticsItemTemp.SliceTimeUnit = IntParse(dr["C013"].ToString(), 0);
                        AListStatisticsItem.Add(statisticsItemTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetAllStatisticsItem()", ex.Message);
            } 
        }

        /// <summary>
        /// 得到所有的梆定
        /// </summary>
        private void GetAllBinding( ref List<StatisticsMapping>  AlstStatisticsMapping,string AStrRent) 
        {
            AlstStatisticsMapping.Clear();

            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT   *  FROM T_31_052_{0} ORDER BY C002 ", AStrRent);
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow dr in LDataTableReturn.Rows)
                    {
                        StatisticsMapping statisticsMappingTemp= new StatisticsMapping();
                        statisticsMappingTemp.StatisticsMappingID= LongParse(dr["C001"].ToString(),0);
                        statisticsMappingTemp.StatisticsID = LongParse(dr["C002"].ToString(),0);
                        statisticsMappingTemp.OrgIDOrSkillID= LongParse(dr["C003"].ToString(),0);
                        statisticsMappingTemp.DropDown= IntParse(dr["C004"].ToString(),0);
                        statisticsMappingTemp.ApplayAll= IntParse(dr["C005"].ToString(),0);
                        statisticsMappingTemp.MappingColumnID= IntParse(dr["C008"].ToString(),0);
                        statisticsMappingTemp.FrequencyID= LongParse(dr["C009"].ToString(),0);
                        statisticsMappingTemp.UpdateValueTime = IntParse(dr["C010"].ToString(), 0);
                        statisticsMappingTemp.UpdateTimeUnit = IntParse(dr["C011"].ToString(), 0);
                        AlstStatisticsMapping.Add(statisticsMappingTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetAllBinding()", ex.Message);
            } 
        }

        /// <summary>
        /// 得到所有的机构
        /// </summary>
        /// <param name="AListOrgInfo"></param>
        /// <param name="AStrRent"></param>
        private void GetAllOrgInfo(ref List<OrgInfo> AListOrgInfo,string AStrRent)
        {
            AListOrgInfo.Clear();
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT * FROM T_11_006_{0} WHERE C005='1' "
                            , AStrRent);
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow LDataRowSingleRow in LDataTableReturn.Rows)
                    {
                        OrgInfo orgInfoTemp = new OrgInfo();
                        orgInfoTemp.ObjID = LongParse(LDataRowSingleRow["C001"].ToString(), 0);
                        orgInfoTemp.ObjName = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRow["C002"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        orgInfoTemp.ParentOrgID = LongParse(LDataRowSingleRow["C004"].ToString(), 0);
                        AListOrgInfo.Add(orgInfoTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetAllOrgInfo()", ex.Message);
            } 
        }

        /// <summary>
        /// 得到所有的用户信息
        /// </summary>
        /// <param name="AListUserInfo"></param>
        /// <param name="AStrRent"></param>
        private void GetAllUserInfo(ref List<UserInfo> AListUserInfo, string AStrRent)
        {
            AListUserInfo.Clear();
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT * FROM T_11_005_{0} WHERE C007<>'H' AND C010='1' AND C011='0' "
                            , AStrRent);
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow LDataRowSingleRow in LDataTableReturn.Rows)
                    {
                        UserInfo userInfoTemp = new UserInfo();
                        userInfoTemp.ObjID = LongParse(LDataRowSingleRow["C001"].ToString(), 0);
                        userInfoTemp.ObjName = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRow["C002"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        userInfoTemp.BeyondOrgID=LongParse(LDataRowSingleRow["C006"].ToString(),0);
                        AListUserInfo.Add(userInfoTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetAllUserInfo()", ex.Message);
            } 
        }

        /// <summary>
        /// 得到所有座席信息
        /// </summary>
        /// <param name="AListAgentInfo"></param>
        /// <param name="AStrRent"></param>
        private void GetAllAgentInfo(ref List<AgentInfo> AListAgentInfo,string AStrRent)
        {
            AListAgentInfo.Clear();
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 >= 1030000000000000000 AND C001 < 1040000000000000000 AND C002=1  AND C012='1' "
                            , AStrRent);
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow LDataRowSingleRow in LDataTableReturn.Rows)
                    {
                        AgentInfo agentInfoTemp = new AgentInfo();
                        agentInfoTemp.ObjID = LongParse(LDataRowSingleRow["C001"].ToString(), 0);
                        agentInfoTemp.BeyondOrgID = LongParse(LDataRowSingleRow["C011"].ToString(),0);
                        agentInfoTemp.ObjName = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRow["C017"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        AListAgentInfo.Add(agentInfoTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetAllAgentInfo()", ex.Message);
            }           
        }


        /// <summary>
        /// 得到该租户的月和周的设定
        /// 12010101每周开始于默认值为0
        /// 0为周日，1星期一，6为星期六
        /// 12010102每月开始于默认值为1
        /// 1为自然月,2为2号,最大28为28号
        /// 12010401 为分机和座席 E为分机 A为座席 E char(27)A为座席+分机 R为真实分机
        /// 
        /// </summary>
        private void   GetGlobalSetting(ref string AStrParamValue,string AStrRent,string AStrParamNumber) 
        {
            AStrParamValue = string.Empty;
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT * FROM T_11_001_{0} WHERE C003={1}"
                            , AStrRent
                            , AStrParamNumber);
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow LDataRowSingleRow in LDataTableReturn.Rows)
                    {
                        AStrParamValue = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRow["C006"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102).Trim(' ').Substring(AStrParamNumber.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetGlobalSetting()", ex.Message);
            }           
        }


        /// <summary>
        /// 得到技能组信息
        /// </summary>
        /// <param name="AListSkillInfo"></param>
        /// <param name="AStrRent"></param>
        private void GetSkillInfo(ref List<SkillInfo> AListSkillInfo, string AStrRent) 
        {
            AListSkillInfo.Clear();

            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL =string.Format("SELECT * FROM T_11_009_{0} WHERE C000 = 2 AND C004 = 1 ORDER BY C002"
                            , AStrRent);
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow LDataRowSingleRow in LDataTableReturn.Rows)
                    {
                        SkillInfo skillInfoTemp = new SkillInfo();
                        skillInfoTemp.SkillID = LongParse(LDataRowSingleRow["C001"].ToString(), 0);
                        skillInfoTemp.SkillName = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRow["C008"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        AListSkillInfo.Add(skillInfoTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetSkillInfo()", ex.Message);
            }           
        }


        /// <summary>
        /// 根据机构或技能组在T_11_201表查询它下面对应的用户和座席
        /// </summary>
        /// <param name="AObjID">机构或座席ID</param>
        /// <param name="AResourceCode">资源ID的前三位</param>
        /// <param name="AStrRent"></param>
        /// <param name="AListObjID">返回的用户ID和座席ID的list</param>
        private void GetObjInfoMapping( string AResourceCodeBegin,string AResourceCodeEnd, string AStrRent, ref List<long> AListObjMappingID)
        {
            AListObjMappingID.Clear();

            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;
            long LResourceCodeBegin = LongParse(AResourceCodeBegin + "0000000000000001", 0);
            long LResourceCodeEnd = LongParse(AResourceCodeEnd + "0000000000000001", 0);
            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 >={1} AND C003<{2} "
                            , AStrRent, LResourceCodeBegin, LResourceCodeEnd);
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow LDataRowSingleRow in LDataTableReturn.Rows)
                    {
                        long LLongObjMappingID = 0;
                        LLongObjMappingID = LongParse(LDataRowSingleRow["C004"].ToString(), 0);
                        AListObjMappingID.Add(LLongObjMappingID);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetObjInfoMapping()", ex.Message);
            } 
        }


        /// <summary>
        /// 根据梆定码和统计大项资源码得到全部的设定值
        /// C001
        /// 统计分析表(资源码311) 
        /// 统计分析关梆定到部门和技能组时(资源码312)
        /// C002
        /// 统计分析子项表(资源码314)
        /// </summary>
        /// <param name="ARent"></param>
        /// <param name="AResourceCodeBegin"></param>
        /// <param name="AResourceCodeEnd"></param>
        /// <returns></returns>
        private void GetStatisticsSetValue(string AStrRent, string AResourceCodeBegin, string AResourceCodeEnd,string AFieldName, ref List<StatisticsSetValue> AListStatisticsSetValue) 
        {
            AListStatisticsSetValue.Clear();

            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;
            long LResourceCodeBegin = LongParse(AResourceCodeBegin + "0000000000000001", 0);
            long LResourceCodeEnd = LongParse(AResourceCodeEnd + "0000000000000001", 0);
            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT * FROM T_31_044_{0} WHERE {3}>={1} AND {3}<{2} "
                            , AStrRent, LResourceCodeBegin, LResourceCodeEnd,AFieldName);
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow LDataRowSingleRow in LDataTableReturn.Rows)
                    {
                        StatisticsSetValue LStatisticsSetValueTemp = new StatisticsSetValue();
                        LStatisticsSetValueTemp.MappingIDOrStatisticsID = LongParse(LDataRowSingleRow["C001"].ToString(), 0);
                        LStatisticsSetValueTemp.StatisticsItemID = LongParse(LDataRowSingleRow["C002"].ToString(), 0);
                        LStatisticsSetValueTemp.IsStart = LDataRowSingleRow["C003"].ToString();
                        if (LDataRowSingleRow["C004"] != DBNull.Value)
                        {
                            LStatisticsSetValueTemp.SetValue004 = LDataRowSingleRow["C004"].ToString();
                        }
                        if (LDataRowSingleRow["C005"] != DBNull.Value)
                        {
                            LStatisticsSetValueTemp.SetValue004 = LDataRowSingleRow["C005"].ToString();
                        }
                        if (LDataRowSingleRow["C006"] != DBNull.Value)
                        {
                            LStatisticsSetValueTemp.SetValue004 = LDataRowSingleRow["C006"].ToString();
                        }
                        if (LDataRowSingleRow["C007"] != DBNull.Value)
                        {
                            LStatisticsSetValueTemp.SetValue004 = LDataRowSingleRow["C007"].ToString();
                        }
                        if (LDataRowSingleRow["C008"] != DBNull.Value)
                        {
                            LStatisticsSetValueTemp.SetValue004 = LDataRowSingleRow["C008"].ToString();
                        }
                        AListStatisticsSetValue.Add(LStatisticsSetValueTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetStatisticsSetValue()", ex.Message);
            } 
        }


        #region 根据租户Token、表名获取已经存在逻辑分区表
        private DataTable ObtainRentExistLogicPartitionTables(string AStrRentToken, string AStrTableName,ref List<string> AListStringTableName)
        {
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            AListStringTableName.Clear();
            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                if (IIntDatabaseType == 2)
                {
                    LStrDynamicSQL = "SELECT NAME AS TABLE_NAME FROM SYSOBJECTS WHERE NAME LIKE '" + AStrTableName + "_" + AStrRentToken + "_%' ORDER BY NAME ASC";
                }
                if (IIntDatabaseType == 3)
                {
                    LStrDynamicSQL = "SELECT TABLE_NAME FROM USER_TABLES WHERE TABLE_NAME LIKE '" + AStrTableName + "_" + AStrRentToken + "_%' ORDER BY TABLE_NAME ASC";
                }
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                   
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    string LStringTableName = string.Empty;
                    foreach (DataRow dr in LDataTableReturn.Rows)
                    {                        
                        LStringTableName = dr["TABLE_NAME"].ToString();                        
                        AListStringTableName.Add(LStringTableName);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("ObtainRentExistLogicPartitionTables()", "ERROR:"+ex.Message);
            }
            return LDataTableReturn;
        }
        #endregion

        #region  公用方法
        /// <summary>
        /// 从list取出适合标准差的记录
        /// </summary>
        /// <param name="AListObjInfoALL"></param>
        /// <param name="AListObjInfoNotFill"></param>
        /// <param name="AListObjInfoFill"></param>
        /// <param name="AStandValueSet"></param>
        public void GetNotFillStandardDeviation(List<ObjInfo> AListObjInfoALL, ref List<ObjInfo> AListObjInfoNotFill, ref List<ObjInfo> AListObjInfoFill, double AStandValueSet) 
        {
            double LDoubleSTDValue;
            double LDoubleAVGValue;

            if (AListObjInfoALL.Count > 0) 
            {
                LDoubleSTDValue = GetStandardDeviationValue(AListObjInfoALL);
                while (LDoubleSTDValue > AStandValueSet)
                {
                    AListObjInfoALL = AListObjInfoALL.OrderBy(p => p.ObjValue00).ToList();
                    LDoubleAVGValue = GetAVGValue(AListObjInfoALL);
                    ObjInfo LObjInfoMax = AListObjInfoALL[AListObjInfoALL.Count - 1];
                    ObjInfo LObjInfoMin = AListObjInfoALL[0];
                    //看谁离平均值最远
                    if (Math.Abs(LDoubleAVGValue - LObjInfoMax.ObjValue00) > Math.Abs(LDoubleAVGValue - LObjInfoMin.ObjValue00))
                    {
                        AListObjInfoNotFill.Add(LObjInfoMax);
                        AListObjInfoALL.Remove(LObjInfoMax);
                    }
                    else
                    {
                        //不适合的
                        AListObjInfoNotFill.Add(LObjInfoMin);
                        AListObjInfoALL.Remove(LObjInfoMin);
                    }
                    LDoubleSTDValue = GetStandardDeviationValue(AListObjInfoALL);
                }

                //适合的
                AListObjInfoFill = AListObjInfoALL;
            }
        }

        /// <summary>
        /// 计算出List的标准差
        /// </summary>
        /// <param name="AListObjInfoAll"></param>
        /// <returns></returns>
        public double GetStandardDeviationValue(List<ObjInfo> AListObjInfoAll)
        {
            double LDoubleSTD = 0;
            if(AListObjInfoAll.Count>0)
            {
                double LDoubleAVG = GetAVGValue(AListObjInfoAll);
                foreach (ObjInfo o in AListObjInfoAll)
                {
                    LDoubleSTD += Math.Pow((o.ObjValue00-LDoubleAVG), 2);
                }
                LDoubleSTD = LDoubleSTD / AListObjInfoAll.Count;

                LDoubleSTD = Math.Sqrt(LDoubleSTD);
            }
            return LDoubleSTD;
        }


        /// <summary>
        /// 得到List中ObjValue的平均值
        /// </summary>
        /// <param name="AListObjInfoAll"></param>
        /// <returns></returns>
        public double GetAVGValue(List<ObjInfo> AListObjInfoAll) 
        {
            double LDoubleAVG = 0;
            double LDoubleSUM = 0;
            if(AListObjInfoAll.Count>0)
            {
                foreach (ObjInfo o in AListObjInfoAll) 
                {
                    LDoubleSUM += o.ObjValue00;
                }
                LDoubleAVG = LDoubleSUM / AListObjInfoAll.Count;
            }
            return LDoubleAVG;
        }



        /// <summary>
        /// 创建主键
        /// </summary>
        /// <param name="listParams"></param>
        /// <param name="AStrRent"></param>
        /// <returns></returns>
        private OperationReturn GetSerialID(List<string> listParams, string AStrRent)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     模块编码
                //1     模块内编码
                //2     时间变量
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string moduleID = listParams[0];
                string resourceID = listParams[1];
                string dateFormat = listParams[2];
                string rentToken = AStrRent;
                string strSerialID = string.Empty;
                long errNumber = 0;
                string strErrMsg = string.Empty;
                switch (IIntDatabaseType)
                {
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@AInParam01",MssqlDataType.Varchar,2),
                            MssqlOperation.GetDbParameter("@AInParam02",MssqlDataType.Varchar,3),
                            MssqlOperation.GetDbParameter("@AInParam03",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@Ainparam04",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AOutParam01",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AOutErrorNumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@AOutErrorString",MssqlDataType.NVarchar,4000)
                        };
                        mssqlParameters[0].Value = moduleID;
                        mssqlParameters[1].Value = resourceID;
                        mssqlParameters[2].Value = rentToken;
                        mssqlParameters[3].Value = dateFormat;
                        mssqlParameters[4].Value = strSerialID;
                        mssqlParameters[5].Value = errNumber;
                        mssqlParameters[6].Value = strErrMsg;
                        mssqlParameters[4].Direction = ParameterDirection.Output;
                        mssqlParameters[5].Direction = ParameterDirection.Output;
                        mssqlParameters[6].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(IStrDatabaseProfile, "P_00_001",
                           mssqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (mssqlParameters[5].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[5].Value, mssqlParameters[6].Value);
                        }
                        else
                        {
                            strSerialID = mssqlParameters[4].Value.ToString();
                            optReturn.Data = strSerialID;
                        }
                        break;
                    case 3:
                        DbParameter[] orclParameters =
                        {
                            OracleOperation.GetDbParameter("AInParam01",OracleDataType.Varchar2,2),
                            OracleOperation.GetDbParameter("AInParam02",OracleDataType.Varchar2,3),
                            OracleOperation.GetDbParameter("AInParam03",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("Ainparam04",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("AOutParam01",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("AOutErrorNumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("AOutErrorString",OracleDataType.Nvarchar2,4000)
                        };
                        orclParameters[0].Value = moduleID;
                        orclParameters[1].Value = resourceID;
                        orclParameters[2].Value = rentToken;
                        orclParameters[3].Value = dateFormat;
                        orclParameters[4].Value = strSerialID;
                        orclParameters[5].Value = errNumber;
                        orclParameters[6].Value = strErrMsg;
                        orclParameters[4].Direction = ParameterDirection.Output;
                        orclParameters[5].Direction = ParameterDirection.Output;
                        orclParameters[6].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(IStrDatabaseProfile, "P_00_001",
                           orclParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (orclParameters[5].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", orclParameters[5].Value, orclParameters[6].Value);
                        }
                        else
                        {
                            strSerialID = orclParameters[4].Value.ToString();
                            optReturn.Data = strSerialID;
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", IIntDatabaseType);
                        return optReturn;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        private string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }

        private int IntParse(string str, int defaultValue)
        {
            int outRet = defaultValue;
            int.TryParse(str, out outRet);

            return outRet;
        }

        private long LongParse(string str, long defaultValue) 
        {
            long outRet = defaultValue;
            long.TryParse(str, out outRet);

            return outRet;
        }

        private double DoubleParse(string str, Double defaultValue) 
        {
            double outRet = defaultValue;
            double.TryParse(str, out outRet);
            return outRet;
        }

        private  DateTime StringToDateTime(string source)
        {
            if (source.Length < 14)
            {
                return DateTime.Parse("2100-1-1 00:00:00");
            }
            DateTime dt;
            string strTime = source.Substring(0, 4) + "-";
            strTime += source.Substring(4, 2) + "-";
            strTime += source.Substring(6, 2) + " ";
            strTime += source.Substring(8, 2) + ":";
            strTime += source.Substring(10, 2) + ":";
            strTime += source.Substring(12, 2);

            //dt = DateTime.Parse(strTime);
            dt = DateTime.Parse(DateTime.Parse(strTime).ToString("yyyy-MM-dd HH:mm:ss"));
            return dt;
        }

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
    }
}
