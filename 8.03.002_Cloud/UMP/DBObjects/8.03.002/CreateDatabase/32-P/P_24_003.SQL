CREATE PROCEDURE P_24_003
(
  AInParam01  varchar2,
  AInParam02  varchar2,
  AInParam03  varchar2,
  AOutParam01 out nvarchar2,
  AOutErrorNumber out int ,
  AOutErrorString out nvarchar2
)
AS
 v_IntGeneratorCount    int;
 v_AInParam02           VARCHAR2(20);
 v_err_num              NUMBER;
 v_err_msg              NVARCHAR2 (2000);   


BEGIN

  AOutErrorNumber:= 0;
  AOutErrorString := '';
  AOutParam01 := '';
  
  v_AInParam02:=replace(ainparam02,'-');
  v_AInParam02:=replace(v_AInParam02,' ');
  v_AInParam02:=replace(v_AInParam02,':');
  
  SELECT COUNT(1) into v_IntGeneratorCount  from t_24_004 WHERE c002 = AInParam03 AND c006 = '1';

  IF(v_IntGeneratorCount = 0) THEN
      AOutParam01 := 'GeneratorIDError';
  ELSE
      UPDATE T_24_005 SET C008 = v_AInParam02 WHERE C001 = AInParam01;
      COMMIT;
  END IF;
EXCEPTION
   WHEN OTHERS
   THEN
      ROLLBACK;
      v_err_num := SQLCODE;
      v_err_msg := SUBSTR (SQLERRM, 1, 200);
      AOuterrornumber := v_err_num;
      AOuterrorstring := v_err_msg;
END;
