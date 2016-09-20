using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.ScoreSheets;

namespace Wcf31011
{
    public partial class Service31011
    {
        private OperationReturn SaveScoreSheet(SessionInfo session, ScoreSheet scoreSheet, string userID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                List<ScoreItem> listItems = new List<ScoreItem>();
                scoreSheet.GetAllScoreItem(ref listItems);


                #region 评分表

                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_001_{0} WHERE C001 = {1}", rentToken, scoreSheet.ID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_001_{0} WHERE C001 = {1}", rentToken, scoreSheet.ID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
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
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("DataSet table not exist");
                        return optReturn;
                    }

                    DataRow dr;
                    bool isAdd = false;
                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        dr = objDataSet.Tables[0].NewRow();
                        dr["C001"] = scoreSheet.ID;
                        isAdd = true;
                    }
                    else
                    {
                        dr = objDataSet.Tables[0].Rows[0];
                    }
                    dr["C002"] = scoreSheet.Title;
                    dr["C003"] = scoreSheet.ViewClassic == ScoreItemClassic.Table ? "C" : "T";
                    dr["C004"] = scoreSheet.TotalScore;
                    dr["C005"] = scoreSheet.Creator;
                    dr["C006"] = scoreSheet.CreateTime;
                    dr["C007"] = DateTime.Now.ToString();
                    dr["C008"] = scoreSheet.Creator;
                    dr["C009"] = DateTime.Parse("2014/1/1");
                    dr["C010"] = DateTime.Parse("2199/12/31");
                    dr["C011"] = scoreSheet.QualifiedLine;
                    dr["C012"] = listItems.Count;
                    dr["C013"] = "Y";
                    dr["C014"] = scoreSheet.ScoreType == ScoreType.YesNo
                        ? "F"
                        : scoreSheet.ScoreType == ScoreType.Pecentage ? "P" : "S";
                    dr["C015"] = 0;
                    dr["C016"] = DateTime.Now;
                    dr["C017"] = scoreSheet.UseTag;
                    dr["C018"] = (scoreSheet.Flag & 1) == 0 ? "Y" : "N";
                    dr["C019"] = scoreSheet.Description;
                    dr["C020"] = scoreSheet.CalAdditionalItem ? "Y" : "N";
                    dr["C021"] = scoreSheet.UsePointSystem ? "Y" : "N";

                    dr["C101"] = scoreSheet.ScoreWidth;
                    dr["C102"] = scoreSheet.TipWidth;
                    dr["C103"] = scoreSheet.HasAutoStandard ? "Y" : "N";
                    dr["C104"] = scoreSheet.AllowModifyScore ? "Y" : "N";
                    if (isAdd)
                    {
                        objDataSet.Tables[0].Rows.Add(dr);

                        #region 管理

                        OperationReturn mmtReturn = SaveScoreSheetManagement(session, scoreSheet.ID.ToString(), userID);
                        if (!mmtReturn.Result)
                        {
                            return mmtReturn;
                        }

                        #endregion

                        strMsg = string.Format("Insert {0};", scoreSheet.ID);
                    }
                    else
                    {
                        strMsg = string.Format("Update {0};", scoreSheet.ID);
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

                if (!optReturn.Result)
                {
                    return optReturn;
                }

                #endregion


                #region 子项

                OperationReturn subItemReturn;
                for (int i = 0; i < scoreSheet.Items.Count; i++)
                {
                    ScoreItem subItem = scoreSheet.Items[i];
                    subItem.ScoreSheet = scoreSheet;
                    subItem.Parent = scoreSheet;
                    subItem.OrderID = i;
                    subItemReturn = SaveScoreItem(session, subItem);
                    if (!subItemReturn.Result)
                    {
                        return subItemReturn;
                    }
                    strMsg += subItemReturn.Data;
                }
                subItemReturn = RemoveScoreItems(session, scoreSheet);
                if (!subItemReturn.Result)
                {
                    return subItemReturn;
                }
                strMsg += subItemReturn.Data;

                #endregion


                #region 备注

                OperationReturn commentReturn;
                for (int i = 0; i < scoreSheet.Comments.Count; i++)
                {
                    Comment comment = scoreSheet.Comments[i];
                    comment.ScoreItem = scoreSheet;
                    comment.ScoreSheet = scoreSheet;
                    commentReturn = SaveComment(session, comment);
                    if (!commentReturn.Result)
                    {
                        return commentReturn;
                    }
                    strMsg += commentReturn.Data;
                }

                commentReturn = RemoveComments(session, scoreSheet);
                if (!commentReturn.Result)
                {
                    return commentReturn;
                }
                strMsg += commentReturn.Data;

                #endregion


                #region 控制项

                OperationReturn controlReturn;
                for (int i = 0; i < scoreSheet.ControlItems.Count; i++)
                {
                    ControlItem controlItem = scoreSheet.ControlItems[i];
                    controlItem.ScoreSheet = scoreSheet;
                    controlReturn = SaveControlItem(session, controlItem);
                    if (!controlReturn.Result)
                    {
                        return controlReturn;
                    }
                    strMsg += controlReturn.Data;
                }
                controlReturn = RemoveControlItems(session, scoreSheet);
                if (!controlReturn.Result)
                {
                    return controlReturn;
                }
                strMsg += controlReturn.Data;

                #endregion


                #region 样式

                OperationReturn styleReturn;
                if (scoreSheet.TitleStyle != null)
                {
                    VisualStyle style = scoreSheet.TitleStyle;
                    style.ScoreObject = scoreSheet;
                    style.ScoreSheet = scoreSheet;
                    styleReturn = SaveVisualStyle(session, style, "T");
                    if (!styleReturn.Result)
                    {
                        return styleReturn;
                    }
                    strMsg += styleReturn.Data;
                }
                else
                {
                    styleReturn = RemoveVisualStyle(session, scoreSheet.ID.ToString(), "T");
                    if (!styleReturn.Result)
                    {
                        return styleReturn;
                    }
                    strMsg += styleReturn.Data;
                }

                if (scoreSheet.PanelStyle != null)
                {
                    VisualStyle style = scoreSheet.PanelStyle;
                    style.ScoreObject = scoreSheet;
                    style.ScoreSheet = scoreSheet;
                    styleReturn = SaveVisualStyle(session, style, "P");
                    if (!styleReturn.Result)
                    {
                        return styleReturn;
                    }
                    strMsg += styleReturn.Data;
                }
                else
                {
                    styleReturn = RemoveVisualStyle(session, scoreSheet.ID.ToString(), "P");
                    if (!styleReturn.Result)
                    {
                        return styleReturn;
                    }
                    strMsg += styleReturn.Data;
                }

                #endregion


                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn SaveScoreItem(SessionInfo session, ScoreItem scoreItem)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                ScoreSheet scoreSheet = scoreItem.ScoreSheet;
                if (scoreSheet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreSheet is null");
                    return optReturn;
                }


                #region 评分项

                ScoreGroup scoreGroup;
                Standard standard;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_002_{0} WHERE C001 = {1} AND C003 = {2}", rentToken, scoreItem.ItemID, scoreSheet.ID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_002_{0} WHERE C001 = {1} AND C003 = {2}", rentToken, scoreItem.ItemID, scoreSheet.ID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
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
                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("DataSet table not exist");
                        return optReturn;
                    }
                    DataRow dr;
                    bool isAdd = false;
                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        dr = objDataSet.Tables[0].NewRow();
                        dr["C001"] = scoreItem.ItemID;
                        dr["C003"] = scoreSheet.ID;
                        isAdd = true;
                    }
                    else
                    {
                        dr = objDataSet.Tables[0].Rows[0];
                    }
                    dr["C002"] = scoreItem.ID;
                    dr["C004"] = scoreItem.Parent != null ? scoreItem.Parent.ID : 0;
                    dr["C005"] = scoreItem.OrderID;
                    dr["C006"] = scoreItem.Title;
                    dr["C007"] = scoreItem.Description;
                    dr["C008"] = scoreItem.TotalScore;
                    dr["C009"] = scoreItem.Type == ScoreObjectType.ScoreGroup ? "N" : "Y";
                    dr["C010"] = scoreItem.IsAbortScore ? "Y" : "N";
                    dr["C011"] = (scoreItem.ControlFlag & 1) == 1 ? "Y" : "N";
                    dr["C012"] = scoreItem.IsKeyItem ? "Y" : "N";
                    dr["C013"] = scoreItem.IsAllowNA ? "Y" : "N";
                    dr["C014"] = "N";
                    dr["C017"] = scoreItem.IsAddtionItem ? "Y" : "N";
                    dr["C019"] = scoreItem.UsePointSystem ? "Y" : "N";
                    dr["C020"] = scoreItem.ScoreType == ScoreType.YesNo
                        ? "F"
                        : scoreItem.ScoreType == ScoreType.Pecentage ? "P" : "S";
                    dr["C021"] = "S";
                    dr["C022"] = scoreItem.Tip;
                    dr["C030"] = string.Empty;
                    standard = scoreItem as Standard;
                    if (standard != null)
                    {
                        dr["C015"] = (int)standard.StandardType;
                        dr["C016"] = "N";
                        dr["C018"] = standard.PointSystem;
                        dr["C101"] = standard.IsAutoStandard ? "Y" : "N";
                        dr["C102"] = standard.StatisticalID;
                        dr["C103"] = standard.AllowModifyScore ? "Y" : "N";
                        SliderStandard sliderStandard = standard as SliderStandard;
                        if (sliderStandard != null)
                        {
                            dr["C023"] = sliderStandard.MinValue;
                            dr["C024"] = sliderStandard.MaxValue;
                            dr["C026"] = sliderStandard.Interval;
                            dr["C029"] = sliderStandard.DefaultValue;
                        }
                        NumericStandard numericStandard = standard as NumericStandard;
                        if (numericStandard != null)
                        {
                            dr["C027"] = numericStandard.MaxValue;
                            dr["C028"] = numericStandard.MinValue;
                            dr["C029"] = numericStandard.DefaultValue;
                        }
                        YesNoStandard yesNoStandard = standard as YesNoStandard;
                        if (yesNoStandard != null)
                        {
                            if (yesNoStandard.DefaultValue)
                            {
                                dr["C029"] = 1.0;
                            }
                            else
                            {
                                dr["C029"] = 0.0;
                            }
                            dr["C104"] = yesNoStandard.ReverseDisplay ? "1" : "0";
                        }
                        ItemStandard itemStandard = standard as ItemStandard;
                        if (itemStandard != null)
                        {
                            dr["C029"] = itemStandard.DefaultIndex;
                        }
                    }
                    else
                    {
                        dr["C015"] = 0;
                        dr["C018"] = 0;
                        scoreGroup = scoreItem as ScoreGroup;
                        if (scoreGroup != null)
                        {
                            dr["C016"] = scoreGroup.IsAvg ? "Y" : "N";
                        }
                        else
                        {
                            dr["C016"] = "N";
                        }
                    }
                    if (isAdd)
                    {
                        objDataSet.Tables[0].Rows.Add(dr);
                        strMsg = string.Format("Insert {0};", scoreItem.ID);
                    }
                    else
                    {
                        strMsg = string.Format("Update {0};", scoreItem.ID);
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

                if (!optReturn.Result)
                {
                    return optReturn;
                }

                #endregion


                #region 子项

                scoreGroup = scoreItem as ScoreGroup;
                if (scoreGroup != null)
                {
                    ScoreItem subItem;
                    OperationReturn subItemReturn;
                    for (int i = 0; i < scoreGroup.Items.Count; i++)
                    {
                        subItem = scoreGroup.Items[i];
                        subItem.ScoreSheet = scoreGroup.ScoreSheet;
                        subItem.Parent = scoreGroup;
                        subItem.OrderID = i;
                        subItemReturn = SaveScoreItem(session, subItem);
                        if (!subItemReturn.Result)
                        {
                            return subItemReturn;
                        }
                        strMsg += subItemReturn.Data;
                    }
                    subItemReturn = RemoveScoreItems(session, scoreGroup);
                    if (!subItemReturn.Result)
                    {
                        return subItemReturn;
                    }
                    strMsg += subItemReturn.Data;
                }

                #endregion


                #region 备注

                OperationReturn commentReturn;
                for (int i = 0; i < scoreItem.Comments.Count; i++)
                {
                    Comment comment = scoreItem.Comments[i];
                    comment.ScoreItem = scoreItem;
                    comment.ScoreSheet = scoreItem.ScoreSheet;
                    commentReturn = SaveComment(session, comment);
                    if (!commentReturn.Result)
                    {
                        return commentReturn;
                    }
                    strMsg += commentReturn.Data;
                }

                commentReturn = RemoveComments(session, scoreItem);
                if (!commentReturn.Result)
                {
                    return commentReturn;
                }
                strMsg += commentReturn.Data;

                #endregion


                #region 样式

                if (scoreItem.TitleStyle != null)
                {
                    VisualStyle style = scoreItem.TitleStyle;
                    style.ScoreObject = scoreItem;
                    style.ScoreSheet = scoreItem.ScoreSheet;
                    optReturn = SaveVisualStyle(session, style, "T");
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    strMsg += optReturn.Data;
                }
                else
                {
                    optReturn = RemoveVisualStyle(session, scoreItem.ID.ToString(), "T");
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    strMsg += optReturn.Data;
                }

                if (scoreItem.PanelStyle != null)
                {
                    VisualStyle style = scoreItem.PanelStyle;
                    style.ScoreObject = scoreItem;
                    style.ScoreSheet = scoreItem.ScoreSheet;
                    optReturn = SaveVisualStyle(session, style, "P");
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    strMsg += optReturn.Data;
                }
                else
                {
                    optReturn = RemoveVisualStyle(session, scoreItem.ID.ToString(), "P");
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    strMsg += optReturn.Data;
                }

                #endregion


                #region 如果是多值型评分标准，保存标准子项

                var temp = scoreItem as ItemStandard;
                if (temp != null)
                {
                    OperationReturn subStandardItemReturn;
                    for (int i = 0; i < temp.ValueItems.Count; i++)
                    {
                        StandardItem standardItem = temp.ValueItems[i];
                        standardItem.ScoreItem = scoreItem;
                        standardItem.OrderID = i;
                        subStandardItemReturn = SaveStandardItem(session, standardItem);
                        if (!subStandardItemReturn.Result)
                        {
                            return subStandardItemReturn;
                        }
                        strMsg += subStandardItemReturn.Data;
                    }
                    subStandardItemReturn = RemoveStandardItems(session, temp);
                    if (!subStandardItemReturn.Result)
                    {
                        return subStandardItemReturn;
                    }
                    strMsg += subStandardItemReturn.Data;
                }

                #endregion

                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn SaveComment(SessionInfo session, Comment comment)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                ScoreItem scoreItem = comment.ScoreItem;
                ScoreSheet scoreSheet = comment.ScoreSheet;
                if (scoreItem == null || scoreSheet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreSheet or ScoreItem is null");
                    return optReturn;
                }

                #region 备注

                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_004_{0} WHERE C001 = {1}",
                            rentToken, comment.ID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_004_{0} WHERE C001 = {1}",
                            rentToken, comment.ID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
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
                    DataRow dr;
                    bool isAdd = false;
                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        dr = objDataSet.Tables[0].NewRow();
                        dr["C001"] = comment.ID;
                        dr["C002"] = scoreItem.ID;
                        dr["C003"] = scoreSheet.ID;
                        isAdd = true;
                    }
                    else
                    {
                        dr = objDataSet.Tables[0].Rows[0];
                    }
                    dr["C004"] = comment.OrderID;
                    dr["C009"] = comment.Title;
                    CommentStyle commentStyle = comment.Style;
                    dr["C008"] = (int)commentStyle;
                    if (commentStyle == CommentStyle.Item)
                    {
                        ItemComment itemComment = comment as ItemComment;
                        if (itemComment != null)
                        {

                        }
                    }
                    else
                    {
                        TextComment textComment = comment as TextComment;
                        if (textComment != null)
                        {
                            dr["C005"] = textComment.DefaultText;
                        }
                    }
                    var temp = scoreItem as ScoreSheet;
                    if (temp != null)
                    {
                        dr["C010"] = "Y";
                    }
                    else
                    {
                        dr["C010"] = "N";
                    }
                    if (isAdd)
                    {
                        objDataSet.Tables[0].Rows.Add(dr);
                        strMsg = string.Format("Insert {0};", comment.ID);
                    }
                    else
                    {
                        strMsg = string.Format("Update {0};", comment.ID);
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

                if (!optReturn.Result)
                {
                    return optReturn;
                }

                #endregion


                #region 样式

                if (comment.TitleStyle != null)
                {
                    VisualStyle style = comment.TitleStyle;
                    style.ScoreObject = comment;
                    style.ScoreSheet = comment.ScoreSheet;
                    optReturn = SaveVisualStyle(session, style, "T");
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    strMsg += optReturn.Data;
                }
                else
                {
                    optReturn = RemoveVisualStyle(session, comment.ID.ToString(), "T");
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    strMsg += optReturn.Data;
                }

                if (comment.PanelStyle != null)
                {
                    VisualStyle style = comment.PanelStyle;
                    style.ScoreObject = comment;
                    style.ScoreSheet = comment.ScoreSheet;
                    optReturn = SaveVisualStyle(session, style, "P");
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    strMsg += optReturn.Data;
                }
                else
                {
                    optReturn = RemoveVisualStyle(session, comment.ID.ToString(), "P");
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    strMsg += optReturn.Data;
                }

                #endregion


                #region 如果是多值型备注，保存备注子项

                var tempItem = comment as ItemComment;
                if (tempItem != null)
                {
                    OperationReturn commentItemReturn;
                    for (int i = 0; i < tempItem.ValueItems.Count; i++)
                    {
                        CommentItem commentItem = tempItem.ValueItems[i];
                        commentItem.Comment = comment;
                        commentItem.OrderID = i;
                        commentItemReturn = SaveCommentItem(session, commentItem);
                        if (!commentItemReturn.Result)
                        {
                            return commentItemReturn;
                        }
                        strMsg += commentItemReturn.Data;
                    }
                    commentItemReturn = RemoveCommentItems(session, tempItem);
                    if (!commentItemReturn.Result)
                    {
                        return commentItemReturn;
                    }
                    strMsg += commentItemReturn.Data;
                }

                #endregion

                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn SaveVisualStyle(SessionInfo session, VisualStyle visualStyle, string type)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                ScoreObject scoreObject = visualStyle.ScoreObject;
                ScoreSheet scoreSheet = visualStyle.ScoreSheet;
                if (scoreObject == null || scoreSheet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreObject or ScoreSheet is null");
                    return optReturn;
                }

                #region 样式

                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_012_{0} WHERE C001 = {1}",
                            rentToken,
                            visualStyle.ID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_012_{0} WHERE C001 = {1}",
                            rentToken,
                            visualStyle.ID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null ||
                    objAdapter == null ||
                    objCmdBuilder == null)
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

                    if (objDataSet.Tables.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Db object is null");
                        return optReturn;
                    }
                    bool isAdd = false;
                    DataRow dr;
                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        dr = objDataSet.Tables[0].NewRow();
                        dr["C001"] = visualStyle.ID;
                        isAdd = true;
                    }
                    else
                    {
                        dr = objDataSet.Tables[0].Rows[0];
                    }
                    var temp = scoreObject as ScoreSheet;
                    if (temp != null)
                    {
                        //评分表
                        dr["C002"] = "T";
                    }
                    else
                    {
                        var temp2 = scoreObject as ScoreItem;
                        if (temp2 != null)
                        {
                            //子项
                            dr["C002"] = "I";
                        }
                        else
                        {
                            var temp3 = scoreObject as Comment;
                            if (temp3 != null)
                            {
                                //备注
                                dr["C002"] = "C";
                            }
                            else
                            {
                                //其他
                                dr["C002"] = "O";
                            }
                        }
                    }
                    dr["C003"] = scoreObject.ID;
                    dr["C004"] = scoreSheet.ID;
                    dr["C005"] = type;
                    dr["C006"] = visualStyle.FontSize;
                    dr["C007"] = visualStyle.StrFontWeight;
                    dr["C008"] = visualStyle.StrFontFamily;
                    dr["C009"] = visualStyle.StrForeColor;
                    dr["C010"] = visualStyle.StrBackColor;
                    dr["C011"] = visualStyle.Width;
                    dr["C012"] = visualStyle.Height;
                    if (isAdd)
                    {
                        objDataSet.Tables[0].Rows.Add(dr);
                        strMsg = string.Format("Insert {0};", visualStyle.ID);
                    }
                    else
                    {
                        strMsg = string.Format("Update {0};", visualStyle.ID);
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

                #endregion

                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn SaveStandardItem(SessionInfo session, StandardItem standardItem)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                ScoreItem scoreItem = standardItem.ScoreItem;
                if (scoreItem == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreItem is null");
                    return optReturn;
                }
                ScoreSheet scoreSheet = scoreItem.ScoreSheet;
                if (scoreSheet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreSheet is null");
                    return optReturn;
                }

                #region 评分标准子项

                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_003_{0} WHERE C001 = {1}",
                           rentToken, standardItem.ID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_003_{0} WHERE C001 = {1}",
                            rentToken, standardItem.ID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
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
                    DataRow dr;
                    bool isAdd = false;
                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        dr = objDataSet.Tables[0].NewRow();
                        dr["C001"] = standardItem.ID;
                        isAdd = true;
                    }
                    else
                    {
                        dr = objDataSet.Tables[0].Rows[0];
                    }
                    dr["C002"] = scoreItem.ID;
                    dr["C003"] = scoreSheet.ID;
                    dr["C004"] = standardItem.OrderID;
                    dr["C005"] = standardItem.Value;
                    dr["C006"] = standardItem.Display;
                    if (isAdd)
                    {
                        objDataSet.Tables[0].Rows.Add(dr);
                        strMsg += string.Format("Insert:{0};", standardItem.ID);
                    }
                    else
                    {
                        strMsg += string.Format("Update:{0}", standardItem.ID);
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message + strSql;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }

                #endregion

                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn SaveCommentItem(SessionInfo session, CommentItem commentItem)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                Comment comment = commentItem.Comment;
                if (comment == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Comment is null");
                    return optReturn;
                }
                ScoreSheet scoreSheet = comment.ScoreSheet;
                if (scoreSheet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreSheet is null");
                    return optReturn;
                }

                #region 备注子项

                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_005_{0} WHERE C001 = {1}",
                            rentToken, commentItem.ID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_005_{0} WHERE C001 = {1}",
                            rentToken, commentItem.ID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
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
                    DataRow dr;
                    bool isAdd = false;
                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        dr = objDataSet.Tables[0].NewRow();
                        dr["C001"] = commentItem.ID;
                        isAdd = true;
                    }
                    else
                    {
                        dr = objDataSet.Tables[0].Rows[0];
                    }
                    dr["C002"] = comment.ID;
                    dr["C003"] = commentItem.OrderID;
                    dr["C005"] = commentItem.Display;
                    if (isAdd)
                    {
                        objDataSet.Tables[0].Rows.Add(dr);
                        strMsg += string.Format("Insert:{0};", commentItem.ID);
                    }
                    else
                    {
                        strMsg += string.Format("Update:{0}", commentItem.ID);
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message + strSql;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }

                #endregion

                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn SaveControlItem(SessionInfo session, ControlItem controlItem)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string rentToken = session.RentInfo.Token;
                ScoreSheet scoreSheet = controlItem.ScoreSheet;
                if (scoreSheet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Comment is null");
                    return optReturn;
                }

                #region 控制项

                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_007_{0} WHERE C001 = {1}",
                            rentToken, controlItem.ID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_007_{0} WHERE C001 = {1}",
                            rentToken, controlItem.ID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
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
                    DataRow dr;
                    bool isAdd = false;
                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        dr = objDataSet.Tables[0].NewRow();
                        dr["C001"] = controlItem.ID;
                        isAdd = true;
                    }
                    else
                    {
                        dr = objDataSet.Tables[0].Rows[0];
                    }
                    dr["C002"] = scoreSheet.ID;
                    dr["C003"] = controlItem.SourceID;
                    dr["C004"] = (int)controlItem.JugeType;
                    dr["C005"] = controlItem.JugeValue1;
                    dr["C006"] = controlItem.JugeValue2;
                    dr["C007"] = controlItem.UsePonitSystem ? 2 : 1;
                    dr["C008"] = controlItem.TargetID;
                    dr["C009"] = controlItem.TargetType;
                    dr["C010"] = (int)controlItem.ChangeType;
                    dr["C011"] = controlItem.ChangeValue;
                    //dr["C009"] = (int)controlItem.ChangeType;
                    //dr["C010"] = controlItem.ChangeValue;
                    //dr["C011"] = controlItem.ChangeType;
                    dr["C012"] = controlItem.OrderID;
                    dr["C013"] = controlItem.Title;
                    if (isAdd)
                    {
                        objDataSet.Tables[0].Rows.Add(dr);
                        strMsg += string.Format("Insert:{0};", controlItem.ID);
                    }
                    else
                    {
                        strMsg += string.Format("Update:{0}", controlItem.ID);
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message + strSql;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }

                #endregion

                optReturn.Data = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn SaveScoreSheetManagement(SessionInfo session, string scoreSheetID, string userID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;

                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 = {1} AND C004 = {2}",
                            rentToken,
                            userID,
                            scoreSheetID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 = {1} AND C004 = {2}",
                            rentToken,
                            userID,
                            scoreSheetID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
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
                    DataRow dr;
                    bool isAdd = false;
                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        dr = objDataSet.Tables[0].NewRow();
                        dr["C003"] = userID;
                        isAdd = true;
                    }
                    else
                    {
                        dr = objDataSet.Tables[0].Rows[0];
                    }
                    dr["C001"] = 0;
                    dr["C002"] = 0;
                    dr["C004"] = scoreSheetID;
                    dr["C005"] = DateTime.Parse("2014/1/1");
                    dr["C006"] = DateTime.Parse("2199/12/31");
                    if (isAdd)
                    {
                        objDataSet.Tables[0].Rows.Add(dr);
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message + strSql;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }

            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }
    }
}