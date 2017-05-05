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
