ALTER  PROCEDURE [dbo].[P_51_001]
( @AInParam01       VARCHAR(64),
  @AOutErrornumber         BIGINT         OUTPUT,
  @AOutErrorstring         NVARCHAR(4000) OUTPUT
)

AS

-- ==========================================================================================================================
-- Name:           P_51_001
-- Create date:    <2015-10-10>
-- Description:    <从录音记录表中取数据写入到情绪分析表T_51_001>
-- ==========================================================================================================================
-- Parameters Description:
-- ==========================================================================================================================
   DECLARE @v_count       SMALLINT;
   DECLARE @v_year        VARCHAR(2);
   DECLARE @v_month	      VARCHAR(2);
   DECLARE @v_tablename	  VARCHAR(30);
   DECLARE @v_execsql     NVARCHAR(4000);   
BEGIN TRY

  SET @AOutErrorNumber= 0;
  SET @AOutErrorString = 'successful';

  DELETE FROM T_99_007
  
  SELECT @v_year=SUBSTRING(@AInParam01,DATALENGTH(@AInParam01)-14,2);
  SELECT @v_month=SUBSTRING(@AInParam01,DATALENGTH(@AInParam01)-12,2);
  SELECT @v_tablename='T_21_001_00000'+'_'+@v_year+@v_month;

  SET   @v_execsql ='SELECT @v_count=COUNT(1) FROM  sys.tables WHERE name='''+@v_tablename+'''  AND  type=''u'''
  print @v_execsql
  EXECUTE sp_executesql @v_execsql, N'@v_count INT OUTPUT', @v_count Output
  print @v_count
  IF @v_count=0 
  BEGIN
     SET  @v_tablename='T_21_001_00000';
  END;

  SET @v_execsql ='INSERT  T_99_007 SELECT C001,C002,C003,C006,C007,C010,C011,C019,C020,C021,C039,C042,'''
        +@v_tablename+''',C058,C005,C009,C040,C041,C045,C012 FROM '+@v_tablename+' WHERE C077='''+@AInParam01+'''';
  print @v_execsql
  EXEC SP_EXECUTESQL @v_execsql

  SELECT * FROM T_99_007


END	TRY
BEGIN CATCH
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH
