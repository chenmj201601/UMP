using System;
using System.ServiceProcess;
using VoiceCyber.Common;

namespace UMPService10
{
    class Program
    {

        /// <summary>
        /// Debug模式，专门用于本地调试
        /// </summary>
        public static bool IsDebug = false;
        /// <summary>
        /// 是否以控制台模式运行
        /// </summary>
        public static bool IsConsole = false;

        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                if (args[0].ToUpper() == "C")
                {
                    //以控制台运行
                    try
                    {
                        IsConsole = true;
                        MonitorServer MonitorServer = new MonitorServer();
                        MonitorServer.Debug += MonitorServer_Debug;
                        //调试模式
                        if (args.Length > 1 && args[1].ToUpper() == "D")
                        {
                            IsDebug = true;
                        }
                        MonitorServer.Start();
                        Console.ReadLine();
                        MonitorServer.Stop();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    return;
                }
            }
            //以Windows服务器运行
            MonitorService MonitorService = new MonitorService();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                MonitorService
            };
            ServiceBase.Run(ServicesToRun);
        }

        static void MonitorServer_Debug(LogMode mode, string category, string msg)
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
