//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c07ebcbb-0f19-4461-8ba4-6726e196637a
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyChangedExt
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Utilities
//        File Name:                PropertyChangedExt
//
//        created by Charley at 2014/7/23 15:35:12
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;

namespace VoiceCyber.Wpf.PropertyGrids.Utilities
{
    internal static class PropertyChangedExt
    {
        #region Notify Methods

        public static void Notify<TMember>(
          this INotifyPropertyChanged sender,
          PropertyChangedEventHandler handler,
          Expression<Func<TMember>> expression)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");

            if (expression == null)
                throw new ArgumentNullException("expression");

            var body = expression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("The expression must target a property or field.", "expression");

            string propertyName = PropertyChangedExt.GetPropertyName(body, sender.GetType());

            PropertyChangedExt.NotifyCore(sender, handler, propertyName);
        }

        public static void Notify(this INotifyPropertyChanged sender, PropertyChangedEventHandler handler, string propertyName)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");

            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            ReflectionHelper.ValidatePropertyName(sender, propertyName);

            PropertyChangedExt.NotifyCore(sender, handler, propertyName);
        }

        private static void NotifyCore(INotifyPropertyChanged sender, PropertyChangedEventHandler handler, string propertyName)
        {
            if (handler != null)
            {
                handler(sender, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region PropertyChanged Verification Methods

        internal static bool PropertyChanged(string propertyName, PropertyChangedEventArgs e, bool targetPropertyOnly)
        {
            string target = e.PropertyName;
            if (target == propertyName)
                return true;

            return (!targetPropertyOnly)
                && (string.IsNullOrEmpty(target));
        }

        internal static bool PropertyChanged<TOwner, TMember>(
          Expression<Func<TMember>> expression,
          PropertyChangedEventArgs e,
          bool targetPropertyOnly)
        {
            var body = expression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("The expression must target a property or field.", "expression");

            return PropertyChangedExt.PropertyChanged(body, typeof(TOwner), e, targetPropertyOnly);
        }

        internal static bool PropertyChanged<TOwner, TMember>(
          Expression<Func<TOwner, TMember>> expression,
          PropertyChangedEventArgs e,
          bool targetPropertyOnly)
        {
            var body = expression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("The expression must target a property or field.", "expression");

            return PropertyChangedExt.PropertyChanged(body, typeof(TOwner), e, targetPropertyOnly);
        }

        private static bool PropertyChanged(MemberExpression expression, Type ownerType, PropertyChangedEventArgs e, bool targetPropertyOnly)
        {
            var propertyName = PropertyChangedExt.GetPropertyName(expression, ownerType);

            return PropertyChangedExt.PropertyChanged(propertyName, e, targetPropertyOnly);
        }

        #endregion

        private static string GetPropertyName(MemberExpression expression, Type ownerType)
        {
            var targetType = expression.Expression.Type;
            if (!targetType.IsAssignableFrom(ownerType))
                throw new ArgumentException("The expression must target a property or field on the appropriate owner.", "expression");

            return ReflectionHelper.GetPropertyOrFieldName(expression);
        }
    }
}
