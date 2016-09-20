using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Configuration;
using UMPCommon;

namespace UMPSftp
{
    class MainService
    {
        bool _Exit = false;
        readonly string ini_path = "\\voicecyber\\UMP\\config\\localmachine.ini";

        void UpdateParam()
        {
            Parameters param = Parameters.Instance();
            if (!param.ReadConfig())
            {
                LogHelper.ErrorLog("UpdateParam failed,use before param");
            }
        }

        public void StartWork()
        {
            try
            {
                string app_path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string ini_file = app_path + ini_path;
                string machine_id = Common.ReadValue("LocalMachine", "MachineID", ini_file);
                string log_path = Common.ReadValue("LocalMachine", "LogPath", ini_file);
                System.Diagnostics.Process processes = System.Diagnostics.Process.GetCurrentProcess();
                log_path += string.Format("\\{0}\\", processes.ProcessName);
                LogHelper.AutoOutputFunName = true;
                LogHelper.LogInitial(log_path, false);
                LogHelper.InfoLog(string.Format("MainService Start, Log path is:{0}", log_path));

                string stmp = Common.ReadValue("LocalMachine", "SubscribeAddress", ini_file);
                string[] addresses = stmp.Split(',');
                ParamNotifier obj = new ParamNotifier(machine_id);
                obj.FuncNotifier = UpdateParam;
                if (addresses.Length == 0)
                {
                    LogHelper.ErrorLog("not found subscribeAddress info,using default value:224.0.2.26,3789");
                    obj.Connect("224.0.2.26", 3789);
                }
                else
                    obj.Connect(addresses[0], Convert.ToInt16(addresses[1]));

                UpdateParam();
            }
            catch (System.Exception ex)
            {
                LogHelper.ErrorLog(ex);
            }
            Thread thr = new Thread(new ThreadStart(WorkerThread));
            thr.Start();
        }

        public void Shutdown()
        {
            _Exit = true;
        }

        void WorkerThread()
        {
            Parameters pars = Parameters.Instance();
            SftpWorker sftp_worker = new SftpWorker();
            ls_FTP_Server ftp_worker = new ls_FTP_Server(pars.SftpParam.RootDir, "all", pars.SftpParam.FTP_Port + 1);

            sftp_worker.StartSFtpServer();
            TimeSpan sp = new TimeSpan();
            while (!_Exit)
            {
                if (!Common.IntervalTime(ref sp, 15))
                {
                    LogHelper.InfoLog("Main Service actived");
                }
                Thread.Sleep(1000);
            }
            ftp_worker.EndServer();
            sftp_worker.StopSFtpServer();
            LogHelper.InfoLog("MainService stopped!");
        }
    }
}
