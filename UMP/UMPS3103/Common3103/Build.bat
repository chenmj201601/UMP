Set BuildProject=Common3103.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Common3103.csproj /rebuild "Release|AnyCPU" /project Common3103 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End