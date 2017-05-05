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

-- description:    用户、角色管理或控制的资源列表为T_11_201_00000
-- create date:    <2014-9-19>
-- =========================================================================================
--AInParam01        租户ID（默认为‘00000’）
--AInParam02        父级编号，对应本表 C001，默认值 0
--AInParam03        用户、角色、技能组编号
--AInParam04        被控制、管理对象的编号
--AInParam05        开始时间
--AInParam06        结束时间
-- =========================================================================================
--aoutparam01       用户、角色管理或控制的资源列表的ID
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

   p_00_001 ('11', '105', v_AInParam01, NULL, v_serialid, v_err_num, v_err_msg); --取得流水号  模块大类编号是11，小类是105
   
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
