CREATE PROCEDURE P_31_007 (
   @AInParam00              VARCHAR(5) = '00000',
   @AInParam02              VARCHAR(20),
   @AInParam03              VARCHAR(20),
   @AInParam04              VARCHAR(20),
   @AInParam05              VARCHAR(20),
   @AInParam06              VARCHAR(20),
   @AInParam07              VARCHAR(20),
   @AInParam08              VARCHAR(20),
   @AInParam09              VARCHAR(20),
   @AInParam10              VARCHAR(20),
   @AInParam11              VARCHAR(20),
   @AOutParam01             VARCHAR(20)    OUTPUT,
   @AOutErrornumber         BIGINT         OUTPUT,
   @AOutErrorstring         NVARCHAR(4000) OUTPUT
)
AS
-- description:    所有任务以及服务类运行时间及周期 T_31_052_00000
-- create date:    <2015-7-28>
-- =========================================================================================
--AInParam00        租户标识（默认为‘00000’）
--AInParam02        T_31_050.C001
--AInParam03        机构ID或技能组ID
--AInParam04        当为机构时，统计机构下座席时分机是平行还是向下钻取的1平行 2钻取
--AInParam05        1应用 ，2不应用(如果是部门时，可以选择是否自动添加到所有的机构上）
--AInParam06        该统计有效期的开始时间
--AInParam07        该统计有效期的结束时间
--AInParam08        统计出的结果在录音表或成绩表对应的列号
--AInParam09        T_31_026_00000.C001运行周期
--AInParam10        更新多长时间内的数据
--AInParam11        时间单位1、年，2、月 ，3、工作月，4、工作周 5、周，6、天，7、小时,8、分钟

-- =========================================================================================
--AOutParam01       返回流水号ID
-- =========================================================================================
  DECLARE  @v_datetime     VARCHAR (19);  
  DECLARE  @v_serialid     VARCHAR (19);
  DECLARE  @v_err_num      BIGINT;
  DECLARE  @v_err_msg      NVARCHAR (200);
  DECLARE  @v_execsql      NVARCHAR (4000);

BEGIN TRY


   SELECT @v_datetime=SUBSTRING(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(100), GETDATE(), 25) , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14)
   
   EXEC P_00_001  '31','312',@ainparam00,@v_datetime,@v_serialid OUTPUT, @v_err_num OUTPUT, @v_err_msg OUTPUT

   IF (CONVERT(BIGINT, @v_serialid)<0 or  @v_err_num<>0 )
   BEGIN
     SET @AOutErrornumber=-1
     SET @AOutErrorString='Serial number generation error'
   RETURN
   END

  SET @v_execsql =
         'INSERT INTO T_31_052_'
      + @AInParam00
      + '  VALUES ('''
      + @v_serialid
      + ''','''
      + @AInParam02
      + ''','''
      + @AInParam03
      + ''','''
      + @AInParam04
      + ''','''
      + @AInParam05
      + ''',CONVERT(DATETIME, '''+@AInParam06+''') ' 
      + ',CONVERT(DATETIME, '''+@AInParam07+''') ' 
      + ','''
      + @AInParam08
      + ''','''
      + @AInParam09
      + ''','''
      + @AInParam10
      + ''','''
      + @AInParam11                            
      + ''')';

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
