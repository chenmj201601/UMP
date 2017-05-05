CREATE PROCEDURE P_11_013 (
   AInParam01              VARCHAR2 := '00000',
   AInParam02              VARCHAR2,
   AOutErrorNumber   OUT   NUMBER,
   AOutErrorString   OUT   NVARCHAR2
)
IS
-- description:    删除用户账户
-- create date:    <2014-9-5>
-- ===============================================================
--ainparam01        租户ID（默认为‘00000’）
--ainparam02        用户ID
-- ===============================================================
   v_ainparam01   VARCHAR2 (5);
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);
   v_execsql      VARCHAR2 (500);
BEGIN
   IF ainparam01 IS NULL
   THEN
      v_ainparam01 := '00000';
   ELSE
      v_ainparam01 := ainparam01;
   END IF;

   v_execsql :=
         'DELETE FROM T_11_005_'
      || v_ainparam01
      || ' WHERE C001 = '''
      || ainparam02
      || '''';

   EXECUTE IMMEDIATE v_execsql;

   v_execsql :=
         'DELETE FROM T_11_101_'
      || v_ainparam01
      || ' WHERE C001 = '''
      || ainparam02
      || '''';

   EXECUTE IMMEDIATE v_execsql;

   v_execsql :=
         'DELETE FROM T_11_201_'
      || v_ainparam01
      || ' WHERE C003 = '''
      || ainparam02
      || '''';

   EXECUTE IMMEDIATE v_execsql;
   
   v_execsql :=
         'DELETE FROM T_11_201_'
      || v_ainparam01
      || ' WHERE C004 = '''
      || ainparam02
      || '''';

   EXECUTE IMMEDIATE v_execsql;   
    
    v_execsql :='DELETE T_11_203_'
      || v_AInParam01   
      ||' WHERE C001='||AInParam02;
      
   DBMS_OUTPUT.put_line (v_execsql);   
   EXECUTE IMMEDIATE v_execsql;   
   
   COMMIT;
   
   aouterrornumber := '0';
   aouterrorstring := 'successful';
EXCEPTION
   WHEN OTHERS
   THEN
      ROLLBACK;
      v_err_num := SQLCODE;
      v_err_msg := SUBSTR (SQLERRM, 1, 200);
      aouterrornumber := v_err_num;
      aouterrorstring := v_err_msg;
END; 