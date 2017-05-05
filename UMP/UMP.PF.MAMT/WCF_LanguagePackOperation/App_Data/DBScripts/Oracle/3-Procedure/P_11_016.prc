CREATE OR REPLACE PROCEDURE P_11_016 (
   AInParam01          VARCHAR2 := '00000',
   AInParam02          VARCHAR2,
   AInParam03          VARCHAR2,
   AOutErrorNumber     OUT   NUMBER,
   AOutErrorString     OUT   NVARCHAR2
)
IS

-- description:    用户、角色管理或控制的资源列表为T_11_202_00000
-- create date:    <2014-9-19>
-- =========================================================================================
--AInParam01        租户ID（默认为‘00000’）
--AInParam02        用户ID
--AInParam03        操作人ID
-- =========================================================================================

   v_err_num     NUMBER;
   v_AInParam01   VARCHAR2 (5);
   v_err_msg     NVARCHAR2 (200);
   v_execsql     varchar2 (4000);

BEGIN
  
   IF AInParam01 IS NULL
   THEN
      v_AInParam01 := '00000';
   ELSE
      v_AInParam01 := AInParam01;
   END IF;
     

   v_execsql:='DELETE FROM t_11_202_'||v_AInParam01||' WHERE C001='||AInParam02;
   dbms_output.put_line(v_execsql);
   execute immediate v_execsql;   
  
   v_execsql:='INSERT INTO t_11_202_'||v_AInParam01||' A  
               select t.c001,t.c002, t.c003, t.c004, t.c005,
                      t.c006, t.c007, t.c008, t.c009, t.c010, t.c011, t.c012,
                      t.c013, t.c014, t.c015, t.c016, t.c017, t.c018, t.c019,
                      t.c020 from (select y.*,row_number() over
                      (partition by c001,c002 order by rowid desc) as aa
                      from  (select '||AInParam02||' as c001,b.c002,b.c003, b.c004, b.c005,'
                      ||AInParam03||' as c006, b.c007, b.c008, b.c009, b.c010, b.c011, b.c012,
                      b.c013, b.c014, b.c015, b.c016, b.c017, b.c018, b.c019,
                      b.c020 from t_11_202_'||v_AInParam01||' b
             WHERE b.c001 IN (SELECT c.c003
              FROM t_11_201_'||v_AInParam01||'  c WHERE c.c003 like ''106%'' and c.c004 = '||AInParam02||')) y) t where aa=1';
   --取被控制用户的上级用户的资源，写入到资源表中
   dbms_output.put_line(v_execsql);
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
