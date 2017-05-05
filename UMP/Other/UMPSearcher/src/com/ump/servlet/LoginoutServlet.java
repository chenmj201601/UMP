package com.ump.servlet;

import java.io.IOException;
import java.io.PrintWriter;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import java.rmi.RemoteException;
import java.util.ArrayList;

import org.apache.axis2.AxisFault;
import org.jdom.*;
import org.jdom.input.*;
import com.ump.wcf.*;


public class LoginoutServlet extends HttpServlet {

	/**
	 * @category 退出登录的Servlet,注销
	 * @author Bird
	 */
	private static final long serialVersionUID = 1L;

	public void doGet(HttpServletRequest request, HttpServletResponse response)
			throws ServletException, IOException {
		HttpSession session = request.getSession(false);// 防止创建Session
		if (session == null) {
			request.getRequestDispatcher("Login.jsp")
					.forward(request, response);
			return;
		}
		ValidLoginOut(request);
		session.removeAttribute("sCurrentUser");
		session.removeAttribute("sSearch");
		session.removeAttribute("sAllAgent");
		session.removeAttribute("sAllExtension");
		session.removeAttribute("WcfInfo");
		session.removeAttribute("slanguage");
		session.removeAttribute("DBInfo");
		session.removeAttribute("DBInfoInit");
		session.removeAttribute("curPermission");
		response.sendRedirect("Login.jsp");
	}
	
	
	private  Boolean  ValidLoginOut(HttpServletRequest request) throws AxisFault{
		boolean flag = true;
		HttpSession session=request.getSession();
		String  loginSessionString= (String)session.getAttribute("LoginSessionID");
		ArrayList<String> arrayListWcfinfoArrayList = new ArrayList<String>();
		arrayListWcfinfoArrayList= (ArrayList<String>)session.getAttribute("WcfInfo"); 
		
		String userIdString=(String)session.getAttribute("UserID");
		
		
		//登出
		Service000A1Stub ssssteStub= new  Service000A1Stub(arrayListWcfinfoArrayList.get(2));		
		Service000A1Stub.DoOperation  repsssDoOperationResponse= 
				new  Service000A1Stub.DoOperation();		
		Service000A1Stub.SDKRequest  sdkRequest= new Service000A1Stub. SDKRequest();		
		sdkRequest.setCode(34011);		
		Service000A1Stub.ArrayOfstring  arrayOfstring= new Service000A1Stub.ArrayOfstring();		
		arrayOfstring.addString("00000");	//租户编号 
		arrayOfstring.addString(userIdString); //用户ID
		arrayOfstring.addString(loginSessionString); //登录分配的SessionID
		sdkRequest.setListData(arrayOfstring);   
		repsssDoOperationResponse.setWebRequest(sdkRequest);		
		Service000A1Stub.DoOperationResponse ddddDoOperationResponse;
		try {
			ddddDoOperationResponse = ssssteStub.DoOperation(repsssDoOperationResponse);
			Service000A1Stub.SDKReturn sdkReturn=ddddDoOperationResponse.getDoOperationResult();
			int code=0;
			code= sdkReturn.getCode();
			if (code==0) {
				Service000A1Stub.ArrayOfstring arrayOfstring2=sdkReturn.getListData();
				String[] ArrayStr = arrayOfstring2.getString();
				if(ArrayStr.length>2){
					if(ArrayStr[0].equals("S01AA1")){
						return flag;
					}else {
						return flag= false;
					}
					
				}else {
					return false;
				}
			}else {
				//失败了
				return false;
			}
			
		} catch (RemoteException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			return false;
		}
	}

	public void doPost(HttpServletRequest request, HttpServletResponse response)
			throws ServletException, IOException {
	}

}
