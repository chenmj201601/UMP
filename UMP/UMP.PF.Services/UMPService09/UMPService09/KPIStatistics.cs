using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using UMPService09.Model;
using UMPService09.Utility;
using UMPService09.Utility.InterFace;
using UMPService09.DAL;
using VoiceCyber.Common;
using UMPService09.Log;


namespace UMPService09
{
    public class KPIStatistics : BasicMethod, IStatistics
    {
        private DataBaseConfig IDataBaseConfig;

        //服务统计末时间
        private ServiceConfigInfo IServiceConfigInfo;

        //全局参数
        private GlobalSetting IGlobalSetting;

        //形成趋势的次数
        private int ITrendCyclesNum ;

        #region
        List<KPIDefine> LstKpiDefine ;
        List<KPIFormulaChar> LstKpiFormulaChar ;
        List<KPIFormulaColumn> LstKpiFormulaColumn;
        List<KPIMapping> LstKpiMapping;

        List<ObjectInfo> IListAgentInfo;
        List<ObjectInfo> IListOrgInfo;
        List<ObjectInfo> IListUserInfo;
        List<ObjectInfo> IListSkillInfo;
        List<ObjectInfo> IListExtension;
        #endregion
    
        /// <summary>
        /// 实际的统计
        /// </summary>
        public void DoAction(DataBaseConfig ADataBaseConfig, ServiceConfigInfo AServiceConfigInfo, GlobalSetting AGolbalSetting)
        {
            this.IDataBaseConfig = ADataBaseConfig;
            this.IServiceConfigInfo = AServiceConfigInfo;
            this.IGlobalSetting = AGolbalSetting;
            LstKpiDefine = new List<KPIDefine>();
            LstKpiFormulaChar = new List<KPIFormulaChar>();
            LstKpiFormulaColumn = new List<KPIFormulaColumn>();
            LstKpiMapping = new List<KPIMapping>();
            List<KPIMapping> lstKpiMappingTemp =new  List<KPIMapping>();
            DateTimeSplite datesplite = new DateTimeSplite();
            DALKPIDefine.GetAllKPIDefine(IDataBaseConfig, IGlobalSetting, ref LstKpiDefine);
            DALKPIFormulaChar.GetAllKPIFormulaChar(IDataBaseConfig, IGlobalSetting, ref LstKpiFormulaChar);
            DALKPIFormulaColumn.GetAllKPIFormulaColumn(IDataBaseConfig, IGlobalSetting, ref LstKpiFormulaColumn);
            DALKPIMapping.GetAllKPIMapping(IDataBaseConfig, IGlobalSetting, ref LstKpiMapping);
            ITrendCyclesNum = IntParse(ConfigurationManager.AppSettings["TrendCyclesNum"] ?? "4", 4);

           


            IListAgentInfo =new  List<ObjectInfo>();
            IListOrgInfo=new   List<ObjectInfo>();
            IListUserInfo=new   List<ObjectInfo>();
            IListSkillInfo = new List<ObjectInfo>();
            IListExtension = new List<ObjectInfo>();
            DALAgentInfo.GetAllAgentInfo(IDataBaseConfig, ref IListAgentInfo, IGlobalSetting);
            DALExtensionInfo.GetAllExtensionInfo(IDataBaseConfig, ref IListExtension, IGlobalSetting);
            DALOrgInfo.GetAllOrgInfo(IDataBaseConfig, ref IListOrgInfo, IGlobalSetting.StrRent);
            DALSkillInfo.GetSkillInfo(IDataBaseConfig, ref IListSkillInfo, IGlobalSetting.StrRent);
            DALUserInfo.GetAllUserInfo(IDataBaseConfig,IGlobalSetting, ref IListUserInfo);

            foreach (KPIDefine kpidefine in LstKpiDefine)
            {
                
                DateTimeSplite LDateTimeSplite = new DateTimeSplite();
                List<KPIFormulaChar> lstKPIFormulaCharTemp = new List<KPIFormulaChar>();
                List<KPIFormulaColumn> lstKPIFormulaColumnTemp = new List<KPIFormulaColumn>();

                //FileLog.WriteInfo("KPI名字", "" + kpidefine.KpiName);
                lstKpiMappingTemp = LstKpiMapping.Where(p => p.KpiID == kpidefine.KpiID).ToList();
                lstKPIFormulaCharTemp = LstKpiFormulaChar.Where(p => p.KpiID == kpidefine.KpiID).ToList();

                //得到公式拆分的值
                List<string> lstParam = new List<string>();
                lstParam = GetFieldNameList(kpidefine.NewFormula);

                if (lstParam.Count != lstKPIFormulaCharTemp.Count) { continue; }
                foreach(KPIFormulaChar  kChar in lstKPIFormulaCharTemp)
                {
                    KPIFormulaColumn kpicolumn = new KPIFormulaColumn();
                    kpicolumn = LstKpiFormulaColumn.Where(p => p.FormulaCharID == kChar.FormulaCharID).First() ?? null;
                    lstKPIFormulaColumnTemp.Add(kpicolumn);
                }

                if (lstParam.Count != lstKPIFormulaColumnTemp.Count) { continue; }

                //看当前的这个梆定属于什么周期，将周期的开始时间和结束时间算出来
                foreach (KPIMapping kpimapping in lstKpiMappingTemp)
                {
                    int LocalInt = kpimapping.ActualApplyCycle.IndexOf('1');
                    switch (LocalInt)
                    {
                        case 0 ://1、年
                            {
                                kpimapping.KpiSliceType = EnumKPISliceType.KYear;
                                LDateTimeSplite = GetCycleStartAndStopTime(IServiceConfigInfo.StartTime.Date, IGlobalSetting, 1);
                                while (LDateTimeSplite.StopStatisticsTime.Year<=DateTime.Now.Year)
                                {

                                    CalculateSingleKPI(kpimapping, LDateTimeSplite.StartStatisticsTime, LDateTimeSplite.StopStatisticsTime, lstKPIFormulaCharTemp, lstKPIFormulaColumnTemp, kpidefine.NewFormula, lstParam);
                                    LDateTimeSplite.StartStatisticsTime = LDateTimeSplite.StartStatisticsTime.AddYears(1);
                                    LDateTimeSplite.StopStatisticsTime = LDateTimeSplite.StopStatisticsTime.AddYears(1);
                                }
                            }
                            break;
                        case 1 ://2、月 
                            {
                                kpimapping.KpiSliceType = EnumKPISliceType.KMonth;
                                LDateTimeSplite = GetCycleStartAndStopTime(IServiceConfigInfo.StartTime.Date, IGlobalSetting, 2);
                                while(LDateTimeSplite.StopStatisticsTime<= DateTime.Now)
                                {
                                    CalculateSingleKPI(kpimapping, LDateTimeSplite.StartStatisticsTime, LDateTimeSplite.StopStatisticsTime, lstKPIFormulaCharTemp, lstKPIFormulaColumnTemp, kpidefine.NewFormula, lstParam);
                                    LDateTimeSplite.StartStatisticsTime = LDateTimeSplite.StartStatisticsTime.AddMonths(1);
                                    LDateTimeSplite.StopStatisticsTime = LDateTimeSplite.StopStatisticsTime.AddMonths(1);
                                }
                            }
                            break;
                        case 2 ://，3、周
                            {
                                kpimapping.KpiSliceType = EnumKPISliceType.KWeek;
                                LDateTimeSplite = GetCycleStartAndStopTime(IServiceConfigInfo.StartTime.Date, IGlobalSetting, 3);
                                while(LDateTimeSplite.StopStatisticsTime<=DateTime.Now)
                                {
                                    CalculateSingleKPI(kpimapping, LDateTimeSplite.StartStatisticsTime, LDateTimeSplite.StopStatisticsTime, lstKPIFormulaCharTemp, lstKPIFormulaColumnTemp, kpidefine.NewFormula, lstParam);
                                    LDateTimeSplite.StartStatisticsTime = LDateTimeSplite.StartStatisticsTime.AddDays(7);
                                    LDateTimeSplite.StopStatisticsTime = LDateTimeSplite.StopStatisticsTime.AddDays(7);
                                }
                            }
                            break;
                        case 3 ://，4、天 
                            { 
                                kpimapping.KpiSliceType = EnumKPISliceType.KDay;
                                LDateTimeSplite = GetCycleStartAndStopTime(IServiceConfigInfo.StartTime.Date, IGlobalSetting, 4);
                                while(LDateTimeSplite.StopStatisticsTime<=DateTime.Now)
                                {
                                    CalculateSingleKPI(kpimapping, LDateTimeSplite.StartStatisticsTime, LDateTimeSplite.StopStatisticsTime, lstKPIFormulaCharTemp, lstKPIFormulaColumnTemp, kpidefine.NewFormula, lstParam);
                                    LDateTimeSplite.StartStatisticsTime = LDateTimeSplite.StartStatisticsTime.AddDays(1);
                                    LDateTimeSplite.StopStatisticsTime = LDateTimeSplite.StopStatisticsTime.AddDays(1);
                                }
                                
                            }
                            break;
                        case 4 ://5、1小时 
                            {
                                kpimapping.KpiSliceType = EnumKPISliceType.KHour;
                                LDateTimeSplite = GetCycleStartAndStopTime(IServiceConfigInfo.StartTime.Date, IGlobalSetting, 5);
                                while (LDateTimeSplite.StopStatisticsTime <= DateTime.Now)
                                {                                   
                                    for (int i = 1; i <= 24;i++ )
                                    {
                                        CalculateSingleKPI(kpimapping, LDateTimeSplite.StartStatisticsTime.AddHours(i - 1), LDateTimeSplite.StartStatisticsTime.AddHours(i), lstKPIFormulaCharTemp, lstKPIFormulaColumnTemp, kpidefine.NewFormula, lstParam,i);
                                    }
                                    LDateTimeSplite.StartStatisticsTime = LDateTimeSplite.StartStatisticsTime.AddDays(1);
                                    LDateTimeSplite.StopStatisticsTime = LDateTimeSplite.StopStatisticsTime.AddDays(1);
                                }
                            }
                            break;
                        case 5 ://6、 30分钟 
                            {
                                kpimapping.KpiSliceType = EnumKPISliceType.K30M;
                                LDateTimeSplite = GetCycleStartAndStopTime(IServiceConfigInfo.StartTime.Date, IGlobalSetting, 6);
                                while (LDateTimeSplite.StopStatisticsTime <= DateTime.Now)
                                {
                                    for (int i = 1; i <= 48; i++) 
                                    {
                                        CalculateSingleKPI(kpimapping, LDateTimeSplite.StartStatisticsTime.AddMinutes((i - 1) * 30), LDateTimeSplite.StartStatisticsTime.AddMinutes(i * 30), lstKPIFormulaCharTemp, lstKPIFormulaColumnTemp, kpidefine.NewFormula, lstParam,i);
                                    }
                                    LDateTimeSplite.StartStatisticsTime = LDateTimeSplite.StartStatisticsTime.AddDays(1);
                                    LDateTimeSplite.StopStatisticsTime = LDateTimeSplite.StopStatisticsTime.AddDays(1);
                                }
                            }
                            break;
                        case 6 ://7 、15分钟 
                            {
                                kpimapping.KpiSliceType = EnumKPISliceType.K15M;
                                LDateTimeSplite = GetCycleStartAndStopTime(IServiceConfigInfo.StartTime.Date, IGlobalSetting, 7);
                                while (LDateTimeSplite.StopStatisticsTime <= DateTime.Now)
                                {
                                    for (int i = 1; i <= 96; i++)
                                    {
                                        CalculateSingleKPI(kpimapping, LDateTimeSplite.StartStatisticsTime.AddMinutes((i - 1) * 15), LDateTimeSplite.StartStatisticsTime.AddMinutes(i * 15), lstKPIFormulaCharTemp, lstKPIFormulaColumnTemp, kpidefine.NewFormula, lstParam,i);
                                    }
                                    LDateTimeSplite.StartStatisticsTime = LDateTimeSplite.StartStatisticsTime.AddDays(1);
                                    LDateTimeSplite.StopStatisticsTime = LDateTimeSplite.StopStatisticsTime.AddDays(1);
                                }
                            }
                            break;
                        case 7 ://8、10分钟  
                            {
                                kpimapping.KpiSliceType = EnumKPISliceType.K10M;
                                LDateTimeSplite = GetCycleStartAndStopTime(IServiceConfigInfo.StartTime.Date, IGlobalSetting, 8);
                                while (LDateTimeSplite.StopStatisticsTime <= DateTime.Now) 
                                {
                                    for (int i = 1; i <= 144; i++) 
                                    {
                                        CalculateSingleKPI(kpimapping, LDateTimeSplite.StartStatisticsTime.AddMinutes((i - 1) * 10), LDateTimeSplite.StartStatisticsTime.AddMinutes(i * 10), lstKPIFormulaCharTemp, lstKPIFormulaColumnTemp, kpidefine.NewFormula, lstParam,i);
                                    }
                                    LDateTimeSplite.StartStatisticsTime = LDateTimeSplite.StartStatisticsTime.AddDays(1);
                                    LDateTimeSplite.StopStatisticsTime = LDateTimeSplite.StopStatisticsTime.AddDays(1);
                                }
                            }
                            break;
                        case 8 ://19、 5分钟
                            {
                                kpimapping.KpiSliceType = EnumKPISliceType.K5M;
                                LDateTimeSplite = GetCycleStartAndStopTime(IServiceConfigInfo.StartTime.Date, IGlobalSetting, 9);
                                while (LDateTimeSplite.StopStatisticsTime <= DateTime.Now) 
                                {
                                    for (int i = 1; i <= 288;i++ )
                                    {
                                        CalculateSingleKPI(kpimapping, LDateTimeSplite.StartStatisticsTime.AddMinutes((i - 1) * 5), LDateTimeSplite.StartStatisticsTime.AddMinutes(i * 5), lstKPIFormulaCharTemp, lstKPIFormulaColumnTemp, kpidefine.NewFormula, lstParam,i);
                                    }
                                    LDateTimeSplite.StartStatisticsTime = LDateTimeSplite.StartStatisticsTime.AddDays(1);
                                    LDateTimeSplite.StopStatisticsTime = LDateTimeSplite.StopStatisticsTime.AddDays(1);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// //实际计算KPI
        /// </summary>
        /// <param name="AKPIMapping"></param>
        /// <param name="AStartTime"></param>
        /// <param name="AStopTime"></param>
        /// <param name="AListKPIFormulaCharTemp"></param>
        /// <param name="AListKPIFormulaColumnTemp"></param>
        /// <param name="ANewFormula">公式</param>
        /// <param name="AListStrChar">公式拆分出来的字母从左到右</param>
        private void CalculateSingleKPI(KPIMapping AKPIMapping, DateTime AStartTime, DateTime AStopTime, List<KPIFormulaChar> AListKPIFormulaCharTemp, List<KPIFormulaColumn> AListKPIFormulaColumnTemp, string ANewFormula,List<string> AListStrChar,int ASliceOrder=-1) 
        {  
            //初始化数据
            KPIStatisticsData kpistatistiscsdata = new KPIStatisticsData();
            InitPMKpiStatistics(ref kpistatistiscsdata,AKPIMapping,AStartTime);
            List<ObjectInfo> lstObjInfoTemp = new List<ObjectInfo>();
            List<string> lstStrIDTemp = new List<string>();
            double[] lstValues = new double[AListStrChar.Count];
            OperationReturn operationTemp=new OperationReturn();
            string LSeriID=string.Empty;

 #region
            if (AKPIMapping.ObjectID >= 1010000000000000001 && AKPIMapping.ObjectID < 1020000000000000000)//机构
            {

                    // 1座席 2分机  3用户 4真实分机 5机构  6 技能组
                    //机构座席
                    if (AKPIMapping.ActualApplyObjType.Substring(0, 1) == "1") //应用于座席
                    {
                        //得到部门下座席
                        GetAgentInfoInOrg(ref lstObjInfoTemp, AKPIMapping.ObjectID, AKPIMapping.IsDrop);                       
                    }

                     if (lstObjInfoTemp.Count>0)
                     {
                         GetListStrOfObjectID(lstObjInfoTemp, ref lstStrIDTemp);
                         operationTemp = DALCommon.InsertTempData(IDataBaseConfig, lstStrIDTemp, IGlobalSetting.StrRent);
                         if (!operationTemp.Result) { return; }
                         LSeriID=operationTemp.Data.ToString();
                         CalculateData(AKPIMapping, AStartTime, AStopTime, AListKPIFormulaCharTemp, AListKPIFormulaColumnTemp, ANewFormula, AListStrChar, 1, LSeriID, ref lstValues, ASliceOrder);
                     }



                    lstObjInfoTemp.Clear();
                    if (AKPIMapping.ActualApplyObjType.Substring(1, 1) == "1") //应用于分机
                    {
                        //得到部门下 分机
                        GetExtensionInfoInOrgORSkill(ref lstObjInfoTemp, AKPIMapping.ObjectID, AKPIMapping.IsDrop); 
                    }
                    if (lstObjInfoTemp.Count > 0)
                    {
                        GetListStrOfObjectID(lstObjInfoTemp, ref lstStrIDTemp);
                        operationTemp = DALCommon.InsertTempData(IDataBaseConfig, lstStrIDTemp, IGlobalSetting.StrRent);
                        if (!operationTemp.Result) { return; }
                        LSeriID = operationTemp.Data.ToString();
                        CalculateData(AKPIMapping, AStartTime, AStopTime, AListKPIFormulaCharTemp, AListKPIFormulaColumnTemp, ANewFormula, AListStrChar, 2, LSeriID, ref lstValues, ASliceOrder);
                    }


                    lstObjInfoTemp.Clear();
                    if (AKPIMapping.ActualApplyObjType.Substring(2, 1) == "1") //应用于用户
                    {
                        //得到部门下用户
                        GetUserInfoInOrgORSkill(ref lstObjInfoTemp, AKPIMapping.ObjectID, AKPIMapping.IsDrop); 
                    }
                    if (lstObjInfoTemp.Count > 0)
                    {
                        GetListStrOfObjectID(lstObjInfoTemp, ref lstStrIDTemp);
                        operationTemp = DALCommon.InsertTempData(IDataBaseConfig, lstStrIDTemp, IGlobalSetting.StrRent);
                        if (!operationTemp.Result) { return; }
                        LSeriID = operationTemp.Data.ToString();
                        CalculateData(AKPIMapping, AStartTime, AStopTime, AListKPIFormulaCharTemp, AListKPIFormulaColumnTemp, ANewFormula, AListStrChar, 3, LSeriID, ref lstValues, ASliceOrder);
                    }


                    if (AKPIMapping.ActualApplyObjType.Substring(3, 1) == "1") //应用于真实分机
                    {
                        if (IGlobalSetting.StrConfigAER == "R")
                        {
                            return;
                        }
                        else 
                        {
                            GetExtensionInfoInOrgORSkill(ref lstObjInfoTemp, AKPIMapping.ObjectID, AKPIMapping.IsDrop);                             
                            if (lstObjInfoTemp.Count>0)
                            {
                             GetListStrOfObjectID(lstObjInfoTemp, ref lstStrIDTemp);
                            operationTemp = DALCommon.InsertTempData(IDataBaseConfig, lstStrIDTemp, IGlobalSetting.StrRent);
                            if (!operationTemp.Result) { return; }
                            LSeriID = operationTemp.Data.ToString();
                            CalculateData(AKPIMapping, AStartTime, AStopTime, AListKPIFormulaCharTemp, AListKPIFormulaColumnTemp, ANewFormula, AListStrChar, 4, LSeriID, ref lstValues, ASliceOrder);
                            }
                        }
                    }
               
            }
            else if (AKPIMapping.ObjectID >= 1020000000000000001 && AKPIMapping.ObjectID < 1030000000000000000)//用户
            {
                lstStrIDTemp.Clear();
                lstStrIDTemp.Add(string.Empty);
                lstStrIDTemp.Add("1");
                lstStrIDTemp.Add( string.Format("{0}{1}", AKPIMapping.ObjectID.ToString(), AscCodeToChr(27)));
                operationTemp = DALCommon.InsertTempData(IDataBaseConfig, lstStrIDTemp, IGlobalSetting.StrRent);
                if (!operationTemp.Result) { return; }
                 LSeriID = operationTemp.Data.ToString();
                CalculateData(AKPIMapping, AStartTime, AStopTime, AListKPIFormulaCharTemp, AListKPIFormulaColumnTemp, ANewFormula, AListStrChar, 3, LSeriID, ref lstValues,ASliceOrder); 
            }
            else if(AKPIMapping.ObjectID >= 1030000000000000001 && AKPIMapping.ObjectID < 1040000000000000000 )//座席
            {
                lstStrIDTemp.Clear();
                lstStrIDTemp.Add(string.Empty);
                lstStrIDTemp.Add("1");
                lstStrIDTemp.Add(string.Format("{0}{1}", AKPIMapping.ObjectID.ToString(), AscCodeToChr(27)));
                lstStrIDTemp.Add(AKPIMapping.ObjectID.ToString());
                operationTemp = DALCommon.InsertTempData(IDataBaseConfig, lstStrIDTemp, IGlobalSetting.StrRent);
                if (!operationTemp.Result) { return; }
                 LSeriID = operationTemp.Data.ToString();
                CalculateData(AKPIMapping, AStartTime, AStopTime, AListKPIFormulaCharTemp, AListKPIFormulaColumnTemp, ANewFormula, AListStrChar, 1, LSeriID, ref lstValues, ASliceOrder); 
            }
            else if (AKPIMapping.ObjectID >= 1040000000000000001 && AKPIMapping.ObjectID < 1050000000000000000) //分机
            {
                if (IGlobalSetting.StrConfigAER == "R")  //此时分组方式为真实分机,不统计分机
                {
                    return;
                }
                else
                {
                    lstStrIDTemp.Clear();
                    lstStrIDTemp.Add(string.Empty);
                    lstStrIDTemp.Add("1");
                    lstStrIDTemp.Add(string.Format("{0}{1}", AKPIMapping.ObjectID.ToString(), AscCodeToChr(27)));
                    lstStrIDTemp.Add(AKPIMapping.ObjectID.ToString());
                     operationTemp = DALCommon.InsertTempData(IDataBaseConfig, lstStrIDTemp, IGlobalSetting.StrRent);
                    if (!operationTemp.Result) { return; }
                     LSeriID = operationTemp.Data.ToString();
                    CalculateData(AKPIMapping, AStartTime, AStopTime, AListKPIFormulaCharTemp, AListKPIFormulaColumnTemp, ANewFormula, AListStrChar, 2, LSeriID, ref lstValues, ASliceOrder); 
                }
            }
            else if (AKPIMapping.ObjectID >= 1050000000000000001 && AKPIMapping.ObjectID < 1060000000000000000) //真实分机
            {
                lstStrIDTemp.Clear();
                lstStrIDTemp.Add(string.Empty);
                lstStrIDTemp.Add("1");
                lstStrIDTemp.Add(string.Format("{0}{1}", AKPIMapping.ObjectID.ToString(), AscCodeToChr(27)));
                lstStrIDTemp.Add(AKPIMapping.ObjectID.ToString());
                operationTemp = DALCommon.InsertTempData(IDataBaseConfig, lstStrIDTemp, IGlobalSetting.StrRent);
                if (!operationTemp.Result) { return; }
                 LSeriID = operationTemp.Data.ToString();
                CalculateData(AKPIMapping, AStartTime, AStopTime, AListKPIFormulaCharTemp, AListKPIFormulaColumnTemp, ANewFormula, AListStrChar, 4, LSeriID, ref lstValues, ASliceOrder); 
            }
            else if (AKPIMapping.ObjectID >= 9060000000000000001 && AKPIMapping.ObjectID < 9070000000000000000)//技能组
            {
                //统计对象类型（1:分机；2:座席；3:技能组； 4机构 5 用户）
                //机构座席
                if (AKPIMapping.ActualApplyObjType.Substring(0, 1) == "1") //应用于座席
                {
                    //得到部门下座席
                    GetAgentInfoInOrg(ref lstObjInfoTemp, AKPIMapping.ObjectID, 1);
                }
                if (lstObjInfoTemp.Count > 0)
                {
                    GetListStrOfObjectID(lstObjInfoTemp, ref lstStrIDTemp);
                     operationTemp = DALCommon.InsertTempData(IDataBaseConfig, lstStrIDTemp, IGlobalSetting.StrRent);
                    if (!operationTemp.Result) { return; }
                     LSeriID = operationTemp.Data.ToString();
                    CalculateData(AKPIMapping, AStartTime, AStopTime, AListKPIFormulaCharTemp, AListKPIFormulaColumnTemp, ANewFormula, AListStrChar,1, LSeriID, ref lstValues, ASliceOrder);
                 }


                lstObjInfoTemp.Clear();
                if (AKPIMapping.ActualApplyObjType.Substring(1, 1) == "1") //应用于分机
                {
                    //得到部门下 分机
                    GetExtensionInfoInOrgORSkill(ref lstObjInfoTemp, AKPIMapping.ObjectID, 1);
                }
                if (lstObjInfoTemp.Count > 0)
                {
                    GetListStrOfObjectID(lstObjInfoTemp, ref lstStrIDTemp);
                    operationTemp = DALCommon.InsertTempData(IDataBaseConfig, lstStrIDTemp, IGlobalSetting.StrRent);
                    if (!operationTemp.Result) { return; }
                    LSeriID = operationTemp.Data.ToString();
                    CalculateData(AKPIMapping, AStartTime, AStopTime, AListKPIFormulaCharTemp, AListKPIFormulaColumnTemp, ANewFormula, AListStrChar, 2, LSeriID, ref lstValues, ASliceOrder);
                }


                lstObjInfoTemp.Clear();
                if (AKPIMapping.ActualApplyObjType.Substring(2, 1) == "1") //应用于用户
                {
                    //得到部门下用户
                    GetUserInfoInOrgORSkill(ref lstObjInfoTemp, AKPIMapping.ObjectID, 1);
                }
                if (lstObjInfoTemp.Count > 0)
                {
                    GetListStrOfObjectID(lstObjInfoTemp, ref lstStrIDTemp);
                    operationTemp = DALCommon.InsertTempData(IDataBaseConfig, lstStrIDTemp, IGlobalSetting.StrRent);
                    if (!operationTemp.Result) { return; }
                    LSeriID = operationTemp.Data.ToString();
                    CalculateData(AKPIMapping, AStartTime, AStopTime, AListKPIFormulaCharTemp, AListKPIFormulaColumnTemp, ANewFormula, AListStrChar, 3, LSeriID, ref lstValues, ASliceOrder);
                }


                if (AKPIMapping.ActualApplyObjType.Substring(3, 1) == "1") //应用于真实分机
                {
                    if (IGlobalSetting.StrConfigAER == "R")
                    {
                        return;
                    }
                    else
                    {
                        GetExtensionInfoInOrgORSkill(ref lstObjInfoTemp, AKPIMapping.ObjectID, 1);
                        if (lstObjInfoTemp.Count > 0)
                        {
                            GetListStrOfObjectID(lstObjInfoTemp, ref lstStrIDTemp);
                            operationTemp = DALCommon.InsertTempData(IDataBaseConfig, lstStrIDTemp, IGlobalSetting.StrRent);
                            if (!operationTemp.Result) { return; }
                            LSeriID = operationTemp.Data.ToString();
                            CalculateData(AKPIMapping, AStartTime, AStopTime, AListKPIFormulaCharTemp, AListKPIFormulaColumnTemp, ANewFormula, AListStrChar, 4, LSeriID, ref lstValues, ASliceOrder);
                        }
                    }
                }
               
            }
            else
            {
                return;
            }

#endregion


            //整理公式里的值
            NEval target = new NEval();
            string[] lstValuesTemp = new string[AListStrChar.Count];
            for (int i = 0; i < lstValues.Length; i++) 
            {
                lstValuesTemp[i] = lstValues[i].ToString();
            }
            if (lstValuesTemp.Length == 1)
            {

                kpistatistiscsdata.ActualValue = DecimalParse( lstValuesTemp[0],0);
            }
            else 
            {

                kpistatistiscsdata.ActualValue = target.Eval(ANewFormula, lstValuesTemp);
            }

                

            kpistatistiscsdata.ActualCompareGoal1 = CompareSign(kpistatistiscsdata.Goal1, kpistatistiscsdata.ActualValue, kpistatistiscsdata.CompareSign1);
            
            
            decimal Trend1 = 2;//连续几期上升形成一个趋势  0表示不变 1表示上升 -1表示下降  2表示n/a
            decimal ComparePrior = 0; //和上期比较 提高多少减少多少 有正负值

            //得到连续几期的数据  要根据时间换出orderID  
            List<KPIStatisticsData> lstKpiStatisticsTemp = new List<KPIStatisticsData>();
            GetContinuousData( ref lstKpiStatisticsTemp, AStartTime, AKPIMapping, AKPIMapping.ObjectID);
            CalculateTrendAndComparePrior(lstKpiStatisticsTemp, kpistatistiscsdata.CompareSign1, kpistatistiscsdata.ActualCompareGoal1, ref Trend1, ref ComparePrior);
            kpistatistiscsdata.Trend1 = Trend1;
            kpistatistiscsdata.ComparePrior = ComparePrior;


            //如果是写到第一张表的数据要写
            TimeSpan ts= AStopTime-AStartTime;
            if(ts.TotalHours<24)
            {
                int ARowID = -1;
                int AColumnNumber = -1;

                GetRowIDAndColumnNumber(AKPIMapping.KpiSliceType, AStartTime, ref ARowID, ref AColumnNumber);
                kpistatistiscsdata.RowID = ARowID;
                kpistatistiscsdata.ColumnOrder = AColumnNumber;
            }
           

            #region            //存数据
            switch (AKPIMapping.KpiSliceType)
            {
                case EnumKPISliceType.KYear:
                    DALKPIStatistics.InsertKpiStatisticsSliceData(IDataBaseConfig, IGlobalSetting, 5, kpistatistiscsdata);
                    break;
                case EnumKPISliceType.KMonth:
                    DALKPIStatistics.InsertKpiStatisticsSliceData(IDataBaseConfig, IGlobalSetting, 4, kpistatistiscsdata);
                    break;
                case EnumKPISliceType.KWeek:
                    DALKPIStatistics.InsertKpiStatisticsSliceData(IDataBaseConfig, IGlobalSetting, 3, kpistatistiscsdata);
                    break;
                case EnumKPISliceType.KDay:
                    DALKPIStatistics.InsertKpiStatisticsSliceData(IDataBaseConfig, IGlobalSetting, 2, kpistatistiscsdata);
                    break;
                case EnumKPISliceType.KHour:
                case EnumKPISliceType.K30M:
                case EnumKPISliceType.K15M:
                case EnumKPISliceType.K10M:
                case EnumKPISliceType.K5M:
                    DALKPIStatistics.InsertKpiStatisticsSliceData(IDataBaseConfig, IGlobalSetting, 1, kpistatistiscsdata);
                    break;
                default:
                    break;
            }
            #endregion
        }

        /// <summary>
        /// 得到连续向上连续几期的数据
        /// </summary>
        /// <param name="AListKPIStatistiscData"></param>
        /// <param name="AstartTime"></param>
        /// <param name="AKpiMapping"></param>
        private void GetContinuousData(ref List<KPIStatisticsData> AListKPIStatistiscData, DateTime AstartTime, KPIMapping AKpiMapping, long AObjectID) 
        {
            AListKPIStatistiscData.Clear();
            KPIStatisticsData kpistatisticsdatatemp = new KPIStatisticsData();
            for (int i =1; i <= ITrendCyclesNum;i++ )
            {
                kpistatisticsdatatemp = new KPIStatisticsData();
                int ARowID = -1;
                int ASliceType = -1;
                int AColumnNumber = -1;
                //要算出来
                switch (AKpiMapping.KpiSliceType)
                {
                    case EnumKPISliceType.KYear:
                        {
                            kpistatisticsdatatemp = DALKPIStatistics.SelectKPIStatisticsData(IDataBaseConfig, IGlobalSetting, 5, AKpiMapping.KPIMappingID, LongParse(AstartTime.AddYears(-1*i).ToString("yyyyMMdd000000"), 0), AObjectID);
                            if (kpistatisticsdatatemp != null)
                            {
                                AListKPIStatistiscData.Add(kpistatisticsdatatemp);
                            }
                        }break;
                    case EnumKPISliceType.KMonth:
                        {
                            kpistatisticsdatatemp = DALKPIStatistics.SelectKPIStatisticsData(IDataBaseConfig, IGlobalSetting, 4, AKpiMapping.KPIMappingID, LongParse(AstartTime.AddMonths(-1*i).ToString("yyyyMMdd000000"), 0), AObjectID);
                            if (kpistatisticsdatatemp != null)
                            {
                                AListKPIStatistiscData.Add(kpistatisticsdatatemp);
                            }
                        }break;
                    case EnumKPISliceType.KWeek:
                         {
                            kpistatisticsdatatemp = DALKPIStatistics.SelectKPIStatisticsData(IDataBaseConfig, IGlobalSetting, 3, AKpiMapping.KPIMappingID, LongParse(AstartTime.AddDays(-7*i).ToString("yyyyMMdd000000"), 0), AObjectID);
                            if (kpistatisticsdatatemp != null)
                            {
                                AListKPIStatistiscData.Add(kpistatisticsdatatemp);
                            }
                        }break;
                    case EnumKPISliceType.KDay:
                        {
                            kpistatisticsdatatemp = DALKPIStatistics.SelectKPIStatisticsData(IDataBaseConfig, IGlobalSetting, 2, AKpiMapping.KPIMappingID, LongParse(AstartTime.AddDays(-1*i).ToString("yyyyMMdd000000"), 0), AObjectID);
                            if (kpistatisticsdatatemp != null)
                            {
                                AListKPIStatistiscData.Add(kpistatisticsdatatemp);
                            }
                        }
                        break;
                    case EnumKPISliceType.KHour:
                        {
                            DateTime dateTimeStartTemp = AstartTime.AddHours(-1*i);
                            //算出这个时间是这一天中的第几个点
                            ASliceType = 5;
                            GetRowIDAndColumnNumber(AKpiMapping.KpiSliceType, dateTimeStartTemp, ref ARowID, ref AColumnNumber);
                            kpistatisticsdatatemp = DALKPIStatistics.SelectKPIStatisticsData(IDataBaseConfig, IGlobalSetting, 1, AKpiMapping.KPIMappingID, LongParse(dateTimeStartTemp.ToString("yyyyMMdd000000"), 0), AObjectID, ARowID, ASliceType, AColumnNumber);
                            if (kpistatisticsdatatemp != null)
                            {
                                AListKPIStatistiscData.Add(kpistatisticsdatatemp);
                            }
                        }
                        break;
                    case EnumKPISliceType.K30M:
                        {
                            ASliceType = 6;
                            DateTime dateTimeStartTemp = AstartTime.AddMinutes(-30 * i);
                            GetRowIDAndColumnNumber(AKpiMapping.KpiSliceType, dateTimeStartTemp, ref ARowID, ref AColumnNumber);
                            kpistatisticsdatatemp = DALKPIStatistics.SelectKPIStatisticsData(IDataBaseConfig, IGlobalSetting, 1, AKpiMapping.KPIMappingID, LongParse(dateTimeStartTemp.ToString("yyyyMMdd000000"), 0), AObjectID, ARowID, ASliceType, AColumnNumber);
                            if (kpistatisticsdatatemp != null)
                            {
                                AListKPIStatistiscData.Add(kpistatisticsdatatemp);
                            }
                        }
                        break;
                    case EnumKPISliceType.K15M: 
                        {
                            ASliceType = 7;
                            DateTime dateTimeStartTemp = AstartTime.AddMinutes(-15 * i);
                            GetRowIDAndColumnNumber(AKpiMapping.KpiSliceType, dateTimeStartTemp, ref ARowID, ref AColumnNumber);
                            kpistatisticsdatatemp = DALKPIStatistics.SelectKPIStatisticsData(IDataBaseConfig, IGlobalSetting, 1, AKpiMapping.KPIMappingID, LongParse(dateTimeStartTemp.ToString("yyyyMMdd000000"), 0), AObjectID, ARowID, ASliceType, AColumnNumber);
                            if (kpistatisticsdatatemp !=null)
                            {
                                AListKPIStatistiscData.Add(kpistatisticsdatatemp);
                            }
                        }
                        break;
                    case EnumKPISliceType.K10M: 
                        {
                            ASliceType = 8;
                            DateTime dateTimeStartTemp = AstartTime.AddMinutes(-10 * i);
                            GetRowIDAndColumnNumber(AKpiMapping.KpiSliceType, dateTimeStartTemp, ref ARowID, ref AColumnNumber);
                            kpistatisticsdatatemp = DALKPIStatistics.SelectKPIStatisticsData(IDataBaseConfig, IGlobalSetting, 1, AKpiMapping.KPIMappingID, LongParse(dateTimeStartTemp.ToString("yyyyMMdd000000"), 0), AObjectID, ARowID, ASliceType, AColumnNumber);
                            if (kpistatisticsdatatemp != null)
                            {
                                AListKPIStatistiscData.Add(kpistatisticsdatatemp);
                            }
                        }
                        break;
                    case EnumKPISliceType.K5M:
                        {
                            ASliceType = 9;
                            DateTime dateTimeStartTemp = AstartTime.AddMinutes(-5 * i);
                            GetRowIDAndColumnNumber(AKpiMapping.KpiSliceType, dateTimeStartTemp, ref ARowID, ref AColumnNumber);
                            kpistatisticsdatatemp = DALKPIStatistics.SelectKPIStatisticsData(IDataBaseConfig, IGlobalSetting, 1, AKpiMapping.KPIMappingID, LongParse(dateTimeStartTemp.ToString("yyyyMMdd000000"), 0), AObjectID, ARowID, ASliceType, AColumnNumber);
                            if (kpistatisticsdatatemp != null)
                            {
                                AListKPIStatistiscData.Add(kpistatisticsdatatemp);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void GetRowIDAndColumnNumber(EnumKPISliceType AKPISliceType,DateTime AStartTime,ref int ARowID ,ref int AColumnNumber) 
        {
            DateTime dateTimeStartTemp = AStartTime.Date;
            TimeSpan TimeSpanTemp = AStartTime - dateTimeStartTemp;
            int TotalMinutes = (int)TimeSpanTemp.TotalMinutes;
            int LIntTemp = -1;
            switch (AKPISliceType)
            {
                case EnumKPISliceType.KHour: 
                    {
                        LIntTemp = TotalMinutes / 60;

                        ARowID = LIntTemp / 12 + 1;
                        AColumnNumber = LIntTemp % 12 + 1;
                    }
                    break;
                case EnumKPISliceType.K30M:
                    {
                        LIntTemp = TotalMinutes / 30;

                        ARowID = LIntTemp / 12 +1;
                        AColumnNumber = LIntTemp % 12+1;
                    }
                    break;
                case EnumKPISliceType.K15M: 
                    {
                        LIntTemp = TotalMinutes / 15;
                        ARowID = LIntTemp / 12 + 1;
                        AColumnNumber = LIntTemp % 12+1;
                    }
                    break;
                case EnumKPISliceType.K10M: 
                    {
                        LIntTemp = TotalMinutes / 10;
                        ARowID = LIntTemp / 12 + 1;
                        AColumnNumber = LIntTemp % 12+1;
                    }
                    break;
                case EnumKPISliceType.K5M:
                    {
                        LIntTemp = TotalMinutes / 5;
                        ARowID = LIntTemp / 12 + 1;
                        AColumnNumber = LIntTemp % 12+1;
                    }
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// 得到趋势和与上一期实际值比较的结果
        /// </summary>
        /// <param name="AListKPIStatistiscData">前面几期的的统计结果</param>
        /// <param name="ACompareSign">比较符号</param>
        /// <param name="AActural">本期实际值</param>
        /// <param name="ATrend">返回趋势</param>
        /// <param name="AComparePrior">返回与上期的比较结果</param>
        private void CalculateTrendAndComparePrior(List<KPIStatisticsData> AListKPIStatistiscData, string ACompareSign,decimal AActural, ref decimal ATrend, ref decimal AComparePrior) 
        {
            if (AListKPIStatistiscData != null && AListKPIStatistiscData.Count >= 1)
            {
                //和上期比较进步了多少还是下降了多少
                AComparePrior = CompareSign(AListKPIStatistiscData[0].ActualValue, AActural, ACompareSign);

                List<decimal> lstComparePrior = new List<decimal>();
                lstComparePrior.Add(AComparePrior);
                foreach (KPIStatisticsData kk in AListKPIStatistiscData)
                {
                    lstComparePrior.Add(kk.ComparePrior);
                }
                ATrend = CalculateValueTrend(lstComparePrior, ITrendCyclesNum);
            }
            else 
            {
                ATrend = 2;
                AComparePrior = 0;
            }
        }




       /// <summary>
       /// 取实际数字算实际值        
       /// </summary>
       /// <param name="AKPIMapping"></param>
       /// <param name="AStartTime"></param>
       /// <param name="AStopTime"></param>
       /// <param name="AListKPIFormulaCharTemp"></param>
       /// <param name="AListKPIFormulaColumnTemp"></param>
       /// <param name="ANewFormula"></param>
       /// <param name="AListStrChar"></param>
        /// <param name="AObjectType">// 1座席 2分机  3用户 4真实分机 5机构  6 技能组</param>
       /// <param name="ObjectSerialID">对象的存到临时表的主键ID</param>
       /// <param name="lstValues"></param>
        private void CalculateData(KPIMapping AKPIMapping, DateTime AStartTime, DateTime AStopTime, List<KPIFormulaChar> AListKPIFormulaCharTemp, List<KPIFormulaColumn> AListKPIFormulaColumnTemp, string ANewFormula, List<string> AListStrChar, int AObjectType, string ObjectSerialID, ref double[] lstValues,int ASliceOrder) 
        {
            int i=0;
            foreach(string StrChar in AListStrChar)
            {
                long ColumnID = AListKPIFormulaCharTemp.Where(p => p.MappingChar == StrChar).Count() > 0 ?
                    AListKPIFormulaCharTemp.Where(p => p.MappingChar == StrChar).First().FormulaCharID : 0;
                if (ColumnID == 0) { return; }
                KPIFormulaColumn kpiformulacolumntemp = AListKPIFormulaColumnTemp.Where(p => p.FormulaCharID == ColumnID).Count() > 0 ?
                    AListKPIFormulaColumnTemp.Where(p => p.FormulaCharID == ColumnID).First() : null;
                if (kpiformulacolumntemp == null) { return ; }
                if (kpiformulacolumntemp.ColumnSource == 2)//存储过程 
                {
                    return ;
                }
                else if (kpiformulacolumntemp.ColumnSource == 3)//常量
                {
                    lstValues[i] =DoubleParse( kpiformulacolumntemp.ColumnName,0);
                }
                else if (kpiformulacolumntemp.ColumnSource == 1)//表里的统计值要除以1000
                {
                    string TableName = kpiformulacolumntemp.ApplayName;
                    string ColumnName = kpiformulacolumntemp.ColumnName;
                    string SpecialObjectTypeNumber = kpiformulacolumntemp.SpecialObjectTypeNumber.ToString();//专用于录音和CTI数据小于天的切片时的数据统计
                    switch (AKPIMapping.KpiSliceType)
                    {                        
                        case EnumKPISliceType.KYear:
                            {
                                TableName = string.Format(TableName.Substring(0, 7) + "{0}", 5);
                                lstValues[i] += DALCommon.GetStatisticsValueDayUp(IDataBaseConfig, IGlobalSetting, TableName, ColumnName, AObjectType, AStartTime.ToString("yyyyMMdd000000"), AStopTime.ToString("yyyyMMdd000000"), ObjectSerialID)/1000;
                            }
                            break;
                        case EnumKPISliceType.KMonth:
                            {
                                TableName = string.Format(TableName.Substring(0, 7) + "{0}", 4);
                                lstValues[i] += DALCommon.GetStatisticsValueDayUp(IDataBaseConfig, IGlobalSetting, TableName, ColumnName, AObjectType, AStartTime.ToString("yyyyMMdd000000"), AStopTime.ToString("yyyyMMdd000000"), ObjectSerialID)/1000;
                            }
                            break;
                        case EnumKPISliceType.KWeek:
                            {
                                TableName = string.Format(TableName.Substring(0, 7) + "{0}", 3);
                                lstValues[i] += DALCommon.GetStatisticsValueDayUp(IDataBaseConfig, IGlobalSetting, TableName, ColumnName, AObjectType, AStartTime.ToString("yyyyMMdd000000"), AStopTime.ToString("yyyyMMdd000000"), ObjectSerialID)/1000;
                            } 
                            break;
                        case EnumKPISliceType.KDay:
                            {
                                TableName = string.Format(TableName.Substring(0, 7) + "{0}", 2);
                                lstValues[i] += DALCommon.GetStatisticsValueDayUp(IDataBaseConfig, IGlobalSetting, TableName, ColumnName, AObjectType, AStartTime.ToString("yyyyMMdd000000"), AStopTime.ToString("yyyyMMdd000000"), ObjectSerialID)/1000;
                            } 
                            break;
                        case EnumKPISliceType.KHour:
                            {
                                TableName = string.Format(TableName.Substring(0, 7) + "{0}", 1);
                                //得到存储过程的参数
                                ColumnName = string.Empty;
                                GetColumnNameOfSlice(ref ColumnName, EnumKPISliceType.KHour, ASliceOrder);
                                lstValues[i] += DALCommon.GetStatisticsValueDayDown(IDataBaseConfig, IGlobalSetting, TableName, ColumnName, AObjectType, AStartTime.Date.ToString("yyyyMMdd000000"), AStartTime.Date.AddDays(1).ToString("yyyyMMdd000000"), ObjectSerialID, kpiformulacolumntemp) / 1000;
                            }
                            break;
                        case EnumKPISliceType.K30M:
                            {
                                TableName = string.Format(TableName.Substring(0, 7) + "{0}", 1);
                                ColumnName = string.Empty;
                                GetColumnNameOfSlice(ref ColumnName, EnumKPISliceType.K30M, ASliceOrder);
                                lstValues[i] += DALCommon.GetStatisticsValueDayDown(IDataBaseConfig, IGlobalSetting, TableName, ColumnName, AObjectType, AStartTime.Date.ToString("yyyyMMdd000000"), AStartTime.Date.AddDays(1).ToString("yyyyMMdd000000"), ObjectSerialID, kpiformulacolumntemp) / 1000;

                            }
                            break;
                        case EnumKPISliceType.K15M:
                            {
                                TableName = string.Format(TableName.Substring(0, 7) + "{0}", 1);
                                ColumnName = string.Empty;
                                GetColumnNameOfSlice(ref ColumnName, EnumKPISliceType.K15M, ASliceOrder);
                                lstValues[i] += DALCommon.GetStatisticsValueDayDown(IDataBaseConfig, IGlobalSetting, TableName, ColumnName, AObjectType, AStartTime.Date.ToString("yyyyMMdd000000"), AStartTime.Date.AddDays(1).ToString("yyyyMMdd000000"), ObjectSerialID, kpiformulacolumntemp) / 1000;

                            }
                            break;
                        case EnumKPISliceType.K10M:
                            {
                                TableName = string.Format(TableName.Substring(0, 7) + "{0}", 1);
                                ColumnName = string.Empty;
                                GetColumnNameOfSlice(ref ColumnName, EnumKPISliceType.K10M, ASliceOrder);
                                lstValues[i] += DALCommon.GetStatisticsValueDayDown(IDataBaseConfig, IGlobalSetting, TableName, ColumnName, AObjectType, AStartTime.Date.ToString("yyyyMMdd000000"), AStartTime.Date.AddDays(1).ToString("yyyyMMdd000000"), ObjectSerialID, kpiformulacolumntemp) / 1000;

                            }
                            break;
                        case EnumKPISliceType.K5M:
                            {
                                TableName = string.Format(TableName.Substring(0, 7) + "{0}", 1); 
                                ColumnName = string.Empty;
                                GetColumnNameOfSlice(ref ColumnName, EnumKPISliceType.K5M, ASliceOrder);
                                lstValues[i] += DALCommon.GetStatisticsValueDayDown(IDataBaseConfig, IGlobalSetting, TableName, ColumnName, AObjectType, AStartTime.Date.ToString("yyyyMMdd000000"), AStartTime.Date.AddDays(1).ToString("yyyyMMdd000000"), ObjectSerialID, kpiformulacolumntemp) / 1000;
                            }
                            break;
                        default:
                            break;
                    }
                }
                i++;
            }
        }

        /// <summary>
        /// 返回的列名语句以逗号隔开
        /// </summary>
        /// <param name="AStrWhere">返回的列名语句以逗号隔开</param>
        /// <param name="ASliceType"> 分段类别</param>
        /// <param name="AOrderID">第几段</param>
        private void GetColumnNameOfSlice(ref string AStrWhere, EnumKPISliceType ASliceType, int AOrderID)
        {
            AStrWhere = string.Empty;
            switch (ASliceType)
            {
                case EnumKPISliceType.KHour:
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
                case EnumKPISliceType.K30M:
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
                case EnumKPISliceType.K15M:
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
                case EnumKPISliceType.K10M:
                    {
                        int tempH = AOrderID / 6;
                        int tempM = AOrderID % 6;
                        if (tempH > 0 && tempM == 0)
                        {
                            tempH--;
                            tempM = 6;
                        }
                        int temp10M = (tempM - 1) * 10;
                        AStrWhere += "C" + tempH.ToString("00") + temp10M.ToString("00");
                        AStrWhere += "," + "C" + tempH.ToString("00") + (temp10M + 5).ToString("00");
                    }
                    break;
                case EnumKPISliceType.K5M:
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
                default:
                    break;
            }
        }

        #region 得到机构或技能组下的对象

        /// <summary>
        /// 得到机构或技能组下用户
        /// </summary>
        /// <param name="AListAgentInfo"></param>
        /// <param name="AOrgIDOrSkillID"></param>
        /// <param name="AType"></param>
        private void GetUserInfoInOrgORSkill(ref List<ObjectInfo> AListUserInfo, long AOrgIDOrSkillID, int AType) 
        {
            if (AOrgIDOrSkillID > 9060000000000000000 && AOrgIDOrSkillID < 9070000000000000000)//技能组
            {
                List<long> LLongSkillMappingID = new List<long>();
                DALCommon.GetObjInfoMapping(IDataBaseConfig, "906", "907", IGlobalSetting.StrRent, ref LLongSkillMappingID);
                foreach (long UserID in LLongSkillMappingID)
                {
                    ObjectInfo agentInfo = IListUserInfo.Where(p => p.ObjID == UserID).Count() > 0 ? IListUserInfo.Where(p => p.ObjID == UserID).First() : null;
                    if (!AListUserInfo.Contains(agentInfo))
                    {
                        AListUserInfo.Add(agentInfo);
                    }
                }

            }
            else if (AOrgIDOrSkillID > 1010000000000000000 && AOrgIDOrSkillID < 1020000000000000000)//机构 
            {
                if (AType == 1)
                {
                    AListUserInfo = IListUserInfo.Where(p => p.BeyondOrgID == AOrgIDOrSkillID).ToList();
                }
                else if (AType == 2)
                {
                    List<ObjectInfo> LListUserInfo = IListUserInfo.Where(p => p.BeyondOrgID == AOrgIDOrSkillID).ToList();
                    foreach (ObjectInfo LAgent in LListUserInfo)
                    {
                        if (!AListUserInfo.Contains(LAgent))
                        {
                            AListUserInfo.Add(LAgent);
                        }
                    }
                    List<ObjectInfo> LListOrgInfo = IListOrgInfo.Where(p => p.ParentOrgID == AOrgIDOrSkillID).ToList();
                    foreach (ObjectInfo LOrg in LListOrgInfo)
                    {
                        GetUserInfoInOrgORSkill(ref AListUserInfo, LOrg.ObjID, AType);
                    }
                }
            }
        }


        /// <summary>
        /// 得到机构或技能组下的分机
        /// </summary>
        /// <param name="AlistExensionInfo"></param>
        /// <param name="AOrgIDOrSkillID"></param>
        /// <param name="AType"></param>
        private void GetExtensionInfoInOrgORSkill(ref List<ObjectInfo> AlistExensionInfo, long AOrgIDOrSkillID, int AType)
        {
            if (AOrgIDOrSkillID > 9060000000000000000 && AOrgIDOrSkillID < 9070000000000000000)//技能组
            {
                List<long> LLongSkillMappingID = new List<long>();
                DALCommon.GetObjInfoMapping(IDataBaseConfig, "906", "907", IGlobalSetting.StrRent, ref LLongSkillMappingID);
                foreach (long Extension in LLongSkillMappingID)
                {
                    ObjectInfo extensionInfo = IListExtension.Where(p => p.ObjID == Extension).Count() > 0 ? IListExtension.Where(p => p.ObjID == Extension).First() : null;
                    if (!AlistExensionInfo.Contains(extensionInfo))
                    {
                        AlistExensionInfo.Add(extensionInfo);
                    }
                }

            }
            else if (AOrgIDOrSkillID > 1010000000000000000 && AOrgIDOrSkillID < 1020000000000000000)//机构 
            {
                if (AType == 1)
                {
                    AlistExensionInfo = IListExtension.Where(p => p.BeyondOrgID == AOrgIDOrSkillID).ToList();
                }
                else if (AType == 2)
                {
                    List<ObjectInfo> LListExtensionInfo = IListExtension.Where(p => p.BeyondOrgID == AOrgIDOrSkillID).ToList();
                    foreach (ObjectInfo Extension in LListExtensionInfo)
                    {
                        if (!AlistExensionInfo.Contains(Extension))
                        {
                            AlistExensionInfo.Add(Extension);
                        }
                    }
                    List<ObjectInfo> LListOrgInfo = IListOrgInfo.Where(p => p.ParentOrgID == AOrgIDOrSkillID).ToList();
                    foreach (ObjectInfo LOrg in LListOrgInfo)
                    {
                        GetExtensionInfoInOrgORSkill(ref AlistExensionInfo, LOrg.ObjID, AType);
                    }
                }
            }
        }


        /// <summary>
        ///  得到机构或技能组下座席
        /// </summary>
        /// <param name="AListAgentInfo"></param>
        /// <param name="AOrgID"></param>
        /// <param name="AType">1平行  2 为钻取 </param>
        private void GetAgentInfoInOrg(ref List<ObjectInfo> AListAgentInfo, long AOrgIDOrSkillID, int AType)
        {
            
            if (AOrgIDOrSkillID > 9060000000000000000 && AOrgIDOrSkillID < 9070000000000000000)//技能组
            {
                List<long> LLongSkillMappingID = new List<long>();
                DALCommon.GetObjInfoMapping(IDataBaseConfig,"906", "907", IGlobalSetting.StrRent, ref LLongSkillMappingID);
                foreach (long AgentID in LLongSkillMappingID)
                {
                    ObjectInfo agentInfo = IListAgentInfo.Where(p => p.ObjID == AgentID).Count() > 0 ? IListAgentInfo.Where(p => p.ObjID == AgentID).First() : null;
                    if (!AListAgentInfo.Contains(agentInfo))
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
                    List<ObjectInfo> LListAgentInfo = IListAgentInfo.Where(p => p.BeyondOrgID == AOrgIDOrSkillID).ToList();
                    foreach (ObjectInfo LAgent in LListAgentInfo)
                    {
                        if (!AListAgentInfo.Contains(LAgent))
                        {
                            AListAgentInfo.Add(LAgent);
                        }
                    }
                    List<ObjectInfo> LListOrgInfo = IListOrgInfo.Where(p => p.ParentOrgID == AOrgIDOrSkillID).ToList();
                    foreach (ObjectInfo LOrg in LListOrgInfo)
                    {
                        GetAgentInfoInOrg(ref AListAgentInfo, LOrg.ObjID, AType);
                    }
                }
            }
        }


        /// <summary>
        ///  得到机构以及其下的机构信息
        /// </summary>
        /// <param name="AListOrgInfo"></param>
        /// <param name="AOrgID"></param>
        private void GetOrgInfoSon(ref List<ObjectInfo> AListOrgInfo, long AOrgID)
        {
            ObjectInfo LOrgInfoTemp = IListOrgInfo.Where(p => p.ObjID == AOrgID).Count() > 0 ? IListOrgInfo.Where(p => p.ObjID == AOrgID).First() : null;
            if (LOrgInfoTemp != null)
            {
                if (!AListOrgInfo.Contains(LOrgInfoTemp))
                {
                    AListOrgInfo.Add(LOrgInfoTemp);
                    List<ObjectInfo> LListOrgInfoTemp = IListOrgInfo.Where(p => p.ParentOrgID == AOrgID).ToList();
                    foreach (ObjectInfo LOrgInfo in LListOrgInfoTemp)
                    {
                        GetOrgInfoSon(ref AListOrgInfo, LOrgInfo.ObjID);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 初始化pmkpistatistics
        /// </summary>
        /// <param name="pmkpistatistics"></param>
        /// <param name="pmkpimapping"></param>
        /// <param name="starttime"></param>
        protected void InitPMKpiStatistics(ref KPIStatisticsData pmkpistatistics, KPIMapping pmkpimapping, DateTime starttime)
        {
            pmkpistatistics = pmkpistatistics ?? (new KPIStatisticsData());
            pmkpistatistics.KPIMappingID = pmkpimapping.KPIMappingID;
            pmkpistatistics.KPIID = pmkpimapping.KpiID;
            pmkpistatistics.RowID = 0;
            pmkpistatistics.ObjectID = pmkpimapping.ObjectID;
            pmkpistatistics.SliceType = Convert.ToInt16(pmkpimapping.KpiSliceType);

            pmkpistatistics.SliceInOrder = 0;
            pmkpistatistics.StartTimeUTC = LongParse(starttime.ToUniversalTime().ToString("yyyyMMddHHmmss"), 0);
            pmkpistatistics.StartTimeLocal = LongParse(starttime.ToString("yyyyMMdd000000"), 0);
            pmkpistatistics.UpdateTime = DateTime.Now;
            pmkpistatistics.Year = starttime.Year;
            pmkpistatistics.Month = starttime.Month;
            pmkpistatistics.Day = starttime.Day;

            pmkpistatistics.ActualValue = 0;
            pmkpistatistics.Goal1 = pmkpimapping.Goal1;
            pmkpistatistics.CompareSign1 = pmkpimapping.CompareSign1;
            pmkpistatistics.ActualCompareGoal1 = 0;
            pmkpistatistics.Show1 = string.Empty;

            
            //跟踪，表示跟上期相同KPI的对比情况，为正代表提高、为负代表下降，数值代表提高或下降多少（或前台展现时动态计算）
            pmkpistatistics.Trend1 = 0;

            pmkpistatistics.Goal2 = pmkpimapping.Goal2;
            pmkpistatistics.ColumnOrder = 0;

            //特性部分初始化
            switch (pmkpimapping.KpiSliceType)
            {
                case EnumKPISliceType.KYear:
                    {
                        pmkpistatistics.SliceInOrder = starttime.Year;
                    }
                    break;
                case EnumKPISliceType.KMonth: 
                    {
                        pmkpistatistics.SliceInOrder = starttime.Month;
                    }
                    break;
                case EnumKPISliceType.KWeek:
                    {
                        pmkpistatistics.SliceInOrder = GetWeekOrder(starttime, IGlobalSetting);
                    }
                    break;
                case EnumKPISliceType.KDay:
                    {
                        pmkpistatistics.SliceInOrder = starttime.DayOfYear;
                    }
                    break;
                case EnumKPISliceType.KHour:
                    {
                        //pmkpistatistics.SliceInOrder = starttime.DayOfYear;
                        //pmkpistatistics.ColumnOrder = starttime.DayOfYear;
                    }
                    break;
                case EnumKPISliceType.K30M:
                    {
                        //pmkpistatistics.SliceInOrder = starttime.DayOfYear;
                        //pmkpistatistics.ColumnOrder = starttime.DayOfYear;
                    }
                    break;
                case EnumKPISliceType.K15M:
                    {
                        //pmkpistatistics.SliceInOrder = starttime.DayOfYear;
                        //pmkpistatistics.ColumnOrder = starttime.DayOfYear;
                    }
                    break;
                case EnumKPISliceType.K10M:
                    {
                        //pmkpistatistics.SliceInOrder = starttime.DayOfYear;
                        //pmkpistatistics.ColumnOrder = starttime.DayOfYear;
                    }
                    break;
                case EnumKPISliceType.K5M:
                    {
                        //pmkpistatistics.SliceInOrder = starttime.DayOfYear;
                        //pmkpistatistics.ColumnOrder = starttime.DayOfYear;
                    }
                    break;
                default:
                    break;
            }
            
        }

        /// <summary>
        /// 将对象ID写入到string 的list中
        /// </summary>
        /// <param name="AlistObjectInfo"></param>
        /// <param name="AListStrID"></param>
        private void GetListStrOfObjectID(List<ObjectInfo> AlistObjectInfo, ref List<string> AListStrID)
        {
            AListStrID.Clear();
            AListStrID.Add(string.Empty);
            AListStrID.Add(AlistObjectInfo.Count.ToString());
            foreach (ObjectInfo o in AlistObjectInfo)
            {
                string strInfo = string.Format("{0}{1}", o.ObjID, AscCodeToChr(27));
                AListStrID.Add(strInfo);
            }
        }

    }
}
