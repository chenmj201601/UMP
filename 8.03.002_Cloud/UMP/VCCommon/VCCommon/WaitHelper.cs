//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5c59138f-e109-4d96-ac02-9641845b3a63
//        CLR Version:              4.0.30319.18063
//        Name:                     WaitHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Common
//        File Name:                WaitHelper
//
//        created by Charley at 2014/3/22 18:11:26
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace VoiceCyber.Common
{
    /// <summary>
    /// 一个处理等待对象的帮助类
    /// 这是一个静态类
    /// </summary>
    public static class WaitHelper
    {
        /// <summary>
        /// 暂存等待对象的集合
        /// </summary>
        public static List<WaitObject> ListWaitObject = new List<WaitObject>();
        /// <summary>
        /// 最长等待时间，秒
        /// </summary>
        public static int MaxTime = 60;          //最长等待1分钟
        private static int mCount;

        /// <summary>
        /// 向集合中插入一个等待对象
        /// </summary>
        /// <param name="waitObject">等待对象</param>
        public static void InsertWaitObject(WaitObject waitObject)
        {
            var temp = ListWaitObject.FirstOrDefault(w => w.Name == waitObject.Name);
            if (temp != null)
            {
                ListWaitObject.Remove(temp);
            }
            waitObject.Result = false;
            waitObject.IsFinished = false;
            ListWaitObject.Add(waitObject);
        }
        /// <summary>
        /// 执行等待操作，即阻塞当前线程，直到指定等待对象的IsFinished属性为true
        /// </summary>
        /// <param name="waitObject">等待对象</param>
        public static void DoWaitObject(WaitObject waitObject)
        {
            mCount = 0;
            while (true)
            {
                if (waitObject.IsFinished || mCount > MaxTime)
                {
                    break;
                }
                mCount++;
                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// 执行等待操作，即阻塞当前线程，直到指定等待对象的IsFinished属性为true，可以指定最长等待时间
        /// </summary>
        /// <param name="waitObject">等待对象</param>
        /// <param name="maxTime">最长等待时间</param>
        public static void DoWaitObject(WaitObject waitObject, int maxTime)
        {
            mCount = 0;
            while (true)
            {
                if (maxTime == 0 && !waitObject.IsFinished)
                {
                    Thread.Sleep(1000);
                }
                if (waitObject.IsFinished || mCount > MaxTime)
                {
                    break;
                }
                mCount++;
                Thread.Sleep(1000);
            }
        }
    }
}
