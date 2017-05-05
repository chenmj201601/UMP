CREATE PROCEDURE P_11_001 (
   @AInParam01          VARCHAR(5) = '00000',
   @AInParam02          VARCHAR(20),
   @AOutErrorNumber     BIGINT OUT,
   @AOutErrorString     NVARCHAR(4000) OUTPUT
)
AS

-- description:    将指定的用户添加到所在租户部门的租户管理的资源管理权限列表（T_11_201）中，仅限UMP CLOUD版
-- create date:    <2016-9-19>
-- ===============================================
--ainparam01        租户ID（默认为‘00000’）
--ainparam02        用户ID
-- ===============================================

   DECLARE @v_token       VARCHAR(5);
   DECLARE @v_userid      VARCHAR(20);
   DECLARE @v_err_num     BIGINT;
   DECLARE @v_err_msg     NVARCHAR (200);
   DECLARE @v_execsql     NVARCHAR (500);

BEGIN TRY

   print @AInParam01
   print @AInParam02

   EXEC P_11_002  @AInParam01,@AInParam02,@v_token OUTPUT, @v_err_num OUTPUT, @v_err_msg OUTPUT

   IF (CONVERT(BIGINT, @v_token)<0 or  @v_err_num<>0 )
   BEGIN
     SET @AOutErrorNumber=-1
     SET @AOutErrorString='Exec P_11_002 Fail'
   RETURN
   END


   PRINT @v_token
   SET @v_execsql='SELECT @v_userid=C001 FROM T_11_005_'+@AInParam01+' WHERE C006='+'10114010100000'+@v_token+' AND C027=''1''';
   print @v_execsql   
   EXECUTE sp_executesql @v_execsql, N'@v_userid VARCHAR(20) OUTPUT', @v_userid Output


   IF (@v_userid is null) 
   BEGIN
     SET @AOuterrornumber = '-1';
     SET @AOuterrorstring = 'Can not find the parent USERID.';     
     return;
   END;
    print 'ffff'  
   SET @v_execsql='INSERT INTO T_11_201_'+@AInParam01+' values(0,0,'+@v_userid+','+@AInParam02+',getdate(),CONVERT(DATETIME, ''2199-12-31''))';   
   print @v_execsql   
   EXEC SP_EXECUTESQL @v_execsql


   print 'ok'

   SET @AOuterrornumber = '0';
   SET @AOuterrorstring = 'successful';
END TRY
BEGIN CATCH
    SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH
