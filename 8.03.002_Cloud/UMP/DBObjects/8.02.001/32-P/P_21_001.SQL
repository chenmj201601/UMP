CREATE PROCEDURE P_21_001
(
  AInParam01      IN NVARCHAR2,
  AInParam02      IN NVARCHAR2,
  AInParam03      IN NVARCHAR2,
  AInParam04      IN NVARCHAR2,
  AInParam05      IN NVARCHAR2,
  AInParam06      IN NVARCHAR2,
  AInParam07      IN NVARCHAR2,
  AInParam08      IN NVARCHAR2,
  AInParam09      IN NVARCHAR2,
  AInParam10      IN NVARCHAR2,
  AOutErrorNumber OUT NUMBER,
  AOutErrorString OUT NVARCHAR2)
AS
  LStrSerialNumber       VARCHAR2(20);
  LIntErrorNumber        NUMBER(10);
  LStrErrorMessage       NVARCHAR2(2000);
  v_err_num        NUMBER;
  v_err_msg        NVARCHAR2(200);
  --录音记录表DB来的数据直接写入临时表T_21_000，进入由服务后台处理写入T_21_001
BEGIN
   
  AOutErrorNumber := 0;
	LStrSerialNumber := 0;
	LIntErrorNumber := 0;
  
  INSERT INTO T_21_000 VALUES(LStrSerialNumber, AInParam01, AInParam02, AInParam03, AInParam04, AInParam05, AInParam06, AInParam07, AInParam08, AInParam09, AInParam10, LIntErrorNumber,LStrErrorMessage, SYSDATE);

  COMMIT;

EXCEPTION
  WHEN OTHERS THEN
    v_err_num := SQLCODE;
    v_err_msg  := substr(SQLERRM,1,200);
    AOuterrornumber := v_err_num;
    AOuterrorstring := v_err_msg;
END;
