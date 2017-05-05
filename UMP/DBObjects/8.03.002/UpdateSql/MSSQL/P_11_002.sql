CREATE PROCEDURE [dbo].[P_11_002]
(@AInParam01 VARCHAR(5)='00000',
 @AInParam02 VARCHAR(20),
 @AOutParam01		VARCHAR(5)	     output,
 @AOutErrorNumber	INT				 output,
 @AOutErrorString	NVARCHAR(200)	 output)
AS

   DECLARE  @vOrgID     VARCHAR(20); --机构ID
   DECLARE  @vToken     VARCHAR(5);
   DECLARE  @v_execsql  NVARCHAR (4000);
   DECLARE  @tablenume  VARCHAR(20);
   DECLARE @error_num     BIGINT

BEGIN TRY

   SET @AOutErrorNumber= 0;
   SET @AOutErrorString= 'Successful';
   set @vToken=0;

   TRUNCATE TABLE T_99_004

   SET  @tablenume='T_11_006_'+@AInParam01;
  
   SET @v_execsql='WITH TEMP AS ( 
     SELECT c001,c004 FROM '+@tablenume+' a WHERE C001='+@AInParam02+' 
     UNION ALL  
     SELECT B.c001,b.c004  FROM '+@tablenume+' B INNER JOIN TEMP T ON B.C001 = T.C004)  
   
     INSERT T_99_004 SELECT C001 FROM temp';
   print @v_execsql
   EXEC SP_EXECUTESQL @v_execsql
   SET @error_num=@@ERROR


  SELECT * FROM MASTER.dbo.syscursors WHERE cursor_name='mCursor'

  IF CURSOR_STATUS('global','mCursor')=1 
  BEGIN
    CLOSE mCursor
    DEALLOCATE mCursor
  END

   DECLARE mCursor cursor GLOBAL FOR SELECT C001 FROM T_99_004 ORDER BY  C001 DESC;
   OPEN mCursor
   WHILE 1=1
   BEGIN
     FETCH NEXT FROM mCursor INTO @vOrgID

	 IF @@FETCH_STATUS <> 0
     break

	          
	 IF (SUBSTRING(@vOrgID,1,3)='101') AND (SUBSTRING(@vOrgID,4,8)='14010100') 
	 BEGIN
        SET @vToken=SUBSTRING(@vOrgID,15,5);
		print @vToken
        SET @AOutParam01=@vToken; 
	 END 
     ELSE
	 BEGIN
        CONTINUE; 
     END; 

   END      
   CLOSE mCursor
   DEALLOCATE mCursor
	
   SET @AOutParam01=@vToken;

END TRY

BEGIN CATCH
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE(),@AOutParam01=0;
END CATCH