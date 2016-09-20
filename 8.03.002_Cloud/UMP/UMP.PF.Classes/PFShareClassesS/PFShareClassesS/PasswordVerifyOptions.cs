using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace PFShareClassesS
{
    public class PasswordVerifyOptions
    {
        /// <summary>
        /// 检查密码是否符合复杂性要求
        /// </summary>
        /// <param name="AStrPassword">密码</param>
        /// <param name="AIntMinLength">最短长度</param>
        /// <param name="AIntMaxLength">最大长度</param>
        /// <param name="AStrNotContainsCharacters">不能包含的字符</param>
        /// <param name="AStrFalseReturn">
        /// E001：密码太短
        /// E002：密码太长
        /// E003：密码中包含了不允许包含的字符
        /// E004：不满足四类字符中的三类字符
        /// E999：检查过程中发生错误
        /// </param>
        /// <returns>True：符合；False：不符合</returns>
        public static bool MeetComplexityRequirements(string AStrPassword, int AIntMinLength, int AIntMaxLength, string AStrNotContainsCharacters, ref string AStrFalseReturn)
        {
            bool LBoolReturn = false;
            string LStrPassword = string.Empty;
            int LIntMinLen = 0, LIntMaxLen = 0;
            string LStrNotContainsChar = string.Empty;
            List<bool> LListBoolIsMatch = new List<bool>();
            int LIntIsMatchedCount = 0;

            try
            {
                AStrFalseReturn = string.Empty;
                LStrPassword = AStrPassword;
                LIntMinLen = AIntMinLength;
                LIntMaxLen = AIntMaxLength;
                if (LIntMaxLen == 0) { LIntMaxLen = 64; }
                LStrNotContainsChar = AStrNotContainsCharacters;

                if (LStrPassword.Length < LIntMinLen) { AStrFalseReturn = "E001"; return LBoolReturn; }
                if (LStrPassword.Length > LIntMaxLen) { AStrFalseReturn = "E002"; return LBoolReturn; }
                if (!string.IsNullOrEmpty(LStrNotContainsChar))
                {
                    if (LStrPassword.ToLower().Contains(LStrNotContainsChar.ToLower())) { AStrFalseReturn = "E003"; return LBoolReturn; }
                }

                //必须包含数字
                Regex LRegex1 = new Regex("(?=.*[0-9])", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

                //必须包含小写字母
                Regex LRegex2 = new Regex("(?=.*[a-z])", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

                //必须包含大写字母
                Regex LRegex3 = new Regex("(?=.*[A-Z])", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

                //必须包含特殊符号
                Regex LRegex4 = new Regex("(?=([\x21-\x7e]+)[^a-zA-Z0-9])", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

                LListBoolIsMatch.Add(LRegex1.IsMatch(LStrPassword));
                LListBoolIsMatch.Add(LRegex2.IsMatch(LStrPassword));
                LListBoolIsMatch.Add(LRegex3.IsMatch(LStrPassword));
                LListBoolIsMatch.Add(LRegex4.IsMatch(LStrPassword));

                foreach (bool LBoolSingleMatched in LListBoolIsMatch)
                {
                    if (LBoolSingleMatched) { LIntIsMatchedCount += 1; }
                }
                if (LIntIsMatchedCount < 3) { AStrFalseReturn = "E004"; return LBoolReturn; }

                LBoolReturn = true;
            }
            catch { LBoolReturn = false; AStrFalseReturn = "E999"; }

            return LBoolReturn;
        }

        /// <summary>
        /// 生成指定长度的随机密码
        /// </summary>
        /// <param name="AIntLenght">生成的密码的字符数。长度必须介于 1 和 128 个字符之间</param>
        /// <param name="AIntNonAlphanumericCharacters">生成的密码中的标点字符数</param>
        /// <returns></returns>
        public static string GeneratePassword(int AIntLenght, int AIntNonAlphanumericCharacters)
        {
            string LStrReturn = string.Empty;

            try
            {
                LStrReturn = Membership.GeneratePassword(AIntLenght, AIntNonAlphanumericCharacters);
            }
            catch
            {
                LStrReturn = Guid.NewGuid().ToString();
            }

            return LStrReturn;
        }
    }
}
