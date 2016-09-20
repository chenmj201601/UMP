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
    public partial class UCResourceType212B : UserControl
    {
        public DataTable IDataTableDecServer = new DataTable();
        private string IStrCurrentDecID = string.Empty;
        private string IStrVerificationCode004 = string.Empty;
        private string IStrVerificationCode104 = string.Empty;

        public Page11100A IPageTopParent = null;

        public bool IBoolShowedData = false;

        public bool IBoolCanEdit = true;

        public UCResourceType212B()
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
            LabelDecServer.Content = "";
            TextBoxDecServer01.Text = App.GetDisplayCharater("1110023");
            TextBoxDecServer02.Text = App.GetDisplayCharater("1110024");
            TextBoxDecServer03.Text = App.GetDisplayCharater("1110027");
            TextBoxDecServer04.Text = App.GetDisplayCharater("1110025");
            TextBoxDecServer05.Text = App.GetDisplayCharater("1110028");

            IPageTopParent.IOperationEvent += IPageTopParent_IOperationEvent;
        }

        public void ShowSettedData(DataSet ADataSetSource, string AStrDecID)
        {
            string LStrServerData011 = string.Empty;
            string LStrServerData012 = string.Empty;
            string LStrServerData013 = string.Empty;
            string LStrServerData014 = string.Empty;
            string LStrServerData015 = string.Empty;
            string LStrServerData016 = string.Empty;
            string LStrServerData017 = string.Empty;
            string LStrServerData018 = string.Empty;

            try
            {
                IStrCurrentDecID = AStrDecID;

                IDataTableDecServer = ADataSetSource.Tables[0];
                GridDecServer01.Children.Clear();
                GridDecServer02.Children.Clear();
                GridDecServer03.Children.Clear();
                GridDecServer04.Children.Clear();
                GridDecServer05.Children.Clear();

                DataRow[] LDataRowDECServer = IDataTableDecServer.Select("C001 = " + AStrDecID, "C002 ASC");
                LStrServerData011 = LDataRowDECServer[0]["C011"].ToString();
                LStrServerData012 = LDataRowDECServer[0]["C012"].ToString();
                LStrServerData013 = LDataRowDECServer[0]["C013"].ToString();
                LStrServerData014 = LDataRowDECServer[0]["C014"].ToString();
                LStrServerData014 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData014, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrServerData015 = LDataRowDECServer[0]["C015"].ToString();
                LStrServerData015 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData015, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrServerData016 = LDataRowDECServer[0]["C016"].ToString();
                LStrServerData016 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData016, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrServerData017 = LDataRowDECServer[0]["C017"].ToString();
                LStrServerData017 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData017, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrServerData018 = LDataRowDECServer[0]["C018"].ToString();
                LStrServerData018 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData018, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LabelDecServer.Content = LStrServerData017;

                UCTextBoxForSettedDataView LUCDataView03 = new UCTextBoxForSettedDataView();
                LUCDataView03.StrDataView = LStrServerData018;
                GridDecServer03.Children.Add(LUCDataView03);

                UCTextBoxForSettedDataView LUCDataView04 = new UCTextBoxForSettedDataView();
                LUCDataView04.StrDataView = App.GetDisplayCharater("UCResourceType212A", "SSLStatus" + LStrServerData016);
                GridDecServer04.Children.Add(LUCDataView04);

                UCTextBoxForSettedDataView LUCDataView05 = new UCTextBoxForSettedDataView();
                LUCDataView05.StrDataView = LStrServerData015;
                GridDecServer05.Children.Add(LUCDataView05);

                if (!IBoolCanEdit)
                {
                    UCTextBoxForSettedDataView LUCDataView01 = new UCTextBoxForSettedDataView();
                    LUCDataView01.StrDataView = LStrServerData017;
                    GridDecServer01.Children.Add(LUCDataView01);

                    UCTextBoxForSettedDataView LUCDataView02 = new UCTextBoxForSettedDataView();
                    LUCDataView02.StrDataView = LStrServerData014;
                    GridDecServer02.Children.Add(LUCDataView02);

                }
                else
                {
                    UCInputType203 LUCServerIPAddress = new UCInputType203();
                    LUCServerIPAddress.SetElementData(LStrServerData017);
                    LUCServerIPAddress.IOperationEvent += LUCServerIPAddress_IOperationEvent;
                    GridDecServer01.Children.Add(LUCServerIPAddress);

                    UCInputType103 LUCServerPort = new UCInputType103();
                    LUCServerPort.SetElementData(LStrServerData014);
                    LUCServerPort.SetMinMaxDefaultValue(1024, 65535, 3072);
                    GridDecServer02.Children.Add(LUCServerPort);
                }

                IBoolShowedData = true;
            }
            catch { }
        }

        private void LUCServerIPAddress_IOperationEvent(object sender, PFShareControls.OperationEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void WriteElement2DataSet()
        {
            string LStrServerIP = string.Empty;
            string LStrServerPort = string.Empty;

            if (!IBoolCanEdit) { return; }

            UCInputType203 LUCServerIPAddress = GridDecServer01.Children[0] as UCInputType203;
            LStrServerIP = LUCServerIPAddress.GetElementData();

            UCInputType103 LUCServerPort = GridDecServer02.Children[0] as UCInputType103;
            LStrServerPort = LUCServerPort.GetElementData();

            DataRow[] LDataRowDecServer = App.IListDataSetReturn[1].Tables[0].Select("C001 = " + IStrCurrentDecID);

            LDataRowDecServer[0]["C014"] = EncryptionAndDecryption.EncryptDecryptString(LStrServerPort, IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
            LDataRowDecServer[0]["C017"] = EncryptionAndDecryption.EncryptDecryptString(LStrServerIP, IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

            OperationEventArgs LEventArgs = new OperationEventArgs();
            LEventArgs.StrObjectTag = "S212";
            LEventArgs.ObjectSource0 = IStrCurrentDecID;
            IPageTopParent.RefreshTreeViewViewData(LEventArgs);

        }
    }
}
