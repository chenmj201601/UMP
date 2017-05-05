Set BuildProject=Common36011.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Common36011.csproj /rebuild "Release|AnyCPU" /project Common36011 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End