CREATE PROCEDURE P_21_001
(
	@AInParam01       NVARCHAR(2000)=NULL,
	@AInParam02       NVARCHAR(2000)=NULL,
	@AInParam03       NVARCHAR(2000)=NULL,
	@AInParam04       NVARCHAR(2000)=NULL,
	@AInParam05       NVARCHAR(2000)=NULL,
	@AInParam06       NVARCHAR(2000)=NULL,
	@AInParam07       NVARCHAR(2000)=NULL,
	@AInParam08       NVARCHAR(2000)=NULL,
	@AInParam09       NVARCHAR(2000)=NULL,
	@AInParam10       NVARCHAR(2000)=NULL,
	@AOutErrorNumber  BIGINT         OUTPUT,
	@AOutErrorString  NVARCHAR(2048) OUTPUT
   )
AS
DECLARE @LStrSerialNumber       VARCHAR(20);
DECLARE @LIntErrorNumber        BIGINT;
DECLARE @LStrErrorMessage		NVARCHAR(2048);
BEGIN TRY
	SET @AOutErrorNumber = 0;
	SET @LStrSerialNumber = 0;
	SET @LIntErrorNumber = 0;
	SET @LStrErrorMessage = N'';
	INSERT INTO T_21_000 VALUES(CONVERT(BIGINT,@LStrSerialNumber), @AInParam01, @AInParam02, @AInParam03, @AInParam04, @AInParam05, @AInParam06, @AInParam07, @AInParam08, @AInParam09, @AInParam10, @LIntErrorNumber, @LStrErrorMessage, GETUTCDATE());

END TRY
BEGIN CATCH
  SELECT @AOutErrorNumber = ERROR_NUMBER() , @AOutErrorString = ERROR_MESSAGE();
END CATCH