//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2a923793-58fb-4390-8cac-513ce9c16c4e
//        CLR Version:              4.0.30319.18063
//        Name:                     FooViewModel
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                TestDemo
//        File Name:                FooViewModel
//
//        created by Charley at 2014/4/4 16:59:21
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.ComponentModel;

namespace TestDemo
{
    public class GridTreeObject : INotifyPropertyChanged
    {
        #region Data

        bool? mIsChecked = false;
        GridTreeObject mParent;

        #endregion // Data

        public GridTreeObject(string name)
        {
            Name = name;
            Children = new List<GridTreeObject>();
        }

        public void Initialize()
        {
            foreach (GridTreeObject child in Children)
            {
                child.mParent = this;
                child.Initialize();
            }
        }

        #region Properties

        public List<GridTreeObject> Children { get; private set; }

        public string Name { get; private set; }

        #region IsChecked
        /// <summary>
        /// Gets/sets the state of the associated UI toggle (ex. CheckBox).
        /// The return value is calculated based on the check state of all
        /// child FooViewModels.  Setting this property to true or false
        /// will set all children to the same check state, and setting it 
        /// to any value will cause the parent to verify its check state.
        /// </summary>
        public bool? IsChecked
        {
            get { return mIsChecked; }
            set { SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == mIsChecked)
                return;
            mIsChecked = value;
            if (updateChildren && mIsChecked.HasValue)
                Children.ForEach(c => c.SetIsChecked(mIsChecked, true, false));
            if (updateParent && mParent != null)
                mParent.VerifyCheckState();
            OnPropertyChanged("IsChecked");
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < Children.Count; ++i)
            {
                bool? current = Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            SetIsChecked(state, false, true);
        }

        #endregion // IsChecked

        #endregion // Properties

        #region INotifyPropertyChanged Members

        protected void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
