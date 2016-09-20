//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ebb553a5-f439-4f25-9331-1bec17fdb891
//        CLR Version:              4.0.30319.18444
//        Name:                     ListSorter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Common
//        File Name:                ListSorter
//
//        created by Charley at 2014/9/16 10:25:32
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Reflection;

namespace VoiceCyber.Common
{
    /// <summary>
    /// 对集合进行排序，如
    /// List<!--<User> users=new List<User>(){.......}-->
    /// <!--ListSorter.SortList<list<User>,User>(ref users,"Name",SortDirection.Ascending);-->
    /// </summary>
    public class ListSorter
    {
        /// <summary>
        /// 对集合进行排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="property"></param>
        /// <param name="direction"></param>
        /// <typeparam name="TCollection"></typeparam>
        /// <typeparam name="TItem"></typeparam>
        public static void SortList<TCollection, TItem>(ref TCollection list, string property, SortDirection direction) where TCollection : IList<TItem>
        {
            PropertyInfo[] propertyinfos = typeof(TItem).GetProperties();
            foreach (PropertyInfo propertyinfo in propertyinfos)
            {
                if (propertyinfo.Name == property)          //取得指定的排序属性
                {
                    QuickSort<TCollection, TItem>(ref list, 0, list.Count - 1, propertyinfo, direction);
                }
            }
        }
        /// <summary>
        /// 快速排序算法
        /// </summary>
        /// <typeparam name="TCollection"><!--集合类型，需要实现Ilist<T>集合--></typeparam>
        /// <typeparam name="TItem">集合中对象的类型</typeparam>
        /// <param name="list">集合对象</param>
        /// <param name="left">起始位置，从0开始</param>
        /// <param name="right">终止位置</param>
        /// <param name="propertyinfo">集合中对象的属性，属性必须要实现IComparable接口</param>
        /// <param name="direction">排序类型（升序或降序）</param>
        private static void QuickSort<TCollection, TItem>(ref TCollection list, int left, int right, PropertyInfo propertyinfo, SortDirection direction) where TCollection : IList<TItem>
        {
            if (left < right)
            {
                int i = left, j = right;
                TItem key = list[left];
                while (i < j)
                {
                    if (direction == SortDirection.Ascending)
                    {
                        while (i < j && ((IComparable)propertyinfo.GetValue(key, null)).CompareTo(propertyinfo.GetValue(list[j], null)) < 0)
                        {
                            j--;
                        }
                        if (i < j)
                        {
                            list[i] = list[j];
                            i++;
                        }

                        while (i < j && ((IComparable)propertyinfo.GetValue(key, null)).CompareTo(propertyinfo.GetValue(list[i], null)) > 0)
                        {
                            i++;
                        }
                        if (i < j)
                        {
                            list[j] = list[i];
                            j--;
                        }
                        list[i] = key;
                    }
                    else
                    {
                        while (i < j && ((IComparable)propertyinfo.GetValue(key, null)).CompareTo(propertyinfo.GetValue(list[j], null)) > 0)
                        {
                            j--;
                        }
                        if (i < j)
                        {
                            list[i] = list[j];
                            i++;
                        }

                        while (i < j && ((IComparable)propertyinfo.GetValue(key, null)).CompareTo(propertyinfo.GetValue(list[i], null)) < 0)
                        {
                            i++;
                        }
                        if (i < j)
                        {
                            list[j] = list[i];
                            j--;
                        }
                        list[i] = key;
                    }
                }
                //执行递归调用
                QuickSort<TCollection, TItem>(ref list, left, i - 1, propertyinfo, direction);
                QuickSort<TCollection, TItem>(ref list, i + 1, right, propertyinfo, direction);
            }
        }
    }
    /// <summary>
    /// 排序类型
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// 升序
        /// </summary>
        Ascending,
        /// <summary>
        /// 降序
        /// </summary>
        Descending
    }
}
