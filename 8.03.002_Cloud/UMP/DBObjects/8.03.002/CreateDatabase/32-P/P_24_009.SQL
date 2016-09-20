CREATE  PROCEDURE P_24_009 (
   ainparam00              VARCHAR2:='00000',
   ainparam01              VARCHAR2,
   ainparam02              VARCHAR2,
   ainparam03              VARCHAR2,
   ainparam04              VARCHAR2,
   ainparam05              VARCHAR2,
   ainparam06              VARCHAR2,
   aouterrornumber   OUT   NUMBER,
   aouterrorstring   OUT   NVARCHAR2
)
IS

   v_sql          VARCHAR2(4000);
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2(200);
   v_C200         NUMBER(20);
   v_C201_w       VARCHAR2(5);
   v_C211_w       VARCHAR2(5);
   v_C221_w       VARCHAR2(5);
   v_C231_w       VARCHAR2(5);

BEGIN

   aouterrornumber := '0';
   aouterrorstring := 'successful';


   v_sql:='select  C200+1 from T_21_001_'||AInParam00||' where C001='||ainparam03;
   DBMS_OUTPUT.put_line(v_sql);

   execute immediate v_sql INTO  v_C200;
   COMMIT;

   IF   v_C200=1  THEN
     v_C201_w:='C201';
     v_C211_w:='C211';
     v_C221_w:='C221';
     v_C231_w:='C231';

   ELSIF v_C200=2 THEN
     v_C201_w:='C202';
     v_C211_w:='C212';
     v_C221_w:='C222';
     v_C231_w:='C232';

   ELSIF v_C200=3 THEN
     v_C201_w:='C203';
     v_C211_w:='C213';
     v_C221_w:='C223';
     v_C231_w:='C233';

   ELSIF v_C200=4 THEN
     v_C201_w:='C204';
     v_C211_w:='C214';
     v_C221_w:='C224';
     v_C231_w:='C234';

   ELSIF v_C200=5 THEN
     v_C201_w:='C205';
     v_C211_w:='C215';
     v_C221_w:='C225';
     v_C231_w:='C235';

   ELSIF v_C200=6 THEN
     v_C201_w:='C206';
     v_C211_w:='C216';
     v_C221_w:='C226';
     v_C231_w:='C236';

   ELSIF v_C200=7 THEN
     v_C201_w:='C207';
     v_C211_w:='C217';
     v_C221_w:='C227';
     v_C231_w:='C237';

  ELSIF v_C200=8 THEN
     v_C201_w:='C208';
     v_C211_w:='C218';
     v_C221_w:='C228';
     v_C231_w:='C238';

  ELSIF v_C200=9  THEN
     v_C201_w:='C209';
     v_C211_w:='C219';
     v_C221_w:='C229';
     v_C231_w:='C239';

  ELSIF v_C200=10 THEN
     v_C201_w:='C210';
     v_C211_w:='C220';
     v_C221_w:='C230';
     v_C231_w:='C240';

   END IF;

   IF ainparam06='1' THEN

     INSERT INTO T_00_206 select * from T_00_205 where C001=ainparam01 AND C002=ainparam02 AND C003=ainparam03;

     v_sql:='UPDATE T_21_001_'||ainparam00||' SET C200='||v_C200||','||v_C201_w||'='''||AInParam02
          ||''','||v_C211_w||'='''||AInParam01
          ||''','||v_C221_w||'=TO_CHAR(TO_DATE('''|| ainparam05|| ''',''yyyy-mm-dd hh24:mi:ss''),''yyyymmddhh24miss''),'
          ||v_C231_w||'='||ainparam04||' where C001='||ainparam03;

     DBMS_OUTPUT.put_line(v_sql); 
     EXECUTE IMMEDIATE v_sql;

   ELSE
     INSERT INTO T_00_207 select * from T_00_205 where C001=ainparam01 AND C002=ainparam02 AND C003=ainparam03;

   END IF;

   DELETE FROM T_00_205  where C001=ainparam01 AND C002=ainparam02 AND C003=ainparam03;

   COMMIT;

EXCEPTION
   WHEN OTHERS
   THEN
      ROLLBACK;
      v_err_num := SQLCODE;
      v_err_msg := SUBSTR (SQLERRM, 1, 200);
      aouterrornumber := v_err_num;
      aouterrorstring := v_err_msg;
END;
