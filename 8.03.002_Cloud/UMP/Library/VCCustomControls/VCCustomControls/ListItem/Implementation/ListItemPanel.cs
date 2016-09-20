//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    154f1979-6337-46e5-ae9c-055c231fe4f5
//        CLR Version:              4.0.30319.18063
//        Name:                     ListItemPanel
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.ListItem.Implementation
//        File Name:                ListItemPanel
//
//        created by Charley at 2015/6/9 16:26:59
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VoiceCyber.Wpf.CustomControls.ListItem.Implementation;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// 
    /// </summary>
    public class ListItemPanel : ContentControl
    {
        static ListItemPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListItemPanel),
                new FrameworkPropertyMetadata(typeof(ListItemPanel)));

            ItemMouseDoubleClickEvent = EventManager.RegisterRoutedEvent("ItemMouseDoubleClick", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<ListItemEventArgs>), typeof(ListItemPanel));
        }

        /// <summary>
        /// 
        /// </summary>
        public ListItemPanel()
        {
            MouseDoubleClick += ListItemPanel_MouseDoubleClick;
        }

        void ListItemPanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListItemEventArgs args = new ListItemEventArgs();
            args.Code = ListItemEventArgs.EVT_MOUSEDOUBLECLICK;
            args.Data = e;
            OnItemMouseDoubleClick(sender, args);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedEvent ItemMouseDoubleClickEvent;

        /// <summary>
        /// 
        /// </summary>
        public event RoutedPropertyChangedEventHandler<ListItemEventArgs> ItemMouseDoubleClick
        {
            add { AddHandler(ItemMouseDoubleClickEvent, value); }
            remove { RemoveHandler(ItemMouseDoubleClickEvent, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnItemMouseDoubleClick(object sender,ListItemEventArgs e)
        {
            var panel = sender as ListItemPanel;
            if (panel != null)
            {
                RoutedPropertyChangedEventArgs<ListItemEventArgs> args =
                new RoutedPropertyChangedEventArgs<ListItemEventArgs>(default(ListItemEventArgs), e);
                args.RoutedEvent = ItemMouseDoubleClickEvent;
                panel.RaiseEvent(args);
            }
           
        }
    }
}
