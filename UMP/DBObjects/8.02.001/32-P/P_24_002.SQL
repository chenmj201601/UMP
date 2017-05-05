CREATE PROCEDURE P_24_002
(
  AInParam01 varchar2,
  AInparam02 varchar2,
  AInParam03 varchar2,
  AInParam04 varchar2,
  AInParam05 varchar2,
  AInParam06 varchar2,
  AInParam07 varchar2,
  AInParam08 char,
  AInParam09 varchar2,
  AOutParam01 out varchar2,
  AOutErrorNumber out int ,
  AOutErrorString out nvarchar2)
-- ==========================================================================================================================
-- Name:    P_24_002   P_EncryptionProcessing01   T_24_005
-- Type:    存储过程
-- Create date:    <2015-7-24>
-- Description:    <加密生成器向密钥字典中插入数据>
-- ==========================================================================================================================
-- Parameters Description:
-- @AInParam01:    策略ID
-- @AInParam02:    加密版本
-- @AInParam03:    加密密钥 Key1b
-- @AInParam04:    加密密钥 Key1d
-- @AInParam05:    密钥启用UTC时间
-- @AInParam06:    密钥失效UTC时间
-- @AInParam07:    密钥验证码
-- @AInParam08:    创建方式。U：用户立即创建；C：生成器创建
-- @AInParam09:    GeneratorID
-- @AOutParam01:  插入成功后返回的KeyID,如果 = 'GeneratorIDError',则说明GeneratorID与系统设置的不匹配，没能插入到数据库
-- ==========================================================================================================================
AS
  v_StrKeyID             NVARCHAR2(25);
  v_IntGeneratorCount    INT;
  v_AInParam05           VARCHAR2(20);
  v_AInParam06           VARCHAR2(20);
  v_datetime             VARCHAR2 (14); 
  v_err_num              NUMBER;
  v_err_msg              NVARCHAR2 (2000); 
  v_AInparam02           VARCHAR2(5);
  v_utcdate              DATE;
BEGIN  

  AOutErrorNumber:= 0;
  AOutErrorString := '';
  
  IF AInparam02 IS NULL then 
    v_AInparam02:='2';
  ELSE
    v_AInparam02:=AInparam02;
  END IF;
  
  
  v_AInParam05:=replace(ainparam05,'-');
  v_AInParam05:=replace(v_AInParam05,' ');
  v_AInParam05:=replace(v_AInParam05,':'); 
     
  v_AInParam06:=replace(ainparam06,'-');
  v_AInParam06:=replace(v_AInParam06,' ');
  v_AInParam06:=replace(v_AInParam06,':'); 
    
  SELECT  COUNT(1) INTO v_IntGeneratorCount from T_24_004 WHERE C002 = AInParam09 AND C006 = '1';

  IF(v_IntGeneratorCount = 0) THEN
      AOutParam01 := 'GeneratorIDError';
  ELSE
      SELECT to_char(sysdate,'yyyymmddhh24miss') INTO v_datetime FROM DUAL; 
      P_00_001 ('24', '402', '00000', v_datetime, v_StrKeyID, v_err_num,v_err_msg);         

      IF v_err_num <> 0
      THEN
        aouterrornumber := -1;
        aouterrorstring := 'Serial number generation error';
        RETURN;
      END IF;
      
      select f_00_004 into v_utcdate from dual;
         
      INSERT INTO T_24_005(C001, C002, C003, C004, C005, C006, C007, C008,C009,C010, C011)
        VALUES(v_StrKeyID, AInParam01, v_AInParam02, AInParam03, AInParam04, TO_CHAR(v_utcdate,'YYYYMMDDHH24MISS'),v_AInParam05, v_AInParam06, AInParam07,0, AInParam08);
        
      commit;  
      
      AOutParam01 := v_StrKeyID;
      
  END IF;

EXCEPTION
  WHEN OTHERS THEN
    ROLLBACK;
    v_err_num := SQLCODE;
    v_err_msg  := substr(SQLERRM,1,200);
    AOutErrorNumber := v_err_num;
    AOutErrorString := v_err_msg;
END;
