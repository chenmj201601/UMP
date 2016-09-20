Set BuildProject=Common000A1.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Common000A1.csproj /rebuild "Release|AnyCPU" /project Common000A1 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End