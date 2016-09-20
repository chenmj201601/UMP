CREATE  PROCEDURE P_21_002
(
  AInParam01      IN NVARCHAR2:='00000',
  AInParam02      IN NVARCHAR2,
  AInParam03      IN NVARCHAR2,
  AInParam04      IN NVARCHAR2,
  result out sys_refcursor,
  AOutErrorNumber OUT NUMBER,
  AOutErrorString OUT NVARCHAR2)
AS
   v_count        INT;
   v_year         VARCHAR(2);
   v_month	      VARCHAR(2);
   v_tablename	  VARCHAR(30);
   v_execsql      VARCHAR(4000);
   begin_time     VARCHAR(20);
   end_time       VARCHAR(20);
   subcall        VARCHAR2(32);
   v_ip           varchar2(128);
-- description:    根据流水号的C002找开始时间-1S和结束时间+1S所对应的录屏为2的记录返回
-- create date:    <2015-8-11>
-- =========================================================================================
--ainparam01        租户ID（默认为‘00000’）
--ainparam02        UTC时间
--ainparam03        C002流水号
--ainparam04        预留
--============================================
BEGIN
   
     AOutErrorNumber := '0';
     AOutErrorString := 'successful';

     v_year:=SUBSTR(ainparam02,3,2);
     v_month:=SUBSTR(ainparam02,6,2);
     v_tablename:='T_21_001_'||AInParam01||'_'||v_year||v_month;


     v_execsql :='SELECT COUNT(1)  FROM  USER_TABLES  WHERE TABLE_NAME='''||v_tablename||'''';
     execute immediate v_execsql into v_count ;

     IF v_count=0 THEN
       v_tablename:='T_21_001_'||AInParam01;
     END IF;


     v_execsql :='SELECT TO_CHAR(C005-1/(24*60*60),''YYYY-MM-DD HH24:MI:SS'') AS C005 ,TO_CHAR(C009+1/(24*60*60),''YYYY-MM-DD HH24:MI:SS'') AS C009,C042,C020 FROM '||v_tablename||' WHERE C002='||ainparam03||' AND C014=1';
	   DBMS_OUTPUT.put_line(v_execsql);
     BEGIN
       execute immediate v_execsql INTO begin_time,end_time,subcall,v_ip;
	     DBMS_OUTPUT.put_line(subcall);
	     DBMS_OUTPUT.put_line(v_ip);        
            
     EXCEPTION
       when others then null;
     END; 
	 
	   v_execsql :='SELECT *  FROM '||v_tablename||' WHERE C014=2 AND C005>=TO_DATE('''||begin_time
     ||''',''YYYY-MM-DD HH24:MI:SS'') AND C009<=TO_DATE('''||end_time
     ||''',''YYYY-MM-DD HH24:MI:SS'') and C042='''||subcall||''' and c020='''||v_ip||'''';
      
	   DBMS_OUTPUT.put_line(v_execsql);    
     open result for v_execsql;   
  

EXCEPTION
  WHEN OTHERS THEN
    AOuterrornumber := SQLCODE;
    AOuterrorstring := substr(SQLERRM,1,200);
END;
