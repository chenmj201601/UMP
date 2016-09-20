using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common2400
{
    //键值对实体
    public class StringValuePairs
    {
        private string _Key;

        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        private string _Value;

        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
    }

}
