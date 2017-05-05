CREATE   PROCEDURE P_24_003
(
  @AInParam01  nvarchar(20),
  @AInParam02  nvarchar(30),
  @AInParam03  nvarchar(128),
  @AOutParam01  nvarchar(20) output ,
  @AOutErrorNumber  int output ,
  @AOutErrorString  nvarchar(4000) output
)
AS
  declare @v_IntGeneratorCount    int;
  declare @v_AInParam02           VARCHAR(20);
  declare @v_err_num              INTEGER;
  declare @v_err_msg              NVARCHAR(2000);   
-- ==========================================================================================================================
-- Name:    P_EncryptionProcessing03
-- Type:    存储过程
-- Author:    <Young Yang>
-- Create date:    <2011-11-18>
-- Description:    <根据KeyID更新该密钥的失效时间>
-- ==========================================================================================================================
-- Parameters Description:
-- @AInParam01:    KeyID
-- @AInParam02:    失效时间 YYYY-MM-DD HH24:MI:SS
-- @AInParam03:		GeneratorID
-- @AOutParam01:	如果 = GeneratorIDError,则说明GeneratorID与系统设置的不匹配，没能更新成功
-- ==========================================================================================================================

BEGIN TRY

  SET @AOutErrorNumber= 0;
  SET @AOutErrorString = '';
  SET @AOutParam01 = '';

   SELECT @v_AInParam02=SUBSTRING(REPLACE(REPLACE(REPLACE(@ainparam02 , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14) 
    
  SELECT @v_IntGeneratorCount=COUNT(1)   from t_24_004 WHERE c002 = @AInParam03 AND c006 = '1';
    
  IF(@v_IntGeneratorCount = 0) 
      SET @AOutParam01 = 'GeneratorIDError';
  ELSE
      UPDATE T_24_005 SET C008 = @v_AInParam02 WHERE C001 = @AInParam01;

END TRY
BEGIN CATCH
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH


