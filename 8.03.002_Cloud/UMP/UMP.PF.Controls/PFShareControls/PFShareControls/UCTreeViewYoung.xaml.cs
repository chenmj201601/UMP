using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PFShareControls
{
    public partial class UCTreeViewYoung : UserControl
    {
        private List<TreeViewItem> IListTreeViewItem = new List<TreeViewItem>();

        public UCTreeViewYoung()
        {
            InitializeComponent();
            IListTreeViewItem.Clear();
        }

        public TreeViewItem AddTreeViewItem(TreeViewItem ATreeViewItemParent, bool? ABoolIsChecked, string AStrImage, string AStrContent, object AObjectData)
        {
            TreeViewItem LTreeViewItemAdded = new TreeViewItem();

            try
            {
                UCTreeViewItem LTreeViewItemHeader = new UCTreeViewItem(ABoolIsChecked, AStrImage, AStrContent);
                LTreeViewItemHeader.IOperationEvent += LTreeViewItemHeader_IOperationEvent;
                LTreeViewItemAdded.Header = LTreeViewItemHeader;
                if (ATreeViewItemParent == null)
                {
                    TreeViewObject.Items.Add(LTreeViewItemAdded);
                }
                else
                {
                    ATreeViewItemParent.Items.Add(LTreeViewItemAdded);
                }
                LTreeViewItemHeader.IObjectThisData = AObjectData;

                IListTreeViewItem.Add(LTreeViewItemAdded);
            }
            catch { LTreeViewItemAdded = null; }

            return LTreeViewItemAdded;
        }

        public List<object> GetCheckedOrUnCheckedItem(string AStrMethod)
        {
            List<object> LListObject = new List<object>();

            foreach (TreeViewItem LTreeViewItemChile in IListTreeViewItem)
            {
                UCTreeViewItem LUCTreeViewItem = LTreeViewItemChile.Header as UCTreeViewItem;
                if (AStrMethod == "1")
                {
                    if (LUCTreeViewItem.BoolIsChecked == true || LUCTreeViewItem.BoolIsChecked == null) { LListObject.Add(LUCTreeViewItem.IObjectThisData); }
                }
                else
                {
                    if (LUCTreeViewItem.BoolIsChecked == false) { LListObject.Add(LUCTreeViewItem.IObjectThisData); } 
                }
            }
            return LListObject;
        }

        public void SetItemCheckedStatus(bool? ABoolStatus, object AObjectData)
        {
            foreach (TreeViewItem LTreeViewItem in IListTreeViewItem)
            {
                UCTreeViewItem LTreeViewItemHeader = LTreeViewItem.Header as UCTreeViewItem;
                if (LTreeViewItemHeader.IObjectThisData.ToString() == AObjectData.ToString())
                {
                    LTreeViewItemHeader.BoolIsChecked = ABoolStatus;
                    SetItemChildParentIsChecked(LTreeViewItemHeader);
                    break;
                }
            }
        }

        public void SetItemCheckedDisabled(bool ABoolChangeStatus, object AObjectData)
        {
            foreach (TreeViewItem LTreeViewItem in IListTreeViewItem)
            {
                UCTreeViewItem LTreeViewItemHeader = LTreeViewItem.Header as UCTreeViewItem;
                if (LTreeViewItemHeader.IObjectThisData.ToString() == AObjectData.ToString())
                {
                    LTreeViewItemHeader.CheckBoxIsChecked.IsEnabled = ABoolChangeStatus;
                    break;
                }
            }
        }

        private void SetItemChildParentIsChecked(UCTreeViewItem AUCTreeViewItem)
        {
            TreeViewItem LTreeViewItemCurrent = AUCTreeViewItem.Parent as TreeViewItem;
            LTreeViewItemCurrent.IsSelected = true;

            SetChildIsChecked(AUCTreeViewItem.BoolIsChecked, LTreeViewItemCurrent);
            SetParentIsChecked(LTreeViewItemCurrent);
        }

        private void LTreeViewItemHeader_IOperationEvent(object sender, OperationEventArgs e)
        {
            UCTreeViewItem LCurrentItem = e.ObjectSource as UCTreeViewItem;
            SetItemChildParentIsChecked(LCurrentItem);
        }

        private void SetChildIsChecked(bool? ABoolValue, TreeViewItem ATreeViewItemCurrent)
        {
            foreach (TreeViewItem LTreeViewItemChild in ATreeViewItemCurrent.Items)
            {
                UCTreeViewItem LUCTreeViewItem = LTreeViewItemChild.Header as UCTreeViewItem;
                LUCTreeViewItem.SetIsChecked(ABoolValue);
                SetChildIsChecked(ABoolValue, LTreeViewItemChild);
            }
        }

        private void SetParentIsChecked(TreeViewItem ATreeViewItemCurrent)
        {
            int LIntAllChildItem = 0;
            int LIntAllCheckedItem = 0;
            int LIntAllUnCheckedItem = 0;
            int LIntAllNullCheckedItem = 0;

            object LObjectParent = ATreeViewItemCurrent.Parent;
            if (LObjectParent.GetType() != typeof(TreeViewItem)) { return; }
            TreeViewItem LTreeViewItemParent = LObjectParent as TreeViewItem;
            LIntAllChildItem = LTreeViewItemParent.Items.Count;
            foreach (TreeViewItem LTreeViewItemChild in LTreeViewItemParent.Items)
            {
                UCTreeViewItem LUCTreeViewItem = LTreeViewItemChild.Header as UCTreeViewItem;
                if (LUCTreeViewItem.BoolIsChecked == true) { LIntAllCheckedItem += 1; }
                if (LUCTreeViewItem.BoolIsChecked == false) { LIntAllUnCheckedItem += 1; }
                if (LUCTreeViewItem.BoolIsChecked == null) { LIntAllNullCheckedItem += 1; LIntAllCheckedItem += 1; }
            }
            UCTreeViewItem LTreeViewItemParentHeader = LTreeViewItemParent.Header as UCTreeViewItem;

            if (LIntAllCheckedItem == LIntAllChildItem) { LTreeViewItemParentHeader.SetIsChecked(true); }
            if (LIntAllCheckedItem > 0 && LIntAllCheckedItem != LIntAllChildItem) { LTreeViewItemParentHeader.SetIsChecked(null); }
            if (LIntAllCheckedItem == 0) { LTreeViewItemParentHeader.SetIsChecked(false); }
            if (LIntAllNullCheckedItem > 0) { LTreeViewItemParentHeader.SetIsChecked(null); }

            SetParentIsChecked(LTreeViewItemParent);
        }
    }
}
