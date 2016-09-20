CREATE PROCEDURE P_11_017 (
   AInParam00          VARCHAR2 := '00000',
   AInParam01          VARCHAR2,
   AInParam02          VARCHAR2,
   AInParam03          VARCHAR2,
   AInParam04          VARCHAR2,
   AInParam05          VARCHAR2,
   AInParam06          VARCHAR2,
   AInParam07          VARCHAR2,
   AInParam08          VARCHAR2,
   AOutErrorNumber     OUT   NUMBER,
   AOutErrorString     OUT   NVARCHAR2
)
IS



   v_err_num     NUMBER;
   v_err_msg     NVARCHAR2 (200);
   v_count       NUMBER;
   v_AInParam00  VARCHAR2 (5);
   v_execsql     varchar2 (4000);

BEGIN

   IF AInParam00 IS NULL
   THEN
      v_AInParam00 := '00000';
   ELSE
      v_AInParam00 := AInParam00;
   END IF;

   v_execsql:='SELECT COUNT(1) FROM T_11_101_'||v_AInParam00||'  A WHERE C001='|| AInParam01;
   EXECUTE IMMEDIATE v_execsql INTO v_count;

   IF v_count=0 THEN
     v_execsql:='INSERT INTO t_11_101_'||v_AInParam00||' (C001,C002,C011,C012,C014,C016,C017,C019,C020) '
              ||'VALUES ('''||AInParam01
              ||''',''1'','''
              ||AInParam02||''','''||AInParam02||''','''||AInParam02||''','''
              ||AInParam03||''','''
              ||AInParam04||''','''
              ||AInParam05||''','''
              ||AInParam06||''')';
     DBMS_OUTPUT.put_line(v_execsql);
     EXECUTE IMMEDIATE v_execsql;

     v_execsql:='INSERT INTO t_11_101_'||v_AInParam00||' (C001,C002,C011,C012) '
              ||'VALUES ('''||AInParam01
              ||''',''92'','''
              ||AInParam07||''','''
              ||AInParam08||''')';
     DBMS_OUTPUT.put_line(v_execsql);
     EXECUTE IMMEDIATE v_execsql;

     v_execsql:='INSERT INTO t_11_101_'||v_AInParam00||' (C001,C002,C011) '
              ||'VALUES ('''||AInParam01
              ||''',''93'','''
              ||AInParam02||''')';
     DBMS_OUTPUT.put_line(v_execsql);
     EXECUTE IMMEDIATE v_execsql;

     COMMIT;
     
   ELSE
     
     v_execsql:='UPDATE t_11_101_'||v_AInParam00
              ||' SET C001='||AInParam01
              ||',C011='''||AInParam02
              ||''',C012='''||AInParam02
              ||''',C014='''||AInParam02
              ||''',C016='''||AInParam03             
              ||''',C017='''||AInParam04
              ||''',C019='''||AInParam05
              ||''',C020='''||AInParam06
              ||''' WHERE C001='||AInParam01
              ||' AND C002=1';
     DBMS_OUTPUT.put_line(v_execsql);
     EXECUTE IMMEDIATE v_execsql;

     v_execsql:='UPDATE t_11_101_'||v_AInParam00
              ||' SET C001='||AInParam01
              ||',C011='''||AInParam07
              ||''',C012='''||AInParam08
              ||''' WHERE C001='||AInParam01
              ||' AND C002=92';
     DBMS_OUTPUT.put_line(v_execsql);
     EXECUTE IMMEDIATE v_execsql;

     v_execsql:='UPDATE t_11_101_'||v_AInParam00
              ||' SET C001='''||AInParam01
              ||''',C011='''||AInParam02
              ||''' WHERE C001='||AInParam01
              ||' AND C002=93';
     DBMS_OUTPUT.put_line(v_execsql);
     EXECUTE IMMEDIATE v_execsql;

     COMMIT;     


   END IF;

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
