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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace UMPS3104
{
    /// <summary>
    /// WebBrowser.xaml 的交互逻辑
    /// </summary>
    public partial class WebBrowser
    {
        public AgentIntelligentClient parentPage;
        string IPTxt = string.Empty;
        public WebBrowser()
        {
            InitializeComponent();
            GetWeb();
            webBrowser.Navigate(new Uri(string.Format("Http://{0}", IPTxt), UriKind.Absolute));
        }

        private void GetWeb()
        {
            try
            {
                string xmlFileName = Environment.CurrentDirectory + string.Format("\\UMPClientInit.xml");
                XmlDocument umpclientinitxml = new XmlDocument();
                umpclientinitxml.Load(xmlFileName);
                IPTxt = umpclientinitxml.SelectSingleNode("root/WcfConfig/WebAccess").InnerText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
