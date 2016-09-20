using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using UMPS3101.Converters;
using UMPS3101.Models;
using UMPS3101.Wcf11012;
using UMPS3101.Wcf31011;
using VoiceCyber.Common;
using VoiceCyber.SharpZips.Zip;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31011;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.ScoreSheets;

namespace UMPS3101
{
    /// <summary>
    /// ExportScoreSheet.xaml 的交互逻辑
    /// </summary>
    public partial class ExportScoreSheet 
    {
        public SSMMainView PageParent;
        public ScoreSheetItem ScoreSheetItem;

        private ScoreSheet mExportScoreSheet;

        BasicScoreSheetInfo ScoreSheetItemInfo;


        public ExportScoreSheet()
        {
            InitializeComponent();
            ScoreSheetItemInfo = new BasicScoreSheetInfo();
            mExportScoreSheet = new ScoreSheet();

            BtnApply.Click += BtnApply_Click;
            BtnClose.Click += BtnClose_Click;
            BtnBrowser.Click += BtnBrowser_Click;

            Loaded += UCExportRecordOption_Loaded;
           
        }

        private void UCExportRecordOption_Loaded(object sender, RoutedEventArgs e)
        {
            LbSaveDir.Text = CurrentApp.GetLanguageInfo("3101N011", "Save Directory");
            BtnApply.Content = CurrentApp.GetLanguageInfo("3101N012", "Apply");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3101N013", "Close");

            BasicScoreSheetInfo temp = PageParent.InitExportScoreSheet(ScoreSheetItem);
            ScoreSheetItemInfo = temp;
            LoadScoreSheetData(ScoreSheetItemInfo);
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                if (ScoreSheetItem != null)
                {

                    string name = string.Format("{0}.xml", ScoreSheetItemInfo.Name.ToString());
                    if (string.IsNullOrWhiteSpace(TxtSaveDir.Text.ToString()) || string.IsNullOrEmpty(TxtSaveDir.Text.ToString()))
                    {
                        ShowException(CurrentApp.GetLanguageInfo("3101N009", "请选择导出路径"));
                        return;
                    }
                    string path = Path.Combine(TxtSaveDir.Text.ToString(), name);
                    OperationReturn optReturn = XMLHelper.SerializeFile(mExportScoreSheet, path);
                    string zipPath = string.Format("{0}.{1}", path, "zip");
                    string message = string.Empty;
                    ZipFile(path, zipPath);
                    var parent = Parent as PopupPanel;
                    if (parent != null)
                    {
                        parent.IsOpen = false;
                    }
                    ShowInformation(CurrentApp.GetLanguageInfo("3101N007","SECCESS"));

                    string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO3101006")), ScoreSheetItemInfo.Name);
                    CurrentApp.WriteOperationLog(S3101Consts.OPT_EXPORTSCORESHEET.ToString(),
                        ConstValue.OPT_RESULT_SUCCESS, msg);
                }
            }
            catch (Exception ex)
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3101N008","FAIL"));
            }
            
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        private void BtnBrowser_Click(object sender, RoutedEventArgs e)
        {
            //====================================在导出录音的地方加了几个提示的语言包=========================
            //=====================================by 汤澈=====================================================
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = string.Format(CurrentApp.GetLanguageInfo("3101N009", "Please select save directory"));
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string dir = dialog.SelectedPath;
                if (!Directory.Exists(dir))
                {
                    ShowException(string.Format(CurrentApp.GetLanguageInfo("3101N010", "Directory not exist")));
                    return;
                }
                TxtSaveDir.Text = dir;
            }
        }

        //private void SaveScoreSheetData(ScoreSheet scoreSheet)
        //{
        //    try
        //    {
        //        OperationReturn optReturn = XMLHelper.SeriallizeObject(scoreSheet);
        //        if (!optReturn.Result)
        //        {
        //            ShowException(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
        //            return;
        //        }
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Code = (int)S3101Codes.SaveScoreSheetInfo;
        //        webRequest.Session = CurrentApp.Session;
        //        webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
        //        webRequest.ListData.Add(optReturn.Data.ToString());
        //        Service31011Client client = new Service31011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
        //            WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31011"));
        //        WebReturn webReturn = client.DoOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return;
        //        }
        //        CurrentApp.WriteLog("SaveScoreSheet", webReturn.Data);

        //        #region 写操作日志

        //        //string strLog = string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001Name"),
        //        //    scoreSheet.Title);
        //        //strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001TotalScore"),
        //        //    scoreSheet.TotalScore);
        //        //strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001ViewClassic"),
        //        // Utils.FormatOptLogString(string.Format("3101Tip002{0}", (int)scoreSheet.ViewClassic)));
        //        //strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("COL3101001ScoreType"),
        //        // Utils.FormatOptLogString(string.Format("3101Tip003{0}", (int)scoreSheet.ScoreType)));
        //        //CurrentApp.WriteOperationLog(CurrentApp.IsModifyScoreSheet ?
        //        //    S3101Consts.OPT_MODIFYSCORESHEET.ToString() : S3101Consts.OPT_CREATESCORESHEET.ToString(),
        //        //    ConstValue.OPT_RESULT_SUCCESS, strLog);

        //        #endregion

        //        //ShowInformation(CurrentApp.GetMessageLanguageInfo("002", "Save ScoreSheet end"));
        //        //mIsChanged = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowException(ex.Message);
        //    }
        //}

        //private long GetSerialID()
        //{
        //    try
        //    {
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Code = (int)RequestCode.WSGetSerialID;
        //        webRequest.Session = CurrentApp.Session;
        //        webRequest.ListData.Add("31");
        //        webRequest.ListData.Add("301");
        //        webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
        //        Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
        //            WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
        //        WebReturn webReturn = client.DoOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return -1;
        //        }
        //        long id = Convert.ToInt64(webReturn.Data);
        //        return id;
        //    }
        //    catch (Exception ex)
        //    {
        //        App.ShowExceptionMessage(ex.Message);
        //        return -1;
        //    }
        //}


        private void LoadScoreSheetData(BasicScoreSheetInfo scoreSheetInfo)
        {
            try
            {
                if (scoreSheetInfo != null)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Code = (int)S3101Codes.GetScoreSheetInfo;
                    webRequest.Session = CurrentApp.Session;
                    webRequest.ListData.Add(scoreSheetInfo.ID.ToString());
                    Service31011Client client = new Service31011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31011"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ScoreSheet>(webReturn.Data);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ScoreSheet scoreSheet = optReturn.Data as ScoreSheet;
                    if (scoreSheet == null)
                    {
                        ShowException(string.Format("ScoreSheet is null"));
                        return;
                    }
                    scoreSheet.ScoreSheet = scoreSheet;
                    scoreSheet.Init();
                    mExportScoreSheet = scoreSheet;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }






        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="inFile">用于压缩的文件</param>
        /// <param name="outZipFile">目标压缩包文件</param>
        public static void ZipFile(string inFile, string outZipFile)
        {
            if (!string.IsNullOrEmpty(outZipFile))  //压缩包名称不为空
            {
                if (File.Exists(outZipFile)) File.Delete(outZipFile);//删除已有文件
                using (ZipOutputStream u = new ZipOutputStream(File.Create(outZipFile)))//创建压缩流
                {
                    using (FileStream f = File.OpenRead(inFile))
                    {
                        byte[] b = new byte[f.Length];
                        f.Read(b, 0, b.Length);          //将文件流加入缓冲字节中
                        ZipEntry z = new ZipEntry(new FileInfo(inFile).Name);
                        u.PutNextEntry(z);             //为压缩文件流提供一个容器
                        u.Write(b, 0, b.Length);  //写入字节
                        f.Close();
                    }
                    u.Finish(); // 结束压缩
                    u.Close();
                }
                File.Delete(inFile);
            }
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="infile">压缩包</param>
        /// <param name="outFilePath">解压地址</param>
        /// 返回值是   解压后的文件的完整位置
        public static string UnZipFile(string infile, string outFilePath)   //解压缩
        {
            string mFileName = string.Empty;//这个是用来表示解压后的文件的完整路径
            if (!string.IsNullOrEmpty(infile))
            {
                if (!Directory.Exists(outFilePath)) Directory.CreateDirectory(outFilePath);
                using (var inFileStream = new FileStream(infile, FileMode.Open, FileAccess.Read, FileShare.Read))//创建读文件流
                {
                    using (var zipInputfs = new ZipInputStream(inFileStream))//创建zip读文件流
                    {
                        while (true)
                        {
                            var zp = zipInputfs.GetNextEntry(); //获取下一个对象
                            if (zp == null) break;
                            if (!zp.IsDirectory && zp.Crc != 00000000L)//验证是不是为目录
                            {
                                int i = 2048;
                                byte[] b = new byte[i];
                                using (var outFileStream = File.Create(Path.Combine(outFilePath, zp.Name)))//创建写文件流
                                {
                                    while (true)
                                    {
                                        i = zipInputfs.Read(b, 0, b.Length);//读流
                                        if (i <= 0) break;
                                        outFileStream.Write(b, 0, i);//写文件
                                    }
                                    outFileStream.Close();
                                    mFileName = Path.Combine(outFilePath, zp.Name);
                                }
                            }
                            else
                            {
                                Directory.CreateDirectory(Path.Combine(outFilePath, zp.Name));//创建目录 
                                mFileName = Path.Combine(outFilePath, zp.Name);
                            }
                        }
                        zipInputfs.Close();
                    }
                    inFileStream.Close();
                }
            }
            return mFileName;
        }


    }
}
