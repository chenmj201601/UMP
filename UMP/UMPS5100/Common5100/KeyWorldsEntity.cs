using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common5100
{
    public class KeyWorldsEntity
    {
        private string _KeyWorldID;

        /// <summary>
        /// 关键词ID
        /// </summary>
        public string KeyWorldID
        {
            get { return _KeyWorldID; }
            set { _KeyWorldID = value; }
        }
        private string _KeyWorldContent;

        /// <summary>
        /// 关键词内容
        /// </summary>
        public string KeyWorldContent
        {
            get { return _KeyWorldContent; }
            set { _KeyWorldContent = value; }
        }
        private string _BookmarkLevelID;

        /// <summary>
        /// 关键词对应的等级ID
        /// </summary>
        public string BookmarkLevelID
        {
            get { return _BookmarkLevelID; }
            set { _BookmarkLevelID = value; }
        }
        private string _BookmarkLevelcolor;

        /// <summary>
        /// 关键词对应的等级颜色
        /// </summary>
        public string BookmarkLevelcolor
        {
            get { return _BookmarkLevelcolor; }
            set { _BookmarkLevelcolor = value; }
        }

        private string _LevelName;

        public string LevelName
        {
            get { return _LevelName; }
            set { _LevelName = value; }
        }
    }
}
