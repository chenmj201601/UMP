CREATE PROCEDURE P_00_001
(
	@AInParam01			  VARCHAR(2),
	@AInParam02			  VARCHAR(3),
	@AInParam03			  VARCHAR(5)='00000',
	@Ainparam04       VARCHAR(20)='20140101000000',
	@AOutParam01		  VARCHAR(20)	    output,
	@AOutErrorNumber	INT				      output,
	@AOutErrorString	NVARCHAR(4000)	output
)
--with encryption 
AS

-- ==========================================================================================================================
-- Name:      P_00_001 
-- Real Name:    GetSerialNumber 在表T_00_001记录结果  
-- Type:      存储过程
-- Author:      <cloud>
-- Create date:    <2014-08-11>
-- Description:    <获取系统自动编码>
-- ==========================================================================================================================
-- Parameters Description:
--          以上两个参数是用来记录调用Procedure的日志
-- @ainparam01:   模块编码
-- @ainparam02:   模块内编码 100 ～ 999
-- @ainparam03:   租户编号,默认为00000
-- @ainparam04:   时间变量 固定时间或录音记录的开始录音时间
-- @AOutParam01:  返回流水号
-- @AOutErrorNumber:  成功后返回的系统自动编号bigint型
--          -10：模块编码错误，-11：模块不存在；
--          -20：内部编码错误；-21：内部编码范围错误
--                  yymmddhh000000000:成功后返回的系统自动编号varchar型
--操作码记录表 
--SELECT TOP 1000 [CType]
--      ,[CValue]
--      ,[CDescriber]
--      ,[CTypeName]
--  FROM [UMPDevInfo].[dbo].[T_Type_List]

-- ==========================================================================================================================

DECLARE @StrCurrentHour       VARCHAR(20);
DECLARE @IntSerialNumber      INT;
DECLARE @IntDataCount         INT;
DECLARE @IntCurrentNumber     BIGINT;
DECLARE @IntModuleID          SMALLINT;
DECLARE @IntSerialType        SMALLINT;
DECLARE @script               NVARCHAR(MAX)

BEGIN TRY
 
  
  SELECT @AOutParam01 = 0;

  IF(ISNUMERIC(@AInParam01) = 0)
    BEGIN
      SELECT @AOutErrorNumber = -10; 
        SELECT @AOutErrorString = 'failed'      
      RETURN;
    END;
  IF(ISNUMERIC(@AInParam02) = 0)
    BEGIN
      SELECT AOutErrorNumber = -20; 
      SELECT @AOutErrorString = 'failed'
      RETURN;
    END;

  SELECT @IntModuleID = CONVERT(SMALLINT, @AInParam01), @IntSerialType = CONVERT(SMALLINT, @AInParam02);
  
  IF(@IntModuleID < 11 OR @IntModuleID > 91)
    BEGIN
      SELECT @AOutErrorNumber = -10;
      SELECT @AOutErrorString = 'failed' 
      RETURN;
    END;
  IF(@IntSerialType < 1 OR @IntSerialType > 999)
    BEGIN
      SELECT @AOutErrorNumber = -21; 
      SELECT @AOutErrorString = 'failed'
      RETURN;
    END;

  SET @script='SELECT @IntDataCount= COUNT(1) FROM T_00_008 WHERE C001 =CAST('+@AInParam01+' AS SMALLINT)'

  EXECUTE SP_EXECUTESQL @script, N'@IntDataCount SMALLINT OUTPUT', @IntDataCount OUTPUT
  
  IF(@IntDataCount = 0)
    BEGIN
      SELECT @AOutErrorNumber     = -11; 
      SELECT @AOutErrorString = 'failed'
      RETURN;
    END;
    

  SELECT @StrCurrentHour = SUBSTRING(REPLACE(REPLACE(REPLACE(@AInParam04 , '-' , '' ) , ' ' , '' ) , ':' , '' ), 3, 8);


  SELECT @IntSerialNumber = CONVERT(INT , @StrCurrentHour);

  SELECT @IntDataCount = COUNT(1)  FROM T_00_001 WHERE c000=@AInParam03 AND  C001 = @IntModuleID AND C002 = @IntSerialType AND C003 = @IntSerialNumber;
  
  IF(@IntDataCount = 0)
    INSERT INTO t_00_001 VALUES(@ainparam03,@IntModuleID, @IntSerialType, @IntSerialNumber, 0);
  
  SELECT @IntCurrentNumber = C004 + 1 FROM t_00_001 WHERE c000=@ainparam03 AND c001 = @IntModuleID AND C002 = @IntSerialType AND C003 = @IntSerialNumber;
  
  UPDATE T_00_001 SET C004 = @IntCurrentNumber WHERE C000=@ainparam03 AND C001 = @IntModuleID AND C002 = @IntSerialType AND C003 = @IntSerialNumber;

  IF @ainparam01='21' AND @ainparam02='201' AND @ainparam04='20140101000000' 
  BEGIN
    SELECT @AOutParam01 =@IntCurrentNumber; --录音记录流水号 c001  
  END
  ELSE IF  @ainparam01='21' AND @ainparam02='201' AND @ainparam04<>'20140101000000' 
  BEGIN
    SELECT @AOutParam01 = CONVERT(VARCHAR, CAST(@StrCurrentHour AS BIGINT) *100000000000+@IntCurrentNumber);--录音记录流水号 c002 8+11 19 yymmddhh00000000001
  END
  ELSE
  BEGIN
    SELECT @AOutParam01 = @ainparam02+CONVERT(VARCHAR, CAST(@StrCurrentHour AS BIGINT) *100000000+@IntCurrentNumber);--操作码3位 +时间8位 +流水号 00000000
  END;

  SELECT @AOutErrorNumber = 0, @AOutErrorString = 'successful';
  
END TRY

BEGIN CATCH
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH