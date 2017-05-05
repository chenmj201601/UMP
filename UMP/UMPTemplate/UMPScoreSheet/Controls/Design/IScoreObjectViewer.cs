//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    deec1235-1241-482a-8a7f-8fadf11b6ec8
//        CLR Version:              4.0.30319.18444
//        Name:                     IScoreObjectViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                IScoreObjectViewer
//
//        created by Charley at 2014/6/18 17:12:44
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;

namespace VoiceCyber.UMP.ScoreSheets.Controls.Design
{
    /// <summary>
    /// IScoreObjectViewer
    /// </summary>
    public interface IScoreObjectViewer
    {
        /// <summary>
        /// 设置信息
        /// </summary>
        List<ScoreSetting> Settings { get; set; }
        /// <summary>
        /// 语言信息
        /// </summary>
        List<ScoreLangauge> Languages { get; set; } 
        /// <summary>
        /// 风格
        /// </summary>
        ScoreItemClassic ViewClassic { get; set; }
        /// <summary>
        /// 语言类型
        /// </summary>
        int LangID { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        void Init();
        /// <summary>
        /// 加载完成
        /// </summary>
        event Action ViewerLoaded;
    }
}
