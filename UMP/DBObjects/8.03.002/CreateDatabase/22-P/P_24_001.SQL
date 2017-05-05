CREATE  PROCEDURE P_24_001 (
   @ainparam01              VARCHAR(5) = '00000',
   @ainparam02              NVARCHAR(1024),
   @ainparam03              VARCHAR='1',
   @ainparam04              VARCHAR='U',
   @ainparam05              VARCHAR(20)='0',
   @ainparam06              VARCHAR(1024),
   @ainparam07              VARCHAR='M',
   @ainparam08              VARCHAR(20),
   @ainparam09              VARCHAR(20),
   @ainparam10              VARCHAR(5)='0',
   @ainparam11              VARCHAR='1',
   @ainparam12              VARCHAR='1',
   @ainparam13              VARCHAR='1',
   @ainparam14              VARCHAR(5)='0',
   @ainparam15              VARCHAR='1',
   @ainparam16              VARCHAR(5)='0',
   @ainparam17              VARCHAR='1',
   @ainparam18              VARCHAR(5)='0',
   @ainparam19              VARCHAR='0',
   @ainparam20              VARCHAR(5)='0',
   @ainparam21              VARCHAR(5)='0',
   @ainparam22              VARCHAR(5)='0',
   @ainparam23              VARCHAR(5)='6',
   @ainparam24              VARCHAR(5)='64',
   @ainparam25              VARCHAR(20)='0',
   @ainparam26              VARCHAR(20),
   @ainparam27              VARCHAR(1024)='',
   @aOutParam01             VARCHAR(20)    output,
   @aouterrornumber         bigint         output,
   @aouterrorstring         NVARCHAR(4000) output
)
AS
-- description:    增加加密密钥策略配置表T_24_001
-- create date:    <2015-7-23>
-- =========================================================================================
--@ainparam01        租户标识（默认为‘00000’）
--@ainparam02        策略名称
--@ainparam03        是否启用。'1'、启用；'2'、停用
--@ainparam04        策略类型。c、重复执行（自动更新）；u、用户输入
--@ainparam05        类型为 “用户输入”的生成密钥时间，必须晚于当前时间。如果值为 20140101000000 表示立即执行
--@ainparam06        类型为 “用户输入”时加密密钥。启用该密钥时间为typeutime，数据使用key_normal1加密处理
--@ainparam07        自动生成密钥执行周期。d：1天；w：1周；t：1旬；m：1月；u：自定义天数
--@ainparam08        持续时间（开始 utc）。同时表示该策略生效时间。now表示立即生效
--@ainparam09        持续时间（结束 utc）。unlimited表示无结束日期
--@ainparam10        执行周期变量。“typeccycle = w”时   0：周日；1：周一。默认为 0；“typeccycle = u”时   填写实际天数。如：10、30、60、120。
--@ainparam11        立即更新密钥后执行周期是否重置。'0'否；'1'是。(只针对 “重复执行周期”类型为 “u：自定义天数”有效)
--@ainparam12        密码复杂性校验。'0'、不校验；'1'、校验
--@ainparam13        必须包含大写字母。 a ～ z
--@ainparam14        包含大写字母个数
--@ainparam15        必须包含小写字母。a ~ z
--@ainparam16        包含小写字母个数
--@ainparam17        必须包含数字。 0 ～ 9
--@ainparam18        包含数字个数。
--@ainparam19        必须包含特殊字符。如：! $ # %
--@ainparam20        包含特殊字符个数。
--@ainparam21        在多少个密钥内不能重复
--@ainparam22        在多少天密钥内不能重复
--@ainparam23        最短长度。
--@ainparam24        最长长度
--@ainparam25        策略创建人
--@ainparam26        策略创建时间 utc
--@ainparam27        策略描述

-- ================================================
--aoutparam01       用户ID
-- ================================================
--流水号 模块号 24 操作码 402
-- ================================================
   DECLARE @v_serialid     VARCHAR (19);
   DECLARE @v_err_num      BIGINT;
   DECLARE @v_err_msg      NVARCHAR (4000);
   DECLARE @v_ainparam03   VARCHAR (1);
   DECLARE @v_ainparam08   VARCHAR (20);
   DECLARE @v_ainparam09   VARCHAR (20);
   DECLARE @v_ainparam05   VARCHAR (20);           
   DECLARE @v_ainparam26   VARCHAR (20);         
   DECLARE @v_execsql      NVARCHAR (4000);
   DECLARE @v_datetime     VARCHAR (14);
   
BEGIN TRY
  
   select @v_ainparam05=replace(@ainparam05,'-','') 
   select @v_ainparam05=replace(@v_ainparam05,' ','')
   select @v_ainparam05=replace(@v_ainparam05,':','') 

   select @v_ainparam08=replace(@ainparam08,'-','') 
   select @v_ainparam08=replace(@v_ainparam08,' ','')
   select @v_ainparam08=replace(@v_ainparam08,':','') 

   select @v_ainparam09=replace(@ainparam09,'-','') 
   select @v_ainparam09=replace(@v_ainparam09,' ','')
   select @v_ainparam09=replace(@v_ainparam09,':','') 


   select @v_ainparam26=replace(@ainparam26,'-','') 
   select @v_ainparam26=replace(@v_ainparam26,' ','')
   select @v_ainparam26=replace(@v_ainparam26,':','') 

	print '11'
                  
   SELECT @v_datetime=SUBSTRING(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(100), GETDATE(), 25) , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14)

   print @ainparam01

   print @v_datetime

   EXEC P_00_001  '24','402',@ainparam01, @v_datetime,@v_serialid OUTPUT, @v_err_num OUTPUT, @v_err_msg OUTPUT

   IF (CONVERT(BIGINT, @v_serialid)<0 or  @v_err_num<>0 )
   BEGIN
     SET @AOutErrorNumber=-1
     SET @AOutErrorString='Serial number generation error'
   RETURN
   END
   PRINT @v_ainparam26
   SET @v_execsql =
         'INSERT INTO T_24_001'
      + ' VALUES ('''
      + @ainparam01
      + ''','''
      + @v_serialid
      + ''','''
      + @ainparam02
      + ''', '''
      + @ainparam03
      + ''','''
      + @ainparam04
      + ''','''
      + @v_ainparam05
      + ''','''
      + @ainparam06
      + ''', '''
      + @ainparam07
      + ''','''
      + @v_ainparam08
      + ''','''
      + @v_ainparam09
      + ''','''
      + @ainparam10
      + ''','''
      + @ainparam11
      + ''','''
      + @ainparam12
      + ''','''
      + @ainparam13
      + ''','''
      + @ainparam14
      + ''','''
      + @ainparam15
      + ''','''
      + @ainparam16
      + ''','''
      + @ainparam17
      + ''','''
      + @ainparam18
      + ''','''
      + @ainparam19
      + ''','''
      + @ainparam20
      + ''','''
      + @ainparam21
      + ''','''
      + @ainparam22
      + ''','''
      + @ainparam23
      + ''','''
      + @ainparam24
      + ''',0,'''','''','''
      + @ainparam25
      + ''','''
      +@v_ainparam26
      +''',0,'+@v_ainparam26+','''','''','''','''
      + @ainparam27
      +''')';

	print @v_execsql

    EXEC SP_EXECUTESQL @v_execsql

    IF @@ERROR=0
    BEGIN
	  SET @aOutParam01=@v_serialid;
      SET @AOutErrorNumber = '0';
      SET @AOutErrorString = 'successful';
    END
    ELSE
    BEGIN
	  SET @aOutParam01=0;
      SET @AOutErrorNumber = '-1';
      SET @AOutErrorString = 'failed';
    END
   
END TRY
BEGIN CATCH
    SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH
