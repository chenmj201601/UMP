//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c270130e-7f6d-4de4-b492-830adf6e8b4a
//        CLR Version:              4.0.30319.18063
//        Name:                     PopupPanel
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                PopupPanel
//
//        created by Charley at 2014/8/20 21:16:13
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VoiceCyber.Wpf.CustomControls;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// 弹出窗口，支持拖动及模式窗口
    /// </summary>
    public class PopupPanel : UMPUserControl
    {
        #region Memebers

        private const string PART_GridMain = "PART_GridMain";
        private const string PART_Panel = "PART_Panel";

        private DragHelper mDragHelper;
        private Grid mGridMain;
        private DragPanel mDragPanel;

        #endregion


        #region Constructor

        static PopupPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupPanel),
                new FrameworkPropertyMetadata(typeof(PopupPanel)));

            CommandManager.RegisterClassCommandBinding(typeof(PopupPanel),
              new CommandBinding(ApplicationCommands.Close, CloseCommand_Executed, CloseCommand_CanExecute));

            OpenedEvent = EventManager.RegisterRoutedEvent("Opended",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(PopupPanel));
        }

        public PopupPanel()
        {
            mDragHelper = new DragHelper();
        }

        #endregion


        #region TitleProperty

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(PopupPanel), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion


        #region IconProperty

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(PopupPanel), new PropertyMetadata(default(ImageSource)));

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        #endregion


        #region IsOpenProperty

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(PopupPanel), new PropertyMetadata(true, OnIsOpenChanged));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (PopupPanel)d;
            if (panel != null)
            {
                panel.OnIsOpenChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        public void OnIsOpenChanged(bool oldValue, bool newValue)
        {
            Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
            IsMask = newValue && IsModal;
            if (newValue)
            {
                //引发Opended事件
                RoutedEventArgs args = new RoutedEventArgs();
                args.RoutedEvent = OpenedEvent;
                RaiseEvent(args);
            }
        }

        #endregion


        #region IsMaskProperty

        public static readonly DependencyProperty IsMaskProperty =
            DependencyProperty.Register("IsMask", typeof(bool), typeof(PopupPanel), new PropertyMetadata(false));

        public bool IsMask
        {
            get { return (bool)GetValue(IsMaskProperty); }
            set { SetValue(IsMaskProperty, value); }
        }

        #endregion


        #region IsModalProperty

        public static readonly DependencyProperty IsModalProperty =
            DependencyProperty.Register("IsModal", typeof(bool), typeof(PopupPanel), new PropertyMetadata(true));

        public bool IsModal
        {
            get { return (bool)GetValue(IsModalProperty); }
            set { SetValue(IsModalProperty, value); }
        }

        #endregion


        #region PanelWidthProperty

        public static readonly DependencyProperty PanelWidthProperty =
            DependencyProperty.Register("PanelWidth", typeof(int), typeof(PopupPanel), new PropertyMetadata(default(int)));

        public int PanelWidth
        {
            get { return (int)GetValue(PanelWidthProperty); }
            set { SetValue(PanelWidthProperty, value); }
        }

        #endregion


        #region PanelHeightProperty

        public static readonly DependencyProperty PanelHeightProperty =
            DependencyProperty.Register("PanelHeight", typeof(int), typeof(PopupPanel), new PropertyMetadata(default(int)));

        public int PanelHeight
        {
            get { return (int)GetValue(PanelHeightProperty); }
            set { SetValue(PanelHeightProperty, value); }
        }

        #endregion


        #region CloseCommand

        private static void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PopupPanel panel = (PopupPanel)sender;
            if (panel != null)
            {
                if (panel.Closing != null)
                {
                    CancelEventArgs args = new CancelEventArgs(false);
                    panel.Closing(panel, args);
                    if (args.Cancel) { return; }
                }
                panel.IsOpen = false;
            }
        }

        private static void CloseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs ex)
        {
            ex.CanExecute = true;
        }

        #endregion


        #region Opended Event

        public static readonly RoutedEvent OpenedEvent;

        public event RoutedEventHandler Opended
        {
            add { AddHandler(OpenedEvent, value); }
            remove { RemoveHandler(OpenedEvent, value); }
        }

        #endregion


        #region Closing Event

        public event CancelEventHandler Closing;

        #endregion


        #region Template

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mGridMain = GetTemplateChild(PART_GridMain) as Grid;
            if (mGridMain != null)
            {

            }
            mDragPanel = GetTemplateChild(PART_Panel) as DragPanel;
            if (mDragPanel != null)
            {
                if (PanelWidth > 0)
                {
                    mDragPanel.Width = PanelWidth;
                }
                if (PanelHeight > 0)
                {
                    mDragPanel.Height = PanelHeight;
                }
            }
            if (mGridMain != null && mDragPanel != null)
            {
                mDragHelper.Init(mGridMain, mDragPanel);
            }
        }

        #endregion


        #region Languages

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            var child = Content as ILanguagePage;
            if (child != null)
            {
                child.ChangeLanguage();
            }
        }

        #endregion
    }
}
