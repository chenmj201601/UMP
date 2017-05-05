CREATE PROCEDURE P_11_014 (
   AInParam01          VARCHAR2 := '00000',
   AInParam02          VARCHAR2,
   AInParam03          VARCHAR2,
   AInParam04          NVARCHAR2,
   AInParam05          CHAR := '1',
   AInParam06          CHAR := '0',
   AInParam07          VARCHAR2 := '11111111111111111111111111111111',
   AInParam08          VARCHAR2,
   AInParam09          VARCHAR2,
   AInParam10          VARCHAR2,
   AInParam11          VARCHAR2,
   AoutParam01         OUT   VARCHAR2,
   AOutErrorNumber     OUT   NUMBER,
   AOutErrorString     OUT   NVARCHAR2
)
IS


   v_serialid    VARCHAR2 (19);
   v_err_num     NUMBER;
   v_err_msg     NVARCHAR2 (200);
   v_AInParam01  VARCHAR2 (5);
   v_AInParam05  VARCHAR2 (1);
   v_AInParam06  VARCHAR2 (1);
   v_AInParam07  VARCHAR2 (32);
   v_AInParam11  VARCHAR2 (30);   
   v_execsql     VARCHAR2 (4000);
   v_datetime    VARCHAR2 (19);
BEGIN
  
   IF AInParam01 IS NULL THEN
     v_AInParam01:='00000';
   ELSE
     v_AInParam01:=AInParam01;
   END IF;

   IF AInParam05 IS NULL THEN
     v_AInParam05:='1';
   ELSE
     v_AInParam05:=AInParam05;
   END IF;

   iF AInParam06 IS NULL THEN
     v_AInParam06:='0';
   ELSE
     v_AInParam06:=AInParam06;
   END IF;

   iF AInParam07 IS NULL THEN
     v_AInParam07:='11111111111111111111111111111111';
   ELSE
     v_AInParam07:=AInParam07;
   END IF;
   
   IF AInParam11 IS NULL
   THEN
      v_AInParam11 := to_char(SYSDATE,'yyyy-mm-dd hh24:mi:ss');
   ELSE
      v_AInParam11 := AInParam11;
   END IF;
   
   SELECT to_char(sysdate,'yyyymmddhh24miss') INTO v_datetime FROM DUAL;    
   
   P_00_001 ('11', '106', v_AInParam01, v_datetime, v_serialid, v_err_num, v_err_msg); 
   
   IF v_serialid<0 OR v_err_num <>0 THEN
     AOuterrornumber := -1;
     AOuterrorstring := 'Serial number generation error';     
     RETURN;
   END IF;   

   v_execsql:='INSERT INTO t_11_004_'||v_AInParam01||'
            VALUES ('''||v_serialid||''','''||AInParam02||''', '''||AInParam03||''','''||AInParam04||''','''||v_AInParam05||''',
                '''||v_AInParam06||''', '''||v_AInParam07||''','''||AInParam08||''','''||AInParam09||''','''||AInParam10||''','||
                'TO_DATE('''||v_AInParam11||''',''yyyy-mm-dd hh24:mi:ss''))';

   EXECUTE IMMEDIATE v_execsql;
   COMMIT;
   aoutparam01 := TO_NUMBER(v_serialid);

   AOuterrornumber := '0';
   AOuterrorstring := 'successful';
EXCEPTION
   WHEN OTHERS
   THEN
      ROLLBACK;
      v_err_num := SQLCODE;
      v_err_msg := SUBSTR (SQLERRM, 1, 200);
      AOuterrornumber := v_err_num;
      AOuterrorstring := v_err_msg;
END;