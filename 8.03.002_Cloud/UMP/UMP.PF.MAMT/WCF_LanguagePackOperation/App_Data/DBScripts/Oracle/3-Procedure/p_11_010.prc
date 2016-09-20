CREATE OR REPLACE PROCEDURE P_11_010 (
   AInParam01              VARCHAR2 := '00000',
   AInParam02              NVARCHAR2,
   AInParam03              VARCHAR2,
   AInParam04              VARCHAR2,
   AInParam05              CHAR := '1',
   AInParam06              CHAR := '0',
   AInParam07              VARCHAR2 := '11111111111111111111111111111111',
   AInParam08              VARCHAR2,
   AInParam09              VARCHAR2,
   AInParam10              VARCHAR2,
   AInParam11              VARCHAR2,
   AInParam12              NVARCHAR2,
   AOutParam01       OUT   VARCHAR2,
   AOutErrorNumber   OUT   NUMBER,
   AOutErrorString   OUT   NVARCHAR2
)
IS
-- description:    新机构的ID生成过程，记录目标表为机构部门T_11_006_00000,用户、角色管理或控制的资源列表t_11_201_00000
-- create date:    <2014-9-3>
-- =========================================================================================
--AInParam01        租户ID（默认为‘00000’）
--AInParam02        机构名称
--AInParam03        机构类型
--AInParam04        父级机构ID
--AInParam05        是否活动
--AInParam06        是否删除
--AInParam07        其他状态
--AInParam08        开始时间
--AInParam09        失效时间
--AInParam10        创建人
--AInParam11        创建时间
--AInParam12        描述
-- =========================================================================================
--aoutparam01       新机构的ID
   v_serialid     VARCHAR2 (19);
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);
   v_AInParam01   VARCHAR2 (5);
   v_AInParam05   VARCHAR2 (1);
   v_AInParam06   VARCHAR2 (1);
   v_AInParam07   VARCHAR2 (32);
   v_AInParam11   VARCHAR2 (20);   
   v_execsql      VARCHAR2 (500);
BEGIN
   IF AInParam01 IS NULL
   THEN
      v_AInParam01 := '00000';
   ELSE
      v_AInParam01 := AInParam01;
   END IF;

   IF AInParam05 IS NULL
   THEN
      v_AInParam05 := '1';
   ELSE
      v_AInParam05 := AInParam05;
   END IF;

   IF AInParam06 IS NULL
   THEN
      v_AInParam06 := '0';
   ELSE
      v_AInParam06 := AInParam06;
   END IF;

   IF AInParam07 IS NULL
   THEN
      v_AInParam07 := '11111111111111111111111111111111';
   ELSE
      v_AInParam07 := AInParam07;
   END IF;
   
   IF AInParam11 IS NULL
   THEN
      v_AInParam11 := to_char(sysdate,'yyyy-mm-dd hh24:mi:ss');
   ELSE
      v_AInParam11 := AInParam11;
   END IF;   

   p_00_001 ('11', '101', v_AInParam01, NULL, v_serialid, v_err_num,
             v_err_msg);
             
   if v_serialid<0 or v_err_num <>0 then
     AOuterrornumber := to_number(v_serialid);
     AOuterrorstring := 'Serial number generation error';     
     Return;
   end if;
                
   v_execsql :=
         'INSERT INTO t_11_006_'
      || v_AInParam01
      || '  
            VALUES ('''
      || v_serialid
      || ''','''
      || AInParam02
      || ''', '''
      || AInParam03
      || ''','''
      || AInParam04
      || ''','''
      || v_AInParam05
      || ''','''
      || v_AInParam06
      || ''', '''
      || v_AInParam07
      || ''','''
      || AInParam08
      || ''','''
      || AInParam09
      || ''','''
      || AInParam10
      || ''',to_date('''
      || v_AInParam11
      || ''',''yyyy-mm-dd hh24:mi:ss''),'''
      || AInParam12
      || ''')';

   EXECUTE IMMEDIATE v_execsql;

   v_execsql :=
         'INSERT INTO t_11_201_'
      || v_AInParam01
      || '  
            VALUES (0, 0, '
      || AInParam10
      || ','
      || v_serialid
      || ',sysdate,'
      || 'to_date(''2199-12-31'',''YYYY-MM-DD HH24:MI:SS''))';
   
   --dbms_output.put_line(v_execsql);
 

   EXECUTE IMMEDIATE v_execsql;

   COMMIT;
   aoutparam01 := v_serialid;
   aouterrornumber := '0';
   aouterrorstring := 'successful';
EXCEPTION
   WHEN OTHERS
   THEN
      ROLLBACK;
      v_err_num := SQLCODE;
      v_err_msg := SUBSTR (SQLERRM, 1, 200);
      aouterrornumber := v_err_num;
      aouterrorstring := v_err_msg;
END; 
/
