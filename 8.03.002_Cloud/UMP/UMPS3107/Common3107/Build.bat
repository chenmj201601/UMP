Set BuildProject=Common3107.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Common3107.csproj /rebuild "Release|AnyCPU" /project Common3107 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End