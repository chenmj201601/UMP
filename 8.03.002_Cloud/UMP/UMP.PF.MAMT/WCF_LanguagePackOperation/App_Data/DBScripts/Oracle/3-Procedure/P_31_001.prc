CREATE OR REPLACE PROCEDURE P_31_001 (
   AInParam01              VARCHAR2 := '00000',
   AInParam02              VARCHAR2 := 'Y',
   AInParam03              VARCHAR2:='0',
   AInParam04              NVARCHAR2,
   AInParam05              NVARCHAR2,
   AInParam06              VARCHAR2 := '0',
   AInParam07              VARCHAR2 := '-1',
   AInParam08              VARCHAR2 := '-1',
   AInParam09              CHAR := 'F',
   AOutParam01       OUT   VARCHAR2,
   AOutErrornumber   OUT   NUMBER,
   AOutErrorstring   OUT   NVARCHAR2
)
IS
-- description:    增加自定义查询界面表记录t_31_040_00000
-- create date:    <2014-10-30>
-- =========================================================================================
--AInParam01        租户标识（默认为‘00000’）
--AInParam02        Y为系统内置，N用户个人自定义的
--AInParam03        处于tabitem索引的值,从0开始
--AInParam04        Tab的名称
--AInParam05        用户控制名
--AInParam06        在一个TABITEM里的顺序
--AInParam07        用户ID ,对应 T_11_005_00000.C001
--AInParam08        控件高度
--AInParam09        F为全宽,H为一半

   v_serialid     VARCHAR2 (19);
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);
   v_AInParam01   VARCHAR2 (5);
   v_AInParam02   VARCHAR2 (1);
   v_AInParam03   VARCHAR2 (5);
   
   v_AInParam06   VARCHAR2 (5);
   v_AInParam07   VARCHAR2 (20);
   v_AInParam08   VARCHAR2 (5); 
   v_AInParam09   CHAR (1);
   v_execsql      VARCHAR2 (1024);

BEGIN
   IF AInParam01 IS NULL
   THEN
      v_AInParam01 := '00000';
   ELSE
      v_AInParam01 := AInParam01;
   END IF;

   IF AInParam02 IS NULL
   THEN
      v_AInParam02 :='Y';
   ELSE
      v_AInParam02 := AInParam02;
   END IF;

   IF AInParam03 IS NULL
   THEN
      v_AInParam03 := '0';
   ELSE
      v_AInParam03 := AInParam03;
   END IF;

   IF AInParam06 IS NULL
   THEN
      v_AInParam06 := '0';
   ELSE
      v_AInParam06 := AInParam06;
   END IF;

   IF AInParam07 IS NULL
   THEN
      v_AInParam07 := '-1';
   ELSE
      v_AInParam07 := AInParam07;
   END IF;
   
   IF AInParam08 IS NULL
   THEN
      v_AInParam08 := '-1';
   ELSE
      v_AInParam08 := AInParam08;
   END IF;

   IF AInParam09 IS NULL
   THEN
      v_AInParam09 := 'F';
   ELSE
      v_AInParam09 := AInParam09;
   END IF;   
   
   p_00_001 ('31', '302', v_AInParam01, NULL, v_serialid, v_err_num,
             v_err_msg);

   if v_serialid<0 or v_err_num <>0 then
     AOuterrornumber := to_number(v_serialid);
     AOuterrorstring := 'Serial number generation error';
     Return;
   end if;

  v_execsql :=
         'INSERT INTO t_31_040_'
      || v_AInParam01
      || '  
            VALUES ('''
      || v_serialid
      || ''','''
      || v_AInParam02
      || ''', '''
      || v_AInParam03
      || ''','''
      || AInParam04
      || ''','''
      || AInParam05
      || ''','''
      || v_AInParam06
      || ''', '''
      || v_AInParam07
      || ''','''
      || v_AInParam08
      || ''','''
      || v_AInParam09
      || ''')';
      
   --DBMS_OUTPUT.put_line (v_execsql);
   EXECUTE IMMEDIATE v_execsql;

   COMMIT;
   
   AOutParam01 := v_serialid;
   AOutErrornumber := '0';
   AOutErrorstring := 'successful';
   
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
