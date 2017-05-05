using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common3106
{
    /// <summary>
    /// WCF 相关
    /// </summary>
    public enum S3106Codes
    {
        /// <summary>
        /// 获取权限
        /// </summary>
        GetUserOperationList=0,
        GetControlAgentInfoList = 1,
        GetControlOrgInfoList = 2,
        GetQA = 3,
        /// <summary>
        /// 文件夹操作（新建、重命名）
        /// </summary>
        FolderDBO=4,
        /// <summary>
        /// 删除文件夹、文件
        /// </summary>
        DeleteFolder=5,
        /// <summary>
        /// 分配文件夹访问权限
        /// </summary>
        AllotAgent=6,
        /// <summary>
        /// 上传文件
        /// </summary>
        UploadFile=7,
        /// <summary>
        /// 获取_58下的文件夹信息
        /// </summary>
        GetFolder=8,
        /// <summary>
        /// 获取文件信息
        /// </summary>
        GetFiles=9,
        /// <summary>
        /// 上传文件到磁盘
        /// </summary>
        UploadToDisk=10,
        /// <summary>
        /// 获取查询上传的录音信息
        /// </summary>
        GetRecordInfo=11,
        /// <summary>
        /// 获取UMP安装路径的根目录
        /// </summary>
        GetUMPSetupPath=12,
        /// <summary>
        /// 写教材浏览记录
        /// </summary>
        WriteBrowseHistory=13,
        /// <summary>
        /// 获取教材浏览记录
        /// </summary>
        GetBrowseHistory=14,
    }
}
