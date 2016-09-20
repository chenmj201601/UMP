CREATE PROCEDURE P_31_001 (
   AInParam01              VARCHAR2 := '00000',
   AInParam02              NVARCHAR2,
   AInParam03              VARCHAR2,
   AInParam04              VARCHAR2,
   AInParam05              VARCHAR2 := '0',
   AInParam06              NVARCHAR2 ,
   AInParam07              VARCHAR2 := '0',
   AInParam08              VARCHAR2 := '0',
   AInParam09              VARCHAR2, 
   AInParam10              NVARCHAR2,        
   AOutParam01       OUT   VARCHAR2,
   AOutErrornumber   OUT   NUMBER,
   AOutErrorstring   OUT   NVARCHAR2
)
IS
-- description:    增加自定义查询界面表记录t_31_040_00000
-- create date:    <2014-10-30>
-- =========================================================================================
--AInParam01        租户标识（默认为‘00000’）
--AInParam02        名称
--AInParam03        规格(1: 1*4 2: 1*2 3:1*1)
--AInParam04        类型(1:普通文件 2:模糊匹配文本 3:逗号间隔多值文本 4:下拉枚举 5:多选 6:复选 10:时间范围 11:时长范围
--AInParam05        TabItem索引(0-9内置标签)
--AInParam06        TabItem名称
--AInParam07        在一个TabItem下的顺号
--AInParam08        默认是否显示(1:显示 0:不显示)
--AInParam09        保留(标识使用)
--AInParam10        保留(额外数据使用)
-- =========================================================================================
--AOutParam01       返回流水号ID   31  303
-- =========================================================================================
   v_serialid     VARCHAR2 (19);
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);
   v_AInParam01   VARCHAR2 (5);
   
   v_AInParam05   VARCHAR2 (5);
   v_AInParam07   VARCHAR2 (5);
   v_AInParam08   VARCHAR2 (5); 
   v_execsql      VARCHAR2 (1024);

BEGIN
   IF AInParam01 IS NULL
   THEN
      v_AInParam01 := '00000';
   ELSE
      v_AInParam01 := AInParam01;
   END IF;

   IF AInParam05 IS NULL
   THEN
      v_AInParam05 := '0';
   ELSE
      v_AInParam05 := AInParam05;
   END IF;

   IF AInParam07 IS NULL
   THEN
      v_AInParam07 := '0';
   ELSE
      v_AInParam07 := AInParam07;
   END IF;
   
   IF AInParam08 IS NULL
   THEN
      v_AInParam08 := '0';
   ELSE
      v_AInParam08 := AInParam08;
   END IF; 
   
   P_00_001 ('31', '303', v_AInParam01, NULL, v_serialid, v_err_num,
             v_err_msg);

   IF v_serialid<0 OR v_err_num <>0 THEN
     AOuterrornumber := -1;
     AOuterrorstring := 'Serial number generation error';
     RETURN;
   END IF;

  v_execsql :=
         'INSERT INTO T_31_040_'
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
      || AInParam06
      || ''', '''
      || v_AInParam07
      || ''','''
      || v_AInParam08
      || ''','''
      || AInParam09
      || ''','''
      || AInParam10
      || ''')';
      
   DBMS_OUTPUT.put_line (v_execsql);
   EXECUTE IMMEDIATE v_execsql;

   COMMIT;
   
   AOutParam01 := v_serialid;
   AOutErrornumber := '0';
   AOutErrorstring := 'successful';
   
EXCEPTION
   WHEN OTHERS
   THEN
      ROLLBACK;
      v_err_num := SQLCODE;
      v_err_msg := SUBSTR (SQLERRM, 1, 200);
      aouterrornumber := v_err_num;
      aouterrorstring := v_err_msg;
END;