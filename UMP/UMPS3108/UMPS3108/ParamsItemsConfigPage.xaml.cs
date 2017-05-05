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
using UMPS3108.Models;
using UMPS3108.Wcf31081;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common31081;
using VoiceCyber.UMP.Communications;

namespace UMPS3108
{
    /// <summary>
    /// ParamsItemsConfigPage.xaml 的交互逻辑
    /// </summary>
    public partial class ParamsItemsConfigPage
    {

        #region Member
        /// <summary>
        /// 当前选中的参数大项的内容 可以看这个类的内容
        /// </summary>
        public  StatisticalParam StatisticalParam;

        public static StatisticalParam StatisticalParam_;

        /// <summary>
        /// 参数大项对应的参数小项的内容
        /// </summary>
        private ObservableCollection<CombinedParamItemModel> mListParamItems;

        private ObservableCollection<StatisticalParamItem> mListStatisticalParamItem;

        public List<StatisticalParamItemDetail> ListStatisticalParamItemDetails;

        public List<ParamItemViewItem> ListParamItemViewItem;

        public bool DesignAndSaveIsEnabled;

        public SCMainView SCParent;

        private ObservableCollection<ParamItemSubItem> mListConditionItemSubItems;

        #endregion

        public ParamsItemsConfigPage()
        {
            InitializeComponent();

            mListParamItems = new ObservableCollection<CombinedParamItemModel>();
            ListStatisticalParamItemDetails = new List<StatisticalParamItemDetail>();
            mListStatisticalParamItem = new ObservableCollection<StatisticalParamItem>();
            ListParamItemViewItem = new List<ParamItemViewItem>();
            ListBoxStatisticItems.ItemsSource = mListParamItems;
            mListConditionItemSubItems = new ObservableCollection<ParamItemSubItem>();

            ComboxStatisticTime.ItemsSource = mListConditionItemSubItems;

            Loaded += ParamsItemsConfigPage_Loaded;
        }

        void ParamsItemsConfigPage_Loaded(object sender, RoutedEventArgs e)
        {
            Image aaa = new Image();
            aaa.SetResourceReference(StyleProperty,"TuDing");
            ThisCB.Header = aaa;

            InitmStatisticalParam();
        }

        public void InitmStatisticalParam()
        {
            try
            {
                IsDistributeOrgSkg();
                LoadParamState();
                InitComboxContent();
                LoadParamContent();
                IsHidden();
                SetDuritiomValue();
                ChangeLanguage();
                // DesignAndSaveIsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        #region 公共函数[用来加载不同参数大项的里面内容的]

        //按照选择的是哪个大项参数来加载其状态情况的
        private void LoadParamState()
        {
            //是否可组合
            //if (StatisticalParam.IsCombine == "1")
            //{
            //    IsCombine.IsChecked = true;

            //}
            //else
            //{
            //    IsCombine.IsChecked = false;
            //}

            //是否启用
            if (StatisticalParam.IsUsed == "1")
            {
                IsUsed.IsChecked = true;
                SetIsEnableEdit(true);
            }
            else
            {
                IsUsed.IsChecked = false;
                SetIsEnableEdit(false);
            }

            //是否可编辑 [这个是根据这个对象是否分配到了技能组来看的,如果分配到了技能组,那么就不能编辑,默认能编辑]
            ///内容先不写  之后补起来   好了已经补齐来了
            if (IsEdit.IsChecked == true)
            {
                SetIsEnableEdit(true);
            }
            else
            {
                SetIsEnableEdit(false);
            }
        }

        //获取参数大项里面的内容 也就是参数小项
        private void LoadParamContent()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.GetSelectParamItemsInfos;
                webRequest.Session = CurrentApp.Session;
                //先传入选择的参数大项的ID
                webRequest.ListData.Add(StatisticalParam.StatisticalParamID.ToString());
               
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }

                mListParamItems.Clear();
                List<CombinedParamItemModel> templist = new List<CombinedParamItemModel>();
                mListStatisticalParamItem.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<StatisticalParamItem>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    StatisticalParamItem item = optReturn.Data as StatisticalParamItem;//注意看这里的类型
                    mListStatisticalParamItem.Add(item);
                    if (item == null)
                    {
                        ShowException(string.Format("Fail. CustomConditionItem is null"));
                        return;
                    }
                   
                    CombinedParamItemModel itemItem = new CombinedParamItemModel(item,CurrentApp);//再看下这里的类型  看下这个是怎么实现的
                    templist.Add(itemItem);
                }
                var itemList = templist.OrderBy(temp => temp.SortID).ToList();
                for (int i = 0; i < itemList.Count; i++)
                {
                    mListParamItems.Add(itemList[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        //将从T_31_044表里存的值放到界面上  也就是传到 ParamItemViewItem界面
        private void CreateParamItemValue()
        {
            //根据参数大项  来加载  每个参数大项里面参数小项的值
            for (int i = 0; i < ListStatisticalParamItemDetails.Count; i++)
            {
                for (int j = 0; j < mListStatisticalParamItem.Count; j++)
                {
                    if (ListStatisticalParamItemDetails[i].StatisticalParamItemID == mListStatisticalParamItem[j].StatisticalParamItemID)
                    {
                        ParamItemViewItem paramItemViewItem = new ParamItemViewItem();
                        paramItemViewItem.CurrentApp = CurrentApp;
                        //paramItemViewItem.InitParamItemAndParamItemValue(mListStatisticalParamItem[j],ListStatisticalParamItemDetails[i]);
                        //paramItemViewItem.ParamItem = mListStatisticalParamItem[j];
                        //paramItemViewItem.ParamItemValue = mListStatisticalParamItemDetails[i];
                        //paramItemViewItem.Tag = mListStatisticalParamItem[j];
                        //paramItemViewItem.SetValue();
                    }
                }
            }
        }

        public bool CreateConditionResultList()//这个就是把界面上的写的内容  在点击保存按钮之后，将条件放入_____   再存入数据库
        //（后来存入了 mListUCConditionItems 里面）,放到mListQueryConditionDetails里面
        {
            try
            {
                //mListSubItems.Clear();
                ListStatisticalParamItemDetails.Clear();
                for (int i = 0; i < mListStatisticalParamItem.Count; i++)
                {
                    for (int j = 0; j < mListParamItems.Count; j++)
                    {
                        if (mListParamItems[j].StatisticalParamItemID == mListStatisticalParamItem[i].StatisticalParamItemID.ToString())
                        {
                            ParamItemViewItem ucConditionItem = mListParamItems[j].ParamItemViewItem_;
                            ucConditionItem.CombinedParamItem = mListParamItems[j];
                            //ucConditionItem.Value01 = "1";
                            if (mListParamItems[j].StatisticalParamItemID == S3108Consts.CON_AfterDealDurationSec.ToString() ||
                                mListParamItems[j].StatisticalParamItemID == S3108Consts.CON_CallDurationCompareAva.ToString() ||
                                mListParamItems[j].StatisticalParamItemID == S3108Consts.CON_CallDurationComparePec.ToString())
                            {
                                if (JustCheckIt() == false)
                                {
                                    return false;
                                }
                                //如果是这三个条件  那么界面上统计区段也要获取到
                                ucConditionItem.ParamItemValue.Value2 = IUD.Text.ToString();
                                var temp = ComboxStatisticTime.SelectedItem as ParamItemSubItem;
                                if (temp != null)
                                {
                                    ucConditionItem.ParamItemValue.Value3 = temp.Value.ToString();
                                }

                            }
                            if (ucConditionItem.GetValue(mListStatisticalParamItem[i])==false)
                            {
                                return false;
                            }
                            //ucConditionItem.GetValue(mListStatisticalParamItem[i]);//输入内容判断改为bool返回类型（可否？）
                            ListStatisticalParamItemDetails.Add(ucConditionItem.ParamItemValue);//mListQueryConditionDetails是把所有输入的查询条件的集合
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                ShowException(ex.Message);
            }
        }
        
        //从数据库里读取值  然后 放到界面上   这里是  统计区段的值和统计区段的单位
        private void SetDuritiomValue()
        {
            var temp = mListStatisticalParamItem.FirstOrDefault(p =>
                p.StatisticalParamItemID == S3108Consts.CON_AfDeDurMoreAvaDeDurSec ||
                p.StatisticalParamItemID == S3108Consts.CON_CallDurationCompareAva ||
                p.StatisticalParamItemID == S3108Consts.CON_CallDurationComparePec);
            if (temp == null)
            {
                //如果可组合的条件里面没有需要带上统计区段的  那么 这个时间的就不能编辑
                StatisticDurition.IsEnabled = false;
            }
            else
            {
                IUD.Text = temp.StatisticDuration.ToString();
                if (ComboxStatisticTime != null)
                {
                    //单位存在value3里面  时间单位1、年，2、月 ， 3、周，4、天，5、小时,6、分钟
                    var tempIndex = temp.StatisticUnit.ToString();
                    //这里绝对不能用tempIndex == string.empty 因为  tempIndex是
                    if ( tempIndex == null)
                    {
                        tempIndex = string.Empty;
                    }
                    var temp_ = mListConditionItemSubItems.FirstOrDefault(t => t.Value.ToString() == tempIndex.ToString());
                    if (temp_ != null)
                    {

                        temp_.IsChecked = true;

                        if (ComboxStatisticTime != null)
                        {
                            ComboxStatisticTime.SelectedItem = temp_;
                        }
                    }
                }
            }
        }

        //要写一个函数  作用是写入数据到T_31_050表C006字段  这个里面是放是否启动这个参数大项的设置
        public void ModifyStatisticParam()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.ModifyStatisticParam;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(StatisticalParam.StatisticalParamID.ToString());
                if (IsUsed.IsChecked == true)
                {
                    StatisticalParam.IsUsed = "1";
                }
                else
                {
                    StatisticalParam.IsUsed = "0";
                }
                webRequest.ListData.Add(StatisticalParam.IsUsed);
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //是否能编辑的控制
        private void SetIsEnableEdit(bool isEnable)
        {
            BorderABCD.IsEnabled = isEnable;
        }

        //这个参数大项是否分配到了技能组如果分配到了技能组或者机构 不过要在生效时间之内的才算有效  现在暂时不考虑生效时间
        private void IsDistributeOrgSkg()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.IsDistributeOrgSkg;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(StatisticalParam.StatisticalParamID.ToString());
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                string item = webReturn.Data as string;
                if (item == "1")
                {
                    IsEdit.IsChecked = true;
                    DesignAndSaveIsEnabled = true;
                    StatisticDurition.IsEnabled = true;
                    
                }
                if (item == "0")
                {
                    IsEdit.IsChecked = false;
                    DesignAndSaveIsEnabled = false;
                    StatisticDurition.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //统计区段是否隐藏的操作
        private void IsHidden()
        {
            if (StatisticalParam.StatisticalParamID == S3108Consts.CON_ServiceAttitude_ ||
                StatisticalParam.StatisticalParamID == S3108Consts.CON_ProfessionalLevel_)
            {
                StatisticDurition.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void InitComboxContent()
        {
            try
            {
                mListConditionItemSubItems.Clear();
                ParamItemSubItem item = new ParamItemSubItem();

                item = new ParamItemSubItem();
                item.Name = "Month";
                item.Value = 2;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3108C002"), "Month");
                mListConditionItemSubItems.Add(item);
                item = new ParamItemSubItem();
                item.Name = "Week";
                item.Value = 3;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3108C003"), "Week");
                mListConditionItemSubItems.Add(item);
                item = new ParamItemSubItem();
                item.Name = "Day";
                item.Value = 4;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3108C004"), "Day");
                mListConditionItemSubItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            long temp = StatisticalParam.StatisticalParamID - 3110000000000000000;
            string tempstr = temp.ToString();
            
            ParamStateTitle.Content = CurrentApp.GetLanguageInfo(string.Format("FO3108010200{0}", tempstr), "ParamState");
            //IsCombine.Content = CurrentApp.GetLanguageInfo("310801P003", "Can Combine");
            IsUsed.Content = CurrentApp.GetLanguageInfo("310801P004", "Can Use");
            IsEdit.Content = CurrentApp.GetLanguageInfo("310801P005", "Can Edit");
        }

        private bool JustCheckIt()
        {
            var temp__ = ComboxStatisticTime.SelectedItem as ParamItemSubItem;
            if (temp__ == null)
            {
                return false;
            }
            if (temp__.Value == 2 && double.Parse(IUD.Text.ToString()) > 12)
            {
                ShowException(CurrentApp.GetLanguageInfo("3108T004", "需要统计的时间过长"));
                return false;
            }
            if (temp__.Value == 3 && double.Parse(IUD.Text.ToString()) > 52)
            {
                ShowException(CurrentApp.GetLanguageInfo("3108T004", "需要统计的时间过长"));
                return false;
            }
            if (temp__.Value == 4 && double.Parse(IUD.Text.ToString()) > 365)
            {
                ShowException(CurrentApp.GetLanguageInfo("3108T004", "需要统计的时间过长"));
                return false;
            }
            int i;
            if (int.TryParse(IUD.Text.ToString(), out i)==false)
            {
                return false;
            }
            return true;
        }

    }
}
