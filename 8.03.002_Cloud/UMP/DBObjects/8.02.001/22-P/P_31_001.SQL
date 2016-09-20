CREATE PROCEDURE P_31_001 (
   @AInParam01         VARCHAR(5) = '00000',
   @AInParam02         NVARCHAR(512),
   @AInParam03         VARCHAR(5)='0',
   @AInParam04         VARCHAR(5),
   @AInParam05         VARCHAR(5)='0',
   @AInParam06         NVARCHAR(512)=null,
   @AInParam07         VARCHAR(5)='0',
   @AInParam08         VARCHAR(5)='0',
   @AInParam09         VARCHAR(1)=null,
   @AInParam10         NVARCHAR(1014)=null,
   @AOutParam01        VARCHAR(20)    OUTPUT,
   @AOutErrorNumber    BIGINT         OUTPUT,
   @AOutErrorString    NVARCHAR(4000) OUTPUT
)
AS

-- description:    增加自定义查询界面表记录t_31_040_00000
-- create date:    <2014-10-30>
-- =========================================================================================
--AInParam01        租户标识（默认为‘00000’）
--AInParam02        名称
--AInParam03        规格(1: 1*4 2: 1*2 3:1*1)
--AInParam04        类型(1:普通文件 2:模糊匹配文本 3:逗号间隔多值文本 4:下拉枚举 5:多选 6:复选 10:时间范围 11:时长范围
--AInParam05        TabItem索引(0-9内置标签)
--AInParam06        TabItem名称
--AInParam07        在一个TabItem下的顺号
--AInParam08        默认是否显示(1:显示 0:不显示)
--AInParam09        保留(标识使用)
--AInParam10        保留(额外数据使用)
-- =========================================================================================
--AOutParam01       返回流水号ID   31  303
-- =========================================================================================

   DECLARE @v_serialid    VARCHAR(19);
   DECLARE @v_err_num     BIGINT;
   DECLARE @v_err_msg     VARCHAR(200);
   DECLARE @v_ainparam01  VARCHAR(5);      
   DECLARE @v_execsql     NVARCHAR(4000);

BEGIN TRY

   IF @AInParam06 IS  NULL
   BEGIN
     SET @AInParam06=''
   END
     
   IF @AInParam09 IS  NULL
   BEGIN
     SET @AInParam09=''
   END
   IF @AInParam10 IS  NULL
   BEGIN
     SET @AInParam10=''
   END

   EXEC P_00_001  '31','303',@ainparam01, DEFAULT,@v_serialid OUTPUT, @v_err_num OUTPUT, @v_err_msg OUTPUT

   print @v_serialid
   print @v_err_num

   IF (CONVERT(BIGINT, @v_serialid)<0 OR  @v_err_num<>0 )
   BEGIN
     SET @AOutErrorNumber=-1
     SET @AOutErrorString='Serial number generation error'
	 RETURN
   END

   SET @v_execsql ='INSERT INTO T_31_040_'+@ainparam01+' VALUES ('+@v_serialid+',N'''+@ainparam02+''','+@ainparam03
               +','+@ainparam04+',N'''+@ainparam05+''',N'''+@ainparam06+''', '+@ainparam07
         +','+@ainparam08+','''
               +@ainparam09
         +''',N'''+@ainparam10+''')';


   EXEC SP_EXECUTESQL @v_execsql

   IF @@ROWCOUNT=1 
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
