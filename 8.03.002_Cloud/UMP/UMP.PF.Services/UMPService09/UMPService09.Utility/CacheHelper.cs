using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace UMPService09.Utility
{
    /// <summary>
    /// CacheHelper, gets or sets cache
    /// </summary>
    public class CacheHelper
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="CacheHelper" /> class from being created.
        /// </summary>
        private CacheHelper()
        {
        }
        /// <summary>
        /// The lock object
        /// </summary>
        static Object objlock = new Object();
        /// <summary>
        /// The cache
        /// </summary>
        private static readonly Cache _cache;
        /// <summary>
        /// The default cache saved seconds
        /// </summary>
        private static int defaultSeconds = 120 * 60;
        /// <summary>
        /// Reset the defaultSeconds.
        /// </summary>
        /// <param name="cacheFactor">The cache saved seconds.</param>
        public static void ReSetSeconds(int cacheFactor)
        {
            defaultSeconds = cacheFactor;
        }
        /// <summary>
        /// Static initializer should ensure we only have to look up the current cache
        /// instance once.
        /// </summary>
        static CacheHelper()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                _cache = context.Cache;
            }
            else
            {
                _cache = HttpRuntime.Cache;
            }
        }
        /// <summary>
        /// Determines whether the key is in cache
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if [contains] [the specified key]; otherwise, <c>false</c>.</returns>
        public static bool Contains(string key)
        {
            bool isContained = false;
            lock (objlock)
            {
                if (_cache[key] != null)
                {
                    isContained = true;
                }
            }
            return isContained;
        }
        /// <summary>
        /// clears all the Cache objects
        /// </summary>
        public static void Clear()
        {
            lock (objlock)
            {
                IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
                while (CacheEnum.MoveNext())
                {
                    _cache.Remove(CacheEnum.Key.ToString());
                }
            }
        }
        /// <summary>
        /// Removes Cache by Pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        public static void RemoveByPattern(string pattern)
        {
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            lock (objlock)
            {
                IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
                while (CacheEnum.MoveNext())
                {
                    if (regex.IsMatch(CacheEnum.Key.ToString()))
                        _cache.Remove(CacheEnum.Key.ToString());
                }
            }
        }
        /// <summary>
        /// Removes Cache by key
        /// </summary>
        /// <param name="key">key</param>
        public static void Remove(string key)
        {
            lock (objlock)
            {
                _cache.Remove(key);
            }
        }
        /// <summary>
        /// Inserts an object into Cache
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="obj">object</param>
        public static void Insert(string key, object obj)
        {
            Insert(key, obj, null, defaultSeconds, CacheItemPriority.Normal);
        }
        /// <summary>
        /// Inserts an object into Cache with CacheDependency
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="obj">object</param>
        /// <param name="dep">Cache Dependency</param>
        public static void Insert(string key, object obj, CacheDependency dep)
        {
            Insert(key, obj, dep, defaultSeconds, CacheItemPriority.Normal);//MinuteFactor * 3
        }

        //把对象加载到Cache,附加过期时间信息
        /// <summary>
        /// Inserts an object into Cache with absoluteExpiration
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="obj">object</param>
        /// <param name="seconds">Cache saved time(s)</param>
        public static void Insert(string key, object obj, int seconds)
        {
            Insert(key, obj, null, seconds, CacheItemPriority.Normal);
        }

        //把对象加载到Cache,附加过期时间信息和优先级
        /// <summary>
        /// Inserts an object into Cache with absoluteExpiration and CacheItemPriority
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="obj">object</param>
        /// <param name="seconds">Cache saved times(s)</param>
        /// <param name="priority">priority</param>
        public static void Insert(string key, object obj, int seconds, CacheItemPriority priority)
        {
            Insert(key, obj, null, seconds, priority);
        }

        //把对象加载到Cache,附加缓存依赖和过期时间(多少秒后过期)
        // (默认优先级为Normal)
        /// <summary>
        /// Inserts an object into Cache with CacheDependency and cache saved time, default CacheItemPriority Normal
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="obj">object</param>
        /// <param name="dep">Cache Dependency</param>
        /// <param name="seconds">cache saved time(s)</param>
        public static void Insert(string key, object obj, CacheDependency dep, int seconds)
        {
            Insert(key, obj, dep, seconds, CacheItemPriority.Normal);
        }

        //把对象加载到Cache,附加缓存依赖和过期时间(多少秒后过期)及优先级
        /// <summary>
        /// Inserts an object into Cache with CacheDependency, cache saved times and CacheItemPriority
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="obj">object</param>
        /// <param name="dep">Cache Dependency</param>
        /// <param name="seconds">Cache saved time(s)</param>
        /// <param name="priority">Cache Priority</param>
        public static void Insert(string key, object obj, CacheDependency dep, int seconds, CacheItemPriority priority)
        {
            if (obj != null)
            {
                lock (objlock)
                {
                    _cache.Insert(key, obj, dep, DateTime.Now.AddSeconds(seconds), TimeSpan.Zero, priority, null);//Factor * 
                }
            }
        }

        //把对象加到缓存并忽略优先级
        /// <summary>
        /// Inserts an object into Cache, Ignore the Priority
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="obj">object</param>
        /// <param name="seconds">Cache saved times(s)</param>
        public static void MicroInsert(string key, object obj, int seconds)
        {
            if (obj != null)
            {
                lock (objlock)
                {
                    _cache.Insert(key, obj, null, DateTime.Now.AddSeconds(seconds), TimeSpan.Zero);
                }
            }
        }

        //把对象加到缓存,并把过期时间设为最大值
        /// <summary>
        /// Inserts an object into Cache, default absoluteExpiration values DateTime.MaxValue
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="obj">object</param>
        public static void Max(string key, object obj)
        {
            Max(key, obj, null);
        }

        //把对象加到缓存,并把过期时间设为最大值,附加缓存依赖信息
        /// <summary>
        /// Inserts an object into Cache with CacheDependency and default absoluteExpiration values DateTime.MaxValue
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="obj">object</param>
        /// <param name="dep">Cache Dependency</param>
        public static void Max(string key, object obj, CacheDependency dep)
        {
            if (obj != null)
            {
                lock (objlock)
                {
                    _cache.Insert(key, obj, dep, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.AboveNormal, null);
                }
            }
        }
        /// <summary>
        /// Inserts an Permanent object into Cache(default CacheItemPriority NotRemovable）
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="obj">object</param>
        public static void Permanent(string key, object obj)
        {
            Permanent(key, obj, null);
        }

        //插入持久性缓存,附加缓存依赖
        /// <summary>
        /// Inserts an Permanent object into Cache with CacheDependency(default CacheItemPriority NotRemovable）
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="obj">object</param>
        /// <param name="dep">Cache Dependency</param>
        public static void Permanent(string key, object obj, CacheDependency dep)
        {
            if (obj != null)
            {
                lock (objlock)
                {
                    _cache.Insert(key, obj, dep, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.NotRemovable, null);
                }
            }
        }

        //根据键获取被缓存的对象
        /// <summary>
        /// Gets cache value with the key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>cache value</returns>
        public static object Get(string key)
        {
            lock (objlock)
            {
                return _cache[key];
            }
        }
    }
}
