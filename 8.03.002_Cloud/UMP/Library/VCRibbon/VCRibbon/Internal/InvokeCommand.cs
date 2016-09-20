//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    39a4ca43-6f2f-4456-a7f2-f48f1cc4e1d1
//        CLR Version:              4.0.30319.18444
//        Name:                     InvokeCommand
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Ribbon.Internal
//        File Name:                InvokeCommand
//
//        created by Charley at 2014/5/28 11:23:31
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace VoiceCyber.Ribbon.Internal
{
    /// <summary>
    /// This trigger action binds a command/command parameter for MVVM usage with 
    /// a Blend based trigger.  This is used in place of the one in the Blend samples - 
    /// it has a problem in it as of the current (first) release.  Once it is fixed, this
    /// command can go away.
    /// </summary>
    [DefaultTrigger(typeof(ButtonBase), typeof(System.Windows.Interactivity.EventTrigger), "Click")]
    [DefaultTrigger(typeof(UIElement), typeof(System.Windows.Interactivity.EventTrigger), "MouseLeftButtonDown")]
    public class InvokeCommand : TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// ICommand to execute
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommand), new PropertyMetadata(null));

        /// <summary>
        /// Command parameter to pass to command execution
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommand), new PropertyMetadata(null));

        /// <summary>
        /// Command to execute
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Command parameter
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// This is called to execute the command when the trigger conditions are satisfied.
        /// </summary>
        /// <param name="parameter">parameter (not used)</param>
        protected override void Invoke(object parameter)
        {
            var commandParameter = BindingOperations.IsDataBound(this, CommandParameterProperty)
                ? CommandParameter
                : parameter;
            var command = this.Command;

            if ((command != null) && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }
    }
}
