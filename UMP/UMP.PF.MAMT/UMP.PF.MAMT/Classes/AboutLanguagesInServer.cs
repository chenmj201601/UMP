using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMP.PF.MAMT.WCF_LanPackOperation;
using System.ServiceModel;

namespace UMP.PF.MAMT.Classes
{
   public class AboutLanguagesInServer
    {
       /// <summary>
        /// 调用WCF的OperationMthodA(传入的参数少 只传数据库连接信息时调用)
       /// </summary>
       /// <param name="strProtocol"></param>
       /// <param name="strHost"></param>
       /// <param name="strPort"></param>
       /// <param name="strDirName"></param>
       /// <param name="wcfName"></param>
       /// <param name="iSeconds"></param>
       /// <param name="dbInfo"></param>
       /// <returns></returns>
       public static ReturnResult WCFOperationMthodA(string strProtocol, string strHost, string strPort, int OperationID, DBInfo dbInfo)
       {
           List<string> lstArgs = new List<string>();
           lstArgs.Add(dbInfo.DbType.ToString());
           lstArgs.Add(dbInfo.Host);
           lstArgs.Add(dbInfo.Port);
           lstArgs.Add(dbInfo.ServiceName);
           lstArgs.Add(dbInfo.LoginName);
           lstArgs.Add(dbInfo.Password);

           BasicHttpBinding binding = Common.CreateBasicHttpBinding(60);
           //EndpointAddress adress = Common.CreateEndPoint("Http", serverInfo.Host, serverInfo.Port, "WcfServices/WCF_LanguagePackOperation", "LanPackOperation");
           EndpointAddress adress = Common.CreateEndPoint(strProtocol, strHost, strPort, "WcfServices", "Service00001");
           Service00001Client client = new Service00001Client(binding, adress);
           ReturnResult result = new ReturnResult();
           try
           {
               result = client.OperationMethodA(OperationID, lstArgs);
           }
           catch(Exception ex)
           {
               result.BoolReturn = false;
               result.StringReturn = ex.Message;
           }
           finally
           {
               if (client.State == CommunicationState.Opened)
               {
                   client.Close();
               }
           }
           client.Close();
           return result;
       }

       /// <summary>
       /// 调用WCF的OperationMthodA(传入的参数多 需整理好再传入时调用)
       /// </summary>
       /// <param name="strProtocol"></param>
       /// <param name="strHost"></param>
       /// <param name="strPort"></param>
       /// <param name="strDirName"></param>
       /// <param name="wcfName"></param>
       /// <param name="iSeconds"></param>
       /// <param name="OperationID"></param>
       /// <param name="lstArgs"></param>
       /// <returns></returns>
       public static ReturnResult WCFOperationMthodA(string strProtocol, string strHost, string strPort, int OperationID, List<string> lstArgs)
       {
           ReturnResult result = new ReturnResult();
           BasicHttpBinding binding = Common.CreateBasicHttpBinding(60);
           EndpointAddress adress = Common.CreateEndPoint(strProtocol, strHost, strPort, "WcfServices", "Service00001");
           Service00001Client client = new Service00001Client(binding, adress);
           try
           {
               result = client.OperationMethodA(OperationID, lstArgs);
           }
           catch (Exception ex)
           {
               result.BoolReturn = false;
               result.StringReturn = ex.Message;
           }
           finally
           {
               if (client.State == CommunicationState.Opened)
               {
                   client.Close();
               }
           }
           return result;
       }
    }
}
