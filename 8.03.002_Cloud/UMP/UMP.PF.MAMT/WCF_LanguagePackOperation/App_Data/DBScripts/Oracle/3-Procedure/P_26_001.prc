create or replace procedure P_26_001 (AInParam01       in  varchar2,
                                      AInParam02       in  varchar2,
                                      AInParam03       in  varchar2,
                                      AInParam04       in  varchar2,
                                      AInParam05       in  varchar2,
                                      AInParam06       in  varchar2,
                                      AOutErrorNumber  out number,
                                      AOutErrorString  out varchar2)

is
   v_err_num     number;
   v_err_msg     nvarchar2(200);
   v_count       integer;
   i             integer;
   v_temp        nvarchar2(2000);
   v_columnname  nvarchar2(5);
   v_columnvalue nvarchar2(2000);
   v_string      nvarchar2(2000);

-- create date:    <2014-9-2>
-- description:    <����ѯ����ļ�¼д�뵽�鵵����Ԥ���������>
-- =========================================================================================
-- parameters description:
-- @AInParam01:�鵵ID��������ѡ���ͨ��
-- @AInParam02:�Ƿ��Ǳ��ؼ�¼��ʶ
-- @AInParam03:¼����¼�ļ���ˮ��
-- @AInParam04:�������ͣ�0��ɾ��1�鵵��2����
-- @AInParam05:�⻧ID
-- @AInParam06:��ѯ����ID
-- =========================================================================================

begin
   AOuterrornumber:=0;
   AOuterrorstring:='successful';
   v_count:=f_basic_getcharcount(AInParam01,'|');
   v_string:=AInParam01;


  --����ֵ
  AOutErrorNumber := 0;
  AOutErrorString := 'successful';

exception
  when others then
    rollback;
    v_err_num := sqlcode;
    v_err_msg  := substr(sqlerrm,1,200);
    AOutErrorNumber := v_err_num;
    AOutErrorString := v_err_msg;
end;
/
