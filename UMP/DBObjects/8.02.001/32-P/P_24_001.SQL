CREATE  PROCEDURE P_24_001 (
   ainparam01              VARCHAR2 := '00000',
   ainparam02              NVARCHAR2,
   ainparam03              VARCHAR2,
   ainparam04              VARCHAR2,
   ainparam05              VARCHAR2,
   ainparam06              VARCHAR2,
   ainparam07              VARCHAR2,
   ainparam08              VARCHAR2,
   ainparam09              VARCHAR2,
   ainparam10              VARCHAR2,
   ainparam11              VARCHAR2,
   ainparam12              VARCHAR,
   ainparam13              VARCHAR2,
   ainparam14              VARCHAR2,
   ainparam15              VARCHAR2,
   ainparam16              VARCHAR2,
   ainparam17              VARCHAR2,
   ainparam18              VARCHAR2,
   ainparam19              VARCHAR2,
   ainparam20              VARCHAR2,
   ainparam21              VARCHAR2,
   ainparam22              VARCHAR2,
   ainparam23              VARCHAR2,
   ainparam24              VARCHAR2,
   ainparam25              VARCHAR2,
   ainparam26              VARCHAR2,
   ainparam27              NVARCHAR2, 
   AOutParam01       OUT   VARCHAR2,
   aouterrornumber   OUT   NUMBER,
   aouterrorstring   OUT   NVARCHAR2
)
IS
-- description:    增加加密密钥策略配置表T_24_001
-- create date:    <2015-7-23>
-- =========================================================================================
--AInParam01        租户标识（默认为‘00000’）
--AInParam02        策略名称
--AInParam03        是否启用。'1'、启用；'2'、停用
--AInParam04        策略类型。c、重复执行（自动更新）；u、用户输入
--AInParam05        类型为 “用户输入”的生成密钥时间，必须晚于当前时间。如果值为 20140101000000 表示立即执行
--AInParam06        类型为 “用户输入”时加密密钥。启用该密钥时间为typeutime，数据使用key_normal1加密处理
--AInParam07        自动生成密钥执行周期。d：1天；w：1周；t：1旬；m：1月；u：自定义天数
--AInParam08        持续时间（开始 utc）。同时表示该策略生效时间。now表示立即生效
--AInParam09        持续时间（结束 utc）。unlimited表示无结束日期
--AInParam10        执行周期变量。“typeccycle = w”时   0：周日；1：周一。默认为 0；“typeccycle = u”时   填写实际天数。如：10、30、60、120。
--AInParam11        立即更新密钥后执行周期是否重置。'0'否；'1'是。(只针对 “重复执行周期”类型为 “u：自定义天数”有效)
--AInParam12        密码复杂性校验。'0'、不校验；'1'、校验
--AInParam13        必须包含大写字母。 a ～ z
--AInParam14        包含大写字母个数
--AInParam15        必须包含小写字母。a ~ z
--AInParam16        包含小写字母个数
--AInParam17        必须包含数字。 0 ～ 9
--AInParam18        包含数字个数。
--AInParam19        必须包含特殊字符。如：! $ # %
--AInParam20        包含特殊字符个数。
--AInParam21        在多少个密钥内不能重复
--AInParam22        在多少天密钥内不能重复
--AInParam23        最短长度。
--AInParam24        最长长度
--AInParam25        策略创建人
--AInParam26        策略创建时间 utc
--AInParam27        策略描述

--AOutParam01       输出ID
--AOutErrorNumber   输出ERRID 
--AOutErrorString   输出ERR描述
-- ================================================
--aoutparam01       用户ID
-- ================================================
--流水号 模块号 24 操作码 402
-- ================================================
   v_serialid     VARCHAR2 (19);
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);
   v_AInParam01   VARCHAR2 (5); 
   v_ainparam03   CHAR (1);
   v_AInParam04   CHAR (1); 
   v_AInParam05   VARCHAR2 (20); 
   v_AInParam07   CHAR (1);
   v_AInParam08   VARCHAR2 (20); 
   v_AInParam09   VARCHAR2 (20);        
   v_AInParam10   VARCHAR2 (5);
   v_AInParam11   CHAR (1);
   v_AInParam12   CHAR (1); 
   v_AInParam13   CHAR (1); 
   v_AInParam14   VARCHAR2 (5);          
   v_AInParam15   CHAR (1); 
   v_AInParam16   VARCHAR2 (5); 
   v_AInParam17   CHAR (1);
   v_AInParam18   VARCHAR2 (5);
   v_AInParam19   CHAR (1); 
   v_AInParam20   VARCHAR2 (5); 
   v_AInParam21   VARCHAR2 (5);                   
   v_ainparam22   VARCHAR2 (5);    
   v_AInParam23   VARCHAR2 (5); 
   v_AInParam24   VARCHAR2 (5);                   
   v_ainparam25   VARCHAR2 (20);  
   v_ainparam26   VARCHAR2 (20);                  
   v_execsql      VARCHAR2 (4000);
   v_datetime     VARCHAR2 (14);
BEGIN
   
   IF AInParam01 IS NULL THEN
     v_AInParam01:='00000';
   ELSE
     v_AInParam01:=AInParam01;
   END IF;

   IF ainparam03 IS NULL
   THEN
      v_ainparam03 := '1';
   ELSE
      v_ainparam03 := ainparam03;
   END IF;
   

   IF ainparam04 IS NULL
   THEN
      v_ainparam04 := 'U';
   ELSE
      v_ainparam04 := ainparam04;
   END IF;  
    
   IF ainparam05 IS NULL
   THEN
      v_ainparam05 := '20140101000000';
   ELSE
      v_ainparam05 := ainparam05;
   END IF;  
     
   IF ainparam07 IS NULL
   THEN
      v_ainparam07 := 'M';
   ELSE
      v_ainparam07 := ainparam07;
   END IF; 
   
   IF ainparam10 IS NULL
   THEN
      v_ainparam10 := '0';
   ELSE
      v_ainparam10 := ainparam10;
   END IF;
   
   IF ainparam11 IS NULL
   THEN
      v_ainparam11 := '1';
   ELSE
      v_ainparam11 := ainparam11;
   END IF; 
   
   IF ainparam12 IS NULL
   THEN
      v_ainparam12 := '1';
   ELSE
      v_ainparam12 := ainparam12;
   END IF;          
 
   IF ainparam13 IS NULL
   THEN
      v_ainparam13 := '1';
   ELSE
      v_ainparam13 := ainparam13;
   END IF; 
   
   IF ainparam14 IS NULL
   THEN
      v_ainparam14 := '0';
   ELSE
      v_ainparam14 := ainparam14;
   END IF; 
      
   IF ainparam15 IS NULL
   THEN
      v_ainparam15 := '1';
   ELSE
      v_ainparam15 := ainparam12;
   END IF; 
   
   IF ainparam16 IS NULL
   THEN
      v_ainparam16 := '0';
   ELSE
      v_ainparam16 := ainparam16;
   END IF; 
   
   IF ainparam17 IS NULL
   THEN
      v_ainparam17 := '1';
   ELSE
      v_ainparam17 := ainparam17;
   END IF; 
         
   IF ainparam18 IS NULL
   THEN
      v_ainparam18 := '0';
   ELSE
      v_ainparam18 := ainparam18;
   END IF; 
   
   IF ainparam19 IS NULL
   THEN
      v_ainparam19 := '0';
   ELSE
      v_ainparam19 := ainparam19;
   END IF; 
   
   IF ainparam20 IS NULL
   THEN
      v_ainparam20 := '0';
   ELSE
      v_ainparam20 := ainparam20;
   END IF; 
   
   IF ainparam21 IS NULL
   THEN
      v_ainparam21 := '0';
   ELSE
      v_ainparam21 := ainparam21;
   END IF; 
   
   IF ainparam22 IS NULL
   THEN
      v_ainparam22 := '0';
   ELSE
      v_ainparam22 := ainparam22;
   END IF; 
   
   IF ainparam23 IS NULL
   THEN
      v_ainparam23 := '6';
   ELSE
      v_ainparam23 := ainparam23;
   END IF; 
        
   IF ainparam24 IS NULL
   THEN
      v_ainparam24 := '64';
   ELSE
      v_ainparam24 := ainparam24;
   END IF; 
   
   IF ainparam25 IS NULL
   THEN
      v_ainparam25 := '0';
   ELSE
      v_ainparam25 := ainparam25;
   END IF; 
   
   
        
   v_ainparam05:=replace(v_ainparam05,'-');
   v_ainparam05:=replace(v_ainparam05,' ');
   v_ainparam05:=replace(v_ainparam05,':');
   
   v_ainparam08:=replace(ainparam08,'-');
   v_ainparam08:=replace(v_ainparam08,' ');
   v_ainparam08:=replace(v_ainparam08,':'); 
      
   v_ainparam09:=replace(ainparam09,'-');
   v_ainparam09:=replace(v_ainparam09,' ');
   v_ainparam09:=replace(v_ainparam09,':'); 
                                
   v_ainparam26:=replace(ainparam26,'-');
   v_ainparam26:=replace(v_ainparam26,' ');
   v_ainparam26:=replace(v_ainparam26,':');           
   
   SELECT to_char(sysdate,'yyyymmddhh24miss') INTO v_datetime FROM DUAL;

   P_00_001 ('24', '402', v_ainparam01, v_datetime, v_serialid, v_err_num,
             v_err_msg);

   IF v_serialid < 0 OR v_err_num <> 0
   THEN
      aouterrornumber := -1;
      aouterrorstring := 'Serial number generation error';
      RETURN;
   END IF;

   v_execsql :=
         'INSERT INTO T_24_001'
      || ' VALUES ('''
      || v_AInParam01
      || ''','''
      || v_serialid
      || ''','''
      || ainparam02
      || ''', '''
      || v_ainparam03
      || ''','''
      || v_ainparam04
      || ''','''
      || v_ainparam05
      || ''','''
      || ainparam06
      || ''', '''
      || v_ainparam07
      || ''','''
      || v_ainparam08
      || ''','''
      || v_ainparam09
      || ''','''
      || v_ainparam10
      || ''','''
      || v_ainparam11
      || ''','''
      || v_ainparam12
      || ''','''
      || v_ainparam13
      || ''','''
      || v_ainparam14
      || ''','''
      || v_ainparam15
      || ''','''
      || v_ainparam16
      || ''','''
      || v_ainparam17
      || ''','''
      || v_ainparam18
      || ''','''
      || v_ainparam19
      || ''','''
      || v_ainparam20
      || ''','''
      || v_ainparam21
      || ''','''
      || v_ainparam22
      || ''','''
      || v_ainparam23
      || ''','''
      || v_ainparam24
      || ''',0,'''','''','''
      || v_ainparam25
      || ''','''
      ||v_ainparam26
      ||''','''','''||v_ainparam26||''','''','''','''','''
      || ainparam27
      ||''')';

   dbms_output.put_line (v_execsql);

   EXECUTE IMMEDIATE v_execsql;

   COMMIT;
   AOutParam01:=v_serialid;
   aouterrornumber := '0';
   aouterrorstring := 'successful';
EXCEPTION
   WHEN OTHERS
   THEN
      ROLLBACK;
      v_err_num := SQLCODE;
      v_err_msg := SUBSTR (SQLERRM, 1, 200);
      aouterrornumber := v_err_num;
      aouterrorstring := v_err_msg;
END;
