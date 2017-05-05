Set BuildProject=UMPService13.csproj
@echo Build %BuildProject%
@date /t
@time /t

devenv UMPService13.csproj /rebuild "Release|AnyCPU" /project UMPService13 /out "BuildInfo.txt"
@if errorlevel 1 goto error
@Set BuildSuc=1
@goto end

:error
@Set BuildSuc=0

:end
@echo Build End