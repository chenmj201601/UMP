Set BuildProject=UMPService06.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPService06.csproj /rebuild "Release|AnyCPU" /project UMPService06 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End