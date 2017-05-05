CREATE  PROCEDURE P_11_016 
(
   @AInParam01       VARCHAR(5) = '00000',
   @AInParam02       VARCHAR(20),
   @AInParam03       VARCHAR(20),
   @AOutErrorNumber  BIGINT         OUTPUT,
   @AOutErrorString  NVARCHAR(4000) OUTPUT
)
AS

-- description:    维护用户、角色管理或控制的资源列表为T_11_202_00000
-- 取被控制用户的上级用户的资源，写入到资源表中
-- create date:    <2014-9-19>
-- =========================================================================================
--AInParam01        租户ID（默认为‘00000’）
--AInParam02        用户ID
--AInParam03        操作人ID
-- =========================================================================================

   DECLARE @v_err_num     BIGINT;
   DECLARE @v_err_msg     VARCHAR (200);
   DECLARE @v_execsql     NVARCHAR (4000);
   DECLARE @n_count       INTEGER;
   DECLARE @error         BIGINT;
   DECLARE @errorSum      SMALLINT;

BEGIN TRY

	TRUNCATE TABLE T_99_002;
	TRUNCATE TABLE T_99_003;

  BEGIN TRAN  
	   SET @errorSum=0
	   SET @v_execsql='DELETE FROM T_11_202_'+@AInParam01+' WHERE C001='+@AInParam02;

	   EXEC SP_EXECUTESQL @v_execsql;
	   SET   @error=@@error
	   SET   @errorSum=@errorSum+@error
	   print @errorSum

	   SET @v_execsql='INSERT INTO T_99_002   
		  select '+@AInParam02+' as C001,b.C002,b.C003, b.C004, b.C005,'
						  +@AInParam03+' as C006, b.C007, b.C008, b.C009, b.C010, b.C011, b.C012,
						  b.C013, b.C014, b.C015, b.C016, b.C017, b.C018, b.C019,
						  b.C020,row_number()over(order by C001)as ID from  T_11_202_'+@AInParam01+' B where B.C001 in 
		(select C003 FROM T_11_201_'+@AInParam01+' C WHERE c.C003 LIKE ''106%'' AND c.C004='+@AInParam02+')';

	   print @v_execsql
	   EXEC SP_EXECUTESQL @v_execsql;
	   SET   @error=@@error
	   SET   @errorSum=@errorSum+@error
	   print @errorSum

	   INSERT INTO T_99_003 SELECT *  FROM T_99_002 WHERE ID IN (SELECT MAX(ID) FROM T_99_002 GROUP BY c001,c002)
	   SET   @error=@@error
	   SET   @errorSum=@errorSum+@error
	   print @errorSum

	   SET @v_execsql='INSERT INTO T_11_202_'+@AInParam01+'     
              SELECT C001, C002, C003, C004, C005, C006, C007, C008, C009, C010, C011, C012, C013, C014, C015, C016, C017, C018, C019, C020      
		      FROM T_99_003';

	   print @v_execsql
	   EXEC SP_EXECUTESQL @v_execsql;
     SET  @error=@@error
     SET  @errorSum=@errorSum+@error
     IF @errorSum=0
     BEGIN
         COMMIT TRAN;
         SET @AOuterrornumber = '0';
         SET @AOuterrorstring = 'successful';    
     END
     ELSE
     BEGIN
         ROLLBACK TRAN
         SET @AOutErrorNumber = '-1';
         SET @AOutErrorString = 'failed';
     END

END TRY

BEGIN CATCH
    ROLLBACK tran;
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH


