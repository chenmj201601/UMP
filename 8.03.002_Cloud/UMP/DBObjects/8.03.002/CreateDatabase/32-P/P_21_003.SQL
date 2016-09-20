CREATE  PROCEDURE P_21_003 (
   ainparam01        IN       VARCHAR2 := '00000',
   ainparam02        IN       VARCHAR2,
   ainparam03        IN       VARCHAR2,
   ainparam04        IN       VARCHAR2,
   ainparam05        IN       VARCHAR2,
   ainparam06        IN       VARCHAR2,
   ainparam07        IN       VARCHAR2,
   AOutParam01       OUT VARCHAR2,   
   aouterrornumber   OUT      NUMBER,
   aouterrorstring   OUT      NVARCHAR2
)
AS
   v_tablename   VARCHAR (30);
   v_execsql     VARCHAR (4000);
   query_columlist varchar(4000); 

BEGIN
   aouterrornumber := '0';
   aouterrorstring := 'successful';
   if ainparam01 is not null then
     v_tablename := 'T_21_101_' || ainparam01;
   else
     v_tablename := 'T_21_101';
   end if;
   query_columlist:=replace(ainparam07,',','+');
   
   if ainparam03 is not null then
     
     v_execsql :=
           'SELECT nvl(sum('
        || query_columlist
        || '),0) from '||v_tablename
        || ' WHERE C001='
        || ainparam02
        || ' AND C002>='
        || ainparam04      
        || ' AND C002<='
        || ainparam05
        || ' AND C003='''
        || ainparam06           
        || ''' AND C004 in (select c011 from T_00_901 where c001='
        || ainparam03||')';      

   else 
           
     v_execsql :=
           'SELECT nvl(sum('
        || query_columlist
        || '),0) from '||v_tablename
        || ' WHERE C001='
        || ainparam02
        || ' AND C002>='
        || ainparam04      
        || ' AND C002<='
        || ainparam05
        || ' AND C003='''
        || ainparam06           
        || '''';          
   end if;
   dbms_output.put_line(v_execsql);
   EXECUTE IMMEDIATE  v_execsql into  AOutParam01;        

EXCEPTION
   WHEN OTHERS
   THEN
      aouterrornumber := SQLCODE;
      aouterrorstring := SUBSTR (SQLERRM, 1, 200);
END;
