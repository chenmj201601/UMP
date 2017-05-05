using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common5100
{
    public class BookmarkLevelEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        private string _BookmarkLevelID;

        public string BookmarkLevelID
        {
            get { return _BookmarkLevelID; }
            set { _BookmarkLevelID = value; }
        }

        private string _BookmarkLevelName;

        /// <summary>
        /// 标签等级名称
        /// </summary>
        public string BookmarkLevelName
        {
            get { return _BookmarkLevelName; }
            set { _BookmarkLevelName = value; }
        }
        private string _BookmarkLevelColor;

        /// <summary>
        /// 标签等级颜色
        /// </summary>
        public string BookmarkLevelColor
        {
            get { return _BookmarkLevelColor; }
            set { _BookmarkLevelColor = value; }
        }

        private string _BookmarkLevelStatus;

        /// <summary>
        /// 等级是否可用 可用：1 不可用：0
        /// </summary>
        public string BookmarkLevelStatus
        {
            get { return _BookmarkLevelStatus; }
            set { _BookmarkLevelStatus = value; }
        }
    }
}
