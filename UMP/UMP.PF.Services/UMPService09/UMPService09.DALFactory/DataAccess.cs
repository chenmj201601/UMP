using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMPService09.Utility;
using UMPService09.Utility.InterFace;

namespace UMPService09.DALFactory
{
    public class DataAccess
    {
        /// <summary>
        /// Create Object get from Cache
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="CacheKey">The cache key.</param>
        /// <returns>System.Object.</returns>
        public static object CreateObject(string path, string CacheKey)
        {
            object objType = CacheHelper.Get(CacheKey);//get from Cache
            if (objType == null)
            {
                try
                {
                    objType = Assembly.Load(path).CreateInstance(CacheKey);//create reflection
                    CacheHelper.Max(CacheKey, objType);// to write in cache
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return objType;
        }
        /// <summary>
        /// create Statistics Interface
        /// </summary>
        /// <param name="name">Statistics Name</param>
        /// <returns>IStatistics.</returns>
        public static IStatistics CreateStatistic(string name)
        {
            name = string.IsNullOrWhiteSpace(name) ? StatisticsConstDefine.QMStatisticsName : name;
            string CacheKey = StatisticsConstDefine.DefaultAssemblyName + "." + name;
            object objType = CreateObject(StatisticsConstDefine.DefaultAssemblyName, CacheKey);
            return (IStatistics)objType;
        }
    }
}
