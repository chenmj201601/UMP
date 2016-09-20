using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.VCLDAP
{
    /**/
    /*
* ##############################
* 
* ModifyTime : 0--0
* Version : 1
* ##############################
* 
* 
* Description
* 
* version 0
* 按登录名查用户
* 
* Version 1
* 增加按手机号查用户,ADObjectType枚举,查询组
* 
* 
* version 
* 填加常量LDAP_SEARCH_OU_BY_LDAP
* 
* 
*/

    /**/
    /// <summary>
    /// LDAP Search Filters String Creator
    /// </summary>
    public sealed class ADLDAPFilterTemplate
    {

        /**/
        /// <summary>
        /// 通用变量表示字符
        /// </summary>
        public const string LDAP_VALUE = "[value]";

        /**/
        /// <summary>
        /// 登录名找用户的Filter
        /// </summary>
        public const string LDAP_SEARCH_USER_BY_LOGINNAME = "(&(objectclass=user)(!objectclass=computer)(sAMAccountName=[value]))";
        public const string LDAP_SEARCH_USER_BY_CN = "(&(objectclass=user)(!objectclass=computer)(cn=[value]))";
        public const string LDAP_SEARCH_USER_BY_EMAIL = "(&(objectclass=user)(!objectclass=computer)(mail=[value]))";

        #region
        /**/
        /// <summary>
        /// 通过LDAPPath查OU
        /// </summary>
        public const string LDAP_SEARCH_OU_BY_LDAP = "(&(objectclass=organizationalUnit)(adsPath=[value]))";
        #endregion


        public static string GetFilterByLoginName(string loginName)
        {
            return LDAP_SEARCH_USER_BY_LOGINNAME.Replace(
                LDAP_VALUE,
                loginName);
        }

        /**/
        /// <summary>
        /// 通过cn查找
        /// </summary>
        /// <param name="cnName"></param>
        /// <returns></returns>
        public static string GetFilterByCNName(string cnName)
        {
            return LDAP_SEARCH_USER_BY_CN.Replace(
                LDAP_VALUE,
              cnName);
        }

        public static string GetFilterByEmail(string email)
        {
            return LDAP_SEARCH_USER_BY_EMAIL.Replace(LDAP_VALUE, email);
        }



        #region version 1

        public const string LDAP_SEARCH_ALL_USERS = "(&(objectclass=user)(!objectclass=computer)(cn=*)(!email=''))";
        //public const string LDAP_SEARCH_GROUP_BY_NAME = "((&(objectclass=group)(cn=[value])))";
        public const string LDAP_SEARCH_GROUP_BY_NAME = "(&(objectClass=group)(cn=[value]))";
        public const string LDAP_SEARCH_USER_BY_MOBILE = "(&(objectclass=user)(!objectclass=computer)(mobile=[value]))";
        public const string LDAP_SEARCH_USER_BY_NAME = "(&(objectclass=user)(!objectclass=computer)(cn=[value]))";

        public const string LDAP_SEARCH_OU_BY_NAME = "(&(objectClass=organizationalUnit)(!objectclass=computer))";
        /**/
        /// <summary>
        /// 通配找用户的Filter
        /// </summary>
        const string strSAMAccountName = "(SAMAccountName=*[value]*)";
        const string strObjectClass = "(objectclass=[value])";
        const string strCN = "(CN=*[value]*)";
        const string strCity = "(l=*[value]*)";
        const string strCompany = "(company=*[value]*)";
        const string strProvince = "(st=*[value]*)";
        const string strHandset = "(telephonenumber=*[value]*)";
        const string strName = "(name=*[value]*)";
        const string strdisplayname = "(displayname=*[value]*)";
        const string strTitle = "(title=*[value]*)";
        const string strDepartment = "(department=*[value]*)";
        const string strCo = "(co=*[value]*)";
        // const string strUserprincipalname = "(userprincipalname=*[value]*)" ;
        const string strMail = "(mail=*[value]*)";
        const string strMobile = "(mobile=*[value]*)";

        /**/
        /// <summary>
        /// 通过手机号查AD用户
        /// </summary>
        public static string GetADUserFilterByMobile(string mobile)
        {
            return LDAP_SEARCH_USER_BY_MOBILE.Replace(
                 LDAP_VALUE,
                 mobile);
        }


        /**/
        /// <summary>
        /// 查询得到有关查询条件的字符串
        /// </summary>
        /// <param name="type">查询类型。</param>
        /// <param name="searchInfo">查询条件，'，',',',' '分隔。</param>
        /// <returns>查询字符串</returns>
        public static string GetQueryString(ADObjectType type, string searchInfo)
        {
            string strCondition;
            string[] strArrCondition;
            string strHeader = "(|";
            string strFooter = ")";
            string queryString = "";
            string strClass;
            switch (type)
            {
                case ADObjectType.User:
                    strClass = "User";
                    break;
                case ADObjectType.Computer:
                    strClass = "Computer";
                    break;
                case ADObjectType.Group:
                    strClass = "Group";
                    break;
                case ADObjectType.OrganizeUnit:
                    strClass = "OU";
                    break;
                default:
                    strClass = "user";
                    break;
            }
            // 如果选择了查询范围
            if ("" != strClass)
            {
                strHeader = "(&" + strObjectClass.Replace(LDAP_VALUE, strClass) + strHeader;
                strFooter += ")";
            }
            strCondition = searchInfo;
            strArrCondition = strCondition.Split(new char[] { ' ', ',', '，' });
            for (int i = 0; i < strArrCondition.Length; i++)
            {
                strArrCondition[i] = strArrCondition[i].Trim();
                if (strArrCondition[i] != "")
                {
                    queryString += strCN.Replace(LDAP_VALUE, strArrCondition[i]);
                    queryString += strName.Replace(LDAP_VALUE, strArrCondition[i]);
                    queryString += strCity.Replace(LDAP_VALUE, strArrCondition[i]);
                    queryString += strdisplayname.Replace(LDAP_VALUE, strArrCondition[i]);
                    queryString += strProvince.Replace(LDAP_VALUE, strArrCondition[i]);
                    queryString += strMail.Replace(LDAP_VALUE, strArrCondition[i]);
                    if (ADObjectType.User == type)
                    {
                        queryString += strSAMAccountName.Replace(LDAP_VALUE, strArrCondition[i]);
                        // queryString += strCompany.Replace(LDAP_VALUE , strArrCondition[i] ) ;
                        queryString += strHandset.Replace(LDAP_VALUE, strArrCondition[i]);
                        // queryString += strTitle.Replace(LDAP_VALUE , strArrCondition[i] ) ;
                        queryString += strDepartment.Replace(LDAP_VALUE, strArrCondition[i]);
                        // queryString += strCo.Replace(LDAP_VALUE , strArrCondition[i] ) ;
                        // 0--xuzhong 
                        // queryString += strUserprincipalname.Replace(LDAP_VALUE , strArrCondition[i] ) ;
                        queryString += strMobile.Replace(LDAP_VALUE, strArrCondition[i]);
                    }
                }
            }
            queryString = strHeader + queryString + strFooter;
            return queryString;
        }

        /**/
        /// <summary>
        /// 得到查询组
        /// </summary>
        public static string GetGroupQueryByName(string groupName)
        {
            string _groupName = LDAP_SEARCH_GROUP_BY_NAME.Replace(LDAP_VALUE, groupName);
            return _groupName;
        }

        /**/
        /// <summary>
        /// 得到查ou
        /// </summary>
        public static string GetOUQueryByName(string groupName)
        {
            string _groupName = LDAP_SEARCH_OU_BY_NAME.Replace(LDAP_VALUE, groupName);
            return _groupName;
        }

        /// <summary>
        /// 得到查询用户
        /// </summary>
        public static string GetUserByCN(string userName)
        {
            string _userName = LDAP_SEARCH_USER_BY_NAME.Replace(LDAP_VALUE, userName);
            return _userName;
        }
        #endregion

    }
}
