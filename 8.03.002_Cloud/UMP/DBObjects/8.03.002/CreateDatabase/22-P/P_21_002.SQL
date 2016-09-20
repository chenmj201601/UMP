CREATE PROCEDURE P_21_002
(
	@AInParam01       VARCHAR(2000)='00000',
	@AInParam02       VARCHAR(20),
	@AInParam03       VARCHAR(20),
	@AInParam04       VARCHAR(2000)=NULL,
  @AOutErrorNumber  BIGINT         OUTPUT,
  @AOutErrorString  NVARCHAR(4000) OUTPUT
   )
AS

-- description:    根据流水号的C002找开始时间-2S和开始时间+2S所对应的录屏为2的记录返回
-- create date:    <2015-8-11>
-- =========================================================================================
--ainparam01        租户ID（默认为‘00000’）
--ainparam02        UTC时间
--ainparam03        C002流水号
--ainparam04        预留
--============================================
   DECLARE @v_count       smallint;
   DECLARE @v_year        VARCHAR(2);
   DECLARE @v_month	      VARCHAR(2);
   DECLARE @v_tablename	  VARCHAR(30);
   DECLARE @v_execsql     NVARCHAR (4000);
   DECLARE @begin_time    VARCHAR(30);
   DECLARE @end_time      VARCHAR(30);
   DECLARE @subcall       NVARCHAR(32);
   DECLARE @v_ip          NVARCHAR(128);

BEGIN TRY

     SET @AOutErrorNumber = '0';
     SET @AOutErrorString = 'successful';
	   
     SET @v_year=SUBSTRING(@ainparam02,3,2)
     SET @v_month=SUBSTRING(@ainparam02,6,2)
     SET @v_tablename='T_21_001_'+@AInParam01+'_'+@v_year+@v_month


     SET   @v_execsql ='SELECT @v_count=COUNT(1) FROM  sys.tables WHERE name='''+@v_tablename+'''  AND  type=''u'''
     print @v_execsql
     EXECUTE sp_executesql @v_execsql, N'@v_count INT OUTPUT', @v_count Output

     IF @v_count=0 
       SET @v_tablename='T_21_001_'+@AInParam01


     SET @v_execsql ='SELECT @begin_time=convert(varchar(100),dateadd(ss,-2,C005),120),@end_time=convert(varchar(100),dateadd(ss,2,C009),120),@subcall=C042,@v_ip=C020 FROM '+@v_tablename+' WHERE C002='+@ainparam03+' AND C014=1'
     print @v_execsql
	 EXECUTE sp_executesql @v_execsql, N'@begin_time varchar(30) OUTPUT,@end_time varchar(30) OUTPUT,@subcall nvarchar(32) OUTPUT,@v_ip nvarchar(128) OUTPUT', @begin_time Output,@end_time Output,@subcall Output,@v_ip Output
     print @begin_time
     print @end_time
	 
	 SET @v_execsql ='SELECT *  FROM '+@v_tablename+' WHERE C042='''+@subcall+''' and C005>=convert(datetime,'''+@begin_time+''',120) AND C009<=convert(datetime,'''+@end_time+''',120)  and (C014=2 or C014=3)'
     print @v_execsql	 
	 EXECUTE sp_executesql @v_execsql



END TRY
BEGIN CATCH
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH