Set BuildProject=Common11121.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Common11121.csproj /rebuild "Release|AnyCPU" /project Common11121 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End