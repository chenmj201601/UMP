CREATE OR REPLACE PROCEDURE p_21_002 (
   ainparam01        IN       NVARCHAR2 := '00000',
   ainparam02        IN       NVARCHAR2,
   ainparam03        IN       NVARCHAR2,
   ainparam04        IN       NVARCHAR2,
   RESULT            OUT      sys_refcursor,
   aouterrornumber   OUT      NUMBER,
   aouterrorstring   OUT      NVARCHAR2
)
AS
   v_count       INT;
   v_year        VARCHAR (2);
   v_month       VARCHAR (2);
   v_tablename   VARCHAR (30);
   v_execsql     VARCHAR (4000);
   begin_time    VARCHAR (20);
   end_time      VARCHAR (20);
   subcall       VARCHAR2 (32);
   v_ip          VARCHAR2 (128);

BEGIN
   aouterrornumber := '0';
   aouterrorstring := 'successful';
   v_year := SUBSTR (ainparam02, 3, 2);
   v_month := SUBSTR (ainparam02, 6, 2);
   v_tablename := 'T_21_001_' || ainparam01 || '_' || v_year || v_month;
   v_execsql :=
         'SELECT COUNT(1)  FROM  USER_TABLES  WHERE TABLE_NAME='''
      || v_tablename
      || '''';

   EXECUTE IMMEDIATE v_execsql
                INTO v_count;

   IF v_count = 0
   THEN
      v_tablename := 'T_21_001_' || ainparam01;
   END IF;

   v_execsql :=
         'SELECT TO_CHAR(C005-2/(24*60*60),''YYYY-MM-DD HH24:MI:SS'') AS C005 ,TO_CHAR(C009+2/(24*60*60),''YYYY-MM-DD HH24:MI:SS'') AS C009,C042,C020 FROM '
      || v_tablename
      || ' WHERE C002='
      || ainparam03
      || ' AND C014=1';
   DBMS_OUTPUT.put_line (v_execsql);

   BEGIN
      EXECUTE IMMEDIATE v_execsql
                   INTO begin_time, end_time, subcall, v_ip;

      DBMS_OUTPUT.put_line (subcall);
      DBMS_OUTPUT.put_line (v_ip);
   EXCEPTION
      WHEN OTHERS
      THEN
         NULL;
   END;

   v_execsql :=
         'SELECT *  FROM '
      || v_tablename
      || ' WHERE  C042='''
      || subcall
      || ''' AND C005>=TO_DATE('''
      || begin_time
      || ''',''YYYY-MM-DD HH24:MI:SS'') AND C005<=TO_DATE('''
      || end_time
      || ''',''YYYY-MM-DD HH24:MI:SS'') and (C014=2 or C014=3)';
   DBMS_OUTPUT.put_line (v_execsql);

   OPEN RESULT FOR v_execsql;
EXCEPTION
   WHEN OTHERS
   THEN
      aouterrornumber := SQLCODE;
      aouterrorstring := SUBSTR (SQLERRM, 1, 200);
END; 
