CREATE  PROCEDURE P_46_002 (
   @ainparam01               VARCHAR(5) = '00000',
   @ainparam02               VARCHAR(5),
   @ainparam03               NVARCHAR(512),
   @ainparam04               VARCHAR(20),
   @ainparam05               VARCHAR(20),
   @ainparam06               VARCHAR(20),
   @ainparam07               VARCHAR(MAX),
   @AOutParam01              VARCHAR output,
   @AOutErrorNumber  int output ,
   @AOutErrorString  nvarchar(4000) output
)
AS

   DECLARE @v_execsql       NVARCHAR(MAX)
   DECLARE @query_columlist varchar(4000);
   DECLARE @queryresult     BIGINT;
-- description:    录音数据初始统计结果表段表 T_46_031
-- create date:    <2016-1-22>
-- =========================================================================================
--ainparam01        C002租户ID（默认为‘00000’）
--ainparam02        C001座席或分机 对象类型 
--ainparam03        C003是指T_00_901中的C001根据这个来查询座席或分机的记录 对象ID
--ainparam04        C004统计参数 1、录音时长 2、 通话时长 3、呼入电话数 4、呼出电话数  5、转换次数 6、座席讲话时长 7、8、9、10、
--ainparam05        C005开始时间
--ainparam06        C006开始时间
--ainparam07        统计列的字符串c001,c002,c003....
--AOutParam01       统计值输出
--============================================
BEGIN TRY

   SET @aouterrornumber = '0';
   SET @aouterrorstring = 'successful';
   

   SET @query_columlist=replace(@ainparam07,',','+');

   if @ainparam03 is not null 
   begin

   SET  @v_execsql =
           'SELECT isnull(sum('
        + @query_columlist
        + '),0) from T_46_041 '
        + ' WHERE C002='''
        + @ainparam01
        + ''' AND C001='
        + @ainparam02        
        + ' AND C004='
        + @ainparam04
        + ' AND C005>='
        + @ainparam05
        + ' AND C006<='
        + @ainparam06        
        + ' AND C003 in (select c011 from T_00_901 where c001='
        + @ainparam03+')';
   end
   else
   begin

    SET @v_execsql =
           'SELECT isnull(sum('
        + @query_columlist
        + '),0) from T_46_041 '
        + ' WHERE C002='''
        + @ainparam01
        + ''' AND C001='
        + @ainparam02
        + ' AND C004='
        + @ainparam04                 
        + ' AND C005>='
        + @ainparam05
        + ' AND C006<='
        + @ainparam06;
        
   end;
   print @v_execsql;
   EXECUTE SP_EXECUTESQL @v_execsql, N'@queryresult BIGINT OUTPUT', @queryresult OUTPUT

   
   SELECT @AOutParam01=@queryresult;

END TRY
BEGIN CATCH
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH