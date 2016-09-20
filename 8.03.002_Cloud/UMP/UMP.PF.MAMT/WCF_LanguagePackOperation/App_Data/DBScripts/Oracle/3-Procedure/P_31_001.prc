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
-- description:    �����Զ����ѯ������¼t_31_040_00000
-- create date:    <2014-10-30>
-- =========================================================================================
--AInParam01        �⻧��ʶ��Ĭ��Ϊ��00000����
--AInParam02        YΪϵͳ���ã�N�û������Զ����
--AInParam03        ����tabitem������ֵ,��0��ʼ
--AInParam04        Tab������
--AInParam05        �û�������
--AInParam06        ��һ��TABITEM���˳��
--AInParam07        �û�ID ,��Ӧ T_11_005_00000.C001
--AInParam08        �ؼ��߶�
--AInParam09        FΪȫ��,HΪһ��

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
