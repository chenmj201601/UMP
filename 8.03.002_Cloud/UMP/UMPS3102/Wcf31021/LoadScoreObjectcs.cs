using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.ScoreSheets;

namespace Wcf31021
{
    public partial class Service31021
    {
        private OperationReturn LoadScoreSheet(SessionInfo session, string scoreSheetID,string scoreID)
        {
            OperationReturn optReturn = new OperationReturn(); 
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_31_001_{0} WHERE C001 = {1}",
                                rentToken,
                                scoreSheetID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_31_001_{0} WHERE C001 = {1}",
                                rentToken,
                                scoreSheetID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
                        return optReturn;
                }
                if (objDataSet == null || objDataSet.Tables.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null or DataTables empty");
                    return optReturn;
                }
                if (objDataSet.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = string.Format("ScoreSheet not exist");
                    return optReturn;
                }
                DataRow dr = objDataSet.Tables[0].Rows[0];
                ScoreSheet scoreSheet = new ScoreSheet();
                scoreSheet.Type = ScoreObjectType.ScoreSheet;
                scoreSheet.ScoreSheet = scoreSheet;
                scoreSheet.ID = Convert.ToInt64(dr["C001"]);
                scoreSheet.Title = dr["C002"].ToString();
                scoreSheet.ViewClassic = dr["C003"].ToString() == "C" ? ScoreItemClassic.Table : ScoreItemClassic.Tree;
                scoreSheet.TotalScore = Convert.ToDouble(dr["C004"]);
                scoreSheet.Creator = Convert.ToInt64(dr["C005"]);
                scoreSheet.CreateTime = Convert.ToDateTime(dr["C006"]);
                scoreSheet.DateFrom = Convert.ToDateTime(dr["C009"]);
                scoreSheet.DateTo = Convert.ToDateTime(dr["C010"]);
                scoreSheet.QualifiedLine = Convert.ToDouble(dr["C011"]);
                scoreSheet.ScoreType = dr["C014"].ToString() == "F"
                    ? ScoreType.YesNo
                    : dr["C014"].ToString() == "P" ? ScoreType.Pecentage : ScoreType.Numeric;
                scoreSheet.UseTag = Convert.ToInt32(dr["C017"]);
                scoreSheet.Description = dr["C019"].ToString();
                scoreSheet.CalAdditionalItem = dr["C020"].ToString() != "N";
                scoreSheet.UsePointSystem = dr["C021"].ToString() != "N";
                string strScoreWidth = dr["C101"].ToString();
                string strTipWidth = dr["C102"].ToString();
                double intScoreWidth, intTipWidth;
                if (double.TryParse(strScoreWidth, out intScoreWidth))
                {
                    scoreSheet.ScoreWidth = intScoreWidth;
                }
                if (double.TryParse(strTipWidth, out intTipWidth))
                {
                    scoreSheet.TipWidth = intTipWidth;
                }
                scoreSheet.HasAutoStandard = dr["C103"].ToString() != "N";
                scoreSheet.AllowModifyScore = dr["C104"].ToString() != "N";

                #region 子项
                if (scoreID == "0")
                {
                    scoreID = string.Empty;
                }
                else
                {
                    scoreID = string.Format(" AND T3109.C003={0}", scoreID);
                }
                //加载子项
                optReturn = LoadScoreItem(session, scoreSheet, scoreID);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                List<ScoreItem> listItems = optReturn.Data as List<ScoreItem>;
                if (listItems == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ListItems is null");
                    return optReturn;
                }
                for (int i = 0; i < listItems.Count; i++)
                {
                    ScoreItem scoreItem = listItems[i];
                    scoreSheet.Items.Add(scoreItem);
                }
                scoreSheet.Items = scoreSheet.Items.OrderBy(i => i.OrderID).ToList();

                #endregion 子项


                #region 样式

                OperationReturn styleReturn;
                styleReturn = LoadVisualStyle(session, scoreSheet, "T");
                if (!styleReturn.Result)
                {
                    if (styleReturn.Code != Defines.RET_NOT_EXIST)
                    {
                        return styleReturn;
                    }
                    scoreSheet.TitleStyle = null;
                }
                else
                {
                    VisualStyle style = styleReturn.Data as VisualStyle;
                    if (style != null)
                    {
                        style.ScoreSheet = scoreSheet;
                        scoreSheet.TitleStyle = style;
                    }
                }

                styleReturn = LoadVisualStyle(session, scoreSheet, "P");
                if (!styleReturn.Result)
                {
                    if (styleReturn.Code != Defines.RET_NOT_EXIST)
                    {
                        return styleReturn;
                    }
                    scoreSheet.PanelStyle = null;
                }
                else
                {
                    VisualStyle style = styleReturn.Data as VisualStyle;
                    if (style != null)
                    {
                        style.ScoreSheet = scoreSheet;
                        scoreSheet.PanelStyle = style;
                    }
                }

                #endregion


                #region 备注

                OperationReturn commentReturn;
                commentReturn = LoadComment(session, scoreSheet);
                if (!commentReturn.Result)
                {
                    return commentReturn;
                }
                List<Comment> listComments = commentReturn.Data as List<Comment>;
                if (listComments == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("listComments is null");
                    return optReturn;
                }
                for (int i = 0; i < listComments.Count; i++)
                {
                    Comment comment = listComments[i];
                    scoreSheet.Comments.Add(comment);
                }
                scoreSheet.Comments = scoreSheet.Comments.OrderBy(j => j.OrderID).ToList();

                #endregion


                #region 控制项

                OperationReturn controlReturn;
                controlReturn = LoadControlItem(session, scoreSheet);
                if (!controlReturn.Result)
                {
                    return controlReturn;
                }
                List<ControlItem> listControlItems = controlReturn.Data as List<ControlItem>;
                if (listControlItems == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ListCotnrolItems is null");
                    return optReturn;
                }
                for (int i = 0; i < listControlItems.Count; i++)
                {
                    ControlItem controlItem = listControlItems[i];
                    scoreSheet.ControlItems.Add(controlItem);
                }
                scoreSheet.ControlItems = scoreSheet.ControlItems.OrderBy(j => j.OrderID).ToList();
                #endregion

                optReturn.Data = scoreSheet;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn LoadScoreItem(SessionInfo session, ScoreItem parent,string scoreID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                ScoreSheet scoreSheet = parent.ScoreSheet;
                if (scoreSheet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreSheet is null");
                    return optReturn;
                }
                string temp1 = string.Empty;
                string temp2 = string.Empty;
                if (!string.IsNullOrWhiteSpace(scoreID))
                {
                    temp1 = string.Format(",T3109.C005 AS NA");
                    temp2 = string.Format(" LEFT JOIN T_31_009_{0} T3109 ON T3102.C002=T3109.C002", rentToken);
                }
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format(
                            "SELECT T3102.*{4} FROM T_31_002_{0} T3102{5}  WHERE T3102.C003 = {1} AND T3102.C004 = {2}{3} ",
                                rentToken, scoreSheet.ID, parent.ID, scoreID, temp1, temp2);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =string.Format(
                            "SELECT T3102.*{4} FROM T_31_002_{0} T3102{5} WHERE T3102.C003 = {1} AND T3102.C004 = {2}{3} ",
                                rentToken, scoreSheet.ID, parent.ID, scoreID, temp1, temp2);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
                        return optReturn;
                }
                if (objDataSet == null || objDataSet.Tables.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null or DataTables empty");
                    return optReturn;
                }
                List<ScoreItem> listItems = new List<ScoreItem>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    ScoreItem scoreItem;
                    bool isStandard = dr["C009"].ToString() == "Y";
                    if (isStandard)
                    {
                        Standard standard = null;
                        StandardType standardType = (StandardType)Convert.ToInt32(dr["C015"]);
                        switch (standardType)
                        {
                            case StandardType.Numeric:
                                NumericStandard numericStandard = new NumericStandard();
                                numericStandard.Type = ScoreObjectType.NumericStandard;
                                numericStandard.MaxValue = Convert.ToDouble(dr["C027"]);
                                numericStandard.MinValue = Convert.ToDouble(dr["C028"]);
                                numericStandard.DefaultValue = string.IsNullOrWhiteSpace(dr["C029"].ToString()) ? 0 : Convert.ToDouble(dr["C029"]);
                                numericStandard.ScoreClassic = StandardClassic.TextBox;
                                standard = numericStandard;
                                break;
                            case StandardType.YesNo:
                                YesNoStandard yesNoStandard = new YesNoStandard();
                                yesNoStandard.Type = ScoreObjectType.YesNoStandard;
                                if (string.IsNullOrWhiteSpace(dr["C029"].ToString())) 
                                { 
                                    yesNoStandard.DefaultValue = false; 
                                }
                                else
                                {
                                    if (Convert.ToDouble(dr["C029"]) == 1.0)
                                    {
                                        yesNoStandard.DefaultValue = true;
                                    }
                                    else
                                    {
                                        yesNoStandard.DefaultValue = false;
                                    }
                                }
                                yesNoStandard.ReverseDisplay = dr["C104"].ToString() == "1";
                                yesNoStandard.ScoreClassic = StandardClassic.YesNo;
                                standard = yesNoStandard;
                                break;
                            case StandardType.Slider:
                                SliderStandard sliderStandard = new SliderStandard();
                                sliderStandard.Type = ScoreObjectType.SliderStandard;
                                sliderStandard.MinValue = Convert.ToDouble(dr["C023"]);
                                sliderStandard.MaxValue = Convert.ToDouble(dr["C024"]);
                                sliderStandard.Interval = Convert.ToDouble(dr["C026"]);
                                sliderStandard.DefaultValue = string.IsNullOrWhiteSpace(dr["C029"].ToString()) ? 0 : Convert.ToDouble(dr["C029"]);
                                sliderStandard.ScoreClassic = StandardClassic.Slider;
                                standard = sliderStandard;
                                break;
                            case StandardType.Item:
                                ItemStandard itemStandard = new ItemStandard();
                                itemStandard.Type = ScoreObjectType.ItemStandard;
                                itemStandard.ScoreClassic = StandardClassic.DropDownList;
                                itemStandard.DefaultIndex =string.IsNullOrWhiteSpace(dr["C029"].ToString())?0: Convert.ToInt32(dr["C029"]);
                                standard = itemStandard;
                                break;
                        }
                        if (standard == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("Standard is null");
                            return optReturn;
                        }
                        standard.PointSystem = Convert.ToDouble(dr["C018"]);
                        standard.StandardType = standardType;
                        standard.IsAutoStandard = dr["C101"].ToString() == "Y";
                        if (!string.IsNullOrWhiteSpace(scoreID))
                        {
                            if (string.IsNullOrWhiteSpace(dr["NA"].ToString()))
                            {
                                standard.IsNA = false;
                            }
                            else
                            {
                                standard.IsNA = dr["NA"].ToString() == "Y" ? true : false;
                            }
                        }
                        string strStaID = dr["C102"].ToString();
                        long staID;
                        if (long.TryParse(strStaID, out staID))
                        {
                            standard.StatisticalID = staID;
                        }
                        scoreItem = standard;
                    }
                    else
                    {
                        ScoreGroup scoreGroup = new ScoreGroup();
                        scoreGroup.Type = ScoreObjectType.ScoreGroup;
                        scoreGroup.IsAvg = dr["C016"].ToString() == "Y";
                        scoreItem = scoreGroup;
                    }
                    scoreItem.ScoreSheet = parent.ScoreSheet;
                    scoreItem.Parent = parent;
                    scoreItem.ItemID = Convert.ToInt32(dr["C001"]);
                    scoreItem.ID = Convert.ToInt64(dr["C002"]);
                    scoreItem.OrderID = Convert.ToInt32(dr["C005"]);
                    scoreItem.Title = dr["C006"].ToString();
                    scoreItem.Description = dr["C007"].ToString();
                    scoreItem.TotalScore = Convert.ToDouble(dr["C008"]);
                    scoreItem.IsAbortScore = dr["C010"].ToString() == "Y";
                    scoreItem.ControlFlag = dr["C011"].ToString() == "Y" ? 1 : 0;
                    scoreItem.IsKeyItem = dr["C012"].ToString() == "Y";
                    scoreItem.IsAllowNA = dr["C013"].ToString() == "Y";
                    scoreItem.IsJumpItem = dr["C014"].ToString() == "Y";
                    scoreItem.IsAddtionItem = dr["C017"].ToString() == "Y";
                    scoreItem.UsePointSystem = dr["C019"].ToString() == "Y";
                    scoreItem.ScoreType = dr["C020"].ToString() == "F"
                        ? ScoreType.YesNo
                        : dr["C020"].ToString() == "P" ? ScoreType.Pecentage : ScoreType.Numeric;
                    scoreItem.Tip = dr["C022"].ToString();
                    scoreItem.AllowModifyScore = dr["C103"].ToString() == "Y";

                    #region 子项

                    //如果是 ScoreGroup 加载子项
                    if (!isStandard)
                    {
                        ScoreGroup scoreGroup = scoreItem as ScoreGroup;
                        optReturn = LoadScoreItem(session, scoreGroup,scoreID);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        List<ScoreItem> subItems = optReturn.Data as List<ScoreItem>;
                        if (subItems == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("ListItems is null");
                            return optReturn;
                        }
                        for (int j = 0; j < subItems.Count; j++)
                        {
                            ScoreItem subItem = subItems[j];
                            scoreGroup.Items.Add(subItem);
                        }
                        scoreGroup.Items = scoreGroup.Items.OrderBy(j => j.OrderID).ToList();
                    }

                    //如果是多值型评分标准，加载评分标准子项
                    ItemStandard temp = scoreItem as ItemStandard;
                    if (temp != null)
                    {
                        optReturn = LoadStandardItem(session, temp);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        List<StandardItem> valueItems = optReturn.Data as List<StandardItem>;
                        if (valueItems == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("ValueItems is null");
                            return optReturn;
                        }
                        for (int j = 0; j < valueItems.Count; j++)
                        {
                            StandardItem valueItem = valueItems[j];
                            temp.ValueItems.Add(valueItem);
                        }
                        temp.ValueItems = temp.ValueItems.OrderBy(j => j.OrderID).ToList();
                    }

                    #endregion


                    #region 备注

                    OperationReturn commentReturn;
                    commentReturn = LoadComment(session, scoreItem);
                    if (!commentReturn.Result)
                    {
                        return commentReturn;
                    }
                    List<Comment> listComments = commentReturn.Data as List<Comment>;
                    if (listComments == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("listComments is null");
                        return optReturn;
                    }
                    for (int j = 0; j < listComments.Count; j++)
                    {
                        Comment comment = listComments[j];
                        scoreItem.Comments.Add(comment);
                    }
                    scoreItem.Comments = scoreItem.Comments.OrderBy(j => j.OrderID).ToList();

                    #endregion

                    #region 样式

                    OperationReturn styleReturn;
                    styleReturn = LoadVisualStyle(session, scoreItem, "T");
                    if (!styleReturn.Result)
                    {
                        if (styleReturn.Code != Defines.RET_NOT_EXIST)
                        {
                            return styleReturn;
                        }
                        scoreItem.TitleStyle = null;
                    }
                    else
                    {
                        VisualStyle style = styleReturn.Data as VisualStyle;
                        if (style != null)
                        {
                            style.ScoreSheet = scoreSheet;
                            scoreItem.TitleStyle = style;
                        }
                    }

                    styleReturn = LoadVisualStyle(session, scoreItem, "P");
                    if (!styleReturn.Result)
                    {
                        if (styleReturn.Code != Defines.RET_NOT_EXIST)
                        {
                            return styleReturn;
                        }
                        scoreItem.PanelStyle = null;
                    }
                    else
                    {
                        VisualStyle style = styleReturn.Data as VisualStyle;
                        if (style != null)
                        {
                            style.ScoreSheet = scoreSheet;
                            scoreItem.PanelStyle = style;
                        }
                    }

                    #endregion

                    listItems.Add(scoreItem);
                }
                optReturn.Data = listItems;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn LoadComment(SessionInfo session, ScoreItem scoreItem)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                ScoreSheet scoreSheet = scoreItem.ScoreSheet;
                if (scoreSheet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreSheet is null");
                    return optReturn;
                }
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                          string.Format(
                              "SELECT * FROM T_31_004_{0} WHERE C002 = {1} AND C003 = {2}",
                              rentToken,
                              scoreItem.ID,
                              scoreSheet.ID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                          string.Format(
                              "SELECT * FROM T_31_004_{0} WHERE C002 = {1} AND C003 = {2}",
                              rentToken,
                              scoreItem.ID,
                              scoreSheet.ID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
                        return optReturn;
                }
                if (objDataSet == null || objDataSet.Tables.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null or DataTables empty");
                    return optReturn;
                }
                List<Comment> listComments = new List<Comment>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    Comment comment;
                    int intCommentStyle = Convert.ToInt32(dr["C008"]);
                    switch (intCommentStyle)
                    {
                        case (int)CommentStyle.Text:
                            TextComment textComment = new TextComment();
                            textComment.Type = ScoreObjectType.TextComment;
                            textComment.DefaultText = dr["C005"].ToString();
                            comment = textComment;
                            break;
                        case (int)CommentStyle.Item:
                            ItemComment itemComment = new ItemComment();
                            itemComment.Type = ScoreObjectType.ItemComment;
                            comment = itemComment;
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_NOT_EXIST;
                            optReturn.Message = string.Format("CommentStyle not exist");
                            return optReturn;
                    }
                    comment.Style = (CommentStyle)intCommentStyle;
                    comment.ID = Convert.ToInt64(dr["C001"]);
                    comment.ScoreItem = scoreItem;
                    comment.ScoreSheet = scoreSheet;
                    comment.OrderID = Convert.ToInt32(dr["C004"]);
                    comment.Title = dr["C009"].ToString();

                    #region 样式

                    OperationReturn styleReturn;
                    styleReturn = LoadVisualStyle(session, comment, "T");
                    if (!styleReturn.Result)
                    {
                        if (styleReturn.Code != Defines.RET_NOT_EXIST)
                        {
                            return styleReturn;
                        }
                        comment.TitleStyle = null;
                    }
                    else
                    {
                        VisualStyle style = styleReturn.Data as VisualStyle;
                        if (style != null)
                        {
                            style.ScoreSheet = scoreSheet;
                            comment.TitleStyle = style;
                        }
                    }

                    styleReturn = LoadVisualStyle(session, comment, "P");
                    if (!styleReturn.Result)
                    {
                        if (styleReturn.Code != Defines.RET_NOT_EXIST)
                        {
                            return styleReturn;
                        }
                        comment.PanelStyle = null;
                    }
                    else
                    {
                        VisualStyle style = styleReturn.Data as VisualStyle;
                        if (style != null)
                        {
                            style.ScoreSheet = scoreSheet;
                            comment.PanelStyle = style;
                        }
                    }

                    #endregion


                    #region 子项

                    var temp = comment as ItemComment;
                    if (temp != null)
                    {
                        OperationReturn commentItemReturn;
                        commentItemReturn = LoadCommentItem(session, temp);
                        if (!commentItemReturn.Result)
                        {
                            return commentItemReturn;
                        }
                        List<CommentItem> listItems = commentItemReturn.Data as List<CommentItem>;
                        if (listItems == null)
                        {
                            commentItemReturn.Result = false;
                            commentItemReturn.Code = Defines.RET_OBJECT_NULL;
                            commentItemReturn.Message = string.Format("ListCommentItems is null");
                            return commentItemReturn;
                        }
                        for (int j = 0; j < listItems.Count; j++)
                        {
                            temp.ValueItems.Add(listItems[j]);
                        }
                        temp.ValueItems = temp.ValueItems.OrderBy(j => j.OrderID).ToList();
                    }


                    #endregion

                    listComments.Add(comment);
                }
                optReturn.Data = listComments;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn LoadVisualStyle(SessionInfo session, ScoreObject scoreObject, string type)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                if (scoreObject == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreObject is null");
                    return optReturn;
                }
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                           string.Format(
                               "SELECT * FROM T_31_012_{0} WHERE C003 = {1} AND C005 = '{2}'",
                               rentToken,
                               scoreObject.ID,
                               type);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_31_012_{0} WHERE C003 = {1} AND C005 = '{2}'",
                                rentToken,
                                scoreObject.ID,
                                type);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
                        return optReturn;
                }
                if (objDataSet == null || objDataSet.Tables.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null or DataTables empty");
                    return optReturn;
                }
                if (objDataSet.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = string.Format("VisualStyle not exist");
                    return optReturn;
                }
                DataRow dr = objDataSet.Tables[0].Rows[0];
                VisualStyle style = new VisualStyle();
                style.ScoreObject = scoreObject;
                style.Type = ScoreObjectType.VisualStyle;
                style.ID = Convert.ToInt64(dr["C001"]);
                style.FontSize = Convert.ToInt32(dr["C006"]);
                style.StrFontWeight = dr["C007"].ToString();
                style.StrFontFamily = dr["C008"].ToString();
                style.StrForeColor = dr["C009"].ToString();
                style.StrBackColor = dr["C010"].ToString();
                style.Width = Convert.ToInt32(dr["C011"]);
                style.Height = Convert.ToInt32(dr["C012"]);

                optReturn.Data = style;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn LoadStandardItem(SessionInfo session, ItemStandard itemStandard)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                ScoreSheet scoreSheet = itemStandard.ScoreSheet;
                if (scoreSheet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreSheet is null");
                    return optReturn;
                }
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                        string.Format(
                            "SELECT * FROM T_31_003_{0} WHERE C002 = {1}",
                            rentToken,
                            itemStandard.ID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                          string.Format(
                              "SELECT * FROM T_31_003_{0} WHERE C002 = {1}",
                              rentToken,
                              itemStandard.ID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
                        return optReturn;
                }
                if (objDataSet == null || objDataSet.Tables.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null or DataTables empty");
                    return optReturn;
                }
                List<StandardItem> listStandardItems = new List<StandardItem>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    StandardItem standardItem = new StandardItem();
                    standardItem.Type = ScoreObjectType.StandardItem;
                    standardItem.ID = Convert.ToInt64(dr["C001"]);
                    standardItem.ScoreItem = itemStandard;
                    standardItem.OrderID = Convert.ToInt32(dr["C004"]);
                    standardItem.Value = Convert.ToDouble(dr["C005"]);
                    standardItem.Display = dr["C006"].ToString();

                    listStandardItems.Add(standardItem);
                }
                optReturn.Data = listStandardItems;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn LoadCommentItem(SessionInfo session, ItemComment itemComment)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                ScoreSheet scoreSheet = itemComment.ScoreSheet;
                if (scoreSheet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreSheet is null");
                    return optReturn;
                }
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                          string.Format(
                              "SELECT * FROM T_31_005_{0} WHERE C002 = {1}",
                              rentToken,
                              itemComment.ID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                          string.Format(
                              "SELECT * FROM T_31_005_{0} WHERE C002 = {1}",
                              rentToken,
                              itemComment.ID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
                        return optReturn;
                }
                if (objDataSet == null || objDataSet.Tables.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null or DataTables empty");
                    return optReturn;
                }
                List<CommentItem> listCommentItems = new List<CommentItem>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    CommentItem commentItem = new CommentItem();
                    commentItem.Type = ScoreObjectType.CommentItem;
                    commentItem.ID = Convert.ToInt64(dr["C001"]);
                    commentItem.Comment = itemComment;
                    commentItem.OrderID = Convert.ToInt32(dr["C003"]);
                    commentItem.Text = dr["C005"].ToString();

                    listCommentItems.Add(commentItem);
                }
                optReturn.Data = listCommentItems;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn LoadControlItem(SessionInfo session, ScoreSheet scoreSheet)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                          string.Format(
                              "SELECT * FROM T_31_007_{0} WHERE C002 = {1}",
                              rentToken,
                              scoreSheet.ID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                          string.Format(
                              "SELECT * FROM T_31_007_{0} WHERE C002 = {1}",
                              rentToken,
                              scoreSheet.ID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
                        return optReturn;
                }
                if (objDataSet == null || objDataSet.Tables.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null or DataTables empty");
                    return optReturn;
                }
                List<ControlItem> listControlItems = new List<ControlItem>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    ControlItem controlItem = new ControlItem();
                    controlItem.Type = ScoreObjectType.ControlItem;
                    controlItem.ID = Convert.ToInt64(dr["C001"]);
                    controlItem.ScoreSheet = scoreSheet;
                    controlItem.SourceID = Convert.ToInt64(dr["C003"]);
                    controlItem.JugeType = (JugeType)Convert.ToInt32(dr["C004"]);
                    controlItem.JugeValue1 = Convert.ToDouble(dr["C005"]);
                    controlItem.JugeValue2 = string.IsNullOrEmpty(dr["C006"].ToString())
                        ? 0
                        : Convert.ToDouble(dr["C006"]);
                    controlItem.UsePonitSystem = dr["C007"].ToString() == "2";
                    controlItem.TargetID = Convert.ToInt64(dr["C008"]);
                    controlItem.TargetType = Convert.ToInt32(dr["C009"]);
                    controlItem.ChangeType = (ChangeType)Convert.ToInt32(dr["C010"]);
                    controlItem.ChangeValue = Convert.ToDouble(dr["C011"]);
                    controlItem.OrderID = Convert.ToInt32(dr["C012"]);
                    controlItem.Title = dr["C013"].ToString();

                    listControlItems.Add(controlItem);
                }
                optReturn.Data = listControlItems;
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