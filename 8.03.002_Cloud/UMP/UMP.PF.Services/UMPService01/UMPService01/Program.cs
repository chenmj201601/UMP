using System.ServiceProcess;

namespace UMPService01
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            System.Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new UMPService01() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
