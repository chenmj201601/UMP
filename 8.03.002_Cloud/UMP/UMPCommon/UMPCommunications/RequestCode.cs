//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c6deed6a-daac-4eba-b644-923f71553cfa
//        CLR Version:              4.0.30319.18444
//        Name:                     RequestCode
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                RequestCode
//
//        created by Charley at 2014/9/25 15:47:37
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// Web请求消息代码，一般用于子系统之间的通讯
    /// CS          10001 ~ 19999       子系统（客户端）向主系统（服务端）请求的消息代码
    /// SC          20001 ~ 29999       主系统（服务端）向子系统（客户端）通知的消息代码
    /// WS          30001 ~ 39999       公共WCF请求的消息码
    /// NC          40001 ~ 49999       网络通许（Socket通讯）的公共命令
    /// AC          50001 ~ 59999       模块间通讯的公共命令（CompositeWPF），注：部分命令与CS，SC共用
    /// </summary>
    public enum RequestCode
    {
        /// <summary>
        /// 不确定
        /// </summary>
        Unkown = 0,

        #region CS

        /// <summary>
        /// 在PageHead上点了Home按钮，将转到主页面
        /// </summary>
        CSHome = 10001,
        /// <summary>
        /// 在PageHead上点了修改密码的按钮，
        /// 主系统将隐藏当前子系统，弹出修改密码窗口
        /// </summary>
        CSChangePassword = 10002,
        /// <summary>
        /// 在PageHead上点了Logout按钮，
        /// 主系统将注销当前登录用户返回到用户登录界面
        /// </summary>
        CSLogout = 10003,
        /// <summary>
        /// 在PageHead上点了Exit按钮，
        /// 主系统将退出并关闭
        /// </summary>
        CSExit = 10004,

        /// <summary>
        /// 子模块通知主模块全局参数有更新
        /// 参数：
        /// ListData1：参数名（ConstValue中定义）
        /// ListData2：新值
        /// ListData3：旧值
        /// </summary>
        CSGlobalSettingChanged = 10101,

        /// <summary>
        /// 子模块开始加载
        /// 通常当子系统刚刚加载时向主系统发送此消息
        /// 此消息的返回值可以附带特定操作编码，从而告知子系统Load完成后执行什么操作
        /// </summary>
        CSModuleLoading = 11001,
        /// <summary>
        /// 子模块加载完成
        /// 通常当子系统第一个页面的Page_Load完成时向主系统发送此消息
        /// </summary>
        CSModuleLoaded = 11002,
        /// <summary>
        /// 子模块关闭
        /// 通常在子模块程序结束，也就是App_Exist事件向主系统发送此消息
        /// </summary>
        CSModuleClose = 11003,
        /// <summary>
        /// 子系统中需要跳转到其他模块时，向主系统发送此消息
        /// 主系统将关闭当前系统，加载指定的子系统
        /// </summary>
        CSModuleNavigate = 11010,

        /// <summary>
        /// 在PageHead上切换了主题，通知主模块切换新的主题
        /// 更新SessionInfo的ThemeInfo
        /// </summary>
        CSThemeChange = 12001,
        /// <summary>
        /// 在PageHead上切换了语言，通知主模块切换新的语言类型
        /// 更新SessionInfo的LangTypeInfo
        /// </summary>
        CSLanguageChange = 12002,
        /// <summary>
        /// 在PageHead上切换了角色，通知主模块切换到新的角色
        /// 更新SessionInfo的RoleInfo
        /// </summary>
        CSRoleChange = 12003,

        /// <summary>
        /// 子模块向主模块发送检查界面无操作的消息
        /// 参数：Data：Count（计数次数）
        /// </summary>
        CSIdleCheck = 13001,

        /// <summary>
        /// 监视端请求监视消息（由监视终端发给主模块或子模块）
        /// 参数：
        /// Data    命令（有各个子模块自主定义，格式为五位数值）
        /// ListData
        /// 0       参数1
        /// 1       参数2
        /// 2       参数3
        /// ......
        /// </summary>
        CSMonitor = 14001,

        /// <summary>
        /// 激活已经启动的进程
        /// 参数：
        /// Data    激活进程时的命令（各模块自主定义，相当于触发的事件代码）
        /// ListData...     其他参数（比如命令行参数）    
        /// </summary>
        CSActiveProcess = 14101,

        #endregion


        #region SC

        /// <summary>
        /// 当前登录用户的密码已经更改
        /// 客户端需要更新SessionInfo中登录用户的密码
        /// 参数：Data：NewPassword（新密码）
        /// </summary>
        SCChangePassword = 20002,
        /// <summary>
        /// 主系统通知子系统当前用户注销，
        /// 子系统需要结束
        /// </summary>
        SCLogout = 20003,

        /// <summary>
        /// 主模块通知子模块全局参数有更新
        /// ListData1：参数名（ConstValue中定义）
        /// ListData2：新值
        /// ListData3：旧值
        /// </summary>
        SCGlobalSettingChanged = 20101,

        /// <summary>
        /// 主系统通知子系统执行指定的操作
        /// ***暂定***
        /// 由于重新打开不会发生子模块重新加载，所以子模块通过此消息判断应该执行什么操作
        /// 此消息的参数是本模块下的子模块编号（如：2401）
        /// </summary>
        SCOperation = 21010,


        /// <summary>
        /// 主系统通知子系统切换主题
        /// </summary>
        SCThemeChange = 22001,
        /// <summary>
        /// 主系统通知子系统切换语言
        /// </summary>
        SCLanguageChange = 22002,
        /// <summary>
        /// 主系统通知子系统切换角色
        /// </summary>
        SCRoleChange = 22003,

        /// <summary>
        /// 主模块重新打开之前已经打开的子模块发送此消息
        /// 由于重新打开不会发生子模块重新加载，所以子模块通过此消息判断应该执行什么操作
        /// 此消息的参数是本模块下的子模块编号（如：2401）
        /// </summary>
        SCReOpenModule = 22101,

        /// <summary>
        /// 主系统通知子系统启动检查空闲状态的计时器
        /// </summary>
        SCIdleCheckStart = 23002,
        /// <summary>
        /// 主系统通知子系统停止检查空闲状态的计时器
        /// </summary>
        SCIdleCheckStop = 23003,

        #endregion


        #region WS

        /// <summary>
        /// 获取语言列表
        /// </summary>
        WSGetLangList = 30001,

        /// <summary>
        /// 获取操作列表
        /// </summary>
        WSGetUserOptList = 30011,
        /// <summary>
        /// 获取用户管理的对象列表
        /// </summary>
        WSGetUserObjList = 30012,

        /// <summary>
        /// 获取用户管理座席的列表
        /// </summary>
        WSGetAgentObjList = 30013,

        /// <summary>
        /// 获取基础数据信息列表
        /// </summary>
        WSGetBasicDataInfoList = 30014,
        /// <summary>
        /// 功能同WSGetUserObjList，返回的是ResourceObject对象的列表
        /// </summary>
        WSGetUserCtlObjList = 30015,

        /// <summary>
        /// 获取视图列信息列表
        /// </summary>
        WSGetUserViewColumnList = 30021,

        /// <summary>
        /// 获取系统流水号
        /// </summary>
        WSGetSerialID = 31001,

        /// <summary>
        /// 插入临时表数据
        /// </summary>
        WSInsertTempData = 31011,
        /// <summary>
        /// 写操作日志
        /// </summary>
        WSWriteOperationLog = 31012,

        /// <summary>
        /// 获取自定义布局信息
        /// </summary>
        WSGetLayoutInfo = 31021,
        /// <summary>
        /// 保存自定义布局信息
        /// </summary>
        WSSaveLayoutInfo = 31022,

        /// <summary>
        /// 获取资源特定属性信息
        /// </summary>
        WSGetResourceProperty = 32001,

        /// <summary>
        /// 获取全局参数信息
        /// </summary>
        WSGetGlobalParamList = 33001,
        /// <summary>
        /// 获取用户参数信息
        /// </summary>
        WSGetUserParamList = 33002,
        /// <summary>
        /// 获取全局参数信息（2015/11/18新增，功能与WSGetGlobalParamList相同，参数定义不同）
        /// </summary>
        WSGetGlobalParamList2 = 33003,
        /// <summary>
        /// 保存用户参数信息
        /// </summary>
        WSSaveUserParamList = 33012,

        /// <summary>
        /// 获取数据库信息
        /// 
        /// 参数：
        /// 0：UserID（保留，可以指定为0）
        /// 
        /// 返回值：
        /// ListData0：数据库类型
        /// ListData1：服务器地址
        /// ListData2：端口
        /// ListData3：数据库名或服务名
        /// ListData4：登录用户名
        /// ListData5：登录密码（M004加密）
        /// </summary>
        WSGetDBInfo = 35001,

        #endregion


        #region NC

        /// <summary>
        /// 客户端连接服务器后向服务器发送的第一个消息，
        /// 通常这个消息附带客户端的一些环境信息
        /// </summary>
        NCHello = 40001,
        /// <summary>
        /// 服务器在接收到客户端的Hello消息后做出的回复，
        /// 通常这个消息附带服务端的一些环境信息
        /// </summary>
        NCWelcome = 40002,
        /// <summary>
        /// 客户端的认证登录信息，通常附带有登录的用户名，密码后认证码等
        /// </summary>
        NCLogon = 40003,
        /// <summary>
        /// 认证后服务端的回复信息，指示是否认证成功
        /// </summary>
        NCAuth = 40004,
        /// <summary>
        /// 一般错误消息
        /// </summary>
        NCError = 41001,
        /// <summary>
        /// 心跳信息
        /// </summary>
        NCHearbeat = 42001,

        #endregion


        #region AC

        /// <summary>
        /// 登录系统
        /// </summary>
        ACLoginLoginSystem = 50001,
        /// <summary>
        /// 注销登录，并向所有模块发布注销登录的消息
        /// </summary>
        ACLoginLogout = 50002,
        /// <summary>
        /// 处理用户在线消息，由其他模块发布给登录模块
        /// </summary>
        ACLoginOnline = 50003,
        /// <summary>
        /// 注销登录，向登录模块发布注销登录消息
        /// </summary>
        ACPageHeadLogout = 51001,
        /// <summary>
        /// 关闭左侧菜单列
        /// </summary>
        ACPageHeadLeftPanel = 51002,
        /// <summary>
        /// 设置默认页
        /// </summary>
        ACPageHeadDefaultPage = 51003,
        /// <summary>
        /// 设置IM状态（消息数量）
        /// ListData0: 是否显示消息数（0：不显示；1：显示）
        /// ListData1：消息数量
        /// </summary>
        ACPageHeadSetIMState = 51004,
        /// <summary>
        /// 打开IM窗口
        /// </summary>
        ACPageHeadOpenIMPanel = 51005,
        /// <summary>
        /// 转到指定模块
        /// ListData
        /// 0：  要转到的模块的AppID
        /// 1：  启动参数
        /// </summary>
        ACTaskNavigateApp = 52001,
        /// <summary>
        /// 重新打开指定的模块
        /// 0：  要转到的模块的AppID
        /// 1：  启动参数
        /// </summary>
        ACTaskReNavigateApp = 52002,
        /// <summary>
        /// 设置状态消息
        /// ListData
        /// 0：  IsWorking
        /// 1：  Message
        /// </summary>
        ACStatusSetStatus = 53001,

        #endregion

    }
}
