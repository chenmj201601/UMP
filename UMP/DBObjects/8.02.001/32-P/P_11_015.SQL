CREATE PROCEDURE P_11_015 (
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
-- ==========================================================
--aoutparam01       用户、角色管理或控制的资源列表的ID
-- ==========================================================
--流水号 模块号是11 操作码是105
-- ==========================================================
   v_serialid    VARCHAR2 (19);
   v_err_num     NUMBER;
   v_err_msg     NVARCHAR2 (200);
   v_AInParam01  VARCHAR2 (5);  
   v_execsql     varchar2 (4000);
   v_count       NUMBER;
BEGIN
  
   IF AInParam01 IS NULL THEN
     v_AInParam01:='00000';
   ELSE
     v_AInParam01:=AInParam01;
   END IF;
     
   v_execsql :='SELECT COUNT(1) FROM  t_11_201_'||v_AInParam01||' WHERE C003='''||AInParam03||''' AND C004='''||AInParam04||'''';
   --dbms_output.put_line(v_execsql);    
      
   EXECUTE IMMEDIATE v_execsql into v_count;
  
   IF v_count>0 
   THEN
      AOuterrornumber := -1;
      AOuterrorstring := 'Record already exists';
      RETURN;         
   END IF; 

   P_00_001 ('11', '105', v_AInParam01, NULL, v_serialid, v_err_num, v_err_msg); --取得流水号  模块大类编号是11，小类是105
   
   iF v_err_num <>0 THEN
     AOuterrornumber := -2;
     AOuterrorstring := 'Serial number generation error';     
     RETURN;
   END IF;   

   v_execsql:='INSERT INTO T_11_201_'||v_AInParam01||'
            VALUES ('''||v_serialid||''','''||AInParam02||''','''||AInParam03||''', '''||AInParam04||''',TO_DATE('''||AInParam05||''',''yyyy-mm-dd hh24:mi:ss''),'||
                'TO_DATE('''||AInParam06||''',''yyyy-mm-dd hh24:mi:ss''))';

   EXECUTE IMMEDIATE v_execsql;
   COMMIT;

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