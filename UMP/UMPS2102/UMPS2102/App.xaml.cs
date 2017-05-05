using System;
using System.Windows;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Encryptions;

namespace UMPS2102
{ 
    /// <summary>
    /// UMP.xaml 的交互逻辑
    /// </summary>
    public partial class App 
    {

        #region Memebers

        public static UMPApp CurrentApp;

        #endregion


        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentApp = new S2102App(false);
            CurrentApp.Startup();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (CurrentApp != null)
            {
                CurrentApp.Exit();
            }
            base.OnExit(e);
        }


        #region Encryption and Decryption

        public static string EncryptString(string strSource)
        {
            try
            {
                return ClientAESEncryption.EncryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("Encryption", string.Format("Fail.\t{0}", ex.Message));
                return strSource;
            }
        }

        public static string DecryptString(string strSource)
        {
            try
            {
                return ClientAESEncryption.DecryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("Encryption", string.Format("Fail.\t{0}", ex.Message));
                return strSource;
            }
        }

        #endregion

    }
}
