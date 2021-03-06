﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3103.Models
{
    public class ObjectItems : CheckableItemBase
    {
        public long ObjID { get; set; }
        public string Name { get; set; }
        public int ObjType { get; set; }
        public string Description { get; set; }
        public object Data { get; set; }
    }
}
