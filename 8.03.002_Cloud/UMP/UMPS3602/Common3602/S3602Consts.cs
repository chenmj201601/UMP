namespace Common3602
{
    /// <summary>
    /// 常量、特定编码
    /// </summary>
    public static class S3602Consts
    {
        //权限
        public const long OPT_Add = 3602001;
        public const long OPT_Change = 3602002;
        public const long OPT_Search = 3602003;
        public const long OPT_Delete = 3602004;
        public const long OPT_Allot = 3602005;
        public const long OPT_UpLoad = 3602006;
        public const long OPT_DownLoad= 3602007;
        public const long OPT_Play = 3602008;

        //删除成功
        public const string DeleteSuccess = "Delete Success";
        //被已被使用无法删除
        public const string HadUse = "Had Use";
        //试卷已经存在
        public const string PaperExist = "Paper Exist";

        //面板序号
        public const int PANEL_ID_TREE = 1;
        public const int PANEL_ID_SEARCHINFO = 2;
        //面板名称
        public const string PANEL_NAME_TREE = "PanelObjectTreeBox";
        public const string PANEL_NAME_SEARCHINFO = "PanelSearchInfo";

        //面板ContentID
        public const string PANEL_CONTENTID_TREE = "PanelObjectTreeBox";
        public const string PANEL_CONTENTID_SEARCHINFO = "PanelSearchInfo";

        //Table序号
        public const int TABLE_ID_AT = 1;
        //Table名称
        public const string TABLE_NAME_AT = "AddQuestionsDocument";
        //Table ContentID
        public const string TABLE_CONNTENTID_AT = "AddQuestionsDocument";

        //按钮序号
        public const int BUT_ID_TRUNTOFALSE = 1;
        //按钮名称
        public const string BUT_NAME_TRUNTOFALSE = "TrunOrFalse";
        //按钮ContentID
        public const string BUT_CONTENTID_TRUNTOFALSE = "TOFPanelLearnDocument";
    }
}
