CREATE  PROCEDURE P_11_013 (
   @AInParam01       VARCHAR(5)= '00000',
   @AInParam02       VARCHAR(20),
   @AOutErrorNumber  BIGINT OUTPUT,
   @AOutErrorString  NVARCHAR(4000) OUTPUT
)
AS
-- description:    删除用户账户
-- create date:    <2014-9-5>
-- ===============================================================
--ainparam01        租户ID（默认为‘00000’）
--ainparam02        用户ID

   DECLARE @v_ainparam01   VARCHAR (5);
   DECLARE @v_execsql      NVARCHAR(4000);
   declare @error_num      BIGINT;
 
BEGIN TRY
 
    BEGIN TRAN

    SET @error_num=0
    SET @v_execsql ='DELETE FROM T_11_005_'+ @ainparam01+ ' WHERE C001 = '+@ainparam02;
    print @v_execsql;
    EXEC SP_EXECUTESQL @v_execsql
    SET @error_num=@error_num+@@ERROR

     SET @v_execsql = 'DELETE FROM T_11_101_'+ @ainparam01+' WHERE C001 = '+@ainparam02;
     print @v_execsql;
     EXEC SP_EXECUTESQL @v_execsql
     SET @error_num=@error_num+@@ERROR

     SET @v_execsql = 'DELETE FROM T_11_201_'+ @ainparam01+' WHERE C003 = '+@ainparam02;
     print @v_execsql;
     EXEC SP_EXECUTESQL @v_execsql
     SET @error_num=@error_num+@@ERROR

     SET @v_execsql = 'DELETE FROM T_11_201_'+ @ainparam01+' WHERE C004 = '+@ainparam02;
     print @v_execsql;
     EXEC SP_EXECUTESQL @v_execsql
     SET @error_num=@error_num+@@ERROR

     SET @v_execsql = 'DELETE FROM T_11_203_'+ @ainparam01+' WHERE C001 = '+@ainparam02;
     print @v_execsql;
     EXEC SP_EXECUTESQL @v_execsql
     SET @error_num=@error_num+@@ERROR

     IF (@error_num=0) 
     BEGIN
            COMMIT TRAN
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