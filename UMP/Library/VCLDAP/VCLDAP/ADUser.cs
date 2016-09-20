using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;

namespace VoiceCyber.VCLDAP
{
    public class ADUser : ADObject
    {
        /**/
        /// <summary>
        /// 包含活动目录中的用户信息的访问、修改。
        /// </summary>
        public ADUser()
        {
        }
        /**/
        /// <summary>
        /// 通过DirectoryEntry对象初始化ADUser
        /// </summary>
        /// <param name="directoryEntry">指定用户的DirectoryEntry对象</param>
        public ADUser(DirectoryEntry directoryEntry)
            : base(directoryEntry)
        {
            if (directoryEntry == null)
            {
                throw new ADException("传入的DirectoryEntry对象为空。");
            }
            else
            {
                this.MyDirectoryEntry = directoryEntry;
            }
        }

        /**/
        /// <summary>
        /// 通过用户登录账号，初始化ADUser类
        /// </summary>
        /// <param name="loginName">登录名(Server\UserName , or UserName)</param>
        public ADUser(string loginName)
        {
            if (ADObject.Utility != null)
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
                this.MyDirectoryEntry = (ADObject.Utility.GetOnlyOneDEObject(ldapQueryString));
                if (this.MyDirectoryEntry == null)
                    throw new ADException("无法通过登录名\"" + loginName + "\"得到ADUser实体");
            }
            else
            {
                throw new ADException("必须先给XZSoftware.ActiveDirectoryLib类的静态变量Utility赋值");
            }
        }

        public ADUser(char[] cn)
        {

            if (ADObject.Utility != null)
            {
                string userName = new string(cn);
                string ldapQueryString = ADLDAPFilterTemplate.GetFilterByCNName(userName);
                this.MyDirectoryEntry = (ADObject.Utility.GetOnlyOneDEObject(ldapQueryString));
                if (this.MyDirectoryEntry == null)
                    throw new ADException("无法通过CN=\"" + userName + "\"得到ADUser实体");
            }
            else
            {
                throw new ADException("必须先给XZSoftware.ActiveDirectoryLib类的静态变量Utility赋值");
            }
        }


        //public ADUser( string mail )
        //{
        //    if (ADObject.Utility != null)
        //    {
        //        string ldapQueryString = ADLDAPFilterTemplate.GetFilterByEmail( mail ) ;
        //        this.MyDirectoryEntry = (ADObject.Utility.GetOnlyOneDEObject( ldapQueryString) ) ;
        //        if ( this.MyDirectoryEntry == null)
        //          throw new ADException("无法通过Email=\""+mail.ToString()+"\"得到ADUser实体。");
        //   }
        //    else 
        //    {
        //        throw new ADException("必须先给XZSoftware.ActiveDirectoryLib类的静态变量Utility赋值");
        //    }
        //}

        /**/
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mobileNumber">手机号</param>
        public ADUser(ulong mobileNumber)
        {
            if (ADObject.Utility != null)
            {
                string ldapQueryString = ADLDAPFilterTemplate.GetADUserFilterByMobile(mobileNumber.ToString());
                this.MyDirectoryEntry = (ADObject.Utility.GetOnlyOneDEObject(ldapQueryString));
                if (this.MyDirectoryEntry == null)
                    throw new ADException("无法通过手机号\"" + mobileNumber.ToString() + "\"得到ADUser实体");
            }
            else
            {
                throw new ADException("必须先给XZSoftware.ActiveDirectoryLib类的静态变量Utility赋值");
            }
        }

        /**/
        /// <summary>
        /// 中文显示名
        /// </summary>
        public string DisplayName
        {
            get
            {
                return GetProperty("displayName").ToString();
            }
            set
            {
                SetProperty("displayName", value);
            }
        }


        /**/
        /// <summary>
        /// 描述(生日)
        /// </summary>
        public string Description
        {
            get
            {
                return GetProperty("Description").ToString();
            }
            set
            {
                SetProperty("Description", value);
            }
        }

        /**/
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile
        {
            get
            {
                return GetProperty("Mobile").ToString();
            }
            set
            {
                SetProperty("Mobile", value);
            }
        }

        /**/
        /// <summary>
        /// 性别(这是一个新添字段，在别的地方用不了)
        /// </summary>
        public bool Sex
        {
            get
            {
                return Convert.ToBoolean(GetProperty("sex"));
            }
            set
            {
                SetProperty("sex", value);
            }
        }

        /**/
        /// <summary>
        /// 获得CN值(只读)
        /// </summary>
        public string CN
        {
            get
            {
                return GetProperty("cn").ToString();
            }
        }

        /**/
        /// <summary>
        /// 传呼机号
        /// </summary>
        public string Pager
        {
            get
            {
                return GetProperty("pager").ToString();
            }
            set
            {
                SetProperty("pager", value);
            }
        }

        /**/
        /// <summary>
        /// 所在的组
        /// </summary>
        public ADGroupCollection MemberOf
        {
            get
            {
                ADGroupCollection adGroupCollection = new ADGroupCollection();
                System.Collections.IList groupList = this.GetProperties("MemberOf");

                if (groupList != null)
                {
                    foreach (object adGroupPath in groupList)
                    {
                        string groupPath = "";
                        string groupName = "";

                        // 如：CN=全省传输人员组,OU=专业人员,OU=应用系统,DC=nmc
                        groupPath = adGroupPath.ToString();

                        // 要得到"全省传输人员组"
                        int groupNameLength = groupPath.IndexOf(",", 0) - 3; // Get the group's name length

                        groupName = groupPath.Substring(3, groupNameLength);


                        // Create a group object
                        ADGroup group = new ADGroup(groupName);
                        adGroupCollection.Add(group);
                    }
                }

                return adGroupCollection;
            }
        }

        #region version
        /**/
        /// <summary>
        /// 邮箱
        /// </summary>
        public string EMail
        {
            get { return GetProperty("mail").ToString(); }
            set { this.SetProperty("mail", value); }
        }

        /**/
        /// <summary>
        /// 国家
        /// </summary>
        public string Country
        {
            get { return GetProperty("c").ToString(); }
        }

        #endregion


        #region
        public string Title
        {
            get
            {
                return GetProperty("Title").ToString();
            }
            set
            {
                SetProperty("Title", value);
            }
        }
        #endregion


        #region

        /**/
        /// <summary>
        /// 登录名(登录全名SAMAccountName)
        /// </summary>
        public string AccountFullName
        {
            get
            {
                return GetProperty("SAMAccountName").ToString();
            }
        }

        /**/
       /// <summary>
        /// 登录名userPrincipalName
       /// </summary>
        public string AccountName
        {
            get
            {
                return GetProperty("userPrincipalName").ToString();
            }
        }

        /**/
        /// <summary>
        /// 注释
        /// </summary>
        public string Info
        {
            get
            {
                return GetProperty("Info").ToString();
            }
            set
            {
                SetProperty("Info", value);
            }
        }

        #endregion


        #region
        /**/
        /// <summary>
        /// Specifies flags that control password, lockout, disable/enable, script, and home directory behavior for the user. This property also contains a flag that indicates the account type of the object. The flags are defined in UserAccountControlType<br>
        /// 0;启用账号，为禁用
        /// </summary>
        public int UserAccountControl
        {
            get
            {
                return (int)this.GetProperty("userAccountControl");
            }
            set
            {
                this.SetProperty("userAccountControl", value);
            }
        }

        /**/
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PostalCode
        {
            get
            {
                return this.GetProperty("postalCode").ToString();
            }
            set
            {
                this.SetProperty("postalCode", value);
            }
        }
        #endregion



        #region

        /**/
        /// <summary>
        /// 别名（昵称）
        /// </summary>
        public string MailNickName
        {
            get
            {
                return this.GetProperty("mailNickname").ToString();
            }
            set
            {
                this.SetProperty("mailNickname", value);
            }
        }

        /**/
        /// <summary>
        /// 生日
        /// </summary>
        public string Birthday
        {
            get
            {
                return this.GetProperty("Birthday").ToString();
            }
            set
            {
                this.SetProperty("Birthday", value);
            }
        }


        /**/
        /// <summary>
        /// 学历
        /// </summary>
        public string Degree
        {
            get
            {
                return this.GetProperty("Degree").ToString();
            }
            set
            {
                this.SetProperty("Degree", value);
            }
        }


        /**/
        /// <summary>
        /// IP电话
        /// </summary>
        public string IPPhone
        {
            get
            {
                return this.GetProperty("ipPhone").ToString();
            }
            set
            {
                this.SetProperty("ipPhone", value);
            }
        }


        /**/
        /// <summary>
        /// 部门
        /// </summary>
        public string Department
        {
            get
            {
                return this.GetProperty("department").ToString();
            }
            set
            {
                this.SetProperty("department", value);
            }
        }

        /**/
        /// <summary>
        /// 公司
        /// </summary>
        public string Company
        {
            get
            {
                return this.GetProperty("company").ToString();
            }
            set
            {
                this.SetProperty("company", value);
            }
        }


        /**/
        /// <summary>
        /// 管理者（领导）
        /// </summary>
        public ADUser Manager
        {
            get
            {
                System.DirectoryServices.DirectoryEntry entry;
                ADUser user;
                string managerPath;
                try
                {
                    managerPath = this.GetProperty("Manager").ToString();
                    entry = ADObject.Utility.GetDEByLDAPPath(managerPath);
                    user = new ADUser(entry);
                }
                catch
                {
                    user = null;
                }
                return user;
            }
        }


        /**/
        /// <summary>
        /// 街道
        /// </summary>
        public string Street
        {
            get
            {
                return this.GetProperty("Street").ToString();
            }
            set
            {
                this.SetProperty("Street", value);
            }
        }

        /**/
        /// <summary>
        /// 地址
        /// </summary>
        public string StreetAddress
        {
            get
            {
                return this.GetProperty("StreetAddress").ToString();
            }
            set
            {
                this.SetProperty("StreetAddress", value);
            }
        }


        /**/
        /// <summary>
        /// 住宅电话
        /// </summary>
        public string HomePhone
        {
            get
            {
                return this.GetProperty("HomePhone").ToString();
            }
            set
            {
                this.SetProperty("HomePhone", value);
            }
        }


        /**/
        /// <summary>
        /// 主页
        /// </summary>
        public string WWWHomePage
        {
            get
            {
                return this.GetProperty("wWWHomePage").ToString();
            }
            set
            {
                this.SetProperty("wWWHomePage", value);
            }
        }


        /**/
        /// <summary>
        /// Fax
        /// </summary>
        public string FacsimileTelephoneNumber
        {
            get
            {
                return this.GetProperty("FacsimileTelephoneNumber").ToString();
            }
            set
            {
                this.SetProperty("FacsimileTelephoneNumber", value);
            }
        }

        /**/
        /// <summary>
        /// 国家
        /// </summary>
        public string C
        {
            get
            {
                return this.GetProperty("C").ToString();
            }
            set
            {
                this.SetProperty("C", value);
            }
        }

        /**/
        /// <summary>
        /// 国家代码
        /// </summary>
        public int CountryCode
        {
            get
            {
                return Convert.ToInt32(this.GetProperty("CountryCode"));
            }
            set
            {
                this.SetProperty("CountryCode", value);
            }
        }


        /**/
        /// <summary>
        /// 名
        /// </summary>
        public string GivenName
        {
            get
            {
                return this.GetProperty("givenName").ToString();
            }
            set
            {
                this.SetProperty("givenName", value);
            }
        }


        public int LogonCount
        {
            get
            {
                return Convert.ToInt32(this.GetProperty("LogonCount"));
            }
            set
            {
                this.SetProperty("LogonCount", value);
            }
        }


        public bool Married
        {
            get
            {
                return Convert.ToBoolean(this.GetProperty("Married"));
            }
            set
            {
                this.SetProperty("Married", value);
            }
        }


        /**/
        /// <summary>
        /// 办公电话
        /// </summary>
        public string TelephoneNumber
        {
            get
            {
                return this.GetProperty("TelephoneNumber").ToString();
            }
            set
            {
                this.SetProperty("TelephoneNumber", value);
            }
        }

        /**/
        /// <summary>
        /// 办公地址
        /// </summary>
        public string PhysicalDeliveryOfficeName
        {
            get
            {
                return this.GetProperty("PhysicalDeliveryOfficeName").ToString();
            }
            set
            {
                this.SetProperty("PhysicalDeliveryOfficeName", value);
            }
        }


        /**/
        /// <summary>
        /// 下属
        /// </summary>
        public ADUserCollection DirectReports
        {
            get
            {
                string userPath;
                ADUser userObj;
                ADUserCollection uc;
                System.DirectoryServices.DirectoryEntry entryObj;
                System.Collections.IList list;

                uc = new ADUserCollection();
                try
                {
                    list = this.GetProperties("DirectReports");
                    int count = list.Count;
                    for (int i = 0; i < count; i++)
                    {
                        userPath = Convert.ToString(list[i]);
                        entryObj = ADObject.Utility.GetDEByLDAPPath(userPath);
                        userObj = new ADUser(entryObj);
                        uc.Add(userObj);
                    }
                }
                catch
                {
                }
                return uc;

            }
        }
        #endregion


        #region

        /**/
        /// <summary>
        /// 用户AD中的城市信息
        /// </summary>
        public string City
        {
            get
            {
                return this.GetProperty("l").ToString();
            }
        }

        #endregion



        //  #region aaa
        //  private ADOrganizeUnit _ou;
        //  public ADOrganizeUnit OU
        //  {
        //      get
        //      {
        //          if ( _ou == null )
        //          {
        //              string adsPath = this.AdsPath.ToUpper().Replace("CN="+this.CN.ToUpper()+",","");
        //              ADOrganizeUnit ou = new ADOrganizeUnit( adsPath );
        //              _ou = ou ;
        //          }
        //          return _ou ;
        //      }
        //}
        // #endregion
    }
}
