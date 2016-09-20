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
using UMP.PF.MAMT.Classes;
using System.IO;
using UMP.PF.MAMT.UserControls;
using UMP.PF.MAMT.WCF_ServerConfig;

namespace UMP.PF.MAMT
{
    /// <summary>
    /// MainWin.xaml 的交互逻辑
    /// </summary>
    public partial class MainWin : Window
    {
        Rect rcnormal;//定义一个全局rect记录还原状态下窗口的位置和大小。

        public MainWin()
        {
            InitializeComponent();
            this.Loaded += MainWin_Loaded;
        }

        void MainWin_Loaded(object sender, RoutedEventArgs e)
        {
            Home homeWindow = new Home();
            borContent.Child = homeWindow;
            homeWindow.IntoModel_Click += homeWindow_IntoModel_Click;
            SetWindowSize();
            App.DrawWindowsBackGround(this, @"Images\00000005.jpg");
            ButtonApplicationMenu.Click += ButtonApplicationMenu_Click;
            ButtonCloseConnect.Click += ButtonCloseConnect_Click;
            InitMenu();
        }

        /// <summary>
        /// Home页面的IntoModel_Click事件 由此根据参数不同 进入各个模块 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void homeWindow_IntoModel_Click(object sender, Enums.Model e)
        {
            switch (e)
            {
                case Enums.Model.DBModel:
                    UC_DBManager uc = new UC_DBManager();
                    borContent.Child = uc;
                    break;
                case Enums.Model.IISModel:
                    UC_IISManager ucIIS = new UC_IISManager();
                    borContent.Child = ucIIS;
                    break;
                case Enums.Model.LanguageModel:
                    UC_LanManager ucLan = new UC_LanManager();
                    borContent.Child = ucLan;
                    break;
            }
        }

        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonCloseConnect_Click(object sender, RoutedEventArgs e)
        {
            //断开连接 
            OperationDataArgs result = ServerConfigOperationInServer.UserLogOff();
            this.Close();
        }

        /// <summary>
        /// 打开下拉列表事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonApplicationMenu_Click(object sender, RoutedEventArgs e)
        {
            Button ClickedButton = sender as Button;
            //目标   
            ClickedButton.ContextMenu.PlacementTarget = ClickedButton;
            //位置   
            ClickedButton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            //显示菜单   
            ClickedButton.ContextMenu.IsOpen = true;
        }

        /// <summary>
        /// 加载语言菜单
        /// </summary>
        private void InitMenu()
        {
            ContextMenu LocalContextMenu = Common.CreateApplicationMenu(true);
            ButtonApplicationMenu.ContextMenu = LocalContextMenu;
            Common.ReturnHomePage_Click += Common_ReturnHomePage_Click;
        }

        /// <summary>
        /// 菜单中返回主页按钮的事件
        /// </summary>
        void Common_ReturnHomePage_Click()
        {
            Home homeWindow = new Home();
            borContent.Child = homeWindow;
            homeWindow.IntoModel_Click += homeWindow_IntoModel_Click;
        }

        /// <summary>
        /// 设置窗口大小  在全屏时 露出任务栏
        /// </summary>
        private void SetWindowSize()
        {
            rcnormal = new Rect(this.Left, this.Top, this.Width, this.Height);//保存下当前位置与大小
            this.Left = 0;//设置位置
            this.Top = 0;
            Rect rc = SystemParameters.WorkArea;//获取工作区大小
            this.Width = rc.Width;
            this.Height = rc.Height;
        }
    }
}
