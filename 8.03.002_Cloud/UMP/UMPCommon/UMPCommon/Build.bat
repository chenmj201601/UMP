Set BuildProject=UMPCommon.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPCommon.csproj /rebuild "Release|AnyCPU" /project UMPCommon /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End