using System;
using System.Collections.ObjectModel;
using System.Windows;
using VoiceCyber.UMP.Common;

namespace EncryptionDecryptionDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private ObservableCollection<EncryptionTypeItem> mListEncrytionTypes;
        private ObservableCollection<EncryptionModeItem> mListEncryptionModes;

        public MainWindow()
        {
            InitializeComponent();

            mListEncrytionTypes = new ObservableCollection<EncryptionTypeItem>();
            mListEncryptionModes = new ObservableCollection<EncryptionModeItem>();

            Loaded += MainWindow_Loaded;
            BtnTest.Click += BtnTest_Click;
            BtnEncrypt.Click += BtnEncrypt_Click;
            BtnDescrypt.Click += BtnDescrypt_Click;
            BtnGetPassword.Click += BtnGetPassword_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitEncryptionTypeItems();
            InitEncryptionMode();

            ListBoxType.ItemsSource = mListEncrytionTypes;
            ComboMode.ItemsSource = mListEncryptionModes;

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

        #endregion


        #region EventHandlers

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {

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
                if (typeItem == null || modeItem == null) { return; }

                string code = CreateVerificationCode(typeItem, modeItem, true);
                int intMode = modeItem.Value;
                int intVersion = 0;
                int intType = 0;
                switch (intMode)
                {
                    case (int)EncryptionMode.AES256V01Hex:
                        intVersion = 1;
                        intType = 1;
                        break;
                    case (int)EncryptionMode.SHA256V01Hex:
                        intVersion = 1;
                        intType = 2;
                        break;
                    case (int)EncryptionMode.SHA512V01Hex:
                        intVersion = 1;
                        intType = 3;
                        break;
                    case (int)EncryptionMode.AES256V02Hex:
                        intVersion = 2;
                        intType = 1;
                        break;
                    case (int)EncryptionMode.SHA256V02Hex:
                        intVersion = 2;
                        intType = 2;
                        break;
                    case (int)EncryptionMode.SHA512V02Hex:
                        intVersion = 2;
                        intType = 3;
                        break;
                    case (int)EncryptionMode.AES256V04Hex:
                        intVersion = 4;
                        intType = 1;
                        break;
                    case (int)EncryptionMode.SHA256V04Hex:
                        intVersion = 4;
                        intType = 2;
                        break;
                    case (int)EncryptionMode.SHA512V04Hex:
                        intVersion = 4;
                        intType = 3;
                        break;
                }
                string strSource = TxtSource.Text;
                string strReturn = string.Empty;
                string strTemp;
                if (intType == 1)
                {
                    do
                    {
                        if (strSource.Length > 128)
                        {
                            strTemp = strSource.Substring(0, 128);
                            strSource = strSource.Substring(128, strSource.Length - 128);
                        }
                        else
                        {
                            strTemp = strSource;
                            strSource = string.Empty;
                        }
                        if (typeItem.Value == 1)
                        {
                            strReturn += PFShareClassesC.EncryptionAndDecryption.EncryptDecryptString(strTemp, code,
                            (PFShareClassesC.EncryptionAndDecryption.UMPKeyAndIVType)intVersion);
                        }
                        else
                        {
                            strReturn += PFShareClassesS.EncryptionAndDecryption.EncryptDecryptString(strTemp, code,
                               (PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType)intVersion);
                        }

                    } while (strSource.Length > 0);
                }
                if (intType == 2)
                {
                    if (typeItem.Value == 0)
                    {
                        strReturn += PFShareClassesS.EncryptionAndDecryption.EncryptStringSHA256(strSource);
                    }
                }
                if (intType == 3)
                {
                    if (typeItem.Value == 0)
                    {
                        strReturn += PFShareClassesS.EncryptionAndDecryption.EncryptStringSHA512(strSource, code,
                         (PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType)intVersion);
                    }
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
                if (typeItem == null || modeItem == null) { return; }

                string code = CreateVerificationCode(typeItem, modeItem, false);
                int intMode = modeItem.Value;
                int intVersion = 0;
                int intType = 0;
                switch (intMode)
                {
                    case (int)EncryptionMode.AES256V01Hex:
                        intVersion = 1;
                        intType = 1;
                        break;
                    case (int)EncryptionMode.SHA256V01Hex:
                        intVersion = 1;
                        intType = 2;
                        break;
                    case (int)EncryptionMode.SHA512V01Hex:
                        intVersion = 1;
                        intType = 3;
                        break;
                    case (int)EncryptionMode.AES256V02Hex:
                        intVersion = 2;
                        intType = 1;
                        break;
                    case (int)EncryptionMode.SHA256V02Hex:
                        intVersion = 2;
                        intType = 2;
                        break;
                    case (int)EncryptionMode.SHA512V02Hex:
                        intVersion = 2;
                        intType = 3;
                        break;
                    case (int)EncryptionMode.AES256V04Hex:
                        intVersion = 4;
                        intType = 1;
                        break;
                    case (int)EncryptionMode.AES256V25Hex:
                        intVersion = 25;
                        intType = 1;
                        break;
                    case (int)EncryptionMode.SHA256V04Hex:
                        intVersion = 4;
                        intType = 2;
                        break;
                    case (int)EncryptionMode.SHA512V04Hex:
                        intVersion = 4;
                        intType = 3;
                        break;
                }
                intVersion = 100 + intVersion;
                string strSource = TxtSource.Text;
                string strReturn = string.Empty;
                string strTemp;
                if (intType == 1)
                {
                    do
                    {
                        if (strSource.Length > 512)
                        {
                            strTemp = strSource.Substring(0, 512);
                            strSource = strSource.Substring(512, strSource.Length - 512);
                        }
                        else
                        {
                            strTemp = strSource;
                            strSource = string.Empty;
                        }
                        if (typeItem.Value == 1)
                        {
                            strReturn += PFShareClassesC.EncryptionAndDecryption.EncryptDecryptString(strTemp, code,
                            (PFShareClassesC.EncryptionAndDecryption.UMPKeyAndIVType)intVersion);
                        }
                        else
                        {
                            strReturn += PFShareClassesS.EncryptionAndDecryption.EncryptDecryptString(strTemp, code,
                               (PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType)intVersion);
                        }

                    } while (strSource.Length > 0);
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
                //获取密码加密串
                string strSource = TxtSource.Text;
                if (string.IsNullOrEmpty(strSource))
                {
                    ShowErrorMessage(string.Format("Source empty"));
                    return;
                }
                string strID = TxtID.Text;
                if (string.IsNullOrEmpty(strID))
                {
                    ShowErrorMessage(string.Format("ID empty"));
                    return;
                }
                strSource = strID + strSource;
                string code = CreateVerificationCode(1, 0, true);
                string strReturn = PFShareClassesS.EncryptionAndDecryption.EncryptStringSHA512(strSource, code,
                     (PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType)1);
                AppendMessage(strReturn);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion


        #region Others

        private string CreateVerificationCode(EncryptionTypeItem encryptionType, EncryptionModeItem modeItem, bool isEncrypt)
        {
            int intType = encryptionType.Value;
            int intMode = modeItem.Value;
            int intVersion = 0;
            switch (intMode)
            {
                case (int)EncryptionMode.AES256V01Hex:
                case (int)EncryptionMode.SHA256V01Hex:
                case (int)EncryptionMode.SHA512V01Hex:
                    intVersion = 1;
                    break;
                case (int)EncryptionMode.AES256V02Hex:
                case (int)EncryptionMode.SHA256V02Hex:
                case (int)EncryptionMode.SHA512V02Hex:
                    intVersion = 2;
                    break;
                case (int)EncryptionMode.AES256V04Hex:
                case (int)EncryptionMode.SHA256V04Hex:
                case (int)EncryptionMode.SHA512V04Hex:
                    intVersion = 4;
                    break;
                case (int)EncryptionMode.AES256V25Hex:
                    intVersion = 25;
                    break;
            }
            return CreateVerificationCode(intVersion, intType, isEncrypt);
        }

        private string CreateVerificationCode(int intVersion, int intType, bool isEncrypt)
        {
            string strMode;
            if (isEncrypt)
            {
                strMode = intVersion.ToString("000");
            }
            else
            {
                strMode = (100 + intVersion).ToString("000");
            }
            string strReturn;
            int intRandom;
            string strRandom;
            try
            {
                Random random = new Random();
                strReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                intRandom = random.Next(0, 14);
                strRandom = intRandom.ToString("00");
                strReturn = strReturn.Insert(intRandom, "VCT");
                intRandom = random.Next(0, 17);
                strRandom += intRandom.ToString("00");
                strReturn = strReturn.Insert(intRandom, "UMP");
                intRandom = random.Next(0, 20);
                strRandom += intRandom.ToString("00");
                strReturn = strReturn.Insert(intRandom, strMode);
                if (intType == 1)
                {
                    strReturn = PFShareClassesC.EncryptionAndDecryption.EncryptStringY(strRandom + strReturn);
                }
                else
                {
                    strReturn = PFShareClassesS.EncryptionAndDecryption.EncryptStringY(strRandom + strReturn);
                }
            }
            catch (Exception ex)
            {
                strReturn = string.Empty;
                ShowErrorMessage(ex.Message);
            }
            return strReturn;
        }

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
