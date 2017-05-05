CREATE OR REPLACE PROCEDURE p_11_015 (
   AInParam01          VARCHAR2 := '00000',
   AInParam02          VARCHAR2,
   AInParam03          VARCHAR2,
   AInParam04          VARCHAR2,
   AInParam05          VARCHAR2,
   AInParam06          VARCHAR2,   
   AOutErrorNumber     OUT   NUMBER,
   AOutErrorString     OUT   NVARCHAR2
)
IS

-- description:    �û�����ɫ�������Ƶ���Դ�б�ΪT_11_201_00000
-- create date:    <2014-9-19>
-- =========================================================================================
--AInParam01        �⻧ID��Ĭ��Ϊ��00000����
--AInParam02        ������ţ���Ӧ���� C001��Ĭ��ֵ 0
--AInParam03        �û�����ɫ����������
--AInParam04        �����ơ��������ı��
--AInParam05        ��ʼʱ��
--AInParam06        ����ʱ��
-- =========================================================================================
--aoutparam01       �û�����ɫ�������Ƶ���Դ�б��ID
-- =========================================================================================
   v_serialid    VARCHAR2 (19);
   v_err_num     NUMBER;
   v_err_msg     NVARCHAR2 (200);
   v_AInParam01  VARCHAR2 (5);  
   v_execsql     varchar2 (4000);
   v_count       number;
BEGIN
  
   if AInParam01 is null then
     v_AInParam01:='00000';
   else
     v_AInParam01:=AInParam01;
   end if;
     
   v_execsql :='select count(1) from  t_11_201_'||v_AInParam01||' where c003='''||AInParam03||''' and c004='''||AInParam04||'''';
   --dbms_output.put_line(v_execsql);    
      
   EXECUTE IMMEDIATE v_execsql into v_count;
  
   if v_count>0 
   then
      AOuterrornumber := '-1';
      AOuterrorstring := 'Record already exists';
      return;         
   end if; 

   p_00_001 ('11', '105', v_AInParam01, NULL, v_serialid, v_err_num, v_err_msg); --ȡ����ˮ��  ģ���������11��С����105
   
   if v_serialid<0 or v_err_num <>0 then
     AOuterrornumber := to_number(v_serialid);
     AOuterrorstring := 'Serial number generation error';     
     Return;
   end if;   

   v_execsql:='INSERT INTO t_11_201_'||v_AInParam01||'
            VALUES ('''||v_serialid||''','''||AInParam02||''','''||AInParam03||''', '''||AInParam04||''',to_date('''||AInParam05||''',''yyyy-mm-dd hh24:mi:ss''),'||
                'to_date('''||AInParam06||''',''yyyy-mm-dd hh24:mi:ss''))';

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
