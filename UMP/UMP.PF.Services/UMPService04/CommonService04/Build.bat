Set BuildProject=CommonService04.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv CommonService04.csproj /rebuild "Release|AnyCPU" /project CommonService04 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End