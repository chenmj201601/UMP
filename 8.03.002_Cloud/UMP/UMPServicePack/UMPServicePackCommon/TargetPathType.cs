using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPServicePackCommon
{
    /// <summary>
    /// 目标路径类型
    /// </summary>
    public enum TargetPathType
    {
        //安装包的安装路径 
        InstallPath = 0,
        //系统路径（32bit系统：C:\Windows\System ; 64bit系统：C:\Windows\System32）
        WinSysDir=1,
        //ProgramData路径
        ProgramData=3,
    }
}
