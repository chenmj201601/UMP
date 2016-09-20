CREATE  PROCEDURE P_24_004
( AInParam00 varchar,
  AInParam01 varchar,
  AInparam02 varchar2,
  AOutParam01 OUT nvarchar2,
  AOutParam02 OUT nvarchar2,
  AOutParam03 OUT nvarchar2,
  AOutParam04 OUT nvarchar2,
  AOutParam05 OUT nvarchar2,
  AOutParam06 OUT nvarchar2,
  AOutParam07 OUT nvarchar2,
  AOutErrorNumber out int ,
  AOutErrorString out nvarchar2  
)
AS

   v_IntGeneratorCount int;
   v_AOutParam01       varchar2(25);
   v_AInParam00        varchar2(5);
   v_sql               varchar2(4000);

BEGIN 
  AOutErrorNumber:= 0;
  AOutErrorString := '';
  
  if AInParam00 is null then
    v_AInParam00:='00000';
  else
    v_AInParam00:=AInParam00;
  end if;    
  
  AOutParam01:='';
  AOutParam02:='';
  AOutParam03:='';
  AOutParam04:='';
  AOutParam05:='';
  AOutParam06:='';
  AOutParam07:='';

  SELECT COUNT(1) into v_IntGeneratorCount  from t_24_004 WHERE c002 = AInParam02 AND c006 = '1';

  IF (v_IntGeneratorCount = 0) THEN
      AOutParam01 := 'GeneratorIDError';
      AOutErrorNumber:= -1;
      AOutErrorString := 'FAIL';     
      RETURN;
  END IF;
 
	v_sql:='SELECT   COUNT(1)  from T_11_001_'||v_AInParam00||' WHERE C004 = '||AInparam01;
  execute immediate v_sql into v_IntGeneratorCount ;

	IF(V_IntGeneratorCount = 0) THEN
			AOutParam01 := 'NotSetting';
      RETURN;
  END IF;
  
	v_sql:='SELECT  c006   FROM T_11_001_'||v_AInParam00||' WHERE C001 = 0 and C002 =24 and C003  = 240401';
  execute immediate v_sql into V_AOutParam01 ;  
	IF(V_AOutParam01 = '0') THEN
			AOutParam01 := 'NotSent';
      RETURN;
	END IF;

  v_sql:='SELECT C006 FROM T_11_001_'||v_AInParam00||' WHERE C001 = 0 and C002 =24 and C003  = 240402'; 
  execute immediate v_sql into AOutParam01 ;    
	v_sql:='SELECT C006 FROM T_11_001_'||v_AInParam00||' WHERE C001 = 0 and C002 =24 and C003  = 240403'; 
  execute immediate v_sql into AOutParam02 ;   
	v_sql:='SELECT C006 FROM T_11_001_'||v_AInParam00||' WHERE C001 = 0 and C002 =24 and C003  = 240404'; 
  execute immediate v_sql into AOutParam03 ;   
	v_sql:='SELECT C006 FROM T_11_001_'||v_AInParam00||' WHERE C001 = 0 and C002 =24 and C003  = 240405'; 
  execute immediate v_sql into AOutParam04 ;   
	v_sql:='SELECT C006 FROM T_11_001_'||v_AInParam00||' WHERE C001 = 0 and C002 =24 and C003  = 240406'; 
  execute immediate v_sql into AOutParam05 ;   
	v_sql:='SELECT C006 FROM T_11_001_'||v_AInParam00||' WHERE C001 = 0 and C002 =24 and C003  = 240407'; 
  execute immediate v_sql into AOutParam06 ;   
	v_sql:='SELECT C006 FROM T_11_001_'||v_AInParam00||' WHERE C001 = 0 and C002 =24 and C003  = 240408'; 
  execute immediate v_sql into AOutParam07 ;
    
EXCEPTION
   WHEN OTHERS
   THEN
      ROLLBACK;
      AOuterrornumber := SQLCODE;
      AOuterrorstring := SUBSTR (SQLERRM, 1, 200);
END;
