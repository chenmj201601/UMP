CREATE PROCEDURE P_11_014 
(
   @AInParam01       VARCHAR(5) = '00000',
   @AInParam02       VARCHAR(20),
   @AInParam03       VARCHAR(5),
   @AInParam04       NVARCHAR(1024),
   @AInParam05       CHAR = '1',
   @AInParam06       CHAR = '0',
   @AInParam07       VARCHAR(32) = '11111111111111111111111111111111',
   @AInParam08       VARCHAR(512),
   @AInParam09       VARCHAR(512),
   @AInParam10       VARCHAR(20),
   @AInParam11       VARCHAR(20),
   @AOutParam01      VARCHAR(20)    OUTPUT,
   @AOutErrorNumber  BIGINT         OUTPUT,
   @AOutErrorString  NVARCHAR(4000) OUTPUT
)
AS

-- description:    新增角色，记录目标表为T_11_004_00000
-- create date:    <2014-9-12>
-- =========================================================================================
--AInParam01        租户ID（默认为‘00000’）
--AInParam02        父级角色编号
--AInParam03        应用于哪个模块
--AInParam04        角色名称
--AInParam05        是否活动
--AInParam06        是否删除
--AInParam07        其他状态
--AInParam08        开始时间
--AInParam09        失效时间
--AInParam10        创建人
--AInParam11        创建时间
-- =========================================================================================
--aoutparam01       新角色的ID
-- =========================================================================================

   DECLARE @v_serialid    VARCHAR (19);
   DECLARE @v_err_num     BIGINT;
   DECLARE @v_err_msg     VARCHAR (200);
   DECLARE @v_ainparam11  NVARCHAR(19);   
   DECLARE @v_execsql     NVARCHAR (4000);
   DECLARE @v_count       SMALLINT;
   DECLARE @error         BIGINT;
   DECLARE @errorSum      SMALLINT;
   DECLARE @v_datetime    VARCHAR (19);
    
BEGIN TRY

  
   SELECT @v_datetime=SUBSTRING(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(100), GETDATE(), 25) , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14) 

   EXEC P_00_001  '11','106',@AInParam01, @v_datetime,@v_serialid OUTPUT, @v_err_num OUTPUT, @v_err_msg OUTPUT
   
   IF    @v_err_num<>0 
   BEGIN
     SET @AOutErrorNumber=-1
     SET @AOutErrorString='Serial number generation error'
   RETURN
   END

   IF (@AInParam11 is NULL)  OR (@AInParam11='')
     SELECT @v_ainparam11=CONVERT(NVARCHAR(19), GETDATE(), 120)
   ELSE
     SELECT @v_ainparam11=@AInParam11;

   SET @v_execsql ='INSERT INTO T_11_004_'+@AInParam01+' VALUES ('''+@v_serialid+''','''+@AInParam02+''','+@AInParam03
               +',N'''+@AInParam04+''','''+@AInParam05+''','''+@AInParam06+''', '''+@AInParam07
         +''','''+@AInParam08+''','''+@AInParam09
         +''','+@AInParam10
               +',CONVERT(DATETIME, '''+@v_ainparam11+''')'+')';

   EXEC SP_EXECUTESQL @v_execsql    
   SET @error=@@error
   
   IF @error=0 
   BEGIN     
     SET @aoutparam01 = @v_serialid;   
     SET @AOutErrorNumber = 0;
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
