CREATE  PROCEDURE P_24_004
( @AInParam00   varchar(5)='00000', 
  @AInParam01   varchar(20),
  @AInparam02   nvarchar(128),
  @AOutParam01  nvarchar(512) OUTPUT,
  @AOutParam02  nvarchar(512) OUTPUT,
  @AOutParam03  nvarchar(512) OUTPUT,
  @AOutParam04  nvarchar(512) OUTPUT,
  @AOutParam05  nvarchar(512) OUTPUT,
  @AOutParam06  nvarchar(512) OUTPUT,
  @AOutParam07  nvarchar(512) OUTPUT,
  @AOutErrorNumber  int OUTPUT,
  @AOutErrorString  nvarchar(4000) OUTPUT  
)
AS
-- ==========================================================================================================================
-- Name:      P_EncryptionProcessing06
-- Type:      存储过程
-- Author:      <Young Yang>
-- Create date:    <2013-01-20>
-- Description:    <获取发送邮件参数>
-- ==========================================================================================================================
-- Parameters Description:
-- @AInParam01:    参数编码。加密相关 = '240401'
-- @AInParam02:    GeneratorID
-- @AOutParam01:  SMTP服务器。
--          如果 = 'GeneratorIDError'则说明GeneratorID与系统设置的不匹配；
--          如果 = 'NotSetting'则说明没有设置发送邮件参数；
--          如果 = 'NotSent'则说明不需要发送邮件。
-- @AOutParam02:  SMTP服务端口。
-- @AOutParam03:  启用SSL
-- @AOutParam04:  帐号
-- @AOutParam05:  密码
-- @AOutParam06:  需要服务器验证
-- @AOutParam07:  Email 地址
-- ==========================================================================================================================
   DECLARE @v_IntGeneratorCount int;
   DECLARE @V_AOutParam01       varchar(25);
   DECLARE @v_execsql           NVARCHAR (4000);

BEGIN TRY
  if @AInParam00 is null
  set @AInParam00='00000'

  set @AOutErrorNumber= 0;
  set @AOutErrorString= '';
  
  set @AOutParam01='';
  set @AOutParam02='';
  set @AOutParam03='';
  set @AOutParam04='';
  set @AOutParam05='';
  set @AOutParam06='';
  set @AOutParam07='';

  SELECT  @v_IntGeneratorCount=COUNT(1)   from t_24_004 WHERE c002 = @AInParam02 AND c006 = '1';

  IF (@v_IntGeneratorCount = 0) 
  BEGIN
      SET @AOutParam01 = 'GeneratorIDError';
      SET @AOutErrorNumber= -1;
      SET @AOutErrorString = 'FAIL';     
      RETURN;
  END 

  
	SET @v_execsql= 'SELECT   @v_IntGeneratorCount=COUNT(1)  from T_11_001_'+@AInParam00+' WHERE C004 ='+ @AInparam01;
	EXECUTE sp_executesql @v_execsql, N'@v_IntGeneratorCount INT OUTPUT', @v_IntGeneratorCount Output

	IF(@V_IntGeneratorCount = 0) 
	BEGIN
		SET @AOutParam01 = 'NotSetting';
        RETURN;
    END;
  
	SET @v_execsql= 'SELECT  @V_AOutParam01=C006   FROM T_11_001_'+@AInParam00+' WHERE  C001 = 0 and C002 =24 and C003  = 240401';
	EXECUTE sp_executesql @v_execsql, N'@V_AOutParam01 nvarchar(512) OUTPUT', @V_AOutParam01 Output	
	IF(@V_AOutParam01 = '0') 
	BEGIN
	  SET @AOutParam01 = 'NotSent';
      RETURN;
	END ;
  
	SET @v_execsql= 'SELECT  @AOutParam01=C006   FROM T_11_001_'+@AInParam00+' WHERE C001 = 0 and C002 =24 and C003  = 240402';
	EXECUTE sp_executesql @v_execsql, N'@AOutParam01 nvarchar(512) OUTPUT', @AOutParam01 Output

	SET @v_execsql= 'SELECT  @AOutParam02=C006   FROM T_11_001_'+@AInParam00+' WHERE C001 = 0 and C002 =24 and C003  = 240403';
	EXECUTE sp_executesql @v_execsql, N'@AOutParam02 nvarchar(512) OUTPUT', @AOutParam02 Output

	SET @v_execsql= 'SELECT  @AOutParam03=C006   FROM T_11_001_'+@AInParam00+' WHERE C001 = 0 and C002 =24 and C003  = 240404';

	EXECUTE sp_executesql @v_execsql, N'@AOutParam03 nvarchar(512) OUTPUT', @AOutParam03 Output

	SET @v_execsql= 'SELECT  @AOutParam04=C006   FROM T_11_001_'+@AInParam00+' WHERE C001 = 0 and C002 =24 and C003  = 240405';
	print @v_execsql
	EXECUTE sp_executesql @v_execsql, N'@AOutParam04 nvarchar(512) OUTPUT', @AOutParam04 Output

	SET @v_execsql= 'SELECT  @AOutParam05=C006   FROM T_11_001_'+@AInParam00+' WHERE C001 = 0 and C002 =24 and C003  = 240406';
	EXECUTE sp_executesql @v_execsql, N'@AOutParam05 nvarchar(512) OUTPUT', @AOutParam05 Output

	SET @v_execsql= 'SELECT  @AOutParam06=C006   FROM T_11_001_'+@AInParam00+' WHERE C001 = 0 and C002 =24 and C003  = 240407';
	EXECUTE sp_executesql @v_execsql, N'@AOutParam06 nvarchar(512) OUTPUT', @AOutParam06 Output

	SET @v_execsql= 'SELECT  @AOutParam07=C006   FROM T_11_001_'+@AInParam00+' WHERE C001 = 0 and C002 =24 and C003  = 240408';
	EXECUTE sp_executesql @v_execsql, N'@AOutParam07 nvarchar(512) OUTPUT', @AOutParam07 Output
	  
END TRY
BEGIN CATCH
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH