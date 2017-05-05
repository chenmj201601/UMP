ALTER PROCEDURE [dbo].[P_11_010] 
(
   @AInParam01         VARCHAR(5) = '00000',
   @AInParam02         NVARCHAR(1024),
   @AInParam03         VARCHAR(5),
   @AInParam04         VARCHAR(20),
   @AInParam05         CHAR = '1',
   @AInParam06         CHAR = '0',
   @AInParam07         VARCHAR(32) = '11111111111111111111111111111111',
   @AInParam08         VARCHAR(512),
   @AInParam09         VARCHAR(512),
   @AInParam10         VARCHAR(20),
   @AInParam11         VARCHAR(20)=NULL,
   @AInParam12         NVARCHAR(1024)=NULL,
   @AInparam13         NVARCHAR, 
   @AOutParam01        VARCHAR(20)    output,
   @AOutErrorNumber    bigint         output,
   @AOutErrorString    NVARCHAR(4000) output
)
AS

-- description:    新机构的ID生成过程，记录目标表为机构部门T_11_006_00000,资源表T_11_201_00000
-- create date:    <2014-9-3>
-- =========================================================================================
--ainparam01        租户ID（默认为‘00000’）
--ainparam02        机构名称
--ainparam03        机构类型
--ainparam04        父级机构ID
--ainparam05        是否活动
--ainparam06        是否删除
--ainparam07        其他状态
--ainparam08        开始时间
--ainparam09        失效时间
--ainparam10        创建人
--ainparam11        创建时间
--ainparam12        描述
-- ===========================================
--aoutparam01       新机构的ID
--============================================
--          -1：生成流水号错误；
--============================================

   DECLARE @v_serialid    VARCHAR (19);
   DECLARE @v_err_num     BIGINT;
   DECLARE @v_err_msg     VARCHAR (200);
   DECLARE @v_ainparam11  NVARCHAR(19);   
   DECLARE @v_execsql     NVARCHAR (4000);
   DECLARE @error_num     BIGINT
   DECLARE @v_count       SMALLINT;
   DECLARE @v_datetime    VARCHAR (14);
 
BEGIN TRY

  SET @v_execsql ='SELECT @v_count=COUNT(1) FROM  T_11_006_'+@AInParam01+' WHERE C002='''+@AInParam02+''' AND  C004='+@AInParam04
  print @v_execsql
  EXECUTE sp_executesql @v_execsql, N'@v_count INT OUTPUT', @v_count Output

  IF @v_count>0 
  BEGIN
    SET @AOuterrornumber = '-1';
    SET @AOutErrorString = 'Organization already exists'
    RETURN;         
  END; 
  
   IF (@ainparam12 IS NULL) OR (@ainparam12='')
      SET @ainparam12=''

   IF (@ainparam13='1')
   BEGIN  
     SET @v_datetime='20140101000000';
   END     
   ELSE
   BEGIN
     SELECT @v_datetime=SUBSTRING(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(100), GETDATE(), 25) , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14)     
   END;

   EXEC P_00_001  '11','101',@ainparam01, @v_datetime,@v_serialid OUTPUT, @v_err_num OUTPUT, @v_err_msg OUTPUT

   IF (CONVERT(BIGINT, @v_serialid)<0 or  @v_err_num<>0 )
   BEGIN
     SET @AOutErrorNumber=-1
     SET @AOutErrorString='Serial number generation error'
   RETURN
   END

  IF (@ainparam11 is NULL) OR (@ainparam11='')  
    SELECT @v_ainparam11=CONVERT(NVARCHAR(19), GETDATE(), 120)
  ELSE
    SELECT @v_ainparam11=@ainparam11;

    BEGIN TRAN  

     SET @v_execsql ='INSERT INTO T_11_006_'+@ainparam01+' VALUES ('''+@v_serialid+''',N'''+@ainparam02+''','+@ainparam03
           +','+@ainparam04+','''+@ainparam05+''','''+@ainparam06+''', '''+@ainparam07
           +''','''+@ainparam08+''','''+@ainparam09
           +''','+@ainparam10
           +',CONVERT(DATETIME, '''+@v_ainparam11+''')'+',N'''+@ainparam12+''')';

     EXEC SP_EXECUTESQL @v_execsql
     SET @error_num=@error_num+@@ERROR

   
     SET @v_execsql ='INSERT INTO T_11_201_'+@ainparam01+' SELECT 0,0,C003,'''+@v_serialid+''',GETDATE(),CONVERT(DATETIME, ''2199-12-31'') FROM T_11_201_'+@ainparam01+' WHERE C004='''+@ainparam10+''''
       print @v_execsql

     EXEC SP_EXECUTESQL @v_execsql
     SET @error_num=@error_num+@@ERROR

    IF @@ERROR=0
    BEGIN
      COMMIT TRAN  
      SET @aoutparam01 = @v_serialid;    
      SET @AOutErrorNumber = '0';
      SET @AOutErrorString = 'successful';
    END
    ELSE
    BEGIN
      ROLLBACK TRAN
      SET @AOutErrorNumber = '-1';
      SET @AOutErrorString = 'failed';
    END

END TRY
BEGIN CATCH
        ROLLBACK TRAN
    SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH

