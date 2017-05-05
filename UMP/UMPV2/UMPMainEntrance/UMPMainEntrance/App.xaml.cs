using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using YoungClassesLibrary;

namespace UMPMainEntrance
{
    public partial class App : Application
    {
        #region 程序间相互通讯属性定义
        public static ServiceHost GServiceHost = null;
        public static List<string> GListStrClient = new List<string>();
        #endregion

        #region 全局参数
        public static IntermediateSingleFeature GIntermediateParam = new IntermediateSingleFeature();
        #endregion

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            LoadStyle();
        }

        private void LoadStyle()
        {
            DateTime LDateTimeNow = DateTime.Now;
            string LStryID = string.Empty;

            if (LDateTimeNow.Month == 2 || LDateTimeNow.Month == 3 || LDateTimeNow.Month == 4) { LStryID = "01"; }
            if (LDateTimeNow.Month == 5 || LDateTimeNow.Month == 6 || LDateTimeNow.Month == 7) { LStryID = "02"; }
            if (LDateTimeNow.Month == 8 || LDateTimeNow.Month == 9 || LDateTimeNow.Month == 10) { LStryID = "03"; }
            if (LDateTimeNow.Month == 11 || LDateTimeNow.Month == 12 || LDateTimeNow.Month == 1) { LStryID = "04"; }

            ResourceDictionary LResourceDictionary = new ResourceDictionary();
            LResourceDictionary.Source = new Uri("http://192.168.4.186:8088/Themes/Style" + LStryID + ".xaml", UriKind.Absolute);
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(LResourceDictionary);

            GIntermediateParam.StrUseStyle = "Style" + LStryID;
        }

        #region 加载、设置全局参数
        public static string LoadGloabalParameters()
        {
            string LStrReturn = string.Empty;

            try
            {
                LStrReturn = "00000000";
                SetGlobalParameters();

                GIntermediateParam.StrUseProtol = "HTTP";
                GIntermediateParam.StrServerHost = "192.168.4.186";
                GIntermediateParam.StrServerPort = "8088";

                GIntermediateParam.StrLoginSerialID = "11011406261600000001";
                GIntermediateParam.StrLoginAccount = "Young";
                GIntermediateParam.StrLoginUserName = "Young Yang";

                GIntermediateParam.StrFeatureName = "账户密钥策略";

                
            }
            catch (Exception ex)
            {
                LStrReturn = "Error999" + ex.ToString();
            }
            return LStrReturn;
        }

        public static void SetGlobalParameters()
        {
            //GIntermediateParam.StrLoginSerialID = string.Empty;
            //GIntermediateParam.StrLoginAccount = string.Empty;
            //GIntermediateParam.StrLoginUserName = string.Empty;

            GIntermediateParam.StrFeatureID = string.Empty;
            //GIntermediateParam.StrFeatureName = string.Empty;
            GIntermediateParam.StrFeatureImage = string.Empty;
            GIntermediateParam.StrImageSize = string.Empty;

            //GIntermediateParam.StrUseProtol = "HTTP";
            //GIntermediateParam.StrServerHost = "192.168.4.186";
            //GIntermediateParam.StrServerPort = "8088";

            //GIntermediateParam.StrUseStyle = string.Empty;
            GIntermediateParam.StrOpenXbap = string.Empty;
            GIntermediateParam.StrXbapArgs = string.Empty;
        }
        #endregion
    }
}
