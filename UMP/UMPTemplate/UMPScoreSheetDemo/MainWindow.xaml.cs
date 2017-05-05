using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using Oracle.DataAccess.Client;
using VoiceCyber.Common;
using VoiceCyber.UMP.ScoreSheets;

namespace UMPScoreSheetDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private List<ScoreLangauge> mListLanguageInfos;

        public MainWindow()
        {
            InitializeComponent();

            mListLanguageInfos = new List<ScoreLangauge>();

            Loaded += MainWindow_Loaded;
            BtnTest.Click += BtnTest_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            SynchLanguages();
        }

        private void SynchLanguages()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages.xml");
            if (!File.Exists(path))
            {
                ShowErrorMessage(string.Format("{0}\t{1}", "Not Exist Path", path));
                return;
            }
            try
            {
                ScoreLanguageManager manager;
                OperationReturn optReturn = XMLHelper.DeserializeFile<ScoreLanguageManager>(path);
                if (!optReturn.Result)
                {
                    ShowErrorMessage(string.Format("{0}\t{1}\t{2}", "Load fail", optReturn.Code,
                        optReturn.Message));
                    return;
                }
                manager = optReturn.Data as ScoreLanguageManager;
                if (manager == null)
                {
                    ShowErrorMessage(string.Format("{0}\tScoreLanguageManager is null", "Null"));
                    return;
                }
                mListLanguageInfos.Clear();
                for (int i = 0; i < manager.Languages.Count; i++)
                {
                    mListLanguageInfos.Add(manager.Languages[i]);
                }

                List<ScoreLangauge> listLangs = mListLanguageInfos.Where(l => l.Code.StartsWith("PROD301") && l.LangID == 2052).ToList();
                int count = 0;

                string strConn =
                    string.Format(
                        "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3}; Password={4}",
                        "192.168.4.182",
                        1521,
                        "PFOrcl",
                        "PFDEV",
                        "PF,123");
                string strSql = string.Format("select * from t_00_005 where c009 = 0 and c010 = 0");
                OracleConnection objConn = new OracleConnection(strConn);
                OracleDataAdapter objAdapter = new OracleDataAdapter(strSql, objConn);
                OracleCommandBuilder objCmdBuilder = new OracleCommandBuilder(objAdapter);
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    for (int i = 0; i < listLangs.Count; i++)
                    {
                        ScoreLangauge lang = listLangs[i];
                        DataRow dr =
                            objDataSet.Tables[0].Select(string.Format("C002 = '{0}' and c001 = 2052", string.Format("{0}", lang.Code))).FirstOrDefault();
                        if (dr == null)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = 2052;
                            dr["C002"] = string.Format("{0}", lang.Code);
                            dr["C003"] = 0;
                            dr["C004"] = 0;
                            dr["C005"] = lang.Display;
                            dr["C009"] = 0;
                            dr["C010"] = 0;
                            dr["C011"] = lang.Category;
                            objDataSet.Tables[0].Rows.Add(dr);
                            count++;
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                finally
                {
                    objAdapter.Dispose();
                    objConn.Close();
                }
                ShowErrorMessage(string.Format("End\t{0}", count));
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void ShowErrorMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() => MessageBox.Show(msg, "Demo", MessageBoxButton.OK, MessageBoxImage.Error)));
        }

        private void UpDownBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MessageBox.Show("djfid");
        }
    }
}
