using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31081
{
    /// <summary>
    /// 统计参数子项的类型
    /// </summary>
    public enum StatisticalParamItemType
    {
        Unkown = 0,
        SimpleText = 1,
        CheckSelect = 2,
        MultiText = 3,
        DropDownEnum = 4,
        MultiSelect = 5,
        ComplexText = 6
    }
}
