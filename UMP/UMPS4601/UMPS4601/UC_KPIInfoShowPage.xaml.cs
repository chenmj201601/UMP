using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
using UMPS4601.Models;
using UMPS4601.Wcf11012;
using UMPS4601.Wcf46011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common4601;
using VoiceCyber.UMP.Communications;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS4601
{
    /// <summary>
    /// UC_KPIInfoShowPage.xaml 的交互逻辑
    /// </summary>
    public partial class UC_KPIInfoShowPage
    {
        #region Members
        public ObservableCollection<KpiInfoItem> mListKpiInfoItems;
        private KpiInfoItem mCurrentKpiInfoItem;
        private BackgroundWorker mWorker;
        private ObservableCollection<ViewColumnInfo> mListKPIDetailColumns;
        public static ObservableCollection<OperationInfo> ListOperations = new ObservableCollection<OperationInfo>();

        public PMMainView ParentPage = null;
        #endregion

        public UC_KPIInfoShowPage()
        {
            InitializeComponent();
            mListKpiInfoItems = new ObservableCollection<KpiInfoItem>();
            mListKPIDetailColumns = new ObservableCollection<ViewColumnInfo>();
            Loaded += UC_KPIInfoShowPage_Loaded;
            LvRecordScoreDetail.SelectionChanged += LvRecordScoreDetail_SelectionChanged;
        }



        private void UC_KPIInfoShowPage_Loaded(object sender, RoutedEventArgs e)
        {
            LvRecordScoreDetail.ItemsSource = mListKpiInfoItems;
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                InitKPIDetailColumns();
                InitOperation();
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                CreateOptButtons();
                CreateKPIDetailColumns();
                LoadKpiDetailInfo();

                ChangeLanguage();
            };
            mWorker.RunWorkerAsync();
        }

        //加载所有的KPI详细信息
        private void LoadKpiDetailInfo()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S4601Codes.GetAllKPIInfoLists;
                webRequest.Session = CurrentApp.Session;
                Service46011Client client = new Service46011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46011"));
                //Service46011Client client = new Service46011Client();
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                mListKpiInfoItems.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<KpiInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    KpiInfo item = optReturn.Data as KpiInfo;
                    if (item != null)
                    {
                        KpiInfoItem temp = new KpiInfoItem(item);
                        temp.StrActive = CurrentApp.GetLanguageInfo(string.Format("4601IsActive{0}", temp.Active), temp.Active);
                        temp.StrDescription = CurrentApp.GetLanguageInfo(string.Format("4601KPI{0}", temp.KpiInfo.KpiID), temp.Description);
                        if (Regex.Matches(temp.UseType, @"1").Count > 0)
                        {
                            temp.StrUseType = string.Empty;
                            if (temp.UseType.Substring(0, 1) == "1")
                            {
                                temp.StrUseType += CurrentApp.GetLanguageInfo(string.Format("4601BPOBJ{0}", "1000000000"), "1000000000") + ",";
                            }
                            if (temp.UseType.Substring(1, 1) == "1")
                            {
                                temp.StrUseType += CurrentApp.GetLanguageInfo(string.Format("4601BPOBJ{0}", "0100000000"), "0100000000") + ",";
                            }
                            if (temp.UseType.Substring(2, 1) == "1")
                            {
                                temp.StrUseType += CurrentApp.GetLanguageInfo(string.Format("4601BPOBJ{0}", "0010000000"), "0010000000") + ",";
                            }
                            if (temp.UseType.Substring(3, 1) == "1")
                            {
                                temp.StrUseType += CurrentApp.GetLanguageInfo(string.Format("4601BPOBJ{0}", "0001000000"), "0001000000") + ",";
                            }
                            if (temp.UseType.Substring(4, 1) == "1")
                            {
                                temp.StrUseType += CurrentApp.GetLanguageInfo(string.Format("4601BPOBJ{0}", "0000100000"), "0001000000") + ",";
                            }
                            if (temp.UseType.Substring(5, 1) == "1")
                            {
                                temp.StrUseType += CurrentApp.GetLanguageInfo(string.Format("4601BPOBJ{0}", "0000010000"), "0001000000") + " ";
                            }
                        }
                        if (Regex.Matches(temp.ApplyCycle, @"1").Count > 0)
                        {
                            temp.StrApplyCycle = string.Empty;
                            if (temp.ApplyCycle.Substring(0, 1) == "1")
                            {
                                temp.StrApplyCycle += CurrentApp.GetLanguageInfo(string.Format("4601BP{0}", "1000000001"), "1000000001") + ",";
                            }
                            if (temp.ApplyCycle.Substring(1, 1) == "1")
                            {
                                temp.StrApplyCycle += CurrentApp.GetLanguageInfo(string.Format("4601BP{0}", "0100000001"), "0100000001") + ",";
                            }
                            if (temp.ApplyCycle.Substring(2, 1) == "1")
                            {
                                temp.StrApplyCycle += CurrentApp.GetLanguageInfo(string.Format("4601BP{0}", "0010000001"), "0010000001") + ",";
                            }
                            if (temp.ApplyCycle.Substring(3, 1) == "1")
                            {
                                temp.StrApplyCycle += CurrentApp.GetLanguageInfo(string.Format("4601BP{0}", "0001000001"), "0001000001") + ",";
                            }
                            if (temp.ApplyCycle.Substring(4, 1) == "1")
                            {
                                temp.StrApplyCycle += CurrentApp.GetLanguageInfo(string.Format("4601BP{0}", "0000100001"), "0000100001") + ",";
                            }
                            if (temp.ApplyCycle.Substring(5, 1) == "1")
                            {
                                temp.StrApplyCycle += CurrentApp.GetLanguageInfo(string.Format("4601BP{0}", "0000010001"), "0000010001") + ",";
                            }
                            if (temp.ApplyCycle.Substring(6, 1) == "1")
                            {
                                temp.StrApplyCycle += CurrentApp.GetLanguageInfo(string.Format("4601BP{0}", "0000001001"), "0000001001") + ",";
                            }
                            if (temp.ApplyCycle.Substring(7, 1) == "1")
                            {
                                temp.StrApplyCycle += CurrentApp.GetLanguageInfo(string.Format("4601BP{0}", "0000000101"), "0000000101") + ",";
                            }
                            if (temp.ApplyCycle.Substring(8, 1) == "1")
                            {
                                temp.StrApplyCycle += CurrentApp.GetLanguageInfo(string.Format("4601BP{0}", "0000000011"), "0000000011") + " ";
                            }
                        }
                        mListKpiInfoItems.Add(temp);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        private void LvRecordScoreDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KpiInfoItem item = LvRecordScoreDetail.SelectedItem as KpiInfoItem;
            if (item != null)
            {
                mCurrentKpiInfoItem = item;
                LvRecordScoreDetail.SelectedItem = mCurrentKpiInfoItem;
            }
        }

        #region 加载列和操作
        private void InitKPIDetailColumns()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("4603001");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                    if (columnInfo != null)
                    {
                        columnInfo.Display = columnInfo.ColumnName;
                        listColumns.Add(columnInfo);
                    }
                }
                listColumns = listColumns.OrderBy(c => c.SortID).ToList();
                mListKPIDetailColumns.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListKPIDetailColumns.Add(listColumns[i]);
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Load KPIDetailColumns"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateKPIDetailColumns()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListKPIDetailColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListKPIDetailColumns[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = columnInfo.Display;
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL4603001{0}", columnInfo.ColumnName),
                            columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL4603001{0}", columnInfo.ColumnName),
                            columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;
                        //下面这句话要屏蔽掉,因为这里开始就重新绑定了名字之后 那么后面的就不会走进去了
                        //gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                        
                        DataTemplate dt = null;
                        if (columnInfo.ColumnName == "StateType")
                        {
                            dt = Resources["CellStateTypeTemplate"] as DataTemplate;
                        }
                        if (dt != null)
                        {
                            gvc.CellTemplate = dt;
                        }
                        else
                        {
                            //但是如果没有走到资源里 那么 这里还是要写绑定的
                            gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                        }
                        if (columnInfo.ColumnName == "Active")
                        {
                            gvc.DisplayMemberBinding = new Binding("StrActive");
                        }
                        if (columnInfo.ColumnName == "Description")
                        {
                            gvc.DisplayMemberBinding = new Binding("StrDescription");
                        }
                        if (columnInfo.ColumnName == "UseType")
                        {
                            gvc.DisplayMemberBinding = new Binding("StrUseType");
                        }
                        if (columnInfo.ColumnName == "ApplyCycle")
                        {
                            gvc.DisplayMemberBinding = new Binding("StrApplyCycle");
                        }
                        //gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                        gv.Columns.Add(gvc);
                    }
                }
                LvRecordScoreDetail.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitOperation()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("46");
                webRequest.ListData.Add("4603");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                ListOperations.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo optInfo = optReturn.Data as OperationInfo;
                    if (optInfo != null)
                    {
                        optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                        optInfo.Description = optInfo.Display;
                        ListOperations.Add(optInfo);
                    }
                }
                CurrentApp.WriteLog("PageLoad", string.Format("Load Operation"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateOptButtons()
        {
            PanelBasicOpts.Children.Clear();
            OperationInfo item;
            Button btn;
            for (int i = 0; i < ListOperations.Count; i++)
            {
                item = ListOperations[i];
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);
            }
        }

        private void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as Button;
                if (btn != null)
                {
                    var optItem = btn.DataContext as OperationInfo;
                    if (optItem == null)
                    {
                        return;
                    }
                    switch (optItem.ID)
                    {
                        case S4601Consts.OPT_ALTERSTATE:
                            if (mCurrentKpiInfoItem == null)
                            {
                                ShowInformation("请选择一条记录！");
                                return;
                            }
                            //添加变更状态的方法
                            AlterState();
                            break;
                        case S4601Consts.OPT_ALTERVALUE:
                            if (mCurrentKpiInfoItem == null)
                            {
                                ShowInformation("请选择一条记录！");
                                return;
                            }
                            ModifyValue();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }
        #endregion

        private void AlterState()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S4601Codes.AlterState;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(mCurrentKpiInfoItem.KpiInfo.KpiID);//对象ID
                webRequest.ListData.Add(mCurrentKpiInfoItem.Active);
                Service46011Client client = new Service46011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46011"));
                //Service46011Client client = new Service46011Client();
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                else
                {
                    //变更成功之后要刷新下
                    //LoadKpiDetailInfo();
                    if (mCurrentKpiInfoItem.Active == "0")
                    {
                        mCurrentKpiInfoItem.Active = "1";
                        mCurrentKpiInfoItem.StrActive = CurrentApp.GetLanguageInfo(string.Format("4601IsActive{0}", mCurrentKpiInfoItem.Active), mCurrentKpiInfoItem.Active);
                    }
                    else
                    {
                        mCurrentKpiInfoItem.Active = "0";
                        mCurrentKpiInfoItem.StrActive = CurrentApp.GetLanguageInfo(string.Format("4601IsActive{0}", mCurrentKpiInfoItem.Active), mCurrentKpiInfoItem.Active);
                    }
                    //for (int i = 0; i < mListKpiInfoItems.Count; i++)
                    //{
                    //    if (mListKpiInfoItems[i].KpiInfo.KpiID == mCurrentKpiInfoItem.KpiInfo.KpiID)
                    //    {
                    //        mListKpiInfoItems[i].Active = mCurrentKpiInfoItem.Active;
                    //        mListKpiInfoItems[i].StrActive = mCurrentKpiInfoItem.StrActive;
                    //    }
                    //}
                    //MessageBox.Show("SUCCESS");
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }

        private void ModifyValue()
        {
            try 
            {
                PopupPanel.Title = "ModifyValue Page";
                ModifyKpiDetail modifyKpiDetail = new ModifyKpiDetail();
                modifyKpiDetail.CurrentApp = CurrentApp;
                modifyKpiDetail.ParentPage = this;
                modifyKpiDetail.CurrentSelectItem = mCurrentKpiInfoItem;
                PopupPanel.Content = modifyKpiDetail;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region ChangeLanguage
        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();
                //Operation
                for (int i = 0; i < ListOperations.Count; i++)
                {
                    ListOperations[i].Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", ListOperations[i].ID), ListOperations[i].ID.ToString());
                }
                CreateOptButtons();

                //Columns
                CreateKPIDetailColumns();

                //记录列(ListView)中的内容
                LoadKpiDetailInfo();


                PopupPanel.ChangeLanguage();
            }
            catch (Exception ex)
            {
 
            }
        }
        #endregion


    }
}
