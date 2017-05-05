package com.ump.database;
public class QueryParam extends BusinessObject{

    public int iFirstFlag = -1; //是否首页标志
    //public int iLobFlag = -1; // 0:取除了正文，文件以外的普通项目 1:取正文及普通项目 2:取附带文件
    public int[] iIds = null;
    public String strSessionID = null;
    public int iPageSize = -1;
    public int iRecordCount = -1;
    public int iPageCount = -1;
    public String strSql = null;
    public int iCurrPage = -1;
    public int iNextPage = -1;
    public int iObjFlag = -1; //是否启用operobject标志
}