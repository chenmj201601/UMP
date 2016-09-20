CREATE  PROCEDURE P_24_101
( @AInParam01	int,
  @AInparam02	varchar,
  @AInparam03	nvarchar(128),
  @AOutErrorNumber  int OUTPUT ,
  @AOutErrorString  nvarchar(4000) OUTPUT
)
AS
-- ==========================================================================================================================
-- Name:      F_EncryptionProcessing01
-- Type:      表值函数
-- Create date:    <2015-07-26>
-- Description:    <读取策略绑定即将到期的绑定信息>
-- ==========================================================================================================================
-- Parameters Description:
-- @AInParam01:    提前时间 int,如果该参数 = 0， 那么获取系统默认提前通知时间（值 = 密钥策略 85）
-- @AInParam02:    时间单位（天 = 'D'、小时='H'、分钟='M'）
-- @AInParam03:    GeneratorID HOST

--遗留问题  默认提前天数  不能解密写死

-- ==========================================================================================================================

   DECLARE  @v_IntGeneratorCount         int;
   DECLARE  @v_IntAddSeconds             int;
   DECLARE  @v_DT_DurationEnd_Begin      datetime;
   DECLARE  @v_DT_DurationEnd_End        datetime;
   DECLARE  @v_DurationEnd_Begin         varchar(20);
   DECLARE  @v_DurationEnd_End           varchar(20);
   DECLARE  @v_count                     int;


BEGIN TRY

  SET NOCOUNT ON;

  SET @AOutErrorNumber= 0;
  SET @AOutErrorString = 'successful';

  DELETE FROM T_99_005;

  SELECT @v_IntGeneratorCount = COUNT(1) from t_24_004 WHERE c002 = @AInParam03 AND c006 = '1';

  IF (@v_IntGeneratorCount = 0) 
  BEGIN
    
      INSERT INTO T_99_005 VALUES(-1, '', '', '', GETUTCDATE(), GETUTCDATE());
      select * from T_99_005;
  END
  
  ELSE
  BEGIN 

      SELECT @v_DT_DurationEnd_Begin=GETUTCDATE() ; 

      IF(@AInParam01 <= 0) 
	  BEGIN
          SET @v_IntAddSeconds=7 * 24 * 3600; 
	  END
      ELSE
	  BEGIN
          IF(UPPER(@AInParam02) = 'D') 
             SET @v_IntAddSeconds = @AInParam01 * 24 * 3600;
          IF (UPPER(@AInParam02)  = 'H') 
             SET @v_IntAddSeconds = @AInParam01 * 3600;

          IF (UPPER(@AInParam02)  = 'M') 
             SET @v_IntAddSeconds = @AInParam01 * 60;
          ELSE
            SET @v_IntAddSeconds=7 * 24 * 3600;

      END;

	  SELECT @v_DT_DurationEnd_End = DATEADD(SS, @v_IntAddSeconds, @v_DT_DurationEnd_Begin);

      SELECT @v_DurationEnd_Begin=SUBSTRING(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(100), @v_DT_DurationEnd_Begin, 25) , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14)
      SELECT @v_DurationEnd_End=  SUBSTRING(REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR(100), @v_DT_DurationEnd_End, 25) , '-' , '' ) , ' ' , '' ) , ':' , '' ), 1, 14)



	  INSERT INTO T_99_005
	    SELECT c001,c002,c003,c003_2,CONVERT(DATETIME,c004),CONVERT(DATETIME,c004) FROM (
        SELECT -1 as c001,a.C002, a.C003, b.C002 as c003_2, substring(a.c004,1,4)+'-'+substring(a.c004,5,2)+'-'+substring(a.c004,7,2)+' '+substring(a.c004,9,2)+':'+substring(a.c004,11,2)+':'+substring(a.c004,13,2) as c004, substring(a.c005,1,4)+'-'+substring(a.c005,5,2)+'-'+substring(a.c005,7,2)+' '+substring(a.c005,9,2)+':'+substring(a.c005,11,2)+':'+substring(a.c005,13,2) as c005
        FROM T_24_002 a,T_24_001 b
        WHERE A.C003=B.C001 AND CONVERT(BIGINT,A.C004) >= CONVERT(BIGINT,@v_DurationEnd_Begin) 
                            AND CONVERT(BIGINT,A.C005)<=  CONVERT(BIGINT,@v_DurationEnd_End)) B ;


      select * from T_99_005  ;
  
   END;
   SET NOCOUNT OFF;
  
END TRY
BEGIN CATCH
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH