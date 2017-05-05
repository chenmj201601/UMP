CREATE OR REPLACE Procedure zzy_data_xmlfile_create( in_table_name      varchar2,  --表名
                                                     in_where_sql       varchar2,  --where条件
                                                     in_tableflag       varchar2,  --是否初始化本表数据，1-初始化；0-不初始化
                                                     in_tenantflag      varchar2,  --是否初始化租户表数据，1-初始化；0-不初始化
                                                     in_inidatatype     varchar2,  --初始化方式。I-Insert；U-更新；P-追加；A-更新&追加；T-清除所有数据后插入
                                                     out_errornumber    out number,
                                                     out_errorstring    out nvarchar2)

  is

-- descriPtion:    <将表数据转成xml格式文件>
-- tyPe:           <过程>
-- create date:    <2015-02-04>
-- =========================================================================================
-- Parameters descriPtion:
--in_table_name      varchar2,  --表名
--in_tableflag      --是否初始化本表数据，1-初始化；0-不初始化
--in_tenantflag     --是否初始化租户表数据，1-初始化；0-不初始化
--in_inidatatype    --初始化方式。I-Insert；U-更新；P-追加；A-更新&追加；T-清除所有数据后插入

-- =========================================================================================
-- return descriPtion:
-- out_errornumber:成功后返回的系统自动编号bigint型
-- out_errorstring:模块编码错误说明
-- ==========================================================================================
-- 当数据中带有',<,<=,>,>=,&时会出现格式乱的情况，要将'避免  v_comment:=replace(v_comment,'''','');
-- 出现换行符，要避免 v_default:=replace(v_default,chr(10));

 /*&(逻辑与)  &amp;        
   <(小于  )  &lt;        
   >(大于  )  &gt;*/

-- ==========================================================================================
    type cursor_datarow is ref cursor;
    cur_rowlist      cursor_datarow;
    type cur_datacol is ref cursor;
    cur_collist      cur_datacol;    
    v_rowid          integer;           
    v_column_name    varchar2(100);
    v_data_type      varchar2(200);
    v_data_value     varchar2(4000);
    v_in_inidatatype varchar2(10);
    v_in_table_name  varchar(100);
    v_id             int;
    v_err_num        number;
    v_err_msg        nvarchar2(200);
    v_xmlstring      varchar2(4000);
    v_execsql        varchar2(4000);
    v_count          int;
    
begin
    
    v_execsql:='select count(1) from user_tables t where t.table_name=''ZZY_'||in_table_name||'''';
    ----Dbms_output.put_line(v_execsql);    
    execute immediate v_execsql into v_count;
    commit;  
    
    if v_count=0 then
      v_execsql:='create table ZZY_'||in_table_name||' as select *  from '||in_table_name;
      ----Dbms_output.put_line(v_execsql);
      execute immediate v_execsql;
      commit; 
            
      v_execsql:='alter table ZZY_'||in_table_name||' add (idtmp number(20))';    
      execute immediate v_execsql;
      commit;
      
      v_execsql:='update ZZY_'||in_table_name||' set idtmp=rownum';
      execute immediate v_execsql;
      commit; 
    end if;
 
    if in_inidatatype is null then
       v_in_inidatatype:='I';
    else
       v_in_inidatatype:=in_inidatatype;
    end if;
      
    --delete
    v_execsql:='delete from xml_table_file where xml_type=2 and xml_filename='''||in_table_name||'''';
    execute immediate v_execsql;
    commit;

    --xml_title
    v_xmlstring:='<?xml version="1.0" encoding="utf-8" standalone="yes"?>';
    v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id,xml_type) values('''||in_table_name||''','''||v_xmlstring||''',''1'',''2'')';
    execute immediate v_execsql;    
    --tabledefine_begin
    
    /*<!-- 
      P01:表名 
      P02:是否初始化本表数据，  1-初始化；0-不初始化 
      P03:是否初始化租户表数据，1-初始化；0-不初始化 
      P04:初始化方式， I-Insert；U-更新；P-追加；A-更新&追加；T-清除所有数据后插入
      <TableData P01="T_00_004" P02="1" P03="0" P04="I">  
    -->*/

    if substr(in_table_name,1,5)<>'T_44_' then 
      v_in_table_name:=substr(in_table_name,1,8); 
    else
      v_in_table_name:=in_table_name; 
    end if;  
    v_xmlstring:='<TableData P01="'||v_in_table_name||'" P02="'||in_tableflag||'" P03="'||in_tenantflag||'" P04="'||v_in_inidatatype||'">';
    v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id,xml_type) values('''||in_table_name||''','''||v_xmlstring||''',''2'',''2'')';
    ----Dbms_output.put_line(v_execsql); 
    execute immediate v_execsql;
    
    v_xmlstring:='  <RowsList>';
    v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id,xml_type) values('''||in_table_name||''','''||v_xmlstring||''',''3'',''2'')';
    execute immediate v_execsql;    

    if in_where_sql is not null then
      v_execsql := 'select idtmp from ZZY_'||in_table_name||' where 1=1 and '||in_where_sql; --vcwhere 是变量
    else
      v_execsql := 'select idtmp from ZZY_'||in_table_name; --vcwhere 是变量      
    end if;
    ----Dbms_output.put_line(v_execsql);             
    v_id:=3;
    
    open cur_rowlist for v_execsql; --行循环
    loop
      fetch cur_rowlist into v_rowid;
      exit when cur_rowlist%notfound ;        
        v_id:=v_id+1;
        v_xmlstring:='    <DataRow>';
        v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id,xml_type) values('''||in_table_name||''','''||v_xmlstring||''','''||v_id||''',''2'')';       
        Dbms_output.put_line(v_execsql);
        execute immediate v_execsql;
       
        v_execsql := 'select column_name,data_type from user_tab_columns t where table_name='''||in_table_name||'''';
        
        Dbms_output.put_line(substr(in_table_name,1,4));
        
        if substr(in_table_name,1,5)='T_44_' then
            
           v_execsql:=v_execsql||' order by COLUMN_ID';
      
        else
           v_execsql:=v_execsql||' order by COLUMN_NAME';      
        end if;
    
        Dbms_output.put_line(v_execsql);
        open cur_collist for v_execsql; --列循环
        loop
          fetch cur_collist into v_column_name,v_data_type;
          exit when cur_collist%notfound ;
          
             ---Dbms_output.put_line(v_column_name); 
             v_id:=v_id+1;    
             v_xmlstring:='      <'||v_column_name||'>';
 
             if v_data_type='DATE' then
               v_execsql:='select to_char('||v_column_name||',''yyyy-mm-dd hh24:mi:ss'') from ZZY_'||in_table_name||' where idtmp='''||v_rowid||''''; 
             else
               v_execsql:='select '||v_column_name||' from ZZY_'||in_table_name||' where idtmp='''||v_rowid||''''; 
             end if;      
             ----Dbms_output.put_line(v_execsql);
             execute immediate v_execsql into v_data_value;  
             
             
             v_data_value:=replace(v_data_value,'''',''); --不能有'
             
             if (in_table_name='T_00_005' and upper(v_column_name)='C005')  then
  
               if  substr(v_data_value,length(v_data_value),1)=chr(10) then
                   v_data_value:=substr(v_data_value,1,length(v_data_value)-1);
               end if;
                   
                if  substr(v_data_value,length(v_data_value),1)=chr(13) then
                   v_data_value:=substr(v_data_value,1,length(v_data_value)-1); 
                end if;                                    
               --v_data_value:=replace(v_data_value,chr(10)); --不能有换行符，有的语言里有换行
               --v_data_value:=replace(v_data_value,chr(13)); --不能有回车
             elsif (in_table_name='T_00_005' and upper(v_column_name)<>'C005') then
               v_data_value:=replace(v_data_value,chr(10)); --不能有换行符
               v_data_value:=replace(v_data_value,chr(13)); --不能有回车               
             end if;
             
             ----Dbms_output.put_line(v_data_value);        
             
             v_xmlstring:=v_xmlstring||v_data_value;
             v_xmlstring:=v_xmlstring||'</'||v_column_name||'>';
             v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id,xml_type) values('''||in_table_name||''','''||v_xmlstring||''','''||v_id||''',''2'')';       
             ----Dbms_output.put_line(v_execsql);
             execute immediate v_execsql; 
             COMMIT;      
              
       end loop;
       close cur_collist; 
             
       v_id:=v_id+1; 
       v_xmlstring:='    </DataRow>';
       v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id,xml_type) values('''||in_table_name||''','''||v_xmlstring||''','''||v_id||''',''2'')';       
       ----Dbms_output.put_line(v_execsql);
       execute immediate v_execsql;
       COMMIT;
             
    end loop;
    close cur_rowlist;
    

    v_id:=v_id+1;
    v_xmlstring:='  </RowsList>';
    v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id,xml_type) values('''||in_table_name||''','''||v_xmlstring||''','''||v_id||''',''2'')';
    ------Dbms_output.put_line(v_execsql);    
    execute immediate v_execsql;      

    v_id:=v_id+1;
    v_xmlstring:='</TableData>';
    v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id,xml_type) values('''||in_table_name||''','''||v_xmlstring||''','''||v_id||''',''2'')';
    ------Dbms_output.put_line(v_execsql);    
    execute immediate v_execsql;        
    commit; 

    v_execsql:='select count(1) from user_tables t where t.table_name=''ZZY_'||in_table_name||'''';
    ------Dbms_output.put_line(v_execsql);      
    execute immediate v_execsql into v_count;
    commit;  
    
    if v_count>0 then
          
      v_execsql:='drop table ZZY_'||in_table_name;
      execute immediate v_execsql;
      commit;
    end if;
    
    out_errornumber :=0;
    out_errorstring :='successful';

exception
  when others then
    rollback;
    v_execsql:='drop table ZZY_'||in_table_name;
    execute immediate v_execsql;
    commit;
    
    v_err_num := sqlcode;
    v_err_msg  := substr(sqlerrm,1,200);
    out_errornumber := v_err_num;
    out_errorstring := v_err_msg;
end;
