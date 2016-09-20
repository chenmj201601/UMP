CREATE  PROCEDURE P_24_005
( @AInParam00 varchar(5)='00000',
  @AInParam01 nvarchar(25),
  @AInparam02 nvarchar(25)='',
  @AInparam03 char(1),
  @AInparam04 char(1),
  @AInparam05 char(1),
  @AInparam06 nvarchar(250),
  @AOutParam01  nvarchar(250) OUTPUT,
  @AOutErrorNumber  int OUTPUT ,
  @AOutErrorString  nvarchar(4000) OUTPUT
)
AS
-- ==========================================================================================================================
-- Name:      P_EncryptionProcessing07
-- Type:      存储过程
-- Author:      <Young Yang>
-- Create date:    <2013-01-20>
-- Description:    <根据策略ID获取收件人邮件地址和语言代码>
-- ==========================================================================================================================
-- Parameters Description:
-- @AInParam01:    策略ID
-- @AInParam02:    备用
-- @AInParam03:    策略创建人 0-不提取，1-提取
-- @AInParam04:    加密对象绑定策略用户 0-不提取，1-提取
-- @AInParam05:    能管理加密的用户 0-不提取，1-提取
-- @AInParam06:    GeneratorID
-- @AOutParam01:  返回说明,如果 = 'GeneratorIDError',则说明GeneratorID与系统设置的不匹配，不能提取数据
-- ==========================================================================================================================
   DECLARE @v_IntGeneratorCount      int;
   DECLARE @v_execsql      NVARCHAR(4000);

BEGIN TRY

    SET NOCOUNT ON;

	SET @AOutErrorNumber= 0;
    SET @AOutErrorString= '';
    SET @AOutParam01 = '';
  
  SELECT @v_IntGeneratorCount=COUNT(1)    from t_24_004 WHERE c002 = @AInParam06 AND c006 = '1';

  IF(@V_IntGeneratorCount = 0) 
  BEGIN
      SET @AOutParam01='GeneratorIDError';
      RETURN;
  END ;


  delete from T_99_006
  
  SET @v_execsql='INSERT INTO T_99_006
    SELECT C028 FROM T_24_001 WHERE C001 = '+@AInParam01+' AND ''1'' = '+@AInparam03+'
    UNION
    SELECT C006 FROM T_24_002 WHERE C003 = '+@AInParam01+' AND ''1'' = '+@AInparam04+'
    UNION
    SELECT C003 FROM T_11_201_'+@AInParam00+' WHERE  ''1'' = '+@AInparam05+' 
    AND C004 IN (SELECT C001  FROM T_11_101_'+@AInParam00+' WHERE substring(C017,10,100) IN (SELECT
	 C002 FROM T_24_002 WHERE C001=''1'' AND C003 = '+@AInParam01+') AND C001 LIKE ''221%'') AND C004 LIKE ''102%''' ;


  PRINT @v_execsql
  EXECUTE sp_executesql @v_execsql


  SET @v_execsql='SELECT distinct A.ColUserID AS UserID,ISNULL(B.C011,'''') AS EmailAddress,C.C005 AS LanguageID FROM T_99_006 A, t_11_101_'+@AInParam00+' B,T_11_011_'+@AInParam00+' C  
  WHERE A.ColUserID=C.C001  AND A.ColUserID=B.C001 AND C.C002=''1100101''';
  
  print @v_execsql;
  EXECUTE sp_executesql @v_execsql  
     
  SET NOCOUNT OFF;  
END TRY
BEGIN CATCH
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH