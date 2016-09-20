//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0b0717f9-f53d-4228-b5dd-b6b6dd6f4b92
//        CLR Version:              4.0.30319.18063
//        Name:                     JsonHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Common
//        File Name:                JsonHelper
//
//        created by Charley at 2014/3/24 13:19:47
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Common
{
    /// <summary>
    /// 用于构建属性值的回调
    /// </summary>
    /// <param name="property"></param>
    public delegate void SetPropertiesDelegate(JsonObject property);

    /// <summary>
    /// JsonObject属性值类型
    /// </summary>
    public enum JsonPropertyType
    {
        /// <summary>
        /// String
        /// </summary>
        String,
        /// <summary>
        /// Object
        /// </summary>
        Object,
        /// <summary>
        /// Array
        /// </summary>
        Array,
        /// <summary>
        /// Number
        /// </summary>
        Number,
        /// <summary>
        /// Bool
        /// </summary>
        Bool,
        /// <summary>
        /// Null
        /// </summary>
        Null
    }

    /// <summary>
    /// JSON通用对象
    /// </summary>
    public class JsonObject
    {
        private Dictionary<String, JsonProperty> mProperty;
        /// <summary>
        /// 创建一个JSON对象
        /// </summary>
        public JsonObject()
        {
            mProperty = null;
        }
        /// <summary>
        /// 通过字符串构造一个JSON对象
        /// </summary>
        /// <param name="jsonString"></param>
        public JsonObject(String jsonString)
        {
            Parse(ref jsonString);
        }
        /// <summary>
        /// 通过回调函数构造一个JSON对象
        /// </summary>
        /// <param name="callback">回调函数</param>
        public JsonObject(SetPropertiesDelegate callback)
        {
            if (callback != null)
            {
                callback(this);
            }
        }
        /// <summary>
        /// Json字符串解析
        /// </summary>
        /// <param name="jsonString">Json字符串</param>
        private void Parse(ref String jsonString)
        {
            int len = jsonString.Length;
            if (String.IsNullOrEmpty(jsonString)
                || jsonString.Substring(0, 1) != "{"
                || jsonString.Substring(len - 1, 1) != "}")
            {
                throw new ArgumentException(string.Format("Input string not a valid Json string! {0}", jsonString));
            }
            Stack<Char> stackKey = new Stack<Char>();           //键名引号匹配
            Stack<Char> stackType = new Stack<Char>();          //括号（方括号或花括号）匹配
            Stack<Char> stackStrValue = new Stack<Char>();      //属性值引号匹配
            StringBuilder sb = new StringBuilder();
            Char cur;
            bool isValue = false;       //字符串属性值的内容原样保留，不做处理
            JsonProperty last = null;
            for (int i = 1; i <= len - 2; i++)
            {
                cur = jsonString[i];
                if (cur == '}')
                {
                }
                if (cur == ' '
                    && stackKey.Count == 0
                    && stackType.Count == 0
                    && stackStrValue.Count == 0)
                {
                    //键名及属性值以外的空格忽略调
                }
                else if ((cur == '\'' || cur == '\"')
                    && stackKey.Count == 0
                    && !isValue)
                {
                    //进入键名
                    sb.Length = 0;
                    stackKey.Push(cur);
                }
                else if ((cur == '\'' || cur == '\"')
                    && stackKey.Count > 0
                    && stackKey.Peek() == cur
                    && !isValue)
                {
                    //结束键名
                    stackKey.Pop();
                }
                else if ((cur == '\'' || cur == '\"')
                    && isValue
                    && stackType.Count == 0
                    && stackStrValue.Count == 0)
                {
                    //进入属性值
                    sb.Append(cur);
                    stackStrValue.Push(cur);
                }
                else if ((cur == '\'' || cur == '\"')
                    && isValue
                    && stackType.Count == 0
                    && stackStrValue.Count > 0
                    && stackStrValue.Peek() == cur)
                {
                    //结束属性值
                    sb.Append(cur);
                    stackStrValue.Pop();
                }
                else if ((cur == '[' || cur == '{')
                    && isValue
                    && stackKey.Count == 0)
                {
                    //进入数组或子对象
                    stackType.Push(cur == '[' ? ']' : '}');
                    sb.Append(cur);
                }
                else if ((cur == ']' || cur == '}')
                    && isValue
                    && stackKey.Count == 0
                    && stackType.Peek() == cur)
                {
                    //结束数组或子对象
                    stackType.Pop();
                    sb.Append(cur);
                }
                else if (cur == ':'
                    && stackKey.Count == 0
                    && stackType.Count == 0
                    && !isValue)
                {
                    //进入属性值
                    last = new JsonProperty();
                    this[sb.ToString()] = last;
                    isValue = true;
                    sb.Length = 0;
                }
                else if (cur == ','
                    && stackKey.Count == 0
                    && stackType.Count == 0
                    && stackStrValue.Count == 0)
                {
                    //结束属性值
                    if (last != null)
                    {
                        String temp = sb.ToString();
                        last.Parse(ref temp);
                    }
                    isValue = false;
                    sb.Length = 0;
                }
                else
                {
                    //内容
                    sb.Append(cur);
                }
            }
            if (sb.Length > 0
                && last != null
                && last.Type == JsonPropertyType.Null)
            {
                //子对象
                String temp = sb.ToString();
                last.Parse(ref temp);
            }
        }
        /// <summary>
        /// 获取指定名称的属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public JsonProperty this[String propertyName]
        {
            get
            {
                JsonProperty result = null;
                if (mProperty != null && mProperty.ContainsKey(propertyName))
                {
                    result = mProperty[propertyName];
                }
                return result;
            }
            set
            {
                if (mProperty == null)
                {
                    mProperty = new Dictionary<string, JsonProperty>(StringComparer.OrdinalIgnoreCase);
                }
                if (mProperty.ContainsKey(propertyName))
                {
                    mProperty[propertyName] = value;
                }
                else
                {
                    mProperty.Add(propertyName, value);
                }
            }
        }
        /// <summary>
        /// 通过此泛型函数可直接获取指定类型属性的值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public virtual T Properties<T>(String propertyName) where T : class
        {
            JsonProperty p = this[propertyName];
            if (p != null)
            {
                return p.GetValue<T>();
            }
            return default(T);
        }
        /// <summary>
        /// 获取属性名称列表
        /// </summary>
        /// <returns></returns>
        public String[] GetPropertyNames()
        {
            if (mProperty == null)
                return null;
            String[] keys = null;
            if (mProperty.Count > 0)
            {
                keys = new String[mProperty.Count];
                mProperty.Keys.CopyTo(keys, 0);
            }
            return keys;
        }
        /// <summary>
        /// 移除一个属性
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public JsonProperty RemoveProperty(String propertyName)
        {
            if (mProperty != null
                && mProperty.ContainsKey(propertyName))
            {
                JsonProperty p = mProperty[propertyName];
                mProperty.Remove(propertyName);
                return p;
            }
            return null;
        }
        /// <summary>
        /// 是否为空对象
        /// </summary>
        /// <returns></returns>
        public bool IsNull()
        {
            return mProperty == null;
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(string.Empty);
        }
        /// <summary>
        /// 指定格式表达式，转换成字符串
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <returns></returns>
        public string ToString(String format)
        {
            return ToString(format, 0);
        }
        /// <summary>
        /// 指定格式表达式及深度，转换成字符串
        /// </summary>
        /// <param name="format">格式表达式</param>
        /// <param name="level">深度</param>
        /// <returns></returns>
        public virtual string ToString(String format, int level)
        {
            if (IsNull())
            {
                return "{}";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            level++;
            if (format == "F")
            {
                sb.Append("\r\n");
            }
            if (mProperty.Count > 0)
            {
                foreach (String key in mProperty.Keys)
                {
                    if (format == "F")
                    {
                        for (int i = 0; i < level; i++)
                        {
                            sb.Append("\t");
                        }
                    }
                    //sb.Append(key).Append(": ");
                    sb.Append("\"" + key + "\"").Append(":");               //键名带引号更标准
                    sb.Append(mProperty[key].ToString(format, level));
                    sb.Append(",");
                    if (format == "F")
                    {
                        sb.Append("\r\n");
                    }
                }
                if (format == "F")
                {
                    sb.Length -= 2;     //把多出的回车换行删掉
                }
                sb.Length -= 1;         //把多出的一个逗号删掉
            }
            if (format == "F")
            {
                sb.Append("\r\n");
                for (int i = 0; i < level - 1; i++)
                {
                    sb.Append("\t");
                }
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
    /// <summary>
    /// JSON对象属性
    /// </summary>
    public class JsonProperty
    {
        private JsonPropertyType mType;
        private String mValue;
        private JsonObject mObject;
        private List<JsonProperty> mList;
        private bool mBool;
        private double mNumber;

        /// <summary>
        /// 创建一个空的JSON对象属性
        /// </summary>
        public JsonProperty()
        {
            mType = JsonPropertyType.Null;
            mValue = null;
            mObject = null;
            mList = null;
        }
        /// <summary>
        /// 创建一个JSON对象属性
        /// </summary>
        /// <param name="value"></param>
        public JsonProperty(Object value)
        {
            SetValue(value);
        }
        /// <summary>
        /// 通过字符串构造一个JSON对象属性
        /// </summary>
        /// <param name="jsonString">Json字符串</param>
        public JsonProperty(String jsonString)
        {
            Parse(ref jsonString);
        }
        /// <summary>
        /// Json字符串解析
        /// </summary>
        /// <param name="jsonString">Json字符串</param>
        public void Parse(ref String jsonString)
        {
            if (String.IsNullOrEmpty(jsonString))
            {
                SetValue(null);
            }
            else
            {
                string first = jsonString.Substring(0, 1);
                string last = jsonString.Substring(jsonString.Length - 1, 1);
                //集合
                if (first == "[" && last == "]")
                {
                    SetValue(ParseArray(ref jsonString));
                }
                //JSON 对象
                else if (first == "{" && last == "}")
                {
                    SetValue(ParseObject(ref jsonString));
                }
                //字符串类型
                else if ((first == "'" || first == "\"") && first == last)
                {
                    SetValue(ParseString(ref jsonString));
                }
                //Bool类型
                else if (jsonString == "true" || jsonString == "false")
                {
                    SetValue(jsonString == "true");
                }
                //空类型
                else if (jsonString == "null")
                {
                    SetValue(null);
                }
                else
                {
                    double d;
                    //数值类型
                    if (double.TryParse(jsonString, out d))
                    {
                        SetValue(d);
                    }
                    else
                    {
                        SetValue(jsonString);
                    }
                }
            }
        }
        /// <summary>
        /// Json Array解析
        /// </summary>
        /// <param name="jsonString">Json字符串</param>
        /// <returns></returns>
        private List<JsonProperty> ParseArray(ref String jsonString)
        {
            List<JsonProperty> list = new List<JsonProperty>();
            int len = jsonString.Length;
            StringBuilder sb = new StringBuilder();
            Stack<Char> stackKey = new Stack<Char>();       //键名引号匹配
            Stack<Char> stackType = new Stack<Char>();      //括号（方括号或花括号）匹配
            Stack<Char> stackStrValue = new Stack<Char>();  //Value中的特殊字符需要保留
            bool conver = false;
            Char cur;
            for (int i = 1; i <= len - 2; i++)
            {
                cur = jsonString[i];
                if (cur == ' '
                    && stackKey.Count == 0
                    && stackStrValue.Count == 0)
                {
                    //键名及属性值以外的空格忽略调
                }
                else if ((cur == '\'' || cur == '\"')
                    && stackKey.Count == 0
                    && stackType.Count == 0)
                {
                    //进入键名
                    sb.Length = 0;
                    sb.Append(cur);
                    stackKey.Push(cur);
                }
                else if (cur == '\\' && stackKey.Count > 0
                    && !conver)
                {
                    sb.Append(cur);
                    conver = true;
                }
                else if (conver)
                {
                    conver = false;
                    //处理转义字符
                    if (cur == 'u')
                    {
                        sb.Append(new[] { cur, jsonString[i + 1], jsonString[i + 2], jsonString[i + 3] });
                        i += 4;
                    }
                    else
                    {
                        sb.Append(cur);
                    }
                }
                else if ((cur == '\'' || cur == '\"')
                    && stackKey.Count > 0
                    && stackKey.Peek() == cur
                    && stackType.Count == 0)
                {
                    //结束键名
                    sb.Append(cur);
                    //list.Add(new JsonProperty(sb.ToString()));
                    stackKey.Pop();
                }
                else if ((cur == '\'' || cur == '\"')
                    && stackStrValue.Count == 0)
                {
                    //进入属性值
                    sb.Append(cur);
                    stackStrValue.Push(cur);
                }
                else if ((cur == '\'' || cur == '\"')
                    && stackStrValue.Count > 0
                    && stackStrValue.Peek() == cur)
                {
                    sb.Append(cur);
                    stackStrValue.Pop();
                }
                else if ((cur == '[' || cur == '{')
                    && stackKey.Count == 0)
                {
                    if (stackType.Count == 0)
                    {
                        sb.Length = 0;
                    }
                    sb.Append(cur);
                    stackType.Push((cur == '[' ? ']' : '}'));
                }
                else if ((cur == ']' || cur == '}')
                    && stackKey.Count == 0
                    && stackType.Count > 0
                    && stackType.Peek() == cur)
                {
                    sb.Append(cur);
                    stackType.Pop();
                    if (stackType.Count == 0)
                    {
                        list.Add(new JsonProperty(sb.ToString()));
                        sb.Length = 0;
                    }
                }
                else if (cur == ','
                    && stackKey.Count == 0
                    && stackType.Count == 0)
                {
                    if (sb.Length > 0)
                    {
                        list.Add(new JsonProperty(sb.ToString()));
                        sb.Length = 0;
                    }
                }
                else
                {
                    sb.Append(cur);
                }
            }
            if (stackKey.Count > 0 || stackType.Count > 0)
            {
                list.Clear();
                throw new ArgumentException("Can not parse Json Array!");
            }
            if (sb.Length > 0)
            {
                list.Add(new JsonProperty(sb.ToString()));
            }
            return list;
        }
        /// <summary>
        /// Json String解析
        /// </summary>
        /// <param name="jsonString">Json字符串</param>
        /// <returns></returns>
        private String ParseString(ref String jsonString)
        {
            int len = jsonString.Length;
            StringBuilder sb = new StringBuilder();
            bool conver = false;
            Char cur;
            for (int i = 1; i <= len - 2; i++)
            {
                cur = jsonString[i];
                if (cur == '\\' && !conver)
                {
                    conver = true;
                }
                else if (conver)
                {
                    conver = false;
                    if (cur == '\\' || cur == '\"' || cur == '\'' || cur == '/')
                    {
                        sb.Append(cur);
                    }
                    else
                    {
                        if (cur == 'u')
                        {
                            String temp = new String(new[] { cur, jsonString[i + 1], jsonString[i + 2], jsonString[i + 3] });
                            sb.Append((char)Convert.ToInt32(temp, 16));
                            i += 4;
                        }
                        else
                        {
                            switch (cur)
                            {
                                case 'b':
                                    sb.Append('\b');
                                    break;
                                case 'f':
                                    sb.Append('\f');
                                    break;
                                case 'n':
                                    sb.Append('\n');
                                    break;
                                case 'r':
                                    sb.Append('\r');
                                    break;
                                case 't':
                                    sb.Append('\t');
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    sb.Append(cur);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Json Object解析
        /// </summary>
        /// <param name="jsonString">Json字符串</param>
        /// <returns></returns>
        private JsonObject ParseObject(ref String jsonString)
        {
            return new JsonObject(jsonString);
        }
        /// <summary>
        /// 定义一个索引器，如果属性是非数组的，返回本身
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns></returns>
        public JsonProperty this[int index]
        {
            get
            {
                JsonProperty r = null;
                if (mType == JsonPropertyType.Array)
                {
                    if (mList != null && (mList.Count - 1) >= index)
                    {
                        r = mList[index];
                    }
                }
                else if (index == 0)
                {
                    return this;
                }
                return r;
            }
        }
        /// <summary>
        /// 提供一个字符串索引，简化对Object属性的访问
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public JsonProperty this[String propertyName]
        {
            get
            {
                if (mType == JsonPropertyType.Object)
                {
                    return mObject[propertyName];
                }
                return null;
            }
            set
            {
                if (mType == JsonPropertyType.Object)
                {
                    mObject[propertyName] = value;
                }
                else
                {
                    throw new NotSupportedException("Json property not an object!");
                }
            }
        }
        /// <summary>
        /// JsonObject值
        /// </summary>
        public JsonObject Object
        {
            get
            {
                if (mType == JsonPropertyType.Object)
                    return mObject;
                return null;
            }
        }
        /// <summary>
        /// 字符串值
        /// </summary>
        public String Value
        {
            get
            {
                if (mType == JsonPropertyType.String)
                {
                    return mValue;
                }
                if (mType == JsonPropertyType.Number)
                {
                    return mNumber.ToString();
                }
                return null;
            }
        }
        /// <summary>
        /// 对于集合类型，添加一个元素
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public JsonProperty Add(Object value)
        {
            if (mType != JsonPropertyType.Null && mType != JsonPropertyType.Array)
            {
                throw new NotSupportedException("Json property not an Array and can not add element!");
            }
            if (mList == null)
            {
                mList = new List<JsonProperty>();
            }
            JsonProperty jp = new JsonProperty(value);
            mList.Add(jp);
            mType = JsonPropertyType.Array;
            return jp;
        }
        /// <summary>
        /// Array值，如果属性是非数组的，则封装成只有一个元素的数组
        /// </summary>
        public List<JsonProperty> Items
        {
            get
            {
                if (mType == JsonPropertyType.Array)
                {
                    return mList;
                }
                List<JsonProperty> list = new List<JsonProperty>();
                list.Add(this);
                return list;
            }
        }
        /// <summary>
        /// 数值
        /// </summary>
        public double Number
        {
            get
            {
                if (mType == JsonPropertyType.Number)
                {
                    return mNumber;
                }
                return double.NaN;
            }
        }
        /// <summary>
        /// 清空属性值，重置为默认值
        /// </summary>
        public void Clear()
        {
            mType = JsonPropertyType.Null;
            mValue = String.Empty;
            mObject = null;
            if (mList != null)
            {
                mList.Clear();
                mList = null;
            }
        }
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <returns></returns>
        public Object GetValue()
        {
            if (mType == JsonPropertyType.String)
            {
                return mValue;
            }
            if (mType == JsonPropertyType.Object)
            {
                return mObject;
            }
            if (mType == JsonPropertyType.Array)
            {
                return mList;
            }
            if (mType == JsonPropertyType.Bool)
            {
                return mBool;
            }
            if (mType == JsonPropertyType.Number)
            {
                return mNumber;
            }
            return null;
        }
        /// <summary>
        /// 获取指定类型的属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetValue<T>() where T : class
        {
            return (GetValue() as T);
        }
        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetValue(Object value)
        {
            if (value is String)
            {
                mType = JsonPropertyType.String;
                mValue = (String)value;
            }
            else if (value is List<JsonProperty>)
            {
                mList = ((List<JsonProperty>)value);
                mType = JsonPropertyType.Array;
            }
            else if (value is JsonObject)
            {
                mObject = (JsonObject)value;
                mType = JsonPropertyType.Object;
            }
            else if (value is bool)
            {
                mBool = (bool)value;
                mType = JsonPropertyType.Bool;
            }
            else if (value == null)
            {
                mType = JsonPropertyType.Null;
            }
            else
            {
                double d;
                if (double.TryParse(value.ToString(), out d))
                {
                    mNumber = d;
                    mType = JsonPropertyType.Number;
                }
                else
                {
                    throw new ArgumentException("Argument type invalid!");
                }
            }
        }
        /// <summary>
        /// 对于集合类型，返回元素的数目
        /// </summary>
        public virtual int Count
        {
            get
            {
                int c = 0;
                if (mType == JsonPropertyType.Array)
                {
                    if (mList != null)
                    {
                        c = mList.Count;
                    }
                }
                else
                {
                    c = 1;
                }
                return c;
            }
        }
        /// <summary>
        /// 返回属性值类型
        /// </summary>
        public JsonPropertyType Type
        {
            get { return mType; }
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(string.Empty);
        }
        /// <summary>
        /// 指定格式表达式转换成字符串
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public virtual string ToString(String format)
        {
            return ToString(format, 0);
        }
        /// <summary>
        /// 指定格式表达式及深度转换成字符串
        /// </summary>
        /// <param name="format">格式表达式</param>
        /// <param name="level">深度</param>
        /// <returns></returns>
        public virtual string ToString(String format, int level)
        {
            StringBuilder sb = new StringBuilder();
            if (mType == JsonPropertyType.String)
            {
                sb.Append("\"").Append(mValue).Append("\"");
                return sb.ToString();
            }
            if (mType == JsonPropertyType.Bool)
            {
                return mBool ? "true" : "false";
            }
            if (mType == JsonPropertyType.Number)
            {
                return mNumber.ToString();
            }
            if (mType == JsonPropertyType.Null)
            {
                return "null";
            }
            if (mType == JsonPropertyType.Object)
            {
                return mObject.ToString(format, level);
            }
            if (mList == null || mList.Count == 0)
            {
                sb.Append("[]");
            }
            else
            {
                sb.Append("[");
                level++;
                if (format == "F")
                {
                    sb.Append("\r\n");
                }
                if (mList.Count > 0)
                {
                    foreach (JsonProperty p in mList)
                    {
                        if (format == "F")
                        {
                            for (int i = 0; i < level; i++)
                            {
                                sb.Append("\t");
                            }
                        }
                        sb.Append(p.ToString(format, level));
                        sb.Append(",");
                        if (format == "F")
                        {
                            sb.Append("\r\n");
                        }
                    }
                    if (format == "F")
                    {
                        sb.Length -= 2;     //把多出的回车换行删掉
                    }
                    sb.Length -= 1;         //把多出的一个逗号删掉
                }
                if (format == "F")
                {
                    sb.Append("\r\n");
                    for (int i = 0; i < level - 1; i++)
                    {
                        sb.Append("\t");
                    }
                }
                sb.Append("]");
            }
            return sb.ToString();
        }
    }
}
