using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoiceCyber.UMP.Common;
using UMPS1600;
using Common1600;

namespace UMPS1600Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //未读消息数 在聊天窗口被打开时清零
        private static int UnReadMsgCount = 0;
        IMMainPage main = null;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            //窗口关闭时退出登录
            main.LogOff();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> lst = new List<string>();
            main = new IMMainPage(App.Session, lst);
            main.StatusChangeEvent += main_StatusChangeEvent;
            grd.Children.Add(main);
            main.Login();
        }

        /// <summary>
        /// 处理用户状态改变和新消息传入的事件
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="strMsg"></param>
        void main_StatusChangeEvent(int msgType, string strMsg)
        {
            switch (msgType)
            {
                case (int)UserStatusChangeType.Login:
                    img.Source = new BitmapImage(new Uri("Images\\comment.png", UriKind.Relative));
                    break;
                case (int)UserStatusChangeType.LogOff:
                    MessageBox.Show(msgType.ToString());
                    break;
                case (int)UserStatusChangeType.NewMsg:
                    UnReadMsgCount += 1;
                    break;
            }
        }

    }
}
