//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    618fac7c-6c7a-496b-9949-2a4fe8ee5139
//        CLR Version:              4.0.30319.18063
//        Name:                     TileView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.TileViews.Implementation
//        File Name:                TileView
//
//        created by Charley at 2015/7/1 13:42:55
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// 简单平铺的视图,用于在ListView控件中平铺显示列表项
    /// </summary>
    public class TileView : ViewBase
    {
        private DataTemplate mItemTemplate;

        public DataTemplate ItemTemplate
        {
            get { return mItemTemplate; }
            set { mItemTemplate = value; }
        }

        protected override object DefaultStyleKey
        {
            get { return new ComponentResourceKey(GetType(), "TileView"); }
        }

        protected override object ItemContainerDefaultStyleKey
        {
            get { return new ComponentResourceKey(GetType(), "TileViewItem"); }
        }
    }
}
