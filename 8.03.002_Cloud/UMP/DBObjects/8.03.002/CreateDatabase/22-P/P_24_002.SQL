CREATE PROCEDURE P_24_002
(
  @AInParam01		varchar(20),
  @AInparam02		varchar(5),
  @AInParam03		varchar(1024),
  @AInParam04		varchar(1024),
  @AInParam05		varchar(20),
  @AInParam06		varchar(20),
  @AInParam07		varchar(1024),
  @AInParam08		char,
  @AInParam09		Nvarchar(128),
  @AOutParam01      varchar(20) OUTPUT,
  @AOutErrorNumber  int OUTPUT ,
  @AOutErrorString  nvarchar(4000) OUTPUT)
-- ==========================================================================================================================
-- Name:    P_24_002
-- Type:    存储过程
-- Create date:    <2015-7-24>
-- Description:    <加密生成器向密钥字典中插入数据>
-- ==========================================================================================================================
-- Parameters Description:
-- @AInParam01:    策略ID
-- @AInParam02:    加密版本
-- @AInParam03:    加密密钥 Key1b
-- @AInParam04:    加密密钥 Key1d
-- @AInParam05:    密钥启用时间
-- @AInParam06:    密钥失效时间
-- @AInParam07:    密钥验证码
-- @AInParam08:    创建方式。U：用户立即创建；C：生成器创建
-- @AInParam09:    GeneratorID
-- @AOutParam01:  插入成功后返回的KeyID,如果 = 'GeneratorIDError',则说明GeneratorID与系统设置的不匹配，没能插入到数据库
-- ==========================================================================================================================
AS
  DECLARE @v_StrKeyID             varchar(25);
  DECLARE @v_IntGeneratorCount    int;
  DECLARE @v_AInParam05           varchar(20);
  DECLARE @v_AInParam06           varchar(20);
  DECLARE @v_datetime             varchar(20);
  DECLARE @v_err_num              integer;
  DECLARE @v_err_msg              NVARCHAR(2000); 
    
BEGIN  TRY

  SET @AOutErrorNumber= 0;
  SET @AOutErrorString= '';

  SELECT @v_AInParam05=SUBSTRING(REPLACE(REPLACE(REPLACE(@ainparam05 , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14)   
  SELECT @v_AInParam06=SUBSTRING(REPLACE(REPLACE(REPLACE(@ainparam06 , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14) 
  SELECT @v_datetime=SUBSTRING(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(100), GETDATE(), 25) , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14) 

  PRINT  @AInParam09;
  SELECT  @v_IntGeneratorCount=COUNT(1)  from T_24_004 WHERE C002 =@AInParam09 AND C006 = '1';

  
  IF(@v_IntGeneratorCount = 0) 
      SET @AOutParam01 = 'GeneratorIDError';
  ELSE
  BEGIN

	  SELECT @v_datetime=SUBSTRING(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(100), GETDATE(), 25) , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14);
     
      EXEC P_00_001  '11','101',@ainparam01, @v_datetime,@v_StrKeyID OUTPUT, @v_err_num OUTPUT, @v_err_msg OUTPUT

   
      IF ( @v_err_num<>0 )
      BEGIN
        SET @AOutErrorNumber=-1
        SET @AOutErrorString='Serial number generation error'
        RETURN
      END
	  
	  SELECT @v_datetime=SUBSTRING(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(100), GETUTCDATE(), 25) , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14);	  	        
         
      INSERT INTO T_24_005(C001, C002, C003, C004, C005, C006, C007, C008, C009,C010, C011)
        VALUES(@v_StrKeyID, @AInParam01, @AInParam02, @AInParam03, @AInParam04, @v_datetime,@v_AInParam05, @v_AInParam06, @AInParam07,0, @AInParam08);
      
	  SET @AOutParam01 = @v_StrKeyID;
      
  END ;

END TRY
BEGIN CATCH
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH
