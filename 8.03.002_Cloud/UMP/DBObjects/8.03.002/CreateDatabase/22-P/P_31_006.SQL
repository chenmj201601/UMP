CREATE PROCEDURE P_31_006 (
   @AInParam00              VARCHAR(5) = '00000',
   @AInParam01              VARCHAR(20),
   @AInParam02              VARCHAR(20),
   @AInParam03              VARCHAR(20),
   @AOutParam01             VARCHAR(20)    OUTPUT,
   @AOutErrornumber         BIGINT         OUTPUT,
   @AOutErrorstring         NVARCHAR(4000) OUTPUT
)
AS
-- description:    所有任务以及服务类运行时间及周期 T_31_026_00000
-- create date:    <2015-7-28>
-- =========================================================================================
--AInParam00        租户标识（默认为‘00000’）
--AInParam01        运行周期
--AInParam02        周期值
--AInParam03        运行时间

-- =========================================================================================
--AOutParam01       返回流水号ID   
-- =========================================================================================
-- =========================================================================================
  DECLARE  @v_datetime     VARCHAR (19);  
  DECLARE  @v_serialid     VARCHAR (19);
  DECLARE  @v_err_num      BIGINT;
  DECLARE  @v_err_msg      NVARCHAR (200);
  DECLARE  @v_execsql      NVARCHAR (4000);
  DECLARE  @v_ainparam4    NVARCHAR(19);
  DECLARE  @v_C004           VARCHAR(5);--周
  DECLARE  @v_C005           VARCHAR(5);--月
  DECLARE  @v_C017           VARCHAR(5);--年

BEGIN TRY
   SET @v_C004='';
   SET @v_C005='';
   SET @v_C017='';

   IF (@AInParam01='W' AND (CONVERT(INT,@AInParam02)<1 OR CONVERT(INT,@AInParam02)>7)) 
   BEGIN
     SET @AOutErrornumber = -1;
     SET @AOuterrorstring = 'Date parameter error';
     RETURN;    
   END;

   IF (@AInParam01='M' AND (CONVERT(INT,@AInParam02)<1 OR CONVERT(INT,@AInParam02)>31)) 
   BEGIN
     SET @AOuterrornumber = -1;
     SET @AOuterrorstring = 'Date parameter error';
     RETURN;    
   END;  
    
   IF (@AInParam01='Y' AND (CONVERT(INT,@AInParam02)<1 OR CONVERT(INT,@AInParam02)>366)) 
   BEGIN
     SET @AOuterrornumber = -1;
     SET @AOuterrorstring = 'Date parameter error';
     RETURN;    
   END; 

   IF @AInParam01='W'
   BEGIN 
      SET @v_C004=@AInParam02;
   END

   IF @AInParam01='M'
   BEGIN     
      SET @v_C005=@AInParam02;
   END

   IF @AInParam01='Y'
   BEGIN     
      SET @v_C017=@AInParam02;
   END;

   SELECT @v_datetime=SUBSTRING(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(100), GETDATE(), 25) , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14)
   
   EXEC P_00_001  '31','313',@ainparam01,@v_datetime,@v_serialid OUTPUT, @v_err_num OUTPUT, @v_err_msg OUTPUT

   IF (CONVERT(BIGINT, @v_serialid)<0 or  @v_err_num<>0 )
   BEGIN
     SET @AOutErrornumber=-1
     SET @AOutErrorString='Serial number generation error'
   RETURN
   END

   IF @v_C004=''
   BEGIN
     SET @v_C004=',NULL'
   END
   ELSE
   BEGIN
     SET @v_C004=','+@AInParam02
   END


   IF @v_C005=''
   BEGIN
     SET @v_C005=',NULL'
   END
   ELSE
   BEGIN
     SET @v_C005=','+@AInParam02
   END

   IF @v_C017=''
   BEGIN
     SET @v_C017=',NULL'
   END
   ELSE
   BEGIN
     SET @v_C017=','+@AInParam02
   END

  SET @v_execsql =
         'INSERT INTO T_31_026_'
      + @AInParam00
      + ' (C001, C002, C003, C004,C005, C017) 
            VALUES ('''
      + @v_serialid
      + ''','''
      + @AInParam01
      + ''', '''
      + @AInParam03
      + ''''
      + @v_C004
      + @v_C005
      + @v_C017                                       
      + ')';

    print @v_execsql

    EXEC SP_EXECUTESQL @v_execsql

    IF @@ERROR=0
    BEGIN
      SET @aoutparam01 = @v_serialid;     
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
