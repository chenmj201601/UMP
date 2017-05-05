Set BuildProject=Common1102.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Common1102.csproj /rebuild "Release|AnyCPU" /project Common1102 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End