//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cc8c0732-3130-4c6e-829b-bf401e026891
//        CLR Version:              4.0.30319.18063
//        Name:                     LocalMonitorHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                LocalMonitorHelper
//
//        created by Charley at 2015/4/30 11:19:17
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// 在线监视对象的帮助类
    /// 添加，获取监视对象
    /// </summary>
    public class LocalMonitorHelper
    {
        private bool mIsRememberObject;
        private object mListMonitorObjectLocker;

        /// <summary>
        /// 是否记录监视对象
        /// </summary>
        public bool IsRememberObject
        {
            get { return mIsRememberObject; }
            set { mIsRememberObject = value; }
        }

        private List<LocalMonitorObject> mListMonitorObjects;

        /// <summary>
        /// 监视对象集合
        /// </summary>
        public List<LocalMonitorObject> ListMonitorObjects
        {
            get { return mListMonitorObjects; }
        }

        /// <summary>
        /// 创建一个在线监视的帮助类
        /// </summary>
        public LocalMonitorHelper()
        {
            mListMonitorObjects = new List<LocalMonitorObject>();
            mListMonitorObjectLocker = new object();
            mIsRememberObject = false;
        }

        /// <summary>
        /// 指定是否记录监视对象并创建一个在线监视的帮助类
        /// </summary>
        /// <param name="isRemember"></param>
        public LocalMonitorHelper(bool isRemember)
            : this()
        {
            mIsRememberObject = isRemember;
        }

        /// <summary>
        /// 向集合中插入监视对象
        /// </summary>
        /// <param name="obj"></param>
        public void AddObject(LocalMonitorObject obj)
        {
            if (!IsRememberObject) { return; }
            //加锁，防止对集合的并发操作
            lock (mListMonitorObjectLocker)
            {
                mListMonitorObjects.Add(obj);
            }
        }

        /// <summary>
        /// 向集合中插入一个WebRequest对象
        /// </summary>
        /// <param name="request"></param>
        public void AddWebRequest(WebRequest request)
        {
            if (!IsRememberObject) { return; }
            LocalMonitorObject obj = new LocalMonitorObject();
            obj.Type = ConstValue.MONITOR_TYPE_WEBREQUEST;
            obj.Name = string.Empty;
            obj.Data = request.LogInfo();
            AddObject(obj);
        }

        /// <summary>
        /// 向集合中添加一个WebReturn对象
        /// </summary>
        /// <param name="webReturn"></param>
        public void AddWebReturn(WebReturn webReturn)
        {
            if (!IsRememberObject) { return; }
            LocalMonitorObject obj = new LocalMonitorObject();
            obj.Type = ConstValue.MONITOR_TYPE_WEBRETURN;
            obj.Name = string.Empty;
            obj.Data = webReturn.LogInfo();
            AddObject(obj);
        }

        /// <summary>
        /// 处理监视消息，从集合中帅选出指定的监视对象并返回
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public WebReturn DealMonitorMessage(WebRequest request)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Result = true;
            webReturn.Code = 0;
            try
            {
                //ListData
                //0     Open/Close ( 0 表示关闭，之后不再向列表中添加监视对象；1 表示开启）
                //1     Command (指令，0 表示获取监控列表中的对象）
                //2     Type ( 0 表示忽略类型）
                //3     Name （空表示忽略名称）
                //4     Index (跳过指定的个数，可选参数，默认取最后一个）
                if (request.ListData == null || request.ListData.Count < 4)
                {
                    webReturn.Result = false;
                    webReturn.Code = Defines.RET_PARAM_INVALID;
                    webReturn.Message = string.Format("ListData is null or count invalid");
                    return webReturn;
                }
                mIsRememberObject = request.ListData[0] == "1";
                int command;
                if (!int.TryParse(request.ListData[1], out command)
                    || command != 0)
                {
                    webReturn.Result = false;
                    webReturn.Code = Defines.RET_PARAM_INVALID;
                    webReturn.Message = string.Format("Command invalid");
                    return webReturn;
                }
                int type;
                if (!int.TryParse(request.ListData[2], out type))
                {
                    webReturn.Result = false;
                    webReturn.Code = Defines.RET_PARAM_INVALID;
                    webReturn.Message = string.Format("Type invalid");
                    return webReturn;
                }
                string name = request.ListData[3];
                int index = 0;
                if (request.ListData.Count >= 4)
                {
                    if (!int.TryParse(request.ListData[4], out index))
                    {
                        webReturn.Result = false;
                        webReturn.Code = Defines.RET_PARAM_INVALID;
                        webReturn.Message = string.Format("Index invalid");
                        return webReturn;
                    }
                }
                //加锁，防止对集合的并发操作
                lock (mListMonitorObjectLocker)
                {
                    var items = mListMonitorObjects.AsEnumerable();
                    if (type > 0)
                    {
                        items = items.Where(o => o.Type == type);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        items = items.Where(o => o.Name == name);
                    }
                    //反序
                    items = items.Reverse();
                    //跳过指定的个数
                    items = items.Skip(index);
                    //取最近一个对象
                    var obj = items.FirstOrDefault();
                    if (obj == null)
                    {
                        webReturn.Result = false;
                        webReturn.Code = Defines.RET_NOT_EXIST;
                        webReturn.Message = string.Format("Monitor object not exist");
                        return webReturn;
                    }
                    webReturn.Data = obj.Data;
                }
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = 1;
                webReturn.Message = ex.Message;
            }
            return webReturn;
        }
    }
}
