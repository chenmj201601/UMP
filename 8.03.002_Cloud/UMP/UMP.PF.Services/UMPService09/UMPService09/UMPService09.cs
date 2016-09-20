using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using UMPService09.Log;

namespace UMPService09
{
    public partial class UMPService09 : ServiceBase
    {

        bool IBoolUMPService09ThreadWorking = false;
        PMManager IPMManager = new PMManager();

        public UMPService09(string [] args)
        {
            InitializeComponent();
            //OnStart(args);
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
                Thread umpService09Thread = new Thread(UMPService09ThreadWorking);
                IBoolUMPService09ThreadWorking = true;
                umpService09Thread.Start(this);
                FileLog.WriteInfo("UMPService09()", "UMPService09Thread Start()");
            }
            catch (Exception ex)
            {

                FileLog.WriteError("UMPService09 OnStart() ", ex.Message);
            }  
        }

        protected override void OnStop()
        {
            try
            {
                IPMManager.PMManagerStop();
            }
            catch (Exception ex)
            {
                FileLog.WriteError("UMPService09 OnStop() ", ex.Message);
            }
        }

        public static void UMPService09ThreadWorking(object o)
        {
            try
            {
                UMPService09 umpService09 = o as UMPService09;
                if (umpService09.IBoolUMPService09ThreadWorking)
                    umpService09.IPMManager.PMManagerStartup();
                FileLog.WriteError("UMPService09ThreadWorking() ", "UMPService09.UMPService09ThreadWorking Startup()");
            }
            catch (Exception ex)
            {

                FileLog.WriteError("UMPService09ThreadWorking() ", ex.Message);
            }

        }
    }
}
