CREATE OR REPLACE PROCEDURE p_11_012 (
   AInParam01              VARCHAR2 := '00000',
   AInParam02              NVARCHAR2,
   AInParam03              NVARCHAR2,
   AInParam04              VARCHAR2,
   AInParam05              VARCHAR2 := '20002',
   AInParam06              VARCHAR2,
   AInParam07              CHAR := 'S',
   AInParam08              CHAR := '0',
   AInParam09              CHAR := 'N',
   AInParam10              CHAR := '1',
   AInParam11              CHAR := '0',
   AInParam12              VARCHAR
         := '1111111111111111111111111111111111111111',
   AInParam13              VARCHAR2,                                         --
   AInParam14              NVARCHAR2,
   AInParam15              VARCHAR2,
   AInParam16              VARCHAR2,
   AInParam17              VARCHAR2,
   AInParam18              VARCHAR2,
   AInParam19              VARCHAR2,                                         --
   AInParam20              VARCHAR2,                                         --
   AOutParam01       OUT   VARCHAR2,
   AOutErrornumber   OUT   NUMBER,
   AOutErrorstring   OUT   NVARCHAR2
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

   --aoutparam01       用户ID
   v_serialid     VARCHAR2 (19);
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);
   v_AInParam01   VARCHAR2 (5);
   v_AInParam04   VARCHAR2 (1024);
   v_AInParam05   VARCHAR2 (5);
   v_AInParam07   CHAR (1);
   v_AInParam08   CHAR (1);
   v_AInParam09   CHAR (1);
   v_AInParam10   CHAR (1);
   v_AInParam11   CHAR (1);
   v_AInParam12   VARCHAR2 (32);
   v_AInParam13   VARCHAR2 (20);
   v_AInParam17   VARCHAR2 (512);
   v_execsql      VARCHAR2 (1024);
   v_count        INTEGER;
BEGIN
   IF AInParam01 IS NULL
   THEN
      v_AInParam01 := '00000';
   ELSE
      v_AInParam01 := AInParam01;
   END IF;

   IF AInParam04 IS NULL
   THEN
      v_AInParam04 :=
         '3FE3B4AA02D4CFCB26D2D6C8BFA1F095F55320344719954CE0A3B7346BCE6441079503E67FFB8ACA5B8216460E555795CE4C757C18B6FF8C87F6C044777F7C7C';
   ELSE
      v_AInParam04 := AInParam04;
   END IF;

   IF AInParam05 IS NULL
   THEN
      v_AInParam05 := '20002';
   ELSE
      v_AInParam05 := AInParam05;
   END IF;

   IF AInParam07 IS NULL
   THEN
      v_AInParam07 := 'S';
   ELSE
      v_AInParam07 := AInParam07;
   END IF;

   IF AInParam08 IS NULL
   THEN
      v_AInParam08 := '0';
   ELSE
      v_AInParam08 := AInParam08;
   END IF;

   IF AInParam09 IS NULL
   THEN
      v_AInParam09 := 'N';
   ELSE
      v_AInParam09 := AInParam09;
   END IF;

   IF AInParam10 IS NULL
   THEN
      v_AInParam10 := '1';
   ELSE
      v_AInParam10 := AInParam10;
   END IF;

   IF AInParam11 IS NULL
   THEN
      v_AInParam11 := '0';
   ELSE
      v_AInParam11 := AInParam11;
   END IF;

   IF AInParam12 IS NULL
   THEN
      v_AInParam12 := '11111111111111111111111111111111';
   ELSE
      v_AInParam12 := AInParam12;
   END IF;

   IF AInParam13 IS NULL
   THEN
      v_AInParam13 := '2014-01-01 00:00:00';
   ELSE
      v_AInParam13 := AInParam13;
   END IF;

   DBMS_OUTPUT.put_line (v_AInParam17);
   v_execsql :=
         'select count(1) from  t_11_005_'
      || v_AInParam01
      || ' where c002='''
      || AInParam02
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

   p_00_001 ('11', '102', v_AInParam01, NULL, v_serialid, v_err_num,
             v_err_msg);
             
   if v_serialid<0 or v_err_num <>0 then
     AOuterrornumber := to_number(v_serialid);
     AOuterrorstring := 'Serial number generation error';     
     Return;
   end if;
   
   v_execsql :=
         'INSERT INTO t_11_005_'
      || v_AInParam01
      || '  
            VALUES ('''
      || v_serialid
      || ''','''
      || AInParam02
      || ''', '''
      || AInParam03
      || ''','''
      || v_AInParam04
      || ''','''
      || v_AInParam05
      || ''','''
      || AInParam06
      || ''', '''
      || v_AInParam07
      || ''','''
      || v_AInParam08
      || ''','''
      || v_AInParam09
      || ''','''
      || v_AInParam10
      || ''','''
      || v_AInParam11
      || ''','''
      || v_AInParam12
      || ''','''
      || v_AInParam13
      || ''','''','''','''','''
      || AInParam14
      || ''','''
      || AInParam15
      || ''','''
      || AInParam16
      || ''','''
      || AInParam17                                                     --c020
      || ''',to_date('''
      || AInParam18                                                     --c021
      || ''',''yyyy-mm-dd hh24:mi:ss''),to_date('''
      || AInParam19                                                     --c022
      || ''',''yyyy-mm-dd hh24:mi:ss''),to_date('''
      || AInParam20                                                     --c023
      || ''',''yyyy-mm-dd hh24:mi:ss''),0,1,'''')';

   --c024 c025 c026

   --dbms_output.put_line(v_execsql);
   EXECUTE IMMEDIATE v_execsql;

   v_execsql :=
         'INSERT INTO t_11_201_'
      || v_AInParam01
      || '  
            VALUES (0, 0, '
      || AInParam16
      || ','
      || v_serialid
      || ',sysdate,'
      || 'to_date(''2199-12-31'',''YYYY-MM-DD HH24:MI:SS''))';
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
/
