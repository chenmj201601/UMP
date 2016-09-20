CREATE PROCEDURE P_24_101
( AInParam01 int,
  AInParam02 char,
  AInParam03 varchar2,
  result out sys_refcursor,
  AOutErrorNumber OUT int ,
  AOutErrorString OUT nvarchar2
)

AS
  v_IntGeneratorCount         int;
  v_IntAddSeconds             int;
  v_DT_DurationEnd_Begin      date;
  v_DT_DurationEnd_End        date;
  v_DurationEnd_Begin         varchar2(20);
  v_DurationEnd_End           varchar2(20);

-- ==========================================================================================================================
-- Name:      F_EncryptionProcessing01
-- Type:      表值函数
-- Create date:    <2015-07-26>
-- Description:    <读取策略绑定即将到期的绑定信息>
-- ==========================================================================================================================
-- Parameters Description:
-- @AInParam01:    提前时间 int,如果该参数 = 0， 那么获取系统默认提前通知时间（值 = 密钥策略 85）
-- @AInParam02:    时间单位（天 = 'D'、小时='H'、分钟='M'）
-- @AInParam03:    GeneratorID HOST

--遗留问题  默认提前天数  不能解密写死

-- ==========================================================================================================================

BEGIN
  
  AOutErrorNumber:= 0;
  AOutErrorString := 'successful';
  
  DELETE FROM T_99_005;
  COMMIT;

  SELECT COUNT(1) into v_IntGeneratorCount  from t_24_004 WHERE c002 = AInParam03 AND c006 = '1';

  IF (v_IntGeneratorCount = 0) THEN
    
      INSERT INTO T_99_005 VALUES(-1, '', '', '', sysdate, sysdate);
      COMMIT;
      OPEN result FOR select * from T_99_005  ;
  
  ELSE

      SELECT F_00_004 INTO v_DT_DurationEnd_Begin from dual;--UTCSYSDATE
      IF(AInParam01 <= 0) THEN
          v_IntAddSeconds:=7 * 24 * 3600; --不能解密，写死
      ELSE
          IF(UPPER(AInParam02) = 'D') THEN
             v_IntAddSeconds := AInParam01 * 24 * 3600;
          ELSIF (UPPER(AInParam02) = 'H') THEN
             v_IntAddSeconds := AInParam01 * 3600;
          ELSIF (UPPER(AInParam02) = 'M') THEN
             v_IntAddSeconds := AInParam01 * 60;
          ELSE
            v_IntAddSeconds:=7 * 24 * 3600;
          END IF;
      END IF;

      SELECT v_DT_DurationEnd_Begin+v_IntAddSeconds/(24*60*60) into v_DT_DurationEnd_End FROM DUAL;

      select to_char(v_DT_DurationEnd_Begin,'yyyymmddhh24miss') into v_DurationEnd_Begin from dual;
      select to_char(v_DT_DurationEnd_End,'yyyymmddhh24miss')   into v_DurationEnd_End from dual;

      dbms_output.put_line(v_DurationEnd_Begin);
      dbms_output.put_line(v_DurationEnd_End);
          
      INSERT INTO T_99_005
        SELECT -1,a.C002, a.C003, b.C002, to_date(TO_CHAR(a.C004),'yyyy-mm-dd hh24:mi:ss'),to_date(TO_CHAR(a.C005),'yyyy-mm-dd hh24:mi:ss')
        FROM T_24_002 a,T_24_001 b
        WHERE A.C003=B.C001 AND TO_NUMBER(A.C004) >= TO_NUMBER(v_DurationEnd_Begin) 
                            AND TO_NUMBER(A.C005)<= TO_NUMBER(v_DurationEnd_End);
      commit;
      

      OPEN result FOR select * from T_99_005  ;
  
  END IF;

EXCEPTION
   WHEN OTHERS
   THEN
      ROLLBACK;
      AOuterrornumber := SQLCODE;
      AOuterrorstring := SUBSTR (SQLERRM, 1, 200);
END;
