//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e36bf679-76b7-4929-a240-c15dbe6ce4af
//        CLR Version:              4.0.30319.18444
//        Name:                     UCPathLister
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Editors
//        File Name:                UCPathLister
//
//        created by Charley at 2015/2/2 14:42:19
//        http://www.voicecyber.com
//
//======================================================================

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UMPS1110.Models;
using VoiceCyber.UMP.Controls;

namespace UMPS1110.Editors
{
    /// <summary>
    /// 实现树形列表列出服务器上磁盘信息及文件（夹）信息
    /// </summary>
    public partial class UCPathLister
    {

        #region Members

        private ObjectItem mRootItem;
        private ObjectItem mSelectedItem;

        #endregion


        #region Public Property

        public ObjectItem RootItem
        {
            get { return mRootItem; }
        }

        public ObjectItem SelectedItem
        {
            get { return mSelectedItem; }
        }

        #endregion
      

        static UCPathLister()
        {
            PathListerEventEvent = EventManager.RegisterRoutedEvent("PathListerEvent", RoutingStrategy.Bubble,
                typeof (RoutedPropertyChangedEventHandler<PathListerEventEventArgs>), typeof (UCPathLister));
        }

        public UCPathLister()
        {
            InitializeComponent();

            mRootItem = new ObjectItem();

            Loaded += UCPathLister_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void UCPathLister_Loaded(object sender, RoutedEventArgs e)
        {
            TreeViewPathList.ItemsSource = mRootItem.Children;
            ChangeLanguage();
        }


        #region Event Handler

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            mSelectedItem = TreeViewPathList.SelectedItem as ObjectItem;
            PathListerEventEventArgs args = new PathListerEventEventArgs();
            args.Code = 1;
            args.Data = mSelectedItem;
            OnPathListerEvent(args);
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        private void ObjectItem_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var panel = sender as Border;
                if (panel == null) { return; }
                var item = panel.Tag as ObjectItem;
                if (item == null) { return; }
                PathListerEventEventArgs args = new PathListerEventEventArgs();
                args.Code = 3;
                args.Data = item;
                OnPathListerEvent(args);
            }
        }

        #endregion


        #region PathListerEvent

        public static readonly RoutedEvent PathListerEventEvent;

        public event  RoutedPropertyChangedEventHandler<PathListerEventEventArgs> PathListerEvent
        {
            add{AddHandler(PathListerEventEvent,value);}
            remove{RemoveHandler(PathListerEventEvent,value);}
        }

        private void OnPathListerEvent(PathListerEventEventArgs args)
        {
            RoutedPropertyChangedEventArgs<PathListerEventEventArgs> e =
                new RoutedPropertyChangedEventArgs<PathListerEventEventArgs>(null, args);
            e.RoutedEvent = PathListerEventEvent;
            RaiseEvent(e);
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            BtnConfirm.Content = CurrentApp.GetLanguageInfo("11101", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("11100", "Close");

            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.Title = CurrentApp.GetLanguageInfo("111000", "Select a position");
            }
        }

        #endregion
    }
}
