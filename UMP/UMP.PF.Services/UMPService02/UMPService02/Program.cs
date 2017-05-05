using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using VoiceCyber.Common;

namespace UMPService02
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        /// Debug模式，专门用于本地调试
        /// </summary>
        public static bool IsDebug = false;
        /// <summary>
        /// 是否以控制台模式运行
        /// </summary>
        public static bool IsConsole = false;

        static void Main(string[] args)
        {

            #region DEBUG 
            UMPService02 umpService02 = new UMPService02();

            if (args != null && args.Length > 0)
            {
                if (args[0].ToUpper() == "C")
                {
                    //以控制台运行
                    try
                    {
                        IsConsole = true;

                        umpService02.Debug += UmpService02_Debug;
                        //调试模式
                        if (args.Length > 1 && args[1].ToUpper() == "D")
                        {
                            IsDebug = true;
                        }
                        umpService02.TestStart();
                        Console.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    return;
                }
            }
            #endregion

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new UMPService02() 
            };
            ServiceBase.Run(ServicesToRun);
        }
        static void UmpService02_Debug(LogMode mode, string category, string msg)
        {
            if (mode == LogMode.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if (mode == LogMode.Info)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            if (mode == LogMode.Debug)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }
            if (!IsDebug)
            {
                if (mode == LogMode.Debug) { return; }
            }
            Console.WriteLine("{0}\t{1}\t{2}\t{3}", mode, DateTime.Now.ToString("HH:mm:ss.f"), category, msg);
        }
    }
}
