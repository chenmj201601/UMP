CREATE  PROCEDURE P_31_002 (
   AInParam01              VARCHAR2 := '00000',   
   AInParam02              VARCHAR2 := 'I',
   AInParam03    IN OUT    VARCHAR2,
   AInParam04              VARCHAR2,
   AInParam05              VARCHAR2,
   AInParam06              VARCHAR2,
   AInParam07              VARCHAR2,
   AInParam08              NVARCHAR2,
   AInParam09              NVARCHAR2,
   AInParam10              CHAR     :='A',
   AInParam11              CHAR     :='1',
   AInParam12              VARCHAR2,
   AInParam13              VARCHAR2,
   AInParam14              VARCHAR2,
   AInParam15              VARCHAR2,
   AInParam16              VARCHAR2,
   AInParam17              VARCHAR2,
   AInParam18              VARCHAR2 :='0',     
   AInParam19              CHAR, 
   AInParam20              VARCHAR2 :='0',    
   AInParam21              CHAR:='1',         
   AOutErrornumber   OUT   NUMBER,
   AOutErrorstring   OUT   NVARCHAR2
)
IS
-- description:    增、删、改表T_31_042_00000录音记录的BookMark
-- create date:    <2014-11-05>
-- =====================================================================
--AInParam01        租户标识（默认为‘00000’）
--AInParam02        操作标识 I：增加 D：删除 U：修改
--AInParam03        Book Mark 流水号
--AInParam04        录音记录流水号.C001
--AInParam05        录音记录流水号.C002
--AInParam06        OffsetTime.与起点的偏移量，单位：毫秒
--AInParam07        时长。
--AInParam08        主题
--AInParam09        内容
--AInParam10        共享方式 默认为'A'
--AInParam11        状态。'0'删除，'1'正常
--AInParam12        标注人
--AInParam13        标注时间 yyyyMMddHHmmss
--AInParam14        最后修改人
--AInParam15        最后修改时间 yyyyMMddHHmmss
--AInParam16        删除人
--AInParam17        删除时间 yyyyMMddHHmmss
--AInParam18        级别 默认0
--AInParam19        来源.'C'从CQC写入，'U'从UMP写入
--AInParam20        信誉值
--AInParam21        通道号

--操作码记录在182上的UMPDevInfo的T_Type_List中
-- =====================================================================
--AoutParam01       输出流水号
-- =====================================================================
   v_serialid     VARCHAR2 (19);
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);
   v_AInParam01   VARCHAR2 (5);
   v_AInParam02   VARCHAR2 (1);
   v_AInParam10   VARCHAR2 (1);
   v_AInParam11   VARCHAR2 (1);
   v_AInParam18   VARCHAR2 (20); 
   v_AInParam20   VARCHAR2(11); 
   v_AInParam21   VARCHAR2 (1);       
   v_execsql      VARCHAR2 (1024);
   v_datetime     VARCHAR2 (19);

BEGIN
 
   IF AInParam01 IS NULL
   THEN
      v_AInParam01 := '00000';
   ELSE
      v_AInParam01 := AInParam01;
   END IF;

   IF AInParam02 IS NULL
   THEN
      v_AInParam02 :='I';
   ELSE
      v_AInParam02 := AInParam02;
   END IF;
   
   IF v_AInParam02 not in ('I','D','U') THEN
        AOuterrornumber := '-1';
        AOuterrorstring := 'AInParam02 is illegal';
        RETURN;
   END IF;   

   IF AInParam10 IS NULL
   THEN
      v_AInParam10 := 'A';
   ELSE
      v_AInParam10 := AInParam10;
   END IF;

   IF AInParam11 IS NULL
   THEN
      v_AInParam11 := '1';
   ELSE
      v_AInParam11 := AInParam11;
   END IF;

   IF AInParam18 IS NULL
   THEN
      v_AInParam18 := '0';
   ELSE
      v_AInParam18 := AInParam18;
   END IF;
   
   IF AInParam20 IS NULL
   THEN
      v_AInParam20 := '0';
   ELSE
      v_AInParam20 := AInParam20;
   END IF;
   
   IF AInParam21 IS NULL
   THEN
      v_AInParam21 := '1';
   ELSE
      v_AInParam21 := AInParam21;
   END IF;
      
   --如果操作编码是I
   IF v_AInParam02='I'  THEN
     
          SELECT to_char(sysdate,'yyyymmddhh24miss') INTO v_datetime FROM DUAL;
            
          P_00_001 ('31', '304', v_AInParam01, v_datetime, v_serialid, v_err_num,--模块号是31 操作码是142
          v_err_msg);

          IF v_serialid<0 OR v_err_num <>0 THEN
            AOuterrornumber := '-1';
            AOuterrorstring := 'Serial number generation error';
            RETURN;
          END IF;

          v_execsql :=
          'INSERT INTO T_31_042_'
          || v_AInParam01
          || '
          VALUES ('''
          || v_serialid
          || ''','''
          || AInParam04
          || ''','''
          || AInParam05
          || ''','''
          || AInParam06
          || ''','''
          || AInParam07
          || ''', '''
          || AInParam08
          || ''','''
          || AInParam09
          || ''','''
          || v_AInParam10
          || ''','''
          || v_AInParam11
          || ''','''
          || AInParam12
          || ''','''
          || AInParam13
          || ''','''
          || AInParam14
          || ''','''
          || AInParam15
          || ''','''
          || AInParam16
          || ''','''
          || AInParam17
          || ''','''
          || v_AInParam18
          || ''','''
          || AInParam19 
          || ''','''
          || v_AInParam20
          || ''','''
          || v_AInParam21                                                                                                               
          || ''')';

         
          EXECUTE IMMEDIATE v_execsql;

          COMMIT;
          
          AInParam03:=v_serialid;
          
    END IF;
   --如果操作编码是D    
    IF  v_AInParam02='D'  THEN
          v_execsql :=
          'DELETE T_31_042_'
          || v_AInParam01
          || '
          WHERE C001='||AInParam03;

          --DBMS_OUTPUT.put_line (v_execsql);
          EXECUTE IMMEDIATE v_execsql;

          COMMIT;
    END IF;
   --如果操作编码是U                       
    IF  v_AInParam02='U'  THEN
   
          v_execsql :=
          'UPDATE T_31_042_'
          || v_AInParam01
          || '
          SET  C002='''
          || AInParam04
          || ''',C003='''
          || AInParam05           
          || ''',C004='''
          || AInParam06          
          || ''',C005='''
          || AInParam07
          || ''',C006='''
          || AInParam08
          || ''',C007='''
          || AInParam09
          || ''',C008='''
          || AInParam10
          || ''',C009='''
          || AInParam11
          || ''',C010='''
          || AInParam12
          || ''',C011='''
          || AInParam13
          || ''',C012='''
          || AInParam14
          || ''',C013='''
          || AInParam15
          || ''',C014='''
          || AInParam16
          || ''',C015='''
          || AInParam17
          || ''',C016='''
          || AInParam18
          || ''',C017='''
          || AInParam19
          || ''',C018='''
          || v_AInParam20
          || ''',C019='''
          || v_AInParam21                                                                                                         
          || ''' WHERE C001='||AInParam03;

          --DBMS_OUTPUT.put_line (v_execsql);
          EXECUTE IMMEDIATE v_execsql;

          COMMIT;
 
   END IF;
   
   AOutErrornumber := '0';
   AOutErrorstring := 'successful';

EXCEPTION
   WHEN OTHERS
   THEN
      ROLLBACK;
      v_err_num := SQLCODE;
      v_err_msg := SUBSTR (SQLERRM, 1, 200);
      aouterrornumber := v_err_num;
      aouterrorstring := v_err_msg;
END;
