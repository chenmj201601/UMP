CREATE  PROCEDURE [dbo].[P_21_003] (
   @ainparam01               VARCHAR(5) = '00000',
   @ainparam02               VARCHAR(1),
   @ainparam03               VARCHAR(20),
   @ainparam04               VARCHAR(8),
   @ainparam05               VARCHAR(8),
   @ainparam06               VARCHAR(1),
   @ainparam07               VARCHAR(8000),
   @AOutParam01              VARCHAR output,   
   @AOutErrorNumber	         INT	 output,
   @AOutErrorString	         NVARCHAR(4000)	output
)
AS
   DECLARE @v_tablename     VARCHAR (30);
   DECLARE @v_execsql       NVARCHAR(MAX)
   DECLARE @query_columlist varchar(4000);
   DECLARE @queryresult     BIGINT;
-- description:    媒体记录相关的统计信息T_21_101
-- create date:    <2016-1-12>
-- =========================================================================================
--ainparam01        租户ID（默认为‘00000’）
--ainparam02        座席或分机
--ainparam03        T_00_901中的C001根据这个来查询座席或分机的记录  
--ainparam04        开始时间
--ainparam05        结束时间
--ainparam06        时间类型 L U
--ainparam07        统计列的字符串c001,c002,c003....
--AOutParam01       统计值输出
--============================================
BEGIN TRY

   SELECT @AOutErrorNumber = 0; 
   SELECT @AOutErrorString = 'successful';
   SELECT @queryresult =0;
    
   if  (@ainparam01 is  null) or (@ainparam01='') 
   begin
     set @v_tablename = 'T_21_101';
   end
   else
   begin
     set @v_tablename = 'T_21_101_' + @ainparam01;     
   end;

   print @v_tablename;

   set @query_columlist=replace(@ainparam07,',','+');

   print @query_columlist;
   
   if @ainparam03 is not null
   begin
	   set @v_execsql =
			 'SELECT @queryresult=isnull(sum('
		  + @query_columlist
		  + '),0) from '+@v_tablename
		  + ' WHERE C001='
		  + @ainparam02
		  + ' AND C002>='
		  + @ainparam04      
		  + ' AND C002<='
		  + @ainparam05
		  + ' AND C003='''
		  + @ainparam06           
		  + ''' AND C004 in (select c011 from T_00_901 where c001='
		  + @ainparam03+')';

   end
   else
   begin

	   set @v_execsql =
			 'SELECT @queryresult=isnull(sum('
		  + @query_columlist
		  + '),0) from '+@v_tablename
		  + ' WHERE C001='
		  + @ainparam02
		  + ' AND C002>='
		  + @ainparam04      
		  + ' AND C002<='
		  + @ainparam05
		  + ' AND C003='''
		  + @ainparam06           
		  + '''';
   end
   print @v_execsql;

   EXECUTE SP_EXECUTESQL @v_execsql, N'@queryresult BIGINT OUTPUT', @queryresult OUTPUT

   SELECT @AOutParam01=@queryresult;
           
END TRY
BEGIN CATCH
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH
