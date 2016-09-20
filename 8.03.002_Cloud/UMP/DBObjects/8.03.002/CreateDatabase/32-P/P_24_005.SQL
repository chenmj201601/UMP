CREATE PROCEDURE P_24_005                                                                                                                                                
(                                                                                                                                                                                   
  AInParam00 varchar2,                                                                                                                                                              
  AInParam01 varchar2,                                                                                                                                                              
  AInParam02 varchar2,                                                                                                                                                              
  AInparam03 char,                                                                                                                                                                  
  AInparam04 char,                                                                                                                                                                  
  AInparam05 char,                                                                                                                                                                  
  AInparam06 varchar2,                                                                                                                                                              
  AOutParam01 OUT nvarchar2,                                                                                                                                                        
  result out sys_refcursor,                                                                                                                                                         
  AOutErrorNumber OUT int ,                                                                                                                                                         
  AOutErrorString OUT nvarchar2                                                                                                                                                     
)                                                                                                                                                                                   
IS                                                                                                                                                                                  
                                                      
   v_IntGeneratorCount      int;                                                                                                                                                    
   v_AInParam00             varchar(20);                                                                                                                                            
   v_sql                    varchar(4000);                                                                                                                                          
                                                                                                                                                                                    
                                                                                                                                                                                    
BEGIN                                                                                                                                                                               
	AOutErrorNumber:= 0;                                                                                                                                                              
  AOutErrorString:= '';                                                                                                                                                             
  AOutParam01 := '';                                                                                                                                                                
                                                                                                                                                                                    
  if v_AInParam00 is null then                                                                                                                                                      
    v_AInParam00:='00000';                                                                                                                                                          
  else                                                                                                                                                                              
    v_AInParam00:=AInParam00;                                                                                                                                                       
  end if;                                                                                                                                                                           
                                                                                                                                                                                    
  SELECT COUNT(1) into v_IntGeneratorCount  from t_24_004 WHERE c002 = AInParam06 AND c006 = '1';                                                                                   
                                                                                                                                                                                    
  IF(V_IntGeneratorCount = 0) THEN                                                                                                                                                  
      AOutParam01:='GeneratorIDError';                                                                                                                                              
      RETURN;                                                                                                                                                                       
  END IF;                                                                                                                                                                           
                                                                                                                                                                                    
  DELETE FROM T_99_006;                                                                                                                                                             
  COMMIT;                                                                                                                                                                           
  v_sql:='                                                                                                                                                                          
  INSERT INTO T_99_006                                                                                                                                                              
  SELECT C028 FROM T_24_001 WHERE C001 = '||AInParam01||' AND ''1'' = '||AInparam03||                                                                                               
  ' UNION                                                                                                                                                                           
  SELECT C006 FROM T_24_002 WHERE C003 = '||AInParam01||' AND ''1'' = '||AInparam04||                                                                                               
  ' UNION                                                                                                                                                                           
  SELECT C003 FROM T_11_201_'||v_AInParam00||' WHERE  ''1'' ='||AInparam05||                                                                                                        
    ' AND C004 IN (select c001 from t_11_101_'||v_AInParam00||' where substr(c017,10,100) in ( SELECT                                                                               
     C002 FROM T_24_002 WHERE C001 = ''1'' AND C003 = '||AInParam01||') and c001 like ''221%'') and c004 like ''102%''';                                                            
                                                                                                                                                                                    
                                                                                                                                                                                    
  dbms_output.put_line(v_sql);                                                                                                                                                      
  EXECUTE IMMEDIATE v_sql;                                                                                                                                                          
  COMMIT;                                                                                                                                                                           
                                                                                                                                                                                    
  v_sql:='SELECT distinct A.ColUserID AS UserID,nvl(B.C011,'''') AS EmailAddress,C.C005 AS LanguageID FROM T_99_006 A, t_11_101_'||v_AInParam00||' B,T_11_011_'||v_AInParam00||' C  
  WHERE A.ColUserID=C.C001  AND A.ColUserID=B.C001 AND C.C002=''1100101''';                                                                                                         
                                                                                                                                                                                    
  open result for v_sql;                                                                                                                                                            
                                                                                                                                                                                    
                                                                                                                                                                                    
EXCEPTION                                                                                                                                                                           
   WHEN OTHERS                                                                                                                                                                      
   THEN                                                                                                                                                                             
      ROLLBACK;                                                                                                                                                                     
      AOuterrornumber := SQLCODE;                                                                                                                                                   
      AOuterrorstring := SUBSTR (SQLERRM, 1, 200);                                                                                                                                  
END;                                                                                                                                                                                