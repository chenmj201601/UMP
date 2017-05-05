Set BuildProject=UMPService05.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPService05.csproj /rebuild "Release|AnyCPU" /project UMPService05 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End