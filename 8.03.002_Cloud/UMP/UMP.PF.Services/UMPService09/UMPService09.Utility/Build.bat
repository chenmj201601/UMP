Set BuildProject=UMPService09.Utility.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPService09.Utility.csproj /rebuild "Release|AnyCPU" /project UMPService09.Utility /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End