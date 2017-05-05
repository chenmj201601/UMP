<%@page import="com.ump.model.Permission"%>
<%@ page language="java" import="java.util.*" pageEncoding="UTF-8"%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>UMP Search</title>
<%
	try {
	    Thread.sleep(100);
	} catch (InterruptedException e) {
	    e.printStackTrace(); 
	}

	if (session.getAttribute("sCurrentUser") == null) {
		response.sendRedirect("Login.jsp");
		return;
	}
%>
<%
	String showParaName[] = new String[50];
	if (session.getAttribute("slanguage") != null) {
		showParaName = (String[]) session.getAttribute("slanguage");
	}
%>
<%
	Permission permission = new Permission("","","");
	if (session.getAttribute("curPermission") != null) {
		permission = (Permission) session.getAttribute("curPermission");
	}
%>

<link rel="stylesheet" type="text/css"
	href="jquery-easyui-1.3.3/themes/default/easyui.css">
<link rel="stylesheet" type="text/css"
	href="jquery-easyui-1.3.3/themes/icon.css">
<script type="text/javascript" src="jquery-easyui-1.3.3/jquery-3.0.0.min.js"></script>
<script type="text/javascript"
	src="jquery-easyui-1.3.3/jquery.easyui.min.js"></script>
	<script type="text/javascript"
	src="jquery-easyui-1.3.3/locale/easyui-lang-zh_CN.js"></script>

<link rel="stylesheet" href="JQMp3CoolListPlayer/css/reset.css">
<link rel="stylesheet" href="JQMp3CoolListPlayer/css/style.css"
	media="screen" type="text/css" />
<script src="JQMp3CoolListPlayer/js/jquery.jplayer.min.js"></script>
<script src="JQMp3CoolListPlayer/js/jplayer.playlist.min.js"></script>
<script src="JQMp3CoolListPlayer/js/Main.js"></script>
<script src="JQMp3CoolListPlayer/js/util.js"></script>

<%
	if(showParaName[41].equals("2052")){
	%>
	<script type="text/javascript"
	src="jquery-easyui-1.3.3/locale/easyui-lang-zh_CN.js"></script>
	<%}
	if(showParaName[41].equals("1028")){
		%>
		<script type="text/javascript"
		src="jquery-easyui-1.3.3/locale/easyui-lang-zh_TW.js"></script>
		<%}
	if(showParaName[41].equals("1033")){
		%>
		<script type="text/javascript"
		src="jquery-easyui-1.3.3/locale/easyui-lang-en.js"></script>
		<%}
	if(showParaName[41].equals("1041")){
		%>
		<script type="text/javascript"
		src="jquery-easyui-1.3.3/locale/easyui-lang-jp.js"></script>
		<%}

	String query=permission.getQuery();
	String play = permission.getPlay();
	String down = permission.getDownload();
		%>
		 <script type="text/javascript">
		 	window.onload = function() {
		 		$("#curnum").textbox({
		 			disabled : true
		 		});
		 		$("#curtype").combobox({
		 			disabled : true
		 		});
		 		<%
		 		if(query.equals("0")){
		 		%>
				$("#btnq").linkbutton({
					disabled : true
				});
				<%}
		 		if(play.equals("0")){
				%>
				$("#btnp").linkbutton({
					disabled : true
				});
				<%}
		 		if(down.equals("0")){
				%>
				$("#btnd").linkbutton({
					disabled : true
				});
				<%}
				%>
			}
		 </script>
</head>
<body class="easyui-layout" style="font-family: Microsoft YaHei">
	<div region="north" style="height: 50px;background-color: #0065a8;vertical-align: middle">
		<div align="left" style="width: 50%;height:42px;float: left">
			<img src="images/main.jpg" height="41px">
		</div>
		<div align="right">
			<img id="btnq" name="btnq" onclick="javascript:search()" src="images/search.png" height="41" class="easyui-linkbutton" /> &nbsp; 
			<img id="btnp" name="btnp" onclick="javascript:play()" src="images/play.png" height="41" class="easyui-linkbutton" />&nbsp; 
			<img onclick="javascript:addtolist()" src="images/add.png" height="41" class="easyui-linkbutton" />&nbsp; 
			<img id="btnd" name="btnd" onclick="javascript:showdownload()" src="images/download.png" height="41" class="easyui-linkbutton" /> &nbsp; 
			<img onclick="javascript:loginout()" src="images/logout.png" height="41" class="easyui-linkbutton" /> &nbsp; 
			
			<a title='${sCurrentUser.orgNameString}/${sCurrentUser.userNameString}' class="easyui-tooltip"> 
				<img src="images/user.png" height="41" class="easyui-linkbutton"  data-options="disabled:true" /> 
			</a>&nbsp; 
		</div>
	</div>
	<div region="center">
		<div class="easyui-tabs" fit="true" border="false" id="maintabs"
			name="maintabs">
			<div title="<%=showParaName[2]%>" id="tabsearch" name="tabsearch">
				<table id="recordlist" class="easyui-datagrid" fitColumns="true"
					data-options="singleSelect:true" pagination="true"
					rownumbers="true" fit="true" url="RecordSearchServlet"
					pageSize="50" pageList="[10,30,50,100]">
					<thead>
						<tr>
							<th field="serialid" width="60" align="center" hidden="true">记录流水号</th>
							<th field="extension" width="70" align="center"><%=showParaName[4]%></th>
							<th field="agent" width="70" align="center"><%=showParaName[5]%></th>
							<th field="startrecordtime" width="130" align="center"><%=showParaName[6]%></th>
							<th field="recordlength" width="70" align="center"><%=showParaName[8]%></th>
							<th field="direction" width="70" align="center"><%=showParaName[9]%></th>
							<th field="callno" width="70" align="center"><%=showParaName[10]%></th>
							<th field="calledno" width="70" align="center"><%=showParaName[11]%></th>
							<th field="recordreference" width="140" align="center"><%=showParaName[12]%></th>
							<th field="voiceid" width="90" align="center" hidden="true"><%=showParaName[13]%></th>
							<th field="voiceip" width="90" align="center"><%=showParaName[14]%></th>
							<th field="chanel" width="70" align="center" hidden="true"><%=showParaName[15]%></th>
							<th field="isencrypt" width="70" align="center" hidden="true"></th>
							<th field="isscored" width="70" align="center"><%=showParaName[30]%></th>
							<th field="skillgroup" width="100" align="center" hidden="true">技能组</th>
							<th field="realextension" width="100" align="center" hidden="true">真实分机</th>
							<th field="thirdno" width="100" align="center" hidden="true">第三方号码</th>
							<th field="stoprecordtime" width="100" align="center" hidden="true">录音结束时间</th>
							<th field="islabel" width="100" align="center" hidden="true">是否有标签</th>
							<th field="isbookmark" width="100" align="center" hidden="true">是否有备注</th>
							<th field="chanelname" width="100" align="center" hidden="true">通道名称</th>
						</tr>
					</thead>
				</table>
			</div>
			<div title="<%=showParaName[3]%>" id="tabplay" name="tabplay">
				<table id="playlist" class="easyui-datagrid" fitColumns="true"
					pagination="true" rownumbers="true" fit="true" pageSize="50"
					pageList="[10,30,50,100]" data-options="singleSelect:false">
					<thead>
						<tr>
							<th field="serialid" width="60" align="center" hidden="true">记录流水号</th>
							<th field="extension" width="70" align="center"><%=showParaName[4]%></th>
							<th field="agent" width="70" align="center"><%=showParaName[5]%></th>
							<th field="startrecordtime" width="130" align="center"><%=showParaName[6]%></th>
							<th field="recordlength" width="70" align="center"><%=showParaName[8]%></th>
							<th field="direction" width="70" align="center"><%=showParaName[9]%></th>
							<th field="callno" width="70" align="center"><%=showParaName[10]%></th>
							<th field="calledno" width="70" align="center"><%=showParaName[11]%></th>
							<th field="recordreference" width="140" align="center"><%=showParaName[12]%></th>
							<th field="voiceid" width="90" align="center" hidden="true"><%=showParaName[13]%></th>
							<th field="voiceip" width="90" align="center"><%=showParaName[14]%></th>
							<th field="chanel" width="70" align="center" hidden="true"><%=showParaName[15]%></th>
							<th field="isencrypt" width="70" align="center" hidden="true"></th>
							<th field="isscored" width="70" align="center"><%=showParaName[30]%></th>
							<th field="skillgroup" width="100" align="center" hidden="true">技能组</th>
							<th field="realextension" width="100" align="center" hidden="true">真实分机</th>
							<th field="thirdno" width="100" align="center" hidden="true">第三方号码</th>
							<th field="stoprecordtime" width="100" align="center" hidden="true">录音结束时间</th>
							<th field="islabel" width="100" align="center" hidden="true">是否有标签</th>
							<th field="isbookmark" width="100" align="center" hidden="true">是否有备注</th>
							<th field="chanelname" width="100" align="center" hidden="true">通道名称</th>
						</tr>
					</thead>
				</table>
			</div>

		</div>
	</div>
	<div region="south" align="center" style="OVERFLOW-Y:hidden">
		<table style="width:100%;">
			<tr>
				<td>
					<div id="video_container">
						<!------------------------------------------------------------------------------------------------------>
						<div class="music-player">

							<div class="progress jp-seek-bar">
								<span class="jp-play-bar" style="width: 0%"></span>
							</div>
							<div class="controls">
								<div class="play-controls">
								  <a class="current jp-current-time">00:00</a> 
								  <a href="javascript:playslow();" class="icon-slow"></a>
							      <a href="javascript:;" class="icon-play jp-play" title="play"></a>
							      <a href="javascript:;" class="icon-pause jp-pause" title="pause"></a>
								  <a href="javascript:;" class="icon-stop jp-stop" title="stop"></a>
								  <a href="javascript:playfast();" class="icon-fast"></a>
								</div>
							</div>
							<div id="jquery_jplayer" class="jp-jplayer"></div>
						</div>
						<!------------------------------------------------------------------------------------------------------>
					</div></td>
			</tr>
		</table>
	</div>
	<div id="w" class="easyui-window" title="<%=showParaName[2]%>"
		data-options="modal:true,closed:true,minimizable:false,collapsible:false,maximizable:false,footer:'#foot'"
		style="width:500px;height:340px;padding:1px;">
		<form id="fm" method="post">
			<div class="easyui-tabs" style="width:100%;height:265px;">
				<div title="<%=showParaName[16]%>">
					<table style="margin:5px 5px 5px 5px;width:428;height:203"
						cellspacing="5px;">
						<tr>
							<td width="110"><%=showParaName[20]%>：</td>
							<td width="20"><input id="ckcur" name="ckcur"
								type="checkbox" onclick="currentClick()">
							</td>
							<td width="85"><input class="easyui-numberbox" name="curnum"
								id="curnum" style="width: 80px"></input>
							</td>
							<td width="267" colspan="2"><select class="easyui-combobox"
								id="curtype" name="curtype" editable="false" panelHeight="auto"
								style="width: 89px">
									<option value="0"><%=showParaName[21]%></option>
									<option value="1"><%=showParaName[22]%></option>
									<option value="2"><%=showParaName[23]%></option>
									<option value="3"><%=showParaName[24]%></option>
							</select>
							</td>
						</tr>
						<tr>
							<td><%=showParaName[6]%>：</td>
							<td width="20"></td>
							<td width="467" colspan="4"><input
								class="easyui-datetimebox" data-options="required:true"
								id="dtstart" name="dtstart" editable="false" />
							</td>
						</tr>
						<tr>
							<td><%=showParaName[7]%>：</td>
							<td width="20"></td>
							<td colspan="4"><input class="easyui-datetimebox"
								data-options="required:true" id="dtend" name="dtend"
								editable="false" />
							</td>
						</tr>
						<tr>
							<td><%=showParaName[10]%>：</td>
							<td width="20"></td>
							<td colspan="4"><input type="text" name="callno" id="callno"
								class="easyui-validatebox" />
							</td>
						</tr>
						<tr>
							<td><%=showParaName[11]%>：</td>
							<td width="20"></td>
							<td colspan="4"><input type="text" name="calledno"
								id="calledno" class="easyui-validatebox" />
							</td>
						</tr>
						<tr>
							<td><%=showParaName[12]%>：</td>
							<td width="20"></td>
							<td colspan="4"><input type="text" name="reference"
								id="reference" class="easyui-validatebox" />
							</td>
						</tr>
						<tr>
							<td><%=showParaName[9]%>：</td>
							<td width="20"></td>
							<td colspan="4"><select class="easyui-combobox"
								id="direction" name="direction" editable="false"
								panelHeight="auto" style="width: 155px">
									<option value="2"><%=showParaName[17]%></option>
									<option value="1"><%=showParaName[18]%></option>
									<option value="0"><%=showParaName[19]%></option>
							</select>
							</td>
						</tr>
						<tr>
							<td width="110"><%=showParaName[25]%>：</td>
							<td width="20"></td>
							<td width="60"><input class="easyui-timespinner"
								style="width:80px;" id="lenstart" name="lenstart"
								required="required" data-options="showSeconds:true"
								value="00:00:00">
							</td>
							<td style="width:20;text-align:center;">~</td>
							<td><input class="easyui-timespinner" style="width:80px;"
								id="lenend" name="lenend" required="required"
								data-options="showSeconds:true" value="00:30:00">
							</td>
						</tr>
					</table>
				</div>
				<div title="<%=showParaName[5]%>" style="padding:10px">
					<ul id="tragent" name="tragent" class="easyui-tree"
						data-options="method:'get',animate:true,checkbox:true"></ul>
				</div>
				<div title="<%=showParaName[4]%>" style="padding:10px">
					<ul id="trextension" name="trextension" class="easyui-tree"
						data-options="method:'get',animate:true,checkbox:true"></ul>
				</div>
			</div>
		</form>
	</div>
	<div id="foot" style="text-align: right; padding: 5px 5;border:1">
		<a href="javascript:searchRecord()" class="easyui-linkbutton"
			data-options="iconCls:'icon-ok'" plain="true"><%=showParaName[2]%></a>
	</div>

	<div id="winSave" class="easyui-window" title="<%=showParaName[36]%>"
		data-options="modal:true,closed:true,minimizable:false,collapsible:false,maximizable:false,footer:'#saveFoot'"
		style="width:420px;height:300px;padding:1px;">
		<form id="fmsave" method="post">
			<table style="margin:1px 1px 1px 1px;width:370;height:220"
				cellspacing="1px;">
				<tr>
					<td><%=showParaName[34]%>：</td>
					<td width="20"></td>
					<td colspan="4"><select class="easyui-combobox" id="ftype"
						name="ftype" editable="false" panelHeight="auto"
						style="width: 155px">
							<option value="0">MP3</option>
							<option value="1">WAV</option>
					</select>
					</td>
				</tr>
				<tr>
					<td><%=showParaName[35]%>：</td>
					<td width="20">
					</td>
					<td colspan="4">
					</td>
				</tr>
				<tr>
					<td style="text-align:right"><%=showParaName[5]%>：</td>
					<td width="20"></td>
					<td colspan="4">
						<input id="cka" name="cka" type="checkbox" onclick="downcheck(this)">&nbsp;<label id="lbla"></label>
					</td>
				</tr>
				<tr>
					<td style="text-align:right"><%=showParaName[4]%>：</td>
					<td width="20"></td>
					<td colspan="4">
						<input id="cke" name="cke" type="checkbox" onclick="downcheck(this)">&nbsp;<label id="lble"></label>
					</td>
				</tr>
				<tr>
					<td style="text-align:right"><%=showParaName[12]%>：</td>
					<td width="20"></td>
					<td colspan="4">
						<input id="ckr" name="ckr" type="checkbox" onclick="downcheck(this)">&nbsp;<label id="lblr"></label>
					</td>
				</tr>
				<tr>
					<td style="text-align:right"><%=showParaName[6]%>：</td>
					<td width="20"></td>
					<td colspan="4">
						<input id="ckd" name="ckd" type="checkbox" onclick="downcheck(this)">&nbsp;<label id="lbld"></label>
					</td>
				</tr>
				<tr>
					<td style="text-align:right"><%=showParaName[10]%>：</td>
					<td width="20"></td>
					<td colspan="4">
						<input id="ckcl" name="ckcl" type="checkbox" onclick="downcheck(this)">&nbsp;<label id="lblcl"></label>
					</td>
				</tr>
				<tr>
					<td style="text-align:right"><%=showParaName[11]%>：</td>
					<td width="20"></td>
					<td colspan="4">
						<input id="ckcd" name="ckcd" type="checkbox" onclick="downcheck(this)">&nbsp;<label id="lblcd"></label>
					</td>
				</tr>
			</table>
		</form>
	</div>
	<div id="saveFoot" style="text-align: right; padding: 5px 5;border:1">
		<a href="javascript:recdownload()" class="easyui-linkbutton"
			data-options="iconCls:'icon-ok'" plain="true"><%=showParaName[36]%></a>
	</div>

	<div id="enwin" class="easyui-window" title="<%=showParaName[37]%>"
		data-options="modal:true,closed:true,minimizable:false,collapsible:false,maximizable:false,footer:'#enFoot'"
		style="width:310px;height:180px;padding:1px;">
		<form id="fmen" method="post">
			<table style="margin:4px 1px 1px 1px;width:280;height:80"
				cellspacing="5px;">
				<tr>
					<td><%=showParaName[37]%>：</td>
					<td width="20"></td>
					<td colspan="4"><input type="password" name="enpwd" id="enpwd"
						style="width: 170px;height:24px;" data-options="required:true"
						class="easyui-textbox"></input></td>
				</tr>
				<tr>
					<td>记住密码：</td>
					<td width="20"></td>
					<td colspan="4"><input id="ckrember" name="ckrember"
						type="checkbox"></td>
				</tr>
			</table>
		</form>
	</div>
	<div id="enFoot" style="text-align: right; padding: 5px 5;border:1">
		<a href="javascript:encryptplay()" class="easyui-linkbutton"
			data-options="iconCls:'icon-ok'" plain="true">确认</a>
	</div>
</body>
</html>
