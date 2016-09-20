using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UMPCommon;

namespace UMPSftp
{
    class Parameters
    {
        const int Index_Vox = 0;
        const int Index_Vls = 1;
        const int Index_Pcm = 2;
        protected Parameters() { }
        private static Parameters _parameters;

        string _public_key_file = "UMP.SSL.Certificate.pfx";
        string _password = "VoiceCyber,123";
        string _subject = "CN=VoiceCyber.PF";

        public class SftpParams
        {
            public string AuthenticateServerHost { get; set; }
            public int AuthenticateServerPort { get; set; }
            public string RootDir { get; set; }
            public int FTP_Port { get; set; }
            public string ScrDriver { get; set; }
            public string VoxDriver { get; set; }
        }

        SftpParams _sftp_param = new SftpParams();
        public SftpParams SftpParam { get { return _sftp_param; } }

        public string KeyFilePath { get { return _public_key_file; } }
        public string Password { get { return _password; } }
        public string Subject { get { return _subject; } }



        public static Parameters Instance()
        {
            if (_parameters == null)
            {
                _parameters = new Parameters();
            }
            return _parameters;
        }

        public bool ReadConfig()
        {
            //使用局部变量读取参数，在全部成功读取后更新到类成员变量中，防止更新参数时失败导致原有参数丢失
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                path += "\\voicecyber\\UMP\\config\\umpparam_simp.xml";
                XmlDocument xml_doc = new XmlDocument();
                xml_doc.Load(path);
                XmlNode node = xml_doc.SelectSingleNode("Configurations").SelectSingleNode("Configuration");
                string site_id = node.SelectSingleNode("LocalMachine").Attributes["SiteID"].Value;
                string sftp_number = node.SelectSingleNode("LocalMachine").Attributes["SFTPNumber"].Value;
                string scr_number = node.SelectSingleNode("LocalMachine").Attributes["ScreenSvrNumber"].Value;
                string vox_number = node.SelectSingleNode("LocalMachine").Attributes["VoiceModuleNumber"].Value;
                string xml_path = string.Format("/Configurations/Configuration/Sites/Site[@ID=\"{0}\"]/Resources/SFTPs/SFTP[@Key=\"{1}\"]", site_id, sftp_number);
                XmlNode sftp_node = xml_doc.SelectSingleNode(xml_path);
                if (sftp_node == null)
                {
                    return false;
                }
                SftpParams tmpSftpParam = new SftpParams();
                tmpSftpParam.RootDir = sftp_node.Attributes["RootDir"].Value;
                tmpSftpParam.FTP_Port = Convert.ToInt16(EncryDecryHelper.DecryptionXMLString(sftp_node.SelectSingleNode("HostPort").Attributes["Value"].Value));

                //录音保存目录参数
                xml_path = string.Format("/Configurations/Configuration/Sites/Site[@ID=\"{0}\"]/VoiceServers/VoiceServer[@Key=\"{1}\"]", site_id, vox_number);
                XmlNode vox_node = xml_doc.SelectSingleNode(xml_path);
                if (vox_node != null)
                {
                    string driver_inx = vox_node.SelectSingleNode("StorageParamLocal").Attributes["DeviceIndex"].Value;
                    xml_path = string.Format("/Configurations/Configuration/Sites/Site[@ID=\"{0}\"]/Resources/StorageDevices/Device[@Key=\"{1}\"]", site_id, driver_inx);
                    XmlNode device_node = xml_doc.SelectSingleNode(xml_path);
                    if (device_node != null)
                    {
                        tmpSftpParam.VoxDriver = device_node.Attributes["Driver"].Value;
                    }
                }

                //录屏保存目录参数
                xml_path = string.Format("/Configurations/Configuration/Sites/Site[@ID=\"{0}\"]/ScreenSvrs/ScreenSvr[@Key=\"{1}\"]", site_id, scr_number);
                XmlNode scr_node = xml_doc.SelectSingleNode(xml_path);
                if (scr_node != null)
                {
                    string driver_inx = scr_node.SelectSingleNode("ParamPath").SelectSingleNode("LocalPathFormat").Attributes["LocalIndex"].Value;
                    xml_path = string.Format("/Configurations/Configuration/Sites/Site[@ID=\"{0}\"]/Resources/StorageDevices/Device[@Key=\"{1}\"]", site_id, driver_inx);
                    XmlNode device_node = xml_doc.SelectSingleNode(xml_path);
                    if (device_node != null)
                    {
                        tmpSftpParam.ScrDriver = device_node.Attributes["Driver"].Value;
                    }
                }
                //验证服务器IP
                xml_path = string.Format("/Configurations/Configuration/Sites/Site[@ID=\"{0}\"]/Resources/AuthenticateServers/AuthenticateServer", site_id);
                XmlNode authen_node = xml_doc.SelectSingleNode(xml_path);
                if (authen_node != null)
                {
                    string sValue = authen_node.SelectSingleNode("HostAddress").Attributes["Value"].Value;
                    tmpSftpParam.AuthenticateServerHost = EncryDecryHelper.DecryptionXMLString(sValue);
                    sValue = authen_node.SelectSingleNode("HostPort").Attributes["Value"].Value;
                    tmpSftpParam.AuthenticateServerPort = Convert.ToInt16(EncryDecryHelper.DecryptionXMLString(sValue));
                }
                _sftp_param = tmpSftpParam;
            }
            catch (System.Xml.XmlException ex)
            {
                LogHelper.FatalLog(ex);
            }
            catch (System.Exception ex)
            {
                LogHelper.FatalLog(ex);
            }
            LogHelper.InfoLog(string.Format("Sftp port:{0},RootDir:{1},VoxDir:{2},ScrDir:{2};", _sftp_param.FTP_Port,
                _sftp_param.RootDir, _sftp_param.VoxDriver, _sftp_param.ScrDriver));

            return true;
        }
    }
}
