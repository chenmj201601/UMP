Set BuildProject=Wcf44101.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Wcf44101.csproj /rebuild "Release|AnyCPU" /project Wcf44101 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End