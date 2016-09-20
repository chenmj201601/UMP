//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9267334e-cc88-4bb9-ac60-49b7ab040da1
//        CLR Version:              4.0.30319.18444
//        Name:                     DiagramViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.DiagramControl.Implementation
//        File Name:                DiagramViewer
//
//        created by Charley at 2014/9/10 16:28:18
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// 表示关系图
    /// </summary>
    [TemplatePart(Name = PART_Canvas, Type = typeof(DiagramCanvas))]
    public class DiagramViewer : ContentControl
    {
        public event DiagramNodeStateChangedHandler Selected;

        private const string PART_Canvas = "PART_Canvas";
        private DiagramCanvas mBaseCanvas;
        private DiagramNode mRootNode;
        private bool mAutoExpandRoot;

        static DiagramViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramViewer),
                new FrameworkPropertyMetadata(typeof(DiagramViewer)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mBaseCanvas = GetTemplateChild(PART_Canvas) as DiagramCanvas;
            if (mBaseCanvas != null)
            {
               
            }
        }

        /// <summary>
        /// 父节点
        /// </summary>
        public DiagramNode RootNode
        {
            set
            {
                Clear();
                mRootNode = value;
                if (mBaseCanvas != null)
                {
                    mBaseCanvas.Children.Add(mRootNode);
                }

                mRootNode.Location = new Point(
                    (double)GetValue(ActualWidthProperty) / 2.0,
                    (double)GetValue(ActualHeightProperty) / 2.0);

                mRootNode.Selected += NodeSelected;
                mRootNode.Expanded += NodeExpanded;
                mRootNode.Collapsed += NodeCollapsed;
                if (mAutoExpandRoot)
                {
                    NodeExpanded(mRootNode, null);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool AutoExpandRoot
        {
            get { return mAutoExpandRoot; }
            set { mAutoExpandRoot = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            if (mBaseCanvas != null)
            {
                mBaseCanvas.Children.Clear();
                mBaseCanvas.InvalidateArrange();
                mBaseCanvas.UpdateLayout();
                mBaseCanvas.InvalidateVisual();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnSelected(DiagramNode sender, RoutedEventArgs e)
        {
            if (Selected != null)
            {
                Selected(sender, e);
            }
        }

        private double CalculateStartAngle(DiagramNode node)
        {
            if (node == mRootNode)
            {
                return 0.0;
            }
            Vector parentToNode = node.Location - node.DiagramParent.Location;
            parentToNode.Normalize();
            Vector leftToRight = new Vector(1.0, 0.0);
            double angle = Vector.AngleBetween(parentToNode, leftToRight);
            if (angle < 0)
                angle = 360 - angle;

            if (node.DiagramChildren.Count > 1)
            {
                if (node.Location.Y < node.DiagramParent.Location.Y)
                {
                    if (node.Location.X > node.DiagramParent.Location.X)
                        angle -= 180;
                    else
                        angle -= 270;
                }
                else
                {
                    angle -= 90;
                }
            }

            return (Math.PI * angle / 180.0);
        }

        private void NodeSelected(DiagramNode sender, RoutedEventArgs eventArguments)
        {
            if (Selected != null)
            {
                Selected(sender, eventArguments);
            }
        }

        private void NodeExpanded(DiagramNode sender, RoutedEventArgs eventArguments)
        {
            mRootNode.Location = new Point(
                (double)GetValue(ActualWidthProperty) / 2.0,
                (double)GetValue(ActualHeightProperty) / 2.0);

            MakeChildrenVisible(sender);

            if (sender.DiagramParent != null)
            {
                sender.DiagramParent.Visibility = Visibility.Visible;
                foreach (DiagramNode sibling in sender.DiagramParent.DiagramChildren)
                {
                    if (sibling != sender)
                        sibling.Visibility = Visibility.Collapsed;
                }
                if (sender.DiagramParent.DiagramParent != null)
                    sender.DiagramParent.DiagramParent.Visibility = Visibility.Collapsed;
            }

            if (sender.DiagramChildren.Count > 0)
            {
                double startAngle = CalculateStartAngle(sender);
                double angleBetweenChildren = (sender == mRootNode ? Math.PI * 2.0 : Math.PI) / ((double)sender.DiagramChildren.Count - 0);

                double legDistance = CalculateLegDistance(sender, angleBetweenChildren);

                for (int i = 0; i < sender.DiagramChildren.Count; ++i)
                {
                    DiagramNode child = sender.DiagramChildren[i];
                    child.Selected += NodeSelected;
                    child.Expanded += NodeExpanded;
                    child.Collapsed += NodeCollapsed;

                    Point parentLocation = sender.Location;

                    child.Location = new Point(
                        parentLocation.X + Math.Cos(startAngle + angleBetweenChildren * i) * legDistance,
                        parentLocation.Y + Math.Sin(startAngle + angleBetweenChildren * i) * legDistance);

                    foreach (DiagramNode childsChild in child.DiagramChildren)
                    {
                        childsChild.Visibility = Visibility.Collapsed;
                    }
                }
            }
            if (mBaseCanvas != null)
            {
                mBaseCanvas.InvalidateArrange();
                mBaseCanvas.UpdateLayout();
                mBaseCanvas.InvalidateVisual();
            }
        }

        private static double CalculateLegDistance(DiagramNode sender, double angleBetweenChildren)
        {
            double legDistance = 1.0;
            double childToChildMinDistance = 1.0;
            foreach (DiagramNode child in sender.DiagramChildren)
            {
                legDistance = Math.Max(legDistance, sender.BoundingCircle + child.BoundingCircle);
                foreach (DiagramNode otherChild in sender.DiagramChildren)
                {
                    if (otherChild != child)
                    {
                        childToChildMinDistance = Math.Max(childToChildMinDistance, child.BoundingCircle + otherChild.BoundingCircle);
                    }
                }
            }

            legDistance = Math.Max(
                legDistance,
                (childToChildMinDistance / 2.0) / Math.Sin(angleBetweenChildren / 2.0));
            return legDistance;
        }

        private void MakeChildrenVisible(DiagramNode sender)
        {
            foreach (DiagramNode child in sender.DiagramChildren)
            {
                if (mBaseCanvas != null && !mBaseCanvas.Children.Contains(child))
                    mBaseCanvas.Children.Add(child);
                child.Visibility = Visibility.Visible;
            }
            if (mBaseCanvas != null)
            {
                mBaseCanvas.InvalidateArrange();
                mBaseCanvas.UpdateLayout();
                mBaseCanvas.InvalidateVisual();
            }
        }

        private void NodeCollapsed(DiagramNode sender, RoutedEventArgs eventArguments)
        {
            foreach (DiagramNode child in sender.DiagramChildren)
            {
                child.Visibility = Visibility.Collapsed;
                foreach (DiagramNode grandChildren in child.DiagramChildren)
                {
                    grandChildren.Visibility = Visibility.Collapsed;
                }
            }
            if (mBaseCanvas != null)
            {
                mBaseCanvas.InvalidateArrange();
                mBaseCanvas.UpdateLayout();
                mBaseCanvas.InvalidateVisual();
            }
        }
    }
}
