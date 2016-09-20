//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2aa96ba8-0be7-4fc2-93f7-c246337a6352
//        CLR Version:              4.0.30319.18444
//        Name:                     Defines
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio
//        File Name:                Defines
//
//        created by Charley at 2014/12/8 14:37:51
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NAudio
{
    /// <summary>
    /// 常量定义
    /// </summary>
    public class Defines
    {
        #region Return Code

        //基本操作结果
        public const int RET_SUCCESS = 0;
        public const int RET_FAIL = 1;

        //一般操作结果
        public const int RET_PARAM_INVALID = 101;
        public const int RET_NOT_EXIST = 102;
        public const int RET_TIMEOUT = 103;
        public const int RET_OBJECT_NULL = 104;
        public const int RET_STRING_EMPTY = 105;

        //网络相关
        public const int RET_CONNECT_SUCCESS = 201;
        public const int RET_CONNECT_FAIL = 202;
        public const int RET_NOT_CONNECTED = 203;
        public const int RET_AUTH_SUCCESS = 204;
        public const int RET_AUTH_FAIL = 205;
        public const int RET_NOT_AUTHENTICATED = 206;

        //文件相关
        public const int RET_FILE_NOT_EXIST = 301;
        public const int RET_CONFIG_NOT_EXIST = 302;
        public const int RET_CONFIG_INVALID = 303;
        public const int RET_EQUAL_PATH = 304;

        //数据库相关
        public const int RET_DBACCESS_FAIL = 401;
        //其他 500以上

        #region Event Codes

        //AudioPlayer events
        public const int EVT_UC_LOADED = 1001;                  //控件加载完成
        public const int EVT_UC_UNLOADED = 1002;                //控件卸载完成
        public const int EVT_BTN_CLOSE = 1011;                  //点击了Close按钮
        public const int EVT_BTN_PLAY = 1012;                   //点击了Play按钮
        public const int EVT_BTN_STOP = 1013;                   //点击了Stop按钮
        public const int EVT_PLAYBACKSTOPPED = 1021;            //播放结束
        public const int EVT_PLAYBACKSTARTED = 1022;            //开始播放
        public const int EVT_PLAYING = 1023;                    //正在播放
        public const int EVT_MEDIAOPENED = 1024;                //媒体加载完成
        public const int EVT_EXCEPTION = 1031;                  //发生异常

        #endregion

        #endregion

        #region Png Defines

        public const int PNG_WIDTH = 1000;
        public const int PNG_HEIGHT = 100;

        #endregion

    }
}
