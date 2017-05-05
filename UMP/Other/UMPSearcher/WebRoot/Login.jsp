<%@page import="com.ump.util.LanguageSet"%>
<%@ page language="java" import="java.util.*" pageEncoding="UTF-8"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">

<meta http-equiv="pragma" content="no-cache">
<meta http-equiv="cache-control" content="no-cache">
<meta http-equiv="expires" content="0">
<meta http-equiv="keywords" content="keyword1,keyword2,keyword3">
<meta http-equiv="description" content="This is my page">

<link rel="stylesheet" type="text/css" href="jquery-easyui-1.3.3/themes/default/easyui.css">
<link rel="stylesheet" type="text/css" href="jquery-easyui-1.3.3/themes/icon.css">
<link rel="stylesheet" type="text/css" href="jquery-easyui-1.3.3/demo/demo.css">
<script type="text/javascript" src="jquery-easyui-1.3.3/jquery.min.js"></script>
<script type="text/javascript" src="jquery-easyui-1.3.3/jquery.easyui.min.js"></script>
<script type="text/javascript" src="jquery-easyui-1.3.3/locale/easyui-lang-zh_CN.js"></script>
<script src="JQMp3CoolListPlayer/js/util.js"></script>

<title>系统登录</title>
</head>
<script type="text/javascript">
	var lanset="2052";
	function validateLogin() {
		var user = $("#userName").val();
		var pwd = $("#password").val();
		if(user=="" || pwd=="")
			return;
		
		if($("#loginform").form("validate"))
		{
			if (getCookie("umplanset") == null) {
				$("#lanset").val("2052");
			}
			else{
				var lanset = getCookie("umplanset");
				$("#lanset").val(lanset);
			}
			document.getElementById('loginform').submit();
		}
	}
	
	function languagesel(item) {		
		lanset = item.id;
		//$("#lanset").val(lanset);
		//var val = $("#lanset").val();
		setCookie("umplanset",lanset,"d30");
	}
	
	document.onkeydown=function(event){
	    var e = event || window.event || arguments.callee.caller.arguments[0];     
	     if(e && e.keyCode==13){ // enter 键
	    	 validateLogin();
	    }
	};

</script>
<body background="images/Background.jpg">
	<table height="100%" width="100%" border="0">
		<tbody>
            <tr>
                <td align="center" valign="middle">
                    <table cellspacing="0" cellpadding="0" width="500" height="100" align="center">
                        <tbody>
                            <tr>
                                <td style="width:70px">
                                    <img src="images/SystemLogo.png"/>
                                </td>
                                <td colspan="2">
                                	<span style="font-size:20px;font-weight:bold;color:#bddb7b;font-family:microsoft yahei;">Unified Management Portal</span>
                                </td>
                            </tr>
                            <tr>
                                <td id="logform" colspan="3" style="height:100px;" class="yb">
									<form name="loginform" id="loginform" action="LoginServlt" method="post">
										<table>
											<tr nowrap="nowrap">
												<td>
													<img src="images/LoginAccount.png" style="width:24px; height:24px; position: relative; top:50%;"/>
												</td>
												<td>
													<input class="easyui-textbox" style="width: 170px;height:24px;" type="text" name="userName" id="userName"></input>
												</td>
												<td>
													<img src="images/LoginPassword.png" style="width:24px; height:24px; position: relative; top:50%;"/>
												</td>
												<td>
													<input type="password" name="password" id="password" style="width: 170px;height:24px;" class="easyui-textbox"></input>
													<input type="hidden" name="lanset" id="lanset"></input>
												</td>
												<td>
													<img onclick="validateLogin()" src="images/LoginSystem.png" style="width:22px; height:22px; position: relative; top:50%;"/>
												</td>
												<td>
													<a href="#" class="easyui-menubutton"  style="height:24px;" data-options="menu:'#mm2',iconCls:'icon-LoginOptions'"></a>
												    <div id="mm2" name="mm2" style="width:100px;">
												        <div id="2052" onclick="languagesel(this)">简体中文</div>
												        <div id="1028" onclick="languagesel(this)">繁體中文</div>
												        <div id="1033" onclick="languagesel(this)">U.S. English</div>
												        <div id="1041" onclick="languagesel(this)">日本语</div>
												    </div>
												</td>												
											</tr>
										</table>
									</form>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="height:10px;text-align:center" nowrap="nowrap">
					<img src="images/CopyrightLogo.ico" style="width:24px; height:24px; position: relative;"/>
					<span style="font-size:12px;font-family:microsoft yahei;">Copyright © VoiceCyber 2013-2016 All right reserved.</span>
				</td>
            </tr>
        </tbody>
     </table>    
</body>
</html>
<script> 

</script>