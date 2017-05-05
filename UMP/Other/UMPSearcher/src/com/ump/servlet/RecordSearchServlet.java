package com.ump.servlet;
import java.io.IOException;
import java.text.MessageFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;
import java.util.Map;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;
import net.sf.json.JSONArray;
import net.sf.json.JSONObject;
import com.ump.database.DBOperator;
import com.ump.model.*;
import com.ump.submeter.service.SubmeterService;
import com.ump.submeter.service.impl.SubmeterServiceImpl;
import com.ump.util.ResponseUtil;
import com.ump.util.StringUtil;

@SuppressWarnings("serial")
public class RecordSearchServlet extends HttpServlet {

	@Override
	protected void doGet(HttpServletRequest request,
			HttpServletResponse response) throws ServletException, IOException {
		HttpSession session = request.getSession();
		Permission permission = StringUtil.getPermission(session);
		if (permission.getQuery().equals("0")) {
			return;
		}
		// request.getParameter("txtextension");
		// 时间范围
		String dtstart = request.getParameter("dtstart"); // c004 选择的录音开始时间
		String dtend = request.getParameter("dtend"); // c004
		String callno = request.getParameter("callno"); // c040 主叫号码
		String calledno = request.getParameter("calledno");// c041  被叫号码
		String direction = request.getParameter("direction");// c045 呼叫方向

		// 时长
		String lenstart = request.getParameter("lenstart");// C012
		String lenend = request.getParameter("lenend");
		// 流水号
		String reference = request.getParameter("reference");// c077
		// 最近
		String ckcur = request.getParameter("ckcur");//
		String curnum = request.getParameter("curnum");//
		String curtype = request.getParameter("curtype");//
		if (StringUtil.isNotEmpty(ckcur) && ckcur.equals("true")
				&& StringUtil.isNotEmpty(curnum)
				&& StringUtil.isNotEmpty(curtype)) {
			Date nowDate = new Date();
			int tempNum = Integer.parseInt(curnum);
			SimpleDateFormat formatter = new SimpleDateFormat(
					"yyyy-MM-dd HH:mm:ss");
			dtend = formatter.format(nowDate);
			if (curtype.equals("0")) {
				Calendar cal = Calendar.getInstance();
				cal.setTime(nowDate);
				cal.add(Calendar.DATE, -tempNum);
				dtstart = formatter.format(cal.getTime());
			} else if (curtype.equals("1")) {
				Calendar cal = Calendar.getInstance();
				cal.setTime(nowDate);
				cal.add(Calendar.DATE, -7 * tempNum);
				dtstart = formatter.format(cal.getTime());
			} else if (curtype.equals("2")) {
				Calendar cal = Calendar.getInstance();
				cal.setTime(nowDate);
				cal.add(Calendar.MONTH, -tempNum);
				dtstart = formatter.format(cal.getTime());
			} else if (curtype.equals("3")) {
				Calendar cal = Calendar.getInstance();
				cal.setTime(nowDate);
				cal.add(Calendar.YEAR, -tempNum);
				dtstart = formatter.format(cal.getTime());
			}
		}

		String agent = request.getParameter("agent");// c039 A8021,a8001,112e1
		if (agent != null && !agent.equals("") && !agent.equals("''"))
			agent = "'" + agent.replace(",", "','") + "'";
		else
			agent = null;
		String exten = request.getParameter("extension");// c042 8021,8022,112e1
		if (exten != null && !exten.equals("") && !exten.equals("''"))
			exten = "'" + exten.replace(",", "','") + "'";
		else
			exten = null;

		String page = request.getParameter("page");
		String rows = request.getParameter("rows");

		if (dtstart == null || dtend == null)// 传入新的时间为NULL 可能第一次进入，可能在翻页操作等
		{
			SearchParams sp = (SearchParams) session.getAttribute("sSearch");
			if (sp != null && sp.getDtstart() != null && sp.getDtend() != null)// 查询条件session存在，翻页等操作
			{
				try {
					QueryData(page, rows, response, sp, session);
				} catch (ParseException e) {

					e.printStackTrace();
				}
			} else{// 第一次进入 无数据
			
				NoData(request, response);
			}
		} else{// 传入的时间不为空
		
			SearchParams sp = new SearchParams(dtstart, dtend, callno,
					calledno, direction, agent, exten, "", "", reference,
					lenstart, lenend);
			session.setAttribute("sSearch", sp);

			try {
				QueryData(page, rows, response, sp, session);
			} catch (ParseException e) {
				e.printStackTrace();
			}
		}
	}

	@Override
	protected void doPost(HttpServletRequest request,
			HttpServletResponse response) throws ServletException, IOException {
		doGet(request, response);
	}

	@SuppressWarnings({ "unchecked", "rawtypes", "unused" })
	private void QueryData (String page, String rows,
			HttpServletResponse response, SearchParams sp, HttpSession session)
			throws ParseException {

		String dtstart = sp.getDtstart();
		String dtend = sp.getDtend();
		String callno = sp.getCallno();
		String calledno = sp.getCalledno();
		String direction = sp.getDirection();
		String stragent = sp.getAgent();
		String strexten = sp.getExtension();

		String ref = sp.getRecordreference();
		String lenstart = sp.getLenstart();
		String lenend = sp.getLenend();
		if (page == null || page.equals("")){
			page = "1";
		}
		if (rows == null || rows.equals("")){
			rows = "50";
		}
		List<String> arDB = (ArrayList<String>) session.getAttribute("DBInfo");
		if (arDB.size() == 4)// 0,数据库类型 1，数据库URL 2，User 3，PWD
		{
			String showParaName[] = new String[50];
			if (session.getAttribute("slanguage") != null) {
				showParaName = (String[]) session.getAttribute("slanguage");
			}
			
			/**
			 * 是否分表 number="1" 分表，number= "" 不分表
			 */
			String c000 = "00000";
			String c003 = "LP";
			SubmeterService biz = new SubmeterServiceImpl();
			int number = biz.showc004(c000, c003);
			//System.out.println(number);

			String startDate = dtstart;
			String endDate = dtend;
			SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM");
			Calendar min = Calendar.getInstance();
			Calendar max = Calendar.getInstance();
			min.setTime(sdf.parse(startDate));
			min.set(min.get(Calendar.YEAR), min.get(Calendar.MONTH), 1);
			max.setTime(sdf.parse(endDate));
			max.set(max.get(Calendar.YEAR), max.get(Calendar.MONTH), 2);
			int months = (max.get(Calendar.YEAR) - min.get(Calendar.YEAR)) * 12+ max.get(Calendar.MONTH) - min.get(Calendar.MONTH) + 1;
			//System.out.println("相差月份："+months);
			String tableName = "T_21_001_00000_";
			String fullTableName = null;
			// 得到录音开始的表名
			GetBetweenMouth gbm = new GetBetweenMouth();
			List<String> monthList = gbm.getMonthBetween(startDate, endDate);
			String sql = "";
			//得 出选择时间内的所有数据 作为一张
			for (String string : monthList) {
				fullTableName = tableName + string.replace("-", "").substring(2);
				 String sqlAll = "select * from " + fullTableName +" union all ";
				 sql +=sqlAll;
			}
			sql = sql.substring(0, sql.length()-10);
			List<RecordData> rdlist = new ArrayList<RecordData>();
			//全局变量
			int sumtotal = 0;
			int total = 0;
			String strSql = null;
			List lst = null ;
			List list = new ArrayList();
				if (number<=0) {
					if (arDB.get(0).equals("2")) {// sqlserver
						DBOperator dboperator = new DBOperator(arDB.get(0),arDB.get(1), arDB.get(2), arDB.get(3));
						String strSelect = "SELECT * FROM T_21_001_00000 w1 WHERE C001 in (SELECT top "+ rows+ " C001 FROM (SELECT top "+ Integer.parseInt(page)* Integer.parseInt(rows)
								+ " C001, C004 FROM T_21_001_00000 {0} ORDER BY C004 DESC, C001 DESC)w ORDER BY w.C004 ASC, w.C001 ASC) ORDER BY w1.C004 DESC, w1.C001 DESC";
						String strWhere = "";
						strWhere = "WHERE C004>'" + dtstart + "' AND C004<='"+ dtend + "'";
						strWhere += " AND C012>="+ StringUtil.time2sec(lenstart) + " AND C012<"+ StringUtil.time2sec(lenend);
						// AND C040='' AND C041='' AND C045=''";
						if (callno != null && !callno.equals(""))
							strWhere += " AND C040='" + callno + "'";
						if (calledno != null && !calledno.equals(""))
							strWhere += " AND C041='" + calledno + "'";
						if (!direction.equals("2"))
							strWhere += " AND C045='" + direction + "'";
						if (StringUtil.isNotEmpty(ref))
							strWhere += " AND C077='" + ref + "'";
						if (stragent != null && !stragent.equals("")) {
							strWhere += " AND C039 in(" + stragent + ")";
						} else {
							String agentString = "";
							ArrayList<CtrolObject> arryListCtrolAgent = (ArrayList<CtrolObject>) session.getAttribute("sAllAgent");
							if (arryListCtrolAgent != null) {
								for (CtrolObject ctrolObject : arryListCtrolAgent) {
									if (Long.parseLong(ctrolObject.getId()) > 1020000000000000000L)
										agentString += ctrolObject.getText()+ "','";
								}
								if (agentString.length() > 3)
									agentString = agentString.substring(0,agentString.length() - 3);
								strWhere += " AND C039 in('" + agentString + "')";
							}
						}
						if (strexten != null && !strexten.equals("")) {
							strWhere += " AND C042 in(" + strexten + ")";
						} else {
							String extenString = "";
							ArrayList<CtrolObject> arryListCtrolExt = (ArrayList<CtrolObject>) session.getAttribute("sAllExtension");
							if (arryListCtrolExt != null) {
								for (CtrolObject ctrolObject : arryListCtrolExt) {
									if (Long.parseLong(ctrolObject.getId()) > 1020000000000000000L)
										extenString += ctrolObject.getText()+ "','";
								}
								if (extenString.length() > 3)
									extenString = extenString.substring(0,extenString.length() - 3);
								strWhere += " AND C042 in('" + extenString + "')";
							}
						}

						if (!strWhere.equals(""))
							total = dboperator
									.GetDataCount("SELECT COUNT(1) CNUM FROM T_21_001_00000 "+ strWhere);
						if (Integer.parseInt(page) * Integer.parseInt(rows) > total)
							strSelect = "SELECT * FROM T_21_001_00000 w1 WHERE C001 in (SELECT top "
									+ (total - (Integer.parseInt(page) - 1)* Integer.parseInt(rows))
									+ " C001 FROM (SELECT top "+ Integer.parseInt(page)* Integer.parseInt(rows)
									+ " C001, C004 FROM T_21_001_00000 {0} ORDER BY C004 DESC, C001 DESC)w ORDER BY w.C004 ASC, w.C001 ASC) ORDER BY w1.C004 DESC, w1.C001 DESC";
						strSql = MessageFormat.format(strSelect, strWhere);
						
						lst = dboperator.excuteResultSet(strSql);
						
						for (int i = 0; i < lst.size(); i++) {
							Map current = ((Map) lst.get(i));
							String serialid = StringUtil.getStrValue(current.get("C002"));
							String startrecordtime = StringUtil.getStrValue(current.get("C004"));
							String voiceid = StringUtil.getStrValue(current.get("C037"));// current.get("C037").toString();
							String voiceip = StringUtil.getStrValue(current.get("C020"));// current.get("C020").toString();
							String chanel = StringUtil.getStrValue(current.get("C038"));// current.get("C038").toString();
							String extension = StringUtil.getStrValue(current.get("C042"));// current.get("C042").toString();
							String agent = StringUtil.getStrValue(current.get("C039"));// current.get("C039").toString();
							int templen = Integer.parseInt(StringUtil.getStrValue(current.get("C012")));
							String recordlength = StringUtil.secToTime(templen);
							String lanDir = StringUtil.getStrValue(current.get("C045"));
							String dir = lanDir.equals("1") ? showParaName[18]: showParaName[19];
							String cno = StringUtil.getStrValue(current.get("C040"));
							String cedno = StringUtil.getStrValue(current.get("C041"));
							String recordreference = StringUtil.getStrValue(current.get("C077"));// current.get("C077").toString();
							String skillgroup = "";
							String realextension = StringUtil.getStrValue(current.get("C058"));// current.get("C058").toString();
							String thirdno = "";
							String stoprecordtime = StringUtil.getStrValue(current.get("C008"));// current.get("C008").toString();
							String islabel = "";
							String isbookmark = "";
							String strc025 = StringUtil.getStrValue(current.get("C025"));
							String isscored = strc025.equals("0") ? showParaName[32]: showParaName[31];
							String isencrypt = strc025;
							String chanelname = StringUtil.getStrValue(current.get("C046"));// current.get("C046").toString();
							rdlist.add(new RecordData(serialid,startrecordtime, voiceid, voiceip, chanel,
									extension, agent, recordlength, dir, cno,cedno, recordreference, isscored,
									skillgroup, realextension, thirdno,stoprecordtime, islabel, isbookmark,
									chanelname, isencrypt));
						}
						String strjson = JSONArray.fromObject(rdlist).toString();

						JSONObject result = new JSONObject();
						JSONArray jsonArray = new JSONArray();
						jsonArray = JSONArray.fromObject(rdlist);
						result.put("rows", strjson);
						result.put("total", total);
						try {
							ResponseUtil.write(response, result);
						} catch (Exception e) {
							e.printStackTrace();
						}
					} else if (arDB.get(0).equals("3")) {// oracle

						DBOperator dboperator = new DBOperator(arDB.get(0),arDB.get(1), arDB.get(2), arDB.get(3));
						String strSelect = "SELECT * FROM (SELECT ROWNUM AS rowno, t.* FROM T_21_001_00000 t {0} AND ROWNUM <= "
								+ Integer.parseInt(page)* Integer.parseInt(rows)+ ") table_alias WHERE table_alias.rowno > "
								+ (Integer.parseInt(page) - 1)* Integer.parseInt(rows);
						String strWhere = "WHERE C004>to_date('"+ dtstart+ "','YYYY/MM/DD HH24:MI:SS') AND C004<=to_date('"+ dtend + "','YYYY/MM/DD HH24:MI:SS')";
						strWhere += " AND C012>="+ StringUtil.time2sec(lenstart) + " AND C012<"+ StringUtil.time2sec(lenend);
						// AND C040='' AND C041='' AND C045=''";
						if (callno != null && !callno.equals(""))
							strWhere += " AND C040='" + callno + "'";
						if (calledno != null && !calledno.equals(""))
							strWhere += " AND C041='" + calledno + "'";
						if (!direction.equals("2"))
							strWhere += " AND C045='" + direction + "'";
						if (StringUtil.isNotEmpty(ref))
							strWhere += " AND C077='" + ref + "'";
						if (stragent != null && !stragent.equals("")) {
							strWhere += " AND C039 in(" + stragent + ")";
						} else {
							String agentString = "";
							ArrayList<CtrolObject> arryListCtrolAgent = (ArrayList<CtrolObject>) session.getAttribute("sAllAgent");
							if (arryListCtrolAgent != null) {
								for (CtrolObject ctrolObject : arryListCtrolAgent) {
									agentString += ctrolObject.getText()+ "','";
								}
								if (agentString.length() > 3)
									agentString = agentString.substring(0,agentString.length() - 3);
								strWhere += " AND C039 in(" + agentString + ")";
							}
						}
						if (strexten != null && !strexten.equals("")) {
							strWhere += " AND C042 in(" + strexten + ")";
						} else {
							String extenString = "";
							ArrayList<CtrolObject> arryListCtrolExt = (ArrayList<CtrolObject>) session
									.getAttribute("sAllExtension");
							if (arryListCtrolExt != null) {
								for (CtrolObject ctrolObject : arryListCtrolExt) {
									extenString += ctrolObject.getText()
											+ "','";
								}
								if (extenString.length() > 3)
									extenString = extenString.substring(0,
											extenString.length() - 3);
								strWhere += " AND C042 in('" + strexten + "')";
							}
						}

						
						if (!strWhere.equals(""))
							total = dboperator
									.GetDataCount("SELECT COUNT(1) CNUM FROM T_21_001_00000 "
											+ strWhere);
						strSql = MessageFormat.format(strSelect, strWhere);
					 lst = dboperator.excuteResultSet(strSql);
						for (int i = 0; i < lst.size(); i++) {
							Map current = ((Map) lst.get(i));
							String serialid = StringUtil.getStrValue(current.get("C002"));
							String startrecordtime = StringUtil.getStrValue(current.get("C004"));
							String voiceid = StringUtil.getStrValue(current.get("C037"));// current.get("C037").toString();
							String voiceip = StringUtil.getStrValue(current.get("C020"));// current.get("C020").toString();
							String chanel = StringUtil.getStrValue(current.get("C038"));// current.get("C038").toString();
							String extension = StringUtil.getStrValue(current.get("C042"));// current.get("C042").toString();
							String agent = StringUtil.getStrValue(current.get("C039"));// current.get("C039").toString();
							int templen = Integer.parseInt(StringUtil.getStrValue(current.get("C012")));
							String recordlength = StringUtil.secToTime(templen);
							String lanDir = StringUtil.getStrValue(current.get("C045"));
							String dir = lanDir.equals("1") ? showParaName[18]: showParaName[19];
							String cno = StringUtil.getStrValue(current.get("C040"));// current.get("C043").toString();
							String cedno = StringUtil.getStrValue(current.get("C041"));// current.get("C044").toString();
							String recordreference = StringUtil.getStrValue(current.get("C077"));// current.get("C077").toString();
							String skillgroup = "";
							String realextension = StringUtil.getStrValue(current.get("C058"));// current.get("C058").toString();
							String thirdno = "";
							String stoprecordtime = StringUtil.getStrValue(current.get("C008"));// current.get("C008").toString();
							String islabel = "";String isbookmark = "";
							String strc025 = StringUtil.getStrValue(current.get("C025"));
							String isscored = strc025.equals("0") ? showParaName[32]: showParaName[31];
							String isencrypt = strc025;
							String chanelname = StringUtil.getStrValue(current.get("C046"));// current.get("C046").toString();
							rdlist.add(new RecordData(serialid,startrecordtime, voiceid, voiceip, chanel,extension, agent, recordlength, dir, cno,
									cedno, recordreference, isscored,skillgroup, realextension, thirdno,stoprecordtime, islabel, isbookmark,chanelname, isencrypt));
						}
						String strjson = JSONArray.fromObject(rdlist).toString();

						JSONObject result = new JSONObject();
						JSONArray jsonArray = new JSONArray();
						jsonArray = JSONArray.fromObject(rdlist);
						result.put("rows", strjson);
						result.put("total", total);
						try {
							ResponseUtil.write(response, result);
						} catch (Exception e) {
							e.printStackTrace();
						}
					}
				} else if (number>0) {
					if (arDB.get(0).equals("2")) {// sqlserver
						DBOperator dboperator = new DBOperator(arDB.get(0),arDB.get(1), arDB.get(2), arDB.get(3));
						
						/*******************************************************/
						String strSelect = "SELECT w1.* FROM ("+sql +") w1 WHERE C001 in (SELECT top "+ rows+ " C001 FROM (SELECT top "+ Integer.parseInt(page)* Integer.parseInt(rows)
								+ " w.C001, w.C004 FROM ("+sql+") w  {0} ORDER BY C004 DESC, C001 DESC)w ORDER BY w.C004 ASC, w.C001 ASC) ORDER BY w1.C004 DESC, w1.C001 DESC";
						String strWhere = "";
						strWhere = "WHERE C004>'" + dtstart + "' AND C004<='"+ dtend + "'";
						strWhere += " AND C012>="+ StringUtil.time2sec(lenstart) + " AND C012<"+ StringUtil.time2sec(lenend);
						// AND C040='' AND C041='' AND C045=''";
						if (callno != null && !callno.equals(""))
							strWhere += " AND C040='" + callno + "'";
						if (calledno != null && !calledno.equals(""))
							strWhere += " AND C041='" + calledno + "'";
						if (!direction.equals("2"))
							strWhere += " AND C045='" + direction + "'";
						if (StringUtil.isNotEmpty(ref))
							strWhere += " AND C077='" + ref + "'";
						if (stragent != null && !stragent.equals("")) {
							strWhere += " AND C039 in(" + stragent + ")";
						} else {
							String agentString = "";
							ArrayList<CtrolObject> arryListCtrolAgent = (ArrayList<CtrolObject>) session.getAttribute("sAllAgent");
							if (arryListCtrolAgent != null) {
								for (CtrolObject ctrolObject : arryListCtrolAgent) {
									if (Long.parseLong(ctrolObject.getId()) > 1020000000000000000L)
										agentString += ctrolObject.getText()+ "','";
								}
								if (agentString.length() > 3)
									agentString = agentString.substring(0,agentString.length() - 3);
								strWhere += " AND C039 in('" + agentString + "')";
							}
						}
						if (strexten != null && !strexten.equals("")) {
							strWhere += " AND C042 in(" + strexten + ")";
						} else {
							String extenString = "";
							ArrayList<CtrolObject> arryListCtrolExt = (ArrayList<CtrolObject>) session.getAttribute("sAllExtension");
							if (arryListCtrolExt != null) {
								for (CtrolObject ctrolObject : arryListCtrolExt) {
									if (Long.parseLong(ctrolObject.getId()) > 1020000000000000000L)
										extenString += ctrolObject.getText()+ "','";
								}
								if (extenString.length() > 3)
									extenString = extenString.substring(0,extenString.length() - 3);
								strWhere += " AND C042 in('" + extenString + "')";
							}
						}

						if (!strWhere.equals(""))
							total = dboperator.GetDataCount("SELECT COUNT(1) CNUM FROM  ("+sql+" ) b "+ strWhere);
						if (Integer.parseInt(page) * Integer.parseInt(rows) > total)
							strSelect = "SELECT w1.* FROM ("+sql+") w1 WHERE C001 in (SELECT top "
									+ (total - (Integer.parseInt(page) - 1)* Integer.parseInt(rows))
									+ " C001 FROM (SELECT top "+ Integer.parseInt(page)* Integer.parseInt(rows)
									+ " C001, C004 FROM ("+sql+") c {0} ORDER BY C004 DESC, C001 DESC)w ORDER BY w.C004 ASC, w.C001 ASC) ORDER BY w1.C004 DESC, w1.C001 DESC";
						strSql = MessageFormat.format(strSelect, strWhere);
						lst = dboperator.excuteResultSet(strSql);
						/*********************************************************/
						
						
						for (int i = 0; i < lst.size(); i++) {
							Map current = ((Map) lst.get(i));	
							String serialid = StringUtil.getStrValue(current.get("C002"));
							String startrecordtime = StringUtil.getStrValue(current.get("C004"));
							String voiceid = StringUtil.getStrValue(current.get("C037"));// current.get("C037").toString();
							String voiceip = StringUtil.getStrValue(current.get("C020"));// current.get("C020").toString();
							String chanel = StringUtil.getStrValue(current.get("C038"));// current.get("C038").toString();
							String extension = StringUtil.getStrValue(current.get("C042"));// current.get("C042").toString();
							String agent = StringUtil.getStrValue(current.get("C039"));// current.get("C039").toString();
							int templen = Integer.parseInt(StringUtil.getStrValue(current.get("C012")));
							String recordlength = StringUtil.secToTime(templen);
							String lanDir = StringUtil.getStrValue(current.get("C045"));
							String dir = lanDir.equals("1") ? showParaName[18]: showParaName[19];
							String cno = StringUtil.getStrValue(current.get("C040"));
							String cedno = StringUtil.getStrValue(current.get("C041"));
							String recordreference = StringUtil.getStrValue(current.get("C077"));String skillgroup = "";
							String realextension = StringUtil.getStrValue(current.get("C058"));String thirdno = "";
							String stoprecordtime = StringUtil.getStrValue(current.get("C008"));// current.get("C008").toString();
							String islabel = "";String isbookmark = "";
							String strc025 = StringUtil.getStrValue(current.get("C025"));
							String isscored = strc025.equals("0") ? showParaName[32]: showParaName[31];
							String isencrypt = strc025;
							String chanelname = StringUtil.getStrValue(current.get("C046"));// current.get("C046").toString();
							rdlist.add(new RecordData(serialid,startrecordtime, voiceid, voiceip, chanel,extension, agent, recordlength, dir, cno,cedno, recordreference, isscored,skillgroup, realextension, thirdno,stoprecordtime, islabel, isbookmark,chanelname, isencrypt));
						}
						
				        
						//System.out.println("sumtotal的值："+sumtotal);
						String strjson = JSONArray.fromObject(rdlist).toString();
						JSONObject result = new JSONObject();
						JSONArray jsonArray = new JSONArray();
						jsonArray = JSONArray.fromObject(rdlist);
						result.put("rows", strjson);
						result.put("total", total);
						try {
							ResponseUtil.write(response, result);
						} catch (Exception e) {
							e.printStackTrace();
						}
					
					
					} else if (arDB.get(0).equals("3")) {// oracle

						DBOperator dboperator = new DBOperator(arDB.get(0),arDB.get(1), arDB.get(2), arDB.get(3));
						
						String strSelect = "SELECT * FROM (SELECT ROWNUM AS rowno, t.* FROM ("+sql+") t {0} AND ROWNUM <= "
								+ Integer.parseInt(page)* Integer.parseInt(rows)+ ") table_alias WHERE table_alias.rowno > "+ (Integer.parseInt(page) - 1)* Integer.parseInt(rows);
						String strWhere = "WHERE C004>to_date('"+ dtstart+ "','YYYY/MM/DD HH24:MI:SS') AND C004<=to_date('"+ dtend + "','YYYY/MM/DD HH24:MI:SS')";
						strWhere += " AND C012>="+ StringUtil.time2sec(lenstart) + " AND C012<"+ StringUtil.time2sec(lenend);
						// AND C040='' AND C041='' AND C045=''";
						if (callno != null && !callno.equals("")){
							strWhere += " AND C040='" + callno + "'";
						}
						if (calledno != null && !calledno.equals("")){
							strWhere += " AND C041='" + calledno + "'";
						}
						if (!direction.equals("2")){
							strWhere += " AND C045='" + direction + "'";
						}
						if (StringUtil.isNotEmpty(ref)){
							strWhere += " AND C077='" + ref + "'";
						}
						if (stragent != null && !stragent.equals("")) {
							strWhere += " AND C039 in(" + stragent + ")";
						} else {
							String agentString = "";
							ArrayList<CtrolObject> arryListCtrolAgent = (ArrayList<CtrolObject>) session.getAttribute("sAllAgent");
							if (arryListCtrolAgent != null) {
								for (CtrolObject ctrolObject : arryListCtrolAgent) {
									agentString += ctrolObject.getText()+ "','";
								}
								if (agentString.length() > 3)
									agentString = agentString.substring(0,agentString.length() - 3);
								strWhere += " AND C039 in(" + agentString + ")";
							}
						}
						if (strexten != null && !strexten.equals("")) {
							strWhere += " AND C042 in(" + strexten + ")";
						} else {
							String extenString = "";
							ArrayList<CtrolObject> arryListCtrolExt = (ArrayList<CtrolObject>) session.getAttribute("sAllExtension");
							if (arryListCtrolExt != null) {
								for (CtrolObject ctrolObject : arryListCtrolExt) {
									extenString += ctrolObject.getText()+ "','";
								}
								if (extenString.length() > 3)
									extenString = extenString.substring(0,extenString.length() - 3);
								strWhere += " AND C042 in('" + strexten + "')";
							}
						}

						if (!strWhere.equals(""))
							total = dboperator.GetDataCount("SELECT COUNT(1) CNUM FROM ("+sql+") "+ strWhere);
						strSql = MessageFormat.format(strSelect, strWhere);
						lst = dboperator.excuteResultSet(strSql);
						for (int i = 0; i < lst.size(); i++) {
							Map current = ((Map) lst.get(i));
							String serialid = StringUtil.getStrValue(current.get("C002"));
							String startrecordtime = StringUtil.getStrValue(current.get("C004"));
							String voiceid = StringUtil.getStrValue(current.get("C037"));// current.get("C037").toString();
							String voiceip = StringUtil.getStrValue(current.get("C020"));// current.get("C020").toString();
							String chanel = StringUtil.getStrValue(current.get("C038"));// current.get("C038").toString();
							String extension = StringUtil.getStrValue(current.get("C042"));// current.get("C042").toString();
							String agent = StringUtil.getStrValue(current.get("C039"));// current.get("C039").toString();
							int templen = Integer.parseInt(StringUtil.getStrValue(current.get("C012")));
							String recordlength = StringUtil.secToTime(templen);
							String lanDir = StringUtil.getStrValue(current.get("C045"));
							String dir = lanDir.equals("1") ? showParaName[18]: showParaName[19];
							String cno = StringUtil.getStrValue(current.get("C040"));// current.get("C043").toString();
							String cedno = StringUtil.getStrValue(current.get("C041"));// current.get("C044").toString();
							String recordreference = StringUtil.getStrValue(current.get("C077"));// current.get("C077").toString();
							String skillgroup = "";
							String realextension = StringUtil.getStrValue(current.get("C058"));// current.get("C058").toString();
							String thirdno = "";
							String stoprecordtime = StringUtil.getStrValue(current.get("C008"));// current.get("C008").toString();
							String islabel = "";
							String isbookmark = "";
							String strc025 = StringUtil.getStrValue(current.get("C025"));
							String isscored = strc025.equals("0") ? showParaName[32]: showParaName[31];
							String isencrypt = strc025;
							String chanelname = StringUtil.getStrValue(current.get("C046"));// current.get("C046").toString();
							rdlist.add(new RecordData(serialid,startrecordtime, voiceid, voiceip, chanel,extension, agent, recordlength, dir, cno,
									cedno, recordreference, isscored,skillgroup, realextension, thirdno,stoprecordtime, islabel, isbookmark,chanelname, isencrypt));
						}
						
						String strjson = JSONArray.fromObject(rdlist).toString();

						JSONObject result = new JSONObject();
						JSONArray jsonArray = new JSONArray();
						jsonArray = JSONArray.fromObject(rdlist);
						result.put("rows", strjson);
						result.put("total", total);
						try {
							ResponseUtil.write(response, result);
						} catch (Exception e) {
							e.printStackTrace();
						}
					
					}
			}
		}
	}
	
	@SuppressWarnings("unused")
	private void NoData(HttpServletRequest request, HttpServletResponse response) {
		ArrayList<RecordData> arrayList = new ArrayList<RecordData>();
		String strjson = JSONArray.fromObject(arrayList).toString();
		JSONObject result = new JSONObject();
		JSONArray jsonArray = new JSONArray();
		jsonArray = JSONArray.fromObject(arrayList);
		int total = 0;
		result.put("rows", strjson);
		result.put("total", total);
		try {
			ResponseUtil.write(response, result);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	
	
}
