using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace UMPSftp
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }
        MainService Mainser = new MainService();

        protected override void OnStart(string[] args)
        {
            Mainser.StartWork();
        }

        protected override void OnStop()
        {
            Mainser.Shutdown();
        }
    }
}
