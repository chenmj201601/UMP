@date /t
@time /t
@echo on


@echo ===START COPY UMPRoot===

xcopy /y /f ".\UMPClient\UMPClient\bin\Release\UMPClient.exe" "%RootPath%"

xcopy /y /f ".\Other\UMPSetup\UMPUninstall\bin\Release\UMPUninstall.exe" "%RootPath%"


@date /t
@time /t
@echo copy File Finish.