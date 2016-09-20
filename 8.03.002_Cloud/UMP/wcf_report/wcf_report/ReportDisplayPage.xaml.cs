using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoiceCyber.UMP.Controls;
using UMPS6101.Wcf61011;
using UMPS6101.DataSource;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.Common;
using System.ComponentModel;
using UMPS6101.ReportQueryPage;
using System.Text.RegularExpressions;
using UMPS6101.Wcf11012;
using UMPS6101.Models;
using UMPS1101.Models;
using System.Collections.ObjectModel;
using System.Timers;
using Common61011;
using System.Xml;
using UMPS6101.Sharing_Classes;
using System.IO;
using UMPS6101.Wcf61012;

namespace UMPS6101
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class ReportDisplayPage
    {
        #region Members
        public string ReportName;
        private BackgroundWorker mWorker;
        private int number = 0;
        //private int SelectNum = 0;
        private ReportLang RL = new ReportLang();
        public List<string> StrQuery = new List<string>();
        //Is show report?
        private bool ReportShow = false;
        private bool ReportResult = true;
        private ObjectItem mRoot;
        private ObservableCollection<ReportNameTree> ListReport = new ObservableCollection<ReportNameTree>();
        //private List<string> ListReportNum = new List<string>();
        ReportParameter[] rp = new ReportParameter[7];

        Rp1query rp1querypage = new Rp1query();
        Rp2query rp2querypage = new Rp2query();
        Rp3query rp3querypage = new Rp3query();
        Rp4query rp4querypage = new Rp4query();
        Rp5query rp5querypage = new Rp5query();
        Rp6query rp6querypage = new Rp6query();
        Rp7query rp7querypage = new Rp7query();
        Rp8query rp8querypage = new Rp8query();
        Rp9query rp9querypage = new Rp9query();
        Rp10query rp10querypage = new Rp10query();
        Rp11query rp11querypage = new Rp11query();
        Rp12query rp12querypage = new Rp12query();
        Rp13query rp13querypage = new Rp13query();
        Rp14query rp14querypage = new Rp14query();
        Rp15query rp15querypage = new Rp15query();
        Rp16query rp16querypage = new Rp16query();
        Rp17query rp17querypage = new Rp17query();
        Rp18query rp18querypage = new Rp18query();
        Rp19query rp19querypage = new Rp19query();
        Rp20query rp20querypage = new Rp20query();
        Rp21query rp21querypage = new Rp21query();
        Rp22query rp22querypage = new Rp22query();
        Rp23query rp23querypage = new Rp23query();
        Rp24query rp24querypage = new Rp24query();
        Rp25query rp25querypage = new Rp25query();

        ObservableCollection<AgentScoreSheetStatistics> mListAgentScoreSheetStatistics;
        #endregion
        private S6101App s6101App;
        public ReportDisplayPage()
        {
            InitializeComponent();
            mRoot = new ObjectItem();
            ReportName = string.Empty;
            mListAgentScoreSheetStatistics = new ObservableCollection<AgentScoreSheetStatistics>();
            this.ListViewReport.ItemsSource = mListAgentScoreSheetStatistics;
            this.TvReport.ItemsSource = mRoot.Children;
            this.ReportViewer1.ReportExport += ReportViewer1_ReportExport;
            BtnQuery.Click += BtnQuery_Click;
            this.TvReport.MouseDoubleClick += TvReport_MouseDoubleClick;
            this.TvReport.MouseDown += TvReport_MouseDown;
            this.TvReport.SelectedItemChanged += TvReport_SelectedItemChanged;

            //export
            //this.ButExcel.Click += ButExcel_Click;
            //this.ButHTML.Click += ButHTML_Click;
            //this.ButPDF.Click += ButPDF_Click;
            //this.ButWord.Click += ButWord_Click;
            //this.ButJPG.Click += ButJPG_Click;
        }
        #region export click
        void ButExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (number == 0) { return; }
                //OpeanPathSelect
                System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.Title = CurrentApp.GetLanguageInfo("1101T10216", "选择路径");
                dlg.FileName = string.Format("Report{0}.xls", number);
                dlg.Filter = "xls files (*.xlsx)|*.xlsx| (*.xls)|*.xls";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //检查路径正确性        
                    if (dlg.FileName.Trim().Length <= 0)
                    {
                        CurrentApp.GetLanguageInfo("1101N034", "请选择路径。");
                        return;
                    }
                    //检查文件夹是否存在        
                    int n = dlg.FileName.LastIndexOf(@"\") + 1;
                    if (n < 0)
                    {
                        CurrentApp.GetLanguageInfo("1101N035", "路径错误。");
                        return;
                    }
                    string str = dlg.FileName.Substring(0, n);
                    if (!Directory.Exists(str))
                    {
                        CurrentApp.GetLanguageInfo("1101N036", "路径不存在，请检查。");
                        return;
                    }
                    string file = dlg.FileName;
                    var data = ReportViewer1.LocalReport.Render("Excel");
                    File.WriteAllBytes(file, data);
                    CurrentApp.ShowInfoMessage(": )");
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ButWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (number == 0) { return; }
                //OpeanPathSelect
                System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.Title = CurrentApp.GetLanguageInfo("1101T10216", "选择路径");
                dlg.FileName = string.Format("Report{0}.doc", number);
                dlg.Filter = "doc files (*.doc)|*.doc|(*.docx)|*.docx";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //检查路径正确性        
                    if (dlg.FileName.Trim().Length <= 0)
                    {
                        CurrentApp.GetLanguageInfo("1101N034", "请选择路径。");
                        return;
                    }
                    //检查文件夹是否存在        
                    int n = dlg.FileName.LastIndexOf(@"\") + 1;
                    if (n < 0)
                    {
                        CurrentApp.GetLanguageInfo("1101N035", "路径错误。");
                        return;
                    }
                    string str = dlg.FileName.Substring(0, n);
                    if (!Directory.Exists(str))
                    {
                        CurrentApp.GetLanguageInfo("1101N036", "路径不存在，请检查。");
                        return;
                    }
                    string file = dlg.FileName;
                    var data = ReportViewer1.LocalReport.Render("Word");
                    File.WriteAllBytes(file, data);
                    CurrentApp.ShowInfoMessage(": )");
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ButPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (number == 0) { return; }
                //OpeanPathSelect
                System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.Title = CurrentApp.GetLanguageInfo("1101T10216", "选择路径");
                dlg.FileName = string.Format("Report{0}.pdf", number);
                dlg.Filter = "pdf files (*.pdf)|*.pdf";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //检查路径正确性        
                    if (dlg.FileName.Trim().Length <= 0)
                    {
                        CurrentApp.GetLanguageInfo("1101N034", "请选择路径。");
                        return;
                    }
                    //检查文件夹是否存在        
                    int n = dlg.FileName.LastIndexOf(@"\") + 1;
                    if (n < 0)
                    {
                        CurrentApp.GetLanguageInfo("1101N035", "路径错误。");
                        return;
                    }
                    string PathStr = dlg.FileName.Substring(0, n);
                    if (!Directory.Exists(PathStr))
                    {
                        CurrentApp.GetLanguageInfo("1101N036", "路径不存在，请检查。");
                        return;
                    }
                    string file = dlg.FileName;
                    var data = ReportViewer1.LocalReport.Render("PDF");
                    File.WriteAllBytes(file, data);
                    CurrentApp.ShowInfoMessage(": )");
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ButHTML_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (number == 0) { return; }
                //OpeanPathSelect
                System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.Title = CurrentApp.GetLanguageInfo("1101T10216", "选择路径");
                dlg.FileName = string.Format("Report{0}.html", number);
                dlg.Filter = "html files (*.html)|*.html";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //检查路径正确性        
                    if (dlg.FileName.Trim().Length <= 0)
                    {
                        CurrentApp.GetLanguageInfo("1101N034", "请选择路径。");
                        return;
                    }
                    //检查文件夹是否存在        
                    int n = dlg.FileName.LastIndexOf(@"\") + 1;
                    if (n < 0)
                    {
                        CurrentApp.GetLanguageInfo("1101N035", "路径错误。");
                        return;
                    }
                    string PathStr = dlg.FileName.Substring(0, n);
                    if (!Directory.Exists(PathStr))
                    {
                        CurrentApp.GetLanguageInfo("1101N036", "路径不存在，请检查。");
                        return;
                    }
                    //Format
                    string file = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        string.Format("UMP\\{0}\\Report{1}.xls", CurrentApp.AppName, number));
                    string html = dlg.FileName;
                    var data = ReportViewer1.LocalReport.Render("Excel");
                    File.WriteAllBytes(file, data);
                    var app = new Microsoft.Office.Interop.Excel.Application();
                    Microsoft.Office.Interop.Excel.Workbook workbook;
                    workbook = app.Application.Workbooks.Open(file);
                    var ofmt = Microsoft.Office.Interop.Excel.XlFileFormat.xlHtml;
                    workbook.SaveAs(html, ofmt);
                    workbook.Close();
                    app.Quit();
                    CurrentApp.ShowInfoMessage(": )");
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ButJPG_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (number == 0) { return; }
                //OpeanPathSelect
                System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.Title = CurrentApp.GetLanguageInfo("1101T10216", "选择路径");
                dlg.FileName = string.Format("Report{0}.jpg", number);
                dlg.Filter = "jpg files (*.jpg)|*.jpg";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //检查路径正确性        
                    if (dlg.FileName.Trim().Length <= 0)
                    {
                        CurrentApp.GetLanguageInfo("1101N034", "请选择路径。");
                        return;
                    }
                    //检查文件夹是否存在        
                    int n = dlg.FileName.LastIndexOf(@"\") + 1;
                    if (n < 0)
                    {
                        CurrentApp.GetLanguageInfo("1101N035", "路径错误。");
                        return;
                    }
                    string PathStr = dlg.FileName.Substring(0, n);
                    if (!Directory.Exists(PathStr))
                    {
                        CurrentApp.GetLanguageInfo("1101N036", "路径不存在，请检查。");
                        return;
                    }
                    //Format
                    string file = dlg.FileName;
                    var data = ReportViewer1.LocalReport.Render("Image");
                    File.WriteAllBytes(file, data);
                    CurrentApp.ShowInfoMessage(": )");
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        void TvReport_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string temp = string.Empty;
            try
            {
                #region get number
                ObjectItem selectObj = TvReport.SelectedItem as ObjectItem;
                number = selectObj.State;
                ReportName = selectObj.Name;
                #endregion
            }
            catch (Exception ex)
            {
            }
        }

        void TvReport_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        void TvReport_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            #region get number
            try
            {
                ObjectItem SelectObj = TvReport.SelectedItem as ObjectItem;
                if (SelectObj.State != 0)
                {
                    number = SelectObj.State;
            #endregion
                    PopupOpen();
                    ReportShow = false;
                }
            }
            catch (Exception ex)
            {
            }
        }

        void ReportViewer1_ReportExport(object sender, ReportExportEventArgs e)
        {
            if (number != 0)
            {
                MarkToDB();
                #region 记录日志
                string msg = string.Format("{0}:{1}", CurrentApp.Session.UserInfo.UserName, number);
                CurrentApp.WriteOperationLog(string.Format("6101002", number), ConstValue.OPT_RESULT_SUCCESS, msg);
                #endregion
            }
        }

        protected override void Init()
        {
            try
            {
                PageName = "ReportDisplayPage";
                StylePath = "UMPS6101/ReportDisplayPage.xaml";
                base.Init();
                if (CurrentApp != null)
                {
                    s6101App = CurrentApp as S6101App;
                }
                else
                {
                    s6101App = new S6101App(false);
                }
                //ChangeTheme();
                ChangeLanguage();
                SetBusy(true, string.Empty);
                InitOperation();
                s6101App.GetUserPermissions();
                s6101App.DecryptAgentPart();
                s6101App.GetParameters();
                s6101App.GetDataSet();
                s6101App.GetTenant();
                ReportViewer1.Messages = ReportViewerBarLang();
                //ReportViewer1.ShowExportButton = false;
                //mWorker = new BackgroundWorker();
                //mWorker.DoWork += (s, de) =>
                //{
                //};
                //mWorker.RunWorkerCompleted += (s, re) =>
                //{
                //    mWorker.Dispose();
                //};
                //mWorker.RunWorkerAsync();
                //触发Loaded消息
                CurrentApp.SendLoadedMessage();
                //ChangeTheme();
                SetBusy(false, string.Empty);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }

        }

        #region EventHandlers
        private void OpenCloseLeftPanel()
        {
            if (GridLeft.Width.Value > 0)
            {
                GridLeft.Width = new GridLength(0);
            }
            else
            {
                GridLeft.Width = new GridLength(200);
            }
        }
        #endregion
        //换主题
        public override void ChangeTheme()
        {
            base.ChangeTheme();

            bool bPage = false;
            if (AppServerInfo != null)
            {
                //优先从服务器上加载资源文件
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                        AppServerInfo.Protocol,
                        AppServerInfo.Address,
                        AppServerInfo.Port,
                        ThemeInfo.Name
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception)
                {
                    //App.ShowException("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS6101;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //App.ShowException("2" + ex.Message);
                }
            }
        }
        //换语言
        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            try
            {
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("FO6101", "UMP Report");

                QueryLab.Content = CurrentApp.GetLanguageInfo("61010102", "Query");
                ExpBasicOpt.Header = CurrentApp.GetLanguageInfo("61010101", "Report");
                #region 报表语言
                mRoot.Children.Clear();
                CreatTree(mRoot, "0");
                s6101App.DecryptAgentPart();
                #endregion
                TipLabQuery.Content = QueryLab.Content;
                TipLabReport.Content = ExpBasicOpt.Header;
                // 报表工具条
                ReportViewer1.Messages = ReportViewerBarLang();
                //ReportViewer1.ToolStripRenderer.
                GetReportLang();
                ShowReport();
            }
            catch (Exception ex)
            { }
        }

        #region 语言 报表的
        public void GetReportLang()
        {
            try
            {
                RL.ST = string.Empty;
                RL.ET = string.Empty;
                RL.CN = CurrentApp.Session.UserInfo.UserName;
                RL.Creator = CurrentApp.GetLanguageInfo("61011104", "创建者");
                RL.CreateTime = CurrentApp.GetLanguageInfo("61011103", "创建时间");
                RL.StartTime = CurrentApp.GetLanguageInfo("61011101", "开始时间");
                RL.EndTime = CurrentApp.GetLanguageInfo("61011102", "结束时间");
                string[] para1 = { CurrentApp.GetLanguageInfo("6101120101", "分机呼入呼出电话个数"), CurrentApp.GetLanguageInfo("6101120102", "呼入电话"), CurrentApp.GetLanguageInfo("6101120103", "呼出电话")
                                 , CurrentApp.GetLanguageInfo("6101120104","分机号码"), CurrentApp.GetLanguageInfo("6101120105","录音服务器") ,CurrentApp.GetLanguageInfo("6101120106","总电话量")
                                 , CurrentApp.GetLanguageInfo("6101120107","呼入时长"),CurrentApp.GetLanguageInfo("6101120108","呼出时长"), CurrentApp.GetLanguageInfo("6101120109","总时长")
                                 , CurrentApp.GetLanguageInfo("6101120110","平均时长"), CurrentApp.GetLanguageInfo("6101120111","分机电话统计报表")};
                string[] para2 = { CurrentApp.GetLanguageInfo("6101120201", "坐席电话统计图"), CurrentApp.GetLanguageInfo("6101120202", "坐席工号"), CurrentApp.GetLanguageInfo("6101120203", "坐席姓名")
                                 ,CurrentApp.GetLanguageInfo("6101120204","呼入平均时长"),CurrentApp.GetLanguageInfo("6101120205","呼出平均时长"),CurrentApp.GetLanguageInfo("6101120206","总电话量")
                                 , CurrentApp.GetLanguageInfo("6101120207","电话总时长"),CurrentApp.GetLanguageInfo("6101120208","电话总平均时长"),CurrentApp.GetLanguageInfo("6101120209","汇总组")
                                 , CurrentApp.GetLanguageInfo("6101120210","用户电话统计报表") };
                string[] para3 = { CurrentApp.GetLanguageInfo("6101120301", "用户编号"), CurrentApp.GetLanguageInfo("6101120302", "用户姓名"), CurrentApp.GetLanguageInfo("6101120303", "操作模块")
                                 ,CurrentApp.GetLanguageInfo("6101120304","操作动作"),CurrentApp.GetLanguageInfo("6101120305","操作内容"),CurrentApp.GetLanguageInfo("6101120306","操作时间")
                                 ,CurrentApp.GetLanguageInfo("6101120307","结果"),CurrentApp.GetLanguageInfo("6101120308","用户操作日志报表"),CurrentApp.GetLanguageInfo("6101120309","机器") };
                string[] para4 = { CurrentApp.GetLanguageInfo("6101120401","坐席分数统计"), CurrentApp.GetLanguageInfo("6101120402","质检平均分"), CurrentApp.GetLanguageInfo("6101120403","质检最高分")
                                 ,CurrentApp.GetLanguageInfo("6101120404","质检最低分"),CurrentApp.GetLanguageInfo("6101120405","组或部门"),CurrentApp.GetLanguageInfo("6101120406","质检次数")
                                 ,CurrentApp.GetLanguageInfo("6101120407","坐席分数统计") };
                string[] para5 = { CurrentApp.GetLanguageInfo("6101120501", "分数"), CurrentApp.GetLanguageInfo("6101120502", "质检时间"), CurrentApp.GetLanguageInfo("6101120503", "质检人")
                                 ,CurrentApp.GetLanguageInfo("6101120504","录音流水号"), CurrentApp.GetLanguageInfo("6101120505","评语"), CurrentApp.GetLanguageInfo("6101120506","是否申诉")
                             ,CurrentApp.GetLanguageInfo("6101120507","坐席分数详情报表")};
                string[] para6 = { CurrentApp.GetLanguageInfo("6101120601", "组平均分报表") };
                string[] para7 = { CurrentApp.GetLanguageInfo("6101120701", "质检数量"), CurrentApp.GetLanguageInfo("6101120702", "质检员工号"), CurrentApp.GetLanguageInfo("6101120703", "质检员姓名") 
                             ,CurrentApp.GetLanguageInfo("6101120704","任务数量"),CurrentApp.GetLanguageInfo("6101120705","已完成"),CurrentApp.GetLanguageInfo("6101120706","总录音时长")
                             ,CurrentApp.GetLanguageInfo("6101120707","被申诉数量"),CurrentApp.GetLanguageInfo("6101120708","被处理数量"),CurrentApp.GetLanguageInfo("6101120709","质检员工作量报表")};
                string[] para8 = { CurrentApp.GetLanguageInfo("6101120801", "坐席标准差"), CurrentApp.GetLanguageInfo("6101120802", "质检标准差"), CurrentApp.GetLanguageInfo("6101120803", "坐席标准差统计与排名") };
                string[] para9 = { CurrentApp.GetLanguageInfo("6101120901", "被申诉占比"), CurrentApp.GetLanguageInfo("6101120902", "被处理占比"), CurrentApp.GetLanguageInfo("6101120903", "质检员被申诉统计报表"), CurrentApp.GetLanguageInfo("6101120904", "评分表名称") };
                string[] para10 = { CurrentApp.GetLanguageInfo("6101121001", "申诉数量"), CurrentApp.GetLanguageInfo("6101121002", "申诉占比"), CurrentApp.GetLanguageInfo("6101121003", "坐席申诉统计报表") };
                string[] para11 = { CurrentApp.GetLanguageInfo("6101121101", "坐席通话时长分析"), CurrentApp.GetLanguageInfo("6101121102", "最长通话时长"), CurrentApp.GetLanguageInfo("6101121103", "最短通话时长"), 
                                CurrentApp.GetLanguageInfo("6101121104", "平均通话时长"), CurrentApp.GetLanguageInfo("6101121105", "通话个数"), CurrentApp.GetLanguageInfo("6101121106", "通话时长离散系数") };
                string[] para12 = { CurrentApp.GetLanguageInfo("6101121201", "质检员评分水平分析"), CurrentApp.GetLanguageInfo("6101121202", "成功次数") };
                string[] para13 = { };
                string[] para14 = {CurrentApp.GetLanguageInfo("6101121401", "质检员工作效率分析"), CurrentApp.GetLanguageInfo("6101121402", "质检录音数量"), CurrentApp.GetLanguageInfo("6101121403", "质检用时"), 
                                CurrentApp.GetLanguageInfo("6101121404", "用时比"), CurrentApp.GetLanguageInfo("6101121405", "任务完成最大周期（天）"), CurrentApp.GetLanguageInfo("6101121406", "任务完成最小周期（天）"), 
                              CurrentApp.GetLanguageInfo("6101121407", "任务完成平均周期（天）")};
                string[] para15 = { };
                string[] para16 = { CurrentApp.GetLanguageInfo("6101121601", "16坐席评分详情表") };
                string[] para17 = { CurrentApp.GetLanguageInfo("6101121701", "17评分表标准分析"), CurrentApp.GetLanguageInfo("6101121702", "标准名称(问题)"), CurrentApp.GetLanguageInfo("6101121703", "满分"), CurrentApp.GetLanguageInfo("6101121704", "平均分"), CurrentApp.GetLanguageInfo("6101121705", "标准差") };
                string[] para18 = { CurrentApp.GetLanguageInfo("6101121801", "18分机录音情况统计报表") };
                string[] para19 = { CurrentApp.GetLanguageInfo("6101121901", "19坐席评分表类别统计报表"), CurrentApp.GetLanguageInfo("6101121902", "记录条数"), CurrentApp.GetLanguageInfo("6101121903", "总平均分") };
                string[] para20 = { CurrentApp.GetLanguageInfo("6101122001", "20部门电话统计报表"), CurrentApp.GetLanguageInfo("6101122002", "部门或组"), CurrentApp.GetLanguageInfo("6101122003", "呼入电话"), CurrentApp.GetLanguageInfo("6101122004", "呼出电话"),
                                  CurrentApp.GetLanguageInfo("6101122005", "呼入时长"),CurrentApp.GetLanguageInfo("6101122006", "呼出时长"),CurrentApp.GetLanguageInfo("6101122007", "总电话量"),CurrentApp.GetLanguageInfo("6101122008", "总电话时长"),
                                  CurrentApp.GetLanguageInfo("6101122009", "总电话平均时长")};
                string[] para21 = { CurrentApp.GetLanguageInfo("6101122101", "告警发生和修复时间对应表"), CurrentApp.GetLanguageInfo("6101122102", "告警参考号"), CurrentApp.GetLanguageInfo("6101122103", "告警内容"), CurrentApp.GetLanguageInfo("6101122104", "告警时间"), CurrentApp.GetLanguageInfo("6101122105", "告警恢复时间")
                                  ,CurrentApp.GetLanguageInfo("6101122106", "告警邮件接收人"),CurrentApp.GetLanguageInfo("6101122107", "告警客户端接收人"),CurrentApp.GetLanguageInfo("6101122108", "告警APP接收人")};
                string[] para22 = { CurrentApp.GetLanguageInfo("6101122201", "关键词次数分析"), CurrentApp.GetLanguageInfo("6101122202", "关键词"), CurrentApp.GetLanguageInfo("6101122203", "次数"), CurrentApp.GetLanguageInfo("6101122204", "录音数量")
                                      , CurrentApp.GetLanguageInfo("6101122205", "次数比") };
                string[] para23 = { CurrentApp.GetLanguageInfo("6101122301", "分机关键词统计报表"), CurrentApp.GetLanguageInfo("6101122302", "分机") };
                string[] para24 = { CurrentApp.GetLanguageInfo("6101122401", "坐席关键词统计报表"), CurrentApp.GetLanguageInfo("6101122402", "坐席") };
                string[] para25 ={CurrentApp.GetLanguageInfo("6101122501", "分机关键词详情报表"),CurrentApp.GetLanguageInfo("6101122502", "分机"),CurrentApp.GetLanguageInfo("6101122503", "姓名")
                               ,CurrentApp.GetLanguageInfo("6101122504", "录音流水号"),CurrentApp.GetLanguageInfo("6101122505", "关键词"),CurrentApp.GetLanguageInfo("6101122506", "关键词开始时间")};
                string[] para26 = { CurrentApp.GetLanguageInfo("6101122601", "坐席关键词详情报表"), CurrentApp.GetLanguageInfo("6101122602", "坐席") };
                RL.Para1 = para1; RL.Para2 = para2; RL.Para3 = para3; RL.Para4 = para4; RL.Para5 = para5; RL.Para6 = para6; RL.Para7 = para7; RL.Para8 = para8; RL.Para9 = para9;
                RL.Para10 = para10; RL.Para11 = para11; RL.Para12 = para12; RL.Para13 = para13; RL.Para14 = para14; RL.Para15 = para15; RL.Para16 = para16; RL.Para17 = para17;
                RL.Para18 = para18; RL.Para19 = para19; RL.Para20 = para20; RL.Para21 = para21; RL.Para22 = para22; RL.Para23 = para23; RL.Para24 = para24; RL.Para25 = para25;
                RL.Para26 = para26;
            }
            catch (Exception ex)
            { }
        }

        private void TansforRepportLang(ReportParameter[] rp, string[] ListStr, int num)
        {
            int ReportNumber = number;
            if (number == 24) { ReportNumber = 23; }
            for (int i = 1; i <= num; i++)
            {
                if (ReportNumber < 10)
                {
                    string p_num = "P0" + ReportNumber + "0" + i.ToString();
                    if (i > 9)
                        p_num = "P0" + ReportNumber + i.ToString();
                    rp[6 + i] = new ReportParameter(p_num, ListStr[i - 1]);
                }
                else
                {
                    string p_num = "P" + ReportNumber + "0" + i.ToString();
                    if (i > 9)
                        p_num = "P" + ReportNumber + i.ToString();
                    rp[6 + i] = new ReportParameter(p_num, ListStr[i - 1]);
                }
            }
        }

        public void ReportLanguage()
        {
            try
            {
                switch (number)
                {
                    case 1:
                        rp = new ReportParameter[19];
                        TansforRepportLang(rp, RL.Para1, RL.Para1.Count());
                        rp[18] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 2:
                        rp = new ReportParameter[23];
                        TansforRepportLang(rp, RL.Para2, RL.Para2.Count());
                        rp[17] = new ReportParameter("P0102", RL.Para1[1]);
                        rp[18] = new ReportParameter("P0103", RL.Para1[2]);
                        rp[19] = new ReportParameter("P0107", RL.Para1[6]);
                        rp[20] = new ReportParameter("P0108", RL.Para1[7]);
                        rp[21] = new ReportParameter("P0405", RL.Para4[4]);
                        rp[22] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 3:
                        rp = new ReportParameter[17];
                        TansforRepportLang(rp, RL.Para3, RL.Para3.Count());
                        rp[16] = new ReportParameter("Tenant", S6101App.TenantName);
                        //rp[15] = new ReportParameter("P0105", RL.Para1[4]);
                        break;
                    case 4:
                        rp = new ReportParameter[17];
                        TansforRepportLang(rp, RL.Para4, RL.Para4.Count());
                        rp[14] = new ReportParameter("P0202", RL.Para2[1]);
                        rp[15] = new ReportParameter("P0203", RL.Para2[2]);
                        rp[16] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 5:
                        rp = new ReportParameter[18];
                        TansforRepportLang(rp, RL.Para5, RL.Para5.Count());
                        rp[14] = new ReportParameter("P0202", RL.Para2[1]);
                        rp[15] = new ReportParameter("P0203", RL.Para2[2]);
                        rp[16] = new ReportParameter("P0405", RL.Para4[4]);
                        rp[17] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 6:
                        rp = new ReportParameter[14];
                        rp[7] = new ReportParameter("P0601", RL.Para6[0]);
                        rp[8] = new ReportParameter("P0402", RL.Para4[1]);
                        rp[9] = new ReportParameter("P0403", RL.Para4[2]);
                        rp[10] = new ReportParameter("P0404", RL.Para4[3]);
                        rp[11] = new ReportParameter("P0405", RL.Para4[4]);
                        rp[12] = new ReportParameter("P0406", RL.Para4[5]);
                        rp[13] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 7:
                        rp = new ReportParameter[20];
                        TansforRepportLang(rp, RL.Para7, RL.Para7.Count());
                        rp[16] = new ReportParameter("P0402", RL.Para4[1]);
                        rp[17] = new ReportParameter("P0403", RL.Para4[2]);
                        rp[18] = new ReportParameter("P0404", RL.Para4[3]);
                        rp[19] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 8:
                        rp = new ReportParameter[17];
                        TansforRepportLang(rp, RL.Para8, RL.Para8.Count());
                        rp[10] = new ReportParameter("P0202", RL.Para2[1]);
                        rp[11] = new ReportParameter("P0203", RL.Para2[2]);
                        rp[12] = new ReportParameter("P0402", RL.Para4[1]);
                        rp[13] = new ReportParameter("P0403", RL.Para4[2]);
                        rp[14] = new ReportParameter("P0404", RL.Para4[3]);
                        rp[15] = new ReportParameter("P0405", RL.Para4[4]);
                        rp[16] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 9:
                        rp = new ReportParameter[18];
                        TansforRepportLang(rp, RL.Para9, RL.Para9.Count());
                        rp[11] = new ReportParameter("P0701", RL.Para7[0]);
                        rp[12] = new ReportParameter("P0702", RL.Para7[1]);
                        rp[13] = new ReportParameter("P0703", RL.Para7[2]);
                        rp[14] = new ReportParameter("P0706", RL.Para7[5]);
                        rp[15] = new ReportParameter("P0707", RL.Para7[6]);
                        rp[16] = new ReportParameter("P0708", RL.Para7[7]);
                        rp[17] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 10:
                        rp = new ReportParameter[20];
                        TansforRepportLang(rp, RL.Para10, RL.Para10.Count());
                        rp[10] = new ReportParameter("P0202", RL.Para2[1]);
                        rp[11] = new ReportParameter("P0203", RL.Para2[2]);
                        rp[12] = new ReportParameter("P0402", RL.Para4[1]);
                        rp[13] = new ReportParameter("P0403", RL.Para4[2]);
                        rp[14] = new ReportParameter("P0404", RL.Para4[3]);
                        rp[15] = new ReportParameter("P0405", RL.Para4[4]);
                        rp[16] = new ReportParameter("P0406", RL.Para4[5]);
                        rp[17] = new ReportParameter("P0902", RL.Para9[1]);
                        rp[18] = new ReportParameter("P0708", RL.Para7[7]);
                        rp[19] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 11:
                        rp = new ReportParameter[17];
                        TansforRepportLang(rp, RL.Para11, RL.Para11.Count());
                        rp[13] = new ReportParameter("P0202", RL.Para2[1]);
                        rp[14] = new ReportParameter("P0203", RL.Para2[2]);
                        rp[15] = new ReportParameter("P0405", RL.Para4[4]);
                        rp[16] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 12:
                        rp = new ReportParameter[19];
                        TansforRepportLang(rp, RL.Para12, RL.Para12.Count());
                        rp[9] = new ReportParameter("P0702", RL.Para7[1]);
                        rp[10] = new ReportParameter("P0703", RL.Para7[2]);
                        rp[11] = new ReportParameter("P0406", RL.Para4[5]);
                        rp[12] = new ReportParameter("P0402", RL.Para4[1]);
                        rp[13] = new ReportParameter("P0403", RL.Para4[2]);
                        rp[14] = new ReportParameter("P0404", RL.Para4[3]);
                        rp[15] = new ReportParameter("P0802", RL.Para8[1]);
                        rp[16] = new ReportParameter("P0707", RL.Para7[6]);
                        rp[17] = new ReportParameter("P0904", RL.Para9[3]);
                        rp[18] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 13:
                        rp = new ReportParameter[19];
                        rp[7] = new ReportParameter("P0702", RL.Para7[1]);
                        rp[8] = new ReportParameter("P0703", RL.Para7[2]);
                        rp[9] = new ReportParameter("P0705", RL.Para7[4]);
                        rp[10] = new ReportParameter("P0706", RL.Para7[5]);
                        rp[11] = new ReportParameter("P0707", RL.Para7[6]);
                        rp[12] = new ReportParameter("P0708", RL.Para7[7]);
                        rp[13] = new ReportParameter("P0709", RL.Para7[8]);
                        rp[14] = new ReportParameter("P0701", RL.Para7[0]);
                        rp[15] = new ReportParameter("P0402", RL.Para4[1]);
                        rp[16] = new ReportParameter("P0403", RL.Para4[2]);
                        rp[17] = new ReportParameter("P0404", RL.Para4[3]);
                        rp[18] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 14:
                        rp = new ReportParameter[20];
                        TansforRepportLang(rp, RL.Para14, RL.Para14.Count());
                        rp[14] = new ReportParameter("P0702", RL.Para7[1]);
                        rp[15] = new ReportParameter("P0703", RL.Para7[2]);
                        rp[16] = new ReportParameter("P0704", RL.Para7[3]);
                        rp[17] = new ReportParameter("P0705", RL.Para7[4]);
                        rp[18] = new ReportParameter("P0706", RL.Para7[5]);
                        rp[19] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 15:
                        rp = new ReportParameter[15];
                        rp[7] = new ReportParameter("P0702", RL.Para7[1]);
                        rp[8] = new ReportParameter("P0703", RL.Para7[2]);
                        rp[9] = new ReportParameter("P0705", RL.Para7[4]);
                        rp[10] = new ReportParameter("P0706", RL.Para7[5]);
                        rp[11] = new ReportParameter("P1401", RL.Para14[0]);
                        rp[12] = new ReportParameter("P1403", RL.Para14[2]);
                        rp[13] = new ReportParameter("P1404", RL.Para14[3]);
                        rp[14] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 16:
                        rp = new ReportParameter[18];
                        rp[7] = new ReportParameter("P1601", RL.Para16[0]);
                        rp[8] = new ReportParameter("P0202", RL.Para2[1]);
                        rp[9] = new ReportParameter("P0203", RL.Para2[2]);
                        rp[10] = new ReportParameter("P0405", RL.Para4[4]);
                        rp[11] = new ReportParameter("P0501", RL.Para5[0]);
                        rp[12] = new ReportParameter("P0502", RL.Para5[1]);
                        rp[13] = new ReportParameter("P0503", RL.Para5[2]);
                        rp[14] = new ReportParameter("P0504", RL.Para5[3]);
                        rp[15] = new ReportParameter("P0505", RL.Para5[4]);
                        rp[16] = new ReportParameter("P0904", RL.Para9[3]);
                        rp[17] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 17:
                        rp = new ReportParameter[13];
                        rp[7] = new ReportParameter("P1701", RL.Para17[0]);
                        rp[8] = new ReportParameter("P1702", RL.Para17[1]);
                        rp[9] = new ReportParameter("P1703", RL.Para17[2]);
                        rp[10] = new ReportParameter("P1704", RL.Para17[3]);
                        rp[11] = new ReportParameter("P1705", RL.Para17[4]);
                        rp[12] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 18:
                        rp = new ReportParameter[10];
                        rp[7] = new ReportParameter("P1801", RL.Para18[0]);
                        rp[8] = new ReportParameter("P1802", RL.Para1[3]);
                        rp[9] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 19:
                        rp = new ReportParameter[15];
                        rp[7] = new ReportParameter("P1901", RL.Para19[0]);
                        rp[8] = new ReportParameter("P1902", RL.Para19[1]);
                        rp[9] = new ReportParameter("P1903", RL.Para19[2]);
                        rp[10] = new ReportParameter("P0202", RL.Para2[1]);
                        rp[11] = new ReportParameter("P0203", RL.Para2[2]);
                        rp[12] = new ReportParameter("P0405", RL.Para4[4]);
                        rp[13] = new ReportParameter("P0904", RL.Para9[3]);
                        rp[14] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 20:
                        rp = new ReportParameter[17];
                        TansforRepportLang(rp, RL.Para20, RL.Para20.Count());
                        rp[16] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 21:
                        rp = new ReportParameter[16];
                        TansforRepportLang(rp, RL.Para21, RL.Para21.Count());
                        rp[15] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 22:
                        rp = new ReportParameter[13];
                        TansforRepportLang(rp, RL.Para22, RL.Para22.Count());
                        rp[12] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 23:
                        rp = new ReportParameter[10];
                        TansforRepportLang(rp, RL.Para23, RL.Para23.Count());
                        rp[9] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 24:
                        rp = new ReportParameter[10];
                        TansforRepportLang(rp, RL.Para24, RL.Para24.Count());
                        rp[9] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 25:
                        rp = new ReportParameter[14];
                        TansforRepportLang(rp, RL.Para25, RL.Para25.Count());
                        rp[13] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    case 26:
                        rp = new ReportParameter[14];
                        rp[7] = new ReportParameter("P2501", RL.Para26[0]);
                        rp[8] = new ReportParameter("P2502", RL.Para26[1]);
                        rp[9] = new ReportParameter("P2503", RL.Para25[2]);
                        rp[10] = new ReportParameter("P2504", RL.Para25[3]);
                        rp[11] = new ReportParameter("P2505", RL.Para25[4]);
                        rp[12] = new ReportParameter("P2506", RL.Para25[5]);
                        rp[13] = new ReportParameter("Tenant", S6101App.TenantName);
                        break;
                    default:
                        break;
                }
                rp[0] = new ReportParameter("Creator", RL.Creator);
                rp[1] = new ReportParameter("StartTime", RL.StartTime);
                rp[2] = new ReportParameter("EndTime", RL.EndTime);
                rp[3] = new ReportParameter("CreateTime", RL.CreateTime);
                rp[4] = new ReportParameter("CN", RL.CN);
                rp[5] = new ReportParameter("ST", RL.ST);
                rp[6] = new ReportParameter("ET", RL.ET);

                if (number != 16 && number != 18 && number != 19 && number != 24)
                    this.ReportViewer1.LocalReport.SetParameters(rp);
                else
                {
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        #endregion

        #region 写好的鼠标事件
        private void PopupOpen()
        {
            //显示界面，报表界面
            //this.ListViewReport.Visibility = Visibility.Collapsed;
            switch (number)
            {
                case 1:
                    rp1querypage.CurrentApp = s6101App;
                    rp1querypage.S6101App = s6101App;
                    rp1querypage.ChangeLanguage();
                    rp1querypage.PageParent = this;
                    Popup.Child = rp1querypage;
                    rp1querypage.mRootItem = new ObjectItem();
                    rp1querypage.mRootItem1 = new ObjectItem();
                    rp1querypage.itemRootIP = new ObjectItem();
                    rp1querypage.itemRootIP.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 2:
                    rp2querypage.CurrentApp = s6101App;
                    rp2querypage.S6101App = s6101App;
                    rp2querypage.ChangeLanguage();
                    rp2querypage.PageParent = this;
                    Popup.Child = rp2querypage;
                    rp2querypage.mRootItem = new ObjectItem();
                    rp2querypage.mRootItem1 = new ObjectItem();
                    rp2querypage.itemRoot = new ObjectItem();
                    rp2querypage.itemRoot.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 3:
                    rp3querypage.CurrentApp = s6101App;
                    rp3querypage.S6101App = s6101App;
                    rp3querypage.ChangeLanguage();
                    rp3querypage.PageParent = this;
                    Popup.Child = rp3querypage;
                    rp3querypage.mRootItem = new ObjectItem();
                    rp3querypage.mRootItem1 = new ObjectItem();
                    rp3querypage.mRootItem2 = new ObjectItem();
                    rp3querypage.mRootItem3 = new ObjectItem();
                    rp3querypage.mRootItem4 = new ObjectItem();
                    rp3querypage.itemRoot = new ObjectItem();
                    rp3querypage.itemRoot.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    rp3querypage.itemRoot4 = new ObjectItem();
                    rp3querypage.itemRoot4.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 4:
                    rp4querypage.CurrentApp = s6101App;
                    rp4querypage.S6101App = s6101App;
                    rp4querypage.ChangeLanguage();
                    rp4querypage.PageParent = this;
                    Popup.Child = rp4querypage;
                    rp4querypage.mRootItem = new ObjectItem();
                    rp4querypage.mRootItem1 = new ObjectItem();
                    rp4querypage.mRootItem2 = new ObjectItem();
                    rp4querypage.itemRootPFB = new ObjectItem();
                    rp4querypage.itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 5:
                    rp5querypage.CurrentApp = s6101App;
                    rp5querypage.S6101App = s6101App;
                    rp5querypage.ChangeLanguage();
                    rp5querypage.PageParent = this;
                    Popup.Child = rp5querypage;
                    rp5querypage.mRootItem = new ObjectItem();
                    rp5querypage.mRootItem1 = new ObjectItem();
                    rp5querypage.mRootItem2 = new ObjectItem();
                    rp5querypage.itemRootPFB = new ObjectItem();
                    rp5querypage.itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 6:
                    rp6querypage.CurrentApp = s6101App;
                    rp6querypage.S6101App = s6101App;
                    rp6querypage.ChangeLanguage();
                    rp6querypage.PageParent = this;
                    Popup.Child = rp6querypage;
                    rp6querypage.mRootItem = new ObjectItem();
                    rp6querypage.mRootItem1 = new ObjectItem();
                    rp6querypage.itemRootPFB = new ObjectItem();
                    rp6querypage.itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 7:
                    rp7querypage.CurrentApp = s6101App;
                    rp7querypage.S6101App = s6101App;
                    rp7querypage.ChangeLanguage();
                    rp7querypage.PageParent = this;
                    Popup.Child = rp7querypage;
                    rp7querypage.mRootItem = new ObjectItem();
                    rp7querypage.mRootItem1 = new ObjectItem();
                    rp7querypage.itemRootPFB = new ObjectItem();
                    rp7querypage.itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 8:
                    rp8querypage.CurrentApp = s6101App;
                    rp8querypage.S6101App = s6101App;
                    rp8querypage.ChangeLanguage();
                    rp8querypage.PageParent = this;
                    Popup.Child = rp8querypage;
                    rp8querypage.mRootItem = new ObjectItem();
                    rp8querypage.mRootItem1 = new ObjectItem();
                    rp8querypage.mRootItem2 = new ObjectItem();
                    rp8querypage.itemRootPFB = new ObjectItem();
                    rp8querypage.itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 9:
                    rp9querypage.CurrentApp = s6101App;
                    rp9querypage.S6101App = s6101App;
                    rp9querypage.ChangeLanguage();
                    rp9querypage.PageParent = this;
                    Popup.Child = rp9querypage;
                    rp9querypage.mRootItem = new ObjectItem();
                    rp9querypage.mRootItem1 = new ObjectItem();
                    rp9querypage.itemRootPFB = new ObjectItem();
                    rp9querypage.itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 10:
                    rp10querypage.CurrentApp = s6101App;
                    rp10querypage.S6101App = s6101App;
                    rp10querypage.ChangeLanguage();
                    rp10querypage.PageParent = this;
                    Popup.Child = rp10querypage;
                    rp10querypage.mRootItem = new ObjectItem();
                    rp10querypage.mRootItem1 = new ObjectItem();
                    rp10querypage.mRootItem2 = new ObjectItem();
                    rp10querypage.itemRootPFB = new ObjectItem();
                    rp10querypage.itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 11:
                    rp11querypage.CurrentApp = s6101App;
                    rp11querypage.S6101App = s6101App;
                    rp11querypage.ChangeLanguage();
                    rp11querypage.PageParent = this;
                    Popup.Child = rp11querypage;
                    rp11querypage.mRootItem1 = new ObjectItem();
                    rp11querypage.mRootItem2 = new ObjectItem();
                    Popup.IsOpen = true;
                    break;
                case 12:
                    rp12querypage.CurrentApp = s6101App;
                    rp12querypage.S6101App = s6101App;
                    rp12querypage.ChangeLanguage();
                    rp12querypage.PageParent = this;
                    Popup.Child = rp12querypage;
                    rp12querypage.mRootItem1 = new ObjectItem();
                    rp12querypage.mRootItem2 = new ObjectItem();
                    rp12querypage.itemRootPFB = new ObjectItem();
                    rp12querypage.itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 13:
                    rp13querypage.CurrentApp = s6101App;
                    rp13querypage.S6101App = s6101App;
                    rp13querypage.ChangeLanguage();
                    rp13querypage.PageParent = this;
                    Popup.Child = rp13querypage;
                    rp13querypage.mRootItem = new ObjectItem();
                    rp13querypage.mRootItem1 = new ObjectItem();
                    rp13querypage.itemRootPFB = new ObjectItem();
                    rp13querypage.itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 14:
                    rp14querypage.CurrentApp = s6101App;
                    rp14querypage.S6101App = s6101App;
                    rp14querypage.ChangeLanguage();
                    rp14querypage.PageParent = this;
                    Popup.Child = rp14querypage;
                    rp14querypage.mRootItem1 = new ObjectItem();
                    rp14querypage.mRootItem2 = new ObjectItem();
                    rp14querypage.itemRootPFB = new ObjectItem();
                    rp14querypage.itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 15:
                    rp15querypage.CurrentApp = s6101App;
                    rp15querypage.S6101App = s6101App;
                    rp15querypage.ChangeLanguage();
                    rp15querypage.PageParent = this;
                    Popup.Child = rp15querypage;
                    rp15querypage.mRootItem1 = new ObjectItem();
                    rp15querypage.mRootItem2 = new ObjectItem();
                    rp15querypage.itemRootPFB = new ObjectItem();
                    rp15querypage.itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 16:
                    rp16querypage.CurrentApp = s6101App;
                    rp16querypage.S6101App = s6101App;
                    rp16querypage.ChangeLanguage();
                    rp16querypage.PageParent = this;
                    Popup.Child = rp16querypage;
                    rp16querypage.mRootItem = new ObjectItem();
                    rp16querypage.mRootItem1 = new ObjectItem();
                    rp16querypage.mRootItem2 = new ObjectItem();
                    rp16querypage.itemRootPFB = new ObjectItem();
                    rp16querypage.itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 17:
                    rp17querypage.CurrentApp = s6101App;
                    rp17querypage.S6101App = s6101App;
                    rp17querypage.ChangeLanguage();
                    rp17querypage.PageParent = this;
                    Popup.Child = rp17querypage;
                    rp17querypage.mRootItem = new ObjectItem();
                    rp17querypage.mRootItem1 = new ObjectItem();
                    rp17querypage.mRootItem2 = new ObjectItem();
                    rp17querypage.itemRootPFB = new ObjectItem();
                    rp17querypage.itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 18:
                    rp18querypage.CurrentApp = s6101App;
                    rp18querypage.S6101App = s6101App;
                    rp18querypage.ChangeLanguage();
                    rp18querypage.PageParent = this;
                    Popup.Child = rp18querypage;
                    rp18querypage.mRootItem = new ObjectItem();
                    rp18querypage.itemRootPFB = new ObjectItem();
                    rp18querypage.itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 19:
                    //显示ListView界面
                    //this.ListViewReport.Visibility = Visibility.Visible;

                    //打开界面
                    rp19querypage.CurrentApp = s6101App;
                    rp19querypage.S6101App = s6101App;
                    rp19querypage.ChangeLanguage();
                    rp19querypage.PageParent = this;
                    Popup.Child = rp19querypage;
                    //rp19querypage.mRootItem = new ObjectItem();
                    //rp19querypage.itemRootPFB = new ObjectItem();
                    rp19querypage.itemRootPFB.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 20:
                    rp20querypage.CurrentApp = s6101App;
                    rp20querypage.S6101App = s6101App;
                    rp20querypage.ChangeLanguage();
                    rp20querypage.PageParent = this;
                    Popup.Child = rp20querypage;
                    rp20querypage.mRootItem = new ObjectItem();
                    rp20querypage.mRootItem1 = new ObjectItem();
                    rp20querypage.itemRootIP = new ObjectItem();
                    rp20querypage.itemRootIP.DisplayContent = CurrentApp.GetLanguageInfo("610100000016", "ALL SELECT");
                    Popup.IsOpen = true;
                    break;
                case 21:
                    rp21querypage.CurrentApp = s6101App;
                    rp21querypage.S6101App = s6101App;
                    rp21querypage.ChangeLanguage();
                    rp21querypage.PageParent = this;
                    Popup.Child = rp21querypage;
                    Popup.IsOpen = true;
                    break;
                case 22:
                    rp22querypage.CurrentApp = s6101App;
                    rp22querypage.S6101App = s6101App;
                    rp22querypage.ChangeLanguage();
                    rp22querypage.PageParent = this;
                    Popup.Child = rp22querypage;
                    Popup.IsOpen = true;
                    break;
                case 23:
                    rp23querypage.CurrentApp = s6101App;
                    rp23querypage.S6101App = s6101App;
                    rp23querypage.ChangeLanguage();
                    rp23querypage.PageParent = this;
                    Popup.Child = rp23querypage;
                    Popup.IsOpen = true;
                    break;
                case 24:
                    rp24querypage.CurrentApp = s6101App;
                    rp24querypage.S6101App = s6101App;
                    rp24querypage.Rnumber = number;
                    rp24querypage.ChangeLanguage();
                    rp24querypage.PageParent = this;
                    Popup.Child = rp24querypage;
                    Popup.IsOpen = true;
                    break;
                case 25:
                    rp25querypage.CurrentApp = s6101App;
                    rp25querypage.S6101App = s6101App;
                    rp25querypage.Rnumber = number;
                    rp25querypage.ChangeLanguage();
                    rp25querypage.PageParent = this;
                    Popup.Child = rp25querypage;
                    Popup.IsOpen = true;
                    break;
                case 26:
                    rp24querypage.CurrentApp = s6101App;
                    rp24querypage.S6101App = s6101App;
                    rp25querypage.Rnumber = number;
                    rp24querypage.ChangeLanguage();
                    rp24querypage.PageParent = this;
                    Popup.Child = rp24querypage;
                    Popup.IsOpen = true;
                    break;
                default:
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0002", "No Report , Please Select"));
                    break;
            }
        }

        public void PopupClose()
        {
            Popup.IsOpen = false; //TxtStatus.Text = string.Empty;
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            PopupOpen();
        }

        public void ComfireClick(List<string> StrData, AboutDateTime SendDate)
        {
            SetBusy(true, "Reporting......");
            StrQuery.RemoveRange(0, StrQuery.Count);
            StrQuery = StrData; S6101App.ADT = SendDate;
            Popup.IsOpen = false; ShowReport();
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                SetBusy(false, "Report End.");
                ReportShow = true;
            };
            mWorker.RunWorkerAsync();

            //StrQuery.RemoveRange(0, StrQuery.Count);
            //StrQuery = StrData; S6101App.ADT = SendDate;

            //Popup.IsOpen = false;
            //ShowReport();
            //SetBusy(false, "Report End.");
            //ReportShow = true;
        }

        #endregion
        //展示报表，从服务拿数据回来展示
        public void ShowReport()
        {
            this.ReportViewer1.Clear();
            string STime = StrQuery[0];
            string ETime = StrQuery[1];
            if (number > 3 && number != 11 && STime.Length != 19)
            {
                STime = StrQuery[0].Substring(2);
                if (STime.Length != 19)
                    STime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", STime.Substring(0, 4), STime.Substring(4, 2), STime.Substring(6, 2)
                        , STime.Substring(8, 2), STime.Substring(10, 2), STime.Substring(12, 2));
                ETime = StrQuery[1].Substring(2);
                if (ETime.Length != 19)
                    ETime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", ETime.Substring(0, 4), ETime.Substring(4, 2), ETime.Substring(6, 2)
                        , ETime.Substring(8, 2), ETime.Substring(10, 2), ETime.Substring(12, 2));
            }
            RL.ST = (Convert.ToDateTime(STime)).ToLocalTime().ToString("yyyy/MM/dd").ToString().Substring(0, 10);
            RL.ET = (Convert.ToDateTime(ETime)).ToLocalTime().ToString("yyyy/MM/dd").ToString().Substring(0, 10);
            this.ReportViewer1.LocalReport.ReportPath = null;
            this.ReportViewer1.LocalReport.ReportEmbeddedResource = null;
            this.ReportViewer1.PageCountMode = PageCountMode.Actual;
            if (number != 16 && number != 18 && number != 19 && number != 24)
            {
                this.ReportViewer1.LocalReport.ReportEmbeddedResource = "UMPS6101.Report.Report" + number + ".rdlc";
            }
            if (number == 26)
            {
                this.ReportViewer1.LocalReport.ReportEmbeddedResource = "UMPS6101.Report.Report25.rdlc";
            }
            ReportLanguage();
            DataSet dataSet = new DataSet();
            switch (number)
            {
                case 26:
                    R26(StrQuery);
                    break;
                case 25:
                    R25(StrQuery);
                    break;
                case 24:
                    R24(StrQuery);
                    break;
                case 23:
                    R23(StrQuery);
                    break;
                case 22:
                    R22(StrQuery);
                    break;
                case 21:
                    R21(StrQuery);
                    break;
                case 20:
                    R20(StrQuery);
                    break;
                case 19:
                    R19(StrQuery);
                    break;
                case 18:
                    R18(StrQuery);
                    break;
                case 17:
                    R17(StrQuery);
                    break;
                case 16:
                    R16(StrQuery);
                    break;
                case 15:
                    R15(StrQuery);
                    break;
                case 14:
                    R14(StrQuery);
                    break;
                case 13:
                    R13(StrQuery);
                    break;
                case 12:
                    R12(StrQuery);
                    break;
                case 11:
                    R11(StrQuery);
                    break;
                case 10:
                    R10(StrQuery);
                    break;
                case 9:
                    R9(StrQuery);
                    break;
                case 8:
                    R8(StrQuery);
                    break;
                case 7:
                    R7(StrQuery);
                    break;
                case 6:
                    R6(StrQuery);
                    break;
                case 5:
                    R5(StrQuery);
                    break;
                case 4:
                    R4(StrQuery);
                    break;
                case 3:
                    R3(StrQuery);
                    break;
                case 1:
                    R1(StrQuery);
                    break;
                case 2:
                    R2(StrQuery);
                    break;
                default:
                    break;
            }
            if (ReportResult)
            {
                #region 记录日志
                string msg = string.Format("{0}", ReportName);
                CurrentApp.WriteOperationLog(string.Format("6101001", number), ConstValue.OPT_RESULT_SUCCESS, msg);
                #endregion
            }
            else
            {
                #region 记录日志
                string msg = string.Format("{0}", ReportName);
                CurrentApp.WriteOperationLog(string.Format("6101001", number), ConstValue.OPT_RESULT_FAIL, msg);
                #endregion
            }
            //App.TimeTest += "\r" + "全部完成:" + DateTime.Now.ToString("hh:mm:ss:fff");
            //App.ShowInfoMessage(App.TimeTest);

            //打印按钮不显示
            //this.ReportViewer1.ShowPrintButton = false;
            //OperationLogReport_RenderingComplete("Excel");
            //OperationLogReport_RenderingComplete("Word");
            //OperationLogReport_RenderingComplete("PDF");  
            //this.ReportViewer1.ShowBackButton = false;   //后退键
            // this.ReportViewer1.ShowExportButton = false;//导出按钮
            //this.ReportViewer1.ShowToolBar = false;     
        }

        private ReportViewerMessagesZhcn ReportViewerBarLang()
        {
            ReportViewerMessagesZhcn rvm = new ReportViewerMessagesZhcn();
            rvm.BackButtonT = CurrentApp.GetLanguageInfo("61011111", "返回");
            rvm.CurrentPageT = CurrentApp.GetLanguageInfo("61011107", "当前页");
            rvm.ExportButtonT = CurrentApp.GetLanguageInfo("61011117", "导出");
            rvm.FindButtonT = CurrentApp.GetLanguageInfo("61011123", "查找");
            rvm.FindNextButtonT = CurrentApp.GetLanguageInfo("61011125", "查找下一个");
            rvm.FirstPageButtonT = CurrentApp.GetLanguageInfo("61011105", "首页");
            rvm.LastPageButtonT = CurrentApp.GetLanguageInfo("61011110", "尾页");
            rvm.NextPageButtonT = CurrentApp.GetLanguageInfo("61011109", "下一页");
            rvm.PageSetupButtonT = CurrentApp.GetLanguageInfo("61011116", "页面设置");
            rvm.PageWidth = CurrentApp.GetLanguageInfo("61011119", "自适应");
            rvm.PreviousPageButtonT = CurrentApp.GetLanguageInfo("61011106", "上一页");
            rvm.PrintButtonT = CurrentApp.GetLanguageInfo("61011114", "打印");
            rvm.PrintLayoutButtonT = CurrentApp.GetLanguageInfo("61011115", "打印布局");
            rvm.RefreshButtonT = CurrentApp.GetLanguageInfo("61011113", "刷新");
            rvm.SearchTextBoxT = CurrentApp.GetLanguageInfo("61011121", "在报表中查找文本");
            rvm.StopButtonT = CurrentApp.GetLanguageInfo("61011112", "停止呈现");
            rvm.TotalPagesT = CurrentApp.GetLanguageInfo("61011108", "总页数");
            rvm.WholePage = CurrentApp.GetLanguageInfo("61011120", "整页");
            rvm.ZoomControlT = CurrentApp.GetLanguageInfo("6101118", "缩放");
            rvm.FindNextButton = CurrentApp.GetLanguageInfo("61011124", "下一个");
            rvm.FindButton = CurrentApp.GetLanguageInfo("61011122", "Find");

            return rvm;
        }

        #region 报表
        private void R3(List<string> StrQ)
        {
            #region 收集条件
            List<string> IP = new List<string>(); List<string> UA = new List<string>(); List<string> OR = new List<string>();
            List<string> DateSection = new List<string>(); List<string> MN = new List<string>(); string MnId = string.Empty;
            string where = string.Empty; List<string> TableName = new List<string>(); string IpId = string.Empty; string UaId = string.Empty; bool IsUser = true;
            if (StrQ.Count >= 2)
            {
                Regex regex = new Regex(@"\D+"); string st = regex.Replace(StrQ[0], string.Empty); string et = regex.Replace(StrQ[1], string.Empty);
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "IP")
                    {
                        IP = StrToList(str.Substring(2, str.Length - 2));
                        IpId = s6101App.PutTempData(IP);
                        where += string.Format(" AND C007 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", IpId);
                    }
                    if (str.Substring(0, 2) == "MN")
                    {
                        MN = StrToList(str.Substring(2, str.Length - 2));
                        MnId = s6101App.PutTempData(MN);
                        where += string.Format(" AND C006 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", MnId);
                    }
                    if (str.Substring(0, 2) == "OR")
                    {
                        OR = StrToList(str.Substring(2, str.Length - 2));
                        where += " AND " + ListToSql(OR, "C009", 1);
                    }
                    if (str.Substring(0, 2) == "UA")
                    {
                        UA = StrToList(str.Substring(2, str.Length - 2));
                        UaId = s6101App.PutTempData(UA); IsUser = false;
                        where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", UaId);
                    }
                }
                if (IsUser && S6101App.UserList.Count != 0)
                {
                    UaId = s6101App.PutTempData(S6101App.UserList); IsUser = false;
                    where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", UaId);
                }
            #endregion
                int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
                ReportViewer1.LocalReport.DataSources.Clear();
                try
                {
                    for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                    {
                        if (i == stand)
                        {
                            dataSet = s6101App.GetR3DataSet(where, i);
                        }
                        else
                        {
                            DS_temp = s6101App.GetR3DataSet(where, i);
                            if (DS_temp.Tables.Count != 0)
                            {
                                foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                    dataSet.Tables[0].ImportRow(dr);
                            }
                        }
                        if (dataSet.Tables.Count != 0)
                        {
                            ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]);
                            ReportViewer1.LocalReport.DataSources.Add(rds);
                            this.ReportViewer1.RefreshReport();
                        }
                        else
                            stand++;
                    }
                }
                catch (Exception ex)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("3:{0}\n{1}", ex.Message, ex.ToString()));
                }
                if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                    ReportResult = false;
                }
            }
        }

        private void R1(List<string> StrQ)
        {
            #region 统计条件
            List<string> EX = new List<string>(); List<string> IP = new List<string>(); List<string> EP = new List<string>();
            string call = string.Empty; string where = string.Empty; List<string> TableName = new List<string>();
            string ExId = string.Empty; string IpId = string.Empty; string AgId = string.Empty; string EpId = string.Empty;
            if (StrQ.Count >= 2)
            {
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "IP")
                    {
                        IP = StrToList(str.Substring(2, str.Length - 2));
                        IpId = s6101App.PutTempData(IP);
                        //if (App.Session.DBType == 2)
                        //    where += string.Format(" AND A.C020 IN (SELECT CONVERT(VARCHAR2(32),C011) FROM T_00_901 WHERE C001={0})", IpId);
                        //else
                        where += string.Format(" AND A.C020 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", IpId);
                    }
                    if (str.Substring(0, 2) == "EX")
                    {
                        EX = StrToList(str.Substring(2, str.Length - 2));
                        ExId = s6101App.PutTempData(EX);
                        //if(App.Session.DBType==2)
                        //where += string.Format(" AND A.C042 IN (SELECT CONVERT(NVARCHAR(32),C011) FROM T_00_901 WHERE C001={0} AND C012=A.C020) ", ExId);
                        //else
                        where += string.Format(" AND A.C042 IN (SELECT C011 FROM T_00_901 WHERE C001={0}) ", ExId);
                    }
                    if (str.Substring(0, 2) == "CA")
                    {
                        call = str.Substring(2);
                        where += " AND C045=" + call;
                    }
                }
                if (S6101App.AgentNameList.Count != 0 && S6101App.PacketMode.Contains("A"))
                {
                    AgId = s6101App.PutTempData(S6101App.AgentNameList);
                    //if (App.Session.DBType == 2)
                    //    where = string.Format(" AND A.C039 IN (SELECT CONVERT(NVARCHAR(128),C011) FROM T_00_901 WHERE C001={0}) {1}", AgId, where);
                    //else
                    where += string.Format(" AND A.C039 IN (SELECT C011 FROM T_00_901 WHERE C001={0}) ", AgId);
                }
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        CurrentApp.WriteLog(string.Format("(1):Start OK"));
                        dataSet = s6101App.GetR1DataSet(where, i);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR1DataSet(where, i);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        CurrentApp.WriteLog(string.Format("(1):Table2 begine"));
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        #region 图表表
                        DataTable DT_chart = new DataTable(); List<string> ExtenList = new List<string>();
                        DataColumn col_exten = new DataColumn("P110", typeof(string)); DT_chart.Columns.Add(col_exten);
                        DataColumn col_cin = new DataColumn("P111", typeof(string)); DT_chart.Columns.Add(col_cin);
                        DataColumn col_cout = new DataColumn("P112", typeof(string)); DT_chart.Columns.Add(col_cout);
                        foreach (DataRow exten in dataSet.Tables[0].Rows)
                        {
                            bool flag = true;
                            for (int k = 0; k < ExtenList.Count; k++)
                            {
                                if (exten["C042"].ToString() == ExtenList[k])
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                ExtenList.Add(exten["C042"].ToString());
                            }
                        }
                        for (int q = 0; q < ExtenList.Count; q++)
                        {
                            int cin = 0; int cout = 0;
                            foreach (DataRow dr in dataSet.Tables[0].Rows)
                            {
                                if (ExtenList[q] == dr["C042"].ToString())
                                {
                                    cin += int.Parse(dr["P100"].ToString()); cout += int.Parse(dr["P101"].ToString());
                                }
                            }
                            DataRow row = DT_chart.NewRow(); row["P110"] = ExtenList[q].ToString(); row["P111"] = cin; row["P112"] = cout; DT_chart.Rows.Add(row);
                        }
                        #endregion
                        ReportDataSource rds_chart = new ReportDataSource("DataSet2", DT_chart);
                        ReportViewer1.LocalReport.DataSources.Add(rds_chart);
                        this.ReportViewer1.RefreshReport();
                        CurrentApp.WriteLog(string.Format("(1):Report OK"));
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("1:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R2(List<string> StrQ)
        {
            #region 统计条件
            List<string> AG = new List<string>(); List<string> IP = new List<string>(); string call = string.Empty; bool IsAgent = true;
            string where = string.Empty; List<string> TableName = new List<string>(); string AgId = string.Empty; string IpId = string.Empty;
            if (StrQ.Count >= 2)
            {
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "AG")
                    {
                        AG = StrToList(str.Substring(2, str.Length - 2));
                        AgId = s6101App.PutTempData(AG);
                        //if (App.Session.DBType == 2)
                        //{
                        //    where += string.Format(" AND C039 IN (SELECT CONVERT(NVARCHAR(128),C011) FROM T_00_901 WHERE C001={0})", AgId);
                        //}
                        //else
                        //{
                        where += string.Format(" AND C039 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                        //}
                        IsAgent = false;
                    }
                    if (str.Substring(0, 2) == "IP")
                    {
                        IP = StrToList(str.Substring(2, str.Length - 2));
                        IpId = s6101App.PutTempData(IP);
                        //if (App.Session.DBType == 2)
                        //{
                        //    where += string.Format(" AND C020 IN (SELECT CONVERT(VARCHAR2(32),C011) FROM T_00_901 WHERE C001={0})", IpId);
                        //}
                        //else
                        //{
                        where += string.Format(" AND C020 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", IpId);
                        //}
                    }
                    if (str.Substring(0, 2) == "CA" && S6101App.PacketMode.Contains("E"))
                    {
                        call = str.Substring(2);
                        where += " AND C045=" + call;
                    }
                }
                if (IsAgent && S6101App.AgentNameList.Count != 0 && S6101App.PacketMode.Contains("A"))
                {
                    AgId = s6101App.PutTempData(S6101App.AgentNameList); IsAgent = false;
                    //if (App.Session.DBType == 2)
                    //{
                    //    where = string.Format(" AND C039 IN (SELECT CONVERT(NVARCHAR(128),C011) FROM T_00_901 WHERE C001={0}) {1}", AgId, where);
                    //}
                    //else
                    //{
                    where = string.Format(" AND C039 IN (SELECT C011 FROM T_00_901 WHERE C001={0}) {1}", AgId, where);
                    //}
                }
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR2DataSet(where, i);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR2DataSet(where, i);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                            foreach (DataRow dr in DS_temp.Tables[1].Rows)
                                dataSet.Tables[1].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]); ReportViewer1.LocalReport.DataSources.Add(rds);
                        List<string> AgentList = new List<string>(); DataTable DTChart = dataSet.Tables[1].Clone();
                        foreach (DataRow agent in dataSet.Tables[1].Rows)
                        {
                            bool flag = true;
                            for (int k = 0; k < AgentList.Count; k++)
                            {
                                if (agent["C039"].ToString() == AgentList[k])
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                AgentList.Add(agent["C039"].ToString());
                            }
                        }
                        for (int q = 0; q < AgentList.Count; q++)
                        {
                            int cin = 0; int cout = 0;
                            foreach (DataRow dr in dataSet.Tables[1].Rows)
                            {
                                if (AgentList[q] == dr["C039"].ToString())
                                {
                                    cin += int.Parse(dr["P202"].ToString()); cout += int.Parse(dr["P203"].ToString());
                                }
                            }
                            DataRow row = DTChart.NewRow(); row["C039"] = AgentList[q].ToString(); row["P202"] = cin; row["P203"] = cout; DTChart.Rows.Add(row);
                        }
                        ReportDataSource rds_chart = new ReportDataSource("DataSet2", DTChart); ReportViewer1.LocalReport.DataSources.Add(rds_chart);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("2:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R4(List<string> StrQ)
        {
            #region 统计条件
            List<string> AG = new List<string>(); List<string> GN = new List<string>();
            List<string> GP = new List<string>(); string DeId = string.Empty; string GpId = string.Empty; bool IsAgent = true; bool IsInspecter = true;
            string where = string.Empty; string AgId = string.Empty; string GnId = string.Empty; string table_name = string.Empty;
            if (StrQ.Count >= 2)
            {
                if (StrQ[0].Substring(0, 2) == "RT")
                {
                    table_name = "C002 ";
                }
                if (StrQ[0].Substring(0, 2) == "GT")
                {
                    table_name = "C006 ";
                }
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "GN")
                    {
                        GN = StrToList(str.Substring(2, str.Length - 2));
                        GnId = s6101App.PutTempData(GN);
                        where += string.Format(" AND C003 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GnId);
                    }
                    if (str.Substring(0, 2) == "AG")
                    {
                        AG = StrToList(str.Substring(2, str.Length - 2));
                        AgId = s6101App.PutTempData(AG); IsAgent = false;
                        where += string.Format(" AND C013 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                    }
                    if (str.Substring(0, 2) == "GP")
                    {
                        GP = StrToList(str.Substring(2, str.Length - 2));
                        GpId = s6101App.PutTempData(GP); IsInspecter = false;
                        where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                    }
                }
                if (IsAgent && S6101App.AgentList.Count != 0 && S6101App.PacketMode.Contains("A"))
                {
                    AgId = s6101App.PutTempData(S6101App.AgentList); IsAgent = false;
                    where += string.Format(" AND C013 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                }
                //if (IsInspecter && S6101App.InspecterList.Count != 0)
                //{
                //    GpId = S6101App.PutTempData(S6101App.InspecterList); IsInspecter = false;
                //    where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                //}
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet(); DataTable DTChart = new DataTable();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR4DataSet(where, i, table_name);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR4DataSet(where, i, table_name);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                            foreach (DataRow dr in DS_temp.Tables[1].Rows)
                                dataSet.Tables[1].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]); ReportViewer1.LocalReport.DataSources.Add(rds);
                        List<string> AgentList = new List<string>(); DTChart = dataSet.Tables[1].Clone();
                        foreach (DataRow agent in dataSet.Tables[1].Rows)
                        {
                            bool flag = true;
                            for (int k = 0; k < AgentList.Count; k++)
                            {
                                if (agent["P400"].ToString() == AgentList[k])
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                AgentList.Add(agent["P400"].ToString());
                            }
                        }
                        for (int q = 0; q < AgentList.Count; q++)
                        {
                            double min = 1000; double max = 0; double sum = 0; int count = 0;
                            foreach (DataRow dr in dataSet.Tables[1].Rows)
                            {
                                if (AgentList[q] == dr["P400"].ToString())
                                {
                                    count++; sum += double.Parse(dr["P402"].ToString());
                                    if (double.Parse(dr["P403"].ToString()) > max)
                                        max = double.Parse(dr["P403"].ToString());
                                    if (double.Parse(dr["P404"].ToString()) < min)
                                        min = double.Parse(dr["P404"].ToString());
                                }
                            }
                            DataRow row = DTChart.NewRow(); row["P400"] = AgentList[q].ToString(); row["P402"] = sum / count; row["P403"] = max; row["P404"] = min; DTChart.Rows.Add(row);
                        }
                        ReportDataSource rds_chart = new ReportDataSource("DataSet2", DTChart); ReportViewer1.LocalReport.DataSources.Add(rds_chart);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("4:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R5(List<string> StrQ)
        {
            #region 统计条件
            List<string> AG = new List<string>(); List<string> GN = new List<string>(); string IA = string.Empty;
            List<string> GP = new List<string>(); string DeId = string.Empty; string GpId = string.Empty; bool IsAgent = true; bool IsInspecter = true;
            string where = string.Empty; string AgId = string.Empty; string GnId = string.Empty; string table_name = string.Empty; string IaId = string.Empty;
            if (StrQ.Count >= 2)
            {
                if (StrQ[0].Substring(0, 2) == "RT")
                {
                    table_name = "C052 ";
                }
                if (StrQ[0].Substring(0, 2) == "GT")
                {
                    table_name = "C003 ";
                }
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "AG")
                    {
                        AG = StrToList(str.Substring(2, str.Length - 2));
                        AgId = s6101App.PutTempData(AG); IsAgent = false;
                        where += string.Format(" AND C017 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                    }
                    if (str.Substring(0, 2) == "GP")
                    {
                        GP = StrToList(str.Substring(2, str.Length - 2));
                        GpId = s6101App.PutTempData(GP); IsInspecter = false;
                        where += string.Format(" AND C004 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                    }
                    if (str.Substring(0, 2) == "IA")
                    {
                        IA = (str.Substring(2, str.Length - 2));
                        where += string.Format(" AND C007 ='{0}' ", IA);
                    }
                    if (str.Substring(0, 2) == "GN")
                    {
                        GN = StrToList(str.Substring(2, str.Length - 2));
                        GnId = s6101App.PutTempData(GN);
                        where += string.Format(" AND C000 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GnId);
                    }
                }
                if (IsAgent && S6101App.AgentList.Count != 0 && S6101App.PacketMode.Contains("A"))
                {
                    AgId = s6101App.PutTempData(S6101App.AgentList); IsAgent = false;
                    where += string.Format(" AND C017 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                }
                //if (IsInspecter && S6101App.InspecterList.Count!=0)
                //{
                //    GpId = S6101App.PutTempData(S6101App.InspecterList); IsInspecter = false;
                //    where += string.Format(" AND C004 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                //}
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR5DataSet(where, i, table_name);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR5DataSet(where, i, table_name);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]); ReportViewer1.LocalReport.DataSources.Add(rds);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("5:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R6(List<string> StrQ)
        {
            #region 统计条件
            List<string> GN = new List<string>(); List<string> GP = new List<string>();
            string DeId = string.Empty; string GpId = string.Empty; string where = string.Empty;
            string GnId = string.Empty; string table_name = string.Empty; bool IsInspecter = true;
            if (StrQ.Count >= 2)
            {
                if (StrQ[0].Substring(0, 2) == "RT")
                {
                    table_name = "C002 ";
                }
                if (StrQ[0].Substring(0, 2) == "GT")
                {
                    table_name = "C006 ";
                }
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "GN")
                    {
                        GN = StrToList(str.Substring(2, str.Length - 2));
                        GnId = s6101App.PutTempData(GN);
                        where += string.Format(" AND C003 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GnId);
                    }
                    if (str.Substring(0, 2) == "GP")
                    {
                        GP = StrToList(str.Substring(2, str.Length - 2));
                        GpId = s6101App.PutTempData(GP); IsInspecter = false;
                        where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                    }
                }
                //if (IsInspecter && S6101App.InspecterList.Count != 0)
                //{
                //    GpId = S6101App.PutTempData(S6101App.InspecterList); IsInspecter = false;
                //    where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                //}
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR6DataSet(where, i, table_name);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR6DataSet(where, i, table_name);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                            foreach (DataRow dr in DS_temp.Tables[1].Rows)
                                dataSet.Tables[1].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]); ReportViewer1.LocalReport.DataSources.Add(rds);
                        //List<string> DepartList = new List<string>(); DataTable DTChart = dataSet.Tables[1].Clone();
                        //foreach (DataRow depart in dataSet.Tables[1].Rows)
                        //{
                        //    bool flag = true;
                        //    for (int k = 0; k < DepartList.Count; k++)
                        //    {
                        //        flag = true;
                        //        if (depart["C012"].ToString() == DepartList[k])
                        //        {
                        //            flag = false;
                        //            break;
                        //        }
                        //    }
                        //    if (flag)
                        //    {
                        //        DepartList.Add(depart["C012"].ToString());
                        //    }
                        //}
                        //for (int q = 0; q < DepartList.Count; q++)
                        //{
                        //    double score = 0; int count = 0;
                        //    foreach (DataRow dep in dataSet.Tables[1].Rows)
                        //    {
                        //        if (DepartList[q].ToString() == dep["C012"].ToString())
                        //        {
                        //            score += double.Parse(dep["P601"].ToString()); count++;
                        //        }
                        //    }
                        //    if (score != 0)
                        //        score /= count;
                        //    DataRow row = DTChart.NewRow(); row["C012"] = DepartList[q].ToString(); row["P601"] = score; DTChart.Rows.Add(row);
                        //}
                        ReportDataSource rds_chart = new ReportDataSource("DataSet2", dataSet.Tables[1]); ReportViewer1.LocalReport.DataSources.Add(rds_chart);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("6:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R7(List<string> StrQ)
        {
            #region 统计条件
            List<string> GN = new List<string>(); List<string> GP = new List<string>(); bool IsInspecter = true;
            string where = string.Empty; string where1 = string.Empty; string table_name = string.Empty; string GpId = string.Empty; string GnId = string.Empty;
            if (StrQ.Count >= 2)
            {
                if (StrQ[0].Substring(0, 2) == "BT")
                {
                    table_name = " D.C006 ";
                }
                if (StrQ[0].Substring(0, 2) == "FT")
                {
                    table_name = " D.C009 ";
                }
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "GN")
                    {
                        GN = StrToList(str.Substring(2, str.Length - 2));
                        GnId = s6101App.PutTempData(GN);
                        where += string.Format(" AND C003 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GnId);
                    }
                    if (str.Substring(0, 2) == "GP")
                    {
                        GP = StrToList(str.Substring(2, str.Length - 2));
                        GpId = s6101App.PutTempData(GP); IsInspecter = false;
                        where1 += string.Format(" AND B.C002 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                    }
                }
                //if (IsInspecter && S6101App.InspecterList.Count != 0)
                //{
                //    GpId = S6101App.PutTempData(S6101App.InspecterList); IsInspecter = false;
                //    where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                //}
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR7DataSet(where1, where, i, table_name);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR7DataSet(where1, where, i, table_name);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                            foreach (DataRow dr in DS_temp.Tables[1].Rows)
                                dataSet.Tables[1].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]); ReportViewer1.LocalReport.DataSources.Add(rds);
                        List<string> DepartList = new List<string>(); DataTable DTChart = dataSet.Tables[1].Clone();
                        foreach (DataRow depart in dataSet.Tables[1].Rows)
                        {
                            bool flag = true;
                            for (int k = 0; k < DepartList.Count; k++)
                            {
                                if (depart["P710"].ToString() == DepartList[k])
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                DepartList.Add(depart["P710"].ToString());
                            }
                        }
                        for (int q = 0; q < DepartList.Count; q++)
                        {
                            int Num = 0;
                            foreach (DataRow dep in dataSet.Tables[1].Rows)
                            {
                                if (DepartList[q].ToString() == dep["P710"].ToString())
                                {
                                    Num += int.Parse(dep["P701"].ToString());
                                }
                            }
                            DataRow row = DTChart.NewRow(); row["P710"] = DepartList[q].ToString(); row["P701"] = Num; DTChart.Rows.Add(row);
                        }
                        ReportDataSource rds_chart = new ReportDataSource("DataSet2", DTChart); ReportViewer1.LocalReport.DataSources.Add(rds_chart);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("7:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R8(List<string> StrQ)
        {
            #region 统计条件
            List<string> GN = new List<string>(); List<string> GP = new List<string>(); List<string> AG = new List<string>();
            string DeId = string.Empty; string GpId = string.Empty; string where = string.Empty; bool IsInspecter = true;
            string table_name = string.Empty; string AgId = string.Empty; string GnId = string.Empty; bool IsAgent = true;
            if (StrQ.Count >= 2)
            {
                if (StrQ[0].Substring(0, 2) == "RT")
                {
                    table_name = "C002 ";
                }
                if (StrQ[0].Substring(0, 2) == "GT")
                {
                    table_name = "C006 ";
                }
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "AG")
                    {
                        AG = StrToList(str.Substring(2, str.Length - 2));
                        AgId = s6101App.PutTempData(AG); IsAgent = false;
                        where += string.Format(" AND C013 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                    }
                    if (str.Substring(0, 2) == "GN")
                    {
                        GN = StrToList(str.Substring(2, str.Length - 2));
                        GnId = s6101App.PutTempData(GN);
                        where += string.Format(" AND C003 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GnId);
                    }
                    if (str.Substring(0, 2) == "GP")
                    {
                        GP = StrToList(str.Substring(2, str.Length - 2));
                        GpId = s6101App.PutTempData(GP); IsInspecter = false;
                        where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                    }
                }
                if (IsAgent && S6101App.AgentList.Count != 0 && S6101App.PacketMode.Contains("A"))
                {
                    AgId = s6101App.PutTempData(S6101App.AgentList); IsAgent = false;
                    where += string.Format(" AND C013 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                }
                //if (IsInspecter && S6101App.InspecterList.Count != 0)
                //{
                //    GpId = S6101App.PutTempData(S6101App.InspecterList); IsInspecter = false;
                //    where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                //}
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR8DataSet(where, i, table_name);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR8DataSet(where, i, table_name);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                            foreach (DataRow dr in DS_temp.Tables[1].Rows)
                                dataSet.Tables[1].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]); ReportViewer1.LocalReport.DataSources.Add(rds);
                        DataTable DTChart = dataSet.Tables[1].Clone();
                        ReportDataSource rds_chart = new ReportDataSource("DataSet2", dataSet.Tables[1]); ReportViewer1.LocalReport.DataSources.Add(rds_chart);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("8:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R9(List<string> StrQ)
        {
            #region 统计条件
            List<string> GN = new List<string>(); List<string> GP = new List<string>(); bool IsInspecter = true;
            string DeId = string.Empty; string GpId = string.Empty; string where = string.Empty; string GnId = string.Empty;
            string table_name = string.Empty;
            if (StrQ.Count >= 2)
            {
                if (StrQ[0].Substring(0, 2) == "RT")
                {
                    table_name = "C002 ";
                }
                if (StrQ[0].Substring(0, 2) == "GT")
                {
                    table_name = "C006 ";
                }
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "GN")
                    {
                        GN = StrToList(str.Substring(2, str.Length - 2));
                        GnId = s6101App.PutTempData(GN);
                        where += string.Format(" AND C003 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GnId);
                    }
                    if (str.Substring(0, 2) == "GP")
                    {
                        GP = StrToList(str.Substring(2, str.Length - 2));
                        GpId = s6101App.PutTempData(GP); IsInspecter = false;
                        where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                    }
                }
                //if (IsInspecter && S6101App.InspecterList.Count != 0)
                //{
                //    GpId = S6101App.PutTempData(S6101App.InspecterList); IsInspecter = false;
                //    where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                //}
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR9DataSet(where, i, table_name);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR9DataSet(where, i, table_name);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]); ReportViewer1.LocalReport.DataSources.Add(rds);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("9:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R10(List<string> StrQ)
        {
            #region 统计条件
            List<string> GN = new List<string>(); List<string> GP = new List<string>(); List<string> AG = new List<string>();
            string DeId = string.Empty; string GpId = string.Empty; string where = string.Empty; bool IsInspecter = true;
            string GnId = string.Empty; string table_name = string.Empty; string AgId = string.Empty; bool IsAgent = true;
            if (StrQ.Count >= 2)
            {
                if (StrQ[0].Substring(0, 2) == "RT")
                {
                    table_name = "C002 ";
                }
                if (StrQ[0].Substring(0, 2) == "GT")
                {
                    table_name = "C006 ";
                }
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "AG")
                    {
                        AG = StrToList(str.Substring(2, str.Length - 2));
                        AgId = s6101App.PutTempData(AG); IsAgent = false;
                        where += string.Format(" AND C013 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                    }
                    if (str.Substring(0, 2) == "GN")
                    {
                        GN = StrToList(str.Substring(2, str.Length - 2));
                        GnId = s6101App.PutTempData(GN);
                        where += string.Format(" AND C003 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GnId);
                    }
                    if (str.Substring(0, 2) == "GP")
                    {
                        GP = StrToList(str.Substring(2, str.Length - 2));
                        GpId = s6101App.PutTempData(GP); IsInspecter = false;
                        where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                    }
                }
                if (IsAgent && S6101App.AgentList.Count != 0 && S6101App.PacketMode.Contains("A"))
                {
                    AgId = s6101App.PutTempData(S6101App.AgentList); IsAgent = false;
                    where += string.Format(" AND C013 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                }
                //if (IsInspecter && S6101App.InspecterList.Count != 0)
                //{
                //    GpId = S6101App.PutTempData(S6101App.InspecterList); IsInspecter = false;
                //    where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                //}
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR10DataSet(where, i, table_name);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR10DataSet(where, i, table_name);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]); ReportViewer1.LocalReport.DataSources.Add(rds);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("10:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R11(List<string> StrQ)
        {
            #region 统计条件
            List<string> GP = new List<string>(); List<string> AG = new List<string>();
            string DeId = string.Empty; string GpId = string.Empty; string where = string.Empty; string AgId = string.Empty;
            bool IsAgent = true;
            if (StrQ.Count >= 2)
            {
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "AG")
                    {
                        AG = StrToList(str.Substring(2, str.Length - 2));
                        AgId = s6101App.PutTempData(AG); IsAgent = false;
                        //if (App.Session.DBType == 2)
                        //{
                        //    where = string.Format(" AND A.C039 IN (SELECT CONVERT(NVARCHAR(128),C011) FROM T_00_901 WHERE C001={0}) {1}", AgId, where);
                        //}
                        //else
                        where += string.Format(" AND A.C039 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                    }
                    if (str.Substring(0, 2) == "EX")
                    {
                        GP = StrToList(str.Substring(2, str.Length - 2));
                        GpId = s6101App.PutTempData(GP);
                        //if (App.Session.DBType == 2)
                        //{
                        //    where = string.Format(" AND A.C042 IN (SELECT CONVERT(NVARCHAR(128),C011) FROM T_00_901 WHERE C001={0} AND C012=A.C020) {1}", GpId, where);
                        //}
                        //else
                        where += string.Format(" AND A.C042 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                    }
                }
                if (IsAgent && S6101App.AgentNameList.Count != 0 && S6101App.PacketMode.Contains("A"))
                {
                    AgId = s6101App.PutTempData(S6101App.AgentNameList); IsAgent = false;
                    //if (App.Session.DBType == 2)
                    //{
                    //    where = string.Format(" AND A.C039 IN (SELECT CONVERT(NVARCHAR(128),C011) FROM T_00_901 WHERE C001={0}) {1}", AgId, where);
                    //}
                    //else
                    where = string.Format(" AND A.C039 IN (SELECT C011 FROM T_00_901 WHERE C001={0}) {1}", AgId, where);
                }
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR11DataSet(where, i);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR11DataSet(where, i);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                            foreach (DataRow dr in DS_temp.Tables[1].Rows)
                                dataSet.Tables[1].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]); ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportDataSource rds_chart = new ReportDataSource("DataSet2", dataSet.Tables[1]); ReportViewer1.LocalReport.DataSources.Add(rds_chart);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("11:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R12(List<string> StrQ)
        {
            #region 统计条件
            List<string> GN = new List<string>(); List<string> GP = new List<string>();
            string DeId = string.Empty; string GpId = string.Empty; string where = string.Empty; bool IsInspecter = true;
            string GnId = string.Empty; string table_name = string.Empty;
            if (StrQ.Count >= 2)
            {
                if (StrQ[0].Substring(0, 2) == "RT")
                {
                    table_name = "C002 ";
                }
                if (StrQ[0].Substring(0, 2) == "GT")
                {
                    table_name = "C006 ";
                }
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "GN")
                    {
                        GN = StrToList(str.Substring(2, str.Length - 2));
                        GnId = s6101App.PutTempData(GN);
                        where += string.Format(" AND C003 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GnId);
                    }
                    if (str.Substring(0, 2) == "GP")
                    {
                        GP = StrToList(str.Substring(2, str.Length - 2));
                        GpId = s6101App.PutTempData(GP); IsInspecter = false;
                        where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                    }
                }
                //if (IsInspecter && S6101App.InspecterList.Count != 0)
                //{
                //    GpId = S6101App.PutTempData(S6101App.InspecterList); IsInspecter = false;
                //    where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                //}
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR12DataSet(where, i, table_name);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR12DataSet(where, i, table_name);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]); ReportViewer1.LocalReport.DataSources.Add(rds);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("12:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Valuable Data"));
                ReportResult = false;
            }
        }

        private void R13(List<string> StrQ)
        {
            #region 统计条件
            List<string> GN = new List<string>(); List<string> GP = new List<string>(); bool IsInspecter = true;
            string where = string.Empty; string table_name = string.Empty; string GpId = string.Empty; string GnId = string.Empty;
            if (StrQ.Count >= 2)
            {
                if (StrQ[0].Substring(0, 2) == "GT")
                {
                    table_name = "C006 ";
                }
                if (StrQ[0].Substring(0, 2) == "RT")
                {
                    table_name = "C002 ";
                }
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "GN")
                    {
                        GN = StrToList(str.Substring(2, str.Length - 2));
                        GnId = s6101App.PutTempData(GN);
                        where += string.Format(" AND C003 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GnId);
                    }
                    if (str.Substring(0, 2) == "GP")
                    {
                        GP = StrToList(str.Substring(2, str.Length - 2));
                        GpId = s6101App.PutTempData(GP); IsInspecter = false;
                        where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                    }
                }
                //if (IsInspecter && S6101App.InspecterList.Count != 0)
                //{
                //    GpId = S6101App.PutTempData(S6101App.InspecterList); IsInspecter = false;
                //    where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                //}
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR13DataSet(where, i, table_name);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR13DataSet(where, i, table_name);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                            foreach (DataRow dr in DS_temp.Tables[1].Rows)
                                dataSet.Tables[1].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]); ReportViewer1.LocalReport.DataSources.Add(rds);
                        List<string> DepartList = new List<string>(); DataTable DTChart = dataSet.Tables[1].Clone();
                        foreach (DataRow depart in dataSet.Tables[1].Rows)
                        {
                            bool flag = true;
                            for (int k = 0; k < DepartList.Count; k++)
                            {
                                if (depart["P710"].ToString() == DepartList[k])
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                DepartList.Add(depart["P710"].ToString());
                            }
                        }
                        for (int q = 0; q < DepartList.Count; q++)
                        {
                            int Num = 0;
                            foreach (DataRow dep in dataSet.Tables[1].Rows)
                            {
                                if (DepartList[q].ToString() == dep["P710"].ToString())
                                {
                                    Num += int.Parse(dep["P703"].ToString());
                                }
                            }
                            DataRow row = DTChart.NewRow(); row["P710"] = DepartList[q].ToString(); row["P703"] = Num; DTChart.Rows.Add(row);
                        }
                        ReportDataSource rds_chart = new ReportDataSource("DataSet2", DTChart); ReportViewer1.LocalReport.DataSources.Add(rds_chart);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("13:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R14(List<string> StrQ)
        {
            #region 统计条件
            List<string> GN = new List<string>(); List<string> GP = new List<string>();
            string DeId = string.Empty; string GpId = string.Empty; string where = string.Empty; bool IsInspecter = true;
            string GnId = string.Empty; string table_name = string.Empty;
            if (StrQ.Count >= 2)
            {
                if (StrQ[0].Substring(0, 2) == "RT")
                {
                    table_name = "T322.C011 ";
                }
                if (StrQ[0].Substring(0, 2) == "GT")
                {
                    table_name = "T308.C006 ";
                }
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "GN")
                    {
                        GN = StrToList(str.Substring(2, str.Length - 2));
                        GnId = s6101App.PutTempData(GN);
                        where += string.Format(" AND T308.C003 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GnId);
                    }
                    if (str.Substring(0, 2) == "GP")
                    {
                        GP = StrToList(str.Substring(2, str.Length - 2));
                        GpId = s6101App.PutTempData(GP); IsInspecter = false;
                        where += string.Format(" AND T308.C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                    }
                }
                //if (IsInspecter && S6101App.InspecterList.Count != 0)
                //{
                //    GpId = S6101App.PutTempData(S6101App.InspecterList);
                //    where += string.Format(" AND T308.C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                //}
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR14DataSet(where, i, table_name);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR14DataSet(where, i, table_name);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]); ReportViewer1.LocalReport.DataSources.Add(rds);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("14:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R15(List<string> StrQ)
        {
            #region 统计条件
            List<string> GN = new List<string>(); List<string> GP = new List<string>(); string where = string.Empty; bool IsInspecter = true;
            string DeId = string.Empty; string GpId = string.Empty; string GnId = string.Empty; string table_name = string.Empty;
            if (StrQ.Count >= 2)
            {
                if (StrQ[0].Substring(0, 2) == "RT")
                {
                    table_name = "C002 ";
                }
                if (StrQ[0].Substring(0, 2) == "GT")
                {
                    table_name = "C006 ";
                }
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "GN")
                    {
                        GN = StrToList(str.Substring(2, str.Length - 2));
                        GnId = s6101App.PutTempData(GN);
                        where += string.Format(" AND C003 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GnId);
                    }
                    if (str.Substring(0, 2) == "GP")
                    {
                        GP = StrToList(str.Substring(2, str.Length - 2));
                        GpId = s6101App.PutTempData(GP); IsInspecter = false;
                        where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                    }
                }
                //if (IsInspecter && S6101App.InspecterList.Count != 0)
                //{
                //    GpId = S6101App.PutTempData(S6101App.InspecterList); IsInspecter = false;
                //    where += string.Format(" AND C005 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                //}
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR15DataSet(where, i, table_name);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR15DataSet(where, i, table_name);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("15:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R16(List<string> StrQ)
        {
            #region 统计条件
            List<string> AG = new List<string>(); List<string> GN = new List<string>();
            List<string> GP = new List<string>(); string DeId = string.Empty; string GpId = string.Empty;
            bool IsAgent = true; bool IsInspecter = true; bool IsGN = true;
            string where = string.Empty; string AgId = string.Empty; string GnId = string.Empty; string table_name = string.Empty;
            if (StrQ.Count >= 2)
            {
                if (StrQ[0].Substring(0, 2) == "RT")
                {
                    table_name = "C052 ";
                }
                if (StrQ[0].Substring(0, 2) == "GT")
                {
                    table_name = "C003 ";
                }
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "AG")
                    {
                        AG = StrToList(str.Substring(2, str.Length - 2));
                        AgId = s6101App.PutTempData(AG); IsAgent = false;
                        where += string.Format(" AND C017 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                    }
                    if (str.Substring(0, 2) == "GP")
                    {
                        GP = StrToList(str.Substring(2, str.Length - 2));
                        GpId = s6101App.PutTempData(GP); IsInspecter = false;
                        where += string.Format(" AND C004 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                    }

                    if (str.Substring(0, 2) == "GN")
                    {
                        GN = StrToList(str.Substring(2, str.Length - 2));
                        if (GN.Count() > 1)
                        {
                            s6101App.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0004", "仅能选择一张评分表"));
                            return;
                        }
                        //GnId = S6101App.PutTempData(GN); 
                        IsGN = false;
                        where += string.Format(" AND C000 ={0} ", GN[0]);
                    }
                }
                if (IsGN)
                {
                    s6101App.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0003", "请选择一张评分表"));
                    return;
                }
                if (IsAgent && S6101App.AgentList.Count != 0 && S6101App.PacketMode.Contains("A"))
                {
                    AgId = s6101App.PutTempData(S6101App.AgentList);
                    where += string.Format(" AND C017 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                }
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet(); DataTable DTChart = new DataTable();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR16DataSet(where, i, table_name);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR16DataSet(where, i, table_name);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                            foreach (DataRow dr in DS_temp.Tables[1].Rows)
                                dataSet.Tables[1].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        this.ReportViewer1.LocalReport.ReportEmbeddedResource = null;
                        this.ReportViewer1.LocalReport.ReportPath = S6101App.Path16;
                        this.ReportViewer1.LocalReport.SetParameters(rp);
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Add(rds);

                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
                //string pathR = System.IO.Path.GetDirectoryName("Report16.rdlc");
                //MessageBox.Show(pathR);
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("16:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R17(List<string> StrQ)
        {
            #region 统计条件
            List<string> AG = new List<string>(); List<string> GN = new List<string>();
            List<string> GP = new List<string>(); string DeId = string.Empty; string GpId = string.Empty;
            bool IsAgent = true; bool IsInspecter = true; bool IsGN = true;
            string where = string.Empty; string AgId = string.Empty; string GnId = string.Empty; string table_name = string.Empty;
            if (StrQ.Count >= 2)
            {
                if (StrQ[0].Substring(0, 2) == "RT")
                {
                    table_name = "C052 ";
                }
                if (StrQ[0].Substring(0, 2) == "GT")
                {
                    table_name = "C003 ";
                }
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "AG")
                    {
                        AG = StrToList(str.Substring(2, str.Length - 2));
                        AgId = s6101App.PutTempData(AG); IsAgent = false;
                        where += string.Format(" AND C017 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                    }
                    if (str.Substring(0, 2) == "GP")
                    {
                        GP = StrToList(str.Substring(2, str.Length - 2));
                        GpId = s6101App.PutTempData(GP); IsInspecter = false;
                        where += string.Format(" AND C004 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                    }

                    if (str.Substring(0, 2) == "GN")
                    {
                        GN = StrToList(str.Substring(2, str.Length - 2));
                        if (GN.Count() > 1)
                        {
                            CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0004", "仅能选择一张评分表"));
                            return;
                        }
                        GnId = s6101App.PutTempData(GN); IsGN = false;
                        where += string.Format(" AND C000 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GnId);
                    }
                }
                if (IsGN)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0003", "请选择一张评分表"));
                    return;
                }
                if (IsAgent && S6101App.AgentList.Count != 0 && S6101App.PacketMode.Contains("A"))
                {
                    AgId = s6101App.PutTempData(S6101App.AgentList); IsAgent = false;
                    where += string.Format(" AND C017 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                }
                //if (IsInspecter && S6101App.InspecterList.Count != 0)
                //{
                //    GpId = S6101App.PutTempData(S6101App.InspecterList); IsInspecter = false;
                //    where += string.Format(" AND C004 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", GpId);
                //}
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet(); DataTable DTChart = new DataTable();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR17DataSet(where, i, table_name);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR17DataSet(where, i, table_name);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        this.ReportViewer1.LocalReport.SetParameters(rp);
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        #region 图表表
                        DataTable DT_chart = new DataTable(); List<string> ExtenList = new List<string>();
                        DataColumn col_1701 = new DataColumn("P1701", typeof(string)); DT_chart.Columns.Add(col_1701);
                        DataColumn col_1704 = new DataColumn("P1704", typeof(string)); DT_chart.Columns.Add(col_1704);
                        DT_chart = dataSet.Tables[0];
                        #endregion
                        ReportDataSource rds_chart = new ReportDataSource("DataSet2", DT_chart);
                        ReportViewer1.LocalReport.DataSources.Add(rds_chart);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
                //string pathR = System.IO.Path.GetDirectoryName("Report16.rdlc");
                //MessageBox.Show(pathR);
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("17:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R18(List<string> StrQ)
        {
            #region 统计条件:时间+分机
            List<string> AG = new List<string>();
            string where = string.Empty; string AgId = string.Empty; string table_name = string.Empty;
            if (StrQ.Count >= 4)
            {
                table_name = StrQ[3];
                //根据table_name来修改参数内容：数量、时长、转接
                switch (table_name)
                {
                    case "0":
                        rp[7] = new ReportParameter("P1801", CurrentApp.GetLanguageInfo("6101121801", ""));
                        break;
                    case "1":
                        rp[7] = new ReportParameter("P1801", CurrentApp.GetLanguageInfo("6101121802", ""));
                        break;
                    case "2":
                        rp[7] = new ReportParameter("P1801", CurrentApp.GetLanguageInfo("6101121803", ""));
                        break;
                }
                string str = StrQ[2];
                if (str.Substring(0, 2) == "EX")
                {
                    AG = StrToList(str.Substring(2, str.Length - 2));
                    AgId = s6101App.PutTempData(AG);
                    where += string.Format(" AND C042 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                }
            }
            else
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("", "NO Extension"));
                return;
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet(); DataTable DTChart = new DataTable();
            try
            {
                S6101App.Flag18 = true;
                int LoapNum = 99;
                if (S6101App.ADT.BeginDateTime.Count < 99)
                {
                    LoapNum = S6101App.ADT.BeginDateTime.Count;
                }
                for (int i = 0; i < LoapNum; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR18DataSet(where, i, table_name, AG);//传参：查询语句（主要是分机的），i需要，统计哪个（012）
                    }
                    else
                    {
                        DS_temp = s6101App.GetR18DataSet(where, i, table_name, AG);
                        if (DS_temp.Tables.Count != 0)
                        {
                            int numCol = i + 101;
                            string columnName = "C" + numCol.ToString();
                            for (int Rownum = 0; Rownum < DS_temp.Tables[0].Rows.Count; Rownum++)
                            {
                                dataSet.Tables[0].Rows[Rownum][columnName] = DS_temp.Tables[0].Rows[Rownum][columnName].ToString();
                            }
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        this.ReportViewer1.LocalReport.ReportEmbeddedResource = null;
                        this.ReportViewer1.LocalReport.ReportPath = null;
                        this.ReportViewer1.LocalReport.ReportPath = S6101App.Path18;
                        this.ReportViewer1.LocalReport.SetParameters(rp);
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Add(rds);

                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
                //string pathR = System.IO.Path.GetDirectoryName("Report16.rdlc");
                //MessageBox.Show(pathR);
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("18:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R19(List<string> StrQ)
        {
            #region 统计条件
            List<string> AG = new List<string>(); List<string> GN = new List<string>(); List<string> GS = new List<string>();
            List<string> GP = new List<string>(); string DeId = string.Empty; string GpId = string.Empty;
            bool IsAgent = true; bool IsGN = true;
            string where = string.Empty; string AgId = string.Empty; string GnId = string.Empty; string table_name = string.Empty;
            if (StrQ.Count >= 2)
            {
                if (StrQ[0].Substring(0, 2) == "RT")
                {
                    table_name = "C052 ";
                }
                if (StrQ[0].Substring(0, 2) == "GT")
                {
                    table_name = "C003 ";
                }
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "AG")
                    {
                        AG = StrToList(str.Substring(2, str.Length - 2));
                        AgId = s6101App.PutTempData(AG); IsAgent = false;
                        where += string.Format(" AND C017 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                    }
                    if (str.Substring(0, 2) == "GS")
                    {
                        GS = StrToList(str.Substring(2, str.Length - 2));
                    }
                    if (str.Substring(0, 2) == "GN")
                    {
                        GN = StrToList(str.Substring(2, str.Length - 2));
                        if (GN.Count() > 1)
                        {
                            CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0004", "仅能选择一张评分表"));
                            return;
                        }
                        //GnId = S6101App.PutTempData(GN); 
                        IsGN = false;
                        where += string.Format(" AND C000 ={0} ", GN[0]);
                    }
                }
                if (IsGN)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0003", "请选择一张评分表"));
                    return;
                }
                if (IsAgent && S6101App.AgentList.Count != 0 && S6101App.PacketMode.Contains("A"))
                {
                    AgId = s6101App.PutTempData(S6101App.AgentList);
                    where += string.Format(" AND C017 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", AgId);
                }
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet(); DataTable DTChart = new DataTable();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR19DataSet(where, i, table_name, GS);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR19DataSet(where, i, table_name, GS);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        //this.ReportViewer1.LocalReport.ReportEmbeddedResource = null;
                        this.ReportViewer1.LocalReport.ReportPath = S6101App.Path19;
                        this.ReportViewer1.LocalReport.SetParameters(rp);
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportDataSource rds_chart = new ReportDataSource("DataSet2", dataSet.Tables[1]);
                        ReportViewer1.LocalReport.DataSources.Add(rds_chart);
                        this.ReportViewer1.RefreshReport();

                        #region Listview显示
                        //this.ListViewReport.ItemsSource = dataSet.Tables[0].Rows;
                        //foreach (DataRow dr1 in dataSet.Tables[0].Rows)
                        {
                            //AgentScoreSheetStatistics ASS = new AgentScoreSheetStatistics();
                            //ASS.AgentID = dr1["C056"].ToString();
                            //ASS.AgentName = dr1["C017"].ToString();
                            //ASS.Department = dr1["C015"].ToString();
                            //ASS.RecordNum = dr1["C001"].ToString();
                            //ASS.ScoreSheetName = dr1["C000"].ToString();
                            //ASS.TotalAverage = dr1["C100"].ToString();
                            #region 赋值
                            //ASS.ScoreSheetItem1 = dr1["C101"].ToString();
                            //ASS.ScoreSheetItem2 = dr1["C102"].ToString();
                            //ASS.ScoreSheetItem3 = dr1["C103"].ToString();
                            //ASS.ScoreSheetItem4 = dr1["C104"].ToString();
                            //ASS.ScoreSheetItem5 = dr1["C105"].ToString();
                            //ASS.ScoreSheetItem6 = dr1["C106"].ToString();
                            //ASS.ScoreSheetItem7 = dr1["C107"].ToString();
                            //ASS.ScoreSheetItem8 = dr1["C108"].ToString();
                            //ASS.ScoreSheetItem9 = dr1["C109"].ToString();
                            //ASS.ScoreSheetItem10 = dr1["C110"].ToString();

                            //ASS.ScoreSheetItem11 = dr1["C111"].ToString();
                            //ASS.ScoreSheetItem12 = dr1["C112"].ToString();
                            //ASS.ScoreSheetItem13 = dr1["C113"].ToString();
                            //ASS.ScoreSheetItem14 = dr1["C114"].ToString();
                            //ASS.ScoreSheetItem15 = dr1["C115"].ToString();
                            //ASS.ScoreSheetItem16 = dr1["C116"].ToString();
                            //ASS.ScoreSheetItem17 = dr1["C117"].ToString();
                            //ASS.ScoreSheetItem18 = dr1["C118"].ToString();
                            //ASS.ScoreSheetItem19 = dr1["C119"].ToString();
                            //ASS.ScoreSheetItem20 = dr1["C120"].ToString();

                            //ASS.ScoreSheetItem21 = dr1["C121"].ToString();
                            //ASS.ScoreSheetItem22 = dr1["C122"].ToString();
                            //ASS.ScoreSheetItem23 = dr1["C123"].ToString();
                            //ASS.ScoreSheetItem24 = dr1["C124"].ToString();
                            //ASS.ScoreSheetItem25 = dr1["C125"].ToString();
                            //ASS.ScoreSheetItem26 = dr1["C126"].ToString();
                            //ASS.ScoreSheetItem27 = dr1["C127"].ToString();
                            //ASS.ScoreSheetItem28 = dr1["C128"].ToString();
                            //ASS.ScoreSheetItem29 = dr1["C129"].ToString();
                            //ASS.ScoreSheetItem30 = dr1["C130"].ToString();

                            //ASS.ScoreSheetItem31 = dr1["C131"].ToString();
                            //ASS.ScoreSheetItem32 = dr1["C132"].ToString();
                            //ASS.ScoreSheetItem33 = dr1["C133"].ToString();
                            //ASS.ScoreSheetItem34 = dr1["C134"].ToString();
                            //ASS.ScoreSheetItem35 = dr1["C135"].ToString();
                            //ASS.ScoreSheetItem36 = dr1["C136"].ToString();
                            //ASS.ScoreSheetItem37 = dr1["C137"].ToString();
                            //ASS.ScoreSheetItem38 = dr1["C138"].ToString();
                            //ASS.ScoreSheetItem39 = dr1["C139"].ToString();
                            //ASS.ScoreSheetItem40 = dr1["C140"].ToString();

                            //ASS.ScoreSheetItem41 = dr1["C141"].ToString();
                            //ASS.ScoreSheetItem42 = dr1["C142"].ToString();
                            //ASS.ScoreSheetItem43 = dr1["C143"].ToString();
                            //ASS.ScoreSheetItem44 = dr1["C144"].ToString();
                            //ASS.ScoreSheetItem45 = dr1["C145"].ToString();
                            //ASS.ScoreSheetItem46 = dr1["C146"].ToString();
                            //ASS.ScoreSheetItem47 = dr1["C147"].ToString();
                            //ASS.ScoreSheetItem48 = dr1["C148"].ToString();
                            //ASS.ScoreSheetItem49 = dr1["C149"].ToString();
                            //ASS.ScoreSheetItem50 = dr1["C150"].ToString();

                            //ASS.ScoreSheetItem51 = dr1["C151"].ToString();
                            //ASS.ScoreSheetItem52 = dr1["C152"].ToString();
                            //ASS.ScoreSheetItem53 = dr1["C153"].ToString();
                            //ASS.ScoreSheetItem54 = dr1["C154"].ToString();
                            //ASS.ScoreSheetItem55 = dr1["C155"].ToString();
                            //ASS.ScoreSheetItem56 = dr1["C156"].ToString();
                            //ASS.ScoreSheetItem57 = dr1["C157"].ToString();
                            //ASS.ScoreSheetItem58 = dr1["C158"].ToString();
                            //ASS.ScoreSheetItem59 = dr1["C159"].ToString();
                            //ASS.ScoreSheetItem60 = dr1["C160"].ToString();

                            //ASS.ScoreSheetItem61 = dr1["C161"].ToString();
                            //ASS.ScoreSheetItem62 = dr1["C162"].ToString();
                            //ASS.ScoreSheetItem63 = dr1["C163"].ToString();
                            //ASS.ScoreSheetItem64 = dr1["C164"].ToString();
                            //ASS.ScoreSheetItem65 = dr1["C165"].ToString();
                            //ASS.ScoreSheetItem66 = dr1["C166"].ToString();
                            //ASS.ScoreSheetItem67 = dr1["C167"].ToString();
                            //ASS.ScoreSheetItem68 = dr1["C168"].ToString();
                            //ASS.ScoreSheetItem69 = dr1["C169"].ToString();
                            //ASS.ScoreSheetItem70 = dr1["C170"].ToString();

                            //ASS.ScoreSheetItem71 = dr1["C171"].ToString();
                            //ASS.ScoreSheetItem72 = dr1["C172"].ToString();
                            //ASS.ScoreSheetItem73 = dr1["C173"].ToString();
                            //ASS.ScoreSheetItem74 = dr1["C174"].ToString();
                            //ASS.ScoreSheetItem75 = dr1["C175"].ToString();
                            //ASS.ScoreSheetItem76 = dr1["C176"].ToString();
                            //ASS.ScoreSheetItem77 = dr1["C177"].ToString();
                            //ASS.ScoreSheetItem78 = dr1["C178"].ToString();
                            //ASS.ScoreSheetItem79 = dr1["C179"].ToString();
                            //ASS.ScoreSheetItem80 = dr1["C180"].ToString();

                            //ASS.ScoreSheetItem81 = dr1["C181"].ToString();
                            //ASS.ScoreSheetItem82 = dr1["C182"].ToString();
                            //ASS.ScoreSheetItem83 = dr1["C183"].ToString();
                            //ASS.ScoreSheetItem84 = dr1["C184"].ToString();
                            //ASS.ScoreSheetItem85 = dr1["C185"].ToString();
                            //ASS.ScoreSheetItem86 = dr1["C186"].ToString();
                            //ASS.ScoreSheetItem87 = dr1["C187"].ToString();
                            //ASS.ScoreSheetItem88 = dr1["C188"].ToString();
                            //ASS.ScoreSheetItem89 = dr1["C189"].ToString();
                            //ASS.ScoreSheetItem90 = dr1["C190"].ToString();

                            //ASS.ScoreSheetItem91 = dr1["C191"].ToString();
                            //ASS.ScoreSheetItem92 = dr1["C192"].ToString();
                            //ASS.ScoreSheetItem93 = dr1["C193"].ToString();
                            //ASS.ScoreSheetItem94 = dr1["C194"].ToString();
                            //ASS.ScoreSheetItem95 = dr1["C195"].ToString();
                            //ASS.ScoreSheetItem96 = dr1["C196"].ToString();
                            //ASS.ScoreSheetItem97 = dr1["C197"].ToString();
                            //ASS.ScoreSheetItem98 = dr1["C198"].ToString();
                            //ASS.ScoreSheetItem99 = dr1["C199"].ToString();
                            //ASS.ScoreSheetItem100 = dr1["C200"].ToString();

                            //ASS.ScoreSheetItem101 = dr1["C201"].ToString();
                            //ASS.ScoreSheetItem102 = dr1["C202"].ToString();
                            //ASS.ScoreSheetItem103 = dr1["C203"].ToString();
                            //ASS.ScoreSheetItem104 = dr1["C204"].ToString();
                            //ASS.ScoreSheetItem105 = dr1["C205"].ToString();
                            //ASS.ScoreSheetItem106 = dr1["C206"].ToString();
                            //ASS.ScoreSheetItem107 = dr1["C207"].ToString();
                            //ASS.ScoreSheetItem108 = dr1["C208"].ToString();
                            //ASS.ScoreSheetItem109 = dr1["C209"].ToString();
                            //ASS.ScoreSheetItem110 = dr1["C210"].ToString();
                            #endregion
                        }
                        #endregion
                    }
                    else
                        stand++;
                }

            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("19:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R20(List<string> StrQ)
        {
            #region 统计条件
            List<string> EX = new List<string>(); List<string> IP = new List<string>(); List<string> EP = new List<string>();
            string call = string.Empty; string where = string.Empty; List<string> TableName = new List<string>();
            string IpId = string.Empty; string AgId = string.Empty; string EpId = string.Empty; int DepartBasic = 0;

            if (StrQ.Count >= 2)
            {
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "IP")
                    {
                        IP = StrToList(str.Substring(2, str.Length - 2));
                        IpId = s6101App.PutTempData(IP);
                        where += string.Format(" AND A.C020 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", IpId);
                    }
                    if (str.Substring(0, 2) == "DB")
                    {
                        //获取参数，看关联的是哪个字段
                        if (str.Substring(2, 1) == "1")
                        {
                            DepartBasic = 1;
                        }
                    }
                    if (str.Substring(0, 2) == "CA")
                    {
                        call = str.Substring(2);
                        where += " AND C045=" + call;
                    }
                }
                if (S6101App.AgentNameList.Count != 0 && S6101App.PacketMode.Contains("A"))
                {
                    AgId = s6101App.PutTempData(S6101App.AgentNameList);

                    where += string.Format(" AND A.C039 IN (SELECT C011 FROM T_00_901 WHERE C001={0}) ", AgId);
                }
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        s6101App.WriteLog(string.Format("(1):Start OK"));
                        dataSet = s6101App.GetR20DataSet(where, i, DepartBasic);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR20DataSet(where, i, DepartBasic);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        s6101App.WriteLog(string.Format("(1):Table2 begine"));
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        #region 图表表
                        DataTable DT_chart = new DataTable();
                        DT_chart = dataSet.Tables[1];
                        // List<string> ExtenList = new List<string>();
                        //DataColumn col_exten = new DataColumn("P110", typeof(string)); DT_chart.Columns.Add(col_exten);
                        //DataColumn col_cin = new DataColumn("P111", typeof(string)); DT_chart.Columns.Add(col_cin);
                        //DataColumn col_cout = new DataColumn("P112", typeof(string)); DT_chart.Columns.Add(col_cout);
                        //foreach (DataRow exten in dataSet.Tables[0].Rows)
                        //{
                        //    bool flag = true;
                        //    for (int k = 0; k < ExtenList.Count; k++)
                        //    {
                        //        if (exten["P2001"].ToString() == ExtenList[k])
                        //        {
                        //            flag = false;
                        //            break;
                        //        }
                        //    }
                        //    if (flag)
                        //    {
                        //        ExtenList.Add(exten["P2001"].ToString());
                        //    }
                        //}
                        //for (int q = 0; q < ExtenList.Count; q++)
                        //{
                        //    int cin = 0; int cout = 0;
                        //    foreach (DataRow dr in dataSet.Tables[0].Rows)
                        //    {
                        //        if (ExtenList[q] == dr["P2001"].ToString())
                        //        {
                        //            cin += int.Parse(dr["P100"].ToString()); cout += int.Parse(dr["P101"].ToString());
                        //        }
                        //    }
                        //    DataRow row = DT_chart.NewRow(); row["P110"] = ExtenList[q].ToString(); row["P111"] = cin; row["P112"] = cout; DT_chart.Rows.Add(row);
                        //}
                        #endregion
                        ReportDataSource rds_chart = new ReportDataSource("DataSet2", DT_chart);
                        ReportViewer1.LocalReport.DataSources.Add(rds_chart);
                        this.ReportViewer1.RefreshReport();
                        s6101App.WriteLog(string.Format("(1):Report OK"));
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("20:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R21(List<string> StrQ)
        {
            #region 统计条件
            string Deal = string.Empty;
            List<string> TableName = new List<string>();
            string where = string.Empty;
            if (StrQ.Count >= 2)
            {
                for (int i = 2; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "DE")
                    {
                        Deal = str.Substring(2);
                        //where += " AND C019=" + call;
                    }
                }
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR21DataSet(Deal, i);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR21DataSet(Deal, i);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("21:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R22(List<string> StrQ)
        {
            #region 统计条件
            string KW = string.Empty;
            List<string> KWID = new List<string>();
            string where = string.Empty;
            if (StrQ.Count >= 1)
            {
                for (int i = 0; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "KW")
                    {
                        KWID = StrToList(str.Substring(2, str.Length - 2));
                        KW = s6101App.PutTempData(KWID);

                        where += string.Format(" AND C006 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", KW);
                    }
                }
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR22DataSet(where, i);
                    }
                    else
                    {
                        DS_temp = s6101App.GetR22DataSet(where, i);
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("22:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R23(List<string> StrQ)
        {
            #region 统计条件
            string KW = string.Empty;
            List<string> KWID = new List<string>();
            string where = string.Empty;
            if (StrQ.Count >= 1)
            {
                for (int i = 0; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "KW")
                    {
                        KWID = StrToList(str.Substring(2, str.Length - 2));
                        KW = s6101App.PutTempData(KWID);

                        where += string.Format(" AND C006 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", KW);
                    }
                }
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR23DataSet(where, i, "C104");
                    }
                    else
                    {
                        DS_temp = s6101App.GetR23DataSet(where, i, "C104");
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        this.ReportViewer1.LocalReport.ReportEmbeddedResource = null;
                        this.ReportViewer1.LocalReport.ReportPath = S6101App.Path23;
                        this.ReportViewer1.LocalReport.SetParameters(rp);
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("23:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R24(List<string> StrQ)
        {
            #region 统计条件
            string KW = string.Empty;
            List<string> KWID = new List<string>();
            string where = string.Empty;
            if (StrQ.Count >= 1)
            {
                for (int i = 0; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "KW")
                    {
                        KWID = StrToList(str.Substring(2, str.Length - 2));
                        KW = s6101App.PutTempData(KWID);

                        where += string.Format(" AND C006 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", KW);
                    }
                }
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR23DataSet(where, i, "C103");
                    }
                    else
                    {
                        DS_temp = s6101App.GetR23DataSet(where, i, "C103");
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        this.ReportViewer1.LocalReport.ReportEmbeddedResource = null;
                        this.ReportViewer1.LocalReport.ReportPath = S6101App.Path23;
                        this.ReportViewer1.LocalReport.SetParameters(rp);
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("24:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R25(List<string> StrQ)
        {
            #region 统计条件
            string KW = string.Empty;
            List<string> KWID = new List<string>();
            string where = string.Empty;
            if (StrQ.Count >= 1)
            {
                for (int i = 0; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "KW")
                    {
                        KWID = StrToList(str.Substring(2, str.Length - 2));
                        KW = s6101App.PutTempData(KWID);

                        where += string.Format(" AND C006 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", KW);
                    }
                }
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR25DataSet(where, i, "C104");
                    }
                    else
                    {
                        DS_temp = s6101App.GetR25DataSet(where, i, "C104");
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("25:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        private void R26(List<string> StrQ)
        {
            #region 统计条件
            string KW = string.Empty;
            List<string> KWID = new List<string>();
            string where = string.Empty;
            if (StrQ.Count >= 1)
            {
                for (int i = 0; i < StrQ.Count; i++)
                {
                    string str = StrQ[i];
                    if (str.Substring(0, 2) == "KW")
                    {
                        KWID = StrToList(str.Substring(2, str.Length - 2));
                        KW = s6101App.PutTempData(KWID);

                        where += string.Format(" AND C006 IN (SELECT C011 FROM T_00_901 WHERE C001={0})", KW);
                    }
                }
            }
            #endregion
            int stand = 0; DataSet dataSet = new DataSet(); DataSet DS_temp = new DataSet();
            try
            {
                for (int i = 0; i < S6101App.ADT.BeginDateTime.Count; i++)
                {
                    if (i == stand)
                    {
                        dataSet = s6101App.GetR25DataSet(where, i, "C103");
                    }
                    else
                    {
                        DS_temp = s6101App.GetR25DataSet(where, i, "C103");
                        if (DS_temp.Tables.Count != 0)
                        {
                            foreach (DataRow dr in DS_temp.Tables[0].Rows)
                                dataSet.Tables[0].ImportRow(dr);
                        }
                    }
                    if (dataSet.Tables.Count != 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("DataSet1", dataSet.Tables[0]);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        this.ReportViewer1.RefreshReport();
                    }
                    else
                        stand++;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(string.Format("26:{0}\n{1}", ex.Message, ex.ToString()));
            }
            if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("6101N0001", "No Data"));
                ReportResult = false;
            }
        }

        #endregion

        #region 控制窗体中的按钮
        private void OperationLogReport_RenderingComplete(string leadout)
        {
            try
            {
                foreach (RenderingExtension extension in ReportViewer1.LocalReport.ListRenderingExtensions())
                {
                    if (extension.Name.ToUpper().Contains(leadout.ToUpper()))
                    {
                        System.Reflection.FieldInfo LocalFildInfo = extension.GetType().GetField("m_isVisible", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                        LocalFildInfo.SetValue(extension, false);
                    }
                }
            }
            catch { }
        }
        #endregion

        #region Tool
        public static List<string> DateSec(string st, string et)
        {
            string Sy = st.Substring(0, 4); string Sm = st.Substring(5, 2); string Sd = st.Substring(8, 2);
            string Ey = et.Substring(0, 4); string Em = et.Substring(5, 2); string Ed = et.Substring(8, 2); List<string> TableName = new List<string>();
            if (Sy != Ey)
            {
                int Dy = int.Parse(Ey) - int.Parse(Sy);
                for (int i = int.Parse(Sm); i < 13; i++)
                {
                    string mon = i.ToString();
                    if (mon.Length == 1)
                        mon = "0" + mon;
                    TableName.Add(Sy.Substring(2, 2) + mon);
                }
                for (int j = 1; j < Dy; j++)
                {
                    for (int k = 1; k < 13; k++)
                    {
                        string mon = k.ToString();
                        if (mon.Length == 1)
                            mon = "0" + mon;
                        TableName.Add((int.Parse(Sy.Substring(2, 2)) + j).ToString() + mon);
                    }
                }
                for (int l = 1; l <= int.Parse(Em); l++)
                {
                    string mon = l.ToString();
                    if (mon.Length == 1)
                        mon = "0" + mon;
                    TableName.Add(Ey.Substring(2, 2) + mon);
                }
            }
            else
            {
                if (Sm == Em)
                    TableName.Add(Sy.Substring(2, 2) + Sm);
                else
                {
                    for (int i = int.Parse(Sm); i <= int.Parse(Em); i++)
                    {
                        string mon = i.ToString();
                        if (mon.Length == 1)
                            mon = "0" + mon;
                        TableName.Add(Sy.Substring(2, 2) + mon);
                    }
                }
            }
            return TableName;
        }

        public static List<string> StrToList(string Where)
        {
            int pre = 0; int pos = 0; List<string> LS = new List<string>();
            for (int i = 0; i < Where.Length; i++)
            {
                if (Where[i] == ';')
                {
                    pos = i;
                    LS.Add(Where.Substring(pre, pos - pre));
                    pre = pos + 1;
                }
            }
            return LS;
        }

        public static string ListToSql(List<string> list, string col, int mark)
        {
            string sql = " (";
            for (int i = 0; i < list.Count; i++)
            {
                if (mark == 1)
                    sql += col + "='" + list[i] + "' OR ";
                else
                    sql += col + "=" + list[i] + " OR ";
            }
            return sql.Substring(0, sql.Length - 4) + ")";
        }

        private void InitOperation()
        {
            ListReport.Clear();
            for (int i = 1; i <= 9; i++)
            {
                ReportNameTree RNT = new ReportNameTree();
                RNT.Type = "1";
                RNT.ReportID = i.ToString();
                RNT.ParentID = "0";
                switch (i)
                {
                    case 4:
                        RNT.ReportID = "11";
                        RNT.ParentID = "1";
                        break;
                    case 5:
                        RNT.ReportID = "12";
                        RNT.ParentID = "1";
                        break;
                    case 6:
                        RNT.ReportID = "13";
                        RNT.ParentID = "1";
                        break;
                    case 7:
                        RNT.ReportID = "22";
                        RNT.ParentID = "2";
                        break;
                    case 8:
                        RNT.ReportID = "23";
                        RNT.ParentID = "2";
                        break;
                    case 9:
                        RNT.ReportID = "24";
                        RNT.ParentID = "2";
                        break;
                }
                ListReport.Add(RNT);
            }
            InitOperations();
            ControlOperations(mRoot);
        }

        private void InitOperations()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("61");
                string parentID = "6101001";
                webRequest.ListData.Add(parentID);
                CurrentApp.WriteLog(CurrentApp.Session.DBConnectionString + "   [" + CurrentApp.Session.DatabaseInfo.RealPassword);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitOperations Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitOperations Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo optInfo = optReturn.Data as OperationInfo;
                    if (optInfo != null)
                    {
                        string PermissionIDTemp = optInfo.ID.ToString();
                        if (PermissionIDTemp.Length > 7)// && PermissionIDTemp != "61010016")
                        {
                            if (PermissionIDTemp.Substring(0, 4) == "6101")
                            {
                                ReportNameTree rnt = new ReportNameTree();
                                rnt.Type = "2";
                                rnt.ReportID = PermissionIDTemp;
                                int code = Convert.ToInt32(PermissionIDTemp.Substring(PermissionIDTemp.Count() - 2, 2));
                                switch (code)
                                {
                                    case 1:
                                    case 18:
                                    case 20:
                                    case 23:
                                    case 25:
                                        rnt.ParentID = "11";
                                        break;
                                    case 3:
                                    case 21:
                                        rnt.ParentID = "3";
                                        break;
                                    case 2:
                                    case 4:
                                    case 5:
                                    case 6:
                                    case 10:
                                    case 16:
                                    case 19:
                                    case 24:
                                    case 26:
                                    case 27:
                                    case 32:
                                    case 33:
                                        rnt.ParentID = "12";
                                        break;
                                    case 7:
                                    case 9:
                                    case 13:
                                        rnt.ParentID = "13";
                                        break;
                                    case 8:
                                    case 11:
                                    case 17:
                                    case 28:
                                    case 29:
                                    case 30:
                                    case 31:
                                        rnt.ParentID = "22";
                                        break;
                                    case 12:
                                    case 14:
                                    case 15:
                                        rnt.ParentID = "23";
                                        break;
                                    case 22:
                                        rnt.ParentID = "24";
                                        break;
                                }
                                ListReport.Add(rnt);
                            }
                        }
                    }
                }
                CreatTree(mRoot, "0");
                //ListReportNum.Add("61010019");
                //ListReport.Add(CurrentApp.GetLanguageInfo(string.Format("FO{0}", "61010019"), "61010019"));
                CurrentApp.WriteLog("PageInit", string.Format("InitOperations"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ControlOperations(ObjectItem ObjItem)
        {
            if (ObjItem != null)
            {
                if (ObjItem.Children.Count == 0)
                {
                    ObjectItem ParentObj = ObjItem.Parent as ObjectItem;
                    if (ParentObj != null)
                    {
                        ParentObj.RemoveChild(ObjItem);
                    }
                }
                else
                {
                    int childNum = ObjItem.Children.Count;
                    for (int i = childNum - 1; i >= 0; i--)
                    {
                        ObjectItem childObj = ObjItem.Children[i] as ObjectItem;
                        if (childObj.ObjType != 2)
                        {
                            ControlOperations(childObj);
                        }
                    }
                    if (childNum != ObjItem.Children.Count)
                        ControlOperations(ObjItem);
                }
            }
        }

        private void CreatTree(ObjectItem ObjItem, string code)
        {
            List<ReportNameTree> TempList = ListReport.Where(p => p.ParentID == code).ToList();
            if (TempList != null && TempList.Count != 0)
            {
                foreach (ReportNameTree rnt in TempList)
                {
                    ObjectItem obj = new ObjectItem();
                    obj.ObjID = Convert.ToInt64(rnt.ReportID);
                    obj.ObjType = Convert.ToInt32(rnt.Type);
                    if (obj.ObjType == 2)
                    {
                        obj.Name = CurrentApp.GetLanguageInfo(string.Format("FO{0}", rnt.ReportID), rnt.ReportID);
                        obj.State = Convert.ToInt32(rnt.ReportID.Substring(rnt.ReportID.Count() - 2, 2));
                    }
                    else
                    {
                        obj.Name = CurrentApp.GetLanguageInfo(string.Format("6101Tree{0}", rnt.ReportID), rnt.ReportID);
                        obj.State = 0;
                    }
                    obj.Description = obj.Name;
                    obj.Parent = ObjItem;
                    ObjItem.Children.Add(obj);
                    CreatTree(obj, rnt.ReportID);
                }
            }
        }

        public void MarkToDB()
        {
            try
            {
                if (S6101App.ListMarkInfo == null || S6101App.DataSetMark == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)WebCodes.MarkToDB;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData = S6101App.ListMarkInfo;
                webRequest.DataSetData = S6101App.DataSetMark;
                Service61012Client client = new Service61012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service61012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPReportOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitOperations Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                S6101App.DataSetMark.Clear(); S6101App.ListMarkInfo.Clear();
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage("MarkToDB_" + ex.Message); return;
            }
        }

        #endregion
    }
}