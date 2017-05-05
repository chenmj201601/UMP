CREATE PROCEDURE P_11_012 (
   ainparam01              VARCHAR2 := '00000',
   ainparam02              NVARCHAR2,
   ainparam03              NVARCHAR2,
   ainparam04              VARCHAR2,
   ainparam05              VARCHAR2 := '20002',
   ainparam06              VARCHAR2,
   ainparam07              CHAR := 'S',
   ainparam08              CHAR := '0',
   ainparam09              CHAR := 'N',
   ainparam10              CHAR := '1',
   ainparam11              CHAR := '0',
   ainparam12              VARCHAR
         := '1111111111111111111111111111111111111111',
   ainparam13              VARCHAR2,
   ainparam14              NVARCHAR2,
   ainparam15              VARCHAR2,
   ainparam16              VARCHAR2,
   ainparam17              VARCHAR2,
   ainparam18              VARCHAR2,
   ainparam19              VARCHAR2,
   ainparam20              VARCHAR2,
   aoutparam01       OUT   VARCHAR2,
   aouterrornumber   OUT   NUMBER,
   aouterrorstring   OUT   NVARCHAR2
)
IS
-- description:    增加新用户T_11_005_     增加资源t_11_201_
-- create date:    <2014-9-4>
-- =========================================================================================
--AInParam01        租户标识（默认为‘00000’）
--AInParam02        账户
--AInParam03        全名
--AInParam04        密码
--AInParam05        加密版本和方法
--AInParam06        机构ID
--AInParam07        来源
--AInParam08        是否锁定
--AInParam09        锁定方式
--AInParam10        是否活动
--AInParam11        是否删除
--AInParam12        其他状态
--AInParam13        最后登录时间
--AInParam14        开始时间
--AInParam15        失效时间
--AInParam16        创建人
--AInParam17        创建时间
--AInParam18        入职时间
--AInParam19        离职时间
--AInParam20        最后修改密码的时间
-- ================================================
--aoutparam01       用户ID
-- ================================================
--流水号 模块号 11 操作码 102
-- ================================================
   v_serialid     VARCHAR2 (19);
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);
   v_ainparam01   VARCHAR2 (5);
   v_ainparam04   VARCHAR2 (1024);
   v_ainparam05   VARCHAR2 (5);
   v_ainparam07   CHAR (1);
   v_ainparam08   CHAR (1);
   v_ainparam09   CHAR (1);
   v_ainparam10   CHAR (1);
   v_ainparam11   CHAR (1);
   v_ainparam12   VARCHAR2 (32);
   v_ainparam13   VARCHAR2 (20);
   v_ainparam17   VARCHAR2 (512);
   v_execsql      VARCHAR2 (1024);
   v_count        INTEGER;
   v_datetime     VARCHAR2 (14);
BEGIN
   IF ainparam01 IS NULL
   THEN
      v_ainparam01 := '00000';
   ELSE
      v_ainparam01 := ainparam01;
   END IF;

   IF ainparam04 IS NULL
   THEN
      v_ainparam04 :=
         '3FE3B4AA02D4CFCB26D2D6C8BFA1F095F55320344719954CE0A3B7346BCE6441079503E67FFB8ACA5B8216460E555795CE4C757C18B6FF8C87F6C044777F7C7C';
   ELSE
      v_ainparam04 := ainparam04;
   END IF;

   IF ainparam05 IS NULL
   THEN
      v_ainparam05 := '20002';
   ELSE
      v_ainparam05 := ainparam05;
   END IF;

   IF ainparam07 IS NULL
   THEN
      v_ainparam07 := 'S';
   ELSE
      v_ainparam07 := ainparam07;
   END IF;

   IF ainparam08 IS NULL
   THEN
      v_ainparam08 := '0';
   ELSE
      v_ainparam08 := ainparam08;
   END IF;

   IF ainparam09 IS NULL
   THEN
      v_ainparam09 := 'N';
   ELSE
      v_ainparam09 := ainparam09;
   END IF;

   IF ainparam10 IS NULL
   THEN
      v_ainparam10 := '1';
   ELSE
      v_ainparam10 := ainparam10;
   END IF;

   IF ainparam11 IS NULL
   THEN
      v_ainparam11 := '0';
   ELSE
      v_ainparam11 := ainparam11;
   END IF;

   IF ainparam12 IS NULL
   THEN
      v_ainparam12 := '11111111111111111111111111111111';
   ELSE
      v_ainparam12 := ainparam12;
   END IF;

   IF ainparam13 IS NULL
   THEN
      v_ainparam13 := '2014-01-01 00:00:00';
   ELSE
      v_ainparam13 := ainparam13;
   END IF;

   DBMS_OUTPUT.put_line (v_ainparam17);
   v_execsql :=
         'SELECT COUNT(1) FROM  T_11_005_'
      || v_ainparam01
      || ' WHERE c002='''
      || ainparam02
      || '''';
   DBMS_OUTPUT.put_line (v_execsql);

   EXECUTE IMMEDIATE v_execsql
                INTO v_count;

   IF v_count > 0
   THEN
      aouterrornumber := '-1';
      aouterrorstring := 'User already exists';
      RETURN;
   END IF;
   
   SELECT to_char(sysdate,'yyyymmddhh24miss') INTO v_datetime FROM DUAL; 

   P_00_001 ('11', '102', v_ainparam01, v_datetime, v_serialid, v_err_num,
             v_err_msg);

   IF v_serialid < 0 OR v_err_num <> 0
   THEN
      aouterrornumber := -1;
      aouterrorstring := 'Serial number generation error';
      RETURN;
   END IF;

   v_execsql :=
         'INSERT INTO T_11_005_'
      || v_ainparam01
      || '  
            VALUES ('''
      || v_serialid
      || ''','''
      || ainparam02
      || ''', '''
      || ainparam03
      || ''','''
      || v_ainparam04
      || ''','''
      || v_ainparam05
      || ''','''
      || ainparam06
      || ''', '''
      || v_ainparam07
      || ''','''
      || v_ainparam08
      || ''','''
      || v_ainparam09
      || ''','''
      || v_ainparam10
      || ''','''
      || v_ainparam11
      || ''','''
      || v_ainparam12
      || ''','''
      || v_ainparam13
      || ''','''','''',0,'''
      || ainparam14
      || ''','''
      || ainparam15
      || ''','''
      || ainparam16
      || ''','''
      || ainparam17                                                     --c020
      || ''',TO_DATE('''
      || ainparam18                                                     --c021
      || ''',''yyyy-mm-dd hh24:mi:ss''),TO_DATE('''
      || ainparam19                                                     --c022
      || ''',''yyyy-mm-dd hh24:mi:ss''),TO_DATE('''
      || ainparam20                                                     --c023
      || ''',''yyyy-mm-dd hh24:mi:ss''),0,''1'','''')';           --c024 c025 c026
   DBMS_OUTPUT.put_line (v_execsql);

   EXECUTE IMMEDIATE v_execsql;

   v_execsql :=
         'INSERT INTO t_11_201_'
      || v_ainparam01
      || '  
            SELECT 0, 0, c003,'
      || v_serialid
      || ',SYSDATE,'
      || 'TO_DATE(''2199-12-31'',''YYYY-MM-DD HH24:MI:SS'') FROM T_11_201_'
      || v_ainparam01
      || ' WHERE C003 > 1020000000000000000 and C003 < 1030000000000000000  and c004 = '
      || ainparam16;
   DBMS_OUTPUT.put_line (v_execsql);

   EXECUTE IMMEDIATE v_execsql;

   v_execsql :=
         'INSERT INTO t_11_201_'
      || v_ainparam01
      || ' Values (0,0,'
      || v_serialid||','|| v_serialid
      || ',SYSDATE,'
      || 'TO_DATE(''2199-12-31'',''YYYY-MM-DD HH24:MI:SS''))'; 

   EXECUTE IMMEDIATE v_execsql;

   v_execsql :=
         'INSERT INTO T_11_203_'
      || v_ainparam01
      || ' SELECT '
      || v_serialid
      || ', C001, C002, C012, ''1'', ''1'', ''1'', ''1'', ''1'', ''1'','
      || '''1'', ''0'', ''0'', ''0'', ''0'',C007,0 FROM T_00_102';
   DBMS_OUTPUT.put_line (v_execsql);

   EXECUTE IMMEDIATE v_execsql;

   COMMIT;
   aoutparam01 := v_serialid;
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