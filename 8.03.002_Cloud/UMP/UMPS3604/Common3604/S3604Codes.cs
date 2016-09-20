namespace Common3604
{
    public enum S3604Codes
    {
        /// <summary>
        /// 获取权限
        /// </summary>
        GetUserOperationList = 0,
        GetControlAgentInfoList = 1,
        GetControlOrgInfoList = 2,

        /// <summary>
        /// 获取目录
        /// </summary>
        OptGetContents,

        /// <summary>
        /// 创建目录
        /// </summary>
        OptCreateContents,

        /// <summary>
        /// 修改目录
        /// </summary>
        OptChangeContents,

        /// <summary>
        /// 删除目录
        /// </summary>
        OptDeleteContents,
    }
}
