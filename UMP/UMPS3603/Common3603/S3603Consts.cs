namespace Common3603
{
    /// <summary>
    /// 常量、特定编码
    /// </summary>
    public static class S3603Consts
    {
        //权限
        public const long OPT_Add = 3603001;
        public const long OPT_Change = 3603002;
        public const long OPT_Search = 3603003;
        public const long OPT_Delete = 3603004;
        public const long OPT_Allot = 3603005;
        public const long OPT_UpLoad = 3603006;
        public const long OPT_DownLoad= 3603007;
        public const long OPT_Play = 3603008;
        public const long OPT_LoadUser = 3603009;
        public const long OPT_SetUser = 3603010;
        //面板序号
        public const int PANEL_ID_EXAMINFO = 1;
        public const int PANEL_ID_PAPER = 2;
        //面板名称
        public const string PANEL_NAME_EXAMINFO = "ExamineeInformationTable";
        public const string PANEL_NAME_PAPER = "PanelObjectPaper";

        //面板ContentID
        public const string PANEL_CONTENTID_EXAMINFO = "ExamineeInformationTable";
        public const string PANEL_CONTENTID_PAPER = "PanelObjectPaper";

        //删除成功
        public const string DeleteSuccess = "Delete Success";
        //被已被使用无法删除
        public const string HadUse = "Had Use";
        //试卷已经存在
        public const string PaperExist = "Paper Exist";
    }
}
