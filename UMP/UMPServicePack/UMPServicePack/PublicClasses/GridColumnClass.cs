using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPServicePack.PublicClasses
{
    /// <summary>
    /// listview column类
    /// </summary>
    public class GridColumnClass
    {
        public GridColumnClass()
        {
            ColWidth = 200;
        }
        private string _ColName;

        public string ColName
        {
            get { return _ColName; }
            set { _ColName = value; }
        }
        private string _ColDisPlay;

        public string ColDisPlay
        {
            get { return _ColDisPlay; }
            set { _ColDisPlay = value; }
        }
        private int _ColWidth;

        public int ColWidth
        {
            get { return _ColWidth; }
            set { _ColWidth = value; }
        }
    }
}
