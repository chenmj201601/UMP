//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    40cd27c6-1276-4a82-9d1f-e4c805c685d6
//        CLR Version:              4.0.30319.18444
//        Name:                     SelectedInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101.Models
//        File Name:                SelectedInfo
//
//        created by Charley at 2014/9/12 12:56:55
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS1103.Models
{
    public class SelectedInfo
    {
        public int ObjType { get; set; }

        public ICheckableItem Parent { get; set; }

        private List<ObjectItem> mListItems;

        public List<ObjectItem> ListItems
        {
            get { return mListItems; }
        }

        public SelectedInfo()
        {
            mListItems = new List<ObjectItem>();
        }
    }
}
