function searchRecord() {

	var dtstart = $('#dtstart').datetimebox('getValue');
	var dtend = $('#dtend').datetimebox('getValue');
	var callno = $("#callno").val();
	var calledno = $("#calledno").val();
	var direction = $('#direction').combobox('getValue');

	var ckcur = false;
	if ($("#ckcur").is(":checked"))
		ckcur = true;
	var curnum = $("#curnum").val();
	var curtype = $('#curtype').combobox('getValue');
	var reference = $("#reference").val();
	var lenstart = $("#lenstart").val();
	var lenend = $("#lenend").val();

	var agenttree = $('#tragent').tree('getChecked');
	var agent = null;
	for ( var i = 0; i < agenttree.length; i++) {
		if (agent != '')
			agent += ',';
		if (agenttree[i].id > 1020000000000000000)
			agent += agenttree[i].text;
	}

	var exttree = $('#trextension').tree('getChecked');
	var extension = null;
	for ( var i = 0; i < exttree.length; i++) {
		if (extension != '')
			extension += ',';
		if (exttree[i].id > 1020000000000000000)
			extension += exttree[i].text;
	}
	if ($("#fm").form("validate")) {
		$.get("RecordSearchServlet", {
			dtstart : dtstart,
			dtend : dtend,
			callno : callno,
			calledno : calledno,
			direction : direction,
			agent : agent,
			extension : extension,
			ckcur : ckcur,
			curnum : curnum,
			curtype : curtype,
			reference : reference,
			lenstart : lenstart,
			lenend : lenend
		}, function(result) {
			var jdata = JSON.parse(result);
			$("#recordlist").datagrid('loadData', jdata);
			if (result.errorMsg) {
				// $.messager.alert("系统提示",result.errorMsg);
				// return;
			} else {
				// $.messager.alert("系统提示","保存成功");
				// resetValue();
				$("#w").dialog("close");
				// $("#recordlist").datagrid("reload");
			}
		});
	}
}

function BindAgentAndExtension() {
	var agent = $('#tragent')[0].innerText;
	if (agent == "") {
		var url = "GetAgentListServlet";
		$.get(url, null, function(data1) {
			var jdata1 = JSON.parse(data1);
			var agent = jdata1.strnode;
			var jsonDataTree = transData(agent, 'id', 'mOrgID', 'children');
			$("#tragent").tree({
				data : jsonDataTree
			});
		});
	}
	var extension = $('#trextension')[0].innerText;
	if (extension == "") {
		var url = "GetExtensionListServlet";
		$.get(url, null,
				function(data1) {
					var jdata1 = JSON.parse(data1);
					var extension = jdata1.strnode;
					var jsonDataTree = transData(extension, 'id', 'mOrgID',
							'children');
					$("#trextension").tree({
						data : jsonDataTree
					});
				});
	}
}

// 查询
function search() {
	$('#dtstart').datetimebox('setValue', getNowFormatDate(-1));
	$('#dtend').datetimebox('setValue', getNowFormatDate(0));
	BindAgentAndExtension();
	$('#w').window('open');
}

// 播放器初始化
// -----------------------------------------------------------------------------------------
var cssSelector = {
	jPlayer : "#jquery_jplayer",
	cssSelectorAncestor : ".music-player"
};

var options = {
	autoPlay : true,
	swfPath : "Jplayer.swf",
	remainingDuration : true,
	supplied : "ogv, m4v, oga, mp3",
	timeupdate : function(event) {
		$('#player1.seekBar').val(event.jPlayer.status.currentPercentRelative);
	},
	ended : function() {
		var seltabid = $('#maintabs').tabs('getSelected').panel('options').id;
		var row;
		if (seltabid == 'tabsearch') {
			row = $('#recordlist').datagrid('getSelected');
			if (row) {
				var rowIndex = $('#recordlist').datagrid('getRowIndex', row);
				$('#recordlist').datagrid('selectRow', rowIndex + 1);
				play();
			}
		} else {
			row = $('#playlist').datagrid('getSelected');
			if (row) {
				var rowIndex = $('#playlist').datagrid('getRowIndex', row);
				$('#playlist').datagrid('unselectAll');
				$('#playlist').datagrid('selectRow', rowIndex + 1);
				play();
			}
		}
	}
};
var myPlaylist;
$(document).ready(function() {
	var playlist = [];
	myPlaylist= new jPlayerPlaylist(cssSelector, playlist, options);
});

function playslow() {
	var times = $("#jquery_jplayer").data("jPlayer").status.currentTime;
	$("#jquery_jplayer").jPlayer("play", times - 5);
};

function playfast() {
	var times = $("#jquery_jplayer").data("jPlayer").status.currentTime;
	$("#jquery_jplayer").jPlayer("play", times + 5);
};
// -----------------------------------------------------------------------------------------

// 播放
function play() {
	var seltabid = $('#maintabs').tabs('getSelected').panel('options').id;
	var row;
	if (seltabid == 'tabsearch') {
		row = $('#recordlist').datagrid('getSelected');
	} else {
		row = $('#playlist').datagrid('getSelected');
	}
	if (row) {
		var templen = row.recordlength;
		if (templen != "00:00") {
			var encrypt = row.isencrypt;
			if (encrypt == "0") {
				var serialid = row.serialid;
				var recordreference = row.recordreference;
				$.get("GetUrlPlayerServlet", {
					ref : recordreference,
					serialid : serialid,
					isencry : 0,
					umppwd : ""
				}, function(result) {
					myPlaylist = new jPlayerPlaylist(cssSelector, playlist,
							options);
					myPlaylist.add({
						mp3 : result
					});
					myPlaylist.play();
					addtolist();
					jplayer_player();
				});
			} else {
				// setCookie("umpRemberPS","123456","d30");
				// delCookie("umpRemberPS");
				if (getCookie("umpRemberPS") == null) {
					$('#fmen').form('load', {
						enpwd : '',
					});
					$('#enwin').window('open');
				} else {
					/*var pp = getCookie("umpRemberPS");
					$('#fmen').form('load', {
						enpwd : pp,
					});
					$("#ckrember").prop("checked", "checked");
					$('#enwin').window('open');*/
					var pp = getCookie("umpRemberPS");
					rembenplay(pp);
				}
			}
		}
	}
}

function jplayer_player(){
	myPlaylist.play();
}

function rembenplay(pwd){
	var seltabid = $('#maintabs').tabs('getSelected').panel('options').id;
	var row;
	if (seltabid == 'tabsearch') {
		row = $('#recordlist').datagrid('getSelected');
	} else {
		row = $('#playlist').datagrid('getSelected');
	}
	if (row) {
		var templen = row.recordlength;
		if (templen != "00:00") {
			var serialid = row.serialid;
			var recordreference = row.recordreference;
			$.get("GetUrlPlayerServlet", {
				ref : recordreference,
				serialid : serialid,
				isencry : 1,
				umppwd : pwd
			}, function(result) {
				var len = result.length;
				if (len > 4 && result.substr(0, 4) == "http") {
					myPlaylist = new jPlayerPlaylist(cssSelector, playlist,
							options);
					myPlaylist.add({
						mp3 : result
					});
					myPlaylist.play(-1);
					addtolist();
				} else {
					$('#enwin').window('open');
				}
			});
		}
	}
}

function encryptplay() {
	var pwd = $('#enpwd').val();
	var ckrember = false;
	if ($("#ckrember").is(":checked"))
		ckrember = true;
	if (pwd == "")
		return;
	if (ckrember)
		setCookie("umpRemberPS", pwd, "d30");
	else
		delCookie("umpRemberPS");

	var seltabid = $('#maintabs').tabs('getSelected').panel('options').id;
	var row;
	if (seltabid == 'tabsearch') {
		row = $('#recordlist').datagrid('getSelected');
	} else {
		row = $('#playlist').datagrid('getSelected');
	}
	if (row) {
		var templen = row.recordlength;
		if (templen != "00:00") {
			var serialid = row.serialid;
			var recordreference = row.recordreference;
			$.get("GetUrlPlayerServlet", {
				ref : recordreference,
				serialid : serialid,
				isencry : 1,
				umppwd : pwd
			}, function(result) {
				var len = result.length;
				if (len > 4 && result.substr(0, 4) == "http") {
					myPlaylist = new jPlayerPlaylist(cssSelector, playlist,
							options);
					myPlaylist.add({
						mp3 : result
					});
					myPlaylist.play(-1);
					addtolist();
					$('#enwin').window('close');
				} else {
					alert(result);
				}
			});
		}
	}
}

// 添加到播放列表
function addtolist() {
	var rowsel = $('#recordlist').datagrid('getSelected');
	if (rowsel) {
		var datas = $('#playlist').datagrid('getData');
		var isNotContain = true;
		for ( var i = 0; i < datas.rows.length; i++) {
			var rowi = datas.rows[i];
			if (rowi['recordreference'] == rowsel['recordreference'])
				isNotContain = false;
		}
		if (isNotContain)
			$("#playlist").datagrid("appendRow", rowsel);// 添加到播放列表
	}
}

function downloadFile(url) {   
    try{ 
        var elemIF = document.createElement("iframe");   
        elemIF.src = url;   
        elemIF.style.display = "none";   
        document.body.appendChild(elemIF);   
    }catch(e){ 

    } 
}

function showdownload() {
	if (detectOS()){
		$('#winSave').window('open');
	}
}

function recdownload() {
	if(strname=="")
		return;
	var ftype = $('#ftype').combobox('getValue');

	var serialids = "";//
	var refs = "";
	var agents = "";
	var extens = "";
	var dtstarts = "";
	var calls = "";
	var calleds = "";
	var rows = $('#playlist').datagrid('getSelections');
	for ( var i = 0; i < rows.length; i++) {
		var serialid = rows[i].serialid;
		var ref = rows[i].recordreference;
		var agent = rows[i].agent;
		var exten = rows[i].extension;
		var dtstart = rows[i].startrecordtime;
		var callno = rows[i].callno;
		var calledno = rows[i].calledno;

		serialids += serialid + ",";
		refs += ref + ",";
		agents += agent + ",";
		extens += exten + ",";
		dtstarts += dtstart + ",";
		calls += callno + ",";
		calleds += calledno + ",";
	}

	$.get("RecDownloadServlet", {
		serialids : serialids,
		refs : refs,
		agents : agents,
		extens : extens,
		dtstarts : dtstarts,
		ared : strname,
		ftype : ftype,
		calls : calls,
		calleds : calleds
	}, function(result) {
		if(result!=null && result.length>4 && result.substr(0, 4)=="umps"){
			var files = result.substr(4, result.length);
			var strs= new Array(); //定义一数组 
			strs=files.split(","); //字符分割 
			for (i=0;i<strs.length ;i++ ) { 
				window.open("doDownload.jsp?url=" + strs[i]); 
			} 
			$('#winSave').window('close');
		}
		else if(result!=null && result.length>4 && result.substr(0, 4)=="umpf"){
			var msg = result.substr(4, result.length);
			alert(msg);
		}
		else
			alert(result);
	});
}

var chknum = 1;
var strname = "";
function downcheck(chk){
	var strid = chk.id;
	if(chk.checked){
		if(strid=="cka"){
			$("#lbla").html(chknum); 
			chknum+=1;
			strname+="A";
			}
		else if(strid=="cke"){
			$("#lble").html(chknum); 
			chknum+=1;
			strname+="E";
			}
		else if(strid=="ckr"){
			$("#lblr").html(chknum); 
			chknum+=1;
			strname+="R";
			}
		else if(strid=="ckd"){
			$("#lbld").html(chknum); 
			chknum+=1;
			strname+="D";
			}
		else if(strid=="ckcl"){
			$("#lblcl").html(chknum); 
			chknum+=1;
			strname+="Z";
			}
		else if(strid=="ckcd"){
			$("#lblcd").html(chknum); 
			chknum+=1;
			strname+="B";
			}
	}
	else{
		if(strid=="cka"){
			var temp = $("#lbla").html(); 
			if(temp==chknum-1)
				chknum-=1;
			$("#lbla").html(""); 
			strname = strname.replace("A","");
			}
		else if(strid=="cke"){
			var temp = $("#lble").html(); 
			if(temp==chknum-1)
				chknum-=1;
			$("#lble").html(""); 
			strname = strname.replace("E","");
			}
		else if(strid=="ckr"){
			var temp = $("#lblr").html(); 
			if(temp==chknum-1)
				chknum-=1;
			$("#lblr").html(""); 
			strname = strname.replace("R","");
			}
		else if(strid=="ckd"){
			var temp = $("#lbld").html(); 
			if(temp==chknum-1)
				chknum-=1;
			$("#lbld").html(""); 
			strname = strname.replace("D","");
			}
		else if(strid=="ckcl"){
			var temp = $("#lblcl").html(); 
			if(temp==chknum-1)
				chknum-=1;
			$("#lblcl").html(""); 
			strname = strname.replace("Z","");
			}
		else if(strid=="ckcd"){
			var temp = $("#lblcd").html(); 
			if(temp==chknum-1)
				chknum-=1;
			$("#lblcd").html(""); 
			strname = strname.replace("B","");
			}
	}
}

// 下载
function download() {
	var serialids = "";//
	var refs = "";
	var agents = "";
	var extens = "";
	var dtstarts = "";
	var rows = $('#playlist').datagrid('getSelections');
	for ( var i = 0; i < rows.length; i++) {
		var serialid = rows[i].serialid;
		var ref = rows[i].recordreference;
		var agent = rows[i].agent;
		var exten = rows[i].extension;
		var dtstart = rows[i].startrecordtime;

		serialids += serialid + ",";
		refs += ref + ",";
		agents += agent + ",";
		extens += exten + ",";
		dtstarts += dtstart + ",";
	}

	$.get("RecDownloadServlet", {
		serialids : serialids,
		refs : refs,
		agents : agents,
		extens : extens,
		dtstarts : dtstarts
	}, function(result) {
		if (result != "")
			alert(result);
	});
}

function loginout(){
	var url="LoginoutServlet";
	window.location.href=url;
}


