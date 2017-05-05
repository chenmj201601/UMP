-- ----------------------------
-- Records of T_25_006  xue 20160523 
-- ----------------------------

  DELETE  FROM T_25_006 WHERE C001=2020000000000001065 ;
  
    INSERT INTO [dbo].[T_25_006] VALUES (N'2020000000000001065', N'3', N'6412', N'8', N'0', N'1', N'通道无录音', N' ', N' ', N'', N'', N'', N'', N'', N'', N'', N'');
  
  DELETE  FROM T_25_006 WHERE C001=2020000000000001066 ;
    
    INSERT INTO [dbo].[T_25_006] VALUES (N'2020000000000001066', N'3', N'6412', N'8', N'1', N'1', N'通道无录音', N' ', N' ', N'', N'', N'', N'', N'', N'', N'', N'');
  
  DELETE  FROM T_25_006 WHERE C001=2020000000000001067;
  
		INSERT INTO [dbo].[T_25_006] VALUES (N'2020000000000001067', N'3', N'6412', N'9', N'0', N'1', N'加密失败数量', N' ', N' ', N'', N'', N'', N'', N'', N'', N'', N'');
  
  DELETE  FROM T_25_006 WHERE C001=2020000000000001068;
  
		INSERT INTO [dbo].[T_25_006] VALUES (N'2020000000000001068', N'3', N'6412', N'9', N'1', N'1', N'加密失败数量超过上限', N' ', N' ', N'', N'', N'', N'', N'', N'', N'', N'');
  
  DELETE  FROM T_25_006 WHERE C001=2020000000000001069;
  
		INSERT INTO [dbo].[T_25_006] VALUES (N'2020000000000001069', N'3', N'6412', N'10', N'0', N'1', N'归档失败数量', N' ', N' ', N'', N'', N'', N'', N'', N'', N'', N'');
  
  DELETE  FROM T_25_006 WHERE C001=2020000000000001070;
  
		INSERT INTO [dbo].[T_25_006] VALUES (N'2020000000000001070', N'3', N'6412', N'10', N'1', N'1', N'归档失败数量超过上限', N' ', N' ', N'', N'', N'', N'', N'', N'', N'', N'');
  
  DELETE  FROM T_25_006 WHERE C001=2020000000000001071;
  
		INSERT INTO [dbo].[T_25_006] VALUES (N'2020000000000001071', N'3', N'6412', N'11', N'0', N'1', N'录屏数量', N' ', N' ', N'', N'', N'', N'', N'', N'', N'', N'');
  
  DELETE  FROM T_25_006 WHERE C001=2020000000000001072;
  
		INSERT INTO [dbo].[T_25_006] VALUES (N'2020000000000001072', N'3', N'6412', N'11', N'1', N'1', N'录屏数量不足', N' ', N' ', N'', N'', N'', N'', N'', N'', N'', N'');
  
  DELETE  FROM T_25_006 WHERE C001=2020000000000005001;
  
		INSERT INTO [dbo].[T_25_006] VALUES (N'2020000000000005001', N'3', N'5396', N'0', N'0', N'1', N'录音服务器', N' ', N' ', N'', N'', N'', N'', N'', N'', N'', N'');
  
  DELETE  FROM T_25_006 WHERE C001=2020000000000005002;
  
		INSERT INTO [dbo].[T_25_006] VALUES (N'2020000000000005002', N'3', N'5396', N'1', N'0', N'1', N'实时长录音', N' ', N' ', N'', N'', N'', N'', N'', N'', N'', N'');
  
  DELETE  FROM T_25_006 WHERE C001=2020000000000005003;
  
		INSERT INTO [dbo].[T_25_006] VALUES (N'2020000000000005003', N'3', N'5396', N'1', N'1', N'1', N'长录音恢复', N' ', N' ', N'', N'', N'', N'', N'', N'', N'', N'');
  
  DELETE  FROM T_25_006 WHERE C001=2020000000000005004;
  
		INSERT INTO [dbo].[T_25_006] VALUES (N'2020000000000005004', N'3', N'5396', N'1', N'2', N'1', N'录音时长超过上限', N' ', N' ', N'', N'', N'', N'', N'', N'', N'', N'');
  
  DELETE  FROM T_25_006 WHERE C001=2020000000000005005;
  
		INSERT INTO [dbo].[T_25_006] VALUES (N'2020000000000005005', N'3', N'5396', N'2', N'0', N'1', N'通道断线', N' ', N' ', N'', N'', N'', N'', N'', N'', N'', N'');
  
  DELETE  FROM T_25_006 WHERE C001=2020000000000005006;
  
		INSERT INTO [dbo].[T_25_006] VALUES (N'2020000000000005006', N'3', N'5396', N'2', N'1', N'1', N'通道断线恢复', N' ', N' ', N'', N'', N'', N'', N'', N'', N'', N'');
  
  DELETE  FROM T_25_006 WHERE C001=2020000000000005007;
  
		INSERT INTO [dbo].[T_25_006] VALUES (N'2020000000000005007', N'3', N'5396', N'2', N'2', N'1', N'通道断线', N' ', N' ', N'', N'', N'', N'', N'', N'', N'', N'');
  


--告警语言包 xu 20160523


  DELETE  FROM t_00_007 WHERE C001=1028 AND C002=6412 AND C003=3 AND C004=11 AND C005=1 AND C006=0;
  
    INSERT INTO [dbo].[t_00_007] 
      values ('1028', '6412', '3', '11', '1', '0', '錄屏伺服器%1%分機%2%錄屏數量不足', '錄屏資料庫位址：%1%，錄屏伺服器：[%2%]%3%，通道：%4%，分機：%5%，時間：%6%-%7%，錄屏數量：%8%筆，少於%9%筆', null, '請檢查此通道是否正常工作');
  
  
  DELETE  FROM t_00_007 WHERE C001=1033 AND C002=6412 AND C003=3 AND C004=11 AND C005=1 AND C006=0;
  
    INSERT INTO [dbo].[t_00_007] 
      values ('1033', '6412', '3', '11', '1', '0', 'Screen server %1% Extension %2% Insufficient recording', 'Screen Database IP: %1%, Screen Server: [%2%]%3%, Channel: %4%, Extension: %5%, Time: %6%-%7%, Quantity: %8%, less than %9%', null, 'Please check current channel is working normally.');
  
  
  DELETE  FROM t_00_007 WHERE C001=1041 AND C002=6412 AND C003=3 AND C004=11 AND C005=1 AND C006=0;
  
    INSERT INTO [dbo].[t_00_007] 
      values ('1041', '6412', '3', '11', '1', '0', '錄屏サーバ%1%内線%2%錄屏件数不足', '錄屏データベースIP：%1%、錄屏サーバ：[%2%]%3%、チャネル：%4%、内線：%5%、時間：%6%-%7%、錄屏：%8%件、%9%件より少ないです。', null, '該当チャネルが正常に動作していますか');
  
  
  DELETE  FROM t_00_007 WHERE C001=2052 AND C002=6412 AND C003=3 AND C004=11 AND C005=1 AND C006=0;
  
    INSERT INTO [dbo].[t_00_007] 
      values ('2052', '6412', '3', '11', '1', '0', '录屏服务器%1%分机%2%录屏数量不足', '录屏数据库地址：%1%，录屏服务器：[%2%]%3%，通道：%4%，分机：%5%，时间：%6%-%7%，录屏数量：%8%条，少于%9%条', null, '请检查此通道是否正常工作');
  
  
  DELETE  FROM t_00_007 WHERE C001=1028 AND C002=5396 AND C003=3 AND C004=2 AND C005=1 AND C006=0;
    
    INSERT INTO [dbo].[T_00_007] VALUES (N'1028', N'5396', N'3', N'2', N'1', N'0', N'錄音伺服器編號：%1%所有通道狀態恢復正常', N'錄音伺服器編號：%1%，錄音伺服器IP：%2%，所有通道恢復正常', N'', N'');
  
  
  DELETE  FROM t_00_007 WHERE C001=1028 AND C002=5396 AND C003=3 AND C004=2 AND C005=2 AND C006=0;
   
    INSERT INTO [dbo].[T_00_007] VALUES (N'1028', N'5396', N'3', N'2', N'2', N'0', N'錄音伺服器編號%1%有%2%個通道未偵測到線路', N'錄音伺服器編號：%1%，錄音伺服器IP：%2%，通道總數：%3%，未偵測到線路總數：%4%，未偵測到線路通道明細：%5%', N'', N'請檢查通道線路是否接好，或者是否鏡像語音封包');
  
  
  DELETE  FROM t_00_007 WHERE C001=1033 AND C002=5396 AND C003=3 AND C004=2 AND C005=1 AND C006=0;
   
    INSERT INTO [dbo].[T_00_007] VALUES (N'1033', N'5396', N'3', N'2', N'1', N'0', N'Recording Server ID: %1%, all channel can be used.', N'Recording Server ID: %1%, Recording Server IP: %2%, all channel can be used.', N'', N'');
  
  
  DELETE  FROM t_00_007 WHERE C001=1033 AND C002=5396 AND C003=3 AND C004=2 AND C005=2 AND C006=0;
   
    INSERT INTO [dbo].[T_00_007] VALUES (N'1033', N'5396', N'3', N'2', N'2', N'0', N'Recording Server ID %1%, %2% channel cannot be used.', N'Recording Server ID: %1%, Recording Server IP: %2%, Total Channel: %3%, Unusable Channel: %4%, Unusable Channel List: %5%', N'', N'Please check on channel physical connection, or IP packet spanning.');
  
  
  DELETE  FROM t_00_007 WHERE C001=1041 AND C002=5396 AND C003=3 AND C004=2 AND C005=1 AND C006=0;
   
    INSERT INTO [dbo].[T_00_007] VALUES (N'1041', N'5396', N'3', N'2', N'1', N'0', N'録音サーバID：%1%全部のチャンネルが正常です。', N'録音サーバID：%1%、録音サーバIP：%2%、全部のチャンネルが正常です。', N'', N'');
  
  
  DELETE  FROM t_00_007 WHERE C001=1041 AND C002=5396 AND C003=3 AND C004=2 AND C005=2 AND C006=0;
   
    INSERT INTO [dbo].[T_00_007] VALUES (N'1041', N'5396', N'3', N'2', N'2', N'0', N'録音サーバID%1%には%2%個チャンネルが使用不可になっています。', N'録音サーバID：%1%、録音サーバIP：%2%、チャンネル合計：%3%、使用不可チャンネル数：%4%、使用不可チャンネルリスト：%5%', N'', N'該当チャネルの配線をご確認ください、またはミラーリングポートのパケットをご確認ください。');
  
  
  DELETE  FROM t_00_007 WHERE C001=2052 AND C002=5396 AND C003=3 AND C004=2 AND C005=1 AND C006=0;
   
    INSERT INTO [dbo].[T_00_007] VALUES (N'2052', N'5396', N'3', N'2', N'1', N'0', N'录音服务器编号：%1%所有通道状态可用', N'录音服务器编号：%1%，录音服务器IP：%2%，所有通道可用', N'', N'');
  
  
  DELETE  FROM t_00_007 WHERE C001=2052 AND C002=5396 AND C003=3 AND C004=2 AND C005=2 AND C006=0;
   
    INSERT INTO [dbo].[T_00_007] VALUES (N'2052', N'5396', N'3', N'2', N'2', N'0', N'录音服务器编号%1%有%2%个通道状态不可用', N'录音服务器编号：%1%，录音服务器IP：%2%，通道总数：%3%，不可用通道数：%4%，不可用通道列表：%5%', N'', N'请检查通道线路是否接好，或者是否镜像数据包');
  