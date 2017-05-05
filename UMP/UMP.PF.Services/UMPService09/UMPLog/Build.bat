Set BuildProject=UMPService09.Log.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPService09.Log.csproj /rebuild "Release|AnyCPU" /project UMPService09.Log /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End