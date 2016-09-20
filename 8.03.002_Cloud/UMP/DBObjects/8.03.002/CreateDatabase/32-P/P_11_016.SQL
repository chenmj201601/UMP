CREATE PROCEDURE P_11_016 (
   AInParam01          VARCHAR2 := '00000',
   AInParam02          VARCHAR2,
   AInParam03          VARCHAR2,
   AOutErrorNumber     OUT   NUMBER,
   AOutErrorString     OUT   NVARCHAR2
)
IS



   v_err_num     NUMBER;
   v_AInParam01  VARCHAR2 (5);
   v_err_msg     NVARCHAR2 (200);
   v_execsql     varchar2 (4000);

BEGIN
  
   IF AInParam01 IS NULL
   THEN
      v_AInParam01 := '00000';
   ELSE
      v_AInParam01 := AInParam01;
   END IF;
     

   v_execsql:='DELETE FROM T_11_202_'||v_AInParam01||' WHERE C001='||AInParam02;
   dbms_output.put_line(v_execsql);
   EXECUTE IMMEDIATE v_execsql;   
  
   v_execsql:='INSERT INTO t_11_202_'||v_AInParam01||' A  
               SELECT T.C001,T.C002, T.C003, T.C004, T.C005,
                      T.C006, T.C007, T.C008, T.C009, T.C010, T.C011, T.C012,
                      T.C013, T.C014, T.C015, T.C016, T.C017, T.C018, T.C019,
                      T.C020 FROM (SELECT y.*,ROW_NUMBER() OVER
                      (PARTITION BY C001,C002 ORDER BY ROWID DESC) AS aa
                      FROM  (SELECT '||AInParam02||' AS C001,B.C002,B.C003, B.C004, B.C005,'
                      ||AInParam03||' AS C006, B.C007, B.C008, B.C009, B.C010, B.C011, B.C012,
                      B.C013, B.C014, B.C015, B.C016, B.C017, B.C018, B.C019,
                      B.C020 FROM T_11_202_'||v_AInParam01||' b
             WHERE b.C001 IN (SELECT c.C003
              FROM T_11_201_'||v_AInParam01||'  c WHERE c.C003 LIKE ''106%'' AND C.C004 = '||AInParam02||')) Y) T WHERE aa=1';

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