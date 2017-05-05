using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsUserLogon
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("advapi32.dll")]
        //映射函数LogonUser
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string LStrUserID = TextBoxUserID.Text.Trim();
            string LStrPassword = TextBoxPassword.Text.Trim();

            IntPtr tokenHandle = new IntPtr(0);
            tokenHandle = IntPtr.Zero;

            bool checkok = LogonUser(LStrUserID, System.Environment.MachineName, LStrPassword, 2, 0, ref tokenHandle);
            if (checkok)
            {
                MessageBox.Show("欢迎 " + LStrUserID);
            }
            else
            {
                MessageBox.Show("LZ洗洗睡了");
            }
           
        }

        private void ButtonZipFile_Click(object sender, RoutedEventArgs e)
        {

        }


        private void CreateZipFile(string filesPath, string zipFilePath)
        {
            try
            {
                if (!Directory.Exists(filesPath))
                {
                    MessageBox.Show("文件不存在\n" + filesPath); return;
                }

                string[] filenames = Directory.GetFiles(filesPath);
                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
                {

                    s.SetLevel(9); // 压缩级别 0-9
                    //s.Password = "123"; //Zip压缩文件密码
                    byte[] buffer = new byte[4096]; //缓冲区大小
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(System.IO.Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string LStrIndexIMC = System.IO.Path.Combine(@"D:\UMP.PF.Site", "UMP.PF.html");
                string LStrIndexIAC = System.IO.Path.Combine(@"D:\UMP.PF.Site", "i-AC.html");

                Regex LRegexXbapSimple = new Regex("<a href=\u0022UMPMain.xbap\u0022>");
                Regex LRegexXbapFull = new Regex("<a href=\u0022https://[a-zA-Z0-9_:\\.\\-]+/UMPMain.xbap\u0022>");
                string[] LStrArrayIMC = File.ReadAllLines(LStrIndexIMC, Encoding.UTF8);
                for (int LIntLoopLine = 0; LIntLoopLine < LStrArrayIMC.Length; LIntLoopLine++)
                {
                    if (LIntLoopLine == 376)
                    {
                        if (LRegexXbapFull.IsMatch(LStrArrayIMC[LIntLoopLine]))
                        {
                            LStrArrayIMC[LIntLoopLine] = LRegexXbapFull.Replace(LStrArrayIMC[LIntLoopLine], string.Format("<a href=\"{0}://{1}:{2}/UMPMain.xbap\">", "https", "192.168.2.102", "8082"));
                        }

                        if (LRegexXbapSimple.IsMatch(LStrArrayIMC[LIntLoopLine]))
                        {
                            LStrArrayIMC[LIntLoopLine] = LRegexXbapSimple.Replace(LStrArrayIMC[LIntLoopLine], string.Format("<a href=\"{0}://{1}:{2}/UMPMain.xbap\">", "https", "127.0.0.1", "8082"));
                        }
                    }
                }
                File.WriteAllLines(LStrIndexIMC, LStrArrayIMC);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }

}
