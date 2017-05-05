package com.ump.util;

import java.io.IOException;
import java.io.PrintWriter;
import java.text.MessageFormat;
import java.util.ArrayList;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;
import com.ump.model.Permission;

public class StringUtil {
	public static boolean isEmpty(String str) {
		if ("".equals(str) || str == null) {
			return true;
		} else {
			return false;
		}
	}

	public static boolean isNotEmpty(String str) {
		if (!"".equals(str) && str != null) {
			return true;
		} else {
			return false;
		}
	}

	public static String getStrValue(Object o) {
		if (o == null)
			return "";
		else
			return o.toString();
	}

	// /0 数据库类型
	// /1端口
	// /2数据库名或服务名
	// /3数据库账号
	// /4密友
	public static ArrayList<String> getJavaConnectStrings(
			ArrayList<String> initArrayListString) {
		ArrayList<String> arrayList = new ArrayList<String>();
		if (initArrayListString.size() < 5) {
			return arrayList;
		}
		String dbtypeString = initArrayListString.get(0);
		String uRlString = null;
		if (dbtypeString.equals("2")) // sqlserver
		{
			uRlString = MessageFormat.format(
					"jdbc:sqlserver://{0}:{1};DatabaseName={2}",
					initArrayListString.get(1), initArrayListString.get(2),
					initArrayListString.get(3));
		} else if (dbtypeString.equals("3"))// oracle
		{
			uRlString = MessageFormat.format("jdbc:oracle:thin:@{0}:{1}:{2}",
					initArrayListString.get(1), initArrayListString.get(2),
					initArrayListString.get(3));
		} else {
			return arrayList;
		}
		arrayList.add(dbtypeString);
		arrayList.add(uRlString);
		arrayList.add(initArrayListString.get(4));
		arrayList.add(initArrayListString.get(5));
		return arrayList;
	}

	// /0 数据库类型
	// /1端口
	// /2数据库名或服务名
	// /3数据库账号
	// /4密友
	public static ArrayList<String> GetNetConnectString(
			ArrayList<String> initArrayListString) {
		ArrayList<String> arrayList = new ArrayList<String>();
		if (initArrayListString.size() < 5) {
			return arrayList;
		}

		String dbtypeString = initArrayListString.get(0);
		String uRlString = null;
		if (dbtypeString.equals("2")) // sqlserver
		{
			uRlString = MessageFormat
					.format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}",
							initArrayListString.get(1),
							initArrayListString.get(2),
							initArrayListString.get(3),
							initArrayListString.get(4),
							initArrayListString.get(5));
		} else if (dbtypeString.equals("3"))// oracle
		{
			uRlString = MessageFormat
					.format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3}; Password={4}",
							initArrayListString.get(1),
							initArrayListString.get(2),
							initArrayListString.get(3),
							initArrayListString.get(4),
							initArrayListString.get(5));
		} else {
			return arrayList;
		}
		arrayList.add(dbtypeString);
		arrayList.add(uRlString);

		return arrayList;
	}

	// //数据库字符串拼接
	// public string GetConnectionString(string dbType, string dbURL, string
	// dbUserName, string dbPWD)
	// {
	// string str = string.Empty;
	// switch (dbType)
	// {
	// case "2":
	// return
	// string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}",
	// new object[] { this.Host, this.Port, this.DBName, this.LoginName,
	// this.Password });

	// case "3":
	// return
	// string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3}; Password={4}",
	// new object[] { this.Host, this.Port, this.DBName, this.LoginName,
	// this.Password });
	// }
	// return str;
	// }
	// P01="jdbc:sqlserver://192.168.4.182:1433;DatabaseName=UMPDataDB" P02="sa"
	// P03="PF,123"
	// P01="jdbc:oracle:thin:@192.168.4.182:1521:PFOrcl" P02="PFDEV831"
	// P03="pfdev831"

	public static String secToTime(int time) {
		String timeStr = null;
		int hour = 0;
		int minute = 0;
		int second = 0;
		if (time <= 0)
			return "00:00";
		else {
			minute = time / 60;
			if (minute < 60) {
				second = time % 60;
				timeStr = unitFormat(minute) + ":" + unitFormat(second);
			} else {
				hour = minute / 60;
				if (hour > 99)
					return "99:59:59";
				minute = minute % 60;
				second = time - hour * 3600 - minute * 60;
				timeStr = unitFormat(hour) + ":" + unitFormat(minute) + ":"
						+ unitFormat(second);
			}
		}
		return timeStr;
	}

	public static String unitFormat(int i) {
		String retStr = null;
		if (i >= 0 && i < 10)
			retStr = "0" + Integer.toString(i);
		else
			retStr = "" + i;
		return retStr;
	}

	public static String time2sec(String time) {
		if (isNotEmpty(time)) {
			String[] my = time.split(":");
			int hour = Integer.parseInt(my[0]);
			int min = Integer.parseInt(my[1]);
			int sec = Integer.parseInt(my[2]);
			int total = hour * 3600 + min * 60 + sec;
			return "" + total;
		} else {
			return "0";
		}
	}
	
	public static boolean isboolIp(String ipAddress)  
	{
		   String rexp = "([1-9]|[1-9]\\d|1\\d{2}|2[0-4]\\d|25[0-5])(\\.(\\d|[1-9]\\d|1\\d{2}|2[0-4]\\d|25[0-5])){3}";  
	       Pattern pattern = Pattern.compile(rexp);   
	       Matcher matcher = pattern.matcher(ipAddress);   
	       return matcher.matches();   
	}
	
	public static void ThrowInfoMsg(HttpServletResponse response,String msg) throws IOException {
		PrintWriter out = response.getWriter();
		out.print("<script language='javascript'>alert('" + msg + "')</script>");
	}
	
	public static Permission getPermission(HttpSession session)
	{
		Permission p = new Permission("0", "0", "0");
		if (session.getAttribute("curPermission") != null ) {
			p = (Permission) session.getAttribute("curPermission");
		}
		return p;
	}
}
