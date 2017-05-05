--添加参数，告警的ChannelDisconnect peng 20160523

  DELETE  FROM T_00_011 WHERE C001=289 AND C002=2;
  
    INSERT INTO [T_00_011] 
    VALUES ('289', '2', '0', '2', '1', 'group.png', 'ChannelDisconnect', '1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'ChannelDisconnect');
  

  DELETE  FROM T_00_009 WHERE C001=289 AND C002=21;
    
    INSERT INTO [T_00_009] 
    VALUES ('289', '21', '2', '1', NULL, '201', '2', '1', '1', '0', NULL, NULL, NULL, NULL, NULL, ('2016-1-26'), '1020000000000000001', N'是否启用', '0', 'Enable', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
  

  DELETE  FROM T_00_009 WHERE C001=289 AND C002=22;
  
    INSERT INTO [T_00_009] 
      VALUES ('289', '22', '2', '1', NULL, '101', '2', '2', '60', '0', NULL, NULL, NULL, NULL, NULL, ('2016-1-26'), '1020000000000000001', N'检测的间隔时间', '0', 'Interval', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
  