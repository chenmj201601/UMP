CREATE  PROCEDURE P_11_011 (
   @AInParam01       VARCHAR(5) = '00000',
   @AInParam02       VARCHAR(20),
   @AOutErrorNumber  bigint OUTPUT,
   @AOutErrorString  NVARCHAR(4000) OUTPUT
)
AS

-- description:    删除t_11_006_00000机构下的所有记录
-- create date:    <2014-9-2>
-- ===============================================================
--ainparam01        租户ID（默认为‘00000’）
--ainparam02        机构ID (将此机构及其下面子机构全部删除)

   DECLARE @v_err_num     BIGINT;
   DECLARE @v_err_msg     VARCHAR (200);
   DECLARE @v_execsql     NVARCHAR (4000);
   DECLARE @tablenume     VARCHAR(200);
   DECLARE @error_num     BIGINT

BEGIN TRY 
     
     SET @error_num=0
     TRUNCATE TABLE T_99_004

     SET  @tablenume='T_11_006_'+@ainparam01;
  
     SET @v_execsql='WITH TEMP AS ( 
     SELECT * FROM '+@tablenume+' a WHERE C001='+@ainparam02+' 
     UNION ALL  
     SELECT B.*  FROM '+@tablenume+' B INNER JOIN TEMP T ON B.C004 = T.C001)  
   
     INSERT T_99_004 SELECT C001 FROM temp';

     print @v_execsql
     EXEC SP_EXECUTESQL @v_execsql
     SET @error_num=@@ERROR

     BEGIN tran

     SET @v_execsql='DELETE FROM T_11_201_'+@ainparam01+' WHERE C003 IN (SELECT C001 FROM T_99_004)';

     EXEC SP_EXECUTESQL @v_execsql
     SET @error_num=@error_num+@@ERROR

     SET @v_execsql='DELETE FROM T_11_201_'+@ainparam01+' WHERE C004 IN (SELECT C001 FROM T_99_004)';

     EXEC SP_EXECUTESQL @v_execsql
     SET @error_num=@error_num+@@ERROR

     SET @v_execsql='DELETE FROM T_11_006_'+@ainparam01+' WHERE C001 IN (SELECT C001 FROM T_99_004)';

     EXEC SP_EXECUTESQL @v_execsql
     SET @error_num=@error_num+@@ERROR

     IF @error_num=0  
       BEGIN
         COMMIT tran
         SET @AOuterrornumber = '0';
         SET @AOuterrorstring = 'successful';
       END
    ELSE
    BEGIN
        ROLLBACK tran
      SET @AOutErrorNumber = '-1';
      SET @AOutErrorString = 'failed';
    END

END TRY
BEGIN CATCH
    ROLLBACK tran
    SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH
