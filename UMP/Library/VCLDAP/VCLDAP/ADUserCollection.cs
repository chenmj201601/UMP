using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.VCLDAP
{  /**/
    /// <summary>
    /// ADUser集合类
    /// </summary>
    public class ADUserCollection : System.Collections.CollectionBase
    {

        /// <summary>
        /// 局部变量
        /// </summary>
        #region 局部变量
        bool _blnIsReadOnly = false;
        #endregion

        /**/
        /// <summary>
        /// 构造函数
        /// </summary>
        public ADUserCollection() { }

        /**/
        /// <summary>
        /// 返回ADUser对象
        /// </summary>
        public ADUser this[int index]
        {
            get
            {
                return (ADUser)this.List[index];
            }
            set
            {
                this.List.Add(value);
            }
        }


        /**/
        /// <summary>
        /// 填加ADUser对象
        /// </summary>
        /// <param name="value">ADUser实体</param>
        /// <returns>它所在的位置</returns>
        public int Add(ADUser value)
        {
            if (!this._blnIsReadOnly)
            { // 如果可写
                return this.List.Add(value);
            }
            else
            {
                throw new Exception("对象被写保护");
            }
        }

        /**/
        /// <summary>
        /// 是否可写(false:可写,true:不可写)
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return this._blnIsReadOnly;
            }
        }
    }
}
