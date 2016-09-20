Set BuildProject=Common11101.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Common11101.csproj /rebuild "Release|AnyCPU" /project Common11101 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End