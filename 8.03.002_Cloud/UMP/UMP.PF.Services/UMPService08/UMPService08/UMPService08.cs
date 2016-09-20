using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace UMPService08
{
    public partial class UMPService08 : ServiceBase
    {
        bool IBoolUMPService08ThreadWorking = false;
        ABCDManager IABCDManager = new ABCDManager();
        public UMPService08( string [] args)
        {
            InitializeComponent();
            if (args.Length > 0)
            {
                Console.WriteLine(args[0]);
                if (args[0].Trim().ToLower() == "-start")
                {
                    OnStart(args);
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Thread umpService08Thread = new Thread(UMPService08ThreadWorking);
                IBoolUMPService08ThreadWorking = true;
                umpService08Thread.Start(this);
                FileLog.WriteInfo("UMPService08()", "umpService08Thread Start()");
            }
            catch (Exception ex)
            {

                FileLog.WriteError("UMPService08 OnStart() ", ex.Message);
            }            
        }

        public static void UMPService08ThreadWorking(object o) 
        {
            try
            {
                UMPService08 umpService08 = o as UMPService08;

                //检查数据库配置

                if (umpService08.IBoolUMPService08ThreadWorking)
                    umpService08.IABCDManager.ABCDManagerStartup();
                FileLog.WriteError("UMPService08ThreadWorking() ", "umpService08._ABCDManager.StatisticsStartup()");
            }
            catch (Exception ex)
            {

                FileLog.WriteError("UMPService08ThreadWorking() ", ex.Message);
            }

        }

        protected override void OnStop()
        {
            try
            {
                IABCDManager.ABCDManagerStop();
            }
            catch (Exception ex)
            {
                FileLog.WriteError("UMPService08 OnStop() ", ex.Message);
            }
            
        }
    }
}
