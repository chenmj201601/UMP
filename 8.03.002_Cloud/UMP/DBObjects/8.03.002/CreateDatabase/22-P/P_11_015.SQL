CREATE PROCEDURE P_11_015 
(
   @AInParam01       VARCHAR(5) = '00000',
   @AInParam02       VARCHAR(20),
   @AInParam03       VARCHAR(20),
   @AInParam04       VARCHAR(20),
   @AInParam05       VARCHAR(20),
   @AInParam06       VARCHAR(20),
   @AOutErrorNumber  BIGINT         output,
   @AOutErrorString  NVARCHAR(4000) output
)
AS

-- description:    用户、角色管理或控制的资源列表为T_11_201_00000
-- create date:    <2014-9-19>
-- =========================================================================================
--AInParam01        租户ID（默认为‘00000’）
--AInParam02        父级编号，对应本表 C001，默认值 0
--AInParam03        用户、角色、技能组编号
--AInParam04        被控制、管理对象的编号
--AInParam05        开始时间
--AInParam06        结束时间
-- =========================================================================================
--aoutparam01       用户、角色管理或控制的资源列表的ID
-- =========================================================================================

   DECLARE @v_serialid    VARCHAR (19);
   DECLARE @v_err_num     BIGINT;
   DECLARE @v_err_msg     VARCHAR (200);  
   DECLARE @v_execsql     NVARCHAR (4000);
   DECLARE @v_count       INT;
   DECLARE @error         BIGINT;
   DECLARE @errorSum      SMALLINT; 
   DECLARE @v_datetime    VARCHAR (19);

BEGIN TRY

   SET @v_execsql ='SELECT @v_count=COUNT(1) FROM  T_11_201_'+@AInParam01+' WHERE C003='''+@AInParam03+'''  AND C004='''+@AInParam04+'''';
   print @v_execsql
   EXECUTE SP_EXECUTESQL @v_execsql, N'@v_count INT OUTPUT', @v_count Output

   IF @v_count>0 
   BEGIN
      SET @AOuterrornumber = -1;
      SET @AOuterrorstring = 'Record already exists';
      RETURN;         
   END; 
  
   SELECT @v_datetime=SUBSTRING(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(100), GETDATE(), 25) , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14)

   EXEC P_00_001  '11','105',@AInParam01, @v_datetime,@v_serialid OUTPUT, @v_err_num OUTPUT, @v_err_msg OUTPUT

   IF  @v_err_num<>0
   BEGIN

     SET @AOutErrorNumber=-2
     SET @AOutErrorString='Serial number generation error'
	 RETURN
   END

   SET @v_execsql ='INSERT INTO T_11_201_'+@AInParam01+' VALUES ('+@v_serialid+','+@AInParam02+','+@AInParam03
               +','+@AInParam04+',CONVERT(DATETIME, '''+@ainparam05+''')'
               +',CONVERT(DATETIME, '''+@ainparam06+'''))';
   
   print @v_execsql

   EXEC SP_EXECUTESQL @v_execsql
   SET @error=@@error

   IF @error=0 
   BEGIN     
		SET @AOutErrorNumber = '0';
		SET @AOutErrorString = 'successful';
   END
   ELSE
   BEGIN
		SET @AOutErrorNumber = '-1';
		SET @AOutErrorString = 'failed';
   END

END TRY
BEGIN CATCH
	SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH

