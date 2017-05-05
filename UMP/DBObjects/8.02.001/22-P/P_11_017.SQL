CREATE  PROCEDURE P_11_017
(
   @AInParam00       VARCHAR(5) = '00000',
   @AInParam01       VARCHAR(20),
   @AInParam02       NVARCHAR(1024),
   @AInParam03       NVARCHAR(1024),
   @AInParam04       NVARCHAR(1024),
   @AInParam05       NVARCHAR(1024),
   @AInParam06       NVARCHAR(1024),
   @AInParam07       NVARCHAR(1024),
   @AInParam08       NVARCHAR(1024),
   @AOutErrorNumber  BIGINT         OUTPUT,
   @AOutErrorString  NVARCHAR(4000) OUTPUT
)
AS

-- description:    维护系统资源属性（用户、座席可使用该表）数据部分 T_11_101_00000，
-- create date:    <2015-7-31>
-- =========================================================================================
--AInParam00        租户ID（默认为‘00000’）
--AInParam01        资源ID
--AInParam02        KEY--C017
--AInParam03        是否可用 C016
--AInParam04        HOST--C017
--AInParam05        PORT--C019
--AInParam06        MACHINE ID C020
--AInParam07        USER C011
--AInParam08        PASSWORD C012

-- =========================================================================================

   DECLARE @v_err_num     BIGINT;
   DECLARE @v_err_msg     VARCHAR (200);
   DECLARE @v_execsql     NVARCHAR (4000);
   DECLARE @v_count       INTEGER;
   DECLARE @error         SMALLINT;
   DECLARE @errorSum      SMALLINT;

BEGIN TRY

   SET @v_execsql='SELECT @v_count= COUNT(1) FROM T_11_101_'+@AInParam00+'  A WHERE C001='+ @AInParam01;
   print @v_execsql
   EXECUTE sp_executesql @v_execsql, N'@v_count INT OUTPUT', @v_count Output


   IF @v_count=0 
   BEGIN
     BEGIN TRAN
	  
     print 1

	 SET @errorSum=0

     SET @v_execsql='INSERT INTO t_11_101_'+@AInParam00+' (C001,C002,C011,C012,C014,C016,C017,C019,C020) '
              +'VALUES ('''+@AInParam01
              +''',''1'',N'''
              +@AInParam02+''',N'''+@AInParam02+''',N'''+@AInParam02+''',N'''
              +@AInParam03+''',N'''
              +@AInParam04+''',N'''
              +@AInParam05+''',N'''
              +@AInParam06+''')';
     
	 print @v_execsql

     EXECUTE sp_executesql @v_execsql

	 SET   @error=@@error
	 SET   @errorSum=@errorSum+@error

     SET @v_execsql='INSERT INTO t_11_101_'+@AInParam00+' (C001,C002,C011,C012) '
              +'VALUES ('''+@AInParam01
              +''',''92'',N'''
              +@AInParam07+''',N'''
              +@AInParam08+''')';
	 print @v_execsql
     EXECUTE sp_executesql @v_execsql
	 SET   @error=@@error
	 SET   @errorSum=@errorSum+@error

     SET @v_execsql='INSERT INTO t_11_101_'+@AInParam00+' (C001,C002,C011) '
              +'VALUES ('''+@AInParam01
              +''',''93'',N'''
              +@AInParam02+''')';
	 print @v_execsql
     EXECUTE sp_executesql @v_execsql;
	 SET   @error=@@error
	 SET   @errorSum=@errorSum+@error	 	 

     IF @errorSum=0
     BEGIN
         COMMIT TRAN;
		 print @errorSum
         SET @AOuterrornumber = '0';
         SET @AOuterrorstring = 'successful';    
     END
     ELSE
     BEGIN
         ROLLBACK TRAN
		 	 	 print @errorSum
         SET @AOutErrorNumber = '-1';
         SET @AOutErrorString = 'failed';
     END
   END
   ELSE
   BEGIN
        BEGIN TRAN 
	    SET @errorSum=0
		print 2
		 print @v_count

        SET @v_execsql='UPDATE t_11_101_'+@AInParam00
              +' SET C001='+@AInParam01
              +',C011=N'''+@AInParam02
              +''',C012=N'''+@AInParam02
              +''',C014=N'''+@AInParam02
              +''',C016=N'''+@AInParam03             
              +''',C017=N'''+@AInParam04
              +''',C019=N'''+@AInParam05
              +''',C020=N'''+@AInParam06
              +''' WHERE C001='+@AInParam01
              +' AND C002=1';

     EXECUTE sp_executesql @v_execsql;
	 SET   @error=@@error
	 SET   @errorSum=@errorSum+@error

     SET @v_execsql='UPDATE t_11_101_'+@AInParam00
              +' SET C001='+@AInParam01
              +',C011=N'''+@AInParam07
              +''',C012=N'''+@AInParam08
              +''' WHERE C001='+@AInParam01
              +' AND C002=92';

     EXECUTE sp_executesql @v_execsql;
	 SET   @error=@@error
	 SET   @errorSum=@errorSum+@error

     SET @v_execsql='UPDATE t_11_101_'+@AInParam00
              +' SET C001='''+@AInParam01
              +''',C011=N'''+@AInParam07
              +''' WHERE C001='+@AInParam01
              +' AND C002=93';

     EXECUTE sp_executesql @v_execsql;
	 SET   @error=@@error
	 SET   @errorSum=@errorSum+@error
     
	 IF @errorSum=0
     BEGIN
	     print @errorSum
         COMMIT TRAN;
         SET @AOuterrornumber = '0';
         SET @AOuterrorstring = 'successful';    
     END
     ELSE
     BEGIN
	 	 print @errorSum
         ROLLBACK TRAN
         SET @AOutErrorNumber = '-1';
         SET @AOutErrorString = 'failed';
     END

   END

END TRY


BEGIN CATCH
    ROLLBACK tran;
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH


