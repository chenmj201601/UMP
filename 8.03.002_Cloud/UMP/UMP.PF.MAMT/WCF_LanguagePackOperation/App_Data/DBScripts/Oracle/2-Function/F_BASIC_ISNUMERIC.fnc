CREATE OR REPLACE FUNCTION F_BASIC_ISNUMERIC (AStrNumber VARCHAR2)
   RETURN NUMBER
AS --�Ƿ��������ж�
   LNumberConvert   NUMBER;
BEGIN
   LNumberConvert := TO_NUMBER (AStrNumber);
   RETURN 1;
EXCEPTION
   WHEN OTHERS
   THEN
      RETURN 0;
END;
/
