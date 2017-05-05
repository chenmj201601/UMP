package com.ump.servlet;

import java.io.File;
import java.io.IOException;
import java.io.PrintWriter;
import java.rmi.RemoteException;
import java.sql.Connection;

import javax.print.DocFlavor.STRING;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import com.ump.model.Permission;
import com.ump.model.User;
import com.ump.util.*;
import java.util.ArrayList;

import org.apache.axis2.AxisFault;
import org.jdom.*;
import org.jdom.input.*;
import com.ump.wcf.*;

public class LoginServlt extends HttpServlet { 

	public LoginServlt() {
		super();
	}

	public void doGet(HttpServletRequest request, HttpServletResponse response)
			throws ServletException, IOException {
		response.setContentType("text/html; charset=utf-8");
		request.setCharacterEncoding("utf-8");
		response.setCharacterEncoding("utf-8");
		PrintWriter outmsg = response.getWriter();
		String userName = request.getParameter("userName");
		String password = request.getParameter("password");
		if (!userName.equals("") && !password.equals(""))
		{
			HttpSession session = request.getSession();
			String lanset = request.getParameter("lanset");
			if (lanset == null || lanset.equals(""))
				lanset = "2052";

			ArrayList<String> strArrylist = new ArrayList<String>();
			String path3 = request.getSession().getServletContext().getRealPath("")
					+ "\\WEB-INF\\classes\\com\\ump\\servlet";

			SetLanguages(lanset, session,path3);
			strArrylist = GetDataBaseInfo(path3);
			if (strArrylist.size() != 6) {			
				//outmsg.write(showParaName[43]);
				outmsg.print("<script>alert('"+showParaName[43]+"');window.location.href='Login.jsp'</script>");
				return;
			} else {
				// 存入数据库
				session.setAttribute("DBInfoInit", strArrylist);
				ArrayList<String> aryListTemp = StringUtil
						.getJavaConnectStrings(strArrylist);
				session.setAttribute("DBInfo", aryListTemp);
			}

			// 从xml文件里取得wcf信息
			ArrayList<String> arrayListWcfinfoArrayList = new ArrayList<String>();
			arrayListWcfinfoArrayList = GetWcfInfo(path3);
			session.setAttribute("WcfInfo", arrayListWcfinfoArrayList);

			// 验证登录
			boolean falg = validateLogin(request,  response, arrayListWcfinfoArrayList,
					userName, password);
			
			if (falg) {
				//outmsg.write("UMPMAIN");
				GetPermission(request,response);
				response.sendRedirect("Main.jsp");
			} else {
				//outmsg.write(showParaName[42]);
				outmsg.print("<script>alert('"+showParaName[42]+"');window.location.href='Login.jsp'</script>");
			}
		}
		else {
			outmsg.print("<script>window.location.href='Login.jsp'</script>");
		}
	}

	// 读取xml里数据库连接串 
	public ArrayList GetDataBaseInfo(String StrPath) {
		ArrayList<String> strArray = new ArrayList<String>();

		String relativelyPath = System.getProperty("user.dir");
		StringBuilder sBuilder = new StringBuilder();
		sBuilder.append(StrPath).append("\\").append("Connection.xml");

		File directory = new File(".");
		String s = directory.getAbsolutePath();
		try {

			SAXBuilder builder = new SAXBuilder();
			File filetempFile = new File(sBuilder.toString());
			if (filetempFile.exists()) {
				Document dxxDocument = builder.build(new File(sBuilder
						.toString()));
				Element fooElement = dxxDocument.getRootElement();

				java.util.List childelement = fooElement.getChildren();
				String dbtypeString = fooElement.getAttributeValue("P01")
						.toString();
				strArray.add(dbtypeString);

				Element eSql = (Element) childelement.get(0);
				String serviceip = eSql.getAttributeValue("P01").toString();
				strArray.add(serviceip);

				String servicePortString = eSql.getAttributeValue("P02")
						.toString();
				strArray.add(servicePortString);

				String databaseNameString = eSql.getAttributeValue("P03")
						.toString();
				strArray.add(databaseNameString);

				String dbUserNameString = eSql.getAttributeValue("P04")
						.toString();
				strArray.add(dbUserNameString);

				String dbPassword = eSql.getAttributeValue("P05").toString();
				strArray.add(dbPassword);
			}

		} catch (Exception e) {
			e.printStackTrace();
		}

		return strArray;
	}

	// 得到Wcf的信息
	public ArrayList GetWcfInfo(String StrPath) {
		ArrayList<String> strArray = new ArrayList<String>();

		String relativelyPath = System.getProperty("user.dir");
		StringBuilder sBuilder = new StringBuilder();
		sBuilder.append(StrPath).append("\\").append("Connection.xml");

		File directory = new File(".");
		String s = directory.getAbsolutePath();
		try {

			SAXBuilder builder = new SAXBuilder();
			File filetempFile = new File(sBuilder.toString());
			if (filetempFile.exists()) {
				Document dxxDocument = builder.build(new File(sBuilder
						.toString()));
				Element fooElement = dxxDocument.getRootElement();
				String ExtensionType = fooElement.getAttributeValue("P02")
						.toString();
				strArray.add(ExtensionType);// E为分机 T为真实分机

				java.util.List childelement = fooElement.getChildren();

				Element eSql = (Element) childelement.get(1);
				String wcf1 = eSql.getAttributeValue("P01").toString();
				strArray.add(wcf1);

				eSql = (Element) childelement.get(2);
				String wcf2 = eSql.getAttributeValue("P01").toString();
				strArray.add(wcf2);
			}

		} catch (Exception e) {
			// TODO: handle exception
		}
		return strArray;
	}

	public Boolean validateLogin(HttpServletRequest request, HttpServletResponse response,
			ArrayList<String> arrayListWcfinfoArrayList, String UserName,
			String PassWord) throws AxisFault {
		boolean flag = true;
		String remoteIp = request.getRemoteAddr();// 获取客户端的ip搜索
		String serverName1 = request.getRemoteHost();//
		if(!StringUtil.isboolIp(remoteIp))
		{
			remoteIp="127.0.0.1";
			serverName1="127.0.0.1";
		}

		Service000A1Stub ssssteStub1;
		ssssteStub1 = new Service000A1Stub(arrayListWcfinfoArrayList.get(2));

		Service000A1Stub.DoOperation repsssDoOperationResponse1 = new Service000A1Stub.DoOperation();
		Service000A1Stub.SDKRequest sdkRequest1 = new Service000A1Stub.SDKRequest();
		sdkRequest1.setCode(36001);
		Service000A1Stub.ArrayOfstring arrayOfstring1 = new Service000A1Stub.ArrayOfstring();
		arrayOfstring1.addString(UserName);// 用户名
		arrayOfstring1.addString(PassWord);// 密码
		arrayOfstring1.addString("F");// 强制登录
		arrayOfstring1.addString(serverName1);// 客户端机器名
		arrayOfstring1.addString(remoteIp);// 客户端IP
		sdkRequest1.setListData(arrayOfstring1);
		repsssDoOperationResponse1.setWebRequest(sdkRequest1);
		Service000A1Stub.DoOperationResponse ddddDoOperationResponse1;
		try {
			ddddDoOperationResponse1 = ssssteStub1
					.DoOperation(repsssDoOperationResponse1);
			Service000A1Stub.SDKReturn sdkReturn1 = ddddDoOperationResponse1
					.getDoOperationResult();
			int code = sdkReturn1.getCode();
			if (code != 0) {
				return false;
			} else {

				Service000A1Stub.ArrayOfstring arrayOfstring3 = sdkReturn1
						.getListData();
				java.lang.String[] ArrayStr1 = arrayOfstring3.getString();
				if (ArrayStr1.length >= 7) {

					java.lang.String ReturnCode = ArrayStr1[0];
					if (ReturnCode.equals("S01A00")) {
						// 成功
						User curuserUser = new User();
						if(ArrayStr1[5].equals("S01A01")){
						curuserUser = new User(ArrayStr1[0],ArrayStr1[1],
								ArrayStr1[2],ArrayStr1[3],
								ArrayStr1[4],ArrayStr1[6],
								ArrayStr1[7],UserName);
						}
						else {
							curuserUser = new User(ArrayStr1[0],ArrayStr1[1],
									ArrayStr1[2],ArrayStr1[3],
									ArrayStr1[4],ArrayStr1[5],
									ArrayStr1[6],UserName);
						}
						HttpSession session = request.getSession();
						session.setAttribute("sCurrentUser", curuserUser);
						flag = true;

					} else {
						return false;
					}

				} else {
					return false;
				}
			}

		} catch (RemoteException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			return false;
		}
		return flag;
	}

	private void GetPermission(HttpServletRequest request, HttpServletResponse response)
	{
		HttpSession session = request.getSession();
		ArrayList<String> argDbInit = (ArrayList<String>) session.getAttribute("DBInfoInit");
		ArrayList<String> argDbForNet = StringUtil.GetNetConnectString(argDbInit);
		
		ArrayList<String> arrayListWcfinfoArrayList = new ArrayList<String>();
		arrayListWcfinfoArrayList = (ArrayList<String>) session
				.getAttribute("WcfInfo");		
		User curuserUser =(User) session.getAttribute("sCurrentUser");			
		try {
			Service31032Stub clientService31032Stub = new Service31032Stub(
					arrayListWcfinfoArrayList.get(1));
			Service31032Stub.GetUserOperation  requesConUserPermission= new Service31032Stub.GetUserOperation();
			requesConUserPermission.setDbType(argDbForNet.get(0));
			requesConUserPermission.setDbURL(argDbForNet.get(1));
			requesConUserPermission.setUserID(curuserUser.getUserID());
			Service31032Stub.Service02Return  ssssReturn= new Service31032Stub.Service02Return();
			ssssReturn= clientService31032Stub.GetUserOperation(requesConUserPermission).getGetUserOperationResult();
			String ss=ssssReturn.getReturnValueString();

			Permission permission = new Permission("0","0","0");
			
			if(ss.contains("3102001"))//Query
			{
				permission.setQuery("1");
			}
			if(ss.contains("3102002"))//play
			{
				permission.setPlay("1");
			}
			if(ss.contains("3102008"))//Down
			{
				permission.setDownload("1");
			}
			session.setAttribute("curPermission", permission);
		} catch (RemoteException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
	}
	
	public void doPost(HttpServletRequest request, HttpServletResponse response)
			throws ServletException, IOException {
		this.doGet(request, response);
	}
	public String showParaName[];
	private void SetLanguages(String type, HttpSession session,String path3) {
		LanguageSet trans = new LanguageSet(type,path3);
		showParaName = new String[50];
		showParaName = trans.LanguagePara();
		session.setAttribute("slanguage", showParaName);
	}

}
