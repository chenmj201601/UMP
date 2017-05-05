using System;
using System.IO;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace Wcf11012
{
    public partial class Service11012
    {
        public WebReturn UploadOperation(UploadRequest uploadRequest)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Result = true;
            webReturn.Code = 0;
            if (uploadRequest == null)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_PARAM_INVALID;
                webReturn.Message = string.Format("WebRequest is null");
                return webReturn;
            }
            SessionInfo session = uploadRequest.Session;
            if (session == null)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_PARAM_INVALID;
                webReturn.Message = string.Format("SessionInfo is null");
                return webReturn;
            }
            webReturn.Session = session;
            try
            {
                OperationReturn optReturn = new OperationReturn();
                DatabaseInfo dbInfo = session.DatabaseInfo;
                if (dbInfo != null)
                {
                    dbInfo.RealPassword = DecryptString04(dbInfo.Password);
                    session.DBConnectionString = dbInfo.GetConnectionString();
                }
                switch (uploadRequest.Code)
                {
                    case (int)RequestCode.WSUploadFile:
                        optReturn = UploadFile(session, uploadRequest);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    default:
                        webReturn.Result = false;
                        webReturn.Code = Defines.RET_PARAM_INVALID;
                        webReturn.Message = string.Format("Request code invalid.\t{0}", uploadRequest.Code);
                        return webReturn;
                }
                webReturn.Message = optReturn.Message;
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
                return webReturn;
            }
            return webReturn;
        }

        private OperationReturn UploadFile(SessionInfo session, UploadRequest uploadRequest)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {

                /*
                 * ListData0：文件长度（必须），也就是 byte[] 长度
                 * ListData1：ModuleID，所属模块4位编号（必须）
                 * ListData2：Method，0：直接上传到UploadFiles目录；1：上传到MediaData临时目录
                 * ListData3：文件名（可选），如果没有指定，将自动生成文件名（通常用GUID）
                 * ListData4：扩展名（可选），参考文件类型
                 * ListData5：文件保存位置（可选），如果文件需要保存到子目录里，这里需要指定子目录
                 * ListData6：文件类型（可选），通常根据文件名的扩展名确定文件类型
                 * ListData7：（可选）如果文件已经存在，是否替换，默认0 不替换，返回错误；1：替换
                 * 
                 */

                var listParams = uploadRequest.ListData;
                if (listParams == null
                    || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }

               
                #region 校验参数

                string strLength = listParams[0];
                string strModuleID = listParams[1];
                string strMethod = listParams[2];
                int intLength;
                if (!int.TryParse(strLength, out intLength)
                    || intLength < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("FileLenght param invalid.");
                    return optReturn;
                }
                int moduleID;
                if (!int.TryParse(strModuleID, out moduleID)
                    || moduleID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ModuleID param invalid.");
                    return optReturn;
                }
                int intMethod;
                if (!int.TryParse(strMethod, out intMethod)
                    || intMethod < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method param invalid.");
                    return optReturn;
                }
                if (uploadRequest.Content == null
                    || uploadRequest.Content.Length < intLength)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Content length invalid.");
                    return optReturn;
                }


                #region 可选参数

                string strFileName = string.Empty;
                if (listParams.Count > 3)
                {
                    strFileName = listParams[3];
                }
                if (string.IsNullOrEmpty(strFileName))
                {
                    strFileName = string.Format("{0}", Guid.NewGuid());     //默认文件名
                }
                string strExtension = "ump";        //默认使用.ump作为文件名
                if (listParams.Count > 4)
                {
                    strExtension = listParams[4];
                }
                string strSavePath = string.Empty;
                if (listParams.Count > 5)
                {
                    strSavePath = listParams[5];
                }
                string strFileType = "0";
                if (listParams.Count > 6)
                {
                    strFileType = listParams[6];
                }
                int intFileType;
                if (!int.TryParse(strFileType, out intFileType)
                    || intFileType < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("FileType param invalid.");
                    return optReturn;
                }
                string strReplaceMode = "0";
                if (listParams.Count > 7)
                {
                    strReplaceMode = listParams[7];
                }
                int intReplaceMode;
                if (!int.TryParse(strReplaceMode, out intReplaceMode))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ReplaceMode param invalid.");
                    return optReturn;
                }

                #endregion


                #endregion


                #region 生成上传的文件

                string rootDir = AppDomain.CurrentDomain.BaseDirectory;
                rootDir = rootDir.Substring(0, rootDir.LastIndexOf("\\"));
                rootDir = rootDir.Substring(0, rootDir.LastIndexOf("\\"));
                string strTargetDir;
                if (intMethod == 1)
                {
                    strTargetDir = Path.Combine(rootDir, ConstValue.TEMP_DIR_MEDIADATA);
                }
                else
                {
                    strTargetDir = Path.Combine(rootDir, ConstValue.TEMP_DIR_UPLOADFILES);
                    strTargetDir = Path.Combine(strTargetDir, string.Format("UMPS{0}", moduleID.ToString("0000")));
                    strTargetDir = Path.Combine(strTargetDir, strSavePath);
                }
                if (!Directory.Exists(strTargetDir))
                {
                    Directory.CreateDirectory(strTargetDir);
                }
                string strTargetName = string.Format("{0}.{1}", strFileName, strExtension);
                string strTarget = Path.Combine(strTargetDir, strTargetName);
                FileStream fs = File.Create(strTarget);
                fs.Write(uploadRequest.Content, 0, intLength);
                fs.Flush();
                fs.Close();
                fs.Dispose();
                optReturn.Data = strTargetName;

                #endregion

            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }
    }
}