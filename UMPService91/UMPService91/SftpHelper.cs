//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    3c336378-25d1-4db9-bf6b-95a1ba6cfe25
//        CLR Version:              4.0.30319.42000
//        Name:                     SftpHelper
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPService91
//        File Name:                SftpHelper
//
//        Created by Charley at 2016/8/18 11:01:00
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using nsoftware.IPWorksSSH;
using VoiceCyber.Common;


namespace UMPService91
{
    /// <summary>
    /// 封装Sftp客户端，登录Sftp服务器上传下载文件
    /// 调用该类中的方法之前需要先配置参数（SftpHelperConfig）
    /// 可以通过两种方式使用该Helper类：静态方式和实例方式，其中实例方式在上传下载最后要调用Logout方法退出Sftp服务器
    /// 注意：使用此工具类下载或上传并不对服务器证书验证
    /// 注意：以实例方式上传下载文件，最后应调用Logout方法退出Sftp服务器
    /// Charley
    /// 2015/3/28
    /// </summary>
    public class SftpHelper
    {
        private Sftp mSftp;
        private string mName;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
        /// <summary>
        /// 创建一个Sftp帮助器
        /// </summary>
        public SftpHelper()
        {
            mName = string.Empty;
        }
        /// <summary>
        /// 登录到Sftp服务器
        /// </summary>
        /// <param name="config">Sftp参数</param>
        /// <returns></returns>
        public OperationReturn LogOn(SftpHelperConfig config)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mSftp != null)
                {
                    try
                    {
                        if (mSftp.Connected) { mSftp.Connected = false; }
                        mSftp = null;
                    }
                    catch { }
                }
                mSftp = new Sftp();
                mSftp.OnSSHServerAuthentication += (s, se) => se.Accept = true;
                mSftp.SSHHost = config.ServerHost;
                mSftp.SSHPort = config.ServerPort;
                mSftp.SSHUser = config.LoginUser;
                switch (config.AuthType)
                {
                    case 1:
                        mSftp.SSHAuthMode = SftpSSHAuthModes.amPassword;
                        mSftp.SSHPassword = config.LoginPassword;
                        break;
                    case 2:
                        mSftp.SSHAuthMode = SftpSSHAuthModes.amPublicKey;
                        mSftp.SSHCert = new Certificate(CertStoreTypes.cstPFXFile, config.CertFile, config.CertPassword, config.CertPassword);
                        break;
                    case 3:
                        mSftp.SSHAuthMode = SftpSSHAuthModes.amMultiFactor;
                        mSftp.SSHPassword = config.LoginPassword;
                        mSftp.SSHCert = new Certificate(CertStoreTypes.cstPFXFile, config.CertFile, config.CertPassword, config.CertSubject);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_CONFIG_INVALID;
                        optReturn.Message = string.Format("Auth type not support.\t{0}", config.AuthType);
                        return optReturn;
                }
                mSftp.SSHLogon(config.ServerHost, config.ServerPort);
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }
        /// <summary>
        /// 退出Sftp服务器
        /// </summary>
        public void LogOut()
        {
            try
            {
                if (mSftp != null)
                {
                    mSftp.Connected = false;
                    mSftp.Dispose();
                    mSftp = null;
                }
            }
            catch { }
        }
        /// <summary>
        /// 下载文件，下载结束后不会退出Sftp服务器
        /// </summary>
        /// <param name="config">Sftp参数</param>
        /// <returns></returns>
        public OperationReturn DownloadFile1(SftpHelperConfig config)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mSftp == null
                   || !mSftp.Connected)
                {
                    optReturn = LogOn(config);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                }
                if (mSftp == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Sftp is null");
                    return optReturn;
                }
                mSftp.RemoteFile = config.RemoteFile;
                mSftp.LocalFile = config.LocalFile;
                mSftp.Overwrite = true;
                mSftp.Download();
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }
        /// <summary>
        /// 上传文件，上传结束后不会退出Sftp服务器
        /// </summary>
        /// <param name="config">Sftp参数</param>
        /// <returns></returns>
        public OperationReturn UploadFile1(SftpHelperConfig config)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mSftp == null
                   || !mSftp.Connected)
                {
                    optReturn = LogOn(config);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                }
                if (mSftp == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Sftp is null");
                    return optReturn;
                }
                mSftp.RemoteFile = config.RemoteFile;
                mSftp.LocalFile = config.LocalFile;
                mSftp.Overwrite = true;
                mSftp.MakeDirectory(config.RemoteDirectory);
                mSftp.Upload();
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }
        /// <summary>
        /// 下载文件，在config中配置Sftp参数，下载结束后立马退出Sftp服务器
        /// </summary>
        /// <param name="config">Sftp参数</param>
        /// <returns></returns>
        public static OperationReturn DownloadFile(SftpHelperConfig config)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                Sftp sftp = new Sftp();
                sftp.OnSSHServerAuthentication += (s, se) => se.Accept = true;
                sftp.SSHHost = config.ServerHost;
                sftp.SSHPort = config.ServerPort;
                sftp.SSHUser = config.LoginUser;
                sftp.RemoteFile = config.RemoteFile;
                sftp.LocalFile = config.LocalFile;
                switch (config.AuthType)
                {
                    case 1:
                        sftp.SSHAuthMode = SftpSSHAuthModes.amPassword;
                        sftp.SSHPassword = config.LoginPassword;
                        break;
                    case 2:
                        sftp.SSHAuthMode = SftpSSHAuthModes.amPublicKey;
                        sftp.SSHCert = new Certificate(CertStoreTypes.cstPFXFile, config.CertFile, config.CertPassword, config.CertPassword);
                        break;
                    case 3:
                        sftp.SSHAuthMode = SftpSSHAuthModes.amMultiFactor;
                        sftp.SSHPassword = config.LoginPassword;
                        sftp.SSHCert = new Certificate(CertStoreTypes.cstPFXFile, config.CertFile, config.CertPassword, config.CertSubject);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_CONFIG_INVALID;
                        optReturn.Message = string.Format("Auth type not support.\t{0}", config.AuthType);
                        return optReturn;
                }
                sftp.Overwrite = true;
                sftp.Download();
                sftp.Connected = false;
                sftp.Dispose();
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }
        /// <summary>
        /// 上传文件，在config中配置Sftp参数，上传结束后立马退出Sftp服务器
        /// </summary>
        /// <param name="config">Sftp参数</param>
        /// <returns></returns>
        public static OperationReturn UploadFile(SftpHelperConfig config)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                Sftp sftp = new Sftp();
                sftp.OnSSHServerAuthentication += (s, se) => se.Accept = true;
                sftp.SSHHost = config.ServerHost;
                sftp.SSHPort = config.ServerPort;
                sftp.SSHUser = config.LoginUser;
                sftp.RemoteFile = config.RemoteFile;
                sftp.LocalFile = config.LocalFile;
                switch (config.AuthType)
                {
                    case 1:
                        sftp.SSHAuthMode = SftpSSHAuthModes.amPassword;
                        sftp.SSHPassword = config.LoginPassword;
                        break;
                    case 2:
                        sftp.SSHAuthMode = SftpSSHAuthModes.amPublicKey;
                        sftp.SSHCert = new Certificate(CertStoreTypes.cstPFXFile, config.CertFile, config.CertPassword, config.CertPassword);
                        break;
                    case 3:
                        sftp.SSHAuthMode = SftpSSHAuthModes.amMultiFactor;
                        sftp.SSHPassword = config.LoginPassword;
                        sftp.SSHCert = new Certificate(CertStoreTypes.cstPFXFile, config.CertFile, config.CertPassword, config.CertSubject);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_CONFIG_INVALID;
                        optReturn.Message = string.Format("Auth type not support.\t{0}", config.AuthType);
                        return optReturn;
                }
                sftp.Overwrite = true;
                sftp.MakeDirectory(config.RemoteDirectory);
                sftp.Upload();
                sftp.Connected = false;
                sftp.Dispose();
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }
    }
    /// <summary>
    /// Sftp参数配置
    /// </summary>
    public class SftpHelperConfig
    {
        /// <summary>
        /// Sftp服务器地址
        /// </summary>
        public string ServerHost { get; set; }
        /// <summary>
        /// Sftp服务器端口
        /// </summary>
        public int ServerPort { get; set; }
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string LoginUser { get; set; }
        /// <summary>
        /// 登录密码，只有AuthType=1||AuthType=3才有效
        /// </summary>
        public string LoginPassword { get; set; }
        /// <summary>
        /// 远程文件文件夹
        /// </summary>
        public string RemoteDirectory { get; set; }
        /// <summary>
        /// 远程文件路径
        /// </summary>
        public string RemoteFile { get; set; }
        /// <summary>
        /// 本地文件路径
        /// </summary>
        public string LocalFile { get; set; }
        /// <summary>
        /// AuthType
        /// 1       Password Authentication
        /// 2       Publickey   Authentication
        /// 3       Password+Publickey Authentication
        /// </summary>
        public int AuthType { get; set; }
        //以下三个参数只有AuthType=2||AuthType=3才有效
        /// <summary>
        /// 证书文件路径
        /// </summary>
        public string CertFile { get; set; }
        /// <summary>
        /// 证书主题
        /// </summary>
        public string CertSubject { get; set; }
        /// <summary>
        /// 证书文件密码
        /// </summary>
        public string CertPassword { get; set; }
    }
}
