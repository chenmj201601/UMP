CREATE  PROCEDURE P_11_012 (
   @AInParam01          VARCHAR(5) = '00000',
   @AInParam02          NVARCHAR(1024),
   @AInParam03          NVARCHAR(1024),
   @AInParam04          VARCHAR(1024)='3FE3B4AA02D4CFCB26D2D6C8BFA1F095F55320344719954CE0A3B7346BCE6441079503E67FFB8ACA5B8216460E555795CE4C757C18B6FF8C87F6C044777F7C7C',
   @AInParam05          VARCHAR(5) = '20002',
   @AInParam06          VARCHAR(20),
   @AInParam07          CHAR = 'S',
   @AInParam08          CHAR = '0',
   @AInParam09          CHAR = 'N',
   @AInParam10          CHAR = '1',
   @AInParam11          CHAR = '0',
   @AInParam12          VARCHAR(32) = '1111111111111111111111111111111111111111',
   @AInParam13          VARCHAR(20) = '2014-01-01 00:00:00',
   @AInParam14          VARCHAR(256),
   @AInParam15          VARCHAR(128)=NULL,
   @AInParam16          VARCHAR(20),
   @AInParam17          VARCHAR(512),
   @AInParam18          VARCHAR(512)=NULL,
   @AInParam19          VARCHAR(20)=NULL,
   @AInParam20          VARCHAR(20)=NULL,
   @AOutParam01         VARCHAR(20) OUTPUT,
   @AOutErrorNumber     BIGINT OUTPUT,
   @AOutErrorString     NVARCHAR(4000) OUTPUT
)
AS
-- description:    增加新用户T_11_005_  增加资源t_11_201_
-- create date:    <2014-9-4>
-- ==========================================================
--ainparam01        租户标识（默认为‘00000’）
--ainparam02        账户
--ainparam03        全名
--ainparam04        密码
--ainparam05        加密版本和方法
--ainparam06        机构ID
--ainparam07        来源
--ainparam08        是否锁定
--ainparam09        锁定方式
--ainparam10        是否活动
--ainparam11        是否删除
--ainparam12        其他状态
--ainparam13        最后登录时间
--ainparam14        开始时间
--ainparam15        失效时间
--ainparam16        创建人
--ainparam17        创建时间
--ainparam18        入职时间
--ainparam19        离职时间   c022
--ainparam20        最后修改密码的时间
--==============================================================
--aoutparam01       用户ID
--==============================================================
--@AOutErrorNumber -1  User already exists
--==============================================================

   DECLARE @v_serialid     VARCHAR (19);
   DECLARE @v_err_num      BIGINT;
   DECLARE @v_err_msg      VARCHAR (1024);
   DECLARE @v_ainparam20   NVARCHAR(19);
   DECLARE @v_execsql      NVARCHAR (4000);
   DECLARE @v_execsql2     NVARCHAR (4000);
   DECLARE @v_count        INTEGER;
   DECLARE @error_num      BIGINT;
   DECLARE @v_datetime     VARCHAR (14);

BEGIN TRY

   SET @error_num=0
   SET @v_ainparam20 = @AInParam20;
   
   SET @v_execsql ='SELECT @v_count=COUNT(1) FROM T_11_005_'+@AInParam01+' WHERE C002= '''+@AInParam02 + '''';
   print @v_execsql
   EXECUTE sp_executesql @v_execsql, N'@v_count INT OUTPUT', @v_count Output
   print @v_count  
   IF @v_count>0 
   BEGIN
      SET @AOuterrornumber = '-1';
      SET @AOutErrorString = 'User already exists'
      RETURN;         
   END; 
   
   SELECT @v_datetime=SUBSTRING(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(100), GETDATE(), 25) , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14) 
   
   EXEC P_00_001 '11', '102', @AInParam01, @v_datetime, @v_serialid OUTPUT, @v_err_num OUTPUT, @v_err_msg OUTPUT;

   print @v_serialid;
   
   IF  @v_err_num<>0 
   BEGIN
     SET @AOutErrorNumber=@v_err_num
     SET @AOutErrorString='Serial number generation error'
   RETURN
   END

   BEGIN TRAN

     SET @v_execsql ='INSERT INTO T_11_005_'+ @AInParam01
      + ' VALUES (CONVERT(BIGINT,'''
      + @v_serialid
      + '''),'''
      + @AInParam02
      + ''', '''
      + @AInParam03
      + ''','''
      + @ainparam04
      + ''','''
      + @ainparam05
      + ''','
      + @AInParam06
      + ', '''
      + @AInParam07
      + ''','''
      + @AInParam08
      + ''','''
      + @AInParam09
      + ''','''
      + @AInParam10
      + ''','''
      + @AInParam11
      + ''','''
      + @AInParam12

      print @v_execsql;

     IF (@AInParam15 IS NOT NULL) AND (@AInParam15<>'')

     SET @v_execsql2 = ''','''+@ainparam13+''','''','''',0,'''         --c013  c014 c015 c016 c017 c018
      + @AInParam14+ ''','''+@AInParam15+''''
     ELSE
     SET @v_execsql2 = ''','''+@ainparam13+''','''','''',0,'''       
      + @AInParam14+ ''',NULL' 
 
   
     IF ISNUMERIC(@AInParam16)=1
      SET @v_execsql2 =@v_execsql2+ ',CONVERT(BIGINT,'''+@AInParam16+'''),''' --c019
     ELSE
     BEGIN
        RETURN
        SET @AOuterrornumber = '-1';
        SET @AOuterrorstring = '@AInParam16 is error';
     END
     
     SET @v_execsql2 =@v_execsql2+ @AInParam17 + ''',';                           --c020
                                                           
     IF (@AInParam18 IS NOT NULL) AND (@AInParam18<>'')
       SET @v_execsql2 =@v_execsql2 + 'CONVERT(DATETIME,'''+@AInParam18+''')'     --c021
     ELSE
       SET @v_execsql2 =@v_execsql2 + 'NULL' 

     SET @v_execsql2 =@v_execsql2 + ',NULL';                                   --c022

     IF  (@AInParam20 IS NOT NULL) AND (@AInParam20<>'') 
       SET @v_execsql2 =@v_execsql2 + ',CONVERT(DATETIME,'''+@v_ainparam20+''')'  --c023
     ELSE
       SET @v_execsql2 =@v_execsql2 + ',NULL'     

     SET @v_execsql2 =@v_execsql2 + ',0,''1'',NULL)';                 --c024,--c025,--c026
   
     print (@v_execsql+@v_execsql2);

     EXEC  (@v_execsql+@v_execsql2);

     SET @error_num=@error_num+@@ERROR
     print @error_num

     SET @v_execsql='INSERT INTO T_11_201_'
      + @AInParam01
      + ' SELECT 0,0,C003, '
      + @v_serialid
      + ',GETDATE(),'
      + 'CONVERT(DATETIME,''2199-12-31'') FROM T_11_201_'
          + @ainparam01
          + ' WHERE  C003 > 1020000000000000000 and C003 < 1030000000000000000 and C004 = '+@ainparam16 ;
			  

     print @v_execsql

	 
     EXEC SP_EXECUTESQL  @v_execsql;

	 SET  @v_execsql='INSERT INTO T_11_201_'
      + @AInParam01
      + ' Values (0,0,'
      + @v_serialid+','+ @v_serialid
      + ',GETDATE(),'
      + 'CONVERT(DATETIME,''2199-12-31'') )';      

	 print @v_execsql

	 
     EXEC SP_EXECUTESQL  @v_execsql;	  

     print @v_execsql

     SET @error_num=@error_num+@@ERROR
     print  @error_num

     SET @v_execsql='INSERT INTO T_11_203_'
      + @AInParam01
      + ' SELECT  '
      + @v_serialid
      + ',C001,C002,C012,'
      + ' ''1'', ''1'', ''1'', ''1'', ''1'', ''1'',''1'', ''0'', ''0'', ''0'', ''0'''
      +',C007,0 FROM T_00_102';
     print @v_execsql

     EXEC SP_EXECUTESQL  @v_execsql;
     SET @error_num=@error_num+@@ERROR
     print  @error_num


    IF @error_num=0
    BEGIN
      COMMIT TRAN
      SET @aoutparam01 = @v_serialid;   
      SET @AOuterrornumber = '0';
      SET @AOuterrorstring = 'successful';
   END
   ELSE
   BEGIN
      ROLLBACK TRAN
      SET @AOutErrorNumber = '-1';
      SET @AOutErrorString = 'failed';
   END

END	TRY
BEGIN CATCH
    ROLLBACK TRAN
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH