CREATE PROCEDURE P_31_004 (
   AInParam01              VARCHAR2 := '00000',
   AInParam02              VARCHAR2,
   AInParam03              VARCHAR2,
   AInParam04              VARCHAR2,
   AInParam05              VARCHAR2,
   AInParam06              VARCHAR2 ,
   AInParam07              VARCHAR2,
   AInParam08              VARCHAR2,
   AInParam09              VARCHAR2,
   AInParam10              VARCHAR2,
   AInParam11              VARCHAR2,
   AInParam12              VARCHAR2,
   AOutParam01       OUT   VARCHAR2,
   AOutErrornumber   OUT   NUMBER,
   AOutErrorstring   OUT   NVARCHAR2
)
IS
-- description:    维护申诉详情表 T_31_019_00000
-- create date:    <2015-2-11>
-- =========================================================================================
--AInParam01        租户标识（默认为‘00000’）
--AInParam02        T_31_008_00000.c001评分成绩表的成绩ID
--AInParam03        录音记录表的ID,T_21_001.C002
--AInParam04        录音所属座席工号对应的资源编号
--AInParam05        申诉人ID,如果是座席自己申诉的
--AInParam06        申诉流程ID
--AInParam07        申诉流程子项ID(通过ActionID+AppealFlowItemID得到该流程走到那一步了)
--AInParam08        1为申诉，2为审批，3为复核
--AInParam09        对于申诉(1座席申诉，2他人替申)，对于审批(3审批驳回,4审批过通)，对于复核（5复核通过不修改分数，6复核通过重新评分，7复核驳回）
--AInParam10        Y为申诉流程完毕，N 在处理流程中
--AInParam11        当能多次申诉时启用，每再申诉一次+1，客户端第一版写1
--AInParam12        创建时间
-- =========================================================================================
--AOutParam01       返回流水号ID   
-- =========================================================================================
   v_serialid     VARCHAR2 (19);
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);
   v_AInParam01   VARCHAR2 (5);
   v_execsql      VARCHAR2 (1024);
   v_datetime     VARCHAR2 (19);

BEGIN
   IF AInParam01 IS NULL
   THEN
      v_AInParam01 := '00000';
   ELSE
      v_AInParam01 := AInParam01;
   END IF;

   SELECT to_char(sysdate,'yyyymmddhh24miss') INTO v_datetime FROM DUAL;
   
   P_00_001 ('31', '312', v_AInParam01, v_datetime, v_serialid, v_err_num,
             v_err_msg);

   IF v_serialid<0 OR v_err_num <>0 THEN
     AOuterrornumber := -1;
     AOuterrorstring := 'Serial number generation error';
     RETURN;
   END IF;

  v_execsql :=
         'INSERT INTO t_31_019_'
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
      || AInParam05
      || ''','''
      || AInParam06
      || ''','''
      || AInParam07
      || ''','''
      || AInParam08
      || ''','''
      || AInParam09
      || ''','''    
      || AInParam10 
      || ''','''          
      || AInParam11
      || ''',to_date('''
      || AInParam12
      || ''',''yyyy-mm-dd hh24:mi:ss''))';

   --DBMS_OUTPUT.put_line (v_execsql);
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
