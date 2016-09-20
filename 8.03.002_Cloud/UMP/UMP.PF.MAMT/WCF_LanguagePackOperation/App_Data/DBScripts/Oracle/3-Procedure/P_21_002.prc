create or replace procedure P_21_002 (AInParam01       in  nvarchar2,
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

-- create date:    <2014-08-7>
-- description:    <字符串拆分成表格式 维护T_21_001时用到>
-- =========================================================================================
-- parameters description:
-- @AInParam01:传入字符串
-- =========================================================================================
-- return description:
-- @tab_column_value_set:将传入字符串拆分成列名和列值
-- ==========================================================================================
begin
   AOuterrornumber:=0;
   AOuterrorstring:='successful';
   
   v_string:=AInParam01;
   
   if substr(v_string,length(v_string)-1,2)<>chr(27)||chr(27) then
     v_string:=v_string||chr(27)||chr(27);
   end if; 
     
   v_count:=f_basic_getcharcount(v_string,chr(27)||chr(27));

   for i in 0..v_count-1 loop      

          if i=0 then
            v_temp:=substr(v_string,1,instr(v_string,chr(27)||chr(27))-1);
          else
            v_temp:=substr(v_string,instr(v_string,chr(27)||chr(27),1,i)+2,instr(v_string,chr(27)||chr(27),1,i+1)-2-instr(v_string,chr(27)||chr(27),1,i));
          end if;
          dbms_output.put_line(v_temp);
          v_columnname :=substr(v_temp,1,instr(v_string,chr(27))-1);
          v_columnvalue:=substr(v_temp,instr(v_string,chr(27))+1);
          
          dbms_output.put_line(v_columnname);          
          dbms_output.put_line(v_columnvalue);
          
          insert into T_99_001(C001,C002) values (v_columnname,v_columnvalue);
          commit;
          
          v_columnname:='';
          v_columnvalue:='';
       end loop;

  --返回值
  AOuterrornumber := 0;
  AOuterrorstring := 'successful';

exception
  when others then
    rollback;
    v_err_num := sqlcode;
    v_err_msg  := substr(sqlerrm,1,200);
    AOuterrornumber := v_err_num;
    AOuterrorstring := v_err_msg;
end;
/
