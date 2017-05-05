Set BuildProject=Common4602.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv Common4602.csproj /rebuild "Release|AnyCPU" /project Common4602 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End