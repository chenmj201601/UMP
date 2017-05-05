using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Configuration;
using log4net;
using log4net.Appender;

#region 如使用配置模式需要在app.config或web.config中添加以下配置信息
/*
  <configSections>
    <section name="log4net" type="System.Configuration.IgnoreSectionHandler"/>
  </configSections>
  <log4net>
    <!--定义输出到文件中-->
    <appender name="RollingLogFileAppender" type="log4net.Appender.RollingFileAppender">
      <!--定义文件存放位置-->
      <file value="log\\"/>
      <appendToFile value="true"/>
      <!--最小锁定模型以允许多个进程可以写入同一个文件-->
      <param name="lockingModel"  type="log4net.Appender.FileAppender+MinimalLock" />
      <rollingStyle value="Date"/>
      <datePattern value="yyyyMMdd'.log'"/>
      <staticLogFileName value="false"/>
      <param name="MaxSizeRollBackups" value="100"/>
      <layout type="log4net.Layout.PatternLayout">
        <!--每条日志末尾的文字说明-->
        <!--输出格式-->
        <!--样例：2008-03-26 13:42:32,111 [10] INFO  Log4NetDemo.MainClass [(null)] - info-->
        <conversionPattern value="Time:%date ThreadID:[%thread] LogLevel:%-5level %logger property:[%property{NDC}]%nMessage:%message%n%newline"/>
      </layout>
    </appender>
    <!--定义输出到控制台命令行中-->
    <appender name="ConsoleAppender" type="log4net.Appender.ConsoleAppender">
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="Time:%date ThreadID:[%thread] LogLevel:%-5level %logger property:[%property{NDC}]%nMessage:%message%n%newline"/>
      </layout>
    </appender>
    <root>
      <level value="INFO"/>
      <!--文件形式记录日志-->
      <appender-ref ref="RollingLogFileAppender"/>
      <appender-ref ref="ConsoleAppender"/>
    </root>
  </log4net>
*/
#endregion

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace UMPCommon
{
    static class LogHelper
    {
        static private ILog log = null;
        static public bool AutoOutputFunName = false;
        static RollingFileAppender CreateRollingFileAppender(string log_path)
        {
            log4net.Appender.RollingFileAppender rfa = new log4net.Appender.RollingFileAppender();
            rfa.Name = "RollingLogFileAppender";
            rfa.File = log_path;
            rfa.AppendToFile = true;
            rfa.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
            rfa.DatePattern = "yyyyMMdd'.log'";
            rfa.StaticLogFileName = false;
            rfa.MaxSizeRollBackups = 100;
            rfa.LockingModel = new log4net.Appender.FileAppender.MinimalLock();
            rfa.Layout = new log4net.Layout.PatternLayout("Time:%date ThreadID:[%thread] LogLevel:%-5level %nMessage:%message%n%newline");
            rfa.ActivateOptions();
            return rfa;
        }
        static ConsoleAppender CreateConsoleAppender()
        {
            log4net.Appender.ConsoleAppender rfa = new log4net.Appender.ConsoleAppender();
            rfa.Name = "ConsoleAppender";
            rfa.Layout = new log4net.Layout.PatternLayout("Time:%date ThreadID:[%thread] LogLevel:%-5level %nMessage:%message%n%newline");
            return rfa;
        }
        static void ConfigLog(string log_path)
        {
            log4net.Config.BasicConfigurator.Configure(CreateConsoleAppender());
            log4net.Config.BasicConfigurator.Configure(CreateRollingFileAppender(log_path));
            LogManager.GetRepository().Threshold = log4net.Core.Level.Info;
        }
        static public void LogInitial(string log_path = "", bool use_cfg = true)
        {
            try
            {
                if (ConfigurationManager.AppSettings["LogUseCfg"] == "1")
                {
                    log_path = "";
                    use_cfg = true;
                }
                if (use_cfg)
                {
                    if (!string.IsNullOrEmpty(log_path))
                    {
                        var appender = LogManager.GetRepository().GetAppenders();
                        var rolling = appender.First(p => p.Name == "RollingLogFileAppender") as RollingFileAppender;
                        rolling.File = log_path;
                        rolling.ActivateOptions();
                    }
                }
                else
                {
                    ConfigLog(log_path);
                }
                log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                InfoLog(string.Format("Program version is:{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version));
            }
            catch (InvalidOperationException)
            {
                ConfigLog(log_path);
                log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                ErrorLog("not found log4net cfg, use No Cfg mode");
                return;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
        }

        public static void DebugLog(object msg)
        {
            if (log != null && log.IsDebugEnabled)
            {
                string fun_name = "";
                if (AutoOutputFunName)
                {
                    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                    System.Diagnostics.StackFrame sfs = st.GetFrame(1);
                    fun_name = sfs.GetMethod().DeclaringType.FullName + "." + sfs.GetMethod().Name + "()";
                }
                log.Debug(fun_name + " " + msg);
            }
        }
        public static void InfoLog(object msg)
        {
            if (log != null && log.IsInfoEnabled)
            {
                string fun_name = "";
                if (AutoOutputFunName)
                {
                    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                    System.Diagnostics.StackFrame sfs = st.GetFrame(1);
                    fun_name = sfs.GetMethod().DeclaringType.FullName + "." + sfs.GetMethod().Name + "()";
                }
                log.Info(fun_name + " " + msg);
            }
        }
        public static void WarnLog(object msg)
        {
            if (log != null && log.IsWarnEnabled)
            {
                string fun_name = "";
                if (AutoOutputFunName)
                {
                    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                    System.Diagnostics.StackFrame sfs = st.GetFrame(1);
                    fun_name = sfs.GetMethod().DeclaringType.FullName + "." + sfs.GetMethod().Name + "()";
                }
                log.Warn(fun_name + " " + msg);
            }
        }
        public static void ErrorLog(object msg)
        {
            if (log != null && log.IsErrorEnabled)
            {
                string fun_name = "";
                if (AutoOutputFunName)
                {
                    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                    System.Diagnostics.StackFrame sfs = st.GetFrame(1);
                    fun_name = sfs.GetMethod().DeclaringType.FullName + "." + sfs.GetMethod().Name + "()";
                }
                log.Error(fun_name + " " + msg);
            }
        }
        public static void FatalLog(object msg)
        {
            if (log != null && log.IsFatalEnabled)
            {
                string fun_name = "";
                if (AutoOutputFunName)
                {
                    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                    System.Diagnostics.StackFrame sfs = st.GetFrame(1);
                    fun_name = sfs.GetMethod().DeclaringType.FullName + "." + sfs.GetMethod().Name + "()";
                }
                log.Fatal(fun_name + " " + msg);
            }
        }
    }
}
