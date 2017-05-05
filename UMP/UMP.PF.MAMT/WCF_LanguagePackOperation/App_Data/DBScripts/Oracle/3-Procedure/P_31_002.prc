CREATE OR REPLACE PROCEDURE P_31_002 (
   AInParam01              VARCHAR2 := '00000',   
   AInParam02              VARCHAR2 := 'I',
   AInParam03              VARCHAR2,
   AInParam04              VARCHAR2,
   AInParam05              VARCHAR2,
   AInParam06              VARCHAR2,
   AInParam07              VARCHAR2,
   AInParam08              NVARCHAR2,
   AInParam09              NVARCHAR2,
   AInParam10              CHAR     :='A',
   AInParam11              CHAR     :='1',
   AInParam12              VARCHAR2,
   AInParam13              VARCHAR2,
   AInParam14              VARCHAR2,
   AInParam15              VARCHAR2,
   AInParam16              VARCHAR2,
   AInParam17              VARCHAR2,
   AInParam18              VARCHAR2 :='0',     
   AInParam19              CHAR,         
   AOutErrornumber   OUT   NUMBER,
   AOutErrorstring   OUT   NVARCHAR2
)
IS
-- description:    ����ɾ���ı�T_31_042_00000¼����¼��BookMark
-- create date:    <2014-11-05>
-- =========================================================================================
--AInParam01        �⻧��ʶ��Ĭ��Ϊ��00000����
--AInParam02        ������ʶ I������ D��ɾ�� U���޸�
--AInParam03        Book Mark ��ˮ��
--AInParam04        ¼����¼��ˮ��.C001
--AInParam05        ¼����¼��ˮ��.C002
--AInParam06        OffsetTime.������ƫ��������λ������
--AInParam07        ʱ����
--AInParam08        ����
--AInParam09        ����
--AInParam10        ����ʽ Ĭ��Ϊ'A'
--AInParam11        ״̬��'0'ɾ����'1'����
--AInParam12        ��ע��
--AInParam13        ��עʱ�� yyyyMMddHHmmss
--AInParam14        ����޸���
--AInParam15        ����޸�ʱ�� yyyyMMddHHmmss
--AInParam16        ɾ����
--AInParam17        ɾ��ʱ�� yyyyMMddHHmmss
--AInParam18        ���� Ĭ��0
--AInParam19        ��Դ.'C'��CQCд�룬'U'��UMPд��

   v_serialid     VARCHAR2 (19);
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);
   v_AInParam01   VARCHAR2 (5);
   v_AInParam02   VARCHAR2 (1);
   v_AInParam10   VARCHAR2 (1);
   v_AInParam11   VARCHAR2 (1);
   v_AInParam18   VARCHAR2 (1); 

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
      v_AInParam02 :='I';
   ELSE
      v_AInParam02 := AInParam02;
   END IF;
   
   IF v_AInParam02 not in ('I','D','U') THEN
        AOuterrornumber := '-1';
        AOuterrorstring := 'AInParam02 is illegal';
        Return;
   END IF;   

   IF AInParam10 IS NULL
   THEN
      v_AInParam10 := 'A';
   ELSE
      v_AInParam10 := AInParam10;
   END IF;

   IF AInParam11 IS NULL
   THEN
      v_AInParam11 := '1';
   ELSE
      v_AInParam11 := AInParam11;
   END IF;

   IF AInParam18 IS NULL
   THEN
      v_AInParam18 := '0';
   ELSE
      v_AInParam18 := AInParam18;
   END IF;

   --�������������1
   if v_AInParam02='I'  then
          p_00_001 ('31', '142', v_AInParam01, NULL, v_serialid, v_err_num,--ģ�����31 ��������142
          v_err_msg);

          if v_serialid<0 or v_err_num <>0 then
            AOuterrornumber := '-1';
            AOuterrorstring := 'Serial number generation error';
            Return;
          end if;

          v_execsql :=
          'INSERT INTO t_31_042_'
          || v_AInParam01
          || '
          VALUES ('''
          || v_serialid
          || ''','''
          || AInParam04
          || ''','''
          || AInParam05
          || ''','''
          || AInParam06
          || ''','''
          || AInParam07
          || ''', '''
          || AInParam08
          || ''','''
          || AInParam09
          || ''','''
          || v_AInParam10
          || ''','''
          || v_AInParam11
          || ''','''
          || AInParam12
          || ''','''
          || AInParam13
          || ''','''
          || AInParam14
          || ''','''
          || AInParam15
          || ''','''
          || AInParam16
          || ''','''
          || AInParam17
          || ''','''
          || v_AInParam18
          || ''','''
          || AInParam19                                                                                          
          || ''')';

          DBMS_OUTPUT.put_line (v_execsql);
          EXECUTE IMMEDIATE v_execsql;

          COMMIT;
    end if;
    
    if  v_AInParam02='D'  then
          v_execsql :=
          'DELETE t_31_042_'
          || v_AInParam01
          || '
          WHERE C001='||AInParam03;

          DBMS_OUTPUT.put_line (v_execsql);
          EXECUTE IMMEDIATE v_execsql;

          COMMIT;
    end if;
                     
    if  v_AInParam02='U'  then
   
          v_execsql :=
          'UPDATE t_31_042_'
          || v_AInParam01
          || '
          SET  C002='''
          || AInParam04
          || ''',C003='''
          || AInParam05           
          || ''',C004='''
          || AInParam06          
          || ''',C005='''
          || AInParam07
          || ''',C006='''
          || AInParam08
          || ''',C007='''
          || AInParam09
          || ''',C008='''
          || AInParam10
          || ''',C009='''
          || AInParam11
          || ''',C010='''
          || AInParam12
          || ''',C011='''
          || AInParam13
          || ''',C012='''
          || AInParam14
          || ''',C013='''
          || AInParam15
          || ''',C014='''
          || AInParam16
          || ''',C015='''
          || AInParam17
          || ''',C016='''
          || AInParam18
          || ''',C017='''
          || AInParam19                                                                                     
          || ''' WHERE C001='||AInParam03;

          DBMS_OUTPUT.put_line (v_execsql);
          EXECUTE IMMEDIATE v_execsql;

          COMMIT;
 
   end if;
   
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
