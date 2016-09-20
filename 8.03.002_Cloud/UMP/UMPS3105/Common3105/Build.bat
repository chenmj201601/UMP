Set BuildProject=Common3105.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Common3105.csproj /rebuild "Release|AnyCPU" /project Common3105 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End