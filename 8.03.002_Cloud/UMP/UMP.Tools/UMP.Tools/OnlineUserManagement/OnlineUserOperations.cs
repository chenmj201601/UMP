using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using UMP.Tools.UMPWcfService00003;

namespace UMP.Tools.OnlineUserManagement
{
    public static class OnlineUserOperations
    {
        private static OperationDataArgs I00003OperationReturn = new OperationDataArgs();

        public static DataSet GetRentList(ref string AStrReturn)
        {
            DataSet LDataSetReturn = new DataSet();

            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;
            List<string> LListWcfArgs = new List<string>();

            try
            {
                AStrReturn = string.Empty;

                List<string> LListStrDBProfile = App.GSystemMainWindow.GetCurrentDatabaseProfile();
                List<string> LListStrAppServer = App.GSystemMainWindow.GetCurrentAppServerConnection();

                LBasicHttpBinding = App.CreateBasicHttpBinding(true, 25);
                LEndpointAddress = App.CreateEndpointAddress(LListStrAppServer[0], LListStrAppServer[1], true, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);

                foreach (string LStrSingleProfile in LListStrDBProfile) { LListWcfArgs.Add(LStrSingleProfile); }

                I00003OperationReturn = LService00003Client.OperationMethodA(8, LListWcfArgs);
                if (I00003OperationReturn.BoolReturn)
                {
                    LDataSetReturn = I00003OperationReturn.DataSetReturn;
                }
                else
                {
                    AStrReturn = I00003OperationReturn.StringReturn;
                }
            }
            catch (Exception ex)
            {
                AStrReturn = "C002001" + AscCodeToChr(27) + ex.Message;
            }
            finally
            {
                if (LService00003Client != null)
                {
                    if (LService00003Client.State == CommunicationState.Opened) { LService00003Client.Close(); LService00003Client = null; }
                }
            }

            return LDataSetReturn;
        }

        public static DataTable GetRentOnlineUser(string AStrRentToken, ref string AStrReturn)
        {
            DataTable LDataTableOnlineUser = new DataTable();

            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;
            List<string> LListWcfArgs = new List<string>();

            try
            {
                AStrReturn = string.Empty;
                LDataTableOnlineUser.TableName = "T_100_" + AStrRentToken;

                List<string> LListStrDBProfile = App.GSystemMainWindow.GetCurrentDatabaseProfile();
                List<string> LListStrAppServer = App.GSystemMainWindow.GetCurrentAppServerConnection();

                LBasicHttpBinding = App.CreateBasicHttpBinding(true, 25);
                LEndpointAddress = App.CreateEndpointAddress(LListStrAppServer[0], LListStrAppServer[1], true, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);

                foreach (string LStrSingleProfile in LListStrDBProfile) { LListWcfArgs.Add(LStrSingleProfile); }
                LListWcfArgs.Add(AStrRentToken); LListWcfArgs.Add("01");

                I00003OperationReturn = LService00003Client.OperationMethodA(9, LListWcfArgs);
                if (!I00003OperationReturn.BoolReturn)
                {
                    AStrReturn = I00003OperationReturn.StringReturn;
                }
                else
                {
                    LDataTableOnlineUser = I00003OperationReturn.DataSetReturn.Tables[0].Copy();
                    LDataTableOnlineUser.TableName = "T_100_" + AStrRentToken;
                }
            }
            catch (Exception ex)
            {
                AStrReturn = "C002002" + AscCodeToChr(27) + ex.Message;
            }
            finally
            {
                if (LService00003Client != null)
                {
                    if (LService00003Client.State == CommunicationState.Opened) { LService00003Client.Close(); LService00003Client = null; }
                }
            }

            return LDataTableOnlineUser;
        }

        public static bool CancellationOnlineUser(ListViewItemSingleOnlineUser ASingleOnlineUser, ref string AStrReturn)
        {
            bool LBoolReturn = true;

            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;
            List<string> LListWcfArgs = new List<string>();

            try
            {
                AStrReturn = string.Empty;
                List<string> LListStrDBProfile = App.GSystemMainWindow.GetCurrentDatabaseProfile();
                List<string> LListStrAppServer = App.GSystemMainWindow.GetCurrentAppServerConnection();
                
                LBasicHttpBinding = App.CreateBasicHttpBinding(true, 25);
                LEndpointAddress = App.CreateEndpointAddress(LListStrAppServer[0], LListStrAppServer[1], true, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);


                foreach (string LStrSingleProfile in LListStrDBProfile) { LListWcfArgs.Add(LStrSingleProfile); }
                LListWcfArgs.Add(ASingleOnlineUser.RentToken); LListWcfArgs.Add("02");
                LListWcfArgs.Add(ASingleOnlineUser.SessionID);

                I00003OperationReturn = LService00003Client.OperationMethodA(9, LListWcfArgs);
                LBoolReturn = I00003OperationReturn.BoolReturn;
                AStrReturn = I00003OperationReturn.StringReturn;
            }
            catch (Exception ex)
            {
                AStrReturn = "C002003" + AscCodeToChr(27) + ex.Message;
            }
            finally
            {
                if (LService00003Client != null)
                {
                    if (LService00003Client.State == CommunicationState.Opened) { LService00003Client.Close(); LService00003Client = null; }
                }
            }

            return LBoolReturn;
        }

        #region 生成分割符号 AscCodeToChr
        private static string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }
        #endregion
    }
}
