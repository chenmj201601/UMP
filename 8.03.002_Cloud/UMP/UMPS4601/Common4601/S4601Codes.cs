using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common4601
{
    /// <summary>
    /// S4601消息码
    /// </summary>
    public enum S4601Codes
    {
        /// <summary>
        /// 获取用户管理的机构列表
        /// </summary>
        GetControlOrgInfoList = 1,
        /// <summary>
        /// 获取用户管理的坐席列表
        /// </summary>
        GetControlAgentInfoList = 2,
        /// <summary>
        /// 获得用户管理的分机列表
        /// </summary>
        GetControlExtensionInfoList = 3,
        /// <summary>
        /// 获得用户管理的真实分机列表
        /// </summary>
        GetControlRealExtensionInfoList = 4,
        /// <summary>
        /// 获得用户管理的用户列表
        /// </summary>
        GetControlUserInfoList = 5,
        /// <summary>
        /// 获得用户管理的技能组列表
        /// </summary>
        GetControlSkillGroupInfoList = 6,
        /// <summary>
        /// 获得技能组里的对象
        /// </summary>
        GetControlObjectInfoListInSkillGroup = 7,
        /// <summary>
        /// 获得选中对象的KPI绑定的详情
        /// </summary>
        GetKpiMapObjectInfo = 8,
        /// <summary>
        /// 根据传入的获得KPI集合
        /// </summary>
        GetKPIInfoList = 9,
        /// <summary>
        /// 判断该对象是否绑定了当前选中的KPI
        /// </summary>
        IsBandingKpi =10,
        /// <summary>
        /// 保存对象绑定的kpi的信息
        /// </summary>
        SaveKpiMapObjectInfo=11,
        /// <summary>
        /// 根据对象ID以及他的父级机构来在绑定界面加载绑定的值
        /// </summary>
        GetKpiMapObjectInfoInBP = 12,
        /// <summary>
        /// 根据选中对象的来加载绑定到上面的KPI详情信息
        /// </summary>
        LoadKpiMapObjectInfo = 13,
        /// <summary>
        /// 删除对象中的Banding的内容(根据选中的记录列，去T_46_003表中删除相应数据)
        /// </summary>
        DeleteKpiMapObjectInfo = 14,
        /// <summary>
        /// 获得所有KPI详情
        /// </summary>
        GetAllKPIInfoLists =15,
        /// <summary>
        /// 变更KPI状态 修改T_46_001 的C009
        /// </summary>
        AlterState = 16,
        /// <summary>
        /// 修改默认值 T_46_001
        /// </summary>
        ModifyDefaultValue =17,


        /// <summary>
        /// 得到当前人能管理的技能组下的用户
        /// </summary>
        GetControlUserInfoListInSkill=18
    }
}
