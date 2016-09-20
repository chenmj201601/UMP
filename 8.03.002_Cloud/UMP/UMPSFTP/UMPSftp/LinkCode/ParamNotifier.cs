using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace UMPCommon
{
    public class StateObjectPN
    {
        public Socket workSocket = null;
        public const int BUFFER_SIZE = 1024;
        public byte[] buffer = new byte[BUFFER_SIZE];
        public StringBuilder sb = new StringBuilder();
    }

    class ParamNotifier
    {
        public ParamNotifier(string notified_machineID)
        {
            machine_id = notified_machineID;
        }
        string machine_id = "";
        string last_change_id = "";
        Socket s = null;
        public delegate void NotifieFunc();
        public NotifieFunc FuncNotifier { get; set; }

        void Read_Callback(IAsyncResult ar)
        {
            StateObjectPN so = (StateObjectPN)ar.AsyncState;
            Socket s = so.workSocket;
            EndPoint tempRemoteEp = s.LocalEndPoint;

            try
            {
                int read = s.EndReceive(ar);
                so.sb.Append(Encoding.ASCII.GetString(so.buffer, 0, read));

                if (read == so.buffer.Length)
                {
                    s.BeginReceiveFrom(so.buffer, 0, StateObjectPN.BUFFER_SIZE, 0, ref tempRemoteEp,
                                          new AsyncCallback(Read_Callback), so);
                    return;
                }
                byte[] receive_data = Encoding.Default.GetBytes(so.sb.ToString());
                int structSize = Marshal.SizeOf(typeof(NETPACK_BASEHEAD_APPLICATION_VER1));
                byte[] byteMsg = new byte[structSize];
                Array.Copy(receive_data, 64, byteMsg, 0, structSize);
                NETPACK_BASEHEAD_APPLICATION_VER1 vr = (NETPACK_BASEHEAD_APPLICATION_VER1)Common.BytesToStruct(byteMsg, typeof(NETPACK_BASEHEAD_APPLICATION_VER1));
                byte[] byteNetMessage = new byte[Marshal.SizeOf(typeof(_TAG_NETPACK_MESSAGE))];
                Array.Copy(receive_data, 48, byteNetMessage, 0, 16);
                _TAG_NETPACK_MESSAGE message = (_TAG_NETPACK_MESSAGE)Common.BytesToStruct(byteNetMessage, typeof(_TAG_NETPACK_MESSAGE));
                byte[] byteData = new byte[vr._datasize];
                Array.Copy(receive_data, 64 + structSize, byteData, 0, byteData.Length);
                string json = Encoding.Default.GetString(byteData);
                JObject jo = JObject.Parse(json);

                if (machine_id == jo["ParamChange"]["MachineID"].ToString())
                {
                    if (last_change_id != jo["ParamChange"]["ChangeID"].ToString())
                    {
                        last_change_id = jo["ParamChange"]["ChangeID"].ToString();
                        FuncNotifier();
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.ErrorLog(ex);
            }
            finally
            {
                so.sb.Clear();
                s.BeginReceiveFrom(so.buffer, 0, StateObjectPN.BUFFER_SIZE, 0, ref tempRemoteEp,
                                      new AsyncCallback(Read_Callback), so);
            }
        }

        public void Connect(string notif_ip, int notif_port)
        {
            //与和发送方相似的方法建立一个socket
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, notif_port);
            //任何监听端口为4567的IP都将收到数据
            IPAddress ip = IPAddress.Parse(notif_ip);
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            }
            else if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                s = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
            }
            s.SetSocketOption(SocketOptionLevel.IP,
                              SocketOptionName.AddMembership,
                       new MulticastOption(ip, IPAddress.Any));
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            s.Bind(ipep);
            try
            {
                EndPoint tempRemoteEP = (EndPoint)ipep;
                StateObjectPN so2 = new StateObjectPN();
                so2.workSocket = s;
                s.BeginReceiveFrom(so2.buffer, 0, StateObjectPN.BUFFER_SIZE, 0, ref tempRemoteEP,
                                      new AsyncCallback(Read_Callback), so2);
            }
            catch (Exception e)
            {
                LogHelper.ErrorLog(e);
            }
        }
    }
}
