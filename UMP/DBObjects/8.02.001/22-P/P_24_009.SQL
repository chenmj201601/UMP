CREATE PROCEDURE  P_24_009 ( 
   @ainparam00              VARCHAR(5)= '00000', 
   @ainparam01              VARCHAR(20),
   @ainparam02              VARCHAR(20),
   @ainparam03              VARCHAR(20),
   @ainparam04              VARCHAR(20),  
   @ainparam05              VARCHAR(20), 
   @ainparam06              VARCHAR(1),        
   @AOutErrorNumber  BIGINT OUTPUT,
   @AOutErrorString  NVARCHAR(4000) OUTPUT
)
AS
-- description:    二代加密归档和备份更新
-- create date:    <2015-8-20>
-- =========================================================================================
--AInParam00       租户ID
--AInParam01       筛选策略编码
--AInParam02       AR策略编码
--AInParam03       对应的流水号
--ainparam04       设备ID
--ainparam05       UTC C221 
--ainparam06       STATE 1:succ 0 fail
-- ================================================
   DECLARE @v_sql          NVARCHAR(4000);
   DECLARE @v_C200         BIGINT;
   DECLARE @v_C201_w       VARCHAR(5);--AR策略编码1
   DECLARE @v_C211_w       VARCHAR(5);--筛选策略编码1   
   DECLARE @v_C221_w       VARCHAR(5);--归档时间1 UTC YYYYMMDDHH24mmss 
   DECLARE @v_C231_w       VARCHAR(5);--存储设备编码1
   DECLARE @v_C221_d       VARCHAR(20);                            
       
BEGIN TRY
 
 
    SET @aouterrornumber = '0';
    SET @aouterrorstring = 'successful';

	SELECT @v_C221_d=CONVERT(varchar(19), convert(datetime,@ainparam05), 121)
	print @v_C221_d

	SET @v_C221_d=replace(@v_C221_d,'-','');
	SET @v_C221_d=replace(@v_C221_d,' ','');
	SET @v_C221_d=replace(@v_C221_d,':','');

	print @v_C221_d
   
   
   SET @v_sql='SELECT @v_C200=C200+1 FROM T_21_001_'+@AInParam00+' WHERE C001='+@ainparam03;
            
   EXECUTE SP_EXECUTESQL v_sql, N'@v_C200 INT OUTPUT', @v_C200 Output
     
   
   IF @v_C200=1 
   BEGIN 
     SET @v_C201_w='C201';
     SET @v_C211_w='C211';       
     SET @v_C221_w='C221';
     SET @v_C231_w='C231';
   END
       
   IF @v_C200=2 
   BEGIN
     SET @v_C201_w='C202';
     SET @v_C211_w='C212';       
     SET @v_C221_w='C222';
     SET @v_C231_w='C232';
   END
            
   IF @v_C200=3 
   BEGIN
     SET @v_C201_w='C203';
     SET @v_C211_w='C213';       
     SET @v_C221_w='C223';
     SET @v_C231_w='C233';  
   END
                 
   IF @v_C200=4
   BEGIN
     SET @v_C201_w='C204';
     SET @v_C211_w='C214';       
     SET @v_C221_w='C224';
     SET @v_C231_w='C234'; 
   END          
   IF @v_C200=5
   BEGIN
     SET @v_C201_w='C205';
     SET @v_C211_w='C215';       
     SET @v_C221_w='C225';
     SET @v_C231_w='C235'; 
   END         
   IF @v_C200=6
   BEGIN
     SET @v_C201_w='C206';
     SET @v_C211_w='C216';       
     SET @v_C221_w='C226';
     SET @v_C231_w='C236'; 
    END        
   IF @v_C200=7 
   BEGIN
     SET @v_C201_w='C207';
     SET @v_C211_w='C217';       
     SET @v_C221_w='C227';
     SET @v_C231_w='C237'; 
   END         
  IF @v_C200=8
  BEGIN
     SET @v_C201_w='C208';
     SET @v_C211_w='C218';       
     SET @v_C221_w='C228';
     SET @v_C231_w='C238'; 
  END
                  
  IF @v_C200=9 
  BEGIN
     SET @v_C201_w='C209';
     SET @v_C211_w='C219';       
     SET @v_C221_w='C229';
     SET @v_C231_w='C239'; 
  END
           
  IF @v_C200=10 
  BEGIN
     SET @v_C201_w='C210';
     SET @v_C211_w='C220';       
     SET @v_C221_w='C230';
     SET @v_C231_w='C240'; 
   END                                                               
     
   BEGIN TRAN
	   
	   IF @ainparam06='1' 
	   BEGIN

		 INSERT INTO T_00_206 SELECT * FROM T_00_205 WHERE C001=CONVERT(BIGINT,@ainparam01) AND C002=CONVERT(BIGINT,@ainparam02) AND C003=CONVERT(BIGINT,@ainparam03);

	     SET @v_sql='UPDATE T_21_001_'+@ainparam00+' SET  C200='+@v_C200+','+@v_C201_w+'='+@AInParam02
		   +','+@v_C211_w+'='+@AInParam01
		   +','+@v_C221_w+'='+@v_C221_d+','
		   +@v_C231_w+'='+@ainparam04+'  WHERE C001='+@ainparam03; 
         
	     EXEC SP_EXECUTESQL @v_sql;	   
	   
	   END  
	   ELSE
	   BEGIN

		 INSERT INTO T_00_207 SELECT * FROM T_00_205 WHERE C001=CONVERT(BIGINT,@ainparam01) AND C002=CONVERT(BIGINT,@ainparam02) AND C003=CONVERT(BIGINT,@ainparam03);
  
	   END;

	   DELETE FROM T_00_205  WHERE C001=CONVERT(BIGINT,@ainparam01) AND C002=CONVERT(BIGINT,@ainparam02) AND C003=CONVERT(BIGINT,@ainparam03);

   COMMIT TRAN

END TRY
BEGIN CATCH
        ROLLBACK TRAN
    SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH
