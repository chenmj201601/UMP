CREATE PROCEDURE P_31_003 (
   AInParam01              VARCHAR2 := '00000',
   AInParam02              NVARCHAR2,
   AInParam03              NVARCHAR2,
   AInParam04              VARCHAR2,
   AInParam05              VARCHAR2,
   AInParam06              VARCHAR2 ,
   AInParam07              VARCHAR2,
   AInParam08              VARCHAR2,
   AInParam09              VARCHAR2,
   AInParam10              VARCHAR2,
   AInParam11              VARCHAR2,
   AInParam13              VARCHAR2,
   AInParam14              VARCHAR2,
   AInParam15              VARCHAR2,
   AInParam16              NVARCHAR2,
   AInParam17              VARCHAR2,
   AInParam18              VARCHAR2,
   AOutParam01       OUT   VARCHAR2,
   AOutErrornumber   OUT   NUMBER,
   AOutErrorstring   OUT   NVARCHAR2
)
IS
-- description:    自动任务设置 T_31_020_00000
-- create date:    <2014-11-21>
-- =========================================================================================
--AInParam01        租户标识（默认为‘00000’）
--AInParam02        任务名称
--AInParam03        任务描述
--AInParam04        任务类别  1 初检手任务 2初检自动任务，2复检手动任务 3复检自动任务 4推荐录音初检 5推荐录音复检  6 QA质检任务（质检但不计入座席成绩） 7智能任务分配 4:下拉枚举 5:多选 6:复选 10:时间范围 11:时长范围
--AInParam05        是否共享任务 Y共享任务  N非共享任务
--AInParam06        分配时间
--AInParam07        分配人ID ,对应 T_11_005.c001
--AInParam08        分配数量
--AInParam09        完成时间
--AInParam10        完成数量
--AInParam11        日期还有多少天到期发通知

--AInParam13        任务所属年
--AInParam14        任务所属月
--AInParam15        任务完成时间
--AInParam16        分配人帐号名称
--AInParam17        记录总时长
--AInParam18        操作码  由前台来传进来 308 初检 309 复检
-- =========================================================================================
--AOutParam01       返回流水号ID   
-- =========================================================================================
   v_serialid     VARCHAR2 (19);
   v_err_num      NUMBER;
   v_err_msg      NVARCHAR2 (200);
   v_AInParam01   VARCHAR2 (5);
   v_execsql      VARCHAR2 (1024);

BEGIN
   IF AInParam01 IS NULL
   THEN
      v_AInParam01 := '00000';
   ELSE
      v_AInParam01 := AInParam01;
   END IF;

   P_00_001 ('31', AInParam18, v_AInParam01, NULL, v_serialid, v_err_num,
             v_err_msg);

   IF v_serialid<0 OR v_err_num <>0 THEN
     AOuterrornumber := -1;
     AOuterrorstring := 'Serial number generation error';
     RETURN;
   END IF;

  v_execsql :=
         'INSERT INTO t_31_020_'
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
      || ''',to_date('''
      || AInParam06
      || ''',''yyyy-mm-dd hh24:mi:ss''), '''
      || AInParam07
      || ''','''
      || AInParam08
      || ''',to_date('''
      || AInParam09
      || ''',''yyyy-mm-dd hh24:mi:ss''),''' --完成时间
      || AInParam10                         --完成数量
      || ''','''','''','''                  --最后一次修改任务的时间和人员      
      || AInParam11
      || ''',''' 
      || AInParam13
      || ''','''      
      || AInParam14
      || ''',''N'',to_date('''
      || AInParam15
      || ''',''yyyy-mm-dd hh24:mi:ss''),''' --任务完成时间    
      || AInParam16                         --分配人帐号名称
      || ''','''','                         --最后修改人帐号名称
      || AInParam17                         --记录总时长
      || ',0'                               --任务中已评分录音总时长                                             
      || ')';

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