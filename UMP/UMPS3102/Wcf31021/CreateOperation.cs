using System;
using System.Collections.Generic;
using System.IO;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;

namespace Wcf31021
{
    public partial class Service31021
    {
        private OperationReturn CreateQueryConditionString(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0         ABCD查询中T_31_054表的表名
                //1         Count
                //2...      QueryConditionDetail
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                //t_31_54的表名
                string partTable = listParams[0];
                string strCount = listParams[1];
                int intCount;
                if (!int.TryParse(strCount, out intCount) || intCount <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Condition count invalid");
                    return optReturn;
                }
                if (listParams.Count < 2 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Condition count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                QueryConditionDetail queryDetail;
                List<string> listReturn = new List<string>();
                string strReturn = string.Empty;
                string strLog = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        for (int i = 0; i < intCount; i++)
                        {
                            optReturn = XMLHelper.DeserializeObject<QueryConditionDetail>(listParams[i + 2]);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            queryDetail = optReturn.Data as QueryConditionDetail;
                            if (queryDetail == null)
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_OBJECT_NULL;
                                optReturn.Message = string.Format("QueryConditionDetail is null");
                                return optReturn;
                            }
                            if (!queryDetail.IsEnable)
                            {
                                continue;
                            }
                            string strValue01;
                            switch (queryDetail.ConditionItemID)
                            {

                                #region Group1

                                case S3102Consts.CON_TIMETYPEFROMTO:
                                    strReturn +=
                                       string.Format(
                                           "X.C005 >= '{0}' AND X.C005 <= '{1}' AND "
                                           , queryDetail.Value01
                                           , queryDetail.Value02);
                                    strLog += string.Format("{0} {1} --- {2} ", Utils.FormatOptLogString("3102C3031401010000000013"), queryDetail.Value01, queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_TIMEFROMTO://简单时间,这个现在用不到了
                                    strReturn +=
                                        string.Format(
                                            "X.C005 >= '{0}' AND X.C005 <= '{1}' AND "
                                            , queryDetail.Value01
                                            , queryDetail.Value02);
                                    strLog += string.Format("{0} {1} --- {2} ", Utils.FormatOptLogString("3102C3031401010000000001"), queryDetail.Value01, queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_DURATIONFROMTO:
                                    strReturn += string.Format("X.C012 >= {0} AND X.C012 <= {1} AND ", queryDetail.Value01,
                                        queryDetail.Value02);
                                    strLog += string.Format("{0} {1} --- {2} ", Utils.FormatOptLogString("3102C3031401010000000002"), queryDetail.Value01, queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_EXTENSION_MULTITEXT:
                                    if (!string.IsNullOrEmpty(queryDetail.Value01))
                                    {
                                        //strReturn +=
                                        //    string.Format("X.C042 IN (SELECT CONVERT(NVARCHAR(32),C011) FROM T_00_901 WHERE C001 = {0} AND C012 LIKE '104%' AND C014 = X.C020 ) AND "
                                        //    , queryDetail.Value01);
                                        strReturn +=
                                            string.Format("X.C042 IN (SELECT CONVERT(NVARCHAR(32),C011) FROM T_00_901 WHERE C001 = {0}  ) AND "
                                            , queryDetail.Value01);
                                    }

                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000003"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_AGENT_MULTITEXT:
                                    if (!string.IsNullOrEmpty(queryDetail.Value01))
                                    {
                                        //目前做的一次修改 by tangche
                                        strReturn +=
                                            string.Format("X.C039 IN (SELECT CONVERT(NVARCHAR(128),C011) FROM T_00_901 WHERE C001 = {0}) AND "
                                                , queryDetail.Value01);
                                    }
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000004"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CALLERID_LIKETEXT:
                                    if (queryDetail.Value01 != null)
                                    {
                                        string temp_ = queryDetail.Value01;
                                        string[] arrInfo = temp_.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        string tempStrSql = string.Empty;
                                        for (int m = 0; m < arrInfo.Length; m++)
                                        {
                                            if (m == arrInfo.Length - 1)
                                            {
                                                tempStrSql += string.Format(" X.C040 LIKE '%{0}%' ", arrInfo[m]);
                                            }
                                            else
                                            {
                                                tempStrSql += string.Format(" X.C040 LIKE '%{0}%' OR ", arrInfo[m]);
                                            }
                                        }
                                        strReturn += string.Format("({0}) AND ", tempStrSql);
                                        strLog += string.Format("{0} {1}", Utils.FormatOptLogString("3102C3031401010000000005"), queryDetail.Value01);
                                    }
                                    //if (queryDetail.Value02 == "Y")
                                    //{
                                    //    strReturn += string.Format("X.C040 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    //}
                                    //else
                                    //{
                                    //    strReturn += string.Format("X.C040 = '{0}' AND ", queryDetail.Value01);
                                    //}
                                    //strLog += string.Format("{0} {1} {2} ", Utils.FormatOptLogString("3102C3031401010000000005"), queryDetail.Value01,
                                    //    queryDetail.Value02 == "Y" ? Utils.FormatOptLogString("31021120") : "");
                                    break;
                                case S3102Consts.CON_CALLEDID_LIKETEXT:
                                    if (queryDetail.Value01 != null)
                                    {
                                        string temp__ = queryDetail.Value01;
                                        string[] arrInfo = temp__.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        string tempStrSql = string.Empty;
                                        for (int m = 0; m < arrInfo.Length; m++)
                                        {
                                            if (m == arrInfo.Length - 1)
                                            {
                                                tempStrSql += string.Format(" X.C041 LIKE '%{0}%' ", arrInfo[m]);
                                            }
                                            else
                                            {
                                                tempStrSql += string.Format(" X.C041 LIKE '%{0}%' OR ", arrInfo[m]);
                                            }
                                        }
                                        strReturn += string.Format(" ({0}) AND ", tempStrSql);
                                        strLog += string.Format("{0} {1}  ", Utils.FormatOptLogString("3102C3031401010000000006"), queryDetail.Value01);
                                    }
                                    //if (queryDetail.Value02 == "Y")
                                    //{
                                    //    strReturn += string.Format("X.C041 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    //}
                                    //else
                                    //{
                                    //    strReturn += string.Format("X.C041 = '{0}' AND ", queryDetail.Value01);
                                    //}
                                    //strLog += string.Format("{0} {1} {2} ", Utils.FormatOptLogString("3102C3031401010000000006"), queryDetail.Value01,
                                    //    queryDetail.Value02 == "Y" ? Utils.FormatOptLogString("31021120") : "");
                                    break;
                                case S3102Consts.CON_CHANNELNAME:
                                    if (queryDetail.Value01 != null)
                                    {
                                        string temp__ = queryDetail.Value01;
                                        string[] arrInfo = temp__.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        string tempStrSql = string.Empty;
                                        for (int m = 0; m < arrInfo.Length; m++)
                                        {
                                            if (m == arrInfo.Length - 1)
                                            {
                                                tempStrSql += string.Format(" X.C046 LIKE '%{0}%' ", arrInfo[m]);
                                            }
                                            else
                                            {
                                                tempStrSql += string.Format(" X.C046 LIKE '%{0}%' OR ", arrInfo[m]);
                                            }
                                        }
                                        strReturn += string.Format(" ({0}) AND ", tempStrSql);
                                        strLog += string.Format("{0} {1}  ", Utils.FormatOptLogString("3102C3031401010000000030"), queryDetail.Value01);
                                    }
                                    break;
                                case S3102Consts.CON_VOICEID_MULTITEXT:
                                    strReturn +=
                                       string.Format("X.C037 IN (SELECT CONVERT(BIGINT,C011) FROM T_00_901 WHERE C001 = {0}) AND "
                                           , queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000007"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CHANNELID_MULTITEXT:
                                    strReturn +=
                                      string.Format("X.C038 IN (SELECT CONVERT(BIGINT,C011) FROM T_00_901 WHERE C001 = {0}) AND "
                                          , queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000008"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_SERIALID_MULTITEXT:
                                    strReturn +=
                                      string.Format("X.C002 IN (SELECT CONVERT(BIGINT,C011) FROM T_00_901 WHERE C001 = {0}) AND "
                                          , queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000011"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_RECORDREFERENCE_MULTITEXT:
                                    strReturn +=
                                      string.Format("X.C077 IN (SELECT CONVERT(VARCHAR(64),C011) FROM T_00_901 WHERE C001 = {0}) AND "
                                          , queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000009"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_DIRECTION_CHECKSELECT:
                                    strValue01 = queryDetail.Value01;
                                    if (strValue01.Length >= 2)
                                    {
                                        //仅呼入
                                        if (strValue01[0] == '1' && strValue01[1] == '0')
                                        {
                                            strReturn += string.Format("X.C045 = '1' AND ");
                                        }//仅呼出
                                        else if (strValue01[0] == '0' && strValue01[1] == '1')
                                        {
                                            strReturn += string.Format("X.C045 = '0' AND ");
                                        }
                                        strLog += string.Format("{0} {1}{2} ",
                                            Utils.FormatOptLogString("3102C3031401010000000012"),
                                            strValue01[0] == '1' ? Utils.FormatOptLogString("3102C3031401010000000012Callin") : "",
                                            strValue01[1] == '1' ? Utils.FormatOptLogString("3102C3031401010000000012Callout") : "");
                                    }
                                    break;

                                #endregion


                                #region Group2

                                case S3102Consts.CON_MEMOAUTO_LIKETEXT:
                                    strReturn +=
                                      string.Format("X.C072 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000014"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_BOOKMARKTITLE_LIKETEXT:
                                    strReturn +=
                                      string.Format("X.C073 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000015"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_INSPECTOR:
                                    string temp = queryDetail.Value02;
                                    if (!string.IsNullOrEmpty(temp))
                                    {
                                        strReturn += string.Format("X.C002 IN (SELECT C002 FROM T_31_008_{0} WHERE C005 IN ({1}) AND C009='Y') AND ",
                                                                                              rentToken, temp);
                                    }
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000016"), queryDetail.Value01);
                                    break;
                                case S3102Consts.CON_ISSCORED_CHECKSELECT:
                                    strValue01 = queryDetail.Value01;
                                    if (strValue01.Length >= 2)
                                    {
                                        //只勾选已评分的~
                                        if (strValue01[0] == '1' && strValue01[1] == '0')
                                        {
                                            strReturn += string.Format(
                                                "X.C002 IN (SELECT C002 FROM T_31_008_{0} WHERE C009 = 'Y' AND  C004>={1} and C004<={2})  AND ",
                                                                                         rentToken, queryDetail.Value02, queryDetail.Value03);
                                        }//只勾选未评分的
                                        else if (strValue01[0] == '0' && strValue01[1] == '1')
                                        {
                                            strReturn += string.Format("X.C002 NOT IN (SELECT C002 FROM T_31_008_{0} WHERE C009 = 'Y')  AND ", rentToken);
                                        }
                                        else if (strValue01[0] == '1' && strValue01[1] == '1')
                                        {
                                            strReturn += string.Format("(X.C002 NOT IN (SELECT C002 FROM T_31_008_{0} WHERE C009 = 'Y') OR X.C002 IN (SELECT C002 FROM T_31_008_{0} WHERE C009 = 'Y' AND  C004>={1} AND C004<={2}))  AND ", rentToken, queryDetail.Value02, queryDetail.Value03);
                                        }
                                        strLog += string.Format("{0} {1}{2} ",
                                            Utils.FormatOptLogString("3102C3031401010000000017"),
                                            strValue01[0] == '1' ? Utils.FormatOptLogString("3102C3031401010000000017Yes") : "",
                                            strValue01[1] == '1' ? Utils.FormatOptLogString("3102C3031401010000000017No") : "");
                                    }
                                    break;
                                case S3102Consts.CON_SCORESHEET_COMBOBOX:
                                    strReturn +=
                                      string.Format("X.C002 IN (SELECT C002 FROM T_31_008_{0} WHERE C003={1} AND C009 = 'Y')  AND ", rentToken, queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000018"), queryDetail.Value03);
                                    break;
                                case S3102Consts.CON_SKILLGROUP:
                                    string temp1 = queryDetail.Value02;
                                    if (!string.IsNullOrEmpty(temp1))
                                    {
                                        strReturn += string.Format("C107 IN ({1}) AND ",
                                                          rentToken, temp1);
                                    }
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000019"), queryDetail.Value01);
                                    break;
                                case S3102Consts.CON_REALEXTENSION:
                                    if (!string.IsNullOrEmpty(queryDetail.Value01))
                                    {
                                        strReturn +=
                                            string.Format("X.C058 IN (SELECT CONVERT(NVARCHAR(128),C011) FROM T_00_901 WHERE C001 = {0}) AND "
                                                , queryDetail.Value01);
                                    }
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000020"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_PARTICIPANTNUM:
                                    if (!string.IsNullOrEmpty(queryDetail.Value01))
                                    {
                                        strReturn +=
                                            string.Format("C055 IN (SELECT CONVERT(NVARCHAR(1024),C011) FROM T_00_901 WHERE C001 = {0}) AND "
                                                , queryDetail.Value01);
                                    }
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000021"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_SERVICEATTITUDE:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05))
                                    {
                                        if (queryDetail.Value05 == "3")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2')  AND "
                                                , partTable, queryDetail.Value03);
                                        }
                                        else
                                        {
                                            strReturn += String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2})  AND "
                                                , partTable
                                                , queryDetail.Value03
                                                , queryDetail.Value05);
                                        }
                                        if (queryDetail.Value06 != null)
                                        {
                                            strReturn += String.Format(" X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND ", queryDetail.Value06);
                                        }
                                        if (queryDetail.Value07 != null)
                                        {
                                            strReturn += String.Format(" X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;
                                case S3102Consts.CON_RECORDDURATIONEXCEPT:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05))
                                    {
                                        if (queryDetail.Value05 == "3")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2') AND "
                                                , partTable, queryDetail.Value03);
                                        }
                                        else
                                        {
                                            strReturn += String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2}) AND ", partTable
                                                , queryDetail.Value03
                                                , queryDetail.Value05);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value06))
                                        {
                                            strReturn += String.Format(" X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value06);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value07))
                                        {
                                            strReturn += String.Format(" X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;
                                case S3102Consts.CON_EXCEPTIONSCORE:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05)) 
                                    {
                                        if (queryDetail.Value05 == "3")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2') AND ",
                                               partTable, queryDetail.Value03);
                                        }
                                        else 
                                        {
                                            strReturn += String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2}) AND ", partTable
                                                   , queryDetail.Value03
                                                   , queryDetail.Value05);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value06))
                                        {
                                            strReturn += String.Format(" X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value06);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value07))
                                        {
                                            strReturn += String.Format(" X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;
                                case S3102Consts.CON_AFTERDEALDURATIONEXCEPT:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05))
                                    {
                                        if (queryDetail.Value05 == "3")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2') AND ",
                                               partTable, queryDetail.Value03);
                                        }
                                        else
                                        {
                                            strReturn += String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2}) AND ", partTable
                                                   , queryDetail.Value03
                                                   , queryDetail.Value05);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value06))
                                        {
                                            strReturn += String.Format(" X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value06);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value07))
                                        {
                                            strReturn += String.Format(" X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;
                                case S3102Consts.CON_ACSPEEXCEPTPROPORTION:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05))
                                    {
                                        if (queryDetail.Value05 == "3")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2') AND ",
                                               partTable, queryDetail.Value03);
                                        }
                                        else
                                        {
                                            strReturn += String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2}) AND ", partTable
                                                   , queryDetail.Value03
                                                   , queryDetail.Value05);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value06))
                                        {
                                            strReturn += String.Format(" X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value06);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value07))
                                        {
                                            strReturn += String.Format(" X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;
                                #endregion


                                #region Group3

                                case S3102Consts.CON_CALLERDTMF:
                                    strReturn += string.Format("X.C043 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000024"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CALLEDDTMF:
                                    strReturn += string.Format("X.C044 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000025"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CTIREFERENCE:
                                    if (!string.IsNullOrEmpty(queryDetail.Value01))
                                    {
                                        strReturn += string.Format(
                                            "X.C047 IN (SELECT CONVERT(NVARCHAR(128),C011) FROM T_00_901 WHERE C001 = {0}) AND ",
                                            queryDetail.Value01);
                                    }
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000026"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_KEYWORDS:
                                    string temp2 = queryDetail.Value02;
                                    if (!string.IsNullOrEmpty(temp2))
                                    {
                                        strReturn += string.Format("X.C002 IN (SELECT DISTINCT C002 FROM T_51_009_{0} WHERE C008 IN({1})) AND",rentToken,temp2);
                                    }
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000027"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_REPEATEDCALL:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05))
                                    {
                                        if (queryDetail.Value05 == "3")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2')  AND "
                                                , partTable, queryDetail.Value03);
                                        }
                                        else
                                        {
                                            strReturn +=
                                                String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2} )  AND ",
                                                    partTable, queryDetail.Value03
                                                    , queryDetail.Value05);
                                        }

                                        if (queryDetail.Value06 != null)
                                        {
                                            strReturn += String.Format("  X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND ", queryDetail.Value06);
                                        }
                                        if (queryDetail.Value07 != null)
                                        {
                                            strReturn += String.Format("  X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;
                                case S3102Consts.CON_CALLPEAK:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05))
                                    {
                                        if (queryDetail.Value05 == "4")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2' OR {1}='3' ) AND "
                                                , partTable, queryDetail.Value03);
                                        }
                                        else
                                        {
                                            strReturn +=
                                                String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2} ) AND ",
                                                    partTable, queryDetail.Value03
                                                    , queryDetail.Value05);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value06))
                                        {
                                            strReturn += String.Format(" X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value06);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value07))
                                        {
                                            strReturn += String.Format(" X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;
                                case S3102Consts.CON_PROFESSIONAlLEVEL:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05))
                                    {
                                        if (queryDetail.Value05 == "3")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2')  AND "
                                                , partTable, queryDetail.Value03);
                                        }
                                        else
                                        {
                                            strReturn +=
                                                String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2} )  AND ",
                                                    partTable, queryDetail.Value03
                                                    , queryDetail.Value05);
                                        }
                                        if (queryDetail.Value06 != null)
                                        {
                                            strReturn += String.Format("  X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND ", queryDetail.Value06);
                                        }
                                        if (queryDetail.Value07 != null)
                                        {
                                            strReturn += String.Format("  X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;

                                #endregion


                                #region Group4

                                case S3102Consts.CON_CUSTOMFIELD01:
                                    strReturn += string.Format("X.C301 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010001"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD02:
                                    strReturn += string.Format("X.C302 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010002"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD03:
                                    strReturn += string.Format("X.C303 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010003"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD04:
                                    strReturn += string.Format("X.C304 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010004"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD05:
                                    strReturn += string.Format("X.C305 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010005"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD06:
                                    strReturn += string.Format("X.C306 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010006"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD07:
                                    strReturn += string.Format("X.C307 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010007"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD08:
                                    strReturn += string.Format("X.C308 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010008"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD09:
                                    strReturn += string.Format("X.C309 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010009"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD10:
                                    strReturn += string.Format("X.C310 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010010"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD11:
                                    strReturn += string.Format("X.C311 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010011"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD12:
                                    strReturn += string.Format("X.C312 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010012"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD13:
                                    strReturn += string.Format("X.C313 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010013"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD14:
                                    strReturn += string.Format("X.C314 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010014"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD15:
                                    strReturn += string.Format("X.C315 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010015"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD16:
                                    strReturn += string.Format("X.C316 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010016"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD17:
                                    strReturn += string.Format("X.C317 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010017"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD18:
                                    strReturn += string.Format("X.C318 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010018"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD19:
                                    strReturn += string.Format("X.C319 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010019"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD20:
                                    strReturn += string.Format("X.C320 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010020"),
                                        queryDetail.Value02);
                                    break;

                                #endregion
                              
                                //default:
                                //    optReturn.Result = false;
                                //    optReturn.Code = Defines.RET_PARAM_INVALID;
                                //    optReturn.Message = string.Format("ConditionItemID invalid");
                                //    return optReturn;
                            }
                        }
                        if (strReturn.Length > 0)
                        {
                            strReturn = strReturn.Substring(0, strReturn.Length - 4);
                        }
                        break;
                    case 3:
                        for (int i = 0; i < intCount; i++)
                        {
                            optReturn = XMLHelper.DeserializeObject<QueryConditionDetail>(listParams[i + 2]);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            queryDetail = optReturn.Data as QueryConditionDetail;
                            if (queryDetail == null)
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_OBJECT_NULL;
                                optReturn.Message = string.Format("QueryConditionDetail is null");
                                return optReturn;
                            }
                            if (!queryDetail.IsEnable)
                            {
                                continue;
                            }
                            string strValue01;
                            switch (queryDetail.ConditionItemID)
                            {

                                #region Group1

                                case S3102Consts.CON_TIMETYPEFROMTO:
                                    strReturn +=
                                        string.Format(
                                            "X.C005 >= TO_DATE('{0}','YYYY-MM-DD HH24:MI:SS') AND X.C005 <= TO_DATE('{1}','YYYY-MM-DD HH24:MI:SS') AND "
                                            , queryDetail.Value01
                                            , queryDetail.Value02);
                                    strLog += string.Format("{0} {1} --- {2} ", Utils.FormatOptLogString("3102C3031401010000000013"), queryDetail.Value01, queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_TIMEFROMTO:
                                    strReturn +=
                                        string.Format(
                                            "X.C005 >= TO_DATE('{0}','YYYY-MM-DD HH24:MI:SS') AND X.C005 <= TO_DATE('{1}','YYYY-MM-DD HH24:MI:SS') AND "
                                            , queryDetail.Value01
                                            , queryDetail.Value02);
                                    strLog += string.Format("{0} {1} --- {2} ", Utils.FormatOptLogString("3102C3031401010000000001"), queryDetail.Value01, queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_DURATIONFROMTO:
                                    strReturn += string.Format("X.C012 >= {0} AND X.C012 <= {1} AND ", queryDetail.Value01,
                                        queryDetail.Value02);
                                    strLog += string.Format("{0} {1} --- {2} ", Utils.FormatOptLogString("3102C3031401010000000002"), queryDetail.Value01, queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_EXTENSION_MULTITEXT:
                                    if (!string.IsNullOrEmpty(queryDetail.Value01))
                                    {
                                        //strReturn +=
                                        //    string.Format("X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0} AND C012 LIKE '104%' AND C014 = X.C020 ) AND "
                                        //        , queryDetail.Value01);
                                        strReturn +=
                                            string.Format("X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}  ) AND "
                                            , queryDetail.Value01);
                                    }
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000003"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_AGENT_MULTITEXT:
                                    if (!string.IsNullOrEmpty(queryDetail.Value01))
                                    {
                                        strReturn +=
                                            string.Format("X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND "
                                                , queryDetail.Value01);
                                    }
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000004"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CALLERID_LIKETEXT:
                                    if (queryDetail.Value01 != null)
                                    {
                                        string temp_ = queryDetail.Value01;
                                        string[] arrInfo = temp_.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        string tempStrSql = string.Empty;
                                        for (int m = 0; m < arrInfo.Length; m++)
                                        {
                                            if (m == arrInfo.Length - 1)
                                            {
                                                tempStrSql += string.Format(" X.C040 LIKE '%{0}%' ", arrInfo[m]);
                                            }
                                            else
                                            {
                                                tempStrSql += string.Format(" X.C040 LIKE '%{0}%' OR ", arrInfo[m]);
                                            }
                                        }
                                        strReturn += string.Format("({0}) AND ", tempStrSql);
                                        strLog += string.Format("{0} {1}", Utils.FormatOptLogString("3102C3031401010000000005"), queryDetail.Value01);
                                    }
                                    //if (queryDetail.Value02 == "Y")
                                    //{
                                    //    strReturn += string.Format("X.C040 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    //}
                                    //else
                                    //{
                                    //    strReturn += string.Format("X.C040 = '{0}' AND ", queryDetail.Value01);
                                    //}
                                    //strLog += string.Format("{0} {1} {2} ", Utils.FormatOptLogString("3102C3031401010000000005"), queryDetail.Value01,
                                    //    queryDetail.Value02 == "Y" ? Utils.FormatOptLogString("31021120") : "");
                                    break;
                                case S3102Consts.CON_CALLEDID_LIKETEXT:
                                    if (queryDetail.Value01 != null)
                                    {
                                        string temp__ = queryDetail.Value01;
                                        string[] arrInfo = temp__.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        string tempStrSql = string.Empty;
                                        for (int m = 0; m < arrInfo.Length; m++)
                                        {
                                            if (m == arrInfo.Length - 1)
                                            {
                                                tempStrSql += string.Format(" X.C041 LIKE '%{0}%' ", arrInfo[m]);
                                            }
                                            else
                                            {
                                                tempStrSql += string.Format(" X.C041 LIKE '%{0}%' OR ", arrInfo[m]);
                                            }
                                        }
                                        strReturn += string.Format("({0}) AND ", tempStrSql);
                                        strLog += string.Format("{0} {1}", Utils.FormatOptLogString("3102C3031401010000000006"), queryDetail.Value01);
                                    }
                                    //if (queryDetail.Value02 == "Y")
                                    //{
                                    //    strReturn += string.Format("X.C041 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    //}
                                    //else
                                    //{
                                    //    strReturn += string.Format("X.C041 = '{0}' AND ", queryDetail.Value01);
                                    //}
                                    //strLog += string.Format("{0} {1} {2} ", Utils.FormatOptLogString("3102C3031401010000000006"), queryDetail.Value01,
                                    //    queryDetail.Value02 == "Y" ? Utils.FormatOptLogString("31021120") : "");
                                    break;
                                case S3102Consts.CON_CHANNELNAME:
                                    if (queryDetail.Value01 != null)
                                    {
                                        string temp__ = queryDetail.Value01;
                                        string[] arrInfo = temp__.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        string tempStrSql = string.Empty;
                                        for (int m = 0; m < arrInfo.Length; m++)
                                        {
                                            if (m == arrInfo.Length - 1)
                                            {
                                                tempStrSql += string.Format(" X.C046 LIKE '%{0}%' ", arrInfo[m]);
                                            }
                                            else
                                            {
                                                tempStrSql += string.Format(" X.C046 LIKE '%{0}%' OR ", arrInfo[m]);
                                            }
                                        }
                                        strReturn += string.Format(" ({0}) AND ", tempStrSql);
                                        strLog += string.Format("{0} {1}  ", Utils.FormatOptLogString("3102C3031401010000000030"), queryDetail.Value01);
                                    }
                                    break;
                                case S3102Consts.CON_VOICEID_MULTITEXT:
                                    strReturn +=
                                       string.Format("X.C037 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND "
                                           , queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000007"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CHANNELID_MULTITEXT:
                                    strReturn +=
                                      string.Format("X.C038 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND "
                                          , queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000008"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_SERIALID_MULTITEXT:
                                    strReturn +=
                                      string.Format("X.C002 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND "
                                          , queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000011"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_RECORDREFERENCE_MULTITEXT:
                                    strReturn +=
                                      string.Format("X.C077 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND "
                                          , queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000009"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_DIRECTION_CHECKSELECT:
                                    strValue01 = queryDetail.Value01;
                                    if (strValue01.Length >= 2)
                                    {
                                        //仅呼入
                                        if (strValue01[0] == '1' && strValue01[1] == '0')
                                        {
                                            strReturn += string.Format("X.C045 = '1' AND ");
                                        }//仅呼出
                                        else if (strValue01[0] == '0' && strValue01[1] == '1')
                                        {
                                            strReturn += string.Format("X.C045 = '0' AND ");
                                        }
                                        strLog += string.Format("{0} {1}{2} ",
                                            Utils.FormatOptLogString("3102C3031401010000000012"),
                                            strValue01[0] == '1' ? Utils.FormatOptLogString("3102C3031401010000000012Callin") : "",
                                            strValue01[1] == '1' ? Utils.FormatOptLogString("3102C3031401010000000012Callout") : "");
                                    }
                                    break;

                                #endregion


                                #region Group2

                                case S3102Consts.CON_MEMOAUTO_LIKETEXT:
                                    strReturn +=
                                      string.Format("X.C072 LIKE '%{0}%' AND"
                                          , queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000014"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_BOOKMARKTITLE_LIKETEXT:
                                    strReturn +=
                                      string.Format("X.C073 LIKE '%{0}%' AND", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000015"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_INSPECTOR:
                                    string temp = queryDetail.Value02;
                                    if (!string.IsNullOrEmpty(temp))
                                    {
                                        strReturn += string.Format("X.C002 IN (SELECT C002 FROM T_31_008_{0} WHERE C005 IN ({1}) AND X.C009='Y') AND ",
                                                          rentToken, temp);
                                    }
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000016"), queryDetail.Value01);
                                    break;
                                case S3102Consts.CON_ISSCORED_CHECKSELECT:
                                    strValue01 = queryDetail.Value01;
                                    if (strValue01.Length >= 2)
                                    {
                                        //只勾选已评分的~
                                        if (strValue01[0] == '1' && strValue01[1] == '0')
                                        {
                                            strReturn += string.Format(
                                                "X.C002 IN (SELECT C002 FROM T_31_008_{0} WHERE C009 = 'Y' AND  C004>={1} and C004<={2})  AND ",
                                                                                         rentToken, queryDetail.Value02, queryDetail.Value03);
                                        }//只勾选未评分的
                                        else if (strValue01[0] == '0' && strValue01[1] == '1')
                                        {
                                            strReturn += string.Format("X.C002 NOT IN (SELECT C002 FROM T_31_008_{0} WHERE C009 = 'Y')  AND ", rentToken);
                                        }
                                        else if (strValue01[0] == '1' && strValue01[1] == '1')
                                        {
                                            strReturn += string.Format("(X.C002 NOT IN (SELECT C002 FROM T_31_008_{0} WHERE C009 = 'Y') OR X.C002 IN (SELECT C002 FROM T_31_008_{0} WHERE C009 = 'Y' AND  C004>={1} AND C004<={2}))  AND ", rentToken, queryDetail.Value02, queryDetail.Value03);
                                        }
                                        strLog += string.Format("{0} {1}{2} ",
                                            Utils.FormatOptLogString("3102C3031401010000000017"),
                                            strValue01[0] == '1' ? Utils.FormatOptLogString("3102C3031401010000000017Yes") : "",
                                            strValue01[1] == '1' ? Utils.FormatOptLogString("3102C3031401010000000017No") : "");
                                    }
                                    break;
                                case S3102Consts.CON_SCORESHEET_COMBOBOX:
                                    strReturn +=
                                      string.Format("X.C002 IN (SELECT C002 FROM T_31_008_{0} WHERE C003={1} AND C009 = 'Y')  AND ", rentToken, queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000018"), queryDetail.Value03);
                                    break;
                                case S3102Consts.CON_SKILLGROUP:
                                    string temp1 = queryDetail.Value02;
                                    if (!string.IsNullOrEmpty(temp1))
                                    {
                                        strReturn += string.Format("X.C107 IN ({1}) AND ",
                                                          rentToken, temp1);
                                    }
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000019"), queryDetail.Value01);
                                    break;
                                case S3102Consts.CON_REALEXTENSION:
                                    if (!string.IsNullOrEmpty(queryDetail.Value01))
                                    {
                                        strReturn +=
                                            string.Format("X.C058 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND "
                                                , queryDetail.Value01);
                                    }
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000020"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_PARTICIPANTNUM:
                                    if (!string.IsNullOrEmpty(queryDetail.Value01))
                                    {
                                        strReturn +=
                                            string.Format("X.C055 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND "
                                                , queryDetail.Value01);
                                    }
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000021"), queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_SERVICEATTITUDE:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05))
                                    {
                                        if (queryDetail.Value05 == "3")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2')  AND "
                                                , partTable, queryDetail.Value03);
                                        }
                                        else
                                        {
                                            strReturn +=
                                                String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2} )  AND ",
                                                    partTable, queryDetail.Value03
                                                    , queryDetail.Value05);
                                        }

                                        if (queryDetail.Value06 != null)
                                        {
                                            strReturn += String.Format("  X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND ", queryDetail.Value06);
                                        }
                                        if (queryDetail.Value07 != null)
                                        {
                                            strReturn += String.Format("  X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;
                                case S3102Consts.CON_RECORDDURATIONEXCEPT:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05))
                                    {
                                        if (queryDetail.Value05 == "3")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2') AND "
                                                , partTable, queryDetail.Value03);
                                        }
                                        else
                                        {
                                            strReturn +=
                                                String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2} ) AND ",
                                                    partTable, queryDetail.Value03
                                                    , queryDetail.Value05);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value06))
                                        {
                                            strReturn += String.Format(" X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value06);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value07))
                                        {
                                            strReturn += String.Format(" X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;
                                case S3102Consts.CON_EXCEPTIONSCORE:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05))
                                    {
                                        if (queryDetail.Value05 == "3")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2') AND "
                                                , partTable, queryDetail.Value03);
                                        }
                                        else
                                        {
                                            strReturn +=
                                                String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2} ) AND ",
                                                    partTable, queryDetail.Value03
                                                    , queryDetail.Value05);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value06))
                                        {
                                            strReturn += String.Format(" X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value06);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value07))
                                        {
                                            strReturn += String.Format(" X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;
                                case S3102Consts.CON_ACSPEEXCEPTPROPORTION:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05))
                                    {
                                        if (queryDetail.Value05 == "3")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2') AND "
                                                , partTable, queryDetail.Value03);
                                        }
                                        else
                                        {
                                            strReturn +=
                                                String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2} ) AND ",
                                                    partTable, queryDetail.Value03
                                                    , queryDetail.Value05);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value06))
                                        {
                                            strReturn += String.Format(" X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value06);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value07))
                                        {
                                            strReturn += String.Format(" X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;
                                case S3102Consts.CON_AFTERDEALDURATIONEXCEPT:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05))
                                    {
                                        if (queryDetail.Value05 == "3")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2') AND "
                                                , partTable, queryDetail.Value03);
                                        }
                                        else
                                        {
                                            strReturn +=
                                                String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2} ) AND ",
                                                    partTable, queryDetail.Value03
                                                    , queryDetail.Value05);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value06))
                                        {
                                            strReturn += String.Format(" X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value06);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value07))
                                        {
                                            strReturn += String.Format(" X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;
                                #endregion


                                #region Group3

                                case S3102Consts.CON_CALLERDTMF:
                                    strReturn += string.Format("X.C043 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000024"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CALLEDDTMF:
                                    strReturn += string.Format("X.C044 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000025"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CTIREFERENCE:
                                    if (!string.IsNullOrEmpty(queryDetail.Value01))
                                    {
                                        strReturn += string.Format(
                                            "X.C047 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND ",
                                            queryDetail.Value01);
                                    }
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000026"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_REPEATEDCALL:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05))
                                    {
                                        if (queryDetail.Value05 == "3")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2')  AND "
                                                , partTable, queryDetail.Value03);
                                        }
                                        else
                                        {
                                            strReturn += String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2} )  AND ", partTable, queryDetail.Value03
    , queryDetail.Value05);
                                        }

                                        if (queryDetail.Value06 != null)
                                        {
                                            strReturn += String.Format("  X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND ", queryDetail.Value06);
                                        }
                                        if (queryDetail.Value07 != null)
                                        {
                                            strReturn += String.Format("  X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;
                                case S3102Consts.CON_KEYWORDS:
                                    string temp2 = queryDetail.Value02;
                                    if (!string.IsNullOrEmpty(temp2))
                                    {
                                        //strReturn += string.Format(" C002 IN (SELECT DISTINCT C003 FROM T_31_042_{0} WHERE C006 IN ({1})) AND ",
                                        //    rentToken, temp2);
                                        strReturn += string.Format("C002 IN (SELECT DISTINCT C002 FROM T_51_009_{0} WHERE C008 IN({1})) AND", rentToken, temp2);
                                    }
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000000027"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CALLPEAK:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05))
                                    {
                                        if (queryDetail.Value05 == "4")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2' OR {1}='3' ) AND "
                                                , partTable, queryDetail.Value03);
                                        }
                                        else
                                        {
                                            strReturn +=
                                                String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2} ) AND ",
                                                    partTable, queryDetail.Value03
                                                    , queryDetail.Value05);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value06))
                                        {
                                            strReturn += String.Format(" X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value06);
                                        }
                                        if (!string.IsNullOrEmpty(queryDetail.Value07))
                                        {
                                            strReturn += String.Format(" X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})  AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;
                                case S3102Consts.CON_PROFESSIONAlLEVEL:
                                    if (!string.IsNullOrEmpty(queryDetail.Value05))
                                    {
                                        if (queryDetail.Value05 == "3")
                                        {
                                            strReturn += string.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}='1' OR {1}='2')  AND "
                                                , partTable, queryDetail.Value03);
                                        }
                                        else
                                        {
                                            strReturn +=
                                                String.Format("X.C002 IN (SELECT C201 FROM {0} WHERE {1}={2} )  AND ",
                                                    partTable, queryDetail.Value03
                                                    , queryDetail.Value05);
                                        }
                                        if (queryDetail.Value06 != null)
                                        {
                                            strReturn += String.Format("  X.C039 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND ", queryDetail.Value06);
                                        }
                                        if (queryDetail.Value07 != null)
                                        {
                                            strReturn += String.Format("  X.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND ", queryDetail.Value07);
                                        }
                                    }
                                    break;

                                #endregion


                                #region Group4

                                case S3102Consts.CON_CUSTOMFIELD01:
                                    strReturn += string.Format("X.C301 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010001"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD02:
                                    strReturn += string.Format("X.C302 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010002"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD03:
                                    strReturn += string.Format("X.C303 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010003"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD04:
                                    strReturn += string.Format("X.C304 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010004"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD05:
                                    strReturn += string.Format("X.C305 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010005"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD06:
                                    strReturn += string.Format("X.C306 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010006"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD07:
                                    strReturn += string.Format("X.C307 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010007"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD08:
                                    strReturn += string.Format("X.C308 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010008"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD09:
                                    strReturn += string.Format("X.C309 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010009"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD10:
                                    strReturn += string.Format("X.C310 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010010"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD11:
                                    strReturn += string.Format("X.C311 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010011"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD12:
                                    strReturn += string.Format("X.C312 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010012"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD13:
                                    strReturn += string.Format("X.C313 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010013"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD14:
                                    strReturn += string.Format("X.C314 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010014"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD15:
                                    strReturn += string.Format("X.C315 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010015"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD16:
                                    strReturn += string.Format("X.C316 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010016"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD17:
                                    strReturn += string.Format("X.C317 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010017"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD18:
                                    strReturn += string.Format("X.C318 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010018"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD19:
                                    strReturn += string.Format("X.C319 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010019"),
                                        queryDetail.Value02);
                                    break;
                                case S3102Consts.CON_CUSTOMFIELD20:
                                    strReturn += string.Format("X.C320 LIKE '%{0}%' AND ", queryDetail.Value01);
                                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("3102C3031401010000010020"),
                                        queryDetail.Value02);
                                    break;

                                #endregion
                               
                                //default:
                                //    optReturn.Result = false;
                                //    optReturn.Code = Defines.RET_PARAM_INVALID;
                                //    optReturn.Message = string.Format("ConditionItemID invalid");
                                //    return optReturn;
                            }
                        }

                        //这个地方大概就是将条件的最后的AND去掉
                        if (strReturn.Length > 0)
                        {
                            strReturn = strReturn.Substring(0, strReturn.Length - 4);
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
                        return optReturn;
                }
                listReturn.Add(strReturn);
                listReturn.Add(strLog);
                optReturn.Data = listReturn;
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

    }
}