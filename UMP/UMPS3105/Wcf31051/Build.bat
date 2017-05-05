Set BuildProject=Wcf31051.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Wcf31051.csproj /rebuild "Release|AnyCPU" /project Wcf31051 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End