using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;

namespace VoiceCyber.VCLDAP
{
    /**/
    /// <summary>
    /// ADUtility 提供对AD进入查询，验证，填加等方法。
    /// </summary>
    public class ADUtility : IDisposable, IADUtility
    {
        private DirectoryEntry m_directoryEntry;
        public ADUtility()
        {
        }

        /**/
        /// <summary>
        /// ADUtility的构造函数
        /// </summary>
        /// <param name="LDAPPath">AD的连接路径</param>
        /// <param name="userName">登录域用户名(最好具有较高的权限)</param>
        /// <param name="password">登录密码</param>
        public ADUtility(string LDAPPath, string userName, string password)
        {
            // 得到整个活动目录
            m_directoryEntry = new DirectoryEntry(LDAPPath, userName, password, AuthenticationTypes.Secure);
        }

        /**/
        /// <summary>
        /// 通过用户账号名，得到ADUser对象
        /// </summary>
        /// <param name="loginName">用户登录名</param>
        /// <returns>返回一个ADUser对象</returns>
        public ADUser GetADUser(string loginName)
        {
            string userName;
            if (loginName.IndexOf("\\") > 0)
            {
                // 得到没有域名的登录名。
                userName = loginName.Substring(loginName.IndexOf("\\") + 1);
            }
            else
            {
                userName = loginName;
            }
            string ldapQueryString = ADLDAPFilterTemplate.GetFilterByLoginName(userName);
            ADUser user = new ADUser();
            user.MyDirectoryEntry = (this.GetOnlyOneDEObject(ldapQueryString));
            return user;
        }

        /**/
        /// <summary>
        /// 通来LDAP查询字符串，得到一个(only one)DirectoryEntry对象
        /// </summary>
        /// <param name="ldapQueryString">ldap searcher string</param>
        /// <example>
        /// ADUtility utility= new ADUtility();
        /// DirectoryEntry object = utility.GetOnlyOneDEObject("(&(objectclass=user))");
        /// </example>
        public DirectoryEntry GetOnlyOneDEObject(string ldapQueryString)
        {
            DirectorySearcher searcher = new DirectorySearcher();
            searcher.SearchRoot = this.m_directoryEntry;
            searcher.Filter = ldapQueryString;
            searcher.SearchScope = SearchScope.Subtree;
            // 只找一个对象
            SearchResult sr = searcher.FindOne();
            if (sr != null)
            {
                return sr.GetDirectoryEntry();
            }
            else
            {
                return null;
            }
        }
        #region version

        /**/
        /// <summary>
        /// 通过手机号查询得到一个ADUser
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <returns>ADUser</returns>
        public ADUser GetADUserByMobile(string mobile)
        {
            string ldapQueryString = ADLDAPFilterTemplate.GetADUserFilterByMobile(mobile);
            ADUser user = new ADUser();
            user.MyDirectoryEntry = (this.GetOnlyOneDEObject(ldapQueryString));
            return user;
        }

        /**/
        /// <summary>
        /// 得到所有的组
        /// </summary>
        /// <returns>ADGroupCollection集合对象</returns>
        /// <remarks>对Activex Directory的操作</remarks>
        /// <example>
        /// ADUtility utility = new ADUtility(.);
        /// ADGroupCollection = utility.GetAllGroups();
        /// </example>
        public ADGroupCollection GetAllGroups()
        {
            ADGroupCollection gc = new ADGroupCollection();
            System.Collections.Generic.List<DirectoryEntry> list = this.GetAllDEObject(ADLDAPFilterTemplate.GetGroupQueryByName("*"));
            for (int i = 0; i < list.Count; i++)
            {
                gc.Add(new ADGroup(list[i]));
            }
            return gc;
        }


        /**/
        /// <summary>
        /// 得到所有的ou
        /// </summary>
        /// <returns>ADGroupCollection集合对象</returns>
        /// <remarks>对Activex Directory的操作</remarks>
        /// <example>
        /// ADUtility utility = new ADUtility(.);
        /// ADGroupCollection = utility.GetAllGroups();
        /// </example>
        public ADGroupCollection GetAllOUs()
        {
            ADGroupCollection gc = new ADGroupCollection();
            System.Collections.Generic.List<DirectoryEntry> list = this.GetAllDEObject(ADLDAPFilterTemplate.GetOUQueryByName("*"));
            for (int i = 0; i < list.Count; i++)
            {
                gc.Add(new ADGroup(list[i]));
            }
            return gc;
        }



        /**/
        /// <summary>
        /// 得到所有的OU
        /// </summary>
        /// <returns>ADGroupCollection集合对象</returns>
        /// <remarks>对Activex Directory的操作</remarks>
        /// <example>
        /// ADUtility utility = new ADUtility(.);
        /// ADGroupCollection = utility.GetAllGroups();
        /// </example>
        public ADGroupCollection GetAllOrganizationalUnit()
        {
            ADGroupCollection gc = new ADGroupCollection();
            System.Collections.Generic.List<DirectoryEntry> list = this.GetAllDEObject(ADLDAPFilterTemplate.GetOUQueryByName("*"));
            for (int i = 0; i < list.Count; i++)
            {
                gc.Add(new ADGroup(list[i]));
            }
            return gc;
        }

        /**/
        /// <summary>
        /// 得到查询得到的一个结果集
        /// </summary>
        /// <param name="ldapQueryString">查询字符串</param>
        /// <returns>IList接口实现</returns>
        /// <example>
        /// ADUtility utility = new ADUtility(.);
        /// System.Collections.IList list = utility.GetAllDEObject("((objectclass=user)(cn=*))");
        /// </example>
        public System.Collections.Generic.List<DirectoryEntry> GetAllDEObject(string ldapQueryString)
        {
            System.Collections.Generic.List<DirectoryEntry> list = new System.Collections.Generic.List<DirectoryEntry>();

            DirectorySearcher searcher = new DirectorySearcher();
            searcher.SearchRoot = this.m_directoryEntry;
            searcher.Filter = ldapQueryString;
            searcher.SearchScope = SearchScope.Subtree;
            // 只找一个对象
            SearchResultCollection sc = searcher.FindAll();
            for (int index = 0; index < sc.Count; index++)
            {
                list.Add(sc[index].GetDirectoryEntry());
            }
            return list;
        }


        /**/
        /// <summary>
        /// 获得所有的用户
        /// </summary>
        /// <returns>ADUserCollection对象</returns>
        public ADUserCollection GetAllUsers()
        {
            ADUserCollection users = new ADUserCollection();
            //System.Collections.Generic.List<DirectoryEntry> list = this.GetAllDEObject(ADLDAPFilterTemplate.GetGroupQueryByName("*"));
            System.Collections.Generic.List<DirectoryEntry> list = this.GetAllDEObject(ADLDAPFilterTemplate.GetUserByCN("*"));
            for (int i = 0; i < list.Count; i++)
            {
                users.Add(new ADUser(list[i]));
            }
            return users;
        }


        public void Dispose()
        {
            this.m_directoryEntry = null;
        }



        /**/
        /// <summary>
        /// 查询得到要找的用户
        /// </summary>
        /// <param name="queryString">查询内容</param>
        /// <returns>ADUserCollection集合</returns>
        public ADUserCollection GetAllUsers(string queryString)
        {
            ADUserCollection users = new ADUserCollection();
            //System.Collections.IList list = this.GetAllDEObject(ADLDAPFilterTemplate.GetQueryString(ADObjectType.User, queryString));
            System.Collections.IList list = this.GetAllDEObject(queryString);
            for (int i = 0; i < list.Count; i++)
            {
                users.Add(new ADUser((DirectoryEntry)list[i]));
            }
            return users;
        }

        /**/
        /// <summary>
        /// 查询得到要找的用户
        /// </summary>
        /// <param name="queryString">查询内容</param>
        /// <returns>ADUserCollection集合</returns>
        public ADUserCollection GetUsersByName(string queryString)
        {
            ADUserCollection users = new ADUserCollection();
            System.Collections.IList list = this.GetAllDEObject(ADLDAPFilterTemplate.GetUserByCN(queryString));
            for (int i = 0; i < list.Count; i++)
            {
                users.Add(new ADUser((DirectoryEntry)list[i]));
            }
            return users;
        }

        #endregion

        #region
        /**/
        /// <summary>
        /// 通过LDAP查询得到一个DirectoryEntry对象
        /// </summary>
        /// <param name="ldapPath">LDAP路径(如：LDAP://nmc.ln.cmcc/DC=nmc,DC=ln,DC=cmcc)，你写成DC=nmc,DC=ln,DC=cmcc</param>
        /// <returns>DirectoryEntry对象</returns>
        public DirectoryEntry GetDEByLDAPPath(string ldapPath)
        {
            try
            {
                this.m_directoryEntry.Path = ADServerLDAP + ldapPath;
                return m_directoryEntry;
                //return   new DirectoryEntry(    ADServerLDAP + ldapPath,this.m_directoryEntry.Username,this.m_directoryEntry.Password,AuthenticationTypes.Secure);
            }
            catch (Exception exc)
            {
                throw new ADException(ldapPath + "查询无结果", exc);
            }
        }

        string m_ServerLDAP;
        /**/
        /// <summary>
        /// AD服务器的LDAP路径(LDAP://xxxxxx/)
        /// </summary>
        public string ADServerLDAP
        {
            get
            {
                // version ,修改这里的bug ,add m_ServerLDAP == ""
                if (m_ServerLDAP == null || m_ServerLDAP == "")
                {
                    this.m_ServerLDAP = this.m_directoryEntry.Path.Substring(0, this.m_directoryEntry.Path.IndexOf("/", 7) + 1);
                }
                return this.m_ServerLDAP;
            }
        }
        #endregion

        /// <summary>
        /// 获取组下面的用户
        /// </summary>
        /// <param name="Groupname"></param>
        /// <returns></returns>
        public string[] GetUsersForGroup(string Groupname)
        {

            DirectorySearcher ds = new DirectorySearcher(m_directoryEntry);
            ds.Filter = "(&(objectClass=group)(cn=" + Groupname + "))";
            ds.PropertiesToLoad.Add("member");
            SearchResult r = ds.FindOne();

            if (r == null || r.Properties["member"] == null)
            {
                return (null);
            }

            string[] results = new string[r.Properties["member"].Count];
            for (int i = 0; i < r.Properties["member"].Count; i++)
            {
                string theGroupPath = r.Properties["member"][i].ToString();
                //results = theGroupPath.Substring(3, theGroupPath.IndexOf(",") - 3);
                //string email = r.Properties["mail"][i].ToString();

                string tempresults = theGroupPath.Substring(3, theGroupPath.IndexOf(",") - 3);
                tempresults = tempresults.Replace("\\", "");
                tempresults = tempresults.Replace("（", "(");
                tempresults = tempresults.Replace("）", ")");

                ADUserCollection collection = this.GetUsersByName(tempresults + "*");
                if (collection != null && collection.Count > 0)
                {
                    ADUser user = collection[0];
                    tempresults = tempresults + "(" + user.EMail + ")";
                    tempresults = tempresults + "(" + user.GivenName + ")";
                }

                results[i] = tempresults;
            }
            return results;
        }
        /// <summary>
        /// 获取组下面的用户
        /// </summary>
        /// <param name="Groupname"></param>
        /// <returns></returns>
        public List<ADUser> GetUserCollectionForGroup(string Groupname)
        {
            List<ADUser> listUsers = new List<ADUser>();
            DirectorySearcher ds = new DirectorySearcher(m_directoryEntry);
            ds.Filter = "(&(objectClass=group)(cn=" + Groupname + "))";
            ds.PropertiesToLoad.Add("member");
            SearchResult r = ds.FindOne();

            if (r == null || r.Properties["member"] == null)
            {
                return (null);
            }

            for (int i = 0; i < r.Properties["member"].Count; i++)
            {
                string theGroupPath = r.Properties["member"][i].ToString();
                //results = theGroupPath.Substring(3, theGroupPath.IndexOf(",") - 3);
                //string email = r.Properties["mail"][i].ToString();

                string tempresults = theGroupPath.Substring(3, theGroupPath.IndexOf(",") - 3);
                tempresults = tempresults.Replace("\\", "");
                tempresults = tempresults.Replace("（", "(");
                tempresults = tempresults.Replace("）", ")");

                ADUserCollection collection = this.GetUsersByName(tempresults + "*");
                if (collection != null && collection.Count > 0)
                {
                    ADUser user = collection[0];
                    listUsers.Add(user);
                }
            }
            return listUsers;
        }

        /// <summary>
        /// 获取ou下面的用户
        /// </summary>
        /// <param name="Groupname"></param>
        /// <returns></returns>
        public string[] GetUsersForOU(string Groupname)
        {

            DirectorySearcher ds = new DirectorySearcher(m_directoryEntry);
            ds.Filter = "(&(objectClass=organizationalUnit)(ou=" + Groupname + "))";
            ds.PropertiesToLoad.Add("member");
            SearchResult r = ds.FindOne();

            if (r == null || r.Properties["member"] == null)
            {
                return (null);
            }

            string[] results = new string[r.Properties["member"].Count];
            for (int i = 0; i < r.Properties["member"].Count; i++)
            {
                string theGroupPath = r.Properties["member"][i].ToString();
                //results = theGroupPath.Substring(3, theGroupPath.IndexOf(",") - 3);
                //string email = r.Properties["mail"][i].ToString();

                string tempresults = theGroupPath.Substring(3, theGroupPath.IndexOf(",") - 3);
                tempresults = tempresults.Replace("\\", "");
                tempresults = tempresults.Replace("（", "(");
                tempresults = tempresults.Replace("）", ")");

                ADUserCollection collection = this.GetUsersByName(tempresults + "*");
                if (collection != null && collection.Count > 0)
                {
                    ADUser user = collection[0];
                    tempresults = tempresults + "(" + user.EMail + ")";
                    tempresults = tempresults + "(" + user.GivenName + ")";
                }

                results[i] = tempresults;
            }
            return results;
        }
        /// <summary>
        /// 获取ou下面的用户
        /// </summary>
        /// <param name="Groupname"></param>
        /// <returns></returns>
        public List<ADUser> GetUserCollectionForOU(string Groupname)
        {
            List<ADUser> listUsers = new List<ADUser>();
            DirectorySearcher ds = new DirectorySearcher(m_directoryEntry);
            ds.Filter = "(&(objectClass=organization)(ou=" + Groupname + "))";
            ds.PropertiesToLoad.Add("member");
            SearchResult r = ds.FindOne();

            if (r == null || r.Properties["member"] == null)
            {
                return (null);
            }

            for (int i = 0; i < r.Properties["member"].Count; i++)
            {
                string theGroupPath = r.Properties["member"][i].ToString();
                //results = theGroupPath.Substring(3, theGroupPath.IndexOf(",") - 3);
                //string email = r.Properties["mail"][i].ToString();

                string tempresults = theGroupPath.Substring(3, theGroupPath.IndexOf(",") - 3);
                tempresults = tempresults.Replace("\\", "");
                tempresults = tempresults.Replace("（", "(");
                tempresults = tempresults.Replace("）", ")");

                ADUserCollection collection = this.GetUsersByName(tempresults + "*");
                if (collection != null && collection.Count > 0)
                {
                    ADUser user = collection[0];
                    listUsers.Add(user);
                }
            }
            return listUsers;
        }





        #region## 域中是否存在组织单位
        /// <summary> 
        /// 功能：域中是否存在组织单位 
        /// 作者：Wilson 
        /// 时间：2012-12-15
        /// </summary> 
        /// <param name="entry"></param> 
        /// <param name="ou"></param> 
        /// <returns></returns> 
        private bool IsExistOU(DirectoryEntry entry, out DirectoryEntry ou)
        {
            ou = new DirectoryEntry();
            try
            {
                ou = entry.Children.Find("OU=APPS");
                return (ou != null);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        #region## 类型
        /// <summary> 
        /// 类型 
        /// </summary> 
        public enum TypeEnum : int
        {
            /// <summary> 
            /// 组织单位 
            /// </summary> 
            OU = 1,
            /// <summary> 
            /// 用户 
            /// </summary> 
            USER = 2
        }
        #endregion
        #region## Ad域信息实体
        /// <summary> 
        /// Ad域信息实体 
        /// </summary> 
        public class AdModel
        {
            public AdModel(string id, string name, int typeId, string parentId)
            { Id = id; Name = name; TypeId = typeId; ParentId = parentId; }
            public string Id { get; set; }
            public string Name { get; set; }
            public int TypeId { get; set; }
            public string ParentId { get; set; }
        }
        #endregion

        private List<ADUser> list = new List<ADUser>();

        #region## 同步
        /// <summary> 
        /// 功能:同步 
        /// 创建人:Wilson 
        /// 创建时间:2012-12-15 
        /// </summary> 
        /// <param name="entryOU"></param> 
        public void SyncAll(DirectoryEntry entryOU)
        {
            DirectorySearcher mySearcher = new DirectorySearcher(entryOU, "(objectclass=organizationalUnit)");
            //查询组织单位 
            DirectoryEntry root = mySearcher.SearchRoot;
            //查找根OU 
            SyncRootOU(root);
        }

        public List<ADUser> SyncSubOU(string ParentName)
        {
            return SyncSubOU(m_directoryEntry, ParentName);
        }

        public List<ADUser> SyncRootOU()
        {
            return SyncRootOU(m_directoryEntry);
        }
        /// <summary> 
        /// 功能: 同步根组织单位 
        /// 创建人:Wilson 
        /// 创建时间:2012-12-15 
        /// </summary> 
        /// <param name="entry"></param> 
        private List<ADUser> SyncRootOU(DirectoryEntry entry)
        {
            if (entry.Properties.Contains("ou") && entry.Properties.Contains("objectGUID"))
            {
                string rootOuName = entry.Properties["ou"][0].ToString();
                byte[] bGUID = entry.Properties["objectGUID"][0] as byte[];
                string id = BitConverter.ToString(bGUID);
                //list.Add(new ADUser(id, rootOuName, (int)TypeEnum.OU, "0"));
                SyncSubOU(entry, id);
            }
            return list;
        }
        #endregion #region## 同步下属组织单位及下属用户

        /// <summary> 
        /// 功能: 同步下属组织单位及下属用户 
        /// 创建人:Wilson 
        /// 创建时间:2012-12-15 
        /// </summary> 
        /// <param name="entry"></param> 
        /// <param name="parentId"></param> 
        private List<ADUser> SyncSubOU(DirectoryEntry entry, string parentId)
        {
            foreach (DirectoryEntry subEntry in entry.Children)
            {
                string entrySchemaClsName = subEntry.SchemaClassName;
                string[] arr = subEntry.Name.Split('=');
                string categoryStr = arr[0];
                string nameStr = arr[1];
                string id = string.Empty;
                if (subEntry.Properties.Contains("objectGUID")) //SID 
                {
                    byte[] bGUID = subEntry.Properties["objectGUID"][0] as byte[];
                    id = BitConverter.ToString(bGUID);
                }
                bool isExist = list.Exists(d => d.Guid.ToString() == id);
                switch (entrySchemaClsName)
                {
                    case "organizationalUnit":
                        if (!isExist)
                        {
                            //list.Add(new AdModel(id, nameStr, (int)TypeEnum.OU, parentId));
                        }
                        SyncSubOU(subEntry, nameStr);
                        break;
                    case "user":
                        string accountName = string.Empty;
                        if (subEntry.Properties.Contains("samaccountName"))
                        { accountName = subEntry.Properties["samaccountName"][0].ToString(); }
                        if (!isExist)
                        {
                            ADUser aduser = new ADUser(subEntry);
                            list.Add(aduser);
                        }
                        break;
                }
            }
            return list;
        }
    }
}
