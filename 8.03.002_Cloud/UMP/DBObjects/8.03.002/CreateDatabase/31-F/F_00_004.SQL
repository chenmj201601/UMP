CREATE FUNCTION F_00_004
   RETURN DATE
AS
   LDateReturn   DATE; 
BEGIN
   SELECT TO_DATE (
             TO_CHAR (SYS_EXTRACT_UTC (SYSTIMESTAMP),
                      'YYYY-MM-DD HH24:MI:SS'),
             'YYYY-MM-DD HH24:MI:SS')
     INTO LDateReturn
     FROM DUAL;

   RETURN LDateReturn;
END;

