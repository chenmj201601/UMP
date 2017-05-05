CREATE OR REPLACE PROCEDURE P_11_001 (
   AInParam01          VARCHAR2 := '00000',
   AInParam02          VARCHAR2,
   AOutErrorNumber     OUT   NUMBER,
   AOutErrorString     OUT   NVARCHAR2
)
IS


   v_ainparam01  VARCHAR2(5);
   v_token       VARCHAR2(5);
   v_userid      VARCHAR2(20);
   v_err_num     NUMBER;
   v_err_msg     NVARCHAR2 (200);
   v_execsql     VARCHAR2 (500);
BEGIN

   IF ainparam01 IS NULL THEN
     v_ainparam01:='00000';
   ELSE
     v_ainparam01:=ainparam01;
   END IF;
   
   P_11_002 (AInParam01, AInParam02, v_token,v_err_num,v_err_msg);     

   IF v_token ='0' OR v_err_num <> 0
   THEN
      AOuterrornumber := -1;
      AOuterrorstring := 'EXCE P_11_002 FAIL';
      RETURN;
   END IF;
     
   begin
     v_execsql:='SELECT C001 FROM T_11_005_'||v_ainparam01||' WHERE C006='||'10114010100000'||v_token||' AND C027=''1''';
     EXECUTE IMMEDIATE v_execsql INTO v_userid;
   exception
     when others then null;
   end;
   
   if (v_userid is null) then
     AOuterrornumber := '-1';
     AOuterrorstring := 'Can not find the parent USERID.';     
     return;
   end if;
   
   v_execsql:='INSERT INTO T_11_201_'||v_ainparam01||' values(0,0,'||v_userid||','||AInParam02||',sysdate,TO_DATE(''2199-12-31'',''YYYY-MM-DD HH24:MI:SS''))';
   dbms_output.put_line(v_execsql);   
   EXECUTE IMMEDIATE v_execsql;
   COMMIT;

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
