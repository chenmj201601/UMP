CREATE  PROCEDURE P_24_006
( AOutParam01     OUT  nvarchar2, 
  AOutParam02     OUT  date,
  AOutErrorNumber OUT  int,
  AOutErrorString OUT  nvarchar2 
)
IS
  v_sql varchar(2000);
  --取得服务器时间
BEGIN
	AOutErrorNumber:= 0;
  AOutErrorString:= '';

  v_sql:='select to_char(F_00_004,''yyyy-mm-dd hh24:mi:ss''),F_00_004 from dual';
  execute immediate v_sql into AOutParam01,AOutParam02;
  commit;

EXCEPTION
   WHEN OTHERS
   THEN
      ROLLBACK;
      AOuterrornumber := SQLCODE;
      AOuterrorstring := SUBSTR (SQLERRM, 1, 200);
END;
