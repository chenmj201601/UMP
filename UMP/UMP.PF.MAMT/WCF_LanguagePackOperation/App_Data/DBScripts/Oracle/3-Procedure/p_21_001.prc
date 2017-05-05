create or replace procedure p_21_001 (    AInParam01      in nvarchar2,
                                          AInParam02      in nvarchar2,
                                          AInParam03      in nvarchar2,
                                          AInParam04      in nvarchar2,
                                          AInParam05      in nvarchar2,
                                          AInParam06      in nvarchar2,
                                          AInParam07      in nvarchar2,
                                          AInParam08      in nvarchar2,
                                          AInParam09      in nvarchar2,
                                          AInParam10      in nvarchar2,
                                          AOutErrorNumber out number,
                                          AOutErrorString out nvarchar2)

-- type:           过程，维护记录表为T_21_001
-- create date:    <2014-08-6>
-- description:    <录音记录接口表的维护>
-- =========================================================================================
-- parameters description:
-- @AInParam01:   输入的列字段及列值 格式为c001char(27)valuechar(27)char(27)c002vluechar(27)char(27)c003vlue
-- .....
-- @AInParam10:   输入的列字段及列值
-- =========================================================================================
-- out description:
-- @AOutErrorNumber:错误编号
-- @AOutErrorString:错误描述
-- 租户编号是AInParam01中的一部分
-- ==========================================================================================
-- 使用此方法要DBA赋权grant create any context to pfdev ;
-- login as USERNAME:create or replace context MY_CTX using P_21_001;  
-- ==========================================================================================
is
  v_err_num        number;
  v_err_msg        nvarchar2(200);
  i                integer; 
  v_inparam        nvarchar2(2000);
  v_execsql        varchar2(4000);    
 
  
  v_valueint       number(20);
  v_valuechar      nvarchar2(2000);   
  v_valuedate      date; 
  v_columnname     varchar2(10);
  v_columnpar      nvarchar2(2000);
    
  v_columnnamelist varchar2(4000);  
  v_columnvalue    varchar2(4000);
  
  v_columnparlist  varchar2(4000);
  v_datatype       varchar2(10);
  n_count          integer;
  n_c001           number(20);
  n_c002           number(20); 
  v_c003           varchar2(5);
  v_c005           varchar2(20);
  v_c005datestr    varchar2(20); 
  n_c012           number(5);  
  v_c013           varchar2(8);
  v_c016           varchar2(20);
  v_c017           varchar2(20);
  v_c018           varchar2(5);            
  aoutSerialNumber number(11);
  aoutSerialHour   number(20);
 
begin

  --初始化

  v_err_num:='0';
  v_err_msg:='successful';
  
  SELECT COUNT(*)
    INTO n_count
    FROM USER_TABLES
   WHERE TABLE_NAME = 'T_99_001';
   
  IF n_count < 1 THEN
    EXECUTE IMMEDIATE 'CREATE GLOBAL TEMPORARY TABLE T_99_001(C001 varchar2(5),
                                                                  C002 nvarchar2(2000)) ON COMMIT PRESERVE ROWS';
                                                                                                                                    
  END IF;
  EXECUTE IMMEDIATE 'TRUNCATE TABLE T_99_001';
    
  for i in 1..10 loop
    
    v_inparam:='';
    
    if    i=1 then
      
      if AInParam01 is not null then
        v_inparam:=AInParam01;
      else
        exit;
      end if;
      
    elsif i=2 then
    
      if AInParam02 is not null then
        v_inparam:=AInParam02;
      else
        exit;
      end if;
    elsif i=3 then
      if AInParam03 is not null then    
        v_inparam:=AInParam03;
      else
        exit;
      end if;            
    elsif i=4 then
      if AInParam04 is not null then    
        v_inparam:=AInParam04;
      else
        exit;
      end if;      
    elsif i=5 then
      if AInParam05 is not null then    
        v_inparam:=AInParam05;
      else
        exit;
      end if;        
    elsif i=6 then
      if AInParam06 is not null then    
        v_inparam:=AInParam06;
      else
        exit;
      end if;      
    elsif i=7 then
      if AInParam07 is not null then    
        v_inparam:=AInParam07;
      else
        exit;
      end if;        
    elsif i=8 then
      if AInParam08 is not null then    
        v_inparam:=AInParam08;
      else
        exit;
      end if;      
    elsif i=9 then
      if AInParam09 is not null then    
        v_inparam:=AInParam09;
      else
        exit;
      end if;        
    elsif i=10 then
      if AInParam10 is not null then    
        v_inparam:=AInParam10;
      else
        exit;
      end if;      
    end if;

    if v_inparam is not null then
      P_21_002(v_inparam,v_err_num,v_err_msg);
    end if;
    
    if v_err_num<>0 then
       AOuterrornumber := -1;
       AOuterrorstring := 'failed';
       return;     
    end if; 
                
  end loop;
 
  v_execsql:='select  to_char(f_basic_getutcdate,''yyyymmddhh24miss'') from dual';
  execute immediate v_execsql into v_c016;
  
  v_c017:=v_c016;
     
  v_execsql:='select ROUND(TO_NUMBER(sysdate -f_basic_getutcdate ) * 24 * 60) from dual';
  execute immediate v_execsql into v_c018;
  
  select t.C002 into v_c003 from T_99_001 t where upper(t.C001)='C003';
  select t.C002 into v_c005 from T_99_001 t where upper(t.C001)='C005';
  select to_number(t.C002) into n_c012 from T_99_001 t where upper(t.C001)='C012';
  
  v_execsql:='select f_basic_secondtohhmmss('||n_c012||')  from dual';
  execute immediate v_execsql into v_c013;  
  v_execsql:=''; 
      
  v_c005datestr:=to_char(to_date(v_c005,'yyyy-mm-dd hh24:mi:ss'),'yyyymmddhh24miss');  
  
  p_00_001('21','201',v_c003,'20140101000000',aoutSerialHour,v_err_num,v_err_msg);--201 插入录音记录表操作

  if v_err_num=0 and aoutSerialHour>0 then
     aoutSerialNumber:=substr(aoutSerialHour,12,9);    
     n_c001 := aoutSerialNumber;
  else
     AOuterrornumber := to_number(aoutSerialHour);
     AOuterrorstring := 'Serial number generation error';
     return;     
  end if;

  p_00_001('21','201',v_c003,v_c005datestr,aoutSerialHour,v_err_num,v_err_msg);
 
  if v_err_num=0 and aoutSerialHour>0 then
     n_c002 := aoutSerialHour;
  else
     AOuterrornumber := to_number(aoutSerialHour);
     AOuterrorstring := 'Serial number generation error';
     return;     
  end if; 
  
    
   
  v_columnnamelist:='insert into T_21_001(C001,C002,C013,C016,C017,C018'; 
  v_columnparlist:=') values (sys_context(''MY_CTX'',''C001''),sys_context(''MY_CTX'',''C002''),sys_context(''MY_CTX'',''C013''),sys_context(''MY_CTX'',''C016''),sys_context(''MY_CTX'',''C017''),sys_context(''MY_CTX'',''C018'')';

      
  for   c_tab  in (select C001,C002 from T_99_001 order by c001)  
  loop  
        
        v_columnname:=c_tab.C001;
        v_columnvalue:=c_tab.C002;        
        
        v_valueint:='';
        v_valuedate:='';
        v_valuechar:='';
          
        select data_type into v_datatype from user_tab_columns where table_name='T_21_001' and column_name=upper(v_columnname);
            
        if v_datatype='NUMBER' then               
          v_valueint:=to_number(v_columnvalue);                
            
        elsif v_datatype='DATE' then               
           null;
        else
          v_valuechar:=v_columnvalue;
                                 
        end if;
        
        v_columnnamelist:=v_columnnamelist||','||v_columnname; 
        
        if v_datatype='DATE' then
          v_columnpar:='to_date('''||v_columnvalue||''',''yyyy-mm-dd hh24:mi:ss'')';
        else           
          v_columnpar:='sys_context(''MY_CTX'','''||v_columnname||''')';  
        end if;
               
        v_columnparlist:=v_columnparlist||','||v_columnpar; 

                            
        if v_valueint is not null then
           dbms_session.set_context('MY_CTX',v_columnname,v_valueint);
        elsif v_valuechar is not null then
             dbms_session.set_context('MY_CTX',v_columnname,v_valuechar);            
        end if;        
         
  end loop;

  dbms_session.set_context('MY_CTX','C001',n_c001);
  dbms_session.set_context('MY_CTX','C002',n_c002);
  dbms_session.set_context('MY_CTX','C013',v_c013);

  dbms_session.set_context('MY_CTX','C016',v_c016);
  dbms_session.set_context('MY_CTX','C017',v_c017);
  dbms_session.set_context('MY_CTX','C018',v_c018);  
    
  v_execsql:=v_columnnamelist||v_columnparlist||')';
  execute immediate v_execsql;
  
  commit;

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
