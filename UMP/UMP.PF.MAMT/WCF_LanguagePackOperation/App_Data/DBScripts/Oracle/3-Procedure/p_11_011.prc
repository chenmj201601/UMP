CREATE OR REPLACE PROCEDURE p_11_011 (
   AInParam01          VARCHAR2 := '00000',
   AInParam02          VARCHAR2,
   AOutErrorNumber     OUT   NUMBER,
   AOutErrorString     OUT   NVARCHAR2
)
IS

-- description:    删除t_11_006_00000机构下的所有记录
-- create date:    <2014-9-2>
-- ===============================================================
--ainparam01        租户ID（默认为‘00000’）
--ainparam02        机构ID (将此机构及其下面子机构所列的资源t_11_201 及 机构t_11_006本身 全部删除)

   v_ainparam01  VARCHAR2 (5);
   v_err_num     NUMBER;
   v_err_msg     NVARCHAR2 (200);
   v_execsql     varchar2 (500);
BEGIN

   if ainparam01 is null then
     v_ainparam01:='00000';
   else
     v_ainparam01:=ainparam01;     
   end if;
   
   v_execsql:='delete from t_11_201_'||v_ainparam01||' where c003 in (select c001 from t_11_006_'||v_ainparam01||' start with c001='||ainparam02||' connect by c004=prior c001)';
   execute immediate v_execsql;
 

   v_execsql:='delete from t_11_006_'||v_ainparam01||' where c001 in (select c001 from t_11_006_'||v_ainparam01||' start with c001='||ainparam02||' connect by c004=prior c001)';
   execute immediate v_execsql;
   commit; 
     
   
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
/
