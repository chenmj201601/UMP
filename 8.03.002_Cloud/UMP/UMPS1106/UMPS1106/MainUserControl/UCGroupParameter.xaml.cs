using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

namespace UMPS1106.MainUserControl
{
    /// <summary>
    /// UCGroupParameter.xaml 的交互逻辑
    /// </summary>
    public partial class UCGroupParameter : INotifyPropertyChanged, S1106Interface
    {
        public MainView00000A IPageParent = null;

        public event EventHandler<OperationEventArgs> IOperationEvent;

        public string IStrGroupID = string.Empty;

        public UCGroupParameter()
        {
            InitializeComponent();
            GridGroupTitle.PreviewMouseLeftButtonDown += GridGroupTitle_PreviewMouseLeftButtonDown;
        }

        public void ShowGroupParameters(DataTable ADataTable11001, string AStrGroupID)
        {
            string LStr11001003 = string.Empty;

            try
            {
                IStrGroupID = AStrGroupID;
                StrGroupName = S1106App.GetDisplayCharater("UCGroupParameter", "G" + IStrGroupID);
                DataRow[] LDataRowAllParameters = ADataTable11001.Select("C004 = " + IStrGroupID, "C005 ASC");
                foreach (DataRow LDataRowSingleParameter in LDataRowAllParameters)
                {
                    LStr11001003 = LDataRowSingleParameter["C003"].ToString();
                    if (LStr11001003 == "11020104") { continue; }
                    if (LStr11001003 == "11030103") { continue; }
                    UCSingleParameter LUCSingleParameter = new UCSingleParameter(LDataRowSingleParameter);
                    LUCSingleParameter.CurrentApp = CurrentApp;
                    LUCSingleParameter.IPageParent = this.IPageParent;
                    LUCSingleParameter.IOperationEvent += LUCSingleParameter_IOperationEvent;
                    LUCSingleParameter.ShowParameterSettedInfo();
                    LUCSingleParameter.Margin = new Thickness(0, 1, 0, 1);
                    StackPanelContainsParameters.Children.Add(LUCSingleParameter);
                }
            }
            catch { }
        }

        private void LUCSingleParameter_IOperationEvent(object sender, OperationEventArgs e)
        {
            if (IOperationEvent != null) { IOperationEvent(this, e); }
        }

        private void GridGroupTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (StackPanelContainsParameters.Visibility == System.Windows.Visibility.Collapsed)
                {
                    StackPanelContainsParameters.Visibility = System.Windows.Visibility.Visible;
                    ImageUpDownArrow.Style = (Style)App.Current.Resources["ImageUpDownArrowDownStyle"];
                }
                else
                {
                    StackPanelContainsParameters.Visibility = System.Windows.Visibility.Collapsed;
                    ImageUpDownArrow.Style = (Style)App.Current.Resources["ImageUpDownArrowUpStyle"];
                }
            }
            catch { }
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.DataContext = this;
        }

        #region 属性值变化触发事件
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String StrPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(StrPropertyName));
            }
        }
        #endregion

        #region 属性定义
        private string _StrGroupName;
        public string StrGroupName
        {
            get { return _StrGroupName; }
            set
            {
                _StrGroupName = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrGroupName");
                }
            }
        }
        #endregion

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
                catch (Exception ex)
                {
                    //App.ShowExceptionMessage("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = System.IO.Path.Combine(CurrentApp.Session.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes\Style01", "Style1106.xaml");
                    //string uri = string.Format("/Themes/{0}/{1}",
                    //    "Default"
                    //    , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    //Resources.MergedDictionaries.Add(resource);

                    //Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //App.ShowExceptionMessage("2" + ex.Message);
                }
            }


            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            //try
            //{
            //    string uri = string.Format("/Themes/Default/UMPS1106/StyleDictionary1106.xaml");
            //    ResourceDictionary resource = new ResourceDictionary();
            //    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
            //    Resources.MergedDictionaries.Add(resource);
            //}
            //catch (Exception ex)
            //{
            //    //App.ShowExceptionMessage("3" + ex.Message);
            //}

            //var pageHead = PageHead;
            //if (pageHead != null)
            //{
            //    pageHead.ChangeTheme();
            //    pageHead.InitInfo();
            //}

        }
    }
}
