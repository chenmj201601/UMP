CREATE OR REPLACE PROCEDURE P_31_006 (
   AInParam00              VARCHAR2 := '00000',
   AInParam01              VARCHAR2,
   AInParam02              VARCHAR2,
   AInParam03              VARCHAR2,   
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
   
   v_C004         VARCHAR2 (5);
   v_C005         VARCHAR2 (5);
   v_C017         VARCHAR2 (5);

BEGIN
   IF AInParam00 IS NULL
   THEN
      v_AInParam00 := '00000';
   ELSE
      v_AInParam00 := AInParam00;
   END IF;
   
   v_C004:='';
   v_C005:='';
   v_C017:='';
   
   IF (AInParam01='W' AND (TO_NUMBER(AInParam02)<1 OR TO_NUMBER(AInParam02)>7)) 
   THEN
     AOuterrornumber := -1;
     AOuterrorstring := 'Date parameter error';
     RETURN;    
   END IF;

   IF (AInParam01='M' AND (TO_NUMBER(AInParam02)<1 OR TO_NUMBER(AInParam02)>31)) 
   THEN
     AOuterrornumber := -1;
     AOuterrorstring := 'Date parameter error';
     RETURN;    
   END IF;  
    
   IF (AInParam01='Y' AND (TO_NUMBER(AInParam02)<1 OR TO_NUMBER(AInParam02)>366)) 
   THEN
     AOuterrornumber := -1;
     AOuterrorstring := 'Date parameter error';
     RETURN;    
   END IF;  
            
   IF AInParam01='W' THEN 
     v_C004:=AInParam02;
   ELSIF AInParam01='M' THEN 
     v_C005:=AInParam02;
   ELSIF AInParam01='Y' THEN 
     v_C017:=AInParam02;
   END IF;
        
   SELECT to_char(sysdate,'yyyymmddhh24miss') INTO v_datetime FROM DUAL;
   P_00_001 ('31', '313', v_AInParam00,v_datetime,v_serialid, v_err_num,
             v_err_msg);

   IF v_serialid<0 OR v_err_num <>0 THEN
     AOuterrornumber := -1;
     AOuterrorstring := 'Serial number generation error';
     RETURN;
   END IF;

  v_execsql :=
         'INSERT INTO t_31_026_'
      || v_AInParam00
      || '(C001,C002,C003,C004,C005,C017)
            VALUES ('''
      || v_serialid
      || ''','''
      || AInParam01
      || ''','''
      || AInParam03
      || ''','''
      || v_C004
      || ''','''
      || v_C005
      || ''','''      
      || v_C017
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
