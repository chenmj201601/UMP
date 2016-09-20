using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.ServiceModel;
using PFShareClassesC;
using System.Data;
using UMP.PF.MAMT.WCF_LanPackOperation;

namespace UMP.PF.MAMT.Classes
{
    public class Common
    {
        public static event ReturnHomePage ReturnHomePage_Click;

        /// <summary>
        /// 创建切换语言、退出系统、返回主页等的菜单
        /// </summary>
        /// <param name="isMainWin">是否是主页 如果是主页窗口 则需要加载返回主页、推出系统等菜单</param>
        /// <returns></returns>
        public static ContextMenu CreateApplicationMenu(bool isMainWin)
        {
            ContextMenu LocalContextMenu = new ContextMenu();
            LocalContextMenu.Opacity = 0.8;

            //创建切换语言菜单项
            MenuItem miChangeLan = new MenuItem();
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(Path.Combine(App.GStrApplicationDirectory, @"Images\00000004.ico"), UriKind.RelativeOrAbsolute));
            miChangeLan.Icon = img;
            miChangeLan.SetResourceReference(MenuItem.HeaderProperty, "ChangeLan");
            LocalContextMenu.Items.Add(miChangeLan);

            //创建语言列表菜单项
            MenuItem languageItem;
            if (App.GLstLanguages.Count > 0)
            {
                for (int i = 0; i < App.GLstLanguages.Count; i++)
                {
                    languageItem = new MenuItem();
                    languageItem.SetResourceReference(MenuItem.HeaderProperty, App.GLstLanguages[i]);
                    languageItem.Tag = App.GLstLanguages[i];
                    languageItem.Click += languageItem_Click;
                    miChangeLan.Items.Add(languageItem);
                }
            }
            MenuItem item;
            if (isMainWin)
            {
                //返回主页
                item= new MenuItem();
                img = new Image();
                img.Source = new BitmapImage(new Uri(Path.Combine(App.GStrApplicationDirectory, @"Images\00000006.ico"), UriKind.RelativeOrAbsolute));
                img.Width = 24;
                img.Height = 24;
                item.Icon = img;
                item.SetResourceReference(MenuItem.HeaderProperty, "ReturnMain");
                item.Click += GoHomeItem_Click;
                LocalContextMenu.Items.Add(item);
            }
            //关于UMP
            item = new MenuItem();
            img = new Image();
            img.Source = new BitmapImage(new Uri(Path.Combine(App.GStrApplicationDirectory, @"Images\00000014.ico"), UriKind.RelativeOrAbsolute));
            img.Width = 24;
            img.Height = 24;
            item.Icon = img;
            item.SetResourceReference(MenuItem.HeaderProperty, "AboutSystem");
            LocalContextMenu.Items.Add(item);
            return LocalContextMenu;
        }

        /// <summary>
        /// 返回主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void GoHomeItem_Click(object sender, RoutedEventArgs e)
        {
            ReturnHomePage_Click();
        }

        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void languageItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            AboutLanguage.ChangeLanguage(item.Tag.ToString());
        }

        /// <summary>
        /// frame加载的页面去掉导航
        /// </summary>
        /// <param name="page"></param>
        public static  void RemoveAllNavigate(Page page)
        {
            while (page.NavigationService.CanGoBack)
            {
                page.NavigationService.RemoveBackEntry();
            }
        }

        public static string CreateDBToolTip(string strServer, string strDBType, int iStatus)
        {
            string strReturn = string.Empty;

            return strReturn;
        }

        #region 创建basichttpbinding的连接参数
        public static EndpointAddress CreateEndPoint(string strProtocol, string strHost, string strPort, string strDirName, string wcfName)
        {
            string strUrl = string.Empty;
            EndpointAddress adress = null;
            try
            {
                strUrl = strProtocol + "://" + strHost + ":" + strPort + "/" + strDirName + "/" + wcfName + ".svc";
                adress = new EndpointAddress(new Uri(strUrl, UriKind.Absolute));
            }
            catch (Exception ex)
            {
                adress = null;
            }
            return adress;
        }

        public static BasicHttpBinding CreateBasicHttpBinding(int iSeconds)
        {
            int iMinutes, iSecs;
            BasicHttpBinding binding = null;
            try
            {
                TimeSpan timeSpan;
                if (iSeconds == 0)
                {
                    timeSpan = new TimeSpan(0, 10, 0);
                }
                else
                {
                    timeSpan = new TimeSpan(iSeconds * 10000000);
                }
                iMinutes = timeSpan.Minutes;
                iSecs = timeSpan.Seconds;
                binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.MaxBufferSize = int.MaxValue;
                binding.SendTimeout = new TimeSpan(0, iMinutes, iSecs);
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
            }
            catch (Exception ex)
            {
                binding = null;
            }
            return binding;
        }
        #endregion

        /// <summary>
        /// 生成加、解密验证码
        /// </summary>
        /// <param name="AKeyIVID"></param>
        /// <returns></returns>
        public static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
        {
            string LStrReturn = string.Empty;
            int LIntRand = 0;
            string LStrTemp = string.Empty;

            try
            {
                Random LRandom = new Random();
                LStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = LRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "VCT");
                LIntRand = LRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "UMP");
                LIntRand = LRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, ((int)AKeyIVID).ToString("000"));

                LStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + LStrReturn);
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        /// <summary>
        /// 刷新语言包listview
        /// </summary>
        /// <param name="lvData"></param>
        public static void RefershData(ListView lvData)
        {
            ReturnResult result = AboutLanguagesInServer.WCFOperationMthodA("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port, 2, App.GCurrentDBServer);
            if (result.BoolReturn)
            {
                if (result.DataSetReturn.Tables.Count > 0)
                {
                    if (result.DataSetReturn.Tables.Count > 0)
                    {
                        lvData.ItemsSource = null;
                        lvData.ItemsSource = ConvertDataTableToLanInfoList(result.DataSetReturn.Tables[0]);
                    }
                }
            }
        }

        /// <summary>
        /// 讲datatable转成 List<LanguageInfo>  用于绑定数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<LanguageInfo> ConvertDataTableToLanInfoList(DataTable dt)
        {
            List<LanguageInfo> lstViewrSource = new List<LanguageInfo>();
            LanguageInfo lanInfo;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                lanInfo = new LanguageInfo();
                lanInfo.LanguageCode = dt.Rows[i]["C001"].ToString();
                lanInfo.MessageID = dt.Rows[i]["C002"].ToString();
                lanInfo.DisplayMessage = dt.Rows[i]["DISPLAYMESSAGE"].ToString();
                lanInfo.TipMessage = dt.Rows[i]["TIPMESSAGE"].ToString();
                lanInfo.ModuleID = dt.Rows[i]["C009"].ToString();
                lanInfo.ChildModuleID = dt.Rows[i]["C010"].ToString();
                lanInfo.InFrame = dt.Rows[i]["C011"].ToString();
                lanInfo.InObject = dt.Rows[i]["C012"].ToString();
                lstViewrSource.Add(lanInfo);
            }
            return lstViewrSource;
        }
       
    }
    public delegate void ReturnHomePage();
}
