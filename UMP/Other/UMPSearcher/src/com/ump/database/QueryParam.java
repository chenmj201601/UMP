package com.ump.database;
public class QueryParam extends BusinessObject{

    public int iFirstFlag = -1; //�Ƿ���ҳ��־
    //public int iLobFlag = -1; // 0:ȡ�������ģ��ļ��������ͨ��Ŀ 1:ȡ���ļ���ͨ��Ŀ 2:ȡ�����ļ�
    public int[] iIds = null;
    public String strSessionID = null;
    public int iPageSize = -1;
    public int iRecordCount = -1;
    public int iPageCount = -1;
    public String strSql = null;
    public int iCurrPage = -1;
    public int iNextPage = -1;
    public int iObjFlag = -1; //�Ƿ�����operobject��־
}