﻿CREATE OR REPLACE PROCEDURE P_11_010 (
   ainparam01              VARCHAR2 := '00000',
   ainparam02              NVARCHAR2,
   ainparam03              VARCHAR2,
   ainparam04              VARCHAR2,
   ainparam05              CHAR := '1',
   ainparam06              CHAR := '0',
   ainparam07              VARCHAR2 := '11111111111111111111111111111111',
   ainparam08              VARCHAR2,
   ainparam09              VARCHAR2,
   ainparam10              VARCHAR2,
   ainparam11              VARCHAR2,
   ainparam12              NVARCHAR2,
   ainparam13              NVARCHAR2,   
   aoutparam01       OUT   VARCHAR2,
   aouterrornumber   OUT   NUMBER,
   aouterrorstring   OUT   NVARCHAR2
)
IS
-- description:    新机构的ID生成过程，记录目标表为机构部门T_11_006_00000,用户、角色管理或控制的资源列表t_11_201_00000
-- create date:    <2014-9-3>
-- =========================================================================================
--AInParam01        租户ID（默认为‘00000’）
--AInParam02        机构名称
--AInParam03        机构类型
--AInParam04        父级机构ID
--AInParam05        是否活动
--AInParam06        是否删除
--AInParam07        其他状态
--AInParam08        开始时间
--AInParam09        失效时间
--AInParam10        创建人
--AInParam11        创建时间
--AInParam12        描述
--ainparam13        新增机构是否为云机构 1是 0否
-- ======================================
--aoutparam01       新机构的ID
--===================================================
--流水号 模块号 11 操作码 101
--===================================================
   v_serialid     VARCHAR2 (19);
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);
   v_ainparam01   VARCHAR2 (5);
   v_ainparam05   VARCHAR2 (1);
   v_ainparam06   VARCHAR2 (1);
   v_ainparam07   VARCHAR2 (32);
   v_ainparam11   VARCHAR2 (20);
   v_execsql      VARCHAR2 (500);
   v_count        NUMBER (5);
   v_datetime     VARCHAR2 (14);
BEGIN

   IF ainparam01 IS NULL
   THEN
      v_ainparam01 := '00000';
   ELSE
      v_ainparam01 := ainparam01;
   END IF;

   v_execsql :=
         'SELECT COUNT(1) FROM  T_11_006_'
      || v_ainparam01
      || ' WHERE C002='''
      || ainparam02
      || ''' AND C004='
      || ainparam04;
   DBMS_OUTPUT.put_line (v_execsql);

   EXECUTE IMMEDIATE v_execsql
                INTO v_count;

   IF v_count > 0
   THEN
      aouterrornumber := '-1';
      aouterrorstring := 'Organization already exists';
      RETURN;
   END IF;
   
   DBMS_OUTPUT.put_line (v_count);

   IF ainparam05 IS NULL
   THEN
      v_ainparam05 := '1';
   ELSE
      v_ainparam05 := ainparam05;
   END IF;

   IF ainparam06 IS NULL
   THEN
      v_ainparam06 := '0';
   ELSE
      v_ainparam06 := ainparam06;
   END IF;

   IF ainparam07 IS NULL
   THEN
      v_ainparam07 := '11111111111111111111111111111111';
   ELSE
      v_ainparam07 := ainparam07;
   END IF;

   IF ainparam11 IS NULL
   THEN
      v_ainparam11 := TO_CHAR (SYSDATE, 'yyyy-mm-dd hh24:mi:ss');
   ELSE
      v_ainparam11 := ainparam11;
   END IF;

   IF (ainparam13='1') THEN 
     v_datetime:='20140101000000';     
   ELSE
     SELECT to_char(sysdate,'yyyymmddhh24miss') INTO v_datetime FROM DUAL;       
   END IF;  
   
   P_00_001 ('11', '101', v_ainparam01, v_datetime, v_serialid, v_err_num,
               v_err_msg);   
             
    DBMS_OUTPUT.put_line (v_serialid);          

   IF v_serialid < 0 OR v_err_num <> 0
   THEN
      aouterrornumber := -1;
      aouterrorstring := 'Serial number generation error';
      RETURN;
   END IF;

   v_execsql :=
         'INSERT INTO T_11_006_'
      || v_ainparam01
      || '
            VALUES ('''
      || v_serialid
      || ''','''
      || ainparam02
      || ''', '''
      || ainparam03
      || ''','''
      || ainparam04
      || ''','''
      || v_ainparam05
      || ''','''
      || v_ainparam06
      || ''', '''
      || v_ainparam07
      || ''','''
      || ainparam08
      || ''','''
      || ainparam09
      || ''','''
      || ainparam10
      || ''',TO_DATE('''
      || v_ainparam11
      || ''',''yyyy-mm-dd hh24:mi:ss''),'''
      || ainparam12
      || ''')';

   EXECUTE IMMEDIATE v_execsql;

   v_execsql :=
         'INSERT INTO T_11_201_'
      || v_ainparam01
      || '
            SELECT 0, 0, C003,'
      || v_serialid
      || ',SYSDATE,'
      || 'TO_DATE(''2199-12-31'',''YYYY-MM-DD HH24:MI:SS'') FROM T_11_201_'
      || v_ainparam01
      || ' where C004 = '
      || ainparam10;

   --dbms_output.put_line(v_execsql);
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
