Set BuildProject=Common5102.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Common5102.csproj /rebuild "Release|AnyCPU" /project Common5102 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End