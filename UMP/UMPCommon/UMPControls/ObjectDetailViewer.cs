//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cdd5ff7d-b0d4-451c-b930-7d4cfc49fad8
//        CLR Version:              4.0.30319.18444
//        Name:                     ObjectDetailViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                ObjectDetailViewer
//
//        created by Charley at 2014/9/16 11:39:04
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// 详细信息视图
    /// </summary>
    public class ObjectDetailViewer : ListBox
    {
        static ObjectDetailViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ObjectDetailViewer),
                new FrameworkPropertyMetadata(typeof (ObjectDetailViewer)));
        }
        /// <summary>
        /// 标题
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof (string), typeof (ObjectDetailViewer), new PropertyMetadata(default(string)));
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof (string), typeof (ObjectDetailViewer), new PropertyMetadata(default(string)));
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get { return (string) GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
        /// <summary>
        /// 图标
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof (ImageSource), typeof (ObjectDetailViewer), new PropertyMetadata(default(ImageSource)));
        /// <summary>
        /// 图标
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource) GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
    }
}
