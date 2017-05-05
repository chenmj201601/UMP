using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Encryptions;

namespace UMPEncryptionDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private ObservableCollection<EncryptionTypeItem> mListEncrytionTypes;
        private ObservableCollection<EncryptionModeItem> mListEncryptionModes;
        private ObservableCollection<EncryptionEncodingItem> mListEncryptionEncodingItems;

        public MainWindow()
        {
            InitializeComponent();

            mListEncrytionTypes = new ObservableCollection<EncryptionTypeItem>();
            mListEncryptionModes = new ObservableCollection<EncryptionModeItem>();
            mListEncryptionEncodingItems = new ObservableCollection<EncryptionEncodingItem>();

            Loaded += MainWindow_Loaded;
            BtnTest.Click += BtnTest_Click;
            BtnEncrypt.Click += BtnEncrypt_Click;
            BtnDescrypt.Click += BtnDescrypt_Click;
            BtnGetPassword.Click += BtnGetPassword_Click;
            BtnGetOpt006.Click += BtnGetOpt006_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitEncryptionTypeItems();
            InitEncryptionMode();
            InitEncryptionEncodingItems();

            ListBoxType.ItemsSource = mListEncrytionTypes;
            ComboMode.ItemsSource = mListEncryptionModes;
            ListBoxEncoding.ItemsSource = mListEncryptionEncodingItems;

            foreach (var item in mListEncrytionTypes)
            {
                if (item.Value == 0)
                {
                    item.IsSelected = true;
                }
            }
            foreach (var item in ComboMode.Items)
            {
                var temp = item as EncryptionModeItem;
                if (temp != null && temp.Value == (int)EncryptionMode.AES256V02Hex)
                {
                    ComboMode.SelectedItem = item;
                }
            }
            foreach (var item in mListEncryptionEncodingItems)
            {
                if (item.Encoding == Encoding.Unicode)
                {
                    item.IsSelected = true;
                }
            }
        }


        #region Init Load

        private void InitEncryptionTypeItems()
        {
            mListEncrytionTypes.Clear();
            EncryptionTypeItem item = new EncryptionTypeItem();
            item.Name = "Server";
            item.Value = 0;
            item.IsSelected = false;
            mListEncrytionTypes.Add(item);
            item = new EncryptionTypeItem();
            item.Name = "Client";
            item.Value = 1;
            item.IsSelected = false;
            mListEncrytionTypes.Add(item);
        }

        private void InitEncryptionMode()
        {
            mListEncryptionModes.Clear();
            string[] names = Enum.GetNames(typeof(EncryptionMode));
            for (int i = 0; i < names.Length; i++)
            {
                EncryptionModeItem item = new EncryptionModeItem();
                item.Name = names[i];
                item.Value = (int)Enum.Parse(typeof(EncryptionMode), names[i]);
                item.Mode = (EncryptionMode)Enum.Parse(typeof(EncryptionMode), names[i]);
                mListEncryptionModes.Add(item);
            }
        }

        private void InitEncryptionEncodingItems()
        {
            mListEncryptionEncodingItems.Clear();
            EncryptionEncodingItem item = new EncryptionEncodingItem();
            item.Name = "Unicode";
            item.Encoding = Encoding.Unicode;
            item.IsSelected = false;
            mListEncryptionEncodingItems.Add(item);
            item = new EncryptionEncodingItem();
            item.Name = "Ascii";
            item.Encoding = Encoding.ASCII;
            item.IsSelected = false;
            mListEncryptionEncodingItems.Add(item);
            item = new EncryptionEncodingItem();
            item.Name = "UTF8";
            item.Encoding = Encoding.UTF8;
            item.IsSelected = false;
            mListEncryptionEncodingItems.Add(item);
        }

        #endregion


        #region EventHandlers

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strOptID = TxtSource.Text;
                long optID = long.Parse(strOptID);
                long licID = optID + 1000000000;
                string strDatetime = DateTime.Parse("2015-01-01").ToString("yyyy/MM/dd HH:mm:ss");
                string str = string.Format("{0}{1}{2}{1}N", licID, ConstValue.SPLITER_CHAR, strDatetime);
                str = ServerAESEncryption.EncryptString(str, EncryptionMode.AES256V02Hex);
                AppendMessage(str);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var typeItem = ListBoxType.SelectedItem as EncryptionTypeItem;
                var modeItem = ComboMode.SelectedItem as EncryptionModeItem;
                var encItem = ListBoxEncoding.SelectedItem as EncryptionEncodingItem;
                if (typeItem == null || modeItem == null || encItem == null) { return; }

                string strSource = TxtSource.Text;
                string strReturn = string.Empty;
                //string strTemp;

                int mode = (int)modeItem.Mode;
                int type = mode / 1000;
                Encoding encoding = encItem.Encoding;
                switch (type)
                {
                    case 1:
                        //do
                        //{
                        //    if (strSource.Length > 128)
                        //    {
                        //        strTemp = strSource.Substring(0, 128);
                        //        strSource = strSource.Substring(128, strSource.Length - 128);
                        //    }
                        //    else
                        //    {
                        //        strTemp = strSource;
                        //        strSource = string.Empty;
                        //    }
                        //    if (typeItem.Value == 1)
                        //    {
                        //        strReturn += ClientAESEncryption.EncryptString(strTemp, modeItem.Mode, encoding);
                        //    }
                        //    else
                        //    {
                        //        strReturn += ServerAESEncryption.EncryptString(strTemp, modeItem.Mode, encoding);
                        //    }

                        //} while (strSource.Length > 0);
                        if (typeItem.Value == 1)
                        {
                            strReturn += ClientAESEncryption.EncryptString(strSource, modeItem.Mode, encoding);
                        }
                        else
                        {
                            strReturn += ServerAESEncryption.EncryptString(strSource, modeItem.Mode, encoding);
                        }
                        break;
                    case 2:
                    case 3:
                        if (typeItem.Value == 1)
                        {
                            strReturn += ClientHashEncryption.EncryptString(strSource, modeItem.Mode, encoding);
                        }
                        else
                        {
                            strReturn += ServerHashEncryption.EncryptString(strSource, modeItem.Mode, encoding);
                        }
                        break;
                    default:
                        ShowErrorMessage(string.Format("EncryptMode invalid.\t{0}", modeItem.Mode));
                        return;
                }
                AppendMessage(strReturn);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnDescrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var typeItem = ListBoxType.SelectedItem as EncryptionTypeItem;
                var modeItem = ComboMode.SelectedItem as EncryptionModeItem;
                var encItem = ListBoxEncoding.SelectedItem as EncryptionEncodingItem;
                if (typeItem == null || modeItem == null || encItem == null) { return; }

                string strSource = TxtSource.Text;
                string strReturn = string.Empty;
                //string strTemp;

                int mode = (int)modeItem.Mode;
                int type = mode / 1000;
                Encoding encoding = encItem.Encoding;
                switch (type)
                {
                    case 1:
                        //do
                        //{
                        //    if (strSource.Length > 128)
                        //    {
                        //        strTemp = strSource.Substring(0, 128);
                        //        strSource = strSource.Substring(128, strSource.Length - 128);
                        //    }
                        //    else
                        //    {
                        //        strTemp = strSource;
                        //        strSource = string.Empty;
                        //    }
                        //    if (typeItem.Value == 1)
                        //    {
                        //        strReturn += ClientAESEncryption.DecryptString(strTemp, modeItem.Mode, encoding);
                        //    }
                        //    else
                        //    {
                        //        strReturn += ServerAESEncryption.DecryptString(strTemp, modeItem.Mode, encoding);
                        //    }

                        //} while (strSource.Length > 0);
                        if (typeItem.Value == 1)
                        {
                            strReturn = ClientAESEncryption.DecryptString(strSource, modeItem.Mode, encoding);
                        }
                        else
                        {
                            strReturn += ServerAESEncryption.DecryptString(strSource, modeItem.Mode, encoding);
                        }
                        break;
                    default:
                        ShowErrorMessage(string.Format("EncryptMode invalid.\t{0}", modeItem.Mode));
                        return;
                }
                AppendMessage(strReturn);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnGetPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var typeItem = ListBoxType.SelectedItem as EncryptionTypeItem;
                var modeItem = ComboMode.SelectedItem as EncryptionModeItem;
                if (typeItem == null || modeItem == null) { return; }

                string strSource = TxtSource.Text;
                string strID = TxtID.Text;
                if (string.IsNullOrEmpty(strSource)
                    || string.IsNullOrEmpty(strID))
                {
                    ShowErrorMessage(string.Format("Password or ID empty"));
                    return;
                }
                strSource = strID + strSource;
                byte[] temp = ServerHashEncryption.EncryptBytes(Encoding.Unicode.GetBytes(strSource),
                    EncryptionMode.SHA512V00Hex);
                var aes = ServerAESEncryption.EncryptBytes(temp, EncryptionMode.AES256V02Hex);
                AppendMessage(string.Format("{0}", ServerEncryptionUtils.Byte2Hex(aes)));
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnGetOpt006_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strOptID = TxtSource.Text;
                long optID = long.Parse(strOptID);
                long licID = optID + 1000000000;
                string strDatetime = DateTime.Parse("2015-01-01").ToString("yyyy/MM/dd HH:mm:ss");
                string str = string.Format("{0}{1}{2}{1}N", licID, ConstValue.SPLITER_CHAR, strDatetime);
                str = ServerAESEncryption.EncryptString(str, EncryptionMode.AES256V02Hex);
                AppendMessage(str);
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        #endregion


        #region Others

        private void ShowErrorMessage(string msg)
        {
            MessageBox.Show(string.Format("{0}", msg), "Demo", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void AppendMessage(string msg)
        {
            TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), msg));
            TxtMsg.ScrollToEnd();
        }

        #endregion

    }
}
