using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// EditBox is a custom cotrol that can switch between two modes: 
    /// editing and normal. When it is in editing mode, the content is
    /// displayed in a TextBox that provides editing capbability. When 
    /// the EditBox is in normal, its content is displayed in a TextBlock
    /// that is not editable.
    /// 
    /// This control is designed to be used in with a GridView View.
    /// </summary>
    // <!--<SnippetEditBoxStart>
    public class ListItemEditBox : Control
    {
        // <!--</SnippetEditBoxStart>

        #region Static Constructor

        /// <summary>
        /// Static constructor
        /// </summary>
        static ListItemEditBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListItemEditBox),
                new FrameworkPropertyMetadata(typeof(ListItemEditBox)));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when the tree for the EditBox has been generated.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TextBlock textBlock = GetTemplateChild("PART_TextBlockPart")
                   as TextBlock;
            Debug.Assert(textBlock != null, "No TextBlock!");

            mTextBox = new TextBox();
            var tempBox = mTextBox as TextBox;
            tempBox.IsReadOnly = IsReadOnly;
            mAdorner = new EditBoxAdorner(textBlock, mTextBox);
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(textBlock);
            layer.Add(mAdorner);

            mTextBox.KeyDown += OnTextBoxKeyDown;
            mTextBox.LostKeyboardFocus +=
              OnTextBoxLostKeyboardFocus;

            //Receive notification of the event to handle the column 
            //resize. 
            HookTemplateParentResizeEvent();

            //Capture the resize event to  handle ListView resize cases.
            HookItemsControlEvents();

            mListViewItem = GetDependencyObjectFromVisualTree(this,
                typeof(ListViewItem)) as ListViewItem;

            Debug.Assert(mListViewItem != null, "No ListViewItem found");
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// If the ListViewItem that contains the EditBox is selected, 
        /// when the mouse pointer moves over the EditBox, the corresponding
        /// MouseEnter event is the first of two events (MouseUp is the second)
        /// that allow the EditBox to change to editing mode.
        /// </summary>
        //<SnippetOnMouseEnter>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (!IsEditing && IsParentSelected)
            {
                mCanBeEdit = true;
            }
        }
        //</SnippetOnMouseEnter>

        /// <summary>
        /// If the MouseLeave event occurs for an EditBox control that
        /// is in normal mode, the mode cannot be changed to editing mode
        /// until a MouseEnter event followed by a MouseUp event occurs.
        /// </summary>
        //<SnippetOnMouseLeave>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            mIsMouseWithinScope = false;
            mCanBeEdit = false;
        }
        //</SnippetOnMouseLeave>
        /// <summary>
        /// An EditBox switches to editing mode when the MouseUp event occurs
        /// for that EditBox and the following conditions are satisfied:
        /// 1. A MouseEnter event for the EditBox occurred before the 
        /// MouseUp event.
        /// 2. The mouse did not leave the EditBox between the
        /// MouseEnter and MouseUp events.
        /// 3. The ListViewItem that contains the EditBox was selected
        /// when the MouseEnter event occurred.
        /// </summary>
        /// <param name="e"></param>
        //<SnippetOnMouseUp>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.ChangedButton == MouseButton.Right ||
                e.ChangedButton == MouseButton.Middle)
                return;

            if (!IsEditing)
            {
                if (!e.Handled && (mCanBeEdit || mIsMouseWithinScope))
                {
                    IsEditing = true;
                }

                //If the first MouseUp event selects the parent ListViewItem,
                //then the second MouseUp event puts the EditBox in editing 
                //mode
                if (IsParentSelected)
                    mIsMouseWithinScope = true;
            }
        }
        //</SnippetOnMouseUp>

        #endregion

        #region Public Properties

        #region IsReadOnly
        /// <summary>
        /// 是否只读
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof (bool), typeof (ListItemEditBox), new PropertyMetadata(true));
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool) GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        #endregion


        #region Value

        /// <summary>
        /// ValueProperty DependencyProperty.
        /// </summary>
        //<SnippetValueProperty>
        public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register(
                        "Value",
                        typeof(object),
                        typeof(ListItemEditBox),
                        new FrameworkPropertyMetadata(null));
        //</SnippetValueProperty>

        /// <summary>
        /// Gets or sets the value of the EditBox
        /// </summary>
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion

        #region IsEditing

        /// <summary>
        /// IsEditingProperty DependencyProperty
        /// </summary>
        //<SnippetIsEditingProperty>
        public static DependencyProperty IsEditingProperty =
                DependencyProperty.Register(
                        "IsEditing",
                        typeof(bool),
                        typeof(ListItemEditBox),
                        new FrameworkPropertyMetadata(false));
        //</SnippetIsEditingProperty>

        /// <summary>
        /// Returns true if the EditBox control is in editing mode.
        /// </summary>
        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            private set
            {
                SetValue(IsEditingProperty, value);
                mAdorner.UpdateVisibilty(value);
            }
        }

        #endregion

        #region IsParentSelected

        /// <summary>
        /// Gets whether the ListViewItem that contains the 
        /// EditBox is selected.
        /// </summary>
        private bool IsParentSelected
        {
            get
            {
                if (mListViewItem == null)
                    return false;
                return mListViewItem.IsSelected;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// When an EditBox is in editing mode, pressing the ENTER or F2
        /// keys switches the EditBox to normal mode.
        /// </summary>
        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (IsEditing && (e.Key == Key.Enter || e.Key == Key.F2))
            {
                IsEditing = false;
                mCanBeEdit = false;
            }
        }

        /// <summary>
        /// If an EditBox loses focus while it is in editing mode, 
        /// the EditBox mode switches to normal mode.
        /// </summary>
        private void OnTextBoxLostKeyboardFocus(object sender,
                                             KeyboardFocusChangedEventArgs e)
        {
            IsEditing = false;
        }

        /// <summary>
        /// Sets IsEditing to false when the ListViewItem that contains an
        /// EditBox changes its size
        /// </summary>
        private void OnCouldSwitchToNormalMode(object sender,
                                               RoutedEventArgs e)
        {
            IsEditing = false;
        }

        private void HookItemsControlEvents()
        {
            mItemsControl = GetDependencyObjectFromVisualTree(this,
                                typeof(ItemsControl)) as ItemsControl;
            if (mItemsControl != null)
            {
                //Handle the Resize/ScrollChange/MouseWheel 
                //events to determine whether to switch to Normal mode
                mItemsControl.SizeChanged +=
                    OnCouldSwitchToNormalMode;
                mItemsControl.AddHandler(ScrollViewer.ScrollChangedEvent,
                    new RoutedEventHandler(OnScrollViewerChanged));
                mItemsControl.AddHandler(MouseWheelEvent,
                    new RoutedEventHandler(OnCouldSwitchToNormalMode), true);
            }
        }

        /// <summary>
        /// If an EditBox is in editing mode and the content of a ListView is
        /// scrolled, then the EditBox switches to normal mode.
        /// </summary>
        private void OnScrollViewerChanged(object sender, RoutedEventArgs args)
        {
            if (IsEditing && Mouse.PrimaryDevice.LeftButton ==
                                MouseButtonState.Pressed)
            {
                IsEditing = false;
            }
        }

        /// <summary>
        /// Walk visual tree to find the first DependencyObject 
        /// of the specific type.
        /// </summary>
        private DependencyObject
            GetDependencyObjectFromVisualTree(DependencyObject startObject,
                                              Type type)
        {
            //Walk the visual tree to get the parent(ItemsControl) 
            //of this control
            DependencyObject parent = startObject;
            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                    break;
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent;
        }

        /// <summary>
        /// When the size of the column containing the EditBox changes
        /// and the EditBox is in editing mode, switch the mode to normal mode 
        /// </summary>
        private void HookTemplateParentResizeEvent()
        {
            FrameworkElement parent = TemplatedParent as FrameworkElement;
            if (parent != null)
            {
                parent.SizeChanged +=
                    OnCouldSwitchToNormalMode;
            }
        }

        #endregion

        #region private variable

        private EditBoxAdorner mAdorner;
        //A TextBox in the visual tree
        private FrameworkElement mTextBox;
        //Specifies whether an EditBox can switch to editing mode. 
        //Set to true if the ListViewItem that contains the EditBox is 
        //selected, when the mouse pointer moves over the EditBox
        private bool mCanBeEdit;
        //Specifies whether an EditBox can switch to editing mode.
        //Set to true when the ListViewItem that contains the EditBox is 
        //selected when the mouse pointer moves over the EditBox.
        private bool mIsMouseWithinScope;
        //The ListView control that contains the EditBox
        private ItemsControl mItemsControl;
        //The ListViewItem control that contains the EditBox
        private ListViewItem mListViewItem;

        #endregion
        //<SnippetEditBoxEnd>
    }
    //</SnippetEditBoxEnd>

    /// <summary>
    /// An adorner class that contains a TextBox to provide editing capability 
    /// for an EditBox control. The editable TextBox resides in the 
    /// AdornerLayer. When the EditBox is in editing mode, the TextBox is given a size 
    /// it with desired size; otherwise, arrange it with size(0,0,0,0).
    /// </summary>
    //<SnippetEditBoxAdornerStart>
    internal sealed class EditBoxAdorner : Adorner
    {
        //</SnippetEditBoxAdornerStart>
        /// <summary>
        /// Inialize the EditBoxAdorner.
        /// </summary>
        //<SnippetEditBoxAdornerCtor>
        public EditBoxAdorner(UIElement adornedElement,
                              UIElement adorningElement)
            : base(adornedElement)
        {
            mTextBox = adorningElement as TextBox;
            Debug.Assert(mTextBox != null, "No TextBox!");

            mVisualChildren = new VisualCollection(this);

            BuildTextBox();
        }
        //</SnippetEditBoxAdornerCtor>

        #region Public Methods

        /// <summary>
        /// Specifies whether a TextBox is visible 
        /// when the IsEditing property changes.
        /// </summary>
        /// <param name="isVisible"></param>
        public void UpdateVisibilty(bool isVisible)
        {
            mIsVisible = isVisible;
            if (mIsVisible)
            {
                //编辑状态下自动将文本全选
                if (mTextBox != null)
                {
                    mTextBox.SelectAll();
                }
            }
            InvalidateMeasure();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Override to measure elements.
        /// </summary>
        protected override Size MeasureOverride(Size constraint)
        {
            mTextBox.IsEnabled = mIsVisible;
            //if in editing mode, measure the space the adorner element 
            //should cover.
            if (mIsVisible)
            {
                AdornedElement.Measure(constraint);
                mTextBox.Measure(constraint);

                //since the adorner is to cover the EditBox, it should return 
                //the AdornedElement.Width, the extra 15 is to make it more 
                //clear.
                return new Size(AdornedElement.DesiredSize.Width + mExtraWidth,
                                mTextBox.DesiredSize.Height);
            }
            return new Size(0, 0);
        }

        /// <summary>
        /// override function to arrange elements.
        /// </summary>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (mIsVisible)
            {
                mTextBox.Arrange(new Rect(0, 0, finalSize.Width,
                                                finalSize.Height));
            }
            else // if is not is editable mode, no need to show elements.
            {
                mTextBox.Arrange(new Rect(0, 0, 0, 0));
            }
            return finalSize;
        }

        /// <summary>
        /// override property to return infomation about visual tree.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return mVisualChildren.Count; }
        }

        /// <summary>
        /// override function to return infomation about visual tree.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        { return mVisualChildren[index]; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Inialize necessary properties and hook necessary events on TextBox, 
        /// then add it into tree.
        /// </summary>
        private void BuildTextBox()
        {
            mCanvas = new Canvas();
            mCanvas.Children.Add(mTextBox);
            mVisualChildren.Add(mCanvas);

            //Bind Text onto AdornedElement.
            Binding binding = new Binding("Text");
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = AdornedElement;

            mTextBox.SetBinding(TextBox.TextProperty, binding);

            // when layout finishes.
            mTextBox.LayoutUpdated += OnTextBoxLayoutUpdated;
        }

        /// <summary>
        /// When Layout finish, if in editable mode, update focus status 
        /// on TextBox.
        /// </summary>
        private void OnTextBoxLayoutUpdated(object sender, EventArgs e)
        {
            if (mIsVisible)
            {
                mTextBox.Focus();
            }
        }

        #endregion

        #region Private Variables
        //Visual children
        private VisualCollection mVisualChildren;
        //The TextBox that this adorner covers.
        private TextBox mTextBox;
        //Whether the EditBox is in editing mode which means the Adorner 
        //is visible.
        private bool mIsVisible;
        //Canvas that contains the TextBox that provides the ability for it to 
        //display larger than the current size of the cell so that the entire
        //contents of the cell can be edited
        private Canvas mCanvas;

        //Extra padding for the content when it is displayed in the TextBox
        private const double mExtraWidth = 15;
        #endregion
        //<SnippetEditBoxAdornerEnd>
    }
    //</SnippetEditBoxAdornerEnd>
}
