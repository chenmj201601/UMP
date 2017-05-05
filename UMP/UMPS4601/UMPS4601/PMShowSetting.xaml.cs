using Common4602;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UMPS4601.Models;
using UMPS4601.Wcf11012;
using UMPS4601.Wcf46012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4601
{
    /// <summary>
    /// PMShowSetting.xaml 的交互逻辑
    /// </summary>
    public partial class PMShowSetting
    {
        public ProductManagementShow PageParent;

        private ObservableCollection<ViewColumnInfoItem> mListCustomColumns;
        private List<ViewColumnInfoItem> mAllColumnsList;
        private ViewColumnInfoItem mCurrentViewColumnItem;
        private List<ViewColumnInfo> mListObject_Kpi;

        public PMShowSetting()
        {
            InitializeComponent();
            mListCustomColumns = new ObservableCollection<ViewColumnInfoItem>();
            mAllColumnsList = new List<ViewColumnInfoItem>();
            mListObject_Kpi = new List<ViewColumnInfo>();
            lvSetting.SelectionChanged += lvSetting_SelectionChanged;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            ColumnBtnUp.Click += ColumnBtnUp_Click;
            ColumnBtnDown.Click += ColumnBtnDown_Click;
            ColumnBtnVisiblity.Click += ColumnBtnVisiblity_Click;
            this.Loaded += PMShowSetting_Loaded;
            lvSetting.ItemsSource = mListCustomColumns;
        }

        
        void PMShowSetting_Loaded(object sender, RoutedEventArgs e)
        {
            CreatcColumns();
            LoadCustomColumnData();
            ChangeLanguage();
        }

        private void CreatcColumns()
        {
            try
            {
                string[] lens = "4601P00046,4601P00047, 4601P00041".Split(',');
                string[] cols = "ColumnName,SortID,IsVisible".Split(',');
                int[] colwidths = { 125, 80, 120 };
                GridView ColumnGridView = new GridView();
                GridViewColumn gvc;
                for (int i = 0; i < cols.Length; i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = CurrentApp.GetLanguageInfo(lens[i], cols[i]);
                    gvc.Width = colwidths[i];
                    if (cols[i] == "ColumnName")
                    {
                        gvc.DisplayMemberBinding = new Binding("Display");
                    }
                    else if (cols[i] == "IsVisible")
                    {
                        gvc.DisplayMemberBinding = new Binding("StrIsVisible");
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding(cols[i]);
                    }
                    ColumnGridView.Columns.Add(gvc);
                }
                lvSetting.View = ColumnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        
        private void LoadCustomColumnData()
        {
            try
            {
                mListObject_Kpi.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("4602001");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
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
                if (listColumns.Where(p => p.SortID == 0).First().ColumnName == "UERName")//按对象分组
                {
                    rabObject.IsChecked = true;
                }
                else
                {
                    rabKpi.IsChecked = true;
                }
                mListCustomColumns.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    ViewColumnInfoItem item = new ViewColumnInfoItem(listColumns[i]);
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("4601P00{0}", item.LangID.ToString().Substring(1,3)),item.ColumnName);
                    item.StrIsVisible = CurrentApp.GetLanguageInfo(string.Format("4601P0004{0}", listColumns[i].Visibility), listColumns[i].Visibility);
                    if (i % 2 != 0)
                    {
                        item.Background = Brushes.LightGray;
                    }
                    else
                    {
                        item.Background = Brushes.AntiqueWhite;
                    }
                    if (item.ColumnName == "UERName" || item.ColumnName == "KPIName")//对象、PM指标列不能修改顺序,在提交应用时再加上去
                    {
                        mListObject_Kpi.Add(listColumns[i]);
                    }
                    else
                    {
                        mListCustomColumns.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region 按钮、触发事件

        void ColumnBtnUp_Click(object sender, RoutedEventArgs e)
        {
            var item = lvSetting.SelectedItem as ViewColumnInfoItem;
            if (item != null)
            {
                int index = lvSetting.SelectedIndex;
                if (index > 0)
                {
                    int sortID;
                    var preItem = mListCustomColumns[index - 1];
                    sortID = item.SortID;
                    item.SortID = preItem.SortID;
                    item.ViewColumnInfo.SortID = preItem.SortID;
                    preItem.SortID = sortID;
                    preItem.ViewColumnInfo.SortID = sortID;
                    mListCustomColumns.Remove(item);
                    mListCustomColumns.Insert(index - 1, item);
                    lvSetting.SelectedIndex = index - 1;
                }
            }
        }

        void ColumnBtnDown_Click(object sender, RoutedEventArgs e)
        {
            var item = lvSetting.SelectedItem as ViewColumnInfoItem;
            if (item != null)
            {
                int index = lvSetting.SelectedIndex;
                if (index < mListCustomColumns.Count - 1)
                {
                    int sortID;
                    var nextItem = mListCustomColumns[index + 1];
                    sortID = item.SortID;
                    item.SortID = nextItem.SortID;
                    item.ViewColumnInfo.SortID = nextItem.SortID;
                    nextItem.SortID = sortID;
                    nextItem.ViewColumnInfo.SortID = sortID;
                    mListCustomColumns.Remove(item);
                    mListCustomColumns.Insert(index + 1, item);
                    lvSetting.SelectedIndex = index + 1;
                }
            }
        }
        
        void lvSetting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = lvSetting.SelectedItem as ViewColumnInfoItem;
            if (item != null)
            {
                mCurrentViewColumnItem = item;
            }
        }

        void ColumnBtnVisiblity_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrentViewColumnItem != null)
            {
                mCurrentViewColumnItem.IsVisible = !mCurrentViewColumnItem.IsVisible;
                mCurrentViewColumnItem.ViewColumnInfo.Visibility = mCurrentViewColumnItem.IsVisible== true ? "1" : "0";
                mCurrentViewColumnItem.StrIsVisible = CurrentApp.GetLanguageInfo(string.Format("4601P0004{0}", mCurrentViewColumnItem.ViewColumnInfo.Visibility), mCurrentViewColumnItem.ViewColumnInfo.Visibility);
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mAllColumnsList.Clear();
                if (rabObject.IsChecked.Equals(true))//按对象分组
                {
                    mListObject_Kpi.Where(p => p.ColumnName == "UERName").First().SortID = 0;
                    mListObject_Kpi.Where(p => p.ColumnName == "KPIName").First().SortID = 1;
                }
                else
                {
                    mListObject_Kpi.Where(p => p.ColumnName == "KPIName").First().SortID = 0;
                    mListObject_Kpi.Where(p => p.ColumnName == "UERName").First().SortID = 1;
                }
                foreach (ViewColumnInfo item in mListObject_Kpi)
                {
                    ViewColumnInfoItem viewitem = new ViewColumnInfoItem(item);
                    mAllColumnsList.Add(viewitem);
                }
                foreach (ViewColumnInfoItem item in mListCustomColumns)
                {
                    mAllColumnsList.Add(item);
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code =(int)S4602Codes.SaveViewColumnInfos;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("4602001");
                int intCount = mAllColumnsList.Count;
                webRequest.ListData.Add(intCount.ToString());
                OperationReturn optReturn;
                for (int i = 0; i < intCount; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(mAllColumnsList[i].ViewColumnInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                Service46012Client client = new Service46012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo,"Service46012"));
                WebReturn webReturn = client.UPMSOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }

                #region 写操作日志

                //string strLog = string.Format("{0}", Utils.FormatOptLogString("31021211"));
                //CurrentApp.WriteOperationLog(S3102Consts.OPT_CUSTOMSETTING.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                ShowInformation(CurrentApp.GetLanguageInfo("4601P00045", "Save Sucessed!"));
                PageParent.CreateCharTable();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }
        #endregion

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            try
            {
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("4601P00044", "Apply");
                BtnClose.Content = CurrentApp.GetLanguageInfo("4601P00043", "Close");
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("4601P00048", "Columns Setting");
                }
                labSetting1.Content = CurrentApp.GetLanguageInfo("4601P00042", "How to group when selected an object");
                rabObject.Content = CurrentApp.GetLanguageInfo("4601P00032", "According to objects to group");
                rabKpi.Content = CurrentApp.GetLanguageInfo("4601P00033", "According to Kpi to group");
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

    }
}
