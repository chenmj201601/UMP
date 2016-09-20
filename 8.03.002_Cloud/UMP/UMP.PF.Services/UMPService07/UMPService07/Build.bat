Set BuildProject=UMPService07.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPService07.csproj /rebuild "Release|AnyCPU" /project UMPService07 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End