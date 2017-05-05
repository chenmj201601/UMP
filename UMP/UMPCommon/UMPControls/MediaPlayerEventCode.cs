//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b4b83b3b-4c91-493d-984c-c2e4df623ded
//        CLR Version:              4.0.30319.18063
//        Name:                     MediaPlayerEventCode
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                MediaPlayerEventCode
//
//        created by Charley at 2015/7/20 12:31:57
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// UMPMediaPlayer的事件代码（部分公共事件代码在VoiceCyber.Common.Defines定义）
    /// 范围（1000 ~ 9999）
    /// </summary>
    public class MediaPlayerEventCodes
    {
        public const int BTN_CLOSE = 1011;          //点击了关闭按钮
        public const int BTN_PLAY = 1012;           //点击了播放按钮
        public const int BTN_STOP = 1013;           //点击停止按钮
        public const int BTN_PAUSE = 1014;          //点击了暂停按钮
        public const int PLAYBACKSTOPPED = 1021;    //播放结束
        public const int PLAYBACKSTARTED = 1022;    //开始播放
        public const int PLAYING = 1023;            //正在播放
        public const int MEDIAOPENED = 1024;        //媒体加载完成
    }
}
