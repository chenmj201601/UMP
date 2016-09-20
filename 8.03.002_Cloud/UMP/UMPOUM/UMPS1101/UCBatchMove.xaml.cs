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
using UMPS1101.Models;
using UMPS1101.Wcf11011;
using UMPS1101.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11011;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1101
{
    /// <summary>
    /// UCBatchMove.xaml 的交互逻辑
    /// </summary>
    public partial class UCBatchMove
    {
        #region member
        private ObjectItem mRoot;
        private ObjectItem SelectOrgItem;
        public SelectedInfo mSelectInfo;
        public OUMMainView ParentPage;
        //public S1101App CurrentApp;
        #endregion

        public UCBatchMove()
        {
            InitializeComponent();

            mRoot = new ObjectItem();
            SelectOrgItem = new ObjectItem();

            Loaded += UCBatchMove_Loaded;
        }

        void UCBatchMove_Loaded(object sender, RoutedEventArgs e)
        {
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            this.TvSample.ItemsSource = mRoot.Children;

            Init();
            ChangeLanguage();
        }

        private void Init()
        {
            LoadAvaliableOrgs(mRoot, "-1");
        }

        private void LoadAvaliableOrgs(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S1101Codes.GetOrganizationList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service11011Client client = new Service11011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitControledOrgs Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ObjectItem> listChild = new List<ObjectItem>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {

                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicOrgInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitControledOrgs Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicOrgInfo orgInfo = optReturn.Data as BasicOrgInfo;
                    if (orgInfo != null)
                    {
                        ObjectItem item = new ObjectItem();
                        item.ObjType = S1101Consts.OBJTYPE_ORG;
                        item.ObjID = orgInfo.OrgID;
                        orgInfo.OrgName = orgInfo.OrgName;
                        orgInfo.StrStartTime = DateTime.Parse(orgInfo.StrStartTime).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
                        orgInfo.StrEndTime = orgInfo.StrEndTime;
                        if (!orgInfo.StrEndTime.ToUpper().Equals("UNLIMITED"))
                        {
                            orgInfo.EndTime = DateTime.Parse(orgInfo.StrEndTime).ToLocalTime();
                            orgInfo.StrEndTime = DateTime.Parse(orgInfo.StrEndTime).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
                        }
                        orgInfo.CreateTime = orgInfo.CreateTime.ToLocalTime();
                        item.Name = orgInfo.OrgName;
                        item.FullName = item.Name;
                        item.Description = orgInfo.Description;
                        item.State = Convert.ToInt32(orgInfo.IsActived);
                        item.ObjParentID = orgInfo.ParentID;
                        if (item.State == 1)
                        {
                            item.TipState = CurrentApp.GetLanguageInfo("1101T10109", "IsActived");
                        }
                        else
                        {
                            item.TipState = CurrentApp.GetLanguageInfo("1101T10110", "Disable");
                        }
                        if (item.ObjID == ConstValue.ORG_ROOT)
                        {
                            item.Icon = "Images/root.ico";
                        }
                        else
                        {
                            item.Icon = "Images/org.ico";
                        }
                        item.Data = orgInfo;
                        LoadAvaliableOrgs(item, orgInfo.OrgID.ToString());

                        Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region
        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            //获取选中的机构
            var TempObjItem = TvSample.SelectedItem;
            if (TempObjItem != null)
            {
                SelectOrgItem = TempObjItem as ObjectItem;
            }
            //调用move方法移动
            if (mSelectInfo != null && SelectOrgItem != null)
                ParentPage.BatchMove(mSelectInfo, SelectOrgItem);
            //关闭
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }
        #endregion

        #region Languages

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("110110", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("110111", "Close");

                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("1101T10221", "Batch Move");
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion
    }
}
