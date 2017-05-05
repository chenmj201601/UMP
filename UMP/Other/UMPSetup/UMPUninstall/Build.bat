Set BuildProject=UMPUninstall.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPUninstall.csproj /rebuild "Release|AnyCPU" /project UMPUninstall /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End