package com.ump.servlet;

import java.io.File;
import java.io.IOException;
import java.rmi.RemoteException;
import java.util.ArrayList;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.apache.axis2.AxisFault;
import org.jdom.Document;
import org.jdom.Element;
import org.jdom.input.SAXBuilder;

import net.sf.json.JSONArray;
import net.sf.json.JSONObject;

import com.ump.model.CtrolObject;
import com.ump.model.User;
import com.ump.util.ResponseUtil;
import com.ump.util.StringUtil;
import com.ump.wcf.Service31032Stub;

public class GetExtensionListServlet extends HttpServlet {

	/**
	 * Constructor of the object.
	 */
	public GetExtensionListServlet() {
		super();
	}

	public ArrayList<CtrolObject> arryListCtolOrg = new ArrayList<CtrolObject>();
	public ArrayList<CtrolObject> arryListCtrolAgent = new ArrayList<CtrolObject>();
	public ArrayList<CtrolObject> arryListCtolExtensionArrayList = new ArrayList<CtrolObject>();
	public ArrayList<CtrolObject> arryListCtolTrueExtension = new ArrayList<CtrolObject>();

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

	// 得到管理的机构
	private void GetCurrentOrg(ArrayList<String> arDBForNet,
			ArrayList<String> arrayListWcfinfoArrayList, String UserID,
			String OrgID) throws AxisFault {
		Service31032Stub clientService31032Stub = new Service31032Stub(
				arrayListWcfinfoArrayList.get(1));
		Service31032Stub.GetUserControlOrg requesControlOrg = new Service31032Stub.GetUserControlOrg();
		requesControlOrg.setDbType(arDBForNet.get(0));
		requesControlOrg.setDbURL(arDBForNet.get(1));
		requesControlOrg.setUserID(UserID);
		requesControlOrg.setParentID(OrgID);

		Service31032Stub.Service02Return ssssReturn = new Service31032Stub.Service02Return();
		try {
			ssssReturn = clientService31032Stub.GetUserControlOrg(
					requesControlOrg).getGetUserControlOrgResult();
			Service31032Stub.ArrayOfstring strArray = ssssReturn
					.getReturnValueListString();
			String[] ArrayStr = strArray.getString();
			if (ArrayStr != null) {
				for (int i = 0; i < ArrayStr.length; i++) {
					CtrolObject orgObject = new CtrolObject();
					String[] arrayTempStrings = ArrayStr[i].split("\\$");
					orgObject.setId(arrayTempStrings[0]);
					orgObject.setText(arrayTempStrings[1]);
					orgObject.setmOrgID(arrayTempStrings[2]);
					arryListCtolOrg.add(orgObject);

					// 继续往下遍历机构
					GetCurrentOrg(arDBForNet, arrayListWcfinfoArrayList,
							UserID, arrayTempStrings[0]);

					// 查询机构下分机
					if (arrayListWcfinfoArrayList.get(0).equals("E"))// 分机
					{
						GetControlExtenstion(arDBForNet,
								arrayListWcfinfoArrayList, UserID, arrayTempStrings[0]);
					} else if (arrayListWcfinfoArrayList.get(0).equals("T"))// 真实分机
					{
						GetControlTrueExtension(arDBForNet,
								arrayListWcfinfoArrayList, UserID, arrayTempStrings[0]);
					}
				}
			}

		} catch (RemoteException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	// 得到管理的分机 当配置参数为E时
	private void GetControlExtenstion(ArrayList<String> arDBForNet,
			ArrayList<String> arrayListWcfinfoArrayList, String UserID,
			String OrgID) throws AxisFault {
		Service31032Stub clientService31032Stub = new Service31032Stub(
				arrayListWcfinfoArrayList.get(1));
		Service31032Stub.GetUserControlAgentOrExtension requesControlAgentOrExtension = new Service31032Stub.GetUserControlAgentOrExtension();
		requesControlAgentOrExtension.setDbType(arDBForNet.get(0));
		requesControlAgentOrExtension.setDbURL(arDBForNet.get(1));
		requesControlAgentOrExtension.setUserID(UserID);
		requesControlAgentOrExtension.setParentID(OrgID);
		requesControlAgentOrExtension.setObjectType("E");
		Service31032Stub.Service02Return ssssReturn = new Service31032Stub.Service02Return();

		try {
			ssssReturn = clientService31032Stub.GetUserControlAgentOrExtension(
					requesControlAgentOrExtension)
					.getGetUserControlAgentOrExtensionResult();
		} catch (RemoteException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		Service31032Stub.ArrayOfstring strArray = ssssReturn
				.getReturnValueListString();
		String[] ArrayStr = strArray.getString();
		if (ArrayStr != null) {
			for (int i = 0; i < ArrayStr.length; i++) {
				CtrolObject extensionObject = new CtrolObject();
				String[] arrayTempStrings = ArrayStr[i].split("\\$");
				if (arrayTempStrings.length > 2) {
					extensionObject.setId(arrayTempStrings[0]);
					extensionObject.setText(arrayTempStrings[1]);
					extensionObject.setmFullName(arrayTempStrings[2]);
					extensionObject.setmOrgID(OrgID);
					arryListCtolExtensionArrayList.add(extensionObject);
				}

			}
		}
	}

	// 得到管理的 真实分机 当配置参数R时
	private void GetControlTrueExtension(ArrayList<String> arDBForNet,
			ArrayList<String> arrayListWcfinfoArrayList, String UserID,
			String OrgID) throws AxisFault {
		Service31032Stub clientService31032Stub = new Service31032Stub(
				arrayListWcfinfoArrayList.get(1));
		Service31032Stub.GetUserControlAgentOrExtension requesControlAgentOrExtension = new Service31032Stub.GetUserControlAgentOrExtension();
		requesControlAgentOrExtension.setDbType(arDBForNet.get(0));
		requesControlAgentOrExtension.setDbURL(arDBForNet.get(1));
		requesControlAgentOrExtension.setUserID(UserID);
		requesControlAgentOrExtension.setParentID(OrgID);
		requesControlAgentOrExtension.setObjectType("T");
		Service31032Stub.Service02Return ssssReturn = new Service31032Stub.Service02Return();

		try {
			ssssReturn = clientService31032Stub.GetUserControlAgentOrExtension(
					requesControlAgentOrExtension)
					.getGetUserControlAgentOrExtensionResult();
		} catch (RemoteException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		Service31032Stub.ArrayOfstring strArray = ssssReturn
				.getReturnValueListString();
		String[] ArrayStr = strArray.getString();
		if (ArrayStr != null) {
			for (int i = 0; i < ArrayStr.length; i++) {
				CtrolObject trueExtensionObject = new CtrolObject();
				String[] arrayTempStrings = ArrayStr[i].split("\\$");
				if (arrayTempStrings.length > 2) {
					trueExtensionObject.setId(arrayTempStrings[0]);
					trueExtensionObject.setText(arrayTempStrings[1]);
					trueExtensionObject.setText(arrayTempStrings[2]);
					trueExtensionObject.setmOrgID(OrgID);
					arryListCtolTrueExtension.add(trueExtensionObject);
				}
			}
		}
	}

	@Override
	protected void doGet(HttpServletRequest request,
			HttpServletResponse response) throws ServletException, IOException {
		this.doPost(request, response);
	}

	@Override
	protected void doPost(HttpServletRequest request,
			HttpServletResponse response) throws ServletException, IOException {
		HttpSession session = request.getSession();
		arryListCtolOrg.clear();
		arryListCtolExtensionArrayList.clear();

		ArrayList<String> arDB = (ArrayList<String>) session
				.getAttribute("DBInfo");
		
		User curuserUser = (User)session.getAttribute("sCurrentUser");
		if(curuserUser==null)
			return;

		// ++++++++++++++++++++++++++++++++++++++++
		String path3 = request.getSession().getServletContext().getRealPath("")
				+ "\\WEB-INF\\classes\\com\\ump\\servlet";
		ArrayList<String> arrayListWcfinfoArrayList = new ArrayList<String>();
		arrayListWcfinfoArrayList = GetWcfInfo(path3);
		session.setAttribute("WcfInfo", arrayListWcfinfoArrayList);

		ArrayList<String> argDbInit = (ArrayList<String>) session
				.getAttribute("DBInfoInit");
		ArrayList<String> argDbForNet = StringUtil
				.GetNetConnectString(argDbInit);

		GetCurrentOrg(argDbForNet, arrayListWcfinfoArrayList,
				curuserUser.getUserID(), "-1");

		session.setAttribute("sAllExtension", arryListCtolExtensionArrayList);
		arryListCtolExtensionArrayList.addAll(arryListCtolOrg);

		if (arrayListWcfinfoArrayList.get(0).equals("E"))// 分机
		{

		} else if (arrayListWcfinfoArrayList.get(0).equals("T"))// 真实分机
		{

		}
		// ++++++++++++++++++++++++++++++++++++++++
		String strjson = JSONArray.fromObject(arryListCtolExtensionArrayList)
				.toString();

		JSONObject result = new JSONObject();
		result.put("strnode", strjson);
		try {
			ResponseUtil.write(response, result);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

}
