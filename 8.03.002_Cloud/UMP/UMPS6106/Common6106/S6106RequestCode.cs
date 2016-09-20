using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common6106
{
    public enum S6106RequestCode
    {
        //获得配置的统计天数
        GetParam = 61061,
        //通话量统计
        GetRecordCount = 61062,
        //获得UMP使用情况统计
        GetUMPUsedCount = 61063,
        //录音时长统计
        GetRecordLength = 61064,
        //获得录音模式  （分机、坐席）
        GetRecordMode = 61065,
        //获得当前用户所在的机构
        GetOrgInfo = 61066,
        //获得质检数量
        GetQutityCount=61067,
        //获得申诉数量 
        GetAppealCount=61068,
        GetWarningCount=61069,
        GetReplayCount=610610,
        GetAvgScore=610611
    }

    public enum S6106WcfErrorCode
    {
        NoData = 6106901,
        GetUserPermissionFailed = 6106902,
        //获得录音模式错误  分机、坐席、混合模式
        GetRecordModeFailed = 6106903,
        //录音模式为空 
        RecordModeNull = 6106904,
        //获得的录音模式不正确
        RecordModeValueError = 6106905,
        //没有可管理的分机或坐席
        NoAgentOrExtension = 6106906,
    }
}
