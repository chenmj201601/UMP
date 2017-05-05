CREATE PROCEDURE P_31_005 (
   @AInParam01              VARCHAR(5) = '00000',
   @AInParam02              VARCHAR(20),
   @AInParam03              VARCHAR(20),
   @AInParam04              VARCHAR(20),
   @AInParam05              VARCHAR(20),
   @AInParam06              VARCHAR(20) ,
   @AInParam07              VARCHAR(20) ,
   @AInParam08              VARCHAR(5),
   @AInParam09              VARCHAR(5),
   @AOutParam01             VARCHAR(20)    OUTPUT,
   @AOutErrornumber         BIGINT         OUTPUT,
   @AOutErrorstring         NVARCHAR(4000) OUTPUT
)
AS
-- description:    查询历史播放列表 T_31_038_00000
-- create date:    <2015-2-13>
-- =========================================================================================
--AInParam01        租户标识（默认为‘00000’）
--AInParam02        录音流水号
--AInParam03        播放人
--AInParam04        播放日期
--AInParam05        播放时长
--AInParam06        1为查询播放;2 CQC播放
--AInParam07        播放次数
--AInParam08        开始位置
--AInParam09        结束位置
-- =========================================================================================
--AOutParam01       返回流水号ID   
-- =========================================================================================
  DECLARE  @v_serialid     VARCHAR (19);
  DECLARE  @v_err_num      BIGINT;
  DECLARE  @v_err_msg      NVARCHAR (200);
  DECLARE  @v_execsql      NVARCHAR (4000);
  DECLARE  @v_ainparam4    NVARCHAR(19);
  DECLARE  @v_datetime     VARCHAR (19);
BEGIN TRY

   IF (@AInParam04 is NULL) OR (@AInParam04='')
     
     SELECT @v_ainparam4=CONVERT(NVARCHAR(19), GETDATE(), 120)
   ELSE
     SELECT @v_ainparam4=@AInParam04;

   SELECT @v_datetime=SUBSTRING(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(100), GETDATE(), 25) , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14)
      
   EXEC P_00_001  '31','305',@ainparam01,@v_datetime,@v_serialid OUTPUT, @v_err_num OUTPUT, @v_err_msg OUTPUT

   IF (CONVERT(BIGINT, @v_serialid)<0 or  @v_err_num<>0 )
   BEGIN
     SET @AOutErrornumber=-1
     SET @AOutErrorString='Serial number generation error'
   RETURN
   END

  BEGIN TRAN

  SET @v_execsql =
         'INSERT INTO T_31_038_'
      + @AInParam01
      + ' (C001, C002, C003, C004, C005, C006, C007, C008, C009) 
            VALUES ('''
      + @v_serialid
      + ''','''
      + @AInParam02
      + ''', '''
      + @AInParam03
      + ''',CONVERT(DATETIME, '''+@v_ainparam4+''') ' 
      + ','''
      + @AInParam05
      + ''','''
      + @AInParam06
      + ''','''
      + @AInParam07
      + ''','''
      + @AInParam08
      + ''','''
      + @AInParam09                                       
      + ''')';

    print @v_execsql

    EXEC SP_EXECUTESQL @v_execsql

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
