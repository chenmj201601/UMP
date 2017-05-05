CREATE OR REPLACE PROCEDURE P_51_001
( AInParam01 VARCHAR2,
  result out sys_refcursor,
  AOutErrorNumber OUT int ,
  AOutErrorString OUT nvarchar2
)

AS


   v_count        INT;
   v_year         VARCHAR(2);
   v_month        VARCHAR(2);
   v_tablename    VARCHAR(30);
   v_execsql      VARCHAR(4000);
BEGIN

  AOutErrorNumber:= 0;
  AOutErrorString := 'successful';

  v_year:=SUBSTR(AInParam01,-15,2);
  v_month:=SUBSTR(AInParam01,-13,2);
  v_tablename:='T_21_001_00000'||'_'||v_year||v_month;


  v_execsql :='SELECT COUNT(1)  FROM  USER_TABLES  WHERE TABLE_NAME='''||v_tablename||'''';
  EXECUTE IMMEDIATE v_execsql into v_count ;

  IF v_count=0 THEN
       v_tablename:='T_21_001_00000';
  END IF;


  DELETE FROM T_99_007;

  v_execsql :='INSERT INTO T_99_007
        SELECT C001,C002,C003,C005,C006,C007,C009,C010,C011,C012,C019,C020,C021,C039,C040,C041,C042,'''||v_tablename
        ||''',C045,C058  FROM '||v_tablename||' WHERE C077='''||AInParam01||'''';

  dbms_output.put_line(v_execsql);
  EXECUTE IMMEDIATE v_execsql;

  COMMIT;

  OPEN result FOR select * from T_99_007 ;

EXCEPTION
   WHEN OTHERS
   THEN
      ROLLBACK;
      AOuterrornumber := SQLCODE;
      AOuterrorstring := SUBSTR (SQLERRM, 1, 200);
END;
