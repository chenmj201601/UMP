﻿CREATE OR REPLACE Procedure ZZY_TABLE_XMLFILE_CREATE(in_table_name      varchar2,
                                                     out_errornumber    out number,
                                                     out_errorstring    out nvarchar2)

  is

-- descriPtion:    <将建表语句转成xml格式文件>
-- tyPe:           <过程>
-- create date:    <2015-01-21>
-- =========================================================================================
-- Parameters descriPtion:
-- @table_name:       表名
-- @database_tyPe :   数据库类型
-- @Partition_flag:  分区标志    

-- =========================================================================================
-- return descriPtion:
-- @aoutParam01:成功后返回的系统自动编号bigint型
--          -10：模块编码错误，
-- ==========================================================================================
--主键规则 PK_T_00_001
--索引规则 IX_11_007_00000_002005不能超过32个字符，即索引不能超过5个字段
-- 数据类型定义 xml_datatyPe

    v_err_num        number;
    v_err_msg        nvarchar2(200);
    v_xmlstring      varchar2(4000);
    v_execsql        varchar2(4000);
    tyPe cur_tyPe is ref cursor;
    cur_cloumn       cur_tyPe;

    tyPe cur_tyPe_detail is ref cursor;
    cur_cloumn_detail cur_tyPe_detail;
        
    v_column_name   varchar2(50);
    v_data_type     varchar2(20);
    v_data_width    int;        
    v_data_dec      int;
    v_isnull        varchar2(5);         
    v_default       varchar2(100); 
    v_comment       nvarchar2(200);
    v_datatyPeid    varchar2(2);
    v_token         varchar2(1);
    v_table_comment varchar2(500);
    v_database_tyPe varchar2(10);
    v_Partition_flag varchar2(1);  --是否允许创建逻辑分区表,0:不允许; 1:允许
    v_index_name     varchar2(50);
    v_index_name_sub varchar2(50);    
    v_primary        varchar2(1);
    v_unique         varchar2(1);
    v_constraint_type varchar2(10);
    v_order           varchar2(5);
    v_id             int;
    v_in_table_name  varchar(100);
    v_table_ini      varchar(1); --本表初始化
    v_tenant_ini     varchar(1); --租户表初始化

begin
  
    
    v_database_tyPe:='oracle';

    v_Partition_flag:='0';
    
    --delete
    v_execsql:='delete from xml_table_file where xml_filename='''||in_table_name||'''';
    execute immediate v_execsql;
    --dbms_outPut.Put_line(v_execsql);
    commit;

    --xml_title
    v_xmlstring:='<?xml version="1.0" encoding="utf-8" standalone="yes"?>';
    v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id) values('''||in_table_name||''','''||v_xmlstring||''',''1'')';
    --dbms_outPut.Put_line(v_execsql);
    execute immediate v_execsql;
    --commit;    
    --tabledefine_begin
    
    /**<!--
    P01:表名
    P02:版本，目前固定为 8.02.001
    P03:是否创建同义词,0:不创建; 1:创建
    P04:是否带租户创建,0:不带租户Token; 1:带租户Token;2:总表有，租户表也有，即T_21_001及T_21_001_00000都存在 T_11_901及T_11_901_00000都存在
    P05:是否允许创建逻辑分区表,0:不允许; 1:允许
    P06:表描述
    P07:备用
    -->**/
    
    select  t.table_ini,t.tenant_ini  into v_table_ini,v_tenant_ini  from xml_table_list t where t.table_name=in_table_name;
   
    if v_table_ini='1' and v_tenant_ini='1' then
      v_token:='2'; 
    
    elsif v_table_ini='1' and v_tenant_ini='0'  then
       v_token:='0';
       
    elsif v_table_ini='0'  and  v_tenant_ini='1'   then
      v_token:='1';
      
    end if;

   /* if (substr(in_table_name,1,4)='T_00') or (in_table_name='T_51_001') or (substr(in_table_name,1,4)='T_99') or (substr(in_table_name,1,4)='T_25') or (substr(in_table_name,1,4)='T_24') then 
      v_token:='0'; --不带租户
    else
      v_token:='1'; --带租户
    end if;
    
    if  (in_table_name='T_21_001') or (substr(in_table_name,1,8)='T_11_901') then
       v_token:='2'; --带租户和不带租户都存在    
    end if;
    
    if  (in_table_name='T_21_000') or (substr(in_table_name,1,8)='T_21_999') or (substr(in_table_name,1,8)='T_21_998')  then
       v_token:='0'; --不带租户  
    end if;  */ 
    

    select comments into v_table_comment  from user_tab_comments where table_name=in_table_name;

    if instr(in_table_name,'44')=0 then
      v_in_table_name:=substr(in_table_name,1,8);
    else
      v_in_table_name:=in_table_name;
    end if;

    v_xmlstring:='<TableDefine P01="'||v_in_table_name||'" P02="8.02.002" P03="0" P04="'||v_token||'" P05="'||v_Partition_flag||'" P06="'||v_table_comment||'" P07="备用">';
    v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id) values('''||in_table_name||''','''||v_xmlstring||''',''2'')';
    --dbms_output.put_line(v_execsql);
    execute immediate v_execsql;
    --commit;     
    --columndefine_begin
    
    /**<!--
    P01:字段名
    P02:数据类型，对应DBDataTyPeConvert.xml中TyPe0的值
    P03:Width-长度（字符型数据该值必须填写，NUMBER/NUMERIC必须填写，整型数据该值 = 0；时间0，NVARCHAR2是实际的*2，需要处理/2）
    P04:Dec-小数位数（字符型数据该值 = 0； 整型数据该值 = 0；NUMBER/NUMERIC必须填写）
    P05:是否允许为空,0:不允许; 1:允许
    P06:默认值
    P07:描述
    -->**/
    
    v_xmlstring:='  <ColumnDefine>';
    v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id) values('''||in_table_name||''','''||v_xmlstring||''',''3'')';
    execute immediate v_execsql;
    --commit;

    v_execsql := 'select a.column_name,
                  a.data_type,
                  case when a.data_type=''NUMBER''    then a.data_precision
                       when a.data_type=''DATE''      then 0
                       when a.data_type=''NVARCHAR2'' then data_length/2
                       else data_length end as data_length,
                  nvl(data_scale,''0''),
                  case when nullable=''Y'' then ''1'' else ''0'' end as nullable,
                  b.comments   
                  from user_tab_columns a,user_col_comments b 
                  where a.table_name=b.table_name 
                  and a.column_name=b.column_name 
                  and a.table_name='''||in_table_name||'''';
    
    if substr(in_table_name,1,5)='T_44_' then
            
       v_execsql:=v_execsql||' order by COLUMN_ID';
      
    else
       v_execsql:=v_execsql||' order by COLUMN_NAME';      
    end if;
    
    dbms_output.put_line(v_execsql);
    
     --vcwhere 是变量  --44用COLUMN_ID
    dbms_output.put_line(v_execsql);             
    v_id:=3;
    open cur_cloumn for v_execsql; 
    loop
      fetch cur_cloumn into v_column_name,v_data_type,v_data_width,v_data_dec,v_isnull,v_comment;
      exit when cur_cloumn%notfound ;
       v_id:=v_id+1;
       if v_database_type='oracle' then
          select datatypeid into v_datatypeid from xml_datatype where datatypeoracle=v_data_type;
          
          if (v_data_type='NUMBER' and  v_data_width=20)   then
             v_datatypeid:='03';
          elsif (v_data_type='NUMBER' and  v_data_width=11) then
             v_datatypeid:='02';
          elsif (v_data_type='NUMBER' and  v_data_width=5) then
             v_datatypeid:='01'; 
          end if;  
                     
       else
          select datatypeid into v_datatypeid from xml_datatyPe where datatyPesql=v_data_type; 
            
       end if;   
        
       v_default:='';
       select long_2_varchar(in_table_name,v_column_name) into v_default  from dual;
       dbms_output.put_line(v_default);       
       if substr(v_default,1,1)='''' then
         v_default:=TRIM(v_default);        
         v_default:=''''||v_default||'''';
         v_default:=replace(v_default,chr(10));
         v_default:=TRIM(v_default);      
       end if;
       
       --dbms_output.put_line(v_default);
            
       --v_comment:=replace(v_comment,'''','');  
       v_comment:='';--将注释设为空，保密          
       
       v_xmlstring:='    <Column P01="'||v_column_name||'" P02="'||v_datatyPeid||'" P03="'||v_data_width||'" P04="'||v_data_dec||'" P05="'||v_isnull||'" P06="'||v_default||'" P07="'||v_comment||'"/>';
       v_xmlstring:=replace(v_xmlstring,chr(10));--删除换行符
       v_xmlstring:=replace(v_xmlstring,chr(13));--删除回车     
       v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id) values('''||in_table_name||''','''||v_xmlstring||''','''||v_id||''')';       
       --dbms_output.put_line(v_execsql);
       execute immediate v_execsql;
       --commit;       
    end loop;
    close cur_cloumn;

    --columndefine_detail_end
    v_id:=v_id+1;
    v_xmlstring:='  </ColumnDefine>';
    v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id) values('''||in_table_name||''','''||v_xmlstring||''','''||v_id||''')'; 
    execute immediate v_execsql; 
    --commit;    
    --columndefine_end  
    
        
    --indexdefine_begin
    v_id:=v_id+1;
    v_xmlstring:='  <IndexDefine>';
    v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id) values('''||in_table_name||''','''||v_xmlstring||''','''||v_id||''')'; 
    
    execute immediate v_execsql;    
    --commit;    
    /**<!--
    P01:索引名
    P02:是否为主键,1:主键; 0:非主键
    P03:是否为唯一索引,1:是; 0:非唯一索引
    -->**/
                
    --indexdefine_detail_begin
               
    v_execsql:='select index_name
                from user_indexes a
                where a.table_name = '''||in_table_name||'''';
                                
    --dbms_output.put_line(v_execsql);                        
    open cur_cloumn for v_execsql; 
    loop 
      fetch cur_cloumn into v_index_name;
      exit when cur_cloumn%notfound;
      
      v_id:=v_id+1;
      
      v_Primary:='0';
      v_unique:='0';
          
      v_constraint_type:='';
      v_execsql:='select distinct au.constraint_type
                from user_cons_columns cu, user_constraints au
                where cu.constraint_name = au.constraint_name
                and au.constraint_type in (''P'')
                and au.table_name ='''||in_table_name||'''
                and cu.constraint_name='''||v_index_name||''''; 
      
      begin          
        execute immediate v_execsql into v_constraint_type;
        --commit;
      exception
         when others then null;
      end;

      if v_constraint_type='P' then
         v_Primary:='1';
      else
         v_Primary:='0';
      end if;
      
      v_constraint_type:='';      
      
      select t.uniqueness into v_constraint_type
                from user_indexes t
                where t.index_name=v_index_name ;
                   
      --DBMS_OUTPUT.put_line(v_constraint_type);
      if v_constraint_type='UNIQUE' then
         v_unique:='1';         
      else
         v_unique:='0';
      end if; 

      --DBMS_OUTPUT.put_line(v_index_name);
   
      --if v_token=1 then
        --v_index_name_sub:=substr(v_index_name,1,length(v_index_name)-6);
      --else
        v_index_name_sub:=v_index_name;
      --end if;
        
      --DBMS_OUTPUT.put_line(v_index_name);
      
      
      v_xmlstring:='     <Index P01="'||v_index_name_sub||'" P02="'||v_Primary||'" P03="'||v_unique||'">';
      v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id) values('''||in_table_name||''','''||v_xmlstring||''','''||v_id||''')'; 
      --dbms_output.put_line(v_execsql);
      execute immediate v_execsql;
      --commit;                 

      /**<!--
      P01:字段名
      P02:排序顺序; A:升序; D:降序
      -->**/ 
       
     v_execsql:='select t.column_name,t.descend 
                  from user_ind_columns t,user_indexes i 
                  where t.index_name = i.index_name
                  and t.index_name='''||v_index_name||''' 
                  and t.table_name = '''||in_table_name||'''';
                                             
     open cur_cloumn_detail for v_execsql; 
     loop 
       fetch cur_cloumn_detail into v_column_name,v_order;
       exit when cur_cloumn_detail%notfound;
        v_id:=v_id+1;
        if v_order='ASC' then
           v_order:='A';
        else
           v_order:='D';
        end if;
 
       v_xmlstring:='      <Column P01="'||v_column_name||'" P02="'||v_order||'"/>';
       v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id) values('''||in_table_name||''','''||v_xmlstring||''','''||v_id||''')';
       commit;
        --DBMS_OUTPUT.put_line(v_execsql);
     execute immediate v_execsql;
     --commit;        
     
     end loop;    
     close cur_cloumn_detail;
     
     --DBMS_OUTPUT.put_line('01');                         
      /*<!--
      P01:字段名
      P02:排序顺序; A:升序; D:降序
      -->*/
      v_id:=v_id+1;
      v_xmlstring:='    </Index>';
      v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id) values('''||in_table_name||''','''||v_xmlstring||''','''||v_id||''')';
      --DBMS_OUTPUT.put_line(v_execsql);
      execute immediate v_execsql; 
      --commit;        
    end loop;
        
    close cur_cloumn;    
        
    --indexdefine_detail_end
    v_id:=v_id+1;     
    v_xmlstring:='  </IndexDefine>';
    v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id) values('''||in_table_name||''','''||v_xmlstring||''','''||v_id||''')';
    --DBMS_OUTPUT.put_line(v_execsql);
    execute immediate v_execsql;
    --commit;                 
    --indexdefine_end
    
    --tabledefine_end
    v_id:=v_id+1;         
    v_xmlstring:='</TableDefine>';
    v_execsql:='insert into xml_table_file(xml_filename,xml_string,xml_id) values('''||in_table_name||''','''||v_xmlstring||''','''||v_id||''')';
    --DBMS_OUTPUT.put_line(v_execsql);
    execute immediate v_execsql;       
    commit; 

    out_errornumber :=0;
    out_errorstring :='successful';

exception
  when others then
    rollback;
    v_err_num := sqlcode;
    v_err_msg  := substr(sqlerrm,1,200);
    out_errornumber := v_err_num;
    out_errorstring := v_err_msg;
end;
