CREATE  PROCEDURE P_25_001
(AInParam01          VARCHAR2,
 AInParam02          VARCHAR2,
 AOutParam01     OUT VARCHAR2,
 AOutErrorNumber OUT NUMBER,
 AOutErrorString OUT NVARCHAR2)
AS
    v_err_num        NUMBER;
    v_err_msg        NVARCHAR2(200);

    
BEGIN
  
  UPDATE T_25_005
     SET C004=(CASE C003 WHEN 0 THEN SYSDATE ELSE C004 END),
     C003=C003+1,
     C005=SYSDATE
  WHERE  C001=AInParam01 AND C002=AInParam02; 
  
  AOutParam01 :=SQL%ROWCOUNT;
  
  COMMIT;

  AOutErrorNumber :=0;
  AOutErrorString :='successful';

EXCEPTION
  WHEN OTHERS THEN
    ROLLBACK;
    v_err_num := SQLCODE;
    v_err_msg  := substr(SQLERRM,1,200);
    AOutErrorNumber := v_err_num;
    AOutErrorString := v_err_msg;
    
END;
