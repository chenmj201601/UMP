using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace VoiceCyber.Wpf.CustomControls.Charts
{
    /// <summary>
    /// 
    /// </summary>
    public class LabelExtractor : Freezable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new LabelExtractor();
        }

        private static void OnItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            LabelExtractor v = sender as LabelExtractor;
            ItemCollection oldItems = args.OldValue as ItemCollection;
            ItemCollection newItems = args.NewValue as ItemCollection;
            if (oldItems != null)
                ((INotifyCollectionChanged)oldItems).CollectionChanged -= v.OnLabelsCollectionChanged;

            if (v != null && v.Items != null)
            {
                ((INotifyCollectionChanged)v.Items).CollectionChanged += v.OnLabelsCollectionChanged;
                if (v.LabelPath != null)
                    v.GenerateLabelList();
            }
        }

        private static void OnLabelPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            LabelExtractor v = sender as LabelExtractor;
            if (v != null && v.LabelPath != null && v.Items != null)
            {
                v.GenerateLabelList();
            }
        }

        private void OnLabelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action.Equals(NotifyCollectionChangedAction.Reset))
            {
                GenerateLabelList();
            }
            else if (e.Action.Equals(NotifyCollectionChangedAction.Remove))
            {
                for (int i = 0; i < e.OldItems.Count; i++)
                {
                    Labels.RemoveAt(e.OldStartingIndex);
                }
            }
            else if (e.Action.Equals(NotifyCollectionChangedAction.Move))
            {
                Labels.Move(e.OldStartingIndex, e.NewStartingIndex);
            }
            else
            {
                for (int i = 0; i < e.NewItems.Count; i++)
                {
                    CreateInternalBinding(Items[e.NewStartingIndex + i]);
                    if (e.Action.Equals(NotifyCollectionChangedAction.Add))
                        Labels.Insert(e.NewStartingIndex + i, LabelHolder);
                    else
                        Labels[e.NewStartingIndex + i] = LabelHolder;
                }
            }
        }

        private void GenerateLabelList()
        {
            SetValue(LabelsKey, new ObservableCollection<string>());

            ObservableCollection<String> tempLabels = Labels;
            foreach (Object o in Items)
            {
                CreateInternalBinding(o);
                tempLabels.Add(LabelHolder);
            }
        }

        private void CreateInternalBinding(Object source)
        {
            Binding b = new Binding();
            b.Source = source;
            if (IsXmlNodeHelper(source))
                b.XPath = LabelPath.Path;
            else
                b.Path = LabelPath;
            BindingOperations.SetBinding(this, LabelHolderProperty, b);
        }

        private static bool IsXmlNodeHelper(object item)
        {
            return item is System.Xml.XmlNode;
        }
        /// <summary>
        /// 
        /// </summary>
        public ItemCollection Items
        {
            get { return (ItemCollection)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ItemCollection), typeof(LabelExtractor), new FrameworkPropertyMetadata(null, OnItemsChanged));

        /// <summary>
        /// 
        /// </summary>
        public PropertyPath LabelPath
        {
            get { return (PropertyPath)GetValue(LabelPathProperty); }
            set { SetValue(LabelPathProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelPathProperty =
            DependencyProperty.Register("LabelPath", typeof(PropertyPath), typeof(LabelExtractor), new FrameworkPropertyMetadata(null, OnLabelPathChanged));

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<String> Labels
        {
            get { return (ObservableCollection<String>)GetValue(LabelsProperty); }
        }
        /// <summary>
        /// 
        /// </summary>
        private static readonly DependencyPropertyKey LabelsKey =
            DependencyProperty.RegisterReadOnly("Labels", typeof(ObservableCollection<String>), typeof(LabelExtractor), new UIPropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelsProperty = LabelsKey.DependencyProperty;
        /// <summary>
        /// 
        /// </summary>
        private String LabelHolder
        {
            get { return (String)GetValue(LabelHolderProperty); }
            set { SetValue(LabelHolderProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        private static readonly DependencyProperty LabelHolderProperty =
            DependencyProperty.Register("CurrentLabel", typeof(String), typeof(LabelExtractor), new UIPropertyMetadata(null));



    }
}
