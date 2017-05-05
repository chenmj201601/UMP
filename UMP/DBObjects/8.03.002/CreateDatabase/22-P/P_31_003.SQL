CREATE  PROCEDURE P_31_003 (
   @AInParam01              VARCHAR(5) = '00000',
   @AInParam02              NVARCHAR(64),
   @AInParam03              NVARCHAR(1024),
   @AInParam04              VARCHAR(5),
   @AInParam05              VARCHAR,
   @AInParam06              VARCHAR(20) ,
   @AInParam07              VARCHAR(20) ,
   @AInParam08              VARCHAR(10),
   @AInParam09              VARCHAR(20)=NULL ,
   @AInParam10              VARCHAR(10) ,
   @AInParam11              VARCHAR(5) ,
   @AInParam13              VARCHAR(5) ,
   @AInParam14              VARCHAR(5) ,
   @AInParam15              VARCHAR(20)=NULL,
   @AInParam16              NVARCHAR(1024),
   @AInParam17              VARCHAR(20),  
   @AInParam18              VARCHAR(5), 
   @AInParam19              VARCHAR(20), 
   @AOutParam01             VARCHAR(20)    OUTPUT,
   @AOutErrornumber         bigint         OUTPUT,
   @AOutErrorstring         NVARCHAR(4000) OUTPUT
)
AS
-- description:    自动任务设置 T_31_020_00000
-- create date:    <2014-11-21>
-- =========================================================================================
--AInParam01        租户标识（默认为‘00000’）
--AInParam02        任务名称
--AInParam03        任务描述
--AInParam04        任务类别  1 初检手任务 2初检自动任务，2复检手动任务 3复检自动任务 4推荐录音初检 5推荐录音复检  6 QA质检任务（质检但不计入座席成绩） 7智能任务分配 4:下拉枚举 5:多选 6:复选 10:时间范围 11:时长范围
--AInParam05        是否共享任务 Y共享任务  N非共享任务
--AInParam06        分配时间
--AInParam07        分配人ID ,对应 T_11_005.c001
--AInParam08        分配数量
--AInParam09        完成时间
--AInParam10        完成数量
--AInParam11        日期还有多少天到期发通知
--AInParam13        任务所属年
--AInParam14        任务所属月
--AInParam15        任务完成时间
--AInParam16        分配人帐号名称
--AInParam17        记录总时长
--AInParam18        资源编码 308 初检 309 复检
-- =========================================================================================
--AOutParam01       返回流水号ID   31  308
-- =========================================================================================
  DECLARE  @v_serialid     VARCHAR (19);
  DECLARE  @v_err_num      BIGINT;
  DECLARE  @v_err_msg      NVARCHAR (200);
  DECLARE  @v_execsql      NVARCHAR (4000);
  DECLARE  @v_AInParam09   VARCHAR (100);
  DECLARE  @v_AInParam15   VARCHAR (100);
  DECLARE  @v_datetime     VARCHAR (19);

BEGIN TRY

   SELECT @v_datetime=SUBSTRING(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(100), GETDATE(), 25) , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14)

   EXEC P_00_001  '31',@AInParam18,@ainparam01, @v_datetime,@v_serialid OUTPUT, @v_err_num OUTPUT, @v_err_msg OUTPUT

   IF (CONVERT(BIGINT, @v_serialid)<0 or  @v_err_num<>0 )
   BEGIN
     SET @AOutErrornumber=-1
     SET @AOutErrorString='Serial number generation error'
   RETURN
   END

  IF @ainparam09 IS NULL
  BEGIN
     SET @v_AInParam09=''',NULL,'''
  END
  ELSE
  BEGIN
    SET @v_AInParam09=''',CONVERT(DATETIME, '''+@ainparam09+'''), '''--分配时间
  END;

  PRINT @v_AInParam09

  IF @ainparam15 IS NULL
  BEGIN
     SET @v_AInParam15=',NULL,'
  END
  ELSE
  BEGIN
    SET @v_AInParam15=',CONVERT(DATETIME, '''+@ainparam15+'''), '--任务完成时间
  END;

  PRINT @v_AInParam15

  --BEGIN TRAN

  SET @v_execsql =
         'INSERT INTO T_31_020_'
      + @AInParam01
      + '
            VALUES ('''
      + @v_serialid
      + ''',N'''
      + @AInParam02
      + ''', N'''
      + @AInParam03
      + ''','''
      + @AInParam04
      + ''','''
      + @AInParam05
      + ''',CONVERT(DATETIME, '''+@ainparam06+'''), ''' --分配时间
      + @AInParam07
      + ''','''
      + @AInParam08
      + @v_AInParam09       --完成时间
      + @AInParam10         --完成数量
      + ''',null,null,'''   --最后一次修改任务的时间和人员     
      + @AInParam11         --日期还有多少天到期发通知 
      + ''','''        
      + @AInParam13         --任务所属年
      + ''','''       
      + @AInParam14
      + ''',''N'''+@v_AInParam15+'N'''     --Y完成 N未完成 任务完成时间    
      + @AInParam16                         --分配人帐号名称
      + ''','''','                          --最后修改人帐号名称
      + @AInParam17                         --记录总时长
      + ',0,'                              --任务中已评分录音总时长
      + @AInParam19                        -- 任务ID                                               
      + ')';
    PRINT @v_execsql
    EXEC SP_EXECUTESQL @v_execsql

    IF @@ERROR=0
    BEGIN
      --COMMIT TRAN
      SET @aoutparam01 = @v_serialid;     
      SET @AOutErrorNumber = '0';
      SET @AOutErrorString = 'successful';
    END
    ELSE
    BEGIN
      --ROLLBACK TRAN
      SET @AOutErrorNumber = '-1';
      SET @AOutErrorString = 'failed';
    END

END TRY
BEGIN CATCH
        --ROLLBACK TRAN
    SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH
