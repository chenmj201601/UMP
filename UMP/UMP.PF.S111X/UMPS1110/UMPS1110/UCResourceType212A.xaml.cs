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

namespace UMPS1110
{
    public partial class UCResourceType212A : UserControl, S1110Interface
    {
        //发送消息给父页面
        public event EventHandler<OperationEventArgs> IOperationEvent;

        public DataTable IDataTableDecServer = new DataTable();
        private string IStrVerificationCode004 = string.Empty;
        private string IStrVerificationCode104 = string.Empty;

        public Page11100A IPageTopParent = null;

        public bool IBoolShowedData = false;

        private bool IBoolCanEdit = true;

        public UCResourceType212A()
        {
            InitializeComponent();
            IStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
            IStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
            this.Loaded += UCResourceType212A_Loaded;
        }

        private void UCResourceType212A_Loaded(object sender, RoutedEventArgs e)
        {
            IPageTopParent.IOperationEvent += IPageTopParent_IOperationEvent;
        }

        private void IPageTopParent_IOperationEvent(object sender, OperationEventArgs e)
        {
            //if (e.StrObjectTag == "M-Save") { WriteElement2DataSet(); return; }
        }

        public void ShowElementContent()
        {
            ServerIPColumnHeader.Content = App.GetDisplayCharater("1110023");
            ServerPortColumnHeader.Content = App.GetDisplayCharater("1110024");
            UsedSSLColumnHeader.Content = App.GetDisplayCharater("1110025");
        }

        public void ShowSettedData(DataSet ADataSetSource)
        {
            string LStrIconPngName = string.Empty;
            string LStrServerData001 = string.Empty;
            string LStrServerData011 = string.Empty;
            string LStrServerData012 = string.Empty;
            string LStrServerData013 = string.Empty;
            string LStrServerData014 = string.Empty;
            string LStrServerData015 = string.Empty;
            string LStrServerData016 = string.Empty;
            string LStrServerData017 = string.Empty;
            string LStrServerData018 = string.Empty;

            IDataTableDecServer = ADataSetSource.Tables[0];
            ListViewDECServerList.Items.Clear();

            DataRow[] LDataRowCurrentLevelType = App.IDataTable00010.Select("C001 = 212");
            LStrIconPngName = LDataRowCurrentLevelType[0]["C005"].ToString();

            DataRow[] LDataRowDECServer = IDataTableDecServer.Select("C001 >= 2120000000000000001 AND C001 < 2130000000000000000 AND C011 = '1'", "C012 ASC");
            foreach (DataRow LDataRowSingleDecServer in LDataRowDECServer)
            {
                LStrServerData001 = LDataRowSingleDecServer["C001"].ToString();
                LStrServerData011 = LDataRowSingleDecServer["C011"].ToString();
                LStrServerData012 = LDataRowSingleDecServer["C012"].ToString();
                LStrServerData013 = LDataRowSingleDecServer["C013"].ToString();
                LStrServerData014 = LDataRowSingleDecServer["C014"].ToString();
                LStrServerData014 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData014, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrServerData015 = LDataRowSingleDecServer["C015"].ToString();
                LStrServerData015 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData015, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrServerData016 = LDataRowSingleDecServer["C016"].ToString();
                LStrServerData016 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData016, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LStrServerData017 = LDataRowSingleDecServer["C017"].ToString();
                LStrServerData017 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData017, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrServerData018 = LDataRowSingleDecServer["C018"].ToString();
                LStrServerData018 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData018, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                ListViewItem LListViewItemSingleDecServer = new ListViewItem();
                LListViewItemSingleDecServer.Content = new DECServerListColumnDefine(long.Parse(LStrServerData001), IPageTopParent.IStrImageFolder + @"\" + LStrIconPngName, LStrServerData017, LStrServerData014, LStrServerData018, UInt16.Parse(LStrServerData015));
                LListViewItemSingleDecServer.MouseDoubleClick += LListViewItemSingleDecServer_MouseDoubleClick;
                LListViewItemSingleDecServer.Height = 26;
                LListViewItemSingleDecServer.Tag = LDataRowSingleDecServer;
                LListViewItemSingleDecServer.DataContext = LStrServerData001;
                ListViewDECServerList.Items.Add(LListViewItemSingleDecServer);
            }
            ListViewDECServerList.SelectionChanged += ListViewDECServerList_SelectionChanged;
            if (ListViewDECServerList.Items.Count > 0) { ListViewDECServerList.SelectedIndex = 0; }
        }

        private void ListViewDECServerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void LListViewItemSingleDecServer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IOperationEvent != null)
            {
                ListViewItem LListViewItemClicked = sender as ListViewItem;
                if (LListViewItemClicked == null) { return; }
                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrObjectTag = "M-DEC01";
                LEventArgs.ObjectSource0 = LListViewItemClicked.DataContext.ToString();
                IOperationEvent(this, LEventArgs);
            }
        }
    }
}
