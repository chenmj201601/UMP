//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    31c6d5ee-6fd0-4431-913e-d7b505312b44
//        CLR Version:              4.0.30319.42000
//        Name:                     Service12Consts
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService12
//        File Name:                Service12Consts
//
//        Created by Charley at 2016/9/14 17:59:55
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService12
{
    public class Service12Consts
    {
        /// <summary>
        /// 请求播放指定的媒体文件
        /// </summary>
        public const string HTTP_ACTION_NAME_PLAY = "play.ump";

        /// <summary>
        /// 会话Token
        /// </summary>
        public const string HTTP_PARAM_NAME_TOKEN = "token";
        /// <summary>
        /// 记录流水号（C002）
        /// </summary>
        public const string HTTP_PARAM_NAME_SERIALNO = "serialno";
        /// <summary>
        /// 录音流水号（C077）
        /// </summary>
        public const string HTTP_PARAM_NAME_RECORDREFERENCE = "recordreference";
        /// <summary>
        /// 录音开始时间（C005，UTC，格式 yyyy-MM-dd HH:mm:ss）
        /// </summary>
        public const string HTTP_PARAM_NAME_STARTRECORDTIME = "startrecordtime";
        /// <summary>
        /// 文件的本地保存位置（C035）
        /// </summary>
        public const string HTTP_PARAM_NAME_SAVEPATH = "savepath";
        /// <summary>
        /// 录音记录的版本号
        /// </summary>
        public const string HTTP_PARAM_NAME_VERSION = "version";
        /// <summary>
        /// 原始解密（0：不解密；1：解密）
        /// </summary>
        public const string HTTP_PARAM_NAME_ORIGINALDECRYPT = "originaldecrypt";
        /// <summary>
        /// 解密文件（0：不解密；1：解密）
        /// </summary>
        public const string HTTP_PARAM_NAME_DECRYPTFILE = "decryptfile";
        /// <summary>
        /// 解密密码(M004加密）
        /// </summary>
        public const string HTTP_PARAM_NAME_DECRYPTPASSWORD = "decryptpassword";
        /// <summary>
        /// 转换音频编码格式（0：不转换；>0 表示要转换的目标编码代码，注：pcm 为 1）
        /// </summary>
        public const string HTTP_PARAM_NAME_CONVERTWAVFORMAT = "convertwavformat";
        /// <summary>
        /// 转换音频编码格式所使用的工具（默认值为 1）
        /// 1：FileConvert
        /// 2：Lame
        /// 3：ffmpeg
        /// </summary>
        public const string HTTP_PARAM_NAME_CONVERTWAVUTIL = "convertwavutil";
        /// <summary>
        /// 转换音频编码格式所生成文件的扩展名（默认值为 1）
        /// 1：wav
        /// 2：mp3
        /// </summary>
        public const string HTTP_PARAM_NAME_CONVERTWAVEXTENSION = "convertwavext";


        #region 错误码

        public const int DECRYPT_PASSWORD_ERROR = 1001;     //解密密码错误

        #endregion

    }
}
