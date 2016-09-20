using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common5100
{
    public enum S5100RequestCode
    {
        #region 标签等级的wcf操作
        AddBookmarkLevel=5101001,
        ModifyBookmarkLevel=5101002,
        GetAllBookmarkLevels=5101003,
        SetBookmarkLevelStatus=5101004,
        DeleteBookmarkLevel=5101005,
        GetAllKeyWorlds=5102001,
        AddKeyWorld=5102002,
        ModifyKeyWorld=5102003,
        DeleteKeyWorld=5102004,
        #endregion
    }

    public enum S5100WcfErrorCode
    {
        #region 标签等级的wcf操作
        //获得流水号错误
        GetSerialIDError=5101901,
        //流水号转换long型错误
        SerialIDConvertError=5101902,
        //添加标签等级错误（在sql语句处执行出错）
        AddBookmarkLevelError=5101903,
        //获得所有标签等级出错（在sql语句处执行出错）
        GetAllBookmarkLevelError=5101904,
        //添加关键词出错
        GetAllKeyWorldsError=5102901,
        //添加关键词出出现异常
        GenerateKeyWorldXmlException=5102902,
        //创建xml时出错    创建关键词xml时出现错误
        CreateXmlError=5102903,
        //上传关键词xml出现异常
        UploadKeyWorldXmlException=5102904,
        //获得Pcm设备ID出现错误
        GetPCMDeviceIDError=5102905,
        //Pcm設備ID为空，请在”资源管理”中配置
        PCMDeviceIsNull=5102906,
        //Pcm設備ID出现错误，请确认您的配置正确
        PCMDeviceIDIsError=5102907,
        //获得存储设备错误（在sql语句处执行出错）    获得Pcm设备ID出现异常
        GetStorageDeviceError=5102908,
        //找不到对应的存储设备 或存储设备的属性不全
        CanNotFindStorageDevice=5102909,
        // 上传文件失败
        UploadKeyWorldXmlFailed=5102910,
        //上传档出现异常
        UploadKeyWorldXmlToShareException=5102911,
        ConnectCancelError=5102912,
        //将文件写入共享区域出错
        WriteFileError=5102913,
        ModifyBookmarkLevelException=5101905,
        ModifyBookmarkLevelStatusException=5101906,
        //没有可用的语言分析设备
        CanNotFindEnabledMachine=5102914,
        #endregion
    }

    /// <summary>
    /// 属性加密模式
    /// </summary>
    public enum ObjectPropertyEncryptMode
    {
        /// <summary>
        /// 未知(不加密)
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 加密版本2，模式hex，AES加密
        /// </summary>
        E2Hex = 21,
        /// <summary>
        /// SHA256加密
        /// </summary>
        SHA256 = 31
    }
}
