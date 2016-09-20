using PFShareClassesC;
using System;
using System.Collections.Generic;
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
using UMPS1110.ShowStyleControls;

namespace UMPS1110
{
    public partial class UCResourceType211 : UserControl
    {
        public DataTable IDataTableLicenseServer = new DataTable();
        private string IStrVerificationCode004 = string.Empty;
        private string IStrVerificationCode104 = string.Empty;

        public Page11100A IPageTopParent = null;

        public bool IBoolShowedData = false;

        public bool IBoolCanEdit = true;

        public UCResourceType211()
        {
            InitializeComponent();
            IStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
            IStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
        }

        private void IPageTopParent_IOperationEvent(object sender, OperationEventArgs e)
        {
            if (e.StrObjectTag == "M-Save") { WriteElement2DataSet(); return; }
        }

        public void ShowElementContent()
        {
            LabelLicenseServer01.Content = App.GetDisplayCharater("1110021");
            TextBoxLicenseServer0101.Text = App.GetDisplayCharater("1110015");
            TextBoxLicenseServer0102.Text = App.GetDisplayCharater("1110016");
            TextBoxLicenseServer0103.Text = App.GetDisplayCharater("1110017");
            TextBoxLicenseServer0104.Text = App.GetDisplayCharater("1110018");
            TextBoxLicenseServer0105.Text = App.GetDisplayCharater("1110019");

            LabelLicenseServer02.Content = App.GetDisplayCharater("1110022");
            TextBoxLicenseServer0201.Text = App.GetDisplayCharater("1110015");
            TextBoxLicenseServer0202.Text = App.GetDisplayCharater("1110016");
            TextBoxLicenseServer0203.Text = App.GetDisplayCharater("1110017");
            TextBoxLicenseServer0204.Text = App.GetDisplayCharater("1110018");
            TextBoxLicenseServer0205.Text = App.GetDisplayCharater("1110019");

            IPageTopParent.IOperationEvent += IPageTopParent_IOperationEvent;
        }

        public void ShowSettedData(DataSet ADataSetSource)
        {
            try
            {
                IDataTableLicenseServer = ADataSetSource.Tables[0];
                GridLicenseServer0101.Children.Clear();
                GridLicenseServer0102.Children.Clear();
                GridLicenseServer0103.Children.Clear();
                GridLicenseServer0104.Children.Clear();
                GridLicenseServer0105.Children.Clear();

                GridLicenseServer0201.Children.Clear();
                GridLicenseServer0202.Children.Clear();
                GridLicenseServer0203.Children.Clear();
                GridLicenseServer0204.Children.Clear();
                GridLicenseServer0205.Children.Clear();

                #region 显示主服务器信息
                ShowMainServerInformation();
                #endregion

                #region 显示备用服务器信息
                ShowBackServerInformtion();
                #endregion

                IBoolShowedData = true;
            }
            catch { }
        }

        private void ShowMainServerInformation()
        {
            string LStrServerData011 = string.Empty;
            string LStrServerData012 = string.Empty;
            string LStrServerData013 = string.Empty;
            string LStrServerData014 = string.Empty;
            string LStrServerData017 = string.Empty;
            string LStrServerData018 = string.Empty;

            DataRow[] LDataRowLicenseServer1 = IDataTableLicenseServer.Select("C012 = '1'");

            LStrServerData011 = LDataRowLicenseServer1[0]["C011"].ToString();
            LStrServerData012 = LDataRowLicenseServer1[0]["C012"].ToString();
            LStrServerData013 = LDataRowLicenseServer1[0]["C013"].ToString();
            LStrServerData014 = LDataRowLicenseServer1[0]["C014"].ToString();
            LStrServerData014 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData014, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrServerData017 = LDataRowLicenseServer1[0]["C017"].ToString();
            LStrServerData017 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData017, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrServerData018 = LDataRowLicenseServer1[0]["C018"].ToString();
            LStrServerData018 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData018, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

            UCTextBoxForSettedDataView LUCDataView0104 = new UCTextBoxForSettedDataView();
            LUCDataView0104.StrDataView = LStrServerData018;
            GridLicenseServer0104.Children.Add(LUCDataView0104);

            if (!IBoolCanEdit)
            {
                UCTextBoxForSettedDataView LUCDataView0101 = new UCTextBoxForSettedDataView();
                LUCDataView0101.StrDataView = App.GetDisplayCharater("UCResourceType211", "C011V" + LStrServerData011);
                GridLicenseServer0101.Children.Add(LUCDataView0101);

                UCTextBoxForSettedDataView LUCDataView0102 = new UCTextBoxForSettedDataView();
                LUCDataView0102.StrDataView = App.GetDisplayCharater("UCResourceType211", "C012V" + LStrServerData012);
                GridLicenseServer0102.Children.Add(LUCDataView0102);

                UCTextBoxForSettedDataView LUCDataView0103 = new UCTextBoxForSettedDataView();
                LUCDataView0103.StrDataView = LStrServerData017;
                GridLicenseServer0103.Children.Add(LUCDataView0103);

                UCTextBoxForSettedDataView LUCDataView0105 = new UCTextBoxForSettedDataView();
                LUCDataView0105.StrDataView = LStrServerData014;
                GridLicenseServer0105.Children.Add(LUCDataView0105);
            }
            else
            {
                UCInputType201 LUCServerStatus = new UCInputType201();
                LUCServerStatus.StrData1 = App.GetDisplayCharater("1110011");
                LUCServerStatus.StrData0 = App.GetDisplayCharater("1110012");
                LUCServerStatus.SetElementData(LStrServerData011, "");
                LUCServerStatus.IOperationEvent += LUCServerStatus_IOperationEvent;
                GridLicenseServer0101.Children.Add(LUCServerStatus);
                LUCServerStatus.IStrThisName = "S0101";

                UCInputType202 LUCServerMainBack = new UCInputType202();
                LUCServerMainBack.StrData1 = App.GetDisplayCharater("1110014");
                LUCServerMainBack.StrData2 = App.GetDisplayCharater("1110013");
                LUCServerMainBack.IOperationEvent += LUCServerMainBack_IOperationEvent;
                LUCServerMainBack.SetElementData(LStrServerData012, "");
                GridLicenseServer0102.Children.Add(LUCServerMainBack);
                LUCServerMainBack.IStrThisName = "S0102";

                UCInputType203 LUCServerIPAddress = new UCInputType203();
                LUCServerIPAddress.SetElementData(LStrServerData017);
                LUCServerIPAddress.IOperationEvent += LUCServerIPAddress1IOperationEvent;
                GridLicenseServer0103.Children.Add(LUCServerIPAddress);

                UCInputType103 LUCServerPort = new UCInputType103();
                LUCServerPort.SetElementData(LStrServerData014);
                LUCServerPort.SetMinMaxDefaultValue(1024, 65535, 3070);
                GridLicenseServer0105.Children.Add(LUCServerPort);
            }
        }

        private void ShowBackServerInformtion()
        {
            string LStrServerData011 = string.Empty;
            string LStrServerData012 = string.Empty;
            string LStrServerData013 = string.Empty;
            string LStrServerData014 = string.Empty;
            string LStrServerData017 = string.Empty;
            string LStrServerData018 = string.Empty;

            DataRow[] LDataRowLicenseServer2 = IDataTableLicenseServer.Select("C012 = '2'");
            LStrServerData011 = LDataRowLicenseServer2[0]["C011"].ToString();
            LStrServerData012 = LDataRowLicenseServer2[0]["C012"].ToString();
            LStrServerData013 = LDataRowLicenseServer2[0]["C013"].ToString();
            LStrServerData014 = LDataRowLicenseServer2[0]["C014"].ToString();
            LStrServerData014 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData014, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrServerData017 = LDataRowLicenseServer2[0]["C017"].ToString();
            LStrServerData017 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData017, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrServerData018 = LDataRowLicenseServer2[0]["C018"].ToString();
            LStrServerData018 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData018, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

            UCTextBoxForSettedDataView LUCDataView0204 = new UCTextBoxForSettedDataView();
            LUCDataView0204.StrDataView = LStrServerData018;
            GridLicenseServer0204.Children.Add(LUCDataView0204);

            if (!IBoolCanEdit)
            {
                UCTextBoxForSettedDataView LUCDataView0201 = new UCTextBoxForSettedDataView();
                LUCDataView0201.StrDataView = App.GetDisplayCharater("UCResourceType211", "C011V" + LStrServerData011);
                GridLicenseServer0201.Children.Add(LUCDataView0201);

                UCTextBoxForSettedDataView LUCDataView0202 = new UCTextBoxForSettedDataView();
                LUCDataView0202.StrDataView = App.GetDisplayCharater("UCResourceType211", "C012V" + LStrServerData012);
                GridLicenseServer0202.Children.Add(LUCDataView0202);

                UCTextBoxForSettedDataView LUCDataView0203 = new UCTextBoxForSettedDataView();
                LUCDataView0203.StrDataView = LStrServerData017;
                GridLicenseServer0203.Children.Add(LUCDataView0203);

                UCTextBoxForSettedDataView LUCDataView0205 = new UCTextBoxForSettedDataView();
                LUCDataView0205.StrDataView = LStrServerData014;
                GridLicenseServer0205.Children.Add(LUCDataView0205);
            }
            else
            {
                UCInputType201 LUCServerStatus = new UCInputType201();
                LUCServerStatus.StrData1 = App.GetDisplayCharater("1110011");
                LUCServerStatus.StrData0 = App.GetDisplayCharater("1110012");
                LUCServerStatus.SetElementData(LStrServerData011, "");
                LUCServerStatus.IOperationEvent += LUCServerStatus_IOperationEvent;
                GridLicenseServer0201.Children.Add(LUCServerStatus);
                LUCServerStatus.IStrThisName = "S0201";

                UCInputType202 LUCServerMainBack = new UCInputType202();
                LUCServerMainBack.StrData1 = App.GetDisplayCharater("1110014");
                LUCServerMainBack.StrData2 = App.GetDisplayCharater("1110013");
                LUCServerMainBack.IOperationEvent += LUCServerMainBack_IOperationEvent;
                LUCServerMainBack.SetElementData(LStrServerData012, "");
                GridLicenseServer0202.Children.Add(LUCServerMainBack);
                LUCServerMainBack.IStrThisName = "S0202";

                UCInputType203 LUCServerIPAddress = new UCInputType203();
                LUCServerIPAddress.SetElementData(LStrServerData017);
                LUCServerIPAddress.IOperationEvent += LUCServerIPAddress2IOperationEvent;
                GridLicenseServer0203.Children.Add(LUCServerIPAddress);

                UCInputType103 LUCServerPort = new UCInputType103();
                LUCServerPort.SetElementData(LStrServerData014);
                LUCServerPort.SetMinMaxDefaultValue(1024, 65535, 3070);
                GridLicenseServer0205.Children.Add(LUCServerPort);
            }
        }

        private void LUCServerIPAddress1IOperationEvent(object sender, PFShareControls.OperationEventArgs e)
        {
            //MessageBox.Show(e.StrObjectTag, e.ObjectSource.ToString());
        }

        private void LUCServerIPAddress2IOperationEvent(object sender, PFShareControls.OperationEventArgs e)
        {
            //MessageBox.Show(e.StrObjectTag, e.ObjectSource.ToString());
        }

        private void LUCServerStatus_IOperationEvent(object sender, OperationEventArgs e)
        {
            //MessageBox.Show(e.ObjectSource0.ToString(), e.StrObjectTag);
        }

        private void LUCServerMainBack_IOperationEvent(object sender, OperationEventArgs e)
        {
            string LStrSource = string.Empty;
            string LStrCurrentData = string.Empty;
            string LStrChangedData = string.Empty;

            LStrSource = e.StrObjectTag;

            if (string.IsNullOrEmpty(LStrSource)) { return; }

            LStrCurrentData = e.ObjectSource0 as string;
            UCInputType202 LUCServerMainBack = null;

            if (LStrCurrentData == "1") { LStrChangedData = "2"; } else { LStrChangedData = "1"; }

            if (LStrSource == "S0102")
            {
                LUCServerMainBack = GridLicenseServer0202.Children[0] as UCInputType202;
                LabelLicenseServer01.Content = App.GetDisplayCharater("UCResourceType211", "ServerType" + LStrCurrentData);
                LabelLicenseServer02.Content = App.GetDisplayCharater("UCResourceType211", "ServerType" + LStrChangedData);
            }
            else
            {
                LUCServerMainBack = GridLicenseServer0102.Children[0] as UCInputType202;
                LabelLicenseServer02.Content = App.GetDisplayCharater("UCResourceType211", "ServerType" + LStrCurrentData);
                LabelLicenseServer01.Content = App.GetDisplayCharater("UCResourceType211", "ServerType" + LStrChangedData);
            }

            LUCServerMainBack.SetElementData(LStrChangedData, "Change");

        }

        private void WriteElement2DataSet()
        {
            if (!IBoolCanEdit) { return; }

            List<string> LListStrMainSetting = new List<string>();
            List<string> LListStrBackSetting = new List<string>();

            UCInputType201 LUCServerStatusTop = GridLicenseServer0101.Children[0] as UCInputType201;
            UCInputType201 LUCServerStatusButtom = GridLicenseServer0201.Children[0] as UCInputType201;

            UCInputType202 LUCServerMainBackTop = GridLicenseServer0102.Children[0] as UCInputType202;
            UCInputType202 LUCServerMainBackButtom = GridLicenseServer0202.Children[0] as UCInputType202;

            UCInputType203 LUCServerIPAddressTop = GridLicenseServer0103.Children[0] as UCInputType203;
            UCInputType203 LUCServerIPAddressButtom = GridLicenseServer0203.Children[0] as UCInputType203;

            UCInputType103 LUCServerPortTop = GridLicenseServer0105.Children[0] as UCInputType103;
            UCInputType103 LUCServerPortButtom = GridLicenseServer0205.Children[0] as UCInputType103;

            if (LUCServerMainBackTop.GetElementData() == "1")
            {
                LListStrMainSetting.Add(LUCServerStatusTop.GetElementData());
                LListStrMainSetting.Add("1");
                LListStrMainSetting.Add(LUCServerIPAddressTop.GetElementData());
                LListStrMainSetting.Add(LUCServerPortTop.GetElementData());

                LListStrBackSetting.Add(LUCServerStatusButtom.GetElementData());
                LListStrBackSetting.Add("2");
                LListStrBackSetting.Add(LUCServerIPAddressButtom.GetElementData());
                LListStrBackSetting.Add(LUCServerPortButtom.GetElementData());
            }
            else
            {
                LListStrMainSetting.Add(LUCServerStatusButtom.GetElementData());
                LListStrMainSetting.Add("1");
                LListStrMainSetting.Add(LUCServerIPAddressButtom.GetElementData());
                LListStrMainSetting.Add(LUCServerPortButtom.GetElementData());

                LListStrBackSetting.Add(LUCServerStatusTop.GetElementData());
                LListStrBackSetting.Add("2");
                LListStrBackSetting.Add(LUCServerIPAddressTop.GetElementData());
                LListStrBackSetting.Add(LUCServerPortTop.GetElementData());
            }

            DataRow[] LDataRowLicenseServerMain = App.IListDataSetReturn[0].Tables[0].Select("C001 = 2110000000000000001");
            LDataRowLicenseServerMain[0]["C011"] = LListStrMainSetting[0];
            LDataRowLicenseServerMain[0]["C012"] = LListStrMainSetting[1];
            LDataRowLicenseServerMain[0]["C014"] = EncryptionAndDecryption.EncryptDecryptString(LListStrMainSetting[3], IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
            LDataRowLicenseServerMain[0]["C017"] = EncryptionAndDecryption.EncryptDecryptString(LListStrMainSetting[2], IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

            DataRow[] LDataRowLicenseServerBack = App.IListDataSetReturn[0].Tables[0].Select("C001 = 2110000000000000002");
            LDataRowLicenseServerBack[0]["C011"] = LListStrBackSetting[0];
            LDataRowLicenseServerBack[0]["C012"] = LListStrBackSetting[1];
            LDataRowLicenseServerBack[0]["C014"] = EncryptionAndDecryption.EncryptDecryptString(LListStrBackSetting[3], IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
            LDataRowLicenseServerBack[0]["C017"] = EncryptionAndDecryption.EncryptDecryptString(LListStrBackSetting[2], IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

            OperationEventArgs LEventArgs = new OperationEventArgs();
            LEventArgs.StrObjectTag = "S211";
            IPageTopParent.RefreshTreeViewViewData(LEventArgs);
        }
    }
}
