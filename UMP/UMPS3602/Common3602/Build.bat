Set BuildProject=Common36021.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Common36021.csproj /rebuild "Release|AnyCPU" /project Common36021 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End