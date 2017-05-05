Set BuildProject=UMPService09.DAL.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPService09.DAL.csproj /rebuild "Release|AnyCPU" /project UMPService09.DAL /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End