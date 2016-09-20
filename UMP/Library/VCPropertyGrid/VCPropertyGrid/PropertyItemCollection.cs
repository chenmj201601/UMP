//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2fa8195f-54fa-4315-a30c-0793158df90f
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyItemCollection
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids
//        File Name:                PropertyItemCollection
//
//        created by Charley at 2014/7/23 12:10:06
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows.Data;
using VoiceCyber.Wpf.PropertyGrids.Utilities;

namespace VoiceCyber.Wpf.PropertyGrids
{
    public class PropertyItemCollection : ReadOnlyObservableCollection<PropertyItem>
    {
        internal static readonly string CategoryPropertyName;
        internal static readonly string CategoryOrderPropertyName;
        internal static readonly string PropertyOrderPropertyName;
        internal static readonly string DisplayNamePropertyName;

        private bool _preventNotification;

        static PropertyItemCollection()
        {
            PropertyItem p = null;
            CategoryPropertyName = ReflectionHelper.GetPropertyOrFieldName(() => p.Category);
            CategoryOrderPropertyName = ReflectionHelper.GetPropertyOrFieldName(() => p.CategoryOrder);
            PropertyOrderPropertyName = ReflectionHelper.GetPropertyOrFieldName(() => p.PropertyOrder);
            DisplayNamePropertyName = ReflectionHelper.GetPropertyOrFieldName(() => p.DisplayName);
        }

        public PropertyItemCollection(ObservableCollection<PropertyItem> editableCollection)
            : base(editableCollection)
        {
            EditableCollection = editableCollection;
        }

        internal Predicate<object> FilterPredicate
        {
            get { return GetDefaultView().Filter; }
            set { GetDefaultView().Filter = value; }
        }

        public ObservableCollection<PropertyItem> EditableCollection { get; private set; }

        private ICollectionView GetDefaultView()
        {
            return CollectionViewSource.GetDefaultView(this);
        }

        public void GroupBy(string name)
        {
            GetDefaultView().GroupDescriptions.Add(new PropertyGroupDescription(name));
        }

        public void SortBy(string name, ListSortDirection sortDirection)
        {
            GetDefaultView().SortDescriptions.Add(new SortDescription(name, sortDirection));
        }

        public void Filter(string text)
        {
            Predicate<object> filter = PropertyItemCollection.CreateFilter(text);
            GetDefaultView().Filter = filter;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (_preventNotification)
                return;

            base.OnCollectionChanged(args);
        }

        internal void UpdateItems(IEnumerable<PropertyItem> newItems)
        {
            if (newItems == null)
                throw new ArgumentNullException("newItems");

            _preventNotification = true;
            using (GetDefaultView().DeferRefresh())
            {
                EditableCollection.Clear();
                foreach (var item in newItems)
                {
                    this.EditableCollection.Add(item);
                }
            }
            _preventNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        internal void UpdateCategorization(GroupDescription groupDescription, bool isPropertyGridCategorized)
        {
            // Compute Display Order relative to PropertyOrderAttributes on PropertyItem
            // which could be different in Alphabetical or Categorized mode.
            foreach (PropertyItem item in this.Items)
            {
                item.DescriptorDefinition.DisplayOrder = item.DescriptorDefinition.ComputeDisplayOrderInternal(isPropertyGridCategorized);
                item.PropertyOrder = item.DescriptorDefinition.DisplayOrder;
            }

            // Clear view values
            ICollectionView view = this.GetDefaultView();
            using (view.DeferRefresh())
            {
                view.GroupDescriptions.Clear();
                view.SortDescriptions.Clear();

                // Update view values
                if (groupDescription != null)
                {
                    view.GroupDescriptions.Add(groupDescription);
                    SortBy(CategoryOrderPropertyName, ListSortDirection.Ascending);
                    SortBy(CategoryPropertyName, ListSortDirection.Ascending);
                }

                SortBy(PropertyOrderPropertyName, ListSortDirection.Ascending);
                SortBy(DisplayNamePropertyName, ListSortDirection.Ascending);
            }
        }

        internal void RefreshView()
        {
            GetDefaultView().Refresh();
        }

        internal static Predicate<object> CreateFilter(string text)
        {
            Predicate<object> filter = null;

            if (!string.IsNullOrEmpty(text))
            {
                filter = (item) =>
                {
                    var property = item as PropertyItem;
                    if (property.DisplayName != null)
                    {
                        return property.DisplayName.ToLower().StartsWith(text.ToLower());
                    }

                    return false;
                };
            }

            return filter;
        }
    }
}
