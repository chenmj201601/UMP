CREATE PROCEDURE P_24_006
(
 @AOutParam01 nvarchar(19) output , @AOutParam02 datetime output,@ErrorNumber int output, @ErrorString nvarchar(4000) output
)
AS
BEGIN TRY
	SELECT @ErrorNumber = 0, @ErrorString = '';
	SELECT @AOutParam01 = CONVERT(nvarchar(19), GETUTCDATE(), 120), @AOutParam02 = GETUTCDATE();
	SELECT GETUTCDATE() AS ServerTime;
END TRY
BEGIN CATCH
	SELECT @ErrorNumber = ERROR_NUMBER() , @ErrorString = ERROR_MESSAGE();
END CATCH