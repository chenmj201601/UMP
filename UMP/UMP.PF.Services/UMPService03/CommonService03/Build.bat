Set BuildProject=CommonService03.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv CommonService03.csproj /rebuild "Release|AnyCPU" /project CommonService03 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End