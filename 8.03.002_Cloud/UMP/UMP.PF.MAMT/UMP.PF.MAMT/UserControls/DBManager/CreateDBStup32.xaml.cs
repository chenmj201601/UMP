using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using UMP.PF.MAMT.WCF_LanPackOperation;
using UMP.PF.MAMT.Classes;
using System.Data;

namespace UMP.PF.MAMT.UserControls.DBManager
{
    /// <summary>
    /// CreateDBStup32.xaml 的交互逻辑
    /// </summary>
    public partial class CreateDBStup32 : Window
    {
        BackgroundWorker bgwInitDataWorker = null;  //获得所有数据库对象时用到的线程
        ReturnResult RRAllObjectsResult = null; //获得所有数据库对象时返回的结果

        BackgroundWorker bgwCreateDBObject = null;  //创建数据库对象的线程

        public CreateDBStup32()
        {
            InitializeComponent();
            this.Loaded += CreateDBStup32_Loaded;
        }

        void CreateDBStup32_Loaded(object sender, RoutedEventArgs e)
        {
            App.DrawWindowsBackGround(this, @"Images\00000005.jpg");
            ImageCreateDatabase.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000029.ico"), UriKind.RelativeOrAbsolute));
            ButtonCloseWindowButtom.Click += ButtonCloseWindowButtom_Click;
            ButtonCloseWindowTop.Click += ButtonCloseWindowButtom_Click;
            ButtonNextStep.Click += ButtonNextStep_Click;
            ButtonSkipCreate.Click += ButtonSkipCreate_Click;
            ButtonBackStep.Click += ButtonBackStep_Click;
            bgwInitDataWorker = new BackgroundWorker();
            bgwInitDataWorker.DoWork += bgwInitDataWorker_DoWork;
            bgwInitDataWorker.RunWorkerCompleted += bgwInitDataWorker_RunWorkerCompleted;
            bgwInitDataWorker.RunWorkerAsync();
            
        }

        /// <summary>
        /// 获得需要创建的数据库对象列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bgwInitDataWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> lstArgs = new List<string>();
            lstArgs.Add(App.GCreatingDBInfo.DbType.ToString());
            RRAllObjectsResult = AboutLanguagesInServer.WCFOperationMthodA("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port, 9, lstArgs);
        }

        /// <summary>
        /// 处理Dowork返回的结果 加载到listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bgwInitDataWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!RRAllObjectsResult.BoolReturn)
            {
                string strMsg = string.Format(TryFindResource("Error016").ToString(), RRAllObjectsResult.StringReturn);
                MessageBox.Show(strMsg, TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            InitDatabaseObjectData();
            //CreateDBStup42 stup42 = new CreateDBStup42(RRAllObjectsResult.DataSetReturn.Tables[0]);
            //stup42.Show();
            //this.Close();
        }

        private void InitDatabaseObjectData()
        {
            if (RRAllObjectsResult.DataSetReturn.Tables.Count <= 0)
                return;
            DataTable dt= RRAllObjectsResult.DataSetReturn.Tables[0];
            int iTotalCount = dt.Rows.Count;
            if (iTotalCount > 0)
            {
                ListViewItem item = null;
                for (int i = 0; i < iTotalCount; i++)
                {
                    item = new ListViewItem();
                    item.Content = new DatabaseObject(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), Enums.OperationStatus.Wait, TryFindResource("Message005").ToString());
                    lvObjects.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// 上一步 连接数据库参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonBackStep_Click(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// 跳过 进入初始化数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonSkipCreate_Click(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// 下一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonNextStep_Click(object sender, RoutedEventArgs e)
        {
            int iCount = lvObjects.Items.Count;
            for (int i = 0; i < iCount; i++)
            {
                bgwCreateDBObject = new BackgroundWorker();
                bgwCreateDBObject.DoWork += bgwCreateDBObject_DoWork;
                bgwCreateDBObject.RunWorkerCompleted += bgwCreateDBObject_RunWorkerCompleted;
                bgwCreateDBObject.RunWorkerAsync(lvObjects.Items[i]);
            }
               
        }

        void bgwCreateDBObject_DoWork(object sender, DoWorkEventArgs e)
        {
            ListViewItem item = e.Argument as ListViewItem;

        }

        void bgwCreateDBObject_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonCloseWindowButtom_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
