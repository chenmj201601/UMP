using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;

namespace VoiceCyber.VCLDAP
{
    public class ADObject : IADObject
    {
        /**/
        /// <summary>
        /// 对AD进行处理所要用的工具
        /// </summary>
        public static IADUtility Utility;

        /**/
        /// <summary>
        /// 当前DirectoryEntry实例对象
        /// </summary>
        private DirectoryEntry m_directoryEntry;

        public ADObject()
        {
        }
        /**/
        /// <summary>
        /// 通过DirectoryEntry对象实例化该类
        /// </summary>
        /// <param name="directoryEntry">directoryEntry对象</param>
        public ADObject(DirectoryEntry directoryEntry)
        {
            this.m_directoryEntry = directoryEntry;
        }

        /// <summary>
        /// IADObject 成员
        /// </summary>
        #region IADObject 成员
        public string AdsPath
        {
            get
            {
                return this.m_directoryEntry.Path;
            }
        }

        /**/
        /// <summary>
        /// 返回Name
        /// </summary>
        public string Name
        {
            get
            {
                return this.m_directoryEntry.Name;
            }
        }

        /**/
        /// <summary>
        /// 返回唯一Guid
        /// </summary>
        public Guid Guid
        {
            get
            {
                return this.m_directoryEntry.Guid;
            }
        }

        #endregion


        /**/
        /// <summary>
        /// 设置指定属性的值
        /// </summary>
        /// <param name="property">属性名</param>
        /// <param name="value">更新数据</param>
        public void SetProperty(string property, object value)
        {
            int propertyCount;
            try
            {
                propertyCount = this.m_directoryEntry.Properties[property].Count;
            }
            catch
            {
                throw (new ADPropertyException("当前对象没有'"
                    + property + "'属性，操作不被允许。"));
            }
            if (propertyCount != 0)
            {
                this.m_directoryEntry.Properties[property][0] = value;
            }
            else
            {
                this.m_directoryEntry.Properties[property].Add(value);
            }
            try
            {
                this.m_directoryEntry.CommitChanges();
            }
            catch
            {
                throw (new ADPropertyException("更新属性'"
                    + property + "'失败，操作不被允许，"
                    + "可能是当前用户没有权限。"));
            }
        }

        /**/
        /// <summary>
        /// 获取指定属性名的第一个值
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public object GetProperty(string property)
        {
            int propertyCount;
            try
            {
                propertyCount = this.m_directoryEntry.Properties[property].Count;
            }
            catch
            {
                throw (new ADPropertyException("当前对象没有'"
                    + property + "'属性，无法取得该值。"));
            }
            if (propertyCount != 0)
            {
                try
                {
                    return this.m_directoryEntry.Properties[property][0];
                }
                catch (Exception exc)
                {
                    throw new ADException("读取" + property + "属性时出错。", exc);
                }
            }
            else
            {
                return "";
            }
        }

        public System.Collections.IList GetProperties(string property)
        {
            System.Collections.IList ps = null;
            int propertyCount;
            try
            {
                propertyCount = this.m_directoryEntry.Properties[property].Count;
            }
            catch
            {
                throw (new ADPropertyException("当前对象没有'"
                    + property + "'属性，无法取得该值。"));
            }
            if (propertyCount >= 0)
            {
                ps = new System.Collections.ArrayList();
                for (int index = 0; index < propertyCount; index++)
                {
                    ps.Add(this.m_directoryEntry.Properties[property][index]);
                }
            }
            return ps;
        }

        /**/
        /// <summary>
        /// 获得当前对象的DirectoryEntry表示形式
        /// </summary>
        public DirectoryEntry MyDirectoryEntry
        {
            get
            {
                return this.m_directoryEntry;
            }
            set
            {
                if (value == null)
                {
                    throw (new ADObjectEmptyException("初始化ADObject出错，"
                        + "请确认初始化使用的数据是否有效。"));
                }
                this.m_directoryEntry = value;
            }
        }
    }
}
