//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c7fb39af-beb1-4479-81be-456d90f2706b
//        CLR Version:              4.0.30319.18444
//        Name:                     IResourcePropertyEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Editors
//        File Name:                IResourcePropertyEditor
//
//        created by Charley at 2015/4/1 11:22:32
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using UMPS1110.Models;

namespace UMPS1110.Editors
{
    /// <summary>
    /// 属性编辑框的公共操作及事件
    /// </summary>
   public interface IResourcePropertyEditor
   {
       /// <summary>
       /// 更新
       /// </summary>
       void RefreshValue();

       /// <summary>
       /// 属性值更改的事件
       /// </summary>
       event RoutedPropertyChangedEventHandler<PropertyValueChangedEventArgs> PropertyValueChanged;
   }
}
