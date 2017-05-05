Set BuildProject=UMPService04.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPService04.csproj /rebuild "Release|AnyCPU" /project UMPService04 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End