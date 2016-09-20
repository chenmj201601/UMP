//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cfb23892-1e3a-4699-8f5a-a78a2e7e3a0d
//        CLR Version:              4.0.30319.18444
//        Name:                     DiagramNode
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.DiagramControl.Implementation
//        File Name:                DiagramNode
//
//        created by Charley at 2014/9/10 16:23:48
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// 折叠展开操作的事件委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArguments"></param>
    public delegate void DiagramNodeStateChangedHandler(DiagramNode sender, RoutedEventArgs eventArguments);
    /// <summary>
    /// 表示关系图的一个节点
    /// </summary>
    public class DiagramNode:ContentControl
    {
        /// <summary>
        /// 
        /// </summary>
        public event DiagramNodeStateChangedHandler Expanded;
        /// <summary>
        /// 
        /// </summary>
        public event DiagramNodeStateChangedHandler Collapsed;
        /// <summary>
        /// 
        /// </summary>
        public event DiagramNodeStateChangedHandler Selected;

        private List<DiagramNode> mDiagramChildren = new List<DiagramNode>();
        private DiagramNode mDiagramParent;
        private bool mIsExpanded;

        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedUICommand ExpandCommand =
         new RoutedUICommand("expandCommand", "expandCommand", typeof(DiagramNode));

        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedUICommand CollapseCommand =
            new RoutedUICommand("collapseCommand", "collapseCommand", typeof(DiagramNode));

        static DiagramNode()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramNode),
                new FrameworkPropertyMetadata(typeof(DiagramNode)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="treeParent"></param>
        /// <param name="imageUrl"></param>
        /// <param name="itemDescription"></param>
        public DiagramNode(string nodeName, DiagramNode treeParent,
           string imageUrl, string itemDescription)
        {
            CommandBindings.Add(new CommandBinding(ExpandCommand, ExpandCommand_Executed, ExpandCommand_CanExecute));
            CommandBindings.Add(new CommandBinding(CollapseCommand, CollapseCommand_Executed, CollapseCommand_CanExecute));

            ToolTip = itemDescription;
            mDiagramParent = treeParent;
            NodeName = nodeName;
            if (treeParent != null)
            {
                mDiagramParent.mDiagramChildren.Add(this);
            }
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(imageUrl, UriKind.RelativeOrAbsolute);
            image.EndInit();
            Icon = image;
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty NodeNameProperty =
            DependencyProperty.Register("NodeName", typeof(string), typeof(DiagramNode), new PropertyMetadata(default(string)));

        /// <summary>
        /// 
        /// </summary>
        public string NodeName
        {
            get { return (string)GetValue(NodeNameProperty); }
            set { SetValue(NodeNameProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(DiagramNode), new PropertyMetadata(default(string)));

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(DiagramNode), new PropertyMetadata(default(ImageSource)));

        /// <summary>
        /// 
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArguments"></param>
        protected void OnSelected(DiagramNode sender, RoutedEventArgs eventArguments)
        {
            if (Selected != null)
                Selected(sender, eventArguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArguments"></param>
        protected void OnExpanded(DiagramNode sender, RoutedEventArgs eventArguments)
        {
            OnSelected(sender, eventArguments);
            if (Expanded != null)
                Expanded(this, eventArguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArguments"></param>
        protected void OnCollapsed(DiagramNode sender, RoutedEventArgs eventArguments)
        {
            OnSelected(sender, eventArguments);
            if (Collapsed != null)
                Collapsed(this, eventArguments);
        }

        /// <summary>
        /// 
        /// </summary>
        public List<DiagramNode> DiagramChildren
        {
            get { return mDiagramChildren; }
            set { mDiagramChildren = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsExpanded
        {
            get { return mIsExpanded; }
            set { mIsExpanded = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Point Location
        {
            get
            {
                return new Point(
                    (double)GetValue(Canvas.LeftProperty) + (double)GetValue(ActualWidthProperty) / 2.0,
                    (double)GetValue(Canvas.TopProperty) + (double)GetValue(ActualHeightProperty) / 2.0);
            }

            set
            {
                SetValue(Canvas.LeftProperty, value.X - (double)GetValue(ActualWidthProperty) / 2.0);
                SetValue(Canvas.TopProperty, value.Y - (double)GetValue(ActualHeightProperty) / 2.0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DiagramNode DiagramParent
        {
            get { return mDiagramParent; }
        }

        /// <summary>
        /// 
        /// </summary>
        public double BoundingCircle
        {
            get
            {
                double size = Math.Max((double)GetValue(ActualWidthProperty), (double)GetValue(ActualHeightProperty)) / 2;

                return Math.Sqrt(size * size * 2);
            }
        }

        private void ExpandCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mDiagramChildren.Any();
        }

        private void ExpandCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IsExpanded = true;
            OnExpanded(this, new RoutedEventArgs(e.RoutedEvent));
        }

        private void CollapseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsExpanded;
        }

        private void CollapseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IsExpanded = false;
            OnCollapsed(this, new RoutedEventArgs(e.RoutedEvent));
        }
    }
}
