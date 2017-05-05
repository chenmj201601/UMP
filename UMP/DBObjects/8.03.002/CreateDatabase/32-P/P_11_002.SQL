CREATE OR REPLACE PROCEDURE P_11_002
 (AInparam01 VARCHAR2,
  AInparam02 VARCHAR2,
  AOutParam01		      OUT   VARCHAR,
  AOutErrorNumber     OUT   NUMBER,
  AOutErrorString     OUT   NVARCHAR2) 
AS 
   v_TenantID VARCHAR2(20);
   vOrgID     VARCHAR2(20); --机构ID
   vToken     VARCHAR2(5);
   V_EXECSQL  varchar2 (4000);
   type CUR_TYPE is ref cursor;
   CUR  CUR_TYPE;

BEGIN
    IF ainparam01 IS NULL THEN
      v_TenantID:='00000';
    ELSE
      v_TenantID:=ainparam01;
    END IF;
    vToken:=0;
    aouterrornumber := '0';
    aouterrorstring := 'successful';

    V_EXECSQL :='SELECT C001 FROM  t_11_006_'||v_TenantID||' START WITH C001='||AInparam02||' connect by C001 = prior C004 ORDER BY C001 DESC';
    dbms_output.put_line(V_EXECSQL);
    
    OPEN cur for V_EXECSQL;
    LOOP
      FETCH CUR INTO vOrgID;
        EXIT WHEN cur%notfound;
      dbms_output.put_line(vOrgID);
      IF (substr(vOrgID,1,3)='101') AND (substr(vOrgID,4,8)='14010100') THEN
        vToken:=substr(vOrgID,15,5);
        AOutParam01:=vToken;
        EXIT;
      ELSE
        CONTINUE;
      END IF;
    END LOOP;
    CLOSE CUR;
    AOutParam01:=vToken;

EXCEPTION
   WHEN OTHERS
   THEN
      aouterrornumber := SQLCODE;
      aouterrorstring := SUBSTR (SQLERRM, 1, 200);
END;
