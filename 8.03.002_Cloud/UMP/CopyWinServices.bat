@date /t
@time /t
@echo on


@echo ===START COPY WinServices===

xcopy /y /f ".\VCCommon\VCCommon\bin\Release\VCCommon.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Classes\CircleQueueClass\CircleQueueClass\bin\Release\CircleQueueClass.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Classes\PFShareClasses01\PFShareClasses01\bin\Release\PFShareClasses01.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Classes\PFShareClassesS\PFShareClassesS\bin\Release\PFShareClassesS.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\Library\UMPEncryption\UMPEncryptionS\bin\Release\UMPEncryptionS_Secure\UMPEncryptionS.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\Library\VCNAudio\VCNAudio\bin\Release\VCNAudio.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\Library\VCSharpZipLib\VCSharpZipLib\bin\Release\VCSharpZipLib.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\Library\VCDBAccess\VCDBAccess\bin\Release\VCDBAccess.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\SDKs\DEC4Net\DEC4Net\bin\Release\DEC4Net.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\SDKs\NMon4Net\NMon4Net\bin\Release\NMon4Net.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\Library\VCWebSocket\VCWebSocket\bin\Release\VCWebSocket.dll" "%RootPath%\WinServices\"

xcopy /y /f ".\UMPCommon\UMPCommon\bin\Release\UMPCommon.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\UMPCommon\UMPCommunications\bin\Release\UMPCommunications.dll" "%RootPath%\WinServices\"


xcopy /y /f ".\UMPS1110\Common11101\bin\Release\Common11101.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\UMPS1110\UMPResourceXmls\bin\Release\UMPResourceXmls.dll" "%RootPath%\WinServices\"

xcopy /y /f ".\UMP.PF.Services\UMPService03\CommonService03\bin\Release\CommonService03.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService04\CommonService04\bin\Release\CommonService04.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService05\UMPService05\bin\Release\CommonService05.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService05\UMPService05\bin\Release\UMPService05.exe.config" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService07\CommonService07\bin\Release\CommonService07.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService10\CommonService10\bin\Release\CommonService10.dll" "%RootPath%\WinServices\"

xcopy /y /f ".\UMP.PF.Services\UMPService00\UMPService00\bin\Release\UMPService00.exe" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService01\UMPService01\bin\Release\UMPService01.exe" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService02\UMPService02\bin\Release\UMPService02.exe" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService04\UMPService04\bin\Release\UMPService04.exe" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService05\UMPService05\bin\Release\UMPService05.exe" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService06\UMPService06\bin\Release\UMPService06.exe" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService06\UMPService06\bin\Release\UMPService06.exe.config" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService07\UMPService07\bin\Release\UMPService07.exe" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService08\UMPService08\bin\Release\UMPService08.exe" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService08\UMPService08\bin\Release\UMPService08.exe.config" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService10\UMPService10\bin\Release\UMPService10.exe" "%RootPath%\WinServices\"

xcopy /y /f ".\UMP.PF.Services\UMPService09\UMPService09\bin\Release\UMPService09.exe" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService09\UMPService09\bin\Release\PMStatistics.xml" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService09\UMPService09\bin\Release\UMPService09.DAL.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService09\UMPService09\bin\Release\UMPService09.DALFactory.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService09\UMPService09\bin\Release\UMPService09.exe.config" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService09\UMPService09\bin\Release\UMPService09.Log.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService09\UMPService09\bin\Release\UMPService09.Model.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\UMP.PF.Services\UMPService09\UMPService09\bin\Release\UMPService09.Utility.dll" "%RootPath%\WinServices\"

xcopy /y /f ".\SDKs\DEC4Net\DEC4Net\bin\Release\DEC4Net.dll" "%RootPath%\WinServices\"
xcopy /y /f ".\SDKs\License4Net\License4Net\bin\Release\License4Net.dll" "%RootPath%\WinServices\"

@date /t
@time /t
@echo copy File Finish.