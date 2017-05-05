package com.ump.servlet;

import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.PrintWriter;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLConnection;
import java.rmi.RemoteException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.apache.axis2.AxisFault;

import com.ump.model.Permission;
import com.ump.util.DownloadUtil;
import com.ump.util.StringUtil;
import com.ump.wcf.Service000A1Stub;

public class RecDownloadServlet extends HttpServlet {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	/**
	 * Constructor of the object.
	 */
	public RecDownloadServlet() {
		super();
	}
	
	@Override
	protected void doGet(HttpServletRequest request,
			HttpServletResponse response) throws ServletException, IOException {
		response.setContentType("text/html; charset=utf-8");
		request.setCharacterEncoding("utf-8");
		response.setCharacterEncoding("utf-8");
		PrintWriter outmsg = response.getWriter();
		String strared = request.getParameter("ared");//AREDZB
		String ftype = request.getParameter("ftype");
		if (ftype.equals("0"))
			ftype = ".mp3";
		else
			ftype = ".wav";

		String serialids = request.getParameter("serialids");
		String refs = request.getParameter("refs");
		String agents = request.getParameter("agents");
		String extens = request.getParameter("extens");
		String dtstarts = request.getParameter("dtstarts");
		String calls = request.getParameter("calls");
		String calleds = request.getParameter("calleds");

		HttpSession session = request.getSession();
		Permission permission = StringUtil.getPermission(session);
		if(permission.getDownload().equals("0"))
		{
			return;
		}
		String showParaName[] = new String[50];
		if (session.getAttribute("slanguage") != null) {
			showParaName = (String[]) session.getAttribute("slanguage");
		}

		if (StringUtil.isEmpty(serialids) || StringUtil.isEmpty(refs)) {
			outmsg.write(showParaName[28]);
			return;
		}

		String[] strarrsid = serialids.split(",");
		String[] strarrref = refs.split(",");
		String[] strarragent = agents.split(",");
		String[] strarrexten = extens.split(",");
		String[] strarrstars = dtstarts.split(",");
		String[] strarrcall = calls.split(",");
		String[] strarrcalled = calleds.split(",");

		String msgsuccess = "";
		String msgfield = "";
		String getfName = "";
		if (!strarrsid.equals("") && strarrsid.length > 0) {
			for (int i = 0; i < strarrsid.length; i++) {
				if (strarrsid[i].equals(""))
					continue;
				String murlString = getRecordURL(request, response,
						strarrsid[i], false, "");
				if (ftype.equals(".wav"))
					murlString = murlString.substring(4,
							murlString.length() - 4);

				getfName = GetFileName(strarragent, strarrexten, strarrref,
						strarrstars, strared, i,strarrcall,strarrcalled);
				ArrayList<String> arrayListWcfinfoArrayList = new ArrayList<String>();
				arrayListWcfinfoArrayList = (ArrayList<String>) session
						.getAttribute("WcfInfo");
				if (murlString != null && !murlString.equals("")) {

					int num = arrayListWcfinfoArrayList.get(1).indexOf(
							"Wcf2Client");
					String mp3pathString = arrayListWcfinfoArrayList.get(1)
							.substring(0, num);
					String fileName = mp3pathString + "MediaData/" + murlString;//UMP中的MP3文件
					
					String propath=this.getServletConfig().getServletContext().getRealPath("/") + "temp\\";
					String localpath="";
					
					if(DownloadUtil.downFiletoLocal(fileName, propath, getfName + ftype,localpath, request,response))
					{
						msgsuccess += getfName + ftype + ",";
					}
					else {
						msgfield += getfName + ftype + ",";
					}
				}
			}
			if (!msgfield.equals("")) {
				outmsg.write("umpf" + msgfield + showParaName[26]);
			} else if (!msgsuccess.equals("")) {
				outmsg.write("umps" + msgsuccess.substring(0, msgsuccess.length()-1));
			}
		}
	}

	public static String GetFileName(String agent[], String exten[],
			String ref[], String date[], String save28, int index,
			String call[],String called[]) {
		String nameString = "";
		for (int i = 0; i < save28.length(); i++) {
			char item = save28.charAt(i);
			switch (item) {
			case 'A':
				if(agent[index].equals("N/A"))
					nameString += "NA";
				else
					nameString += agent[index];
				break;
			case 'E':
				nameString += exten[index];
				break;
			case 'R':
				nameString += ref[index];
				break;
			case 'D':
				SimpleDateFormat sdf = new SimpleDateFormat(
						"yyyy-MM-dd HH:mm:ss");
				Date tempdate = new Date();
				try {
					tempdate = sdf.parse(date[index]);
				} catch (ParseException e) {
					e.printStackTrace();
				}
				SimpleDateFormat sdf1 = new SimpleDateFormat("yyyyMMddHHmmss");
				String str = sdf1.format(tempdate);
				nameString += str;
				break;
			case 'Z':
				nameString += call[index];
				break;
			case 'B':
				nameString += called[index];
				break;
			default:
				break;
			}
		}
		if(nameString.equals(""))
			nameString += ref[index];
		return nameString;
	}

	public static boolean httpDownload(String httpUrl, String saveFile) {
		// 下载网络文件
		int bytesum = 0;
		int byteread = 0;

		URL url = null;
		try {
			url = new URL(httpUrl);
		} catch (MalformedURLException e1) {
			// TODO Auto-generated catch block
			e1.printStackTrace();
			return false;
		}

		try {
			URLConnection conn = url.openConnection();
			InputStream inStream = conn.getInputStream();
			FileOutputStream fs = new FileOutputStream(saveFile);

			byte[] buffer = new byte[1204];
			while ((byteread = inStream.read(buffer)) != -1) {
				bytesum += byteread;
				fs.write(buffer, 0, byteread);
			}
			fs.close();
			return true;
		} catch (FileNotFoundException e) {
			e.printStackTrace();
			return false;
		} catch (IOException e) {
			e.printStackTrace();
			return false;
		}
	}

	private String getRecordURL(HttpServletRequest request,
			HttpServletResponse response, String RecordReference,
			Boolean IsRequireDecry, String DecryptPassWord) throws AxisFault {
		String StrURL = "";
		HttpSession session = request.getSession();
		ArrayList<String> arrayListWcfinfoArrayList = new ArrayList<String>();
		arrayListWcfinfoArrayList = (ArrayList<String>) session
				.getAttribute("WcfInfo");

		Service000A1Stub ssssteStub = new Service000A1Stub(
				arrayListWcfinfoArrayList.get(2));
		Service000A1Stub.DoOperation repsssDoOperationResponse = new Service000A1Stub.DoOperation();
		Service000A1Stub.SDKRequest sdkRequest = new Service000A1Stub.SDKRequest();

		sdkRequest.setCode(34011);
		Service000A1Stub.ArrayOfstring arrayOfstring = new Service000A1Stub.ArrayOfstring();

		arrayOfstring.addString("1020000000000000002"); // 用户编号（备用，可以指定0）1020000000000000002为sftp下载的内置账号不能修改
		arrayOfstring.addString("0"); // Method（方式）
		arrayOfstring.addString(RecordReference); // SerialID（流水号或录音信息Json）
		if (IsRequireDecry) {
			arrayOfstring.addString(DecryptPassWord); // DecryptPassword（解密密码，M004）
		} else {
			arrayOfstring.addString(""); // DecryptPassword（解密密码，M004）
		}

		arrayOfstring.addString("Upload.User.123"); // LoginPassword（登录密码，M004）
		arrayOfstring.addString("{\"ConvertWaveFormat\":\"3\"}"); // /Option（其他选项）Json格式，每个选项都有特定的键名，参考S000AConsts中的定义

		sdkRequest.setListData(arrayOfstring);
		repsssDoOperationResponse.setWebRequest(sdkRequest);
		Service000A1Stub.DoOperationResponse ddddDoOperationResponse;
		try {
			ddddDoOperationResponse = ssssteStub
					.DoOperation(repsssDoOperationResponse);
			Service000A1Stub.SDKReturn sdkReturn = ddddDoOperationResponse
					.getDoOperationResult();
			int code = 0;
			code = sdkReturn.getCode();
			if (code == 0) {
				Service000A1Stub.ArrayOfstring arrayOfstring2 = sdkReturn
						.getListData();
				String[] ArrayStr = arrayOfstring2.getString();
				StrURL = ArrayStr[0];
			} else {
				// 失败了
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
