using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.VCLDAP
{
    /**/
    /// <summary>
    /// IADObject活动目录基本接口
    /// </summary>
    public interface IADObject
    {
        /**/
        /// <summary>
        /// 获得显示名
        /// </summary>
        string Name
        {
            get;
        }

        /**/
        /// <summary>
        /// 获得adsPath路径
        /// </summary>
        string AdsPath
        {
            get;
        }

        /**/
        /// <summary>
        /// 获取 DirectoryEntry 的全局唯一标识符 (GUID)。
        /// </summary>
        Guid Guid
        {
            get;
        }
    }
}
