CREATE OR REPLACE PROCEDURE p_11_014 (
   AInParam01          VARCHAR2 := '00000',
   AInParam02          VARCHAR2,
   AInParam03          VARCHAR2,
   AInParam04          NVARCHAR2,
   AInParam05          CHAR := '1',
   AInParam06          CHAR := '0',
   AInParam07          VARCHAR2 := '11111111111111111111111111111111',
   AInParam08          VARCHAR2,
   AInParam09          VARCHAR2,
   AInParam10          VARCHAR2,
   AInParam11          VARCHAR2,
   AoutParam01         OUT   VARCHAR2,
   AOutErrorNumber     OUT   NUMBER,
   AOutErrorString     OUT   NVARCHAR2
)
IS

-- description:    新增角色，记录目标表为T_11_004_00000
-- create date:    <2014-9-12>
-- =========================================================================================
--AInParam01        租户ID（默认为‘00000’）
--AInParam02        父级角色编号
--AInParam03        应用于哪个模块
--AInParam04        角色名称
--AInParam05        是否活动
--AInParam06        是否删除
--AInParam07        其他状态
--AInParam08        开始时间
--AInParam09        失效时间
--AInParam10        创建人
--AInParam11        创建时间
-- =========================================================================================
--aoutparam01       新角色的ID
-- =========================================================================================
   v_serialid    VARCHAR2 (19);
   v_err_num     NUMBER;
   v_err_msg     NVARCHAR2 (200);
   v_AInParam01  VARCHAR2 (5);
   v_AInParam05  VARCHAR2 (1);
   v_AInParam06  VARCHAR2 (1);
   v_AInParam07  VARCHAR2 (32);
   v_AInParam11  VARCHAR2 (30);   
   v_execsql     varchar2 (4000);
   v_count       number;
BEGIN
  
   if AInParam01 is null then
     v_AInParam01:='00000';
   else
     v_AInParam01:=AInParam01;
   end if;
   
   v_execsql :='select count(1) from  t_11_004_'||v_AInParam01||' where c004='''||AInParam04||'''';
   --dbms_output.put_line(v_execsql);    
      
   EXECUTE IMMEDIATE v_execsql into v_count;
  
   if v_count>0 
   then
      AOuterrornumber := '-1';
      AOuterrorstring := 'Role already exists';
      return;         
   end if; 

   if AInParam05 is null then
     v_AInParam05:='1';
   else
     v_AInParam05:=AInParam05;
   end if;

   if AInParam06 is null then
     v_AInParam06:='0';
   else
     v_AInParam06:=AInParam06;
   end if;

   if AInParam07 is null then
     v_AInParam07:='11111111111111111111111111111111';
   else
     v_AInParam07:=AInParam07;
   end if;
   
  IF AInParam11 IS NULL
   THEN
      v_AInParam11 := to_char(SYSDATE,'yyyy-mm-dd hh24:mi:ss');
   ELSE
      v_AInParam11 := AInParam11;
   END IF;
   
   
   
   p_00_001 ('11', '106', v_AInParam01, NULL, v_serialid, v_err_num, v_err_msg); --取得流水号  模块大类编号是11，小类是104
   
   if v_serialid<0 or v_err_num <>0 then
     AOuterrornumber := to_number(v_serialid);
     AOuterrorstring := 'Serial number generation error';     
     Return;
   end if;   

   v_execsql:='INSERT INTO t_11_004_'||v_AInParam01||'
            VALUES ('''||v_serialid||''','''||AInParam02||''', '''||AInParam03||''','''||AInParam04||''','''||v_AInParam05||''',
                '''||v_AInParam06||''', '''||v_AInParam07||''','''||AInParam08||''','''||AInParam09||''','''||AInParam10||''','||
                'to_date('''||v_AInParam11||''',''yyyy-mm-dd hh24:mi:ss''))';

   execute immediate v_execsql;
   commit;
   aoutparam01 := to_number(v_serialid);

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
