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
-- description:    <将查询任务的记录写入到归档备份预处理任务表>
-- =========================================================================================
-- parameters description:
-- @AInParam01:归档ID，即界面选择的通道
-- @AInParam02:是否是本地记录标识
-- @AInParam03:录音记录文件流水号
-- @AInParam04:任务类型，0回删，1归档，2备份
-- @AInParam05:租户ID
-- @AInParam06:查询任务ID
-- =========================================================================================

begin
   AOuterrornumber:=0;
   AOuterrorstring:='successful';
   v_count:=f_basic_getcharcount(AInParam01,'|');
   v_string:=AInParam01;


  --返回值
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
