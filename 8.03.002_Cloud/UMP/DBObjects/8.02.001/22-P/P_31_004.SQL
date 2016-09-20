CREATE PROCEDURE P_31_004 (
   @AInParam01              VARCHAR(5) = '00000',
   @AInParam02              VARCHAR(20),
   @AInParam03              VARCHAR(20),
   @AInParam04              VARCHAR(20),
   @AInParam05              VARCHAR(20),
   @AInParam06              VARCHAR(20) ,
   @AInParam07              VARCHAR(20) ,
   @AInParam08              VARCHAR(5),
   @AInParam09              VARCHAR(5),
   @AInParam10              VARCHAR(1) ,
   @AInParam11              VARCHAR(5) ,
   @AInParam12              NVARCHAR(20) ,
   @AOutParam01             VARCHAR(20)    OUTPUT,
   @AOutErrornumber         bigint         OUTPUT,
   @AOutErrorstring         NVARCHAR(4000) OUTPUT
)
AS
-- description:    维护申诉详情表 T_31_019_00000
-- create date:    <2015-2-11>
-- =========================================================================================
--AInParam01        租户标识（默认为‘00000’）
--AInParam02        T_31_008_00000.c001评分成绩表的成绩ID
--AInParam03        录音记录表的ID,T_21_001.C002
--AInParam04        录音所属座席工号对应的资源编号
--AInParam05        申诉人ID,如果是座席自己申诉的
--AInParam06        申诉流程ID
--AInParam07        申诉流程子项ID(通过ActionID+AppealFlowItemID得到该流程走到那一步了)
--AInParam08        1为申诉，2为审批，3为复核
--AInParam09        对于申诉(1座席申诉，2他人替申)，对于审批(3审批驳回,4审批过通)，对于复核（5复核通过不修改分数，6复核通过重新评分，7复核驳回）
--AInParam10        Y为申诉流程完毕，N 在处理流程中
--AInParam11        当能多次申诉时启用，每再申诉一次+1，客户端第一版写1
--AInParam12        创建时间
-- =========================================================================================
--AOutParam01       返回流水号ID   
-- =========================================================================================
  DECLARE  @v_serialid     VARCHAR (19);
  DECLARE  @v_err_num      BIGINT;
  DECLARE  @v_err_msg      NVARCHAR (200);
  DECLARE  @v_execsql      NVARCHAR (4000);
  DECLARE  @v_ainparam12   NVARCHAR(19);
  DECLARE  @v_datetime     VARCHAR (19);
BEGIN TRY

   IF (@AInParam12 is NULL) OR (@AInParam12='')
     
     SELECT @v_ainparam12=CONVERT(NVARCHAR(19), GETDATE(), 120)
   ELSE
     SELECT @v_ainparam12=@AInParam12;


   SELECT @v_datetime=SUBSTRING(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(100), GETDATE(), 25) , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14)

   EXEC P_00_001  '31','312',@ainparam01,@v_datetime,@v_serialid OUTPUT, @v_err_num OUTPUT, @v_err_msg OUTPUT

   IF (CONVERT(BIGINT, @v_serialid)<0 or  @v_err_num<>0 )
   BEGIN
     SET @AOutErrornumber=-1
     SET @AOutErrorString='Serial number generation error'
   RETURN
   END

  BEGIN TRAN

  SET @v_execsql =
         'INSERT INTO T_31_019_'
      + @AInParam01
      + '
            VALUES ('''
      + @v_serialid
      + ''','''
      + @AInParam02
      + ''', '''
      + @AInParam03
      + ''','''
      + @AInParam04
      + ''','''
      + @AInParam05
      + ''','''
      + @AInParam06
      + ''','''
      + @AInParam07
      + ''','''
      + @AInParam08
      + ''','''
      + @AInParam09 
      + ''','''	           
      + @AInParam10           
      + ''','''        
      + @AInParam11    
      + ''',CONVERT(DATETIME, '''+@v_ainparam12+''') '                                         
      + ')';

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
