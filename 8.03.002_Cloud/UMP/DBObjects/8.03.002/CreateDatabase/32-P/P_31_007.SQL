CREATE OR REPLACE PROCEDURE P_31_007 (
   AInParam00              VARCHAR2 := '00000',
   AInParam02              VARCHAR2,
   AInParam03              VARCHAR2,
   AInParam04              VARCHAR2,
   AInParam05              VARCHAR2,
   AInParam06              VARCHAR2,
   AInParam07              VARCHAR2,
   AInParam08              VARCHAR2,
   AInParam09              VARCHAR2,
   AInParam10              VARCHAR2,
   AInParam11              VARCHAR2,         
   AOutParam01       OUT   VARCHAR2,
   AOutErrornumber   OUT   NUMBER,
   AOutErrorstring   OUT   NVARCHAR2
)
IS

   v_datetime     VARCHAR2(14);
   v_serialid     VARCHAR2 (19);
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);
   v_AInParam00   VARCHAR2 (5);
   v_execsql      VARCHAR2 (1024);


BEGIN
   IF AInParam00 IS NULL
   THEN
      v_AInParam00 := '00000';
   ELSE
      v_AInParam00 := AInParam00;
   END IF;


   SELECT to_char(sysdate,'yyyymmddhh24miss') INTO v_datetime FROM DUAL;
   P_00_001 ('31', '312', v_AInParam00,v_datetime,v_serialid, v_err_num,
             v_err_msg);

   IF v_serialid<0 OR v_err_num <>0 THEN
     AOuterrornumber := -1;
     AOuterrorstring := 'Serial number generation error';
     RETURN;
   END IF;

  v_execsql :=
         'INSERT INTO t_31_052_'
      || v_AInParam00
      || '  VALUES ('''
      || v_serialid
      || ''','''
      || AInParam02
      || ''','''
      || AInParam03
      || ''','''
      || AInParam04
      || ''','''
      || AInParam05
      || ''',TO_DATE('''
      || AInParam06
      || ''',''yyyy-mm-dd hh24:mi:ss'')'
      || ',TO_DATE('''
      || ainparam07
      || ''',''yyyy-mm-dd hh24:mi:ss''),'''
      || AInParam08
      || ''','''
      || AInParam09
      || ''','''
      || AInParam10
      || ''','''
      || AInParam11                            
      || ''')';

   DBMS_OUTPUT.put_line (v_execsql);
   EXECUTE IMMEDIATE v_execsql;
   COMMIT;

   AOutParam01 := v_serialid;
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
