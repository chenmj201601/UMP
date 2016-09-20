using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31081
{
    /// <summary>
    /// S3108消息码
    /// </summary>
    public enum S3108Codes
    {

        /// <summary>
        /// 获取质检参数
        /// </summary>
        GetQualityParam = 101,

        /// <summary>
        /// 保存参数配置
        /// </summary>
        SaveQualityParam = 103,

        /// <summary>
        /// 获取该用户可管理的参数
        /// </summary>
        GetAuthorityParam = 102,

        /// <summary>
        /// 获取机构
        /// </summary>
        GetOrganizationList = 104,

        /// <summary>
        /// 获取技能组信息
        /// </summary>
        GetSkillGroupList = 105,

        /// <summary>
        /// 获得统计参数的信息,从T_31_050表
        /// </summary>
        GetStatisticalParam = 1,

        /// <summary>
        /// 获得统计参数子项的信息,从T_31_051表
        /// </summary>
        GetStatisticalParamItem = 2,

        /// <summary>
        /// 获得所有可组合参数子项
        /// </summary>
        GetAllCombinedParamItems = 3,

        /// <summary>
        /// 获得已加到大项参数的子项
        /// </summary>
        GetAddedCombinedParamItems = 4,

        /// <summary>
        /// 保存已添加的参数子项的信息(存放到T_31_051表中)
        /// </summary>
        SaveAddedParamItemsInfos = 5,

        /// <summary>
        /// 根据点击的参数大项获得这个参数大项里面的参数子项信息,从T_31_051里获取
        /// </summary>
        GetSelectParamItemsInfos = 6,

        /// <summary>
        /// 获取设置的统计参数子项的值,从T_31_044里获取
        /// </summary>
        GetParamItemsValue = 7,

        /// <summary>
        /// 修改可组合的参数子项的值,在设计参数子项和参数大项的时候  (向T_31_044表Add或Delete一列记录) 
        /// </summary>
        ModifyCombinedParamItems = 8,

        /// <summary>
        /// 获取所有参数子项的值(从T_31_044表中获取),根据参数大项ID来获取
        /// </summary>
        GetAllParamItemsValue = 9,

         /// <summary>
        /// 将参数配置界面上写的数据保存到数据库
        /// </summary>
        SaveParamItemValue = 10,

        /// <summary>
        /// 根据在界面上修改的是否启用来 修改T_31_050的C006字段
        /// </summary>
        ModifyStatisticParam = 11,

        /// <summary>
        /// 该参数[大项]是否分配到机构或者技能组
        /// </summary>
        IsDistributeOrgSkg=12,

        /// <summary>
        /// 保存机构和大项的关系
        /// </summary>
        SaveConfig = 100,

        /// <summary>
        /// 获取机构和大项关系52表
        /// </summary>
        GetOrgItemRelation = 99,

        /// <summary>
        /// 获取大项50表
        /// </summary>
        GetABCDList = 98,

        /// <summary>
        /// 获取小项51表
        /// </summary>
        GetABCDItemList = 97,

        /// <summary>
        /// 获取大项信息52表
        /// </summary>
        GetABCDInfo = 96,

        /// <summary>
        /// 删除绑定设置
        /// </summary>
        DeleteConfig=95
    }
}
