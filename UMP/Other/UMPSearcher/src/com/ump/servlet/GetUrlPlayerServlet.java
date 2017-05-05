package com.ump.servlet;

import java.io.IOException;
import java.io.PrintWriter;
import java.rmi.RemoteException;
import java.util.ArrayList;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;
import com.ump.model.Permission;
import com.ump.util.StringUtil;

import org.apache.axis2.AxisFault;
import com.ump.wcf.*;

public class GetUrlPlayerServlet extends HttpServlet {

	public GetUrlPlayerServlet() {
		super();
	}

	@SuppressWarnings("unchecked")
	@Override
	protected void doGet(HttpServletRequest request, 
			HttpServletResponse response) throws ServletException, IOException {
		response.setContentType("text/html; charset=utf-8");
		request.setCharacterEncoding("utf-8");
		response.setCharacterEncoding("utf-8");
		PrintWriter out = response.getWriter();
		String serialid = request.getParameter("serialid");
		String isencryt = request.getParameter("isencry");
		String umppwd = request.getParameter("umppwd");
		
		String filePath=this.getServletConfig().getServletContext().getRealPath("/");
		
		String murlString="";
		if(isencryt.equals("1"))
			murlString = getRecordURL(request,response,serialid,true,umppwd);
		else
			murlString = getRecordURL(request,response,serialid,false,"");
		HttpSession session=request.getSession();
		
		Permission permission = StringUtil.getPermission(session);
		if(permission.getPlay().equals("0"))
		{
			return;
		}
		
		String showParaName[] = new String[50];
		if (session.getAttribute("slanguage") != null) {
			showParaName = (String[]) session.getAttribute("slanguage");
		}
		
		ArrayList<String> arrayListWcfinfoArrayList = new ArrayList<String>();
		arrayListWcfinfoArrayList= (ArrayList<String>)session.getAttribute("WcfInfo");  
		if(murlString!=null && !murlString.equals("")){
			int num = arrayListWcfinfoArrayList.get(1).indexOf("Wcf2Client");
			String mp3pathString=arrayListWcfinfoArrayList.get(1).substring(0, num);
			String allpathString = mp3pathString +"MediaData//"+ murlString;
			out.write(allpathString);
		}
		else {
			out.write(showParaName[40]);
		}
	}
	
	private  String getRecordURL(HttpServletRequest request,HttpServletResponse response,String  RecordReference 
			,Boolean IsRequireDecry ,String DecryptPassWord) throws AxisFault {
		String StrURL= "";
		HttpSession session=request.getSession();
		ArrayList<String> arrayListWcfinfoArrayList = new ArrayList<String>();
		arrayListWcfinfoArrayList= (ArrayList<String>)session.getAttribute("WcfInfo"); 
		
		Service000A1Stub ssssteStub= new  Service000A1Stub(arrayListWcfinfoArrayList.get(2));		
		Service000A1Stub.DoOperation  repsssDoOperationResponse= 
				new  Service000A1Stub.DoOperation();		
		Service000A1Stub.SDKRequest  sdkRequest= new Service000A1Stub.SDKRequest();
		
		sdkRequest.setCode(34011);		
		Service000A1Stub.ArrayOfstring  arrayOfstring= new Service000A1Stub.ArrayOfstring();	
		
		arrayOfstring.addString("1020000000000000002");	//用户编号（备用，可以指定0）1020000000000000002为sftp下载的内置账号不能修改
		arrayOfstring.addString("0"); //Method（方式）
		arrayOfstring.addString(RecordReference); //SerialID（流水号或录音信息Json）
		if (IsRequireDecry) {
			arrayOfstring.addString(DecryptPassWord); //DecryptPassword（解密密码，M004）
		}else {
			arrayOfstring.addString(""); //DecryptPassword（解密密码，M004）
		}
		
		arrayOfstring.addString("Upload.User.123"); //LoginPassword（登录密码，M004）
		arrayOfstring.addString("{\"ConvertWaveFormat\":\"3\"}"); ///Option（其他选项）Json格式，每个选项都有特定的键名，参考S000AConsts中的定义	
		
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
				StrURL= ArrayStr[0];
			}else {
				//失败了
			}	
		} catch (RemoteException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		return StrURL;		
	}

	@Override
	protected void doPost(HttpServletRequest request,
			HttpServletResponse response) throws ServletException, IOException {
		doGet(request, response);
	}
}
