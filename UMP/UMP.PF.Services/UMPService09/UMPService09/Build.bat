Set BuildProject=UMPService09.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPService09.csproj /rebuild "Release|AnyCPU" /project UMPService09 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End