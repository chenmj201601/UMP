CREATE PROCEDURE P_11_011 (
   AInParam01          VARCHAR2 := '00000',
   AInParam02          VARCHAR2,
   AOutErrorNumber     OUT   NUMBER,
   AOutErrorString     OUT   NVARCHAR2
)
IS

-- description:    删除t_11_006_00000机构下的所有记录
-- create date:    <2014-9-2>
-- ===============================================
--ainparam01        租户ID（默认为‘00000’）
--ainparam02        机构ID (将此机构及其下面子机构所列的资源t_11_201 及 机构t_11_006本身 全部删除)
-- ===============================================

   v_ainparam01  VARCHAR2 (5);
   v_err_num     NUMBER;
   v_err_msg     NVARCHAR2 (200);
   v_execsql     VARCHAR2 (500);
BEGIN

   IF ainparam01 IS NULL THEN
     v_ainparam01:='00000';
   ELSE
     v_ainparam01:=ainparam01;     
   END IF;
   
   v_execsql:='DELETE FROM T_11_201_'||v_ainparam01||' WHERE C003 IN (SELECT c001 FROM t_11_006_'||v_ainparam01||' START WITH c001='||ainparam02||' CONNECT BY c004=PRIOR C001)';
   EXECUTE IMMEDIATE v_execsql;
   
   v_execsql:='DELETE FROM T_11_201_'||v_ainparam01||' WHERE C004 IN (SELECT c001 FROM t_11_006_'||v_ainparam01||' START WITH c001='||ainparam02||' CONNECT BY c004=PRIOR C001)';
   EXECUTE IMMEDIATE v_execsql; 

   v_execsql:='DELETE FROM T_11_006_'||v_ainparam01||' WHERE C001 IN (SELECT c001 FROM t_11_006_'||v_ainparam01||' START WITH c001='||ainparam02||' CONNECT BY c004=PRIOR C001)';
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