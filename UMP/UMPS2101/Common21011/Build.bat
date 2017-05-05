Set BuildProject=Common21011.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Common21011.csproj /rebuild "Release|AnyCPU" /project Common21011 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End