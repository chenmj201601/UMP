//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    8902978e-7d23-4342-8cf8-9717a28820db
//        CLR Version:              4.0.30319.42000
//        Name:                     PolicyServer
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPService91
//        File Name:                PolicyServer
//
//        Created by Charley at 2016/8/18 15:17:26
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using VoiceCyber.Common;


namespace UMPService91
{
    public class PolicyServer
    {
        TcpListener mListener;
        TcpClient mClient;
        const string POLICY_REQUEST_STRING = "<policy-file-request/>";
        private const int POLICY_PORT = 943;
        int mRecieveLength;
        byte[] mBytePolicy;
        byte[] mRecieveBuffer;

        private void InitializeData()
        {
            string policyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "policy.xml");
            if (!File.Exists(policyFile))
            {
                OnDebug(LogMode.Error, string.Format("Policy file not exist.\t{0}", policyFile));
                return;
            }
            try
            {
                using (FileStream fs = new FileStream(policyFile, FileMode.Open))
                {
                    mBytePolicy = new byte[fs.Length];
                    fs.Read(mBytePolicy, 0, mBytePolicy.Length);
                }
                mRecieveBuffer = new byte[POLICY_REQUEST_STRING.Length];
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("Read policy file data fail.\t{0}", ex.Message));
            }
        }

        public void StartSocketServer()
        {
            InitializeData();

            try
            {
                mListener = new TcpListener(IPAddress.Any, POLICY_PORT);
                mListener.Start();
                OnDebug(LogMode.Info, string.Format("Server started.\t{0}", mListener.LocalEndpoint));
                mListener.BeginAcceptSocket(OnBeginAccept, mListener);
            }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("Start server fail.\t{0}", ex.Message));
            }
        }

        public void StopSocketServer()
        {
            try
            {
                if (mListener != null)
                {
                    mListener.Stop();
                }
                mListener = null;
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopSocketServer fail.\t{0}", ex.Message));
            }
        }

        private void OnBeginAccept(IAsyncResult ar)
        {
            try
            {
                mClient = mListener.EndAcceptTcpClient(ar);
                mClient.Client.BeginReceive(mRecieveBuffer, 0, POLICY_REQUEST_STRING.Length, SocketFlags.None,
                    OnReceiveComplete, null);
                mListener.BeginAcceptSocket(OnBeginAccept, mListener);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("BeginAccept fail.\t{0}", ex.Message));
            }
        }

        private void OnReceiveComplete(IAsyncResult ar)
        {
            try
            {
                mRecieveLength += mClient.Client.EndReceive(ar);
                if (mRecieveLength < POLICY_REQUEST_STRING.Length)
                {
                    mClient.Client.BeginReceive(mRecieveBuffer, mRecieveLength,
                        POLICY_REQUEST_STRING.Length - mRecieveLength,
                        SocketFlags.None, OnReceiveComplete, null);
                    return;
                }
                string request = System.Text.Encoding.UTF8.GetString(mRecieveBuffer, 0, mRecieveLength);
                if (StringComparer.InvariantCultureIgnoreCase.Compare(request, POLICY_REQUEST_STRING) != 0)
                {
                    mClient.Client.Close();
                    return;
                }
                mClient.Client.BeginSend(mBytePolicy, 0, mBytePolicy.Length, SocketFlags.None,
                   OnSendComplete, null);
            }
            catch (Exception ex)
            {
                mClient.Client.Close();
                OnDebug(LogMode.Error, string.Format("OnReceiveComplete fail.\t{0}", ex.Message));
            }
            mRecieveLength = 0;
        }

        private void OnSendComplete(IAsyncResult ar)
        {
            try
            {
                mClient.Client.EndSendFile(ar);
            }
            catch (Exception ex)
            {
                //OnDebug(LogMode.Error, string.Format("OnSendComplete fail.\t{0}", ex.Message));
            }
            finally
            {
                mClient.Client.Close();
            }
        }


        #region Debug

        public event Action<LogMode, string, string> Debug;

        private void OnDebug(LogMode mode, string category, string msg)
        {
            if (Debug != null)
            {
                Debug(mode, category, msg);
            }
        }

        private void OnDebug(LogMode mode, string msg)
        {
            OnDebug(mode, "PolicyServer", msg);
        }

        #endregion

    }
}
